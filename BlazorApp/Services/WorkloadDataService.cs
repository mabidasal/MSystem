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
            new() { Name = "Marlou John Aquino", Role = "Backend", CapacityPercent = 80, TaskCount = 4 },
            new() { Name = "Marcus Dacaymat",    Role = "Backend", CapacityPercent = 50, TaskCount = 2 },
            new() { Name = "Elijah Payok",       Role = "Backend", CapacityPercent = 95, TaskCount = 6 },
            new() { Name = "Polo Dacaymat",      Role = "Backend", CapacityPercent = 30, TaskCount = 1 },
            new() { Name = "Jerrick Decena",     Role = "Backend", CapacityPercent = 70, TaskCount = 3 },
            new() { Name = "Khen Albarico",      Role = "Backend", CapacityPercent = 70, TaskCount = 3 },
            new() { Name = "Luis Flores",        Role = "Backend", CapacityPercent = 70, TaskCount = 3 },
        };

        var today = DateOnly.FromDateTime(DateTime.Today);
        Leaves = new()
        {
            new() { DeveloperName = "Marlou John Aquino", StartDate = today.AddDays(2),  EndDate = today.AddDays(4),  LeaveType = "Vacation"  },
            new() { DeveloperName = "Elijah Payok",       StartDate = today.AddDays(7),  EndDate = today.AddDays(7),  LeaveType = "Sick Leave" },
            new() { DeveloperName = "Jerrick Decena",     StartDate = today.AddDays(9),  EndDate = today.AddDays(13), LeaveType = "Vacation"  },
            new() { DeveloperName = "Marcus Dacaymat",    StartDate = today.AddDays(-2), EndDate = today.AddDays(-1), LeaveType = "Personal"  },
        };

        Tasks = new()
        {
            new() { Title = "Login page redesign",   Assignee = "Marlou John Aquino", DueDate = today.AddDays(5),  Priority = "High",   Status = "In Progress" },
            new() { Title = "API endpoint refactor", Assignee = "Marcus Dacaymat",    DueDate = today.AddDays(7),  Priority = "Medium", Status = "In Progress" },
            new() { Title = "Database migration",    Assignee = "Elijah Payok",       DueDate = today.AddDays(3),  Priority = "High",   Status = "In Progress" },
            new() { Title = "CI/CD pipeline fix",    Assignee = "Polo Dacaymat",      DueDate = today.AddDays(9),  Priority = "Low",    Status = "To Do"       },
            new() { Title = "Unit test coverage",    Assignee = "Jerrick Decena",     DueDate = today.AddDays(6),  Priority = "Medium", Status = "In Progress" },
            new() { Title = "Auth token refresh",    Assignee = "Khen Albarico",      DueDate = today.AddDays(4),  Priority = "High",   Status = "To Do"       },
            new() { Title = "Mobile nav component",  Assignee = "Luis Flores",        DueDate = today.AddDays(8),  Priority = "Medium", Status = "Done"        },
            new() { Title = "Docker compose update", Assignee = "Polo Dacaymat",      DueDate = today.AddDays(10), Priority = "Low",    Status = "To Do"       },
        };
    }

    // -------------------------------------------------------
    // Placeholder: wire up ClosedXML or NPOI here to parse
    // an uploaded .xlsx and populate the three lists above.
    // -------------------------------------------------------
    public async Task LoadFromExcelAsync(Stream stream, string fileName)
    {
        // TODO: implement Excel parsing
        // Example with ClosedXML:
        //   using var wb = new XLWorkbook(stream);
        //   var devSheet = wb.Worksheet("Developers");
        //   Developers = devSheet.RowsUsed().Skip(1).Select(row => new Developer { ... }).ToList();
        LoadedFileName = fileName;
        await Task.CompletedTask;
    }

    // ---- Aggregates ----
    public int TotalDevelopers => Developers.Count;
    public int OnLeaveToday => Leaves.Count(l => l.IsActive);
    public int AverageCapacity => Developers.Any() ? (int)Developers.Average(d => d.CapacityPercent) : 0;
    public int HighPriorityTasks => Tasks.Count(t => t.Priority == "High");
    public int TotalTasks => Tasks.Count;
}