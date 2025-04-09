using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HrmsApi.Modules.Leave.Application.DTOs;
using HrmsApi.Modules.Leave.Application.Interfaces;
using HrmsApi.Modules.Leave.Domain;
using HrmsApi.Modules.Leave.Domain.Interfaces;
using HrmsApi.Modules.Employee.Domain.Interfaces;

namespace HrmsApi.Modules.Leave.Application.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ILeaveRequestRepository _leaveRequestRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public LeaveRequestService(ILeaveRequestRepository leaveRequestRepository, IEmployeeRepository employeeRepository)
        {
            _leaveRequestRepository = leaveRequestRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<IEnumerable<LeaveRequestDTO>> GetAllLeaveRequestsAsync()
        {
            var leaveRequests = await _leaveRequestRepository.GetAllAsync();
            return await MapToDtoListAsync(leaveRequests);
        }

        public async Task<LeaveRequestDTO> GetLeaveRequestByIdAsync(Guid id)
        {
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(id);
            return await MapToDtoAsync(leaveRequest);
        }

        public async Task<IEnumerable<LeaveRequestDTO>> GetLeaveRequestsByEmployeeAsync(Guid employeeId)
        {
            var leaveRequests = await _leaveRequestRepository.GetByEmployeeAsync(employeeId);
            return await MapToDtoListAsync(leaveRequests);
        }

        public async Task<IEnumerable<LeaveRequestDTO>> GetLeaveRequestsByStatusAsync(string status)
        {
            var leaveRequests = await _leaveRequestRepository.GetByStatusAsync(status);
            return await MapToDtoListAsync(leaveRequests);
        }

        public async Task<LeaveRequestDTO> CreateLeaveRequestAsync(CreateLeaveRequestDTO leaveRequestDto)
        {
            var leaveRequest = new LeaveRequest
            {
                Id = Guid.NewGuid(),
                EmployeeId = leaveRequestDto.EmployeeId,
                StartDate = leaveRequestDto.StartDate,
                EndDate = leaveRequestDto.EndDate,
                LeaveType = leaveRequestDto.LeaveType,
                Reason = leaveRequestDto.Reason,
                Status = "Pending",
                RequestDate = DateTime.UtcNow
            };

            var createdLeaveRequest = await _leaveRequestRepository.CreateAsync(leaveRequest);
            return await MapToDtoAsync(createdLeaveRequest);
        }

        public async Task<LeaveRequestDTO> UpdateLeaveRequestAsync(UpdateLeaveRequestDTO leaveRequestDto)
        {
            var existingLeaveRequest = await _leaveRequestRepository.GetByIdAsync(leaveRequestDto.Id);
            
            existingLeaveRequest.StartDate = leaveRequestDto.StartDate;
            existingLeaveRequest.EndDate = leaveRequestDto.EndDate;
            existingLeaveRequest.LeaveType = leaveRequestDto.LeaveType;
            existingLeaveRequest.Reason = leaveRequestDto.Reason;

            await _leaveRequestRepository.UpdateAsync(existingLeaveRequest);
            return await MapToDtoAsync(existingLeaveRequest);
        }

        public async Task<LeaveRequestDTO> ApproveLeaveRequestAsync(ApproveLeaveRequestDTO approveDto)
        {
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(approveDto.Id);
            
            leaveRequest.Status = "Approved";
            leaveRequest.ApprovedBy = approveDto.ApprovedBy;
            leaveRequest.ApprovalDate = DateTime.UtcNow;
            leaveRequest.Comments = approveDto.Comments;

            await _leaveRequestRepository.UpdateAsync(leaveRequest);
            return await MapToDtoAsync(leaveRequest);
        }

        public async Task<LeaveRequestDTO> RejectLeaveRequestAsync(RejectLeaveRequestDTO rejectDto)
        {
            var leaveRequest = await _leaveRequestRepository.GetByIdAsync(rejectDto.Id);
            
            leaveRequest.Status = "Rejected";
            leaveRequest.ApprovedBy = rejectDto.RejectedBy;
            leaveRequest.ApprovalDate = DateTime.UtcNow;
            leaveRequest.Comments = rejectDto.Comments;

            await _leaveRequestRepository.UpdateAsync(leaveRequest);
            return await MapToDtoAsync(leaveRequest);
        }

        public async Task DeleteLeaveRequestAsync(Guid id)
        {
            await _leaveRequestRepository.DeleteAsync(id);
        }

        private async Task<LeaveRequestDTO> MapToDtoAsync(LeaveRequest leaveRequest)
        {
            var employee = await _employeeRepository.GetByIdAsync(leaveRequest.EmployeeId);
            
            return new LeaveRequestDTO
            {
                Id = leaveRequest.Id,
                EmployeeId = leaveRequest.EmployeeId,
                EmployeeName = employee?.FullName ?? string.Empty,
                StartDate = leaveRequest.StartDate,
                EndDate = leaveRequest.EndDate,
                LeaveType = leaveRequest.LeaveType,
                Reason = leaveRequest.Reason,
                Status = leaveRequest.Status,
                RequestDate = leaveRequest.RequestDate,
                ApprovedBy = leaveRequest.ApprovedBy,
                ApprovalDate = leaveRequest.ApprovalDate,
                Comments = leaveRequest.Comments
            };
        }

        private async Task<IEnumerable<LeaveRequestDTO>> MapToDtoListAsync(IEnumerable<LeaveRequest> leaveRequests)
        {
            var dtoList = new List<LeaveRequestDTO>();
            
            foreach (var leaveRequest in leaveRequests)
            {
                dtoList.Add(await MapToDtoAsync(leaveRequest));
            }
            
            return dtoList;
        }
    }
}
