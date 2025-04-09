using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HrmsApi.Modules.Leave.Domain;

namespace HrmsApi.Modules.Leave.Domain.Interfaces
{
    public interface ILeaveRequestRepository
    {
        Task<IEnumerable<LeaveRequest>> GetAllAsync();
        Task<LeaveRequest> GetByIdAsync(Guid id);
        Task<IEnumerable<LeaveRequest>> GetByEmployeeAsync(Guid employeeId);
        Task<IEnumerable<LeaveRequest>> GetByStatusAsync(string status);
        Task<LeaveRequest> CreateAsync(LeaveRequest leaveRequest);
        Task UpdateAsync(LeaveRequest leaveRequest);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
