using BlazorApp.Models;

namespace BlazorApp.Services;

public class WorkloadDataService
{
    public List<Developer> Developers { get; private set; } = new();
    public List<LeaveRecord> Leaves { get; private set; } = new();
    public List<TaskItem> Tasks { get; private set; } = new();
    public string? LoadedFileName { get; private set; }

    public WorkloadDataService()
    {
        LoadSampleData();
    }

    public void LoadSampleData()
    {
        LoadedFileName = "Sample Data";

        Developers = new()
    {
        new() { Name = "Marlou John Aquino", Role = "Developer", CapacityPercent = 80, TaskCount = 4 },
        new() { Name = "Marcus Dacaymat",    Role = "Developer", CapacityPercent = 50, TaskCount = 2 },
        new() { Name = "Elijah Payok",       Role = "Developer", CapacityPercent = 95, TaskCount = 6 },
        new() { Name = "Polo Dacaymat",      Role = "Developer", CapacityPercent = 30, TaskCount = 1 },
        new() { Name = "Jerrick Decena",     Role = "Developer", CapacityPercent = 70, TaskCount = 3 },
        new() { Name = "Khen Albarico",      Role = "Developer", CapacityPercent = 70, TaskCount = 3 },
        new() { Name = "Luis Flores",        Role = "Developer", CapacityPercent = 70, TaskCount = 3 },
    };

        var today = DateOnly.FromDateTime(DateTime.Today);
        Leaves = new()
    {
        // Active today — Marlou is currently on leave (started yesterday, ends tomorrow)
        new() { DeveloperName = "Marlou John Aquino", StartDate = today.AddDays(-1), EndDate = today.AddDays(1),  LeaveType = "Vacation"  },

        // Upcoming leaves
        new() { DeveloperName = "Elijah Payok",       StartDate = today.AddDays(3),  EndDate = today.AddDays(5),  LeaveType = "Sick Leave" },
        new() { DeveloperName = "Jerrick Decena",     StartDate = today.AddDays(7),  EndDate = today.AddDays(11), LeaveType = "Vacation"   },

        // Past leave
        new() { DeveloperName = "Marcus Dacaymat",    StartDate = today.AddDays(-3), EndDate = today.AddDays(-2), LeaveType = "Personal"   },
    };

        Tasks = new()
    {
        new() { Title = "Login page redesign",   Assignee = "Marlou John Aquino", ProjectName = "Elite Service Bus",  DueDate = today.AddDays(5),  Priority = "High",   Status = "In Progress" },
        new() { Title = "API endpoint refactor", Assignee = "Marcus Dacaymat",    ProjectName = "Zoho",               DueDate = today.AddDays(7),  Priority = "Medium", Status = "In Progress" },
        new() { Title = "Database migration",    Assignee = "Elijah Payok",       ProjectName = "ChromeRiver",        DueDate = today.AddDays(3),  Priority = "High",   Status = "In Progress" },
        new() { Title = "CI/CD pipeline fix",    Assignee = "Polo Dacaymat",      ProjectName = "Proceedings AT",     DueDate = today.AddDays(9),  Priority = "Low",    Status = "To Do"       },
        new() { Title = "Unit test coverage",    Assignee = "Jerrick Decena",     ProjectName = "Raise",              DueDate = today.AddDays(6),  Priority = "Medium", Status = "In Progress" },
        new() { Title = "Auth token refresh",    Assignee = "Khen Albarico",      ProjectName = "TM Monitoring",      DueDate = today.AddDays(4),  Priority = "High",   Status = "To Do"       },
        new() { Title = "Mobile nav component",  Assignee = "Luis Flores",        ProjectName = "Chorus",             DueDate = today.AddDays(8),  Priority = "Medium", Status = "Done"        },
        new() { Title = "Docker compose update", Assignee = "Polo Dacaymat",      ProjectName = "Proceedings AT",     DueDate = today.AddDays(10), Priority = "Low",    Status = "To Do"       },
    };
    }

    public async Task LoadFromExcelAsync(Stream stream, string fileName)
    {
        LoadedFileName = fileName;
        await Task.CompletedTask;
    }

    // ---- Aggregates ----
    public int TotalDevelopers => Developers.Count;
    public int OnLeaveToday => Leaves.Count(l => l.IsActive);
    public int AverageCapacity => Developers.Any() ? (int)Developers.Average(d => d.CapacityPercent) : 0;
    public int HighPriorityTasks => Tasks.Count(t => t.Priority == "High");
    public int TotalTasks => Tasks.Count;

    // ---- Leave Helpers ----

    /// <summary>Returns the active leave record for a developer, or null if not on leave today.</summary>
    public LeaveRecord? GetActiveLeave(string developerName)
        => Leaves.FirstOrDefault(l => l.DeveloperName == developerName && l.IsActive);

    /// <summary>Returns upcoming leaves starting within the next 7 days for a developer.</summary>
    public IEnumerable<LeaveRecord> GetUpcomingLeaves(string developerName)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return Leaves.Where(l =>
            l.DeveloperName == developerName &&
            l.StartDate > today &&
            l.StartDate <= today.AddDays(7));
    }

    /// <summary>Returns all developers who are on leave today.</summary>
    public IEnumerable<Developer> DevelopersOnLeaveToday()
        => Developers.Where(d => GetActiveLeave(d.Name) != null);

    /// <summary>Effective capacity: 0 if on leave today, otherwise normal CapacityPercent.</summary>
    public int EffectiveCapacity(Developer dev)
        => GetActiveLeave(dev.Name) != null ? 0 : dev.CapacityPercent;
}