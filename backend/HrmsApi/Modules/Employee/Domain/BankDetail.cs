using System;

namespace HrmsApi.Modules.Employee.Domain
{
    public class BankDetail
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }  // FK
        public string? BankName { get; set; }
        public string? AccountHolderName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSCCode { get; set; } // For India, or SWIFT code for international
        public string? BranchName { get; set; }

        // Navigation
        public Employee? Employee { get; set; }
    }
}
