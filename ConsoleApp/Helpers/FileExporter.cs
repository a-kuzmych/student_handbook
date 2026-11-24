using System.Text;

namespace ConsoleApp.Helpers;

public static class FileExporter
{
    private const string ExportFolder = "exports";

    public static void Export(string title, string fileNamePrefix, TableData table)
    {
        if (table.Rows.Count == 0)
        {
            Console.WriteLine("Немає даних для збереження.");
            return;
        }

        var defaultName = $"{fileNamePrefix}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
        var name = ConsoleInput.ReadOptionalString($"Імʼя файлу (Enter — {defaultName}): ");

        if (name.Length == 0)
            name = defaultName;

        name = Path.GetFileName(name);

        if (!name.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            name += ".txt";

        var lines = new List<string>
        {
            $"{title} — {DateTime.Now:dd.MM.yyyy HH:mm}",
            ""
        };

        lines.AddRange(TableFormatter.Format(table));
        lines.Add("");
        lines.Add($"Всього записів: {table.Rows.Count}");

        try
        {
            Directory.CreateDirectory(ExportFolder);

            var path = Path.GetFullPath(Path.Combine(ExportFolder, name));

            File.WriteAllLines(path, lines, Encoding.UTF8);

            Console.WriteLine($"Збережено у файл: {path}");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException
                                       or NotSupportedException)
        {
            ConsoleInput.PrintError($"Не вдалося зберегти файл: {ex.Message}");
        }
    }
}
