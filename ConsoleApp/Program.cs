using System.Text;
using ConsoleApp;
using ConsoleApp.Menus;
using Handbook;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var storagePath = Path.GetFullPath("handbook.json");

HandbookManager handbook;

try
{
    handbook = File.Exists(storagePath)
        ? HandbookStorage.Load(storagePath)
        : DemoData.Create();
}
catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException
                               or System.Text.Json.JsonException or ArgumentException)
{
    Console.WriteLine($"Не вдалося прочитати файл даних {storagePath}: {ex.Message}");
    Console.WriteLine("Виправте або видаліть файл і запустіть програму знову.");
    return;
}

var menu = new MainMenu(handbook, storagePath);
menu.Run();
