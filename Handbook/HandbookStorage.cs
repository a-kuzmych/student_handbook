using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Handbook;

internal sealed record ClassroomDto(Guid Id, string Number, string Building, int Floor, string Description);

internal sealed record TeacherDto(
    Guid Id,
    string Name,
    string Department,
    string? Email,
    DayOfWeek? ConsultationDay,
    TimeOnly? ConsultationStart,
    TimeOnly? ConsultationEnd,
    Guid? ConsultationRoomId);

internal sealed record ScheduleItemDto(
    string Kind,
    Guid Id,
    string Title,
    DayOfWeek Day,
    TimeOnly StartTime,
    int DurationMinutes,
    string Notes,
    LessonType? Type,
    Guid? TeacherId,
    Guid? ClassroomId,
    string? Location);

internal sealed record TaskDto(
    Guid Id,
    string Title,
    string Description,
    DateTime? DueDate,
    TaskPriority Priority,
    bool IsCompleted,
    DateTime? CompletedAt);

internal sealed record ServiceDto(
    Guid Id,
    string Name,
    string Phone,
    string? Location,
    string? WorkHours,
    string Description);

internal sealed record HandbookData(
    List<ClassroomDto> Classrooms,
    List<TeacherDto> Teachers,
    List<ScheduleItemDto> Schedule,
    List<TaskDto> Tasks,
    List<ServiceDto> Services);
public static class HandbookStorage
{
    private const string LessonKind = "Lesson";
    private const string ActivityKind = "Activity";

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { new JsonStringEnumConverter() }
    };

    public static void Save(HandbookManager handbook, string path)
    {
        var data = new HandbookData(
            handbook.Classrooms
                .Select(c => new ClassroomDto(c.Id, c.Number, c.Building, c.Floor, c.Description))
                .ToList(),
            handbook.Teachers
                .Select(t => new TeacherDto(
                    t.Id, t.Name, t.Department, t.Email,
                    t.ConsultationDay, t.ConsultationStart, t.ConsultationEnd, t.ConsultationRoom?.Id))
                .ToList(),
            handbook.Schedule.Select(ToDto).ToList(),
            handbook.Tasks
                .Select(t => new TaskDto(
                    t.Id, t.Title, t.Description, t.DueDate, t.Priority, t.IsCompleted, t.CompletedAt))
                .ToList(),
            handbook.Services
                .Select(s => new ServiceDto(s.Id, s.Name, s.Phone, s.Location, s.WorkHours, s.Description))
                .ToList()
        );

        var directory = Path.GetDirectoryName(Path.GetFullPath(path));

        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(path, JsonSerializer.Serialize(data, Options));
    }

    public static HandbookManager Load(string path)
    {
        var data = JsonSerializer.Deserialize<HandbookData>(File.ReadAllText(path), Options)
                   ?? throw new InvalidDataException("Файл даних порожній.");

        var handbook = new HandbookManager();

        foreach (var dto in data.Classrooms ?? [])
            handbook.AddClassroom(new Classroom(dto.Number, dto.Building, dto.Floor, dto.Description, dto.Id));

        foreach (var dto in data.Teachers ?? [])
        {
            var teacher = new Teacher(dto.Name, dto.Department, dto.Email, dto.Id);
            var room = dto.ConsultationRoomId is { } roomId ? handbook.GetClassroomById(roomId) : null;

            teacher.SetConsultation(dto.ConsultationDay, dto.ConsultationStart, dto.ConsultationEnd, room);
            handbook.AddTeacher(teacher);
        }

        foreach (var dto in data.Schedule ?? [])
            handbook.AddScheduleItem(FromDto(dto, handbook));

        foreach (var dto in data.Tasks ?? [])
        {
            var task = new StudentTask(dto.Title, dto.Description, dto.DueDate, dto.Priority, dto.Id);

            if (dto is { IsCompleted: true, CompletedAt: not null })
                task.Complete(dto.CompletedAt.Value);

            handbook.AddTask(task);
        }

        foreach (var dto in data.Services ?? [])
            handbook.AddService(new ServiceContact(
                dto.Name, dto.Phone, dto.Location, dto.WorkHours, dto.Description, dto.Id));

        return handbook;
    }

    private static ScheduleItemDto ToDto(ScheduleItem item)
    {
        var minutes = (int)item.Duration.TotalMinutes;

        return item switch
        {
            Lesson lesson => new ScheduleItemDto(
                LessonKind, lesson.Id, lesson.Title, lesson.Day, lesson.StartTime, minutes, lesson.Notes,
                lesson.Type, lesson.Teacher?.Id, lesson.Classroom?.Id, null),

            ActivityItem activity => new ScheduleItemDto(
                ActivityKind, activity.Id, activity.Title, activity.Day, activity.StartTime, minutes,
                activity.Notes, null, null, null, activity.Location),

            _ => throw new InvalidOperationException("Невідомий тип запису розкладу.")
        };
    }

    private static ScheduleItem FromDto(ScheduleItemDto dto, HandbookManager handbook)
    {
        var duration = TimeSpan.FromMinutes(dto.DurationMinutes);

        switch (dto.Kind)
        {
            case LessonKind:
                var teacher = dto.TeacherId is { } teacherId ? handbook.GetTeacherById(teacherId) : null;
                var classroom = dto.ClassroomId is { } classroomId ? handbook.GetClassroomById(classroomId) : null;

                return new Lesson(
                    dto.Title, dto.Type ?? LessonType.Lecture, dto.Day, dto.StartTime, duration,
                    teacher, classroom, dto.Notes, dto.Id);

            case ActivityKind:
                return new ActivityItem(
                    dto.Title, dto.Day, dto.StartTime, duration, dto.Location, dto.Notes, dto.Id);

            default:
                throw new InvalidDataException($"Невідомий тип запису розкладу: {dto.Kind}");
        }
    }
}
