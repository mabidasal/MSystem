using OfficeOpenXml;
using BlazorApp.Models;

namespace BlazorApp.Services;

public class WorkloadDataService
{
    // ── Data collections (UI binds to these) ─────────
    public List<Developer> Developers { get; private set; } = new();
    public List<TaskItem> Tasks { get; private set; } = new();
    public List<LeaveRecord> Leaves { get; private set; } = new();

    // ── Computed metrics ──────────────────────────────
    public int TotalDevelopers => Developers.Count;
    public int OnLeaveToday => Leaves.Count(l => l.IsActive);
    public int AverageCapacity => Developers.Any()
                                        ? (int)Developers.Average(d => d.CapacityPercent) : 0;
    public int TotalTasks => Tasks.Count;
    public int HighPriorityTasks => Tasks.Count(t => t.Priority == "High");

    // ── Upload state ──────────────────────────────────
    public bool IsLoading { get; private set; }
    public bool HasData { get; private set; }
    public string? LastFileName { get; private set; }
    public string? ErrorMessage { get; private set; }

    // ── Main loader ───────────────────────────────────
    public async Task LoadFromExcelAsync(Stream stream, string fileName)
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            // EPPlus 5+ requires this for non-commercial use
            ExcelPackage.License.SetNonCommercialPersonal("MSystem");

            // Copy to memory first — EPPlus needs a seekable stream
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            ms.Position = 0;

            using var package = new ExcelPackage(ms);

            Developers.Clear();
            Tasks.Clear();
            Leaves.Clear();

            ReadDevelopers(package);
            ReadTasks(package);
            ReadLeaves(package);

            // Auto-compute TaskCount from Tasks list if sheet didn't have it
            foreach (var dev in Developers)
            {
                if (dev.TaskCount == 0)
                    dev.TaskCount = Tasks.Count(t => t.Assignee == dev.Name);
            }

            // Auto-compute CapacityPercent if missing (tasks × 20, capped at 100)
            foreach (var dev in Developers)
            {
                if (dev.CapacityPercent == 0 && dev.TaskCount > 0)
                    dev.CapacityPercent = Math.Min(dev.TaskCount * 20, 100);
            }

