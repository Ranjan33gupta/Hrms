using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HrmsApi.Data;
using HrmsApi.Modules.Leave.Domain;
using HrmsApi.Modules.Leave.Domain.Interfaces;

namespace HrmsApi.Modules.Leave.Infrastructure
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly HrmsDbContext _context;

        public LeaveRequestRepository(HrmsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LeaveRequest>> GetAllAsync()
        {
            return await _context.LeaveRequests
                .Include(l => l.Employee)
                .ToListAsync();
        }

        public async Task<LeaveRequest> GetByIdAsync(Guid id)
        {
            return await _context.LeaveRequests
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<LeaveRequest>> GetByEmployeeAsync(Guid employeeId)
        {
            return await _context.LeaveRequests
                .Where(l => l.EmployeeId == employeeId)
                .Include(l => l.Employee)
                .ToListAsync();
        }

        public async Task<IEnumerable<LeaveRequest>> GetByStatusAsync(string status)
        {
            return await _context.LeaveRequests
                .Where(l => l.Status.ToLower() == status.ToLower())
                .Include(l => l.Employee)
                .ToListAsync();
        }

        public async Task<LeaveRequest> CreateAsync(LeaveRequest leaveRequest)
        {
            _context.LeaveRequests.Add(leaveRequest);
            await _context.SaveChangesAsync();
            return leaveRequest;
        }

        public async Task UpdateAsync(LeaveRequest leaveRequest)
        {
            _context.Entry(leaveRequest).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest != null)
            {
                _context.LeaveRequests.Remove(leaveRequest);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.LeaveRequests.AnyAsync(l => l.Id == id);
        }
    }
}
