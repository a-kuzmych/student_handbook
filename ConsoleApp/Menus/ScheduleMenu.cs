using ConsoleApp.Helpers;
using Handbook;

namespace ConsoleApp.Menus;

public class ScheduleMenu : CrudMenu<ScheduleItem>
{
    private readonly HandbookManager _handbook;

    private DayOfWeek? _day;
    private string? _query;

    public ScheduleMenu(HandbookManager handbook)
    {
        _handbook = handbook;
    }

    protected override string Title => "Розклад занять і заходів";
    protected override string ExportName => "schedule";

    protected override IReadOnlyList<string> SortNames =>
        ["За днем і часом", "За назвою", "За місцем", "За типом"];

    protected override string? FilterDescription
    {
        get
        {
            var parts = new List<string>();

            if (_day.HasValue)
                parts.Add($"день — {DisplayNames.DayName(_day.Value)}");

            if (_query is not null)
                parts.Add($"текст — «{_query}»");

            return parts.Count == 0 ? null : string.Join(", ", parts);
        }
    }

    protected override IReadOnlyList<ScheduleItem> GetItems()
    {
        var items = _handbook.SearchSchedule(new ScheduleSearchParams { Day = _day, Query = _query });

        IOrderedEnumerable<ScheduleItem> sorted = SortIndex switch
        {
            1 => items.OrderBy(i => i.Title, StringComparer.CurrentCultureIgnoreCase),
            2 => items.OrderBy(i => i.Place, StringComparer.CurrentCultureIgnoreCase),
            3 => items.OrderBy(i => DisplayNames.ItemTypeName(i), StringComparer.CurrentCultureIgnoreCase),
            _ => items.OrderBy(i => i.DayOrder)
        };

        return sorted.ThenBy(i => i.DayOrder).ThenBy(i => i.StartTime).ToList();
    }

    protected override TableData BuildTable(IReadOnlyList<ScheduleItem> items)
    {
        return TableBuilder.Schedule(items);
    }

    protected override void PrintExtra(IReadOnlyList<ScheduleItem> items)
    {
        var overlaps = items.Where(i => _handbook.FindOverlapsFor(i).Any()).ToList();

        if (overlaps.Count > 0)
            ConsolePrinter.PrintOverlaps(overlaps);
    }

    protected override void ChangeFilter()
    {
        Console.WriteLine();
        Console.WriteLine("=== Фільтр розкладу ===");

        _day = ConsoleInput.ReadOptionalDay("День тижня (Enter — будь-який): ");

        var query = ConsoleInput.ReadOptionalString("Текст для пошуку (Enter — пропустити): ");
        _query = query.Length == 0 ? null : query;
    }

    protected override void ResetFilter()
    {
        _day = null;
        _query = null;
    }

    protected override void AddItem()
    {
        Console.WriteLine();
        Console.WriteLine("=== Додати запис ===");
        Console.WriteLine("1. Заняття");
        Console.WriteLine("2. Захід / тренування");
        Console.WriteLine("3. Назад");

        var command = ConsoleInput.ReadRequiredString("Оберіть команду: ");

        switch (command)
        {
            case "1":
                AddLesson();
                break;

            case "2":
                AddActivity();
                break;

            case "3":
                return;

            default:
                Console.WriteLine("Невідома команда.");
                break;
        }
    }

    protected override void EditItem(ScheduleItem item)
    {
        var fields = new List<(string Title, Action Action)>
        {
            ("Змінити назву", () => item.SetTitle(ConsoleInput.ReadRequiredString("Нова назва: "))),
            ("Змінити день і час початку", () => ChangeStart(item)),
            ("Змінити тривалість", () => ChangeDuration(item)),
            ("Змінити примітку", () => item.SetNotes(ConsoleInput.ReadOptionalString("Нова примітка: ")))
        };

        switch (item)
        {
            case Lesson lesson:
                fields.Add(("Змінити тип заняття", () => lesson.SetType(
                    ConsoleInput.ReadChoice("Оберіть тип: ", DisplayNames.LessonTypes, DisplayNames.LessonTypeName))));
                fields.Add(("Змінити викладача", () => lesson.SetTeacher(
                    EntitySelector.PickTeacher(_handbook, "Номер викладача (Enter — прибрати): "))));
                fields.Add(("Змінити аудиторію", () => lesson.SetClassroom(
                    EntitySelector.PickClassroom(_handbook, "Номер аудиторії (Enter — прибрати): "))));
                break;

            case ActivityItem activity:
                fields.Add(("Змінити місце", () => activity.SetLocation(
                    ConsoleInput.ReadOptionalString("Нове місце: "))));
                break;
        }

        FieldEditMenu.Run(() => item.Title, fields);
    }

