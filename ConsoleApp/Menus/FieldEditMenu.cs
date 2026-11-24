using ConsoleApp.Helpers;

namespace ConsoleApp.Menus;

public static class FieldEditMenu
{
    public static void Run(Func<string> title, IReadOnlyList<(string Title, Action Action)> fields)
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine($"=== Редагування: {title()} ===");

            for (var i = 0; i < fields.Count; i++)
                Console.WriteLine($"{i + 1}. {fields[i].Title}");

            Console.WriteLine($"{fields.Count + 1}. Назад");

            var index = ConsoleInput.ReadIndex("Оберіть дію: ", fields.Count + 1);

            if (index == fields.Count)
                return;

            ConsoleInput.TryRun(fields[index].Action);
        }
    }
}
