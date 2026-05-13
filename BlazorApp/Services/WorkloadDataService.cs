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
            new() { Name = "Alex Rivera",  Role = "Frontend",  CapacityPercent = 80, TaskCount = 4 },
            new() { Name = "Bea Santos",   Role = "Backend",   CapacityPercent = 50, TaskCount = 2 },
            new() { Name = "Carlos Tan",   Role = "Fullstack", CapacityPercent = 95, TaskCount = 6 },
            new() { Name = "Diana Cruz",   Role = "DevOps",    CapacityPercent = 30, TaskCount = 1 },
            new() { Name = "Eli Park",     Role = "Frontend",  CapacityPercent = 70, TaskCount = 3 },
        };

        var today = DateOnly.FromDateTime(DateTime.Today);
        Leaves = new()
        {
            new() { DeveloperName = "Alex Rivera", StartDate = today.AddDays(2),  EndDate = today.AddDays(4),  LeaveType = "Vacation" },
            new() { DeveloperName = "Carlos Tan",  StartDate = today.AddDays(7),  EndDate = today.AddDays(7),  LeaveType = "Sick Leave" },
            new() { DeveloperName = "Diana Cruz",  StartDate = today.AddDays(9),  EndDate = today.AddDays(13), LeaveType = "Vacation" },
            new() { DeveloperName = "Bea Santos",  StartDate = today.AddDays(-2), EndDate = today.AddDays(-1), LeaveType = "Personal" },
        };

        Tasks = new()
        {
            new() { Title = "Login page redesign",    Assignee = "Alex Rivera", DueDate = today.AddDays(5),  Priority = "High",   Status = "In Progress" },
            new() { Title = "API endpoint refactor",  Assignee = "Bea Santos",  DueDate = today.AddDays(7),  Priority = "Medium", Status = "In Progress" },
            new() { Title = "Database migration",     Assignee = "Carlos Tan",  DueDate = today.AddDays(3),  Priority = "High",   Status = "In Progress" },
            new() { Title = "CI/CD pipeline fix",     Assignee = "Diana Cruz",  DueDate = today.AddDays(9),  Priority = "Low",    Status = "To Do" },
            new() { Title = "Unit test coverage",     Assignee = "Eli Park",    DueDate = today.AddDays(6),  Priority = "Medium", Status = "In Progress" },
            new() { Title = "Auth token refresh",     Assignee = "Carlos Tan",  DueDate = today.AddDays(4),  Priority = "High",   Status = "To Do" },
            new() { Title = "Mobile nav component",   Assignee = "Alex Rivera",  DueDate = today.AddDays(8),  Priority = "Medium", Status = "Done" },
            new() { Title = "Docker compose update",  Assignee = "Diana Cruz",  DueDate = today.AddDays(10), Priority = "Low",    Status = "To Do" },
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
