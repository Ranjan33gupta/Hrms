using System;

namespace HrmsApi.Modules.Employee.Domain
{
    public class IncentiveComponent
    {
        public string Name { get; set; } = string.Empty; // e.g., "Performance Bonus", "Sales Commission"
        public decimal Amount { get; set; }
        public string? Reason { get; set; } // Optional: "Exceeded quarterly target", etc.
    }
}
