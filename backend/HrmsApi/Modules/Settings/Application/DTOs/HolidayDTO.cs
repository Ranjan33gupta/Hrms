using System;
using HrmsApi.Modules.Settings.Domain;

namespace HrmsApi.Modules.Settings.Application.DTOs
{
    public class HolidayDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool IsRecurringYearly { get; set; }
        public string? Description { get; set; }
        public HolidayType Type { get; set; } = HolidayType.Company;
        public bool IsActive { get; set; } = true;
    }

    public class CreateHolidayDTO
    {
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool IsRecurringYearly { get; set; }
        public string? Description { get; set; }
        public HolidayType Type { get; set; } = HolidayType.Company;
    }

    public class UpdateHolidayDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool IsRecurringYearly { get; set; }
        public string? Description { get; set; }
        public HolidayType Type { get; set; } = HolidayType.Company;
        public bool IsActive { get; set; } = true;
    }
}
