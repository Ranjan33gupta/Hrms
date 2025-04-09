using System;
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
    public class BankDetailsController : ControllerBase
    {
        private readonly HrmsDbContext _context;

        public BankDetailsController(HrmsDbContext context)
        {
            _context = context;
        }

        // GET: api/BankDetails/employee/{employeeId}
        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<BankDetailDTO>> GetBankDetailByEmployee(Guid employeeId)
        {
            var bankDetail = await _context.BankDetails
                .FirstOrDefaultAsync(b => b.EmployeeId == employeeId);

            if (bankDetail == null)
            {
                // Return empty bank detail instead of 404
                return new BankDetailDTO 
                {
                    EmployeeId = employeeId,
                    BankName = string.Empty,
                    AccountHolderName = string.Empty,
                    AccountNumber = string.Empty,
                    IFSCCode = string.Empty,
                    BranchName = string.Empty
                };
            }

            return bankDetail.Adapt<BankDetailDTO>();
        }

        // POST: api/BankDetails
        [HttpPost]
        public async Task<ActionResult<BankDetailDTO>> CreateBankDetail(BankDetailDTO bankDetailDto)
        {
            // Check if employee exists
            var employee = await _context.Employees.FindAsync(bankDetailDto.EmployeeId);
            if (employee == null)
            {
                return BadRequest("Employee not found");
            }

            // Check if bank detail already exists for this employee
            var existingBankDetail = await _context.BankDetails
                .FirstOrDefaultAsync(b => b.EmployeeId == bankDetailDto.EmployeeId);
            
            if (existingBankDetail != null)
            {
                return Conflict("Bank details already exist for this employee");
            }

            var bankDetail = bankDetailDto.Adapt<BankDetail>();
            _context.BankDetails.Add(bankDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBankDetailByEmployee), new { employeeId = bankDetail.EmployeeId }, bankDetail.Adapt<BankDetailDTO>());
        }

        // PUT: api/BankDetails/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBankDetail(Guid id, BankDetailDTO bankDetailDto)
        {
            if (id != bankDetailDto.Id)
            {
                return BadRequest();
            }

            var bankDetail = await _context.BankDetails.FindAsync(id);
            if (bankDetail == null)
            {
                return NotFound();
            }

            bankDetailDto.Adapt(bankDetail);
            _context.Entry(bankDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BankDetailExists(id))
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

        // DELETE: api/BankDetails/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBankDetail(Guid id)
        {
            var bankDetail = await _context.BankDetails.FindAsync(id);
            if (bankDetail == null)
            {
                return NotFound();
            }

            _context.BankDetails.Remove(bankDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BankDetailExists(Guid id)
        {
            return _context.BankDetails.Any(e => e.Id == id);
        }
    }
}
