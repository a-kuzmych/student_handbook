using Handbook;

namespace ConsoleApp;

public static class DemoData
{
    public static HandbookManager Create()
    {
        var handbook = new HandbookManager();

        // Карта ВНЗ

        var room101 = new Classroom("101", "Корпус 1", 1, "Лекційна аудиторія на 120 місць, ліворуч від гардеробу");
        var room204 = new Classroom("204", "Корпус 1", 2, "Комп'ютерний клас, 20 місць");
        var room310 = new Classroom("310", "Корпус 1", 3, "Аудиторія для практичних занять");
        var room12 = new Classroom("12", "Корпус 2", 1, "Лабораторія фізики");
        var room215 = new Classroom("215", "Корпус 2", 2, "Лекційна аудиторія, вхід з боку подвір'я");
        var gym = new Classroom("1", "Спортивний комплекс", 1, "Великий спортивний зал");

        foreach (var room in new[] { room101, room204, room310, room12, room215, gym })
            handbook.AddClassroom(room);

        // Викладачі

        var kovalenko = new Teacher("Коваленко Ірина Петрівна", "Кафедра програмної інженерії",
            "kovalenko@example.com");
        kovalenko.SetConsultation(DayOfWeek.Wednesday, new TimeOnly(14, 0), new TimeOnly(15, 30), room310);

        var melnyk = new Teacher("Мельник Олег Васильович", "Кафедра вищої математики", "melnyk@example.com");
        melnyk.SetConsultation(DayOfWeek.Tuesday, new TimeOnly(13, 30), new TimeOnly(14, 30), room215);

        var shevchenko = new Teacher("Шевченко Андрій Миколайович", "Кафедра фізики", "shevchenko@example.com");
        shevchenko.SetConsultation(DayOfWeek.Thursday, new TimeOnly(15, 0), new TimeOnly(16, 0), room12);

        var bondar = new Teacher("Бондар Наталія Ігорівна", "Кафедра іноземних мов", "bondar@example.com");
        bondar.SetConsultation(DayOfWeek.Friday, new TimeOnly(12, 0), new TimeOnly(13, 0), room310);

        foreach (var teacher in new[] { kovalenko, melnyk, shevchenko, bondar })
            handbook.AddTeacher(teacher);

        // Розклад

        var pair = TimeSpan.FromMinutes(80);

        handbook.AddScheduleItem(new Lesson("Програмування на C#", LessonType.Lecture, DayOfWeek.Monday,
            new TimeOnly(8, 30), pair, kovalenko, room101));

        handbook.AddScheduleItem(new Lesson("Вища математика", LessonType.Practice, DayOfWeek.Monday,
            new TimeOnly(10, 10), pair, melnyk, room215));

        handbook.AddScheduleItem(new Lesson("Фізика", LessonType.Lecture, DayOfWeek.Tuesday,
            new TimeOnly(8, 30), pair, shevchenko, room215));

        handbook.AddScheduleItem(new Lesson("Програмування на C#", LessonType.Lab, DayOfWeek.Tuesday,
            new TimeOnly(10, 10), pair, kovalenko, room204, "Взяти ноутбук"));

        handbook.AddScheduleItem(new Lesson("Англійська мова", LessonType.Practice, DayOfWeek.Wednesday,
            new TimeOnly(10, 10), pair, bondar, room310));

        handbook.AddScheduleItem(new Lesson("Вища математика", LessonType.Lecture, DayOfWeek.Wednesday,
            new TimeOnly(11, 50), pair, melnyk, room101));

        handbook.AddScheduleItem(new Lesson("Фізика", LessonType.Lab, DayOfWeek.Thursday,
            new TimeOnly(8, 30), pair, shevchenko, room12));

        handbook.AddScheduleItem(new ActivityItem("Волейбол", DayOfWeek.Tuesday,
            new TimeOnly(17, 0), TimeSpan.FromMinutes(90), "Спортивний комплекс, зал 1", "Спортивна форма"));

        handbook.AddScheduleItem(new ActivityItem("Гурток з програмування", DayOfWeek.Friday,
            new TimeOnly(16, 0), TimeSpan.FromMinutes(60), "Корпус 1, ауд. 204"));

        // Справи

        handbook.AddTask(new StudentTask("Здати лабораторну з програмування", "Лабораторна №3",
            DateTime.Today.AddDays(2).AddHours(23), TaskPriority.High));

        handbook.AddTask(new StudentTask("Підготувати реферат з англійської", "Тема: Technology in education",
            DateTime.Today.AddDays(5).AddHours(18)));

        handbook.AddTask(new StudentTask("Купити підручник з математики", null, null, TaskPriority.Low));

        handbook.AddTask(new StudentTask("Оплатити гуртожиток", null,
            DateTime.Today.AddDays(-1).AddHours(18), TaskPriority.High));

        var done = new StudentTask("Здати звіт з фізики", "Лабораторна №2",
            DateTime.Today.AddDays(-2).AddHours(12));
        done.Complete(DateTime.Today.AddDays(-3));
        handbook.AddTask(done);

        // Телефони служб (приклади)

        handbook.AddService(new ServiceContact("Деканат", "+380 33 200-00-01", "Корпус 1, ауд. 105",
            "Пн–Пт 09:00–17:00", "Довідки, заяви, розклад сесії"));

        handbook.AddService(new ServiceContact("Бібліотека", "+380 33 200-00-02", "Корпус 2, 1 поверх",
            "Пн–Пт 09:00–18:00", "Видача підручників, читальний зал"));

        handbook.AddService(new ServiceContact("Студентська поліклініка", "+380 33 200-00-03", "Корпус 2, 2 поверх",
            "Пн–Пт 08:30–15:00", "Медичні довідки"));

        handbook.AddService(new ServiceContact("Гуртожиток (комендант)", "+380 33 200-00-04", "Гуртожиток №1",
            "Щодня 08:00–20:00", "Поселення, оплата, ремонт"));

        handbook.AddService(new ServiceContact("Служба охорони", "+380 33 200-00-05", "Головний вхід",
            "Цілодобово", "Екстрені ситуації на території ВНЗ"));

        handbook.AddService(new ServiceContact("IT-підтримка", "+380 33 200-00-06", "Корпус 1, ауд. 210",
            "Пн–Пт 09:00–16:00", "Wi-Fi, облікові записи, електронний кабінет"));

        return handbook;
    }
}
