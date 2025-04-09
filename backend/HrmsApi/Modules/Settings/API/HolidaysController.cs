using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrmsApi.Data;
using HrmsApi.Modules.Settings.Domain;
using HrmsApi.Modules.Settings.Application.DTOs;
using HrmsApi.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HrmsApi.Modules.Settings.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class HolidaysController : ControllerBase
    {
        private readonly HrmsDbContext _context;

        public HolidaysController(HrmsDbContext context)
        {
            _context = context;
        }

        // GET: api/Holidays
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HolidayDTO>>> GetHolidays()
        {
            var holidays = await _context.Holidays.ToListAsync();
            
            return holidays.Select(h => new HolidayDTO
            {
                Id = h.Id,
                Name = h.Name,
                Date = h.Date,
                IsRecurringYearly = h.IsRecurringYearly,
                Description = h.Description,
                Type = h.Type,
                IsActive = h.IsActive
            }).ToList();
        }

        // GET: api/Holidays/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HolidayDTO>> GetHoliday(Guid id)
        {
            var holiday = await _context.Holidays.FindAsync(id);

            if (holiday == null)
            {
                return NotFound();
            }

            return new HolidayDTO
            {
                Id = holiday.Id,
                Name = holiday.Name,
                Date = holiday.Date,
                IsRecurringYearly = holiday.IsRecurringYearly,
                Description = holiday.Description,
                Type = holiday.Type,
                IsActive = holiday.IsActive
            };
        }

        // GET: api/Holidays/Year/2025
        [HttpGet("Year/{year}")]
        public async Task<ActionResult<IEnumerable<HolidayDTO>>> GetHolidaysByYear(int year)
        {
            var startDate = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var endDate = new DateTime(year, 12, 31, 23, 59, 59, DateTimeKind.Utc);

            var holidays = await _context.Holidays
                .Where(h => h.IsActive && ((h.Date >= startDate && h.Date <= endDate) || h.IsRecurringYearly))
                .ToListAsync();

            var result = new List<HolidayDTO>();
            
            foreach (var holiday in holidays)
            {
                var holidayDto = new HolidayDTO
                {
                    Id = holiday.Id,
                    Name = holiday.Name,
                    Description = holiday.Description,
                    Type = holiday.Type,
                    IsActive = holiday.IsActive,
                    IsRecurringYearly = holiday.IsRecurringYearly
                };
                
                if (holiday.IsRecurringYearly && (holiday.Date.Year != year))
                {
                    // Adjust the date for recurring yearly holidays
                    try {
                        holidayDto.Date = new DateTime(year, holiday.Date.Month, holiday.Date.Day, 
                            holiday.Date.Hour, holiday.Date.Minute, holiday.Date.Second, DateTimeKind.Utc);
                    }
                    catch (ArgumentOutOfRangeException) {
                        // Handle Feb 29 in non-leap years
                        if (holiday.Date.Month == 2 && holiday.Date.Day == 29 && !DateTime.IsLeapYear(year)) {
                            holidayDto.Date = new DateTime(year, 2, 28, 
                                holiday.Date.Hour, holiday.Date.Minute, holiday.Date.Second, DateTimeKind.Utc);
                        }
                        else {
                            throw;
                        }
                    }
                }
                else
                {
                    holidayDto.Date = holiday.Date;
                }
                
                result.Add(holidayDto);
            }

            return result;
        }

        // GET: api/Holidays/Upcoming
        [HttpGet("Upcoming")]
        public async Task<ActionResult<IEnumerable<HolidayDTO>>> GetUpcomingHolidays()
        {
            var today = DateTime.UtcNow.Date;
            var endDate = today.AddDays(90); // Get holidays for the next 90 days
            var currentYear = today.Year;

            // Get all active holidays
            var holidays = await _context.Holidays
                .Where(h => h.IsActive)
                .ToListAsync();

            var result = new List<HolidayDTO>();
            
            foreach (var holiday in holidays)
            {
                DateTime effectiveDate;
                
                if (holiday.IsRecurringYearly)
                {
                    // For recurring holidays, check both this year and next year
                    try 
                    {
                        effectiveDate = new DateTime(currentYear, holiday.Date.Month, holiday.Date.Day);
                        
                        // If the date has already passed this year, use next year's date
                        if (effectiveDate < today)
                        {
                            effectiveDate = new DateTime(currentYear + 1, holiday.Date.Month, holiday.Date.Day);
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        // Handle Feb 29 in non-leap years
                        if (holiday.Date.Month == 2 && holiday.Date.Day == 29)
                        {
                            if (DateTime.IsLeapYear(currentYear))
                            {
                                effectiveDate = new DateTime(currentYear, 2, 29);
                            }
                            else
                            {
                                effectiveDate = new DateTime(currentYear, 2, 28);
                            }
                            
                            // If the date has already passed this year, use next year's date
                            if (effectiveDate < today)
                            {
                                if (DateTime.IsLeapYear(currentYear + 1))
                                {
                                    effectiveDate = new DateTime(currentYear + 1, 2, 29);
                                }
                                else
                                {
                                    effectiveDate = new DateTime(currentYear + 1, 2, 28);
                                }
                            }
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                else
                {
                    effectiveDate = holiday.Date;
                }
                
                // Only include if the holiday is in the next 90 days
                if (effectiveDate >= today && effectiveDate <= endDate)
                {
                    result.Add(new HolidayDTO
                    {
                        Id = holiday.Id,
                        Name = holiday.Name,
                        Date = effectiveDate,
                        IsRecurringYearly = holiday.IsRecurringYearly,
                        Description = holiday.Description,
                        Type = holiday.Type,
                        IsActive = holiday.IsActive
                    });
                }
            }
            
            // Sort by date
            return result.OrderBy(h => h.Date).ToList();
        }

        // POST: api/Holidays
        [HttpPost]
        [Authorize("Admin")]
        public async Task<ActionResult<HolidayDTO>> CreateHoliday(CreateHolidayDTO createHolidayDto)
        {
            try
            {
                var holiday = new Holiday
                {
                    Id = Guid.NewGuid(),
                    Name = createHolidayDto.Name,
                    Date = DateTime.SpecifyKind(createHolidayDto.Date, DateTimeKind.Utc),
                    IsRecurringYearly = createHolidayDto.IsRecurringYearly,
                    Description = createHolidayDto.Description,
                    Type = createHolidayDto.Type,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Holidays.Add(holiday);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetHoliday), new { id = holiday.Id }, new HolidayDTO
                {
                    Id = holiday.Id,
                    Name = holiday.Name,
                    Date = holiday.Date,
                    IsRecurringYearly = holiday.IsRecurringYearly,
                    Description = holiday.Description,
                    Type = holiday.Type,
                    IsActive = holiday.IsActive
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        // PUT: api/Holidays/5
        [HttpPut("{id}")]
        [Authorize("Admin")]
        public async Task<IActionResult> UpdateHoliday(Guid id, UpdateHolidayDTO updateHolidayDto)
        {
            if (id != updateHolidayDto.Id)
            {
                return BadRequest();
            }

            var holiday = await _context.Holidays.FindAsync(id);
            if (holiday == null)
            {
                return NotFound();
            }

            holiday.Name = updateHolidayDto.Name;
            holiday.Date = DateTime.SpecifyKind(updateHolidayDto.Date, DateTimeKind.Utc);
            holiday.IsRecurringYearly = updateHolidayDto.IsRecurringYearly;
            holiday.Description = updateHolidayDto.Description;
            holiday.Type = updateHolidayDto.Type;
            holiday.IsActive = updateHolidayDto.IsActive;
            holiday.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HolidayExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Holidays/5
        [HttpDelete("{id}")]
        [Authorize("Admin")]
        public async Task<IActionResult> DeleteHoliday(Guid id)
        {
            var holiday = await _context.Holidays.FindAsync(id);
            if (holiday == null)
            {
                return NotFound();
            }

            // Soft delete
            holiday.IsActive = false;
            holiday.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HolidayExists(Guid id)
        {
            return _context.Holidays.Any(e => e.Id == id);
        }
    }
}
