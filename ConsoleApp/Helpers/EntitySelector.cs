using Handbook;

namespace ConsoleApp.Helpers;

public static class EntitySelector
{
    public static Teacher? PickTeacher(HandbookManager handbook, string message)
    {
        if (handbook.Teachers.Count == 0)
        {
            Console.WriteLine("Список викладачів порожній.");
            return null;
        }

        ConsolePrinter.PrintTable(TableBuilder.Teachers(handbook.Teachers));

        var index = ConsoleInput.ReadOptionalIndex(message, handbook.Teachers.Count);

        return index.HasValue ? handbook.Teachers[index.Value] : null;
    }

    public static Classroom? PickClassroom(HandbookManager handbook, string message)
    {
        if (handbook.Classrooms.Count == 0)
        {
            Console.WriteLine("Список аудиторій порожній.");
            return null;
        }

        ConsolePrinter.PrintTable(TableBuilder.Classrooms(handbook.Classrooms));

        var index = ConsoleInput.ReadOptionalIndex(message, handbook.Classrooms.Count);

        return index.HasValue ? handbook.Classrooms[index.Value] : null;
    }
}
