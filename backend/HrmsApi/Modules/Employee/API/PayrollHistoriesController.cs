using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Mapster;
using HrmsApi.Modules.Employee.Application.DTOs;
using HrmsApi.Modules.Employee.Application.Interfaces;
using HrmsApi.Modules.Employee.Domain;

namespace HrmsApi.Modules.Employee.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PayrollHistoriesController : ControllerBase
    {
        private readonly IPayrollHistoryRepository _payrollHistoryRepository;
        private readonly ILogger<PayrollHistoriesController> _logger;

        public PayrollHistoriesController(
            IPayrollHistoryRepository payrollHistoryRepository,
            ILogger<PayrollHistoriesController> logger)
        {
            _payrollHistoryRepository = payrollHistoryRepository;
            _logger = logger;
        }

        // GET: api/PayrollHistories
        [HttpGet]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<IEnumerable<PayrollHistoryDTO>>> GetPayrollHistories()
        {
            try
            {
                var payrollHistories = await _payrollHistoryRepository.GetAllPayrollHistoriesAsync();
                return Ok(payrollHistories.Adapt<IEnumerable<PayrollHistoryDTO>>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all payroll histories");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/PayrollHistories/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<PayrollHistoryDTO>> GetPayrollHistory(Guid id)
        {
            try
            {
                var payrollHistory = await _payrollHistoryRepository.GetPayrollHistoryByIdAsync(id);

                if (payrollHistory == null)
                {
                    return NotFound();
                }

                return Ok(payrollHistory.Adapt<PayrollHistoryDTO>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting payroll history with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/PayrollHistories/employee/5
        [HttpGet("employee/{employeeId}")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<IEnumerable<PayrollHistoryDTO>>> GetPayrollHistoriesByEmployee(Guid employeeId)
        {
            try
            {
                var payrollHistories = await _payrollHistoryRepository.GetPayrollHistoriesByEmployeeIdAsync(employeeId);
                return Ok(payrollHistories.Adapt<IEnumerable<PayrollHistoryDTO>>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting payroll histories for employee ID {employeeId}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/PayrollHistories/payroll/5
        [HttpGet("payroll/{payrollId}")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<IEnumerable<PayrollHistoryDTO>>> GetPayrollHistoriesByPayroll(Guid payrollId)
        {
            try
            {
                var payrollHistories = await _payrollHistoryRepository.GetPayrollHistoriesByPayrollIdAsync(payrollId);
                return Ok(payrollHistories.Adapt<IEnumerable<PayrollHistoryDTO>>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting payroll histories for payroll ID {payrollId}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/PayrollHistories
        [HttpPost]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<PayrollHistoryDTO>> CreatePayrollHistory(CreatePayrollHistoryDTO createPayrollHistoryDto)
        {
            try
            {
                // Create a new PayrollHistory
                var payrollHistory = new PayrollHistory
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = createPayrollHistoryDto.EmployeeId,
                    PayrollId = createPayrollHistoryDto.PayrollId,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                    CreatedBy = User.Identity?.Name
                };

                // Ensure the timestamp is UTC
                if (createPayrollHistoryDto.PayrollChange.Timestamp != default)
                {
                    createPayrollHistoryDto.PayrollChange.Timestamp = DateTime.SpecifyKind(createPayrollHistoryDto.PayrollChange.Timestamp, DateTimeKind.Utc);
                }

                // Use the helper method to add changes with proper DateTime handling
                payrollHistory.AddChanges(DateTime.UtcNow, new List<PayrollChangeDetail> { createPayrollHistoryDto.PayrollChange });

                var result = await _payrollHistoryRepository.AddPayrollHistoryAsync(payrollHistory);
                return CreatedAtAction(nameof(GetPayrollHistory), new { id = result.Id }, result.Adapt<PayrollHistoryDTO>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payroll history");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/PayrollHistories/5/changes
        [HttpPut("{id}/changes")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> AddPayrollChange(Guid id, [FromBody] PayrollChangeDetail payrollChange)
        {
            try
            {
                var payrollHistory = await _payrollHistoryRepository.GetPayrollHistoryByIdAsync(id);
                if (payrollHistory == null)
                {
                    return NotFound();
                }

                // Ensure the timestamp is UTC
                if (payrollChange.Timestamp != default)
                {
                    payrollChange.Timestamp = DateTime.SpecifyKind(payrollChange.Timestamp, DateTimeKind.Utc);
                }

                // Use the helper method to add changes with proper DateTime handling
                var now = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                payrollHistory.AddChanges(now, new List<PayrollChangeDetail> { payrollChange });

                // Update audit fields
                payrollHistory.UpdatedAt = now;
                payrollHistory.UpdatedBy = User.Identity?.Name;

                await _payrollHistoryRepository.UpdatePayrollHistoryAsync(payrollHistory);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding payroll change to history with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/PayrollHistories/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePayrollHistory(Guid id)
        {
            try
            {
                var result = await _payrollHistoryRepository.DeletePayrollHistoryAsync(id);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting payroll history with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
