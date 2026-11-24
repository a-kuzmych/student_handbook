namespace Handbook;

public class ActivityItem : ScheduleItem
{
    public string? Location { get; private set; }

    public override string Place => Location ?? string.Empty;

    public ActivityItem(
        string title,
        DayOfWeek day,
        TimeOnly startTime,
        TimeSpan duration,
        string? location = null,
        string? notes = null,
        Guid? id = null
    ) : base(title, day, startTime, duration, notes, id)
    {
        SetLocation(location);
    }

    public void SetLocation(string? location)
    {
        Location = string.IsNullOrWhiteSpace(location) ? null : location.Trim();
    }
}
