using System.Globalization;

namespace ConsoleApp.Helpers;

public static class ConsoleInput
{
    public static string ReadRequiredString(string message)
    {
        while (true)
        {
            Console.Write(message);

            var value = ReadLine();

            if (!string.IsNullOrWhiteSpace(value))
                return value.Trim();

            PrintError("Значення не може бути порожнім.");
        }
    }

    public static string ReadOptionalString(string message)
    {
        Console.Write(message);

        var value = ReadLine();

        return string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.Trim();
    }

    public static DateTime? ReadOptionalDateTime(string message)
    {
        while (true)
        {
            Console.Write(message);

            var value = ReadLine();

            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (DateTime.TryParse(value, out var dateTime))
                return dateTime;

            PrintError("Невірний формат дати. Приклад: 10.05.2026 14:30");
        }
    }

    public static TimeOnly ReadTime(string message)
    {
        while (true)
        {
            var time = ReadOptionalTime(message, false);

            if (time.HasValue)
                return time.Value;
        }
    }

    public static TimeOnly? ReadOptionalTime(string message, bool allowEmpty = true)
    {
        while (true)
        {
            Console.Write(message);

            var value = ReadLine();

            if (string.IsNullOrWhiteSpace(value))
            {
                if (allowEmpty)
                    return null;

                PrintError("Значення не може бути порожнім.");
                continue;
            }

            if (TimeOnly.TryParseExact(
                    value.Trim(),
                    new[] { "H:mm", "HH:mm" },
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var time))
                return time;

            PrintError("Невірний формат часу. Приклад: 14:30");
        }
    }

    public static TimeSpan ReadTimeSpanMinutes(string message)
    {
        while (true)
        {
            Console.Write(message);

            var value = ReadLine();

            if (int.TryParse(value, out var minutes) && minutes > 0)
                return TimeSpan.FromMinutes(minutes);

            PrintError("Введіть додатне число хвилин.");
        }
    }

    public static int ReadInt(string message, int min, int max)
    {
        while (true)
        {
            Console.Write(message);

            var value = ReadLine();

            if (!int.TryParse(value, out var number))
            {
                PrintError("Введіть число.");
                continue;
            }

            if (number < min || number > max)
            {
                PrintError($"Введіть число від {min} до {max}.");
                continue;
            }

            return number;
        }
    }

    public static int ReadIndex(string message, int count)
    {
        return ReadInt(message, 1, count) - 1;
    }

    public static int? ReadOptionalIndex(string message, int count)
    {
        while (true)
        {
            Console.Write(message);

            var value = ReadLine();

            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (!int.TryParse(value, out var index))
            {
                PrintError("Введіть число.");
                continue;
            }

            if (index < 1 || index > count)
            {
                PrintError($"Введіть число від 1 до {count}.");
                continue;
            }

            return index - 1;
        }
    }

    public static T ReadChoice<T>(string message, IReadOnlyList<T> options, Func<T, string> label)
    {
        PrintOptions(options, label);

        return options[ReadIndex(message, options.Count)];
    }

    public static DayOfWeek ReadDay(string message)
    {
        return ReadChoice(message, DisplayNames.Days, DisplayNames.DayName);
    }

    public static DayOfWeek? ReadOptionalDay(string message)
    {
        PrintOptions(DisplayNames.Days, DisplayNames.DayName);

        var index = ReadOptionalIndex(message, DisplayNames.Days.Count);

        return index.HasValue ? DisplayNames.Days[index.Value] : null;
    }

    public static bool Confirm(string message)
    {
        while (true)
        {
            Console.Write($"{message} (y/n): ");

            var value = ReadLine()?.Trim().ToLower();

            switch (value)
            {
                case "y":
                case "yes":
                case "т":
                case "так":
                    return true;

                case "n":
                case "no":
                case "н":
                case "ні":
                    return false;

                default:
                    PrintError("Введіть y/n.");
                    break;
            }
        }
    }

    public static bool TryRun(Action action)
    {
        try
        {
            action();
            return true;
        }
        catch (ArgumentException ex)
        {
            PrintError(ex.Message);
            return false;
        }
    }

    public static void PrintError(string message)
    {
        var oldColor = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ForegroundColor = oldColor;
    }

    // Якщо введення закрито (Ctrl+Z / Ctrl+D), програма завершується, а не зациклюється
    private static string? ReadLine()
    {
        var value = Console.ReadLine();

        if (value is null)
            Environment.Exit(0);

        return value;
    }

    private static void PrintOptions<T>(IReadOnlyList<T> options, Func<T, string> label)
    {
        for (var i = 0; i < options.Count; i++)
            Console.WriteLine($"{i + 1}. {label(options[i])}");
    }
}
