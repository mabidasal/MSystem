namespace BlazorApp.Models;

public class Developer
{
    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
    public int CapacityPercent { get; set; }
    public int TaskCount { get; set; }
    public string AvatarInitials => string.Join("", Name.Split(' ').Select(w => w.FirstOrDefault())).ToUpper();
}

public class LeaveRecord
{
    public string DeveloperName { get; set; } = "";
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string LeaveType { get; set; } = "Vacation";
    public bool IsActive => DateOnly.FromDateTime(DateTime.Today) >= StartDate && DateOnly.FromDateTime(DateTime.Today) <= EndDate;
}

public class TaskItem
{
    public string Title { get; set; } = "";
    public string Assignee { get; set; } = "";
    public DateOnly DueDate { get; set; }
    public string Priority { get; set; } = "Medium";
    public string Status { get; set; } = "In Progress";
}
