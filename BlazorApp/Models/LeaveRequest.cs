using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }

        [Required]
        public string EmployeeName { get; set; } = string.Empty;

        [Required]
        public string LeaveType { get; set; } = "Vacation"; // Vacation, Sick, Personal

        [Required]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1);

        public string Status { get; set; } = "Pending"; // Pending, Approved, Declined

        public string Reason { get; set; } = string.Empty;
    }
}