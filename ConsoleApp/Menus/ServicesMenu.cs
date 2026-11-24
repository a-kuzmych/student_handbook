using ConsoleApp.Helpers;
using Handbook;

namespace ConsoleApp.Menus;

public class ServicesMenu : CrudMenu<ServiceContact>
{
    private readonly HandbookManager _handbook;

    private string? _query;

    public ServicesMenu(HandbookManager handbook)
    {
        _handbook = handbook;
    }

    protected override string Title => "Телефони служб";
    protected override string ExportName => "services";

    protected override IReadOnlyList<string> SortNames => ["За назвою", "За телефоном"];

    protected override string? FilterDescription => _query is null ? null : $"текст — «{_query}»";

    protected override IReadOnlyList<ServiceContact> GetItems()
    {
        var items = _handbook.SearchServices(_query);

        return (SortIndex == 1
                ? items.OrderBy(s => s.Phone, StringComparer.CurrentCultureIgnoreCase)
                : items.OrderBy(s => s.Name, StringComparer.CurrentCultureIgnoreCase))
            .ToList();
    }

    protected override TableData BuildTable(IReadOnlyList<ServiceContact> items)
    {
        return TableBuilder.Services(items);
    }

    protected override void ChangeFilter()
    {
        Console.WriteLine();
        Console.WriteLine("=== Фільтр служб ===");

        var query = ConsoleInput.ReadOptionalString("Назва, опис, розташування або телефон: ");
        _query = query.Length == 0 ? null : query;
    }

    protected override void ResetFilter()
    {
        _query = null;
    }

    protected override void AddItem()
    {
        var name = ConsoleInput.ReadRequiredString("Назва служби: ");
        var phone = ConsoleInput.ReadRequiredString("Телефон: ");
        var location = ConsoleInput.ReadOptionalString("Розташування (Enter — пропустити): ");
        var workHours = ConsoleInput.ReadOptionalString("Години роботи (Enter — пропустити): ");
        var description = ConsoleInput.ReadOptionalString("Опис (Enter — пропустити): ");

        ConsoleInput.TryRun(() =>
        {
            _handbook.AddService(new ServiceContact(name, phone, location, workHours, description));
            Console.WriteLine("Службу додано.");
        });
    }

    protected override void EditItem(ServiceContact service)
    {
        FieldEditMenu.Run(() => service.Name,
        [
            ("Змінити назву", () => service.SetName(ConsoleInput.ReadRequiredString("Нова назва: "))),
            ("Змінити телефон", () => service.SetPhone(ConsoleInput.ReadRequiredString("Новий телефон: "))),
            ("Змінити розташування", () => service.SetLocation(ConsoleInput.ReadOptionalString("Нове розташування: "))),
            ("Змінити години роботи", () => service.SetWorkHours(ConsoleInput.ReadOptionalString("Нові години роботи: "))),
            ("Змінити опис", () => service.SetDescription(ConsoleInput.ReadOptionalString("Новий опис: ")))
        ]);
    }

    protected override void RemoveItem(ServiceContact service)
    {
        _handbook.RemoveService(service.Id);
    }
}
