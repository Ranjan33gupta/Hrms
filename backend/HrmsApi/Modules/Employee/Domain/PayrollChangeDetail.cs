using System;
using System.Collections.Generic;

namespace HrmsApi.Modules.Employee.Domain
{
    public class PayrollChangeDetail
    {
        public PayrollChangeDetail()
        {
            // Ensure timestamp is UTC
            _timestamp = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
        }
        public string Action { get; set; } = string.Empty; // e.g., "Generated", "Updated", "BonusIssued"
        public string PaymentPeriod { get; set; } = string.Empty; // e.g., "April 2025"

        public decimal BasicSalary { get; set; }
        public decimal HRA { get; set; }
        public decimal SpecialAllowance { get; set; }
        public decimal Bonus { get; set; }

        public List<IncentiveComponent> Incentives { get; set; } = new();

        public decimal ProvidentFund { get; set; }
        public decimal ProfessionalTax { get; set; }
        public decimal IncomeTax { get; set; }

        public decimal GrossSalary { get; set; }
        public decimal NetPay { get; set; }

        public string? BankName { get; set; }
        public string? AccountNumberMasked { get; set; }
        public string? IFSC { get; set; }

        public string? Remarks { get; set; }

        private DateTime _timestamp = DateTime.UtcNow;

        public DateTime Timestamp
        {
            get => _timestamp;
            set => _timestamp = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
    }
}
