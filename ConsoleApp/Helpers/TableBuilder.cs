using Handbook;

namespace ConsoleApp.Helpers;

public static class TableBuilder
{
    public static TableData Schedule(IEnumerable<ScheduleItem> items)
    {
        var rows = items.Select((item, index) => new[]
        {
            (index + 1).ToString(),
            DisplayNames.DayName(item.Day),
            $"{DisplayNames.TimeText(item.StartTime)}-{DisplayNames.TimeText(item.EndTime)}",
            DisplayNames.ItemTypeName(item),
            item.Title,
            DisplayNames.Dash((item as Lesson)?.Teacher?.Name),
            DisplayNames.Dash(item.Place),
            DisplayNames.Dash(item.Notes)
        }).ToList<IReadOnlyList<string>>();

        return new TableData(
            ["№", "День", "Час", "Тип", "Назва", "Викладач", "Місце", "Примітка"],
            rows
        );
    }

    public static TableData Tasks(IEnumerable<StudentTask> tasks)
    {
        var now = DateTime.Now;

        var rows = tasks.Select((task, index) => new[]
        {
            (index + 1).ToString(),
            task.Title,
            DisplayNames.Dash(task.Description),
            task.DueDate.HasValue ? $"{task.DueDate.Value:dd.MM.yyyy HH:mm}" : "-",
            DisplayNames.PriorityName(task.Priority),
            task.IsCompleted ? "Виконано" : task.IsOverdue(now) ? "Прострочено" : "Не виконано"
        }).ToList<IReadOnlyList<string>>();

        return new TableData(
            ["№", "Назва", "Опис", "Термін", "Пріоритет", "Статус"],
            rows
        );
    }

    public static TableData Teachers(IEnumerable<Teacher> teachers)
    {
        var rows = teachers.Select((teacher, index) => new[]
        {
            (index + 1).ToString(),
            teacher.Name,
            DisplayNames.Dash(teacher.Department),
            DisplayNames.Dash(teacher.Email),
            ConsultationText(teacher)
        }).ToList<IReadOnlyList<string>>();

        return new TableData(
            ["№", "ПІБ", "Кафедра", "Електронна пошта", "Консультації"],
            rows
        );
    }

    public static TableData Classrooms(IEnumerable<Classroom> classrooms)
    {
        var rows = classrooms.Select((classroom, index) => new[]
        {
            (index + 1).ToString(),
            classroom.Building,
            classroom.Floor.ToString(),
            classroom.Number,
            DisplayNames.Dash(classroom.Description)
        }).ToList<IReadOnlyList<string>>();

        return new TableData(
            ["№", "Корпус", "Поверх", "Аудиторія", "Опис"],
            rows
        );
    }

    public static TableData Services(IEnumerable<ServiceContact> services)
    {
        var rows = services.Select((service, index) => new[]
        {
            (index + 1).ToString(),
            service.Name,
            service.Phone,
            DisplayNames.Dash(service.Location),
            DisplayNames.Dash(service.WorkHours),
            DisplayNames.Dash(service.Description)
        }).ToList<IReadOnlyList<string>>();

        return new TableData(
            ["№", "Служба", "Телефон", "Розташування", "Години роботи", "Опис"],
            rows
        );
    }

    private static string ConsultationText(Teacher teacher)
    {
        if (!teacher.HasConsultation)
            return "-";

        var text = $"{DisplayNames.DayName(teacher.ConsultationDay!.Value)} " +
                   $"{DisplayNames.TimeText(teacher.ConsultationStart!.Value)}-" +
                   $"{DisplayNames.TimeText(teacher.ConsultationEnd!.Value)}";

        return teacher.ConsultationRoom is null
            ? text
            : $"{text}, {teacher.ConsultationRoom.FullName}";
    }
}
