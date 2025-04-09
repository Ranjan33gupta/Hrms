using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrmsApi.Data;
using HrmsApi.Modules.Employee.Domain;
using HrmsApi.Modules.Employee.Application.DTOs;
using HrmsApi.Modules.Employee.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace HrmsApi.Modules.Employee.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeHistoriesController : ControllerBase
    {
        private readonly HrmsDbContext _context;
        private readonly IEmployeeHistoryRepository _employeeHistoryRepository;

        public EmployeeHistoriesController(HrmsDbContext context, IEmployeeHistoryRepository employeeHistoryRepository)
        {
            _context = context;
            _employeeHistoryRepository = employeeHistoryRepository;
        }

        private EmployeeHistoryDTO MapToDto(EmployeeHistory history)
        {
            var dto = new EmployeeHistoryDTO
            {
                Id = history.Id,
                EmployeeId = history.EmployeeId,
                EmployeeName = history.EmployeeName,
                EmployeeChangeDetails = history.EmployeeChangeDetails?.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Select(detail => new EmployeeChangeDetailDTO
                    {
                        Action = detail.Action,
                        FieldChanged = detail.FieldChanged,
                        OldValue = detail.OldValue,
                        NewValue = detail.NewValue,
                        Timestamp = detail.Timestamp
                    }).ToList()
                )
            };
            return dto;
        }

        // GET: api/EmployeeHistories
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<EmployeeHistoryDTO>>> GetEmployeeHistories()
        {
            var histories = await _employeeHistoryRepository.GetAllAsync();
            var historyDtos = new List<EmployeeHistoryDTO>();

            foreach (var history in histories)
            {
                historyDtos.Add(MapToDto(history));
            }

            return historyDtos;
        }

        // GET: api/EmployeeHistories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeHistoryDTO>> GetEmployeeHistory(Guid id)
        {
            var employeeHistory = await _employeeHistoryRepository.GetByIdAsync(id);

            if (employeeHistory == null)
            {
                return NotFound();
            }

            var historyDto = MapToDto(employeeHistory);

            return historyDto;
        }

        // GET: api/EmployeeHistories/employee/5
        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<EmployeeHistoryDTO>>> GetEmployeeHistoriesByEmployeeId(Guid employeeId)
        {
            var histories = await _employeeHistoryRepository.GetByEmployeeIdAsync(employeeId);
            var historyDtos = new List<EmployeeHistoryDTO>();

            foreach (var history in histories)
            {
                historyDtos.Add(MapToDto(history));
            }

            return historyDtos;
        }

        // GET: api/EmployeeHistories/employee/5/flattened
        [HttpGet("employee/{employeeId}/flattened")]
        public async Task<ActionResult<IEnumerable<FlattenedEmployeeHistoryDTO>>> GetFlattenedEmployeeHistoriesByEmployeeId(Guid employeeId)
        {
            try
            {
                var histories = await _employeeHistoryRepository.GetFlattenedHistoryByEmployeeIdAsync(employeeId);
                return Ok(histories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/EmployeeHistories/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<FlattenedEmployeeHistoryDTO>>> SearchEmployeeHistories(
            [FromQuery] Guid? employeeId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? actionType = null,
            [FromQuery] string? fieldName = null)
        {
            try
            {
                var histories = await _employeeHistoryRepository.SearchHistoryAsync(
                    employeeId,
                    startDate,
                    endDate,
                    actionType,
                    fieldName);

                return Ok(histories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/EmployeeHistories/log
        [HttpPost("log")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> AddHistoryLog(Guid employeeId, [FromBody] EmployeeChangeDetail logEntry)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            var result = await _employeeHistoryRepository.AddHistoryLogAsync(employeeId, employee.FullName, logEntry);

            if (result)
            {
                return Ok(true);
            }

            return BadRequest("Failed to add history log");
        }
    }
}
