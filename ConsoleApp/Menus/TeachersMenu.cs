using ConsoleApp.Helpers;
using Handbook;

namespace ConsoleApp.Menus;

public class TeachersMenu : CrudMenu<Teacher>
{
    private readonly HandbookManager _handbook;

    private string? _query;
    private DayOfWeek? _day;
    private TimeOnly? _time;

    public TeachersMenu(HandbookManager handbook)
    {
        _handbook = handbook;
    }

    protected override string Title => "Викладачі";
    protected override string ExportName => "teachers";

    protected override string DeleteQuestion =>
        "Видалити викладача? Він також буде прибраний із занять.";

    protected override IReadOnlyList<string> SortNames =>
        ["За ПІБ", "За кафедрою", "За днем консультації"];

    protected override string? FilterDescription
    {
        get
        {
            var parts = new List<string>();

            if (_query is not null)
                parts.Add($"текст — «{_query}»");

            if (_day.HasValue)
                parts.Add($"консультація в день — {DisplayNames.DayName(_day.Value)}");

            if (_time.HasValue)
                parts.Add($"консультація о {DisplayNames.TimeText(_time.Value)}");

            return parts.Count == 0 ? null : string.Join(", ", parts);
        }
    }

    protected override IReadOnlyList<Teacher> GetItems()
    {
        var items = _handbook.SearchTeachers(new TeacherSearchParams
        {
            Query = _query,
            ConsultationDay = _day,
            ConsultationTime = _time
        });

        IOrderedEnumerable<Teacher> sorted = SortIndex switch
        {
            1 => items.OrderBy(t => t.Department, StringComparer.CurrentCultureIgnoreCase),
            2 => items.OrderBy(t => t.ConsultationDay.HasValue ? ((int)t.ConsultationDay.Value + 6) % 7 : 7)
                .ThenBy(t => t.ConsultationStart),
            _ => items.OrderBy(t => t.Name, StringComparer.CurrentCultureIgnoreCase)
        };

        return sorted.ThenBy(t => t.Name, StringComparer.CurrentCultureIgnoreCase).ToList();
    }

    protected override TableData BuildTable(IReadOnlyList<Teacher> items)
    {
        return TableBuilder.Teachers(items);
    }

    protected override void ChangeFilter()
    {
        Console.WriteLine();
        Console.WriteLine("=== Фільтр викладачів ===");

        var query = ConsoleInput.ReadOptionalString("ПІБ або кафедра (Enter — пропустити): ");
        _query = query.Length == 0 ? null : query;

        _day = ConsoleInput.ReadOptionalDay("День консультації (Enter — будь-який): ");
        _time = ConsoleInput.ReadOptionalTime("Час консультації (ГГ:ХХ, Enter — будь-який): ");
    }

    protected override void ResetFilter()
    {
        _query = null;
        _day = null;
        _time = null;
    }

    protected override void AddItem()
    {
        var name = ConsoleInput.ReadRequiredString("ПІБ викладача: ");
        var department = ConsoleInput.ReadOptionalString("Кафедра (Enter — пропустити): ");
        var email = ConsoleInput.ReadOptionalString("Електронна пошта (Enter — пропустити): ");

        ConsoleInput.TryRun(() =>
        {
            var teacher = new Teacher(name, department, email);

            Console.WriteLine("Консультації:");
            ChangeConsultation(teacher);

            _handbook.AddTeacher(teacher);
            Console.WriteLine("Викладача додано.");
        });
    }

    protected override void EditItem(Teacher teacher)
    {
        FieldEditMenu.Run(() => teacher.Name,
        [
            ("Змінити ПІБ", () => teacher.SetName(ConsoleInput.ReadRequiredString("Нове ПІБ: "))),
            ("Змінити кафедру", () => teacher.SetDepartment(ConsoleInput.ReadOptionalString("Нова кафедра: "))),
            ("Змінити електронну пошту", () => teacher.SetEmail(
                ConsoleInput.ReadOptionalString("Нова електронна пошта (Enter — прибрати): "))),
            ("Змінити консультації", () => ChangeConsultation(teacher))
        ]);
    }

    protected override void RemoveItem(Teacher teacher)
    {
        _handbook.RemoveTeacher(teacher.Id);
    }

    private void ChangeConsultation(Teacher teacher)
    {
        var day = ConsoleInput.ReadOptionalDay("День консультації (Enter — без консультацій): ");

        if (day is null)
        {
            teacher.SetConsultation(null, null, null, null);
            return;
        }

        var start = ConsoleInput.ReadTime("Початок (ГГ:ХХ): ");
        var end = ConsoleInput.ReadTime("Кінець (ГГ:ХХ): ");

        while (end <= start)
        {
            ConsoleInput.PrintError("Кінець консультації має бути пізніше за початок.");
            end = ConsoleInput.ReadTime("Кінець (ГГ:ХХ): ");
        }

        var room = EntitySelector.PickClassroom(_handbook, "Номер аудиторії (Enter — не вказувати): ");

        teacher.SetConsultation(day, start, end, room);
    }
}
