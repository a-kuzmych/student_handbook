namespace Handbook;

public class ScheduleSearchParams
{
    public DayOfWeek? Day { get; init; }
    public TimeOnly? Time { get; init; }
    public string? Query { get; init; }
}

public class TeacherSearchParams
{
    public string? Query { get; init; }
    public DayOfWeek? ConsultationDay { get; init; }
    public TimeOnly? ConsultationTime { get; init; }
}

public enum TaskStatusFilter
{
    All,
    Pending,
    Completed,
    Overdue
}

public class TaskSearchParams
{
    public string? Query { get; init; }
    public TaskStatusFilter Status { get; init; } = TaskStatusFilter.All;
}
