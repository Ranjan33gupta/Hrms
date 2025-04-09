using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace HrmsApi.Modules.Attendance.Application.DTOs
{
    public class AttendancePhotoDTO
    {
        public Guid Id { get; set; }
        public Guid AttendanceId { get; set; }
        public bool IsClockIn { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public DateTime CaptureTime { get; set; }
        public string? DeviceInfo { get; set; }
    }

    public class UploadAttendancePhotoDTO
    {
        [Required]
        public Guid AttendanceId { get; set; }
        
        [Required]
        public bool IsClockIn { get; set; }
        
        [Required]
        public IFormFile Photo { get; set; } = null!;
        
        public string? DeviceInfo { get; set; }
    }
}
