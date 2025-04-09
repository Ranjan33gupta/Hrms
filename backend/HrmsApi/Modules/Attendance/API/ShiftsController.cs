using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrmsApi.Data;
using HrmsApi.Modules.Attendance.Domain;
using HrmsApi.Modules.Attendance.Application.DTOs;
using HrmsApi.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HrmsApi.Modules.Attendance.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftsController : ControllerBase
    {
        private readonly HrmsDbContext _context;

        public ShiftsController(HrmsDbContext context)
        {
            _context = context;
        }

        // GET: api/Shifts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShiftDTO>>> GetShifts()
        {
            var shifts = await _context.Set<Shift>().ToListAsync();
            return shifts.Select(s => new ShiftDTO
            {
                Id = s.Id,
                Name = s.Name,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                GracePeriod = s.GracePeriod,
                IsNightShift = s.IsNightShift,
                Description = s.Description,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            }).ToList();
        }

        // GET: api/Shifts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShiftDTO>> GetShift(Guid id)
        {
            var shift = await _context.Set<Shift>().FindAsync(id);

            if (shift == null)
            {
                return NotFound();
            }

            return new ShiftDTO
            {
                Id = shift.Id,
                Name = shift.Name,
                StartTime = shift.StartTime,
                EndTime = shift.EndTime,
                GracePeriod = shift.GracePeriod,
                IsNightShift = shift.IsNightShift,
                Description = shift.Description,
                IsActive = shift.IsActive,
                CreatedAt = shift.CreatedAt,
                UpdatedAt = shift.UpdatedAt
            };
        }

        // POST: api/Shifts
        [HttpPost]
        [Authorize("Admin")]
        public async Task<ActionResult<ShiftDTO>> CreateShift(CreateShiftDTO createShiftDTO)
        {
            var shift = new Shift
            {
                Id = Guid.NewGuid(),
                Name = createShiftDTO.Name,
                StartTime = createShiftDTO.StartTime,
                EndTime = createShiftDTO.EndTime,
                GracePeriod = createShiftDTO.GracePeriod,
                IsNightShift = createShiftDTO.IsNightShift,
                Description = createShiftDTO.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            _context.Set<Shift>().Add(shift);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetShift), new { id = shift.Id }, new ShiftDTO
            {
                Id = shift.Id,
                Name = shift.Name,
                StartTime = shift.StartTime,
                EndTime = shift.EndTime,
                GracePeriod = shift.GracePeriod,
                IsNightShift = shift.IsNightShift,
                Description = shift.Description,
                IsActive = shift.IsActive,
                CreatedAt = shift.CreatedAt,
                UpdatedAt = shift.UpdatedAt
            });
        }

        // PUT: api/Shifts/5
        [HttpPut("{id}")]
        [Authorize("Admin")]
        public async Task<IActionResult> UpdateShift(Guid id, UpdateShiftDTO updateShiftDTO)
        {
            var shift = await _context.Set<Shift>().FindAsync(id);
            if (shift == null)
            {
                return NotFound();
            }

            shift.Name = updateShiftDTO.Name;
            shift.StartTime = updateShiftDTO.StartTime;
            shift.EndTime = updateShiftDTO.EndTime;
            shift.GracePeriod = updateShiftDTO.GracePeriod;
            shift.IsNightShift = updateShiftDTO.IsNightShift;
            shift.Description = updateShiftDTO.Description;
            shift.IsActive = updateShiftDTO.IsActive;
            shift.UpdatedAt = DateTime.UtcNow;
            shift.UpdatedBy = "System";

            _context.Entry(shift).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShiftExists(id))
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

        // DELETE: api/Shifts/5
        [HttpDelete("{id}")]
        [Authorize("Admin")]
        public async Task<IActionResult> DeleteShift(Guid id)
        {
            var shift = await _context.Set<Shift>().FindAsync(id);
            if (shift == null)
            {
                return NotFound();
            }

            // Check if shift is being used by any attendance record
            var inUse = await _context.Attendances.AnyAsync(a => a.ShiftId == id);
            if (inUse)
            {
                return BadRequest("Cannot delete shift as it is being used by attendance records. Consider deactivating it instead.");
            }

            _context.Set<Shift>().Remove(shift);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ShiftExists(Guid id)
        {
            return _context.Set<Shift>().Any(e => e.Id == id);
        }
    }
}
