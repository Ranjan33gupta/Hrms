using System;

namespace HrmsApi.Modules.Leave.Application.DTOs
{
    public class LeaveRequestDTO
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LeaveType { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string? Comments { get; set; }
        public int DurationInDays => (EndDate - StartDate).Days + 1;
    }

    public class CreateLeaveRequestDTO
    {
        public Guid EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LeaveType { get; set; } = "Annual";
        public string Reason { get; set; } = string.Empty;
    }

    public class UpdateLeaveRequestDTO
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LeaveType { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }

    public class ApproveLeaveRequestDTO
    {
        public Guid Id { get; set; }
        public string ApprovedBy { get; set; } = string.Empty;
        public string? Comments { get; set; }
    }

    public class RejectLeaveRequestDTO
    {
        public Guid Id { get; set; }
        public string RejectedBy { get; set; } = string.Empty;
        public string? Comments { get; set; }
    }
}
