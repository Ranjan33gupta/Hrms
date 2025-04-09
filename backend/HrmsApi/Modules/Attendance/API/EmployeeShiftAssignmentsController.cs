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
    public class EmployeeShiftAssignmentsController : ControllerBase
    {
        private readonly HrmsDbContext _context;

        public EmployeeShiftAssignmentsController(HrmsDbContext context)
        {
            _context = context;
        }

        // GET: api/EmployeeShiftAssignments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeShiftAssignmentDTO>>> GetEmployeeShiftAssignments()
        {
            var assignments = await _context.Set<EmployeeShiftAssignment>()
                .Include(e => e.Shift)
                .ToListAsync();

            var employees = await _context.Employees.ToListAsync();

            return assignments.Select(a => new EmployeeShiftAssignmentDTO
            {
                Id = a.Id,
                EmployeeId = a.EmployeeId,
                EmployeeName = employees.FirstOrDefault(e => e.Id == a.EmployeeId)?.FullName ?? "Unknown",
                ShiftId = a.ShiftId,
                ShiftName = a.Shift?.Name ?? "Unknown",
                EffectiveFrom = a.EffectiveFrom,
                EffectiveTo = a.EffectiveTo,
                IsActive = a.IsActive,
                ShiftStartTime = a.Shift?.StartTime ?? TimeSpan.Zero,
                ShiftEndTime = a.Shift?.EndTime ?? TimeSpan.Zero
            }).ToList();
        }

        // GET: api/EmployeeShiftAssignments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeShiftAssignmentDTO>> GetEmployeeShiftAssignment(Guid id)
        {
            var assignment = await _context.Set<EmployeeShiftAssignment>()
                .Include(e => e.Shift)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (assignment == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(assignment.EmployeeId);

            return new EmployeeShiftAssignmentDTO
            {
                Id = assignment.Id,
                EmployeeId = assignment.EmployeeId,
                EmployeeName = employee?.FullName ?? "Unknown",
                ShiftId = assignment.ShiftId,
                ShiftName = assignment.Shift?.Name ?? "Unknown",
                EffectiveFrom = assignment.EffectiveFrom,
                EffectiveTo = assignment.EffectiveTo,
                IsActive = assignment.IsActive,
                ShiftStartTime = assignment.Shift?.StartTime ?? TimeSpan.Zero,
                ShiftEndTime = assignment.Shift?.EndTime ?? TimeSpan.Zero
            };
        }

        // GET: api/EmployeeShiftAssignments/Employee/5
        [HttpGet("Employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<EmployeeShiftAssignmentDTO>>> GetEmployeeShiftAssignmentsByEmployee(Guid employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            var assignments = await _context.Set<EmployeeShiftAssignment>()
                .Include(e => e.Shift)
                .Where(e => e.EmployeeId == employeeId)
                .ToListAsync();

            return assignments.Select(a => new EmployeeShiftAssignmentDTO
            {
                Id = a.Id,
                EmployeeId = a.EmployeeId,
                EmployeeName = employee.FullName,
                ShiftId = a.ShiftId,
                ShiftName = a.Shift?.Name ?? "Unknown",
                EffectiveFrom = a.EffectiveFrom,
                EffectiveTo = a.EffectiveTo,
                IsActive = a.IsActive,
                ShiftStartTime = a.Shift?.StartTime ?? TimeSpan.Zero,
                ShiftEndTime = a.Shift?.EndTime ?? TimeSpan.Zero
            }).ToList();
        }

        // GET: api/EmployeeShiftAssignments/Employee/5/Current
        [HttpGet("Employee/{employeeId}/Current")]
        public async Task<ActionResult<EmployeeShiftAssignmentDTO>> GetCurrentEmployeeShiftAssignment(Guid employeeId)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(employeeId);
                if (employee == null)
                {
                    return NotFound("Employee not found");
                }

                // Convert local DateTime to UTC for PostgreSQL compatibility
                var today = DateTime.UtcNow.Date;
                
                var assignment = await _context.Set<EmployeeShiftAssignment>()
                    .Include(e => e.Shift)
                    .Where(e => e.EmployeeId == employeeId && 
                               e.EffectiveFrom <= today && 
                               (e.EffectiveTo == null || e.EffectiveTo >= today))
                    .OrderByDescending(e => e.EffectiveFrom)
                    .FirstOrDefaultAsync();

                if (assignment == null)
                {
                    return NotFound("No active shift assignment found for this employee");
                }

                return new EmployeeShiftAssignmentDTO
                {
                    Id = assignment.Id,
                    EmployeeId = assignment.EmployeeId,
                    EmployeeName = employee.FullName,
                    ShiftId = assignment.ShiftId,
                    ShiftName = assignment.Shift?.Name ?? "Unknown",
                    EffectiveFrom = assignment.EffectiveFrom,
                    EffectiveTo = assignment.EffectiveTo,
                    IsActive = assignment.IsActive,
                    ShiftStartTime = assignment.Shift?.StartTime ?? TimeSpan.Zero,
                    ShiftEndTime = assignment.Shift?.EndTime ?? TimeSpan.Zero
                };
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error getting current shift assignment: {ex.Message}");
                Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/EmployeeShiftAssignments
        [HttpPost]
        [Authorize("Admin")]
        public async Task<ActionResult<EmployeeShiftAssignmentDTO>> CreateEmployeeShiftAssignment(CreateEmployeeShiftAssignmentDTO createDTO)
        {
            var employee = await _context.Employees.FindAsync(createDTO.EmployeeId);
            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            var shift = await _context.Set<Shift>().FindAsync(createDTO.ShiftId);
            if (shift == null)
            {
                return NotFound("Shift not found");
            }

            // Check if there's an active assignment for this employee
            if (createDTO.EffectiveTo == null)
            {
                // If this is an ongoing assignment, deactivate any current assignments
                var currentAssignments = await _context.Set<EmployeeShiftAssignment>()
                    .Where(e => e.EmployeeId == createDTO.EmployeeId && e.EffectiveTo == null)
                    .ToListAsync();

                foreach (var assignment in currentAssignments)
                {
                    assignment.EffectiveTo = createDTO.EffectiveFrom.AddDays(-1);
                    assignment.UpdatedAt = DateTime.UtcNow;
                    assignment.UpdatedBy = "System";
                    _context.Entry(assignment).State = EntityState.Modified;
                }
            }

            var employeeShiftAssignment = new EmployeeShiftAssignment
            {
                Id = Guid.NewGuid(),
                EmployeeId = createDTO.EmployeeId,
                ShiftId = createDTO.ShiftId,
                EffectiveFrom = createDTO.EffectiveFrom,
                EffectiveTo = createDTO.EffectiveTo,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            _context.Set<EmployeeShiftAssignment>().Add(employeeShiftAssignment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployeeShiftAssignment), 
                new { id = employeeShiftAssignment.Id }, 
                new EmployeeShiftAssignmentDTO
                {
                    Id = employeeShiftAssignment.Id,
                    EmployeeId = employeeShiftAssignment.EmployeeId,
                    EmployeeName = employee.FullName,
                    ShiftId = employeeShiftAssignment.ShiftId,
                    ShiftName = shift.Name,
                    EffectiveFrom = employeeShiftAssignment.EffectiveFrom,
                    EffectiveTo = employeeShiftAssignment.EffectiveTo,
                    IsActive = employeeShiftAssignment.IsActive,
                    ShiftStartTime = shift.StartTime,
                    ShiftEndTime = shift.EndTime
                });
        }

        // PUT: api/EmployeeShiftAssignments/5
        [HttpPut("{id}")]
        [Authorize("Admin")]
        public async Task<IActionResult> UpdateEmployeeShiftAssignment(Guid id, UpdateEmployeeShiftAssignmentDTO updateDTO)
        {
            var assignment = await _context.Set<EmployeeShiftAssignment>().FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            var shift = await _context.Set<Shift>().FindAsync(updateDTO.ShiftId);
            if (shift == null)
            {
                return NotFound("Shift not found");
            }

            assignment.ShiftId = updateDTO.ShiftId;
            assignment.EffectiveFrom = updateDTO.EffectiveFrom;
            assignment.EffectiveTo = updateDTO.EffectiveTo;
            assignment.UpdatedAt = DateTime.UtcNow;
            assignment.UpdatedBy = "System";

            _context.Entry(assignment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeShiftAssignmentExists(id))
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

        // DELETE: api/EmployeeShiftAssignments/5
        [HttpDelete("{id}")]
        [Authorize("Admin")]
        public async Task<IActionResult> DeleteEmployeeShiftAssignment(Guid id)
        {
            var assignment = await _context.Set<EmployeeShiftAssignment>().FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            _context.Set<EmployeeShiftAssignment>().Remove(assignment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeShiftAssignmentExists(Guid id)
        {
            return _context.Set<EmployeeShiftAssignment>().Any(e => e.Id == id);
        }
    }
}
