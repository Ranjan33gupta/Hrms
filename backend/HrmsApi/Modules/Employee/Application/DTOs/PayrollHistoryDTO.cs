using System;
using System.Collections.Generic;
using HrmsApi.Modules.Employee.Domain;

namespace HrmsApi.Modules.Employee.Application.DTOs
{
    public class PayrollHistoryDTO
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public Guid PayrollId { get; set; }
        
        // Dictionary with DateTime keys and lists of PayrollChangeDetail values
        public IDictionary<DateTime, List<PayrollChangeDetail>>? PayrollChanges { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
    
    // DTO for creating a new payroll history record
    public class CreatePayrollHistoryDTO
    {
        public Guid EmployeeId { get; set; }
        public Guid PayrollId { get; set; }
        public PayrollChangeDetail PayrollChange { get; set; } = new PayrollChangeDetail();
    }
}
