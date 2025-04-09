using System;
using HrmsApi.Shared.Domain;

namespace HrmsApi.Modules.Attendance.Domain
{
    public class Shift : AuditableEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;      // "Morning Shift", "Night Shift"
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public TimeSpan GracePeriod { get; set; } = TimeSpan.FromMinutes(15); // Late buffer
        public bool IsNightShift { get; set; } = false;

        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
