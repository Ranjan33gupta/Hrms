using System;
using System.Collections.Generic;
using HrmsApi.Modules.Employee.Domain;

namespace HrmsApi.Modules.Employee.Application.DTOs
{
    public class EmployeeHistoryDTO
    {
        public Guid? Id { get; set; }
        public Guid? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public Dictionary<string, List<EmployeeChangeDetailDTO>>? EmployeeChangeDetails { get; set; }
    }

    public class EmployeeChangeDetailDTO
    {
        public string? Action { get; set; }
        public string? FieldChanged { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class FlattenedEmployeeHistoryDTO
    {
        public Guid? Id { get; set; }
        public Guid? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public DateTime? ChangeDate { get; set; }
        public string? Action { get; set; }
        public string? FieldChanged { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? ChangedBy { get; set; }
    }
}
