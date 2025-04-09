using System;

namespace HrmsApi.Modules.Employee.Application.DTOs
{
    public class BankDetailDTO
    {
        public Guid Id { get; set; }
        public Guid? EmployeeId { get; set; }
        public string? BankName { get; set; }
        public string? AccountHolderName { get; set; }
        public string? AccountNumber { get; set; }
        public string? IFSCCode { get; set; }
        public string? BranchName { get; set; }
    }
}