            HasData = true;
            LastFileName = fileName;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to read \"{fileName}\": {ex.Message}";
            HasData = false;
        }
        finally
        {
            IsLoading = false;
        }
    }

    // ── Sheet readers ─────────────────────────────────

    private void ReadDevelopers(ExcelPackage package)
    {
        // Match by sheet name first, fall back to first sheet
        var ws = package.Workbook.Worksheets["Developers"]
              ?? package.Workbook.Worksheets["developers"]
              ?? package.Workbook.Worksheets[0];

        if (ws?.Dimension == null) return;

        // Detect column positions from header row (case-insensitive)
        var headers = GetHeaders(ws);

        int colName = ColIndex(headers, "name") ?? 1;
        int colRole = ColIndex(headers, "role") ?? 2;
        int colCapacity = ColIndex(headers, "capacitypercent",
                                            "capacity", "cap") ?? 3;
        int colTasks = ColIndex(headers, "taskcount",
                                            "tasks") ?? 4;

        for (int row = 2; row <= ws.Dimension.End.Row; row++)
        {
            var name = ws.Cells[row, colName].Text.Trim();
            if (string.IsNullOrWhiteSpace(name)) continue;

            Developers.Add(new Developer
            {
                Name = name,
                Role = ws.Cells[row, colRole].Text.Trim(),
                CapacityPercent = ParseInt(ws.Cells[row, colCapacity].Text),
                TaskCount = ParseInt(ws.Cells[row, colTasks].Text),
            });
        }
    }

    private void ReadTasks(ExcelPackage package)
    {
        var ws = package.Workbook.Worksheets["Tasks"]
              ?? package.Workbook.Worksheets["tasks"]
              ?? (package.Workbook.Worksheets.Count > 1
                      ? package.Workbook.Worksheets[1] : null);

        if (ws?.Dimension == null) return;

        var headers = GetHeaders(ws);

        int colTitle = ColIndex(headers, "title", "task", "name") ?? 1;
        int colAssignee = ColIndex(headers, "assignee", "assigned to",
                                            "developer") ?? 2;
        int colProject = ColIndex(headers, "projectname", "project") ?? 3;
        int colDue = ColIndex(headers, "duedate", "due date", "due") ?? 4;
        int colPriority = ColIndex(headers, "priority") ?? 5;
        int colStatus = ColIndex(headers, "status") ?? 6;

        for (int row = 2; row <= ws.Dimension.End.Row; row++)
        {
            var title = ws.Cells[row, colTitle].Text.Trim();
            if (string.IsNullOrWhiteSpace(title)) continue;

            Tasks.Add(new TaskItem
            {
                Title = title,
                Assignee = ws.Cells[row, colAssignee].Text.Trim(),
                ProjectName = ws.Cells[row, colProject].Text.Trim(),
                DueDate = ParseDate(ws.Cells[row, colDue]),
                Priority = Normalise(ws.Cells[row, colPriority].Text, "Medium"),
                Status = Normalise(ws.Cells[row, colStatus].Text, "In Progress"),
            });
        }
    }

    private void ReadLeaves(ExcelPackage package)
    {
        var ws = package.Workbook.Worksheets["Leaves"]
              ?? package.Workbook.Worksheets["Leave"]
              ?? package.Workbook.Worksheets["leaves"]
              ?? (package.Workbook.Worksheets.Count > 2
                      ? package.Workbook.Worksheets[2] : null);

        if (ws?.Dimension == null) return;

        var headers = GetHeaders(ws);

        int colDev = ColIndex(headers, "developername", "developer",
                                          "name", "member") ?? 1;
        int colType = ColIndex(headers, "leavetype", "leave type", "type") ?? 2;
        int colStart = ColIndex(headers, "startdate", "start date", "from",
                                          "start") ?? 3;
        int colEnd = ColIndex(headers, "enddate", "end date", "to", "end") ?? 4;

        for (int row = 2; row <= ws.Dimension.End.Row; row++)
        {
            var dev = ws.Cells[row, colDev].Text.Trim();
            if (string.IsNullOrWhiteSpace(dev)) continue;

            var leaveType = Normalise(ws.Cells[row, colType].Text, "Vacation");

            Leaves.Add(new LeaveRecord
            {
                DeveloperName = dev,
                LeaveType = leaveType,
                StartDate = ParseDate(ws.Cells[row, colStart]),
                EndDate = ParseDate(ws.Cells[row, colEnd]),
            });
        }
    }

    // ── Helpers ───────────────────────────────────────

    /// <summary>Read row 1 into a dictionary of (normalised header → 1-based column index).</summary>
    private static Dictionary<string, int> GetHeaders(ExcelWorksheet ws)
    {
        var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        if (ws.Dimension == null) return dict;

        for (int col = 1; col <= ws.Dimension.End.Column; col++)
        {
            var header = ws.Cells[1, col].Text.Trim()
                           .ToLowerInvariant()
                           .Replace(" ", "");
            if (!string.IsNullOrEmpty(header) && !dict.ContainsKey(header))
                dict[header] = col;
        }
        return dict;
    }

    /// <summary>Find the first matching alias in the header dict.</summary>
    private static int? ColIndex(Dictionary<string, int> headers, params string[] aliases)
    {
        foreach (var alias in aliases)
        {
            var key = alias.ToLowerInvariant().Replace(" ", "");
            if (headers.TryGetValue(key, out int col)) return col;
        }
        return null;
    }

    private static int ParseInt(string text)
        => int.TryParse(text.Replace("%", "").Trim(), out var v) ? v : 0;

    /// <summary>Parse a date cell — handles both text and numeric (Excel serial) dates.</summary>
    private static DateOnly ParseDate(ExcelRange cell)
    {
        // EPPlus can expose the underlying DateTime for date-formatted cells
        if (cell.Value is DateTime dt)
            return DateOnly.FromDateTime(dt);

        if (cell.Value is double d)
            return DateOnly.FromDateTime(DateTime.FromOADate(d));

        if (DateOnly.TryParse(cell.Text.Trim(), out var parsed))
            return parsed;

        return DateOnly.FromDateTime(DateTime.Today);
    }

    /// <summary>Return the trimmed value if non-empty, otherwise the fallback.</summary>
    private static string Normalise(string text, string fallback)
    {
        var t = text.Trim();
        return string.IsNullOrEmpty(t) ? fallback : t;
    }
}