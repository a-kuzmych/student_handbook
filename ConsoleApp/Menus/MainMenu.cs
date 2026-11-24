using ConsoleApp.Helpers;
using Handbook;

namespace ConsoleApp.Menus;

public class MainMenu
{
    private readonly HandbookManager _handbook;
    private readonly string _storagePath;

    public MainMenu(HandbookManager handbook, string storagePath)
    {
        _handbook = handbook;
        _storagePath = storagePath;
    }

    public void Run()
    {
        ConsolePrinter.PrintToday(_handbook);

        while (true)
        {
            Console.WriteLine("=== Довідник студента ===");
            Console.WriteLine("1. Розклад занять і заходів");
            Console.WriteLine("2. План виконання справ");
            Console.WriteLine("3. Викладачі");
            Console.WriteLine("4. Карта ВНЗ (аудиторії)");
            Console.WriteLine("5. Телефони служб");
            Console.WriteLine("6. Пошук");
            Console.WriteLine("7. Зберегти дані");
            Console.WriteLine("8. Завершити роботу");

            var command = ConsoleInput.ReadRequiredString("Оберіть команду: ");

            switch (command)
            {
                case "1":
                    new ScheduleMenu(_handbook).Run();
                    break;

                case "2":
                    new TasksMenu(_handbook).Run();
                    break;

                case "3":
                    new TeachersMenu(_handbook).Run();
                    break;

                case "4":
                    new ClassroomsMenu(_handbook).Run();
                    break;

                case "5":
                    new ServicesMenu(_handbook).Run();
                    break;

                case "6":
                    new SearchMenu(_handbook).Run();
                    break;

                case "7":
                    Save();
                    break;

                case "8":
                    Save();
                    return;

                default:
                    Console.WriteLine("Невідома команда.");
                    break;
            }

            Console.WriteLine();
        }
    }

    private void Save()
    {
        try
        {
            HandbookStorage.Save(_handbook, _storagePath);
            Console.WriteLine($"Дані збережено: {_storagePath}");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            ConsoleInput.PrintError($"Не вдалося зберегти дані: {ex.Message}");
        }
    }
}
