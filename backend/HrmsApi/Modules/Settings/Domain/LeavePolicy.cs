using System;

namespace HrmsApi.Modules.Settings.Domain
{
    public class LeavePolicy
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LeaveType { get; set; } = string.Empty; // Annual, Sick, Casual, etc.
        public int DaysAllowed { get; set; }
        public bool IsCarryForward { get; set; }
        public int? MaxCarryForwardDays { get; set; }
        public bool RequiresApproval { get; set; } = true;
        public int? MinDaysNotice { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
