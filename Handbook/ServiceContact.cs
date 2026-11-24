using System.Text.RegularExpressions;

namespace Handbook;

public class ServiceContact
{
    public Guid Id { get; }
    public string Name { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string? Location { get; private set; }
    public string? WorkHours { get; private set; }
    public string Description { get; private set; } = string.Empty;

    public ServiceContact(
        string name,
        string phone,
        string? location = null,
        string? workHours = null,
        string? description = null,
        Guid? id = null
    )
    {
        Id = id ?? Guid.NewGuid();

        SetName(name);
        SetPhone(phone);
        SetLocation(location);
        SetWorkHours(workHours);
        SetDescription(description);
    }

    public void SetName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
    }

    public void SetPhone(string phone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(phone);

        phone = phone.Trim();

        if (!Regex.IsMatch(phone, @"^\+?[\d\s\-()]{3,20}$"))
            throw new ArgumentException("Невірний формат телефону. Приклад: +380 33 200-00-01");

        Phone = phone;
    }

    public void SetLocation(string? location)
    {
        Location = string.IsNullOrWhiteSpace(location) ? null : location.Trim();
    }

    public void SetWorkHours(string? workHours)
    {
        WorkHours = string.IsNullOrWhiteSpace(workHours) ? null : workHours.Trim();
    }

    public void SetDescription(string? description)
    {
        Description = description?.Trim() ?? string.Empty;
    }
}
