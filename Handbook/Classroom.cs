namespace Handbook;

public class Classroom
{
    public Guid Id { get; }
    public string Number { get; private set; } = string.Empty;
    public string Building { get; private set; } = string.Empty;
    public int Floor { get; private set; }
    public string Description { get; private set; } = string.Empty;

    public string FullName => $"{Building}, ауд. {Number}";

    public Classroom(string number, string building, int floor, string? description = null, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        SetNumber(number);
        SetBuilding(building);
        SetFloor(floor);
        SetDescription(description);
    }

    public void SetNumber(string number)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(number);
        Number = number.Trim();
    }

    public void SetBuilding(string building)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(building);
        Building = building.Trim();
    }

    public void SetFloor(int floor)
    {
        if (floor is < 0 or > 30)
            throw new ArgumentException("Поверх має бути в межах від 0 до 30.");

        Floor = floor;
    }

    public void SetDescription(string? description)
    {
        Description = description?.Trim() ?? string.Empty;
    }
}
