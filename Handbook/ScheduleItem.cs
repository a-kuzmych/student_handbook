namespace Handbook;

public abstract class ScheduleItem
{
    public Guid Id { get; }
    public string Title { get; private set; } = string.Empty;
    public DayOfWeek Day { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeSpan Duration { get; private set; }
    public string Notes { get; private set; } = string.Empty;

    public TimeOnly EndTime => StartTime.Add(Duration);

    public int DayOrder => ((int)Day + 6) % 7;

    public abstract string Place { get; }

    protected ScheduleItem(
        string title,
        DayOfWeek day,
        TimeOnly startTime,
        TimeSpan duration,
        string? notes,
        Guid? id
    )
    {
        Id = id ?? Guid.NewGuid();

        SetTitle(title);
        SetTime(day, startTime, duration);
        SetNotes(notes);
    }

    public void SetTitle(string title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        Title = title.Trim();
    }

    public void SetTime(DayOfWeek day, TimeOnly startTime, TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
            throw new ArgumentException("Тривалість має бути більшою за нуль.");

        if (startTime.ToTimeSpan() + duration >= TimeSpan.FromDays(1))
            throw new ArgumentException("Запис не може завершуватись після півночі.");

        Day = day;
        StartTime = startTime;
        Duration = duration;
    }

    public void SetNotes(string? notes)
    {
        Notes = notes?.Trim() ?? string.Empty;
    }

    public bool OverlapsWith(ScheduleItem other)
    {
        return Id != other.Id
               && Day == other.Day
               && StartTime < other.EndTime
               && other.StartTime < EndTime;
    }

    public bool IsAt(DayOfWeek? day, TimeOnly? time)
    {
        if (day.HasValue && Day != day.Value)
            return false;

        if (time.HasValue && (time.Value < StartTime || time.Value >= EndTime))
            return false;

        return true;
    }
}
