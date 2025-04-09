using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HrmsApi.Modules.Leave.Application.DTOs;

namespace HrmsApi.Modules.Leave.Application.Interfaces
{
    public interface ILeaveRequestService
    {
        Task<IEnumerable<LeaveRequestDTO>> GetAllLeaveRequestsAsync();
        Task<LeaveRequestDTO> GetLeaveRequestByIdAsync(Guid id);
        Task<IEnumerable<LeaveRequestDTO>> GetLeaveRequestsByEmployeeAsync(Guid employeeId);
        Task<IEnumerable<LeaveRequestDTO>> GetLeaveRequestsByStatusAsync(string status);
        Task<LeaveRequestDTO> CreateLeaveRequestAsync(CreateLeaveRequestDTO leaveRequestDto);
        Task<LeaveRequestDTO> UpdateLeaveRequestAsync(UpdateLeaveRequestDTO leaveRequestDto);
        Task<LeaveRequestDTO> ApproveLeaveRequestAsync(ApproveLeaveRequestDTO approveDto);
        Task<LeaveRequestDTO> RejectLeaveRequestAsync(RejectLeaveRequestDTO rejectDto);
        Task DeleteLeaveRequestAsync(Guid id);
    }
}
