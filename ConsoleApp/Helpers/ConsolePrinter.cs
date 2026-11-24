using Handbook;

namespace ConsoleApp.Helpers;

public static class ConsolePrinter
{
    public static void PrintTable(TableData table, string? notFoundMessage = null)
    {
        if (table.Rows.Count == 0)
        {
            if (notFoundMessage != null)
                Console.WriteLine(notFoundMessage);

            return;
        }

        foreach (var line in TableFormatter.Format(table))
            Console.WriteLine(line);
    }

    public static void PrintOverlaps(IEnumerable<ScheduleItem> overlaps)
    {
        var overlapsList = overlaps.ToList();

        if (overlapsList.Count == 0)
        {
            Console.WriteLine("Накладки не знайдено.");
            return;
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Увага! Знайдено накладки:");

        foreach (var overlap in overlapsList)
        {
            Console.WriteLine(
                $"{DisplayNames.DayName(overlap.Day)} " +
                $"{DisplayNames.TimeText(overlap.StartTime)}-{DisplayNames.TimeText(overlap.EndTime)}: " +
                $"{overlap.Title}");
        }

        Console.ResetColor();
    }

    public static void PrintMap(IEnumerable<Classroom> classrooms)
    {
        var buildings = classrooms
            .GroupBy(c => c.Building)
            .OrderBy(g => g.Key, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        if (buildings.Count == 0)
        {
            Console.WriteLine("Карта порожня.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("=== Карта ВНЗ ===");

        foreach (var building in buildings)
        {
            Console.WriteLine(building.Key);

            foreach (var floor in building.GroupBy(c => c.Floor).OrderBy(g => g.Key))
            {
                Console.WriteLine($"  Поверх {floor.Key}:");

                foreach (var room in floor.OrderBy(c => c.Number, StringComparer.CurrentCultureIgnoreCase))
                {
                    var description = string.IsNullOrWhiteSpace(room.Description) ? "" : $" — {room.Description}";
                    Console.WriteLine($"    • ауд. {room.Number}{description}");
                }
            }
        }
    }

    public static void PrintToday(HandbookManager handbook)
    {
        var today = DateTime.Today.DayOfWeek;

        var items = handbook
            .SearchSchedule(new ScheduleSearchParams { Day = today })
            .OrderBy(i => i.StartTime)
            .ToList();

        Console.WriteLine($"Сьогодні: {DisplayNames.DayName(today)}, {DateTime.Today:dd.MM.yyyy}");

        if (items.Count == 0)
        {
            Console.WriteLine("  Занять і заходів на сьогодні немає.");
        }
        else
        {
            foreach (var item in items)
            {
                Console.WriteLine(
                    $"  {DisplayNames.TimeText(item.StartTime)}-{DisplayNames.TimeText(item.EndTime)} " +
                    $"{item.Title} ({DisplayNames.Dash(item.Place)})");
            }
        }

        var overdue = handbook.SearchTasks(new TaskSearchParams { Status = TaskStatusFilter.Overdue }).Count();

        if (overdue > 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Прострочених справ: {overdue}");
            Console.ResetColor();
        }

        Console.WriteLine();
    }
}
