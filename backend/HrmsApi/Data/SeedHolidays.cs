using System;
using System.Collections.Generic;
using System.Linq;
using HrmsApi.Modules.Settings.Domain;

namespace HrmsApi.Data
{
    public static class SeedHolidays
    {
        public static void SeedIndianHolidays(HrmsDbContext context)
        {
            // Check if holidays already exist
            if (context.Holidays.Any())
            {
                return; // Skip seeding if holidays already exist
            }

            var holidays = new List<Holiday>
            {
                // Government Holidays (National)
                new Holiday
                {
                    Id = Guid.NewGuid(),
                    Name = "Republic Day",
                    Date = new DateTime(2025, 1, 26),
                    IsRecurringYearly = true,
                    Description = "Commemorates the adoption of the Constitution of India",
                    Type = HolidayType.Government,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Holiday
                {
                    Id = Guid.NewGuid(),
                    Name = "Independence Day",
                    Date = new DateTime(2025, 8, 15),
                    IsRecurringYearly = true,
                    Description = "Commemorates India's independence from British rule",
                    Type = HolidayType.Government,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Holiday
                {
                    Id = Guid.NewGuid(),
                    Name = "Gandhi Jayanti",
                    Date = new DateTime(2025, 10, 2),
                    IsRecurringYearly = true,
                    Description = "Birth anniversary of Mahatma Gandhi, Father of the Nation",
                    Type = HolidayType.Government,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                
                // Labour Day
                new Holiday
                {
                    Id = Guid.NewGuid(),
                    Name = "Labour Day",
                    Date = new DateTime(2025, 5, 1),
                    IsRecurringYearly = true,
                    Description = "International Workers' Day celebrating the achievements of workers",
                    Type = HolidayType.Government,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                
                // Religious Holidays
                new Holiday
                {
                    Id = Guid.NewGuid(),
                    Name = "Ram Navami",
                    Date = new DateTime(2025, 4, 17), // Date for 2025
                    IsRecurringYearly = true,
                    Description = "Celebrates the birth of Lord Rama, the seventh avatar of Vishnu",
                    Type = HolidayType.Government,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Holiday
                {
                    Id = Guid.NewGuid(),
                    Name = "Holi",
                    Date = new DateTime(2025, 3, 14), // Date for 2025
                    IsRecurringYearly = true,
                    Description = "Festival of colors celebrating the arrival of spring",
                    Type = HolidayType.Government,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Holiday
                {
                    Id = Guid.NewGuid(),
                    Name = "Diwali",
                    Date = new DateTime(2025, 11, 12), // Date for 2025
                    IsRecurringYearly = true,
                    Description = "Festival of lights celebrating the victory of light over darkness",
                    Type = HolidayType.Government,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Holiday
                {
                    Id = Guid.NewGuid(),
                    Name = "Eid al-Fitr",
                    Date = new DateTime(2025, 5, 2), // Date for 2025
                    IsRecurringYearly = true,
                    Description = "Marks the end of Ramadan, the Islamic holy month of fasting",
                    Type = HolidayType.Government,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Holiday
                {
                    Id = Guid.NewGuid(),
                    Name = "Christmas",
                    Date = new DateTime(2025, 12, 25),
                    IsRecurringYearly = true,
                    Description = "Celebrates the birth of Jesus Christ",
                    Type = HolidayType.Government,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                
                // Other Important Holidays
                new Holiday
                {
                    Id = Guid.NewGuid(),
                    Name = "Ambedkar Jayanti",
                    Date = new DateTime(2025, 4, 14),
                    IsRecurringYearly = true,
                    Description = "Birth anniversary of Dr. B.R. Ambedkar, the principal architect of the Indian Constitution",
                    Type = HolidayType.Government,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Holiday
                {
                    Id = Guid.NewGuid(),
                    Name = "Raksha Bandhan",
                    Date = new DateTime(2025, 8, 10), // Date for 2025
                    IsRecurringYearly = true,
                    Description = "Celebrates the bond between brothers and sisters",
                    Type = HolidayType.Optional,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                
                // Company Holidays
                new Holiday
                {
                    Id = Guid.NewGuid(),
                    Name = "Company Foundation Day",
                    Date = new DateTime(2025, 6, 15),
                    IsRecurringYearly = true,
                    Description = "Celebrates the founding of our company",
                    Type = HolidayType.Company,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Holiday
                {
                    Id = Guid.NewGuid(),
                    Name = "Annual Team Building Day",
                    Date = new DateTime(2025, 9, 5),
                    IsRecurringYearly = true,
                    Description = "Day dedicated to team building activities and company culture",
                    Type = HolidayType.Company,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Holiday
                {
                    Id = Guid.NewGuid(),
                    Name = "Year-End Celebration",
                    Date = new DateTime(2025, 12, 30),
                    IsRecurringYearly = true,
                    Description = "Year-end celebration and recognition event",
                    Type = HolidayType.Company,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            context.Holidays.AddRange(holidays);
            context.SaveChanges();
        }
    }
}
