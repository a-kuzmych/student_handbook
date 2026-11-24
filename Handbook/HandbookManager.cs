namespace Handbook;

public class HandbookManager
{
    private readonly List<Classroom> _classrooms = [];
    private readonly List<Teacher> _teachers = [];
    private readonly List<ScheduleItem> _schedule = [];
    private readonly List<StudentTask> _tasks = [];
    private readonly List<ServiceContact> _services = [];

    public IReadOnlyList<Classroom> Classrooms => _classrooms;
    public IReadOnlyList<Teacher> Teachers => _teachers;
    public IReadOnlyList<ScheduleItem> Schedule => _schedule;
    public IReadOnlyList<StudentTask> Tasks => _tasks;
    public IReadOnlyList<ServiceContact> Services => _services;

    // Аудиторії

    public void AddClassroom(Classroom classroom)
    {
        _classrooms.Add(classroom);
    }

    public Classroom? GetClassroomById(Guid id)
    {
        return _classrooms.FirstOrDefault(c => c.Id == id);
    }

    public void RemoveClassroom(Guid id)
    {
        if (_classrooms.RemoveAll(c => c.Id == id) == 0)
            return;

        foreach (var teacher in _teachers)
            teacher.ClearConsultationRoom(id);

        foreach (var lesson in _schedule.OfType<Lesson>().Where(l => l.Classroom?.Id == id))
            lesson.SetClassroom(null);
    }

    // Викладачі

    public void AddTeacher(Teacher teacher)
    {
        _teachers.Add(teacher);
    }

    public Teacher? GetTeacherById(Guid id)
    {
        return _teachers.FirstOrDefault(t => t.Id == id);
    }

    public void RemoveTeacher(Guid id)
    {
        if (_teachers.RemoveAll(t => t.Id == id) == 0)
            return;

        foreach (var lesson in _schedule.OfType<Lesson>().Where(l => l.Teacher?.Id == id))
            lesson.SetTeacher(null);
    }

    // Розклад

    public void AddScheduleItem(ScheduleItem item)
    {
        _schedule.Add(item);
    }

    public void RemoveScheduleItem(Guid id)
    {
        _schedule.RemoveAll(i => i.Id == id);
    }

    // Справи

    public void AddTask(StudentTask task)
    {
        _tasks.Add(task);
    }

    public void RemoveTask(Guid id)
    {
        _tasks.RemoveAll(t => t.Id == id);
    }

    public int RemoveCompletedTasks()
    {
        return _tasks.RemoveAll(t => t.IsCompleted);
    }

    // Служби

    public void AddService(ServiceContact service)
    {
        _services.Add(service);
    }

    public void RemoveService(Guid id)
    {
        _services.RemoveAll(s => s.Id == id);
    }

    // Пошук

    public IEnumerable<ScheduleItem> SearchSchedule(ScheduleSearchParams searchParams)
    {
        return _schedule.Where(item =>
            item.IsAt(searchParams.Day, searchParams.Time)
            && Matches(searchParams.Query, item.Title, item.Notes, item.Place, (item as Lesson)?.Teacher?.Name));
    }

    public IEnumerable<Teacher> SearchTeachers(TeacherSearchParams searchParams)
    {
        var filterByConsultation = searchParams.ConsultationDay.HasValue || searchParams.ConsultationTime.HasValue;

        return _teachers.Where(teacher =>
            Matches(searchParams.Query, teacher.Name, teacher.Department)
            && (!filterByConsultation ||
                teacher.IsConsultingAt(searchParams.ConsultationDay, searchParams.ConsultationTime)));
    }

    public IEnumerable<Classroom> SearchClassrooms(string? query, string? building = null)
    {
        return _classrooms.Where(c =>
            Matches(query, c.Number, c.Description)
            && Matches(building, c.Building));
    }

    public IEnumerable<ServiceContact> SearchServices(string? query)
    {
        return _services.Where(s =>
            Matches(query, s.Name, s.Description, s.Location, s.Phone));
    }

    public IEnumerable<StudentTask> SearchTasks(TaskSearchParams searchParams)
    {
        var now = DateTime.Now;

        return _tasks.Where(task =>
            Matches(searchParams.Query, task.Title, task.Description)
            && searchParams.Status switch
            {
                TaskStatusFilter.Pending => !task.IsCompleted,
                TaskStatusFilter.Completed => task.IsCompleted,
                TaskStatusFilter.Overdue => task.IsOverdue(now),
                _ => true
            });
    }

    // Накладки в розкладі

    public IEnumerable<ScheduleItem> FindOverlaps()
    {
        return _schedule.Where(a => _schedule.Any(b => a.OverlapsWith(b)));
    }

    public IEnumerable<ScheduleItem> FindOverlapsFor(ScheduleItem item)
    {
        return _schedule.Where(other => other.OverlapsWith(item));
    }

    private static bool Matches(string? query, params string?[] fields)
    {
        if (string.IsNullOrWhiteSpace(query))
            return true;

        var text = query.Trim();

        foreach (var field in fields)
        {
            if (field is not null && field.Contains(text, StringComparison.CurrentCultureIgnoreCase))
                return true;
        }

        return false;
    }
}
