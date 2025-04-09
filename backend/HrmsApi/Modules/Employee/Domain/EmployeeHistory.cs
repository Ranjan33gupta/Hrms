using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HrmsApi.Modules.Employee.Domain
{
    public class EmployeeHistory
    {
        public EmployeeHistory()
        {
            // Ensure proper initialization with UTC dates
            _createdAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            EmployeeChangeDetails = new Dictionary<string, List<EmployeeChangeDetail>>();
        }
        public Guid Id { get; set; } // PK

        public Guid EmployeeId { get; set; } // FK to Employee
        public string? EmployeeName { get; set; }

        // Key: Modification datetime (stored as string in ISO 8601 format to avoid DateTime serialization issues)
        // Value: List of changes at that time
        [Column(TypeName = "jsonb")]
        public IDictionary<string, List<EmployeeChangeDetail>>? EmployeeChangeDetails { get; set; } = new Dictionary<string, List<EmployeeChangeDetail>>();

        // Helper method to add changes with proper UTC DateTime handling
        public void AddChanges(DateTime timestamp, List<EmployeeChangeDetail> changes)
        {
            // Ensure UTC kind and convert to ISO 8601 string
            var utcTime = DateTime.SpecifyKind(timestamp, DateTimeKind.Utc);
            var timeKey = utcTime.ToString("o"); // ISO 8601 format

            if (EmployeeChangeDetails == null)
            {
                EmployeeChangeDetails = new Dictionary<string, List<EmployeeChangeDetail>>();
            }

            EmployeeChangeDetails[timeKey] = changes;
        }

        // Navigation property
        public virtual Employee? Employee { get; set; }

        // Audit fields with UTC enforcement
        private DateTime _createdAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
        private DateTime? _updatedAt;

        public DateTime CreatedAt
        {
            get => _createdAt;
            set => _createdAt = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public DateTime? UpdatedAt
        {
            get => _updatedAt;
            set => _updatedAt = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : null;
        }

        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
