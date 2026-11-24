using ConsoleApp.Helpers;
using Handbook;

namespace ConsoleApp.Menus;

public class SearchMenu
{
    private readonly HandbookManager _handbook;

    public SearchMenu(HandbookManager handbook)
    {
        _handbook = handbook;
    }

    public void Run()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=== Пошук ===");
            Console.WriteLine("1. Заняття та заходи (за днем, часом, дисципліною, викладачем, аудиторією)");
            Console.WriteLine("2. Викладач для консультації (за днем і часом)");
            Console.WriteLine("3. Аудиторії");
            Console.WriteLine("4. Служби і телефони");
            Console.WriteLine("5. Назад");

            var command = ConsoleInput.ReadRequiredString("Оберіть команду: ");

            switch (command)
            {
                case "1":
                    SearchSchedule();
                    break;

                case "2":
                    SearchConsultations();
                    break;

                case "3":
                    SearchClassrooms();
                    break;

                case "4":
                    SearchServices();
                    break;

                case "5":
                    return;

                default:
                    Console.WriteLine("Невідома команда.");
                    break;
            }
        }
    }

    private void SearchSchedule()
    {
        Console.WriteLine();
        Console.WriteLine("=== Пошук у розкладі ===");

        var day = ConsoleInput.ReadOptionalDay("День тижня (Enter — будь-який): ");
        var time = ConsoleInput.ReadOptionalTime("Час (ГГ:ХХ, Enter — будь-який): ");
        var query = ConsoleInput.ReadOptionalString("Дисципліна, викладач або аудиторія (Enter — пропустити): ");

        var results = _handbook
            .SearchSchedule(new ScheduleSearchParams { Day = day, Time = time, Query = query })
            .OrderBy(i => i.DayOrder)
            .ThenBy(i => i.StartTime)
            .ToList();

        ShowResults("Результати пошуку в розкладі", "schedule_search", TableBuilder.Schedule(results));
    }

    private void SearchConsultations()
    {
        Console.WriteLine();
        Console.WriteLine("=== Пошук викладача для консультації ===");

        var query = ConsoleInput.ReadOptionalString("ПІБ або кафедра (Enter — пропустити): ");
        var day = ConsoleInput.ReadOptionalDay("День тижня (Enter — будь-який): ");
        var time = ConsoleInput.ReadOptionalTime("Час (ГГ:ХХ, Enter — будь-який): ");

        var results = _handbook
            .SearchTeachers(new TeacherSearchParams
            {
                Query = query,
                ConsultationDay = day,
                ConsultationTime = time
            })
            .OrderBy(t => t.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        ShowResults("Результати пошуку викладачів", "teachers_search", TableBuilder.Teachers(results));
    }

    private void SearchClassrooms()
    {
        Console.WriteLine();
        Console.WriteLine("=== Пошук аудиторій ===");

        var query = ConsoleInput.ReadOptionalString("Номер або опис (Enter — пропустити): ");
        var building = ConsoleInput.ReadOptionalString("Корпус (Enter — пропустити): ");

        var results = _handbook
            .SearchClassrooms(query, building)
            .OrderBy(c => c.Building, StringComparer.CurrentCultureIgnoreCase)
            .ThenBy(c => c.Floor)
            .ToList();

        ShowResults("Результати пошуку аудиторій", "classrooms_search", TableBuilder.Classrooms(results));
    }

    private void SearchServices()
    {
        Console.WriteLine();
        Console.WriteLine("=== Пошук служб ===");

        var query = ConsoleInput.ReadOptionalString("Назва, опис, розташування або телефон: ");

        var results = _handbook
            .SearchServices(query)
            .OrderBy(s => s.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        ShowResults("Результати пошуку служб", "services_search", TableBuilder.Services(results));
    }

    private static void ShowResults(string title, string fileNamePrefix, TableData table)
    {
        ConsolePrinter.PrintTable(table, "Нічого не знайдено.");

        if (table.Rows.Count == 0)
            return;

        Console.WriteLine($"Знайдено записів: {table.Rows.Count}");

        if (ConsoleInput.Confirm("Зберегти результат у файл?"))
            FileExporter.Export(title, fileNamePrefix, table);
    }
}