    protected override void RemoveItem(ScheduleItem item)
    {
        _handbook.RemoveScheduleItem(item.Id);
    }

    private void AddLesson()
    {
        var subject = ConsoleInput.ReadRequiredString("Назва дисципліни: ");

        Console.WriteLine("Тип заняття:");
        var type = ConsoleInput.ReadChoice("Оберіть тип: ", DisplayNames.LessonTypes, DisplayNames.LessonTypeName);

        Console.WriteLine("День тижня:");
        var day = ConsoleInput.ReadDay("Оберіть день: ");
        var start = ConsoleInput.ReadTime("Час початку (ГГ:ХХ): ");
        var duration = ConsoleInput.ReadTimeSpanMinutes("Тривалість у хвилинах: ");
        var notes = ConsoleInput.ReadOptionalString("Примітка (Enter — пропустити): ");

        Console.WriteLine("Викладач:");
        var teacher = EntitySelector.PickTeacher(_handbook, "Номер викладача (Enter — пропустити): ");

        Console.WriteLine("Аудиторія:");
        var classroom = EntitySelector.PickClassroom(_handbook, "Номер аудиторії (Enter — пропустити): ");

        ConsoleInput.TryRun(() =>
        {
            var lesson = new Lesson(subject, type, day, start, duration, teacher, classroom, notes);

            if (!ConfirmOverlaps(lesson))
                return;

            _handbook.AddScheduleItem(lesson);
            Console.WriteLine("Заняття додано.");
        });
    }

    private void AddActivity()
    {
        var title = ConsoleInput.ReadRequiredString("Назва заходу: ");

        Console.WriteLine("День тижня:");
        var day = ConsoleInput.ReadDay("Оберіть день: ");
        var start = ConsoleInput.ReadTime("Час початку (ГГ:ХХ): ");
        var duration = ConsoleInput.ReadTimeSpanMinutes("Тривалість у хвилинах: ");
        var location = ConsoleInput.ReadOptionalString("Місце (Enter — пропустити): ");
        var notes = ConsoleInput.ReadOptionalString("Примітка (Enter — пропустити): ");

        ConsoleInput.TryRun(() =>
        {
            var activity = new ActivityItem(title, day, start, duration, location, notes);

            if (!ConfirmOverlaps(activity))
                return;

            _handbook.AddScheduleItem(activity);
            Console.WriteLine("Захід додано.");
        });
    }

    private void ChangeStart(ScheduleItem item)
    {
        var oldDay = item.Day;
        var oldStart = item.StartTime;

        var day = ConsoleInput.ReadDay("Новий день тижня: ");
        var start = ConsoleInput.ReadTime("Новий час початку (ГГ:ХХ): ");

        item.SetTime(day, start, item.Duration);

        KeepOrRevert(item, () => item.SetTime(oldDay, oldStart, item.Duration), "День і час оновлено.");
    }

    private void ChangeDuration(ScheduleItem item)
    {
        var oldDuration = item.Duration;
        var newDuration = ConsoleInput.ReadTimeSpanMinutes("Нова тривалість у хвилинах: ");

        item.SetTime(item.Day, item.StartTime, newDuration);

        KeepOrRevert(item, () => item.SetTime(item.Day, item.StartTime, oldDuration), "Тривалість оновлено.");
    }

    private void KeepOrRevert(ScheduleItem item, Action revert, string successMessage)
    {
        var overlaps = _handbook.FindOverlapsFor(item).ToList();

        if (overlaps.Count > 0)
        {
            ConsolePrinter.PrintOverlaps(overlaps);

            if (!ConsoleInput.Confirm("Залишити зміни?"))
            {
                revert();
                Console.WriteLine("Зміни скасовано.");
                return;
            }
        }

        Console.WriteLine(successMessage);
    }

    private bool ConfirmOverlaps(ScheduleItem item)
    {
        var overlaps = _handbook.FindOverlapsFor(item).ToList();

        if (overlaps.Count == 0)
            return true;

        ConsolePrinter.PrintOverlaps(overlaps);

        return ConsoleInput.Confirm("Створити запис попри накладку?");
    }
}
