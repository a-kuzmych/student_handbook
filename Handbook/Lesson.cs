namespace Handbook;

public class Lesson : ScheduleItem
{
    public LessonType Type { get; private set; }
    public Teacher? Teacher { get; private set; }
    public Classroom? Classroom { get; private set; }

    public override string Place => Classroom?.FullName ?? string.Empty;

    public Lesson(
        string subject,
        LessonType type,
        DayOfWeek day,
        TimeOnly startTime,
        TimeSpan duration,
        Teacher? teacher = null,
        Classroom? classroom = null,
        string? notes = null,
        Guid? id = null
    ) : base(subject, day, startTime, duration, notes, id)
    {
        SetType(type);
        SetTeacher(teacher);
        SetClassroom(classroom);
    }

    public void SetType(LessonType type)
    {
        Type = type;
    }

    public void SetTeacher(Teacher? teacher)
    {
        Teacher = teacher;
    }

    public void SetClassroom(Classroom? classroom)
    {
        Classroom = classroom;
    }
}
