using System;
using System.Text.Json.Serialization;
using HrmsApi.Modules.Employee.Domain;

namespace HrmsApi.Modules.Leave.Domain
{
    public class LeaveRequest
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        
        // Navigation property
        public Employee.Domain.Employee? Employee { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LeaveType { get; set; } = "Annual"; // Annual, Sick, Personal, etc.
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string? Comments { get; set; }
        
        // Calculated property
        [JsonIgnore]
        public int DurationInDays => (EndDate - StartDate).Days + 1;
    }
}
