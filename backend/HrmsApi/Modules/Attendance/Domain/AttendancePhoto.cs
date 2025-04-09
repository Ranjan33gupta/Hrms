using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HrmsApi.Modules.Attendance.Domain
{
    public class AttendancePhoto
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid AttendanceId { get; set; }

        [ForeignKey("AttendanceId")]
        public virtual Attendance Attendance { get; set; } = null!;

        [Required]
        public bool IsClockIn { get; set; } // true for clock-in, false for clock-out

        [Required]
        [Column(TypeName = "text")]
        public string PhotoUrl { get; set; } = string.Empty;

        public string? StoragePath { get; set; }

        public DateTime CaptureTime { get; set; } = DateTime.UtcNow;

        public string? DeviceInfo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
    }
}
