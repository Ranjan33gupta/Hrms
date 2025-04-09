using System;
using HrmsApi.Modules.Attendance.Domain;

namespace HrmsApi.Modules.Attendance.Application.DTOs
{
    public class AttendanceDTO
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeSpan ClockIn { get; set; }
        public TimeSpan? ClockOut { get; set; }
        public string? Notes { get; set; }
        public double HoursWorked { get; set; }
        
        // Location tracking
        public string? CheckInLocation { get; set; }
        public string? CheckOutLocation { get; set; }
        public string? CheckInIpAddress { get; set; }
        public string? CheckOutIpAddress { get; set; }
        public double? CheckInLatitude { get; set; }
        public double? CheckInLongitude { get; set; }
        public double? CheckOutLatitude { get; set; }
        public double? CheckOutLongitude { get; set; }
        
        // Shift information
        public Guid? ShiftId { get; set; }
        public string? ShiftName { get; set; }
        public AttendanceStatus Status { get; set; }
        public bool IsLate { get; set; }
        public bool IsEarlyDeparture { get; set; }
        
        // Audit fields
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateAttendanceDTO
    {
        public Guid EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan ClockIn { get; set; }
        public TimeSpan? ClockOut { get; set; }
        public string? Notes { get; set; }
        
        // Location tracking
        public string? CheckInLocation { get; set; }
        public string? CheckInDevice { get; set; }
        public string? CheckInIpAddress { get; set; }
        public double? CheckInLatitude { get; set; }
        public double? CheckInLongitude { get; set; }
    }

    public class UpdateAttendanceDTO
    {
        public TimeSpan? ClockOut { get; set; }
        public string? Notes { get; set; }
        
        // Location tracking for clock out
        public string? CheckOutLocation { get; set; }
        public string? CheckOutDevice { get; set; }
        public string? CheckOutIpAddress { get; set; }
        public double? CheckOutLatitude { get; set; }
        public double? CheckOutLongitude { get; set; }
    }
    
    public class ClockInDTO
    {
        public Guid EmployeeId { get; set; }
        public string? CheckInLocation { get; set; }
        public string? CheckInDevice { get; set; }
        public string? CheckInIpAddress { get; set; }
        public double? CheckInLatitude { get; set; }
        public double? CheckInLongitude { get; set; }
        public string? Notes { get; set; }
    }
    
    public class ClockOutDTO
    {
        public Guid EmployeeId { get; set; }
        public string? CheckOutLocation { get; set; }
        public string? CheckOutDevice { get; set; }
        public string? CheckOutIpAddress { get; set; }
        public double? CheckOutLatitude { get; set; }
        public double? CheckOutLongitude { get; set; }
        public string? Notes { get; set; }
    }
}
