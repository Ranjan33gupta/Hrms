using System;

namespace HrmsApi.Modules.Settings.Application.DTOs
{
    public class LeavePolicyDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LeaveType { get; set; } = string.Empty;
        public int DaysAllowed { get; set; }
        public bool IsCarryForward { get; set; }
        public int? MaxCarryForwardDays { get; set; }
        public bool RequiresApproval { get; set; } = true;
        public int? MinDaysNotice { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Description { get; set; }
    }

    public class CreateLeavePolicyDTO
    {
        public string Name { get; set; } = string.Empty;
        public string LeaveType { get; set; } = string.Empty;
        public int DaysAllowed { get; set; }
        public bool IsCarryForward { get; set; }
        public int? MaxCarryForwardDays { get; set; }
        public bool RequiresApproval { get; set; } = true;
        public int? MinDaysNotice { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateLeavePolicyDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LeaveType { get; set; } = string.Empty;
        public int DaysAllowed { get; set; }
        public bool IsCarryForward { get; set; }
        public int? MaxCarryForwardDays { get; set; }
        public bool RequiresApproval { get; set; } = true;
        public int? MinDaysNotice { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Description { get; set; }
    }
}
