using System;

namespace HrmsApi.Modules.Employee.Domain
{
    public class Payroll
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }  // FK
        // Removing EmployeeName property as it doesn't exist in the database
        public DateTime SalaryMonth { get; set; }

        public decimal BasicSalary { get; set; }
        public decimal HRA { get; set; }
        public decimal Allowances { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetSalary { get; set; }
        public DateTime PaymentDate { get; set; }

        // Navigation
        public Employee? Employee { get; set; }
    }
}
