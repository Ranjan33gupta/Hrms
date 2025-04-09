using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HrmsApi.Shared.Domain;

namespace HrmsApi.Modules.Attendance.Domain
{
    public class Attendance : AuditableEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid EmployeeId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan ClockIn { get; set; }

        public TimeSpan? ClockOut { get; set; }

        // Location tracking fields
        public string? CheckInLocation { get; set; }
        public string? CheckOutLocation { get; set; }
        public string? CheckInDevice { get; set; }
        public string? CheckOutDevice { get; set; }
        public string? CheckInIpAddress { get; set; }
        public string? CheckOutIpAddress { get; set; }
        public double? CheckInLatitude { get; set; }
        public double? CheckInLongitude { get; set; }
        public double? CheckOutLatitude { get; set; }
        public double? CheckOutLongitude { get; set; }

        public string? Notes { get; set; }

        // Shift management
        public Guid? ShiftId { get; set; }
        public virtual Shift? Shift { get; set; }
        
        public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;

        // Calculated property for total hours worked
        [NotMapped]
        public double HoursWorked
        {
            get
            {
                if (ClockOut.HasValue)
                {
                    return (ClockOut.Value - ClockIn).TotalHours;
                }
                return 0;
            }
        }

        [NotMapped]
        public bool IsLate 
        {
            get
            {
                if (Shift != null)
                {
                    TimeSpan shiftStart = Shift.StartTime;
                    TimeSpan graceEnd = shiftStart.Add(Shift.GracePeriod);
                    return ClockIn > graceEnd;
                }
                return false;
            }
        }

        [NotMapped]
        public bool IsEarlyDeparture
        {
            get
            {
                if (ClockOut.HasValue && Shift != null)
                {
                    TimeSpan shiftEnd = Shift.EndTime;
                    return ClockOut.Value < shiftEnd;
                }
                return false;
            }
        }
    }
}
