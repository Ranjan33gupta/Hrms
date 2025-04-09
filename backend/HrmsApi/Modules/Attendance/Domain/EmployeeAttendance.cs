using System;
using HrmsApi.Shared.Domain;

namespace HrmsApi.Modules.Attendance.Domain
{
    public class EmployeeAttendance : AuditableEntity
    {
        public Guid Id { get; set; }

        public Guid EmployeeId { get; set; }
        public DateTime AttendanceDate { get; set; }

        public TimeSpan? CheckIn { get; set; }
        public TimeSpan? CheckOut { get; set; }

        public string? CheckInLocation { get; set; }      // IP / Geolocation
        public string? CheckOutLocation { get; set; }

        public string? Remarks { get; set; }              // e.g., "Late due to traffic"
        
        public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;

        public Guid? ShiftId { get; set; }                // FK to assigned shift (optional)
        public virtual Shift? Shift { get; set; }

        // Navigation properties
        public string? CheckInDevice { get; set; }
        public string? CheckOutDevice { get; set; }
        public string? CheckInIpAddress { get; set; }
        public string? CheckOutIpAddress { get; set; }
        public double? CheckInLatitude { get; set; }
        public double? CheckInLongitude { get; set; }
        public double? CheckOutLatitude { get; set; }
        public double? CheckOutLongitude { get; set; }
        
        // Calculated properties
        public TimeSpan? WorkedHours 
        { 
            get 
            {
                if (CheckIn.HasValue && CheckOut.HasValue)
                {
                    return CheckOut.Value - CheckIn.Value;
                }
                return null;
            } 
        }

        public bool IsLate 
        {
            get
            {
                if (CheckIn.HasValue && Shift != null)
                {
                    TimeSpan shiftStart = TimeSpan.FromTicks(Shift.StartTime.Ticks);
                    TimeSpan graceEnd = shiftStart.Add(Shift.GracePeriod);
                    return CheckIn.Value > graceEnd;
                }
                return false;
            }
        }

        public bool IsEarlyDeparture
        {
            get
            {
                if (CheckOut.HasValue && Shift != null)
                {
                    TimeSpan shiftEnd = TimeSpan.FromTicks(Shift.EndTime.Ticks);
                    return CheckOut.Value < shiftEnd;
                }
                return false;
            }
        }
    }
}
