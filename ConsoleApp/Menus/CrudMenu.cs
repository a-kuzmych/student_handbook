using ConsoleApp.Helpers;

namespace ConsoleApp.Menus;

public abstract class CrudMenu<T> where T : class
{
    protected abstract string Title { get; }
    protected abstract string ExportName { get; }
    protected abstract IReadOnlyList<string> SortNames { get; }

    protected virtual string DeleteQuestion => "Видалити запис?";
    protected virtual string? FilterDescription => null;

    protected int SortIndex { get; private set; }

    protected abstract IReadOnlyList<T> GetItems();
    protected abstract TableData BuildTable(IReadOnlyList<T> items);
    protected abstract void AddItem();
    protected abstract void EditItem(T item);
    protected abstract void RemoveItem(T item);
    protected abstract void ChangeFilter();
    protected abstract void ResetFilter();

    protected virtual void PrintExtra(IReadOnlyList<T> items)
    {
    }

    protected virtual IEnumerable<(string Title, Action Action)> ExtraCommands(IReadOnlyList<T> items)
    {
        return [];
    }

    public void Run()
    {
        while (true)
        {
            var items = GetItems();
            var table = BuildTable(items);

            Console.WriteLine();
            Console.WriteLine($"=== {Title} ===");

            if (FilterDescription is not null)
                Console.WriteLine($"Фільтр: {FilterDescription}");

            Console.WriteLine($"Сортування: {SortNames[SortIndex]}");

            ConsolePrinter.PrintTable(table, "Записи не знайдено.");

            if (items.Count > 0)
                Console.WriteLine($"Всього записів: {items.Count}");

            PrintExtra(items);

            Console.WriteLine();

            var commands = new List<(string Title, Action Action)>
            {
                ("Додати", AddItem),
                ("Редагувати", () => WithSelected(items, EditItem)),
                ("Видалити", () => WithSelected(items, RemoveWithConfirm)),
                ("Змінити сортування", ChangeSort),
                ("Встановити фільтр", ChangeFilter),
                ("Скинути фільтр і сортування", Reset),
                ("Зберегти список у файл", () => FileExporter.Export(Title, ExportName, table))
            };

            commands.AddRange(ExtraCommands(items));

            for (var i = 0; i < commands.Count; i++)
                Console.WriteLine($"{i + 1}. {commands[i].Title}");

            Console.WriteLine($"{commands.Count + 1}. Назад");

            var index = ConsoleInput.ReadIndex("Оберіть дію: ", commands.Count + 1);

            if (index == commands.Count)
                return;

            commands[index].Action();
        }
    }

    protected void WithSelected(IReadOnlyList<T> items, Action<T> action)
    {
        if (items.Count == 0)
        {
            Console.WriteLine("Список порожній.");
            return;
        }

        var index = ConsoleInput.ReadIndex("Введіть номер запису: ", items.Count);

        action(items[index]);
    }

    private void RemoveWithConfirm(T item)
    {
        if (!ConsoleInput.Confirm(DeleteQuestion))
            return;

        RemoveItem(item);
        Console.WriteLine("Запис видалено.");
    }

    private void ChangeSort()
    {
        Console.WriteLine("Варіанти сортування:");

        for (var i = 0; i < SortNames.Count; i++)
            Console.WriteLine($"{i + 1}. {SortNames[i]}");

        SortIndex = ConsoleInput.ReadIndex("Оберіть сортування: ", SortNames.Count);
    }

    private void Reset()
    {
        SortIndex = 0;
        ResetFilter();
    }
}
