using System;

namespace HrmsApi.Modules.Employee.Application.DTOs
{
    public class PayrollDTO
    {
        public Guid? Id { get; set; }
        public Guid? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public DateTime? SalaryMonth { get; set; }
        public decimal? BasicSalary { get; set; }
        public decimal? HRA { get; set; }
        public decimal? Allowances { get; set; }
        public decimal? Deductions { get; set; }
        public decimal? NetSalary { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}
