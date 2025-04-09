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
    public class LeavePoliciesController : ControllerBase
    {
        private readonly HrmsDbContext _context;

        public LeavePoliciesController(HrmsDbContext context)
        {
            _context = context;
        }

        // GET: api/LeavePolicies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeavePolicyDTO>>> GetLeavePolicies()
        {
            var policies = await _context.LeavePolicies.ToListAsync();
            
            return policies.Select(p => new LeavePolicyDTO
            {
                Id = p.Id,
                Name = p.Name,
                LeaveType = p.LeaveType,
                DaysAllowed = p.DaysAllowed,
                IsCarryForward = p.IsCarryForward,
                MaxCarryForwardDays = p.MaxCarryForwardDays,
                RequiresApproval = p.RequiresApproval,
                MinDaysNotice = p.MinDaysNotice,
                IsActive = p.IsActive,
                Description = p.Description
            }).ToList();
        }

        // GET: api/LeavePolicies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LeavePolicyDTO>> GetLeavePolicy(Guid id)
        {
            var leavePolicy = await _context.LeavePolicies.FindAsync(id);

            if (leavePolicy == null)
            {
                return NotFound();
            }

            return new LeavePolicyDTO
            {
                Id = leavePolicy.Id,
                Name = leavePolicy.Name,
                LeaveType = leavePolicy.LeaveType,
                DaysAllowed = leavePolicy.DaysAllowed,
                IsCarryForward = leavePolicy.IsCarryForward,
                MaxCarryForwardDays = leavePolicy.MaxCarryForwardDays,
                RequiresApproval = leavePolicy.RequiresApproval,
                MinDaysNotice = leavePolicy.MinDaysNotice,
                IsActive = leavePolicy.IsActive,
                Description = leavePolicy.Description
            };
        }

        // POST: api/LeavePolicies
        [HttpPost]
        [Authorize("Admin")]
        public async Task<ActionResult<LeavePolicyDTO>> CreateLeavePolicy(CreateLeavePolicyDTO createLeavePolicyDto)
        {
            try
            {
                var leavePolicy = new LeavePolicy
                {
                    Id = Guid.NewGuid(),
                    Name = createLeavePolicyDto.Name,
                    LeaveType = createLeavePolicyDto.LeaveType,
                    DaysAllowed = createLeavePolicyDto.DaysAllowed,
                    IsCarryForward = createLeavePolicyDto.IsCarryForward,
                    MaxCarryForwardDays = createLeavePolicyDto.MaxCarryForwardDays,
                    RequiresApproval = createLeavePolicyDto.RequiresApproval,
                    MinDaysNotice = createLeavePolicyDto.MinDaysNotice,
                    IsActive = true,
                    Description = createLeavePolicyDto.Description,
                    CreatedAt = DateTime.UtcNow
                };

                _context.LeavePolicies.Add(leavePolicy);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetLeavePolicy), new { id = leavePolicy.Id }, new LeavePolicyDTO
                {
                    Id = leavePolicy.Id,
                    Name = leavePolicy.Name,
                    LeaveType = leavePolicy.LeaveType,
                    DaysAllowed = leavePolicy.DaysAllowed,
                    IsCarryForward = leavePolicy.IsCarryForward,
                    MaxCarryForwardDays = leavePolicy.MaxCarryForwardDays,
                    RequiresApproval = leavePolicy.RequiresApproval,
                    MinDaysNotice = leavePolicy.MinDaysNotice,
                    IsActive = leavePolicy.IsActive,
                    Description = leavePolicy.Description
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        // PUT: api/LeavePolicies/5
        [HttpPut("{id}")]
        [Authorize("Admin")]
        public async Task<IActionResult> UpdateLeavePolicy(Guid id, UpdateLeavePolicyDTO updateLeavePolicyDto)
        {
            if (id != updateLeavePolicyDto.Id)
            {
                return BadRequest();
            }

            var leavePolicy = await _context.LeavePolicies.FindAsync(id);
            if (leavePolicy == null)
            {
                return NotFound();
            }

            leavePolicy.Name = updateLeavePolicyDto.Name;
            leavePolicy.LeaveType = updateLeavePolicyDto.LeaveType;
            leavePolicy.DaysAllowed = updateLeavePolicyDto.DaysAllowed;
            leavePolicy.IsCarryForward = updateLeavePolicyDto.IsCarryForward;
            leavePolicy.MaxCarryForwardDays = updateLeavePolicyDto.MaxCarryForwardDays;
            leavePolicy.RequiresApproval = updateLeavePolicyDto.RequiresApproval;
            leavePolicy.MinDaysNotice = updateLeavePolicyDto.MinDaysNotice;
            leavePolicy.IsActive = updateLeavePolicyDto.IsActive;
            leavePolicy.Description = updateLeavePolicyDto.Description;
            leavePolicy.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeavePolicyExists(id))
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

        // DELETE: api/LeavePolicies/5
        [HttpDelete("{id}")]
        [Authorize("Admin")]
        public async Task<IActionResult> DeleteLeavePolicy(Guid id)
        {
            var leavePolicy = await _context.LeavePolicies.FindAsync(id);
            if (leavePolicy == null)
            {
                return NotFound();
            }

            // Soft delete
            leavePolicy.IsActive = false;
            leavePolicy.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LeavePolicyExists(Guid id)
        {
            return _context.LeavePolicies.Any(e => e.Id == id);
        }
    }
}
