using ConsoleApp.Helpers;
using Handbook;

namespace ConsoleApp.Menus;

public class TasksMenu : CrudMenu<StudentTask>
{
    private readonly HandbookManager _handbook;

    private string? _query;
    private TaskStatusFilter _status = TaskStatusFilter.All;

    public TasksMenu(HandbookManager handbook)
    {
        _handbook = handbook;
    }

    protected override string Title => "План виконання справ";
    protected override string ExportName => "tasks";

    protected override IReadOnlyList<string> SortNames =>
        ["За терміном", "За пріоритетом", "За назвою"];

    protected override string? FilterDescription
    {
        get
        {
            var parts = new List<string>();

            if (_status != TaskStatusFilter.All)
                parts.Add($"статус — {DisplayNames.TaskStatusName(_status)}");

            if (_query is not null)
                parts.Add($"текст — «{_query}»");

            return parts.Count == 0 ? null : string.Join(", ", parts);
        }
    }

    protected override IReadOnlyList<StudentTask> GetItems()
    {
        var items = _handbook.SearchTasks(new TaskSearchParams { Query = _query, Status = _status });

        IOrderedEnumerable<StudentTask> sorted = SortIndex switch
        {
            1 => items.OrderByDescending(t => t.Priority).ThenBy(t => t.DueDate ?? DateTime.MaxValue),
            2 => items.OrderBy(t => t.Title, StringComparer.CurrentCultureIgnoreCase),
            _ => items.OrderBy(t => t.DueDate ?? DateTime.MaxValue).ThenByDescending(t => t.Priority)
        };

        return sorted.ToList();
    }

    protected override TableData BuildTable(IReadOnlyList<StudentTask> items)
    {
        return TableBuilder.Tasks(items);
    }

    protected override IEnumerable<(string Title, Action Action)> ExtraCommands(IReadOnlyList<StudentTask> items)
    {
        yield return ("Позначити виконаною / невиконаною", () => WithSelected(items, ToggleCompleted));
        yield return ("Видалити всі виконані справи", RemoveCompleted);
    }

    protected override void ChangeFilter()
    {
        Console.WriteLine();
        Console.WriteLine("=== Фільтр справ ===");

        _status = ConsoleInput.ReadChoice("Оберіть статус: ", DisplayNames.TaskStatuses, DisplayNames.TaskStatusName);

        var query = ConsoleInput.ReadOptionalString("Текст для пошуку (Enter — пропустити): ");
        _query = query.Length == 0 ? null : query;
    }

    protected override void ResetFilter()
    {
        _query = null;
        _status = TaskStatusFilter.All;
    }

    protected override void AddItem()
    {
        var title = ConsoleInput.ReadRequiredString("Назва справи: ");
        var description = ConsoleInput.ReadOptionalString("Опис (Enter — пропустити): ");
        var dueDate = ConsoleInput.ReadOptionalDateTime("Термін виконання (Enter — пропустити): ");

        Console.WriteLine("Пріоритет:");
        var priority = ConsoleInput.ReadChoice("Оберіть пріоритет: ", DisplayNames.Priorities,
            DisplayNames.PriorityName);

        ConsoleInput.TryRun(() =>
        {
            _handbook.AddTask(new StudentTask(title, description, dueDate, priority));
            Console.WriteLine("Справу додано.");
        });
    }

    protected override void EditItem(StudentTask task)
    {
        FieldEditMenu.Run(() => task.Title,
        [
            ("Змінити назву", () => task.SetTitle(ConsoleInput.ReadRequiredString("Нова назва: "))),
            ("Змінити опис", () => task.SetDescription(ConsoleInput.ReadOptionalString("Новий опис: "))),
            ("Змінити термін", () => task.SetDueDate(
                ConsoleInput.ReadOptionalDateTime("Новий термін (Enter — прибрати): "))),
            ("Змінити пріоритет", () => task.SetPriority(
                ConsoleInput.ReadChoice("Оберіть пріоритет: ", DisplayNames.Priorities, DisplayNames.PriorityName))),
            ("Позначити виконаною / невиконаною", () => ToggleCompleted(task))
        ]);
    }

    protected override void RemoveItem(StudentTask task)
    {
        _handbook.RemoveTask(task.Id);
    }

    private static void ToggleCompleted(StudentTask task)
    {
        if (task.IsCompleted)
        {
            task.Reopen();
            Console.WriteLine("Справу знову позначено як невиконану.");
        }
        else
        {
            task.Complete(DateTime.Now);
            Console.WriteLine("Справу виконано.");
        }
    }

    private void RemoveCompleted()
    {
        if (!ConsoleInput.Confirm("Видалити всі виконані справи?"))
            return;

        var count = _handbook.RemoveCompletedTasks();

        Console.WriteLine($"Видалено справ: {count}");
    }
}
