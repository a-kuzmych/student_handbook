using System.Globalization;
using Handbook;

namespace ConsoleApp.Helpers;

public static class DisplayNames
{
    public static readonly IReadOnlyList<DayOfWeek> Days =
    [
        DayOfWeek.Monday,
        DayOfWeek.Tuesday,
        DayOfWeek.Wednesday,
        DayOfWeek.Thursday,
        DayOfWeek.Friday,
        DayOfWeek.Saturday,
        DayOfWeek.Sunday
    ];

    public static readonly IReadOnlyList<LessonType> LessonTypes = Enum.GetValues<LessonType>();
    public static readonly IReadOnlyList<TaskPriority> Priorities = Enum.GetValues<TaskPriority>();

    public static readonly IReadOnlyList<TaskStatusFilter> TaskStatuses = Enum.GetValues<TaskStatusFilter>();

    public static string DayName(DayOfWeek day)
    {
        return day switch
        {
            DayOfWeek.Monday => "Понеділок",
            DayOfWeek.Tuesday => "Вівторок",
            DayOfWeek.Wednesday => "Середа",
            DayOfWeek.Thursday => "Четвер",
            DayOfWeek.Friday => "Пʼятниця",
            DayOfWeek.Saturday => "Субота",
            _ => "Неділя"
        };
    }

    public static string LessonTypeName(LessonType type)
    {
        return type switch
        {
            LessonType.Lecture => "Лекція",
            LessonType.Practice => "Практична",
            LessonType.Lab => "Лабораторна",
            _ => "Семінар"
        };
    }

    public static string ItemTypeName(ScheduleItem item)
    {
        return item switch
        {
            Lesson lesson => LessonTypeName(lesson.Type),
            ActivityItem => "Захід",
            _ => "Запис"
        };
    }

    public static string PriorityName(TaskPriority priority)
    {
        return priority switch
        {
            TaskPriority.Low => "Низький",
            TaskPriority.High => "Високий",
            _ => "Звичайний"
        };
    }

    public static string TaskStatusName(TaskStatusFilter status)
    {
        return status switch
        {
            TaskStatusFilter.Pending => "Невиконані",
            TaskStatusFilter.Completed => "Виконані",
            TaskStatusFilter.Overdue => "Прострочені",
            _ => "Всі"
        };
    }

    public static string TimeText(TimeOnly time)
    {
        return time.ToString("HH:mm", CultureInfo.InvariantCulture);
    }

    public static string Dash(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? "-" : value;
    }
}
