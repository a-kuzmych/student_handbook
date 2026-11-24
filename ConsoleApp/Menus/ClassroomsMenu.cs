using ConsoleApp.Helpers;
using Handbook;

namespace ConsoleApp.Menus;

public class ClassroomsMenu : CrudMenu<Classroom>
{
    private readonly HandbookManager _handbook;

    private string? _query;
    private string? _building;

    public ClassroomsMenu(HandbookManager handbook)
    {
        _handbook = handbook;
    }

    protected override string Title => "Карта ВНЗ: аудиторії";
    protected override string ExportName => "classrooms";

    protected override string DeleteQuestion =>
        "Видалити аудиторію? Вона також буде прибрана із занять і консультацій.";

    protected override IReadOnlyList<string> SortNames =>
        ["За корпусом і поверхом", "За номером аудиторії"];

    protected override string? FilterDescription
    {
        get
        {
            var parts = new List<string>();

            if (_query is not null)
                parts.Add($"текст — «{_query}»");

            if (_building is not null)
                parts.Add($"корпус — «{_building}»");

            return parts.Count == 0 ? null : string.Join(", ", parts);
        }
    }

    protected override IReadOnlyList<Classroom> GetItems()
    {
        var items = _handbook.SearchClassrooms(_query, _building);

        IOrderedEnumerable<Classroom> sorted = SortIndex switch
        {
            1 => items.OrderBy(c => c.Number, StringComparer.CurrentCultureIgnoreCase),
            _ => items.OrderBy(c => c.Building, StringComparer.CurrentCultureIgnoreCase)
                .ThenBy(c => c.Floor)
        };

        return sorted.ThenBy(c => c.Number, StringComparer.CurrentCultureIgnoreCase).ToList();
    }

    protected override TableData BuildTable(IReadOnlyList<Classroom> items)
    {
        return TableBuilder.Classrooms(items);
    }

    protected override IEnumerable<(string Title, Action Action)> ExtraCommands(IReadOnlyList<Classroom> items)
    {
        yield return ("Показати карту ВНЗ (за корпусами і поверхами)",
            () => ConsolePrinter.PrintMap(_handbook.Classrooms));
    }

    protected override void ChangeFilter()
    {
        Console.WriteLine();
        Console.WriteLine("=== Фільтр аудиторій ===");

        var query = ConsoleInput.ReadOptionalString("Номер або опис (Enter — пропустити): ");
        _query = query.Length == 0 ? null : query;

        var building = ConsoleInput.ReadOptionalString("Корпус (Enter — пропустити): ");
        _building = building.Length == 0 ? null : building;
    }

    protected override void ResetFilter()
    {
        _query = null;
        _building = null;
    }

    protected override void AddItem()
    {
        var number = ConsoleInput.ReadRequiredString("Номер аудиторії: ");
        var building = ConsoleInput.ReadRequiredString("Корпус: ");
        var floor = ConsoleInput.ReadInt("Поверх: ", 0, 30);
        var description = ConsoleInput.ReadOptionalString("Опис / як знайти (Enter — пропустити): ");

        ConsoleInput.TryRun(() =>
        {
            _handbook.AddClassroom(new Classroom(number, building, floor, description));
            Console.WriteLine("Аудиторію додано.");
        });
    }

    protected override void EditItem(Classroom classroom)
    {
        FieldEditMenu.Run(() => classroom.FullName,
        [
            ("Змінити номер", () => classroom.SetNumber(ConsoleInput.ReadRequiredString("Новий номер: "))),
            ("Змінити корпус", () => classroom.SetBuilding(ConsoleInput.ReadRequiredString("Новий корпус: "))),
            ("Змінити поверх", () => classroom.SetFloor(ConsoleInput.ReadInt("Новий поверх: ", 0, 30))),
            ("Змінити опис", () => classroom.SetDescription(ConsoleInput.ReadOptionalString("Новий опис: ")))
        ]);
    }

    protected override void RemoveItem(Classroom classroom)
    {
        _handbook.RemoveClassroom(classroom.Id);
    }
}
