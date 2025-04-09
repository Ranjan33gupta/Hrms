using System;

namespace HrmsApi.Modules.Settings.Domain
{
    public class Holiday
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool IsRecurringYearly { get; set; }
        public string? Description { get; set; }
        public HolidayType Type { get; set; } = HolidayType.Company;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public enum HolidayType
    {
        Company = 0,
        Government = 1,
        Optional = 2
    }
}
