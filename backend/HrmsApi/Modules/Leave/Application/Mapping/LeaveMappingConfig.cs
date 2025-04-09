using Mapster;
using HrmsApi.Modules.Leave.Application.DTOs;
using HrmsApi.Modules.Leave.Domain;

namespace HrmsApi.Modules.Leave.Application.Mapping
{
    public class LeaveMappingConfig
    {
        public static void Configure()
        {
            // Map from Domain.LeaveRequest to LeaveRequestDTO
            TypeAdapterConfig<LeaveRequest, LeaveRequestDTO>
                .NewConfig()
                .Map(dest => dest.EmployeeName, src => src.Employee != null ? src.Employee.FullName : string.Empty)
                .Map(dest => dest.LeaveType, src => src.LeaveType);

            // Map from CreateLeaveRequestDTO to Domain.LeaveRequest
            TypeAdapterConfig<CreateLeaveRequestDTO, LeaveRequest>
                .NewConfig()
                .IgnoreNonMapped(true)
                .Map(dest => dest.Status, src => "Pending")
                .Map(dest => dest.RequestDate, src => DateTime.UtcNow);

            // Map from UpdateLeaveRequestDTO to Domain.LeaveRequest
            TypeAdapterConfig<UpdateLeaveRequestDTO, LeaveRequest>
                .NewConfig()
                .IgnoreNonMapped(true);
        }
    }
}
