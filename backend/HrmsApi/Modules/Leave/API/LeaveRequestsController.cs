using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrmsApi.Data;
using HrmsApi.Modules.Leave.Domain;
using HrmsApi.Modules.Leave.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace HrmsApi.Modules.Leave.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly HrmsDbContext _context;

        public LeaveRequestsController(HrmsDbContext context)
        {
            _context = context;
        }

        // GET: api/LeaveRequests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveRequest>>> GetLeaveRequests()
        {
            return await _context.LeaveRequests
                .Include(l => l.Employee)
                .ToListAsync();
        }

        // GET: api/LeaveRequests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveRequest>> GetLeaveRequest(Guid id)
        {
            var leaveRequest = await _context.LeaveRequests
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leaveRequest == null)
            {
                return NotFound();
            }

            return leaveRequest;
        }

        // GET: api/LeaveRequests/Employee/5
        [HttpGet("Employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<LeaveRequest>>> GetLeaveRequestsByEmployee(Guid employeeId)
        {
            return await _context.LeaveRequests
                .Where(l => l.EmployeeId == employeeId)
                .ToListAsync();
        }

        // GET: api/LeaveRequests/Status/Pending
        [HttpGet("Status/{status}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<LeaveRequest>>> GetLeaveRequestsByStatus(string status)
        {
            return await _context.LeaveRequests
                .Where(l => l.Status.ToLower() == status.ToLower())
                .Include(l => l.Employee)
                .ToListAsync();
        }

        // POST: api/LeaveRequests
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<LeaveRequest>> PostLeaveRequest(CreateLeaveRequestDTO leaveRequestDto)
        {
            try
            {
                // Check if employee exists
                var employee = await _context.Employees.FindAsync(leaveRequestDto.EmployeeId);
                if (employee == null)
                {
                    return BadRequest(new { message = "Employee not found" });
                }

                // Allow same day leave requests (start date can equal end date)
                if (leaveRequestDto.StartDate > leaveRequestDto.EndDate)
                {
                    return BadRequest(new { message = "Start date must be before or equal to end date" });
                }

                var leaveRequest = new LeaveRequest
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = leaveRequestDto.EmployeeId,
                    StartDate = DateTime.SpecifyKind(leaveRequestDto.StartDate, DateTimeKind.Utc),
                    EndDate = DateTime.SpecifyKind(leaveRequestDto.EndDate, DateTimeKind.Utc),
                    LeaveType = leaveRequestDto.LeaveType,
                    Reason = leaveRequestDto.Reason,
                    Status = "Pending",
                    RequestDate = DateTime.UtcNow
                };
                
                _context.LeaveRequests.Add(leaveRequest);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetLeaveRequest), new { id = leaveRequest.Id }, leaveRequest);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        // PUT: api/LeaveRequests/5
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> PutLeaveRequest(Guid id, LeaveRequest leaveRequest)
        {
            if (id != leaveRequest.Id)
            {
                return BadRequest();
            }

            _context.Entry(leaveRequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeaveRequestExists(id))
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

        // PUT: api/LeaveRequests/Approve/5
        [HttpPut("Approve/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveLeaveRequest(Guid id, [FromBody] ApprovalRequest request)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null)
            {
                return NotFound();
            }

            leaveRequest.Status = "Approved";
            leaveRequest.ApprovedBy = request.ApprovedBy;
            leaveRequest.ApprovalDate = DateTime.UtcNow;
            leaveRequest.Comments = request.Comments;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/LeaveRequests/Reject/5
        [HttpPut("Reject/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectLeaveRequest(Guid id, [FromBody] ApprovalRequest request)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null)
            {
                return NotFound();
            }

            leaveRequest.Status = "Rejected";
            leaveRequest.ApprovedBy = request.ApprovedBy;
            leaveRequest.ApprovalDate = DateTime.UtcNow;
            leaveRequest.Comments = request.Comments;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/LeaveRequests/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<LeaveRequest>> DeleteLeaveRequest(Guid id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null)
            {
                return NotFound();
            }

            _context.LeaveRequests.Remove(leaveRequest);
            await _context.SaveChangesAsync();

            return leaveRequest;
        }

        private bool LeaveRequestExists(Guid id)
        {
            return _context.LeaveRequests.Any(e => e.Id == id);
        }
    }

    public class ApprovalRequest
    {
        public string ApprovedBy { get; set; } = string.Empty;
        public string? Comments { get; set; }
    }
}
