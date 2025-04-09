using System;
using HrmsApi.Shared.Domain;

namespace HrmsApi.Modules.Attendance.Domain
{
    public class EmployeeShiftAssignment : AuditableEntity
    {
        public Guid Id { get; set; }

        public Guid EmployeeId { get; set; }
        public Guid ShiftId { get; set; }

        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; } // null means current

        public bool IsActive => EffectiveTo == null || EffectiveTo >= DateTime.UtcNow;
        
        // Navigation property
        public virtual Shift? Shift { get; set; }
    }
}
