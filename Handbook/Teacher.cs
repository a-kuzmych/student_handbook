using System.Text.RegularExpressions;

namespace Handbook;

public class Teacher
{
    public Guid Id { get; }
    public string Name { get; private set; } = string.Empty;
    public string Department { get; private set; } = string.Empty;
    public string? Email { get; private set; }

    public DayOfWeek? ConsultationDay { get; private set; }
    public TimeOnly? ConsultationStart { get; private set; }
    public TimeOnly? ConsultationEnd { get; private set; }
    public Classroom? ConsultationRoom { get; private set; }

    public bool HasConsultation => ConsultationDay.HasValue;

    public Teacher(string name, string? department = null, string? email = null, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        SetName(name);
        SetDepartment(department);
        SetEmail(email);
    }

    public void SetName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
    }

    public void SetDepartment(string? department)
    {
        Department = department?.Trim() ?? string.Empty;
    }

    public void SetEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            Email = null;
            return;
        }

        email = email.Trim();

        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new ArgumentException("Невірний формат електронної пошти.");

        Email = email;
    }

    public void SetConsultation(DayOfWeek? day, TimeOnly? start, TimeOnly? end, Classroom? room)
    {
        if (day is null)
        {
            ConsultationDay = null;
            ConsultationStart = null;
            ConsultationEnd = null;
            ConsultationRoom = null;
            return;
        }

        if (start is null || end is null)
            throw new ArgumentException("Для консультації потрібно вказати час початку і завершення.");

        if (end <= start)
            throw new ArgumentException("Кінець консультації має бути пізніше за початок.");

        ConsultationDay = day;
        ConsultationStart = start;
        ConsultationEnd = end;
        ConsultationRoom = room;
    }

    public void ClearConsultationRoom(Guid roomId)
    {
        if (ConsultationRoom?.Id == roomId)
            ConsultationRoom = null;
    }

    public bool IsConsultingAt(DayOfWeek? day, TimeOnly? time)
    {
        if (!HasConsultation)
            return false;

        if (day.HasValue && ConsultationDay != day)
            return false;

        if (time.HasValue && (time.Value < ConsultationStart!.Value || time.Value >= ConsultationEnd!.Value))
            return false;

        return true;
    }
}
