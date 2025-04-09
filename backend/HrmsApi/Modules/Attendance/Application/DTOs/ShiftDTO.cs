using System;

namespace HrmsApi.Modules.Attendance.Application.DTOs
{
    public class ShiftDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan GracePeriod { get; set; }
        public bool IsNightShift { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateShiftDTO
    {
        public string Name { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan GracePeriod { get; set; } = TimeSpan.FromMinutes(15);
        public bool IsNightShift { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateShiftDTO
    {
        public string Name { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan GracePeriod { get; set; }
        public bool IsNightShift { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
