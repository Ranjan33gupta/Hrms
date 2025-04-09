using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrmsApi.Data;
using HrmsApi.Modules.Employee.Domain;
using HrmsApi.Modules.Employee.Application.DTOs;
using Mapster;
using Microsoft.AspNetCore.Authorization;

namespace HrmsApi.Modules.Employee.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PayrollsController : ControllerBase
    {
        private readonly HrmsDbContext _context;

        public PayrollsController(HrmsDbContext context)
        {
            _context = context;
        }

        // GET: api/Payrolls
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PayrollDTO>>> GetPayrolls()
        {
            var payrolls = await _context.Payrolls
                .Include(p => p.Employee)
                .ToListAsync();

            var payrollDtos = payrolls.Select(p => {
                var dto = p.Adapt<PayrollDTO>();
                dto.EmployeeName = p.Employee?.FullName ?? "Unknown";
                return dto;
            }).ToList();

            return payrollDtos;
        }

        // GET: api/Payrolls/employee/{employeeId}
        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<IEnumerable<PayrollDTO>>> GetPayrollsByEmployee(Guid employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            var payrolls = await _context.Payrolls
                .Where(p => p.EmployeeId == employeeId)
                .OrderByDescending(p => p.SalaryMonth)
                .ToListAsync();

            var payrollDtos = payrolls.Select(p => {
                var dto = p.Adapt<PayrollDTO>();
                dto.EmployeeName = employee.FullName;
                return dto;
            }).ToList();

            return payrollDtos;
        }

        // GET: api/Payrolls/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PayrollDTO>> GetPayroll(Guid id)
        {
            var payroll = await _context.Payrolls
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payroll == null)
            {
                return NotFound();
            }

            var payrollDto = payroll.Adapt<PayrollDTO>();
            payrollDto.EmployeeName = payroll.Employee?.FullName ?? "Unknown";

            return payrollDto;
        }

        // POST: api/Payrolls
        [HttpPost]
        public async Task<ActionResult<PayrollDTO>> CreatePayroll(PayrollDTO payrollDto)
        {
            // Check if employee exists
            var employee = await _context.Employees.FindAsync(payrollDto.EmployeeId);
            if (employee == null)
            {
                return BadRequest("Employee not found");
            }

            // Calculate Net Salary
            payrollDto.NetSalary = payrollDto.BasicSalary + payrollDto.HRA + payrollDto.Allowances - payrollDto.Deductions;

            var payroll = new Payroll
            {
                Id = Guid.NewGuid(),
                EmployeeId = payrollDto.EmployeeId.Value,
                BasicSalary = payrollDto.BasicSalary ?? 0,
                HRA = payrollDto.HRA ?? 0,
                Allowances = payrollDto.Allowances ?? 0,
                Deductions = payrollDto.Deductions ?? 0,
                NetSalary = payrollDto.NetSalary ?? 0,
                SalaryMonth = payrollDto.SalaryMonth.HasValue 
                    ? DateTime.SpecifyKind(payrollDto.SalaryMonth.Value, DateTimeKind.Utc)
                    : DateTime.SpecifyKind(new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1), DateTimeKind.Utc),
                PaymentDate = payrollDto.PaymentDate.HasValue
                    ? DateTime.SpecifyKind(payrollDto.PaymentDate.Value, DateTimeKind.Utc)
                    : DateTime.UtcNow
            };
            
            _context.Payrolls.Add(payroll);
            await _context.SaveChangesAsync();

            var resultDto = payroll.Adapt<PayrollDTO>();
            resultDto.EmployeeName = employee.FullName;

            return CreatedAtAction(nameof(GetPayroll), new { id = payroll.Id }, resultDto);
        }

        // PUT: api/Payrolls/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayroll(Guid id, PayrollDTO payrollDto)
        {
            if (id != payrollDto.Id)
            {
                return BadRequest();
            }

            var payroll = await _context.Payrolls.FindAsync(id);
            if (payroll == null)
            {
                return NotFound();
            }

            // Calculate Net Salary
            payrollDto.NetSalary = payrollDto.BasicSalary + payrollDto.HRA + payrollDto.Allowances - payrollDto.Deductions;

            // Only update the fields that exist in the database
            payroll.BasicSalary = payrollDto.BasicSalary ?? payroll.BasicSalary;
            payroll.HRA = payrollDto.HRA ?? payroll.HRA;
            payroll.Allowances = payrollDto.Allowances ?? payroll.Allowances;
            payroll.Deductions = payrollDto.Deductions ?? payroll.Deductions;
            payroll.NetSalary = payrollDto.NetSalary ?? payroll.NetSalary;
            
            if (payrollDto.SalaryMonth.HasValue)
            {
                payroll.SalaryMonth = DateTime.SpecifyKind(payrollDto.SalaryMonth.Value, DateTimeKind.Utc);
            }
            
            if (payrollDto.PaymentDate.HasValue)
            {
                payroll.PaymentDate = DateTime.SpecifyKind(payrollDto.PaymentDate.Value, DateTimeKind.Utc);
            }
            
            _context.Entry(payroll).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PayrollExists(id))
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

        // DELETE: api/Payrolls/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayroll(Guid id)
        {
            var payroll = await _context.Payrolls.FindAsync(id);
            if (payroll == null)
            {
                return NotFound();
            }

            _context.Payrolls.Remove(payroll);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PayrollExists(Guid id)
        {
            return _context.Payrolls.Any(e => e.Id == id);
        }
    }
}
