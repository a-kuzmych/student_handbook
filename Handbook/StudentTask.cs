namespace Handbook;

public class StudentTask
{
    public Guid Id { get; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime? DueDate { get; private set; }
    public TaskPriority Priority { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public StudentTask(
        string title,
        string? description = null,
        DateTime? dueDate = null,
        TaskPriority priority = TaskPriority.Normal,
        Guid? id = null
    )
    {
        Id = id ?? Guid.NewGuid();

        SetTitle(title);
        SetDescription(description);
        SetDueDate(dueDate);
        SetPriority(priority);
    }

    public void SetTitle(string title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        Title = title.Trim();
    }

    public void SetDescription(string? description)
    {
        Description = description?.Trim() ?? string.Empty;
    }

    public void SetDueDate(DateTime? dueDate)
    {
        DueDate = dueDate;
    }

    public void SetPriority(TaskPriority priority)
    {
        Priority = priority;
    }

    public void Complete(DateTime completedAt)
    {
        IsCompleted = true;
        CompletedAt = completedAt;
    }

    public void Reopen()
    {
        IsCompleted = false;
        CompletedAt = null;
    }

    public bool IsOverdue(DateTime now)
    {
        return !IsCompleted && DueDate.HasValue && DueDate.Value < now;
    }
}
