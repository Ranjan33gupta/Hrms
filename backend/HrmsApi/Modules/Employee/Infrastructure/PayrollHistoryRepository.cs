using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HrmsApi.Data;
using HrmsApi.Modules.Employee.Application.Interfaces;
using HrmsApi.Modules.Employee.Domain;

namespace HrmsApi.Modules.Employee.Infrastructure
{
    public class PayrollHistoryRepository : IPayrollHistoryRepository
    {
        private readonly HrmsDbContext _context;

        public PayrollHistoryRepository(HrmsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PayrollHistory>> GetAllPayrollHistoriesAsync()
        {
            return await _context.PayrollHistories
                .Include(ph => ph.Employee)
                .Include(ph => ph.Payroll)
                .ToListAsync();
        }

        public async Task<PayrollHistory> GetPayrollHistoryByIdAsync(Guid id)
        {
            return await _context.PayrollHistories
                .Include(ph => ph.Employee)
                .Include(ph => ph.Payroll)
                .FirstOrDefaultAsync(ph => ph.Id == id);
        }

        public async Task<IEnumerable<PayrollHistory>> GetPayrollHistoriesByEmployeeIdAsync(Guid employeeId)
        {
            return await _context.PayrollHistories
                .Include(ph => ph.Employee)
                .Include(ph => ph.Payroll)
                .Where(ph => ph.EmployeeId == employeeId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PayrollHistory>> GetPayrollHistoriesByPayrollIdAsync(Guid payrollId)
        {
            return await _context.PayrollHistories
                .Include(ph => ph.Employee)
                .Include(ph => ph.Payroll)
                .Where(ph => ph.PayrollId == payrollId)
                .ToListAsync();
        }

        public async Task<PayrollHistory> AddPayrollHistoryAsync(PayrollHistory payrollHistory)
        {
            _context.PayrollHistories.Add(payrollHistory);
            await _context.SaveChangesAsync();
            return payrollHistory;
        }

        public async Task<PayrollHistory> UpdatePayrollHistoryAsync(PayrollHistory payrollHistory)
        {
            _context.Entry(payrollHistory).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return payrollHistory;
        }

        public async Task<bool> DeletePayrollHistoryAsync(Guid id)
        {
            var payrollHistory = await _context.PayrollHistories.FindAsync(id);
            if (payrollHistory == null)
            {
                return false;
            }

            _context.PayrollHistories.Remove(payrollHistory);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
