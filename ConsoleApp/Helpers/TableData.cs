namespace ConsoleApp.Helpers;

public record TableData(IReadOnlyList<string> Headers, IReadOnlyList<IReadOnlyList<string>> Rows);

public static class TableFormatter
{
    public static IReadOnlyList<string> Format(TableData table)
    {
        var lines = new List<string>();

        if (table.Rows.Count == 0)
            return lines;

        var widths = table.Headers
            .Select((header, index) =>
                Math.Max(
                    header.Length,
                    table.Rows.Max(row => row[index].Length)
                )
            )
            .ToArray();

        var separator = "+" + string.Join("+", widths.Select(width => new string('-', width + 2))) + "+";

        lines.Add(separator);
        lines.Add(BuildRow(table.Headers, widths));
        lines.Add(separator);

        foreach (var row in table.Rows)
            lines.Add(BuildRow(row, widths));

        lines.Add(separator);

        return lines;
    }

    private static string BuildRow(IReadOnlyList<string> cells, IReadOnlyList<int> widths)
    {
        var values = cells.Select((cell, index) => $" {cell.PadRight(widths[index])} ");

        return "|" + string.Join("|", values) + "|";
    }
}
