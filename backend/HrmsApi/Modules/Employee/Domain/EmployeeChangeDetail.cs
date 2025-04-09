using System;

namespace HrmsApi.Modules.Employee.Domain
{
    public class EmployeeChangeDetail
    {
        public EmployeeChangeDetail()
        {
            // Ensure timestamp is UTC
            _timestamp = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
        }
        public string Action { get; set; } = string.Empty;         // e.g., "Update", "Delete"
        public string FieldChanged { get; set; } = string.Empty;   // e.g., "Designation"
        public string OldValue { get; set; } = string.Empty;
        public string NewValue { get; set; } = string.Empty;
        private DateTime _timestamp = DateTime.UtcNow;

        public DateTime Timestamp
        {
            get => _timestamp;
            set => _timestamp = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
    }
}
