using Mapster;
using HrmsApi.Modules.Employee.Application.DTOs;
using HrmsApi.Modules.Employee.Domain;

namespace HrmsApi.Modules.Employee.Application.Mapping
{
    public class EmployeeMappingConfig
    {
        public static void Configure()
        {
            // Map from Domain.Employee to EmployeeDTO
            TypeAdapterConfig<Domain.Employee, EmployeeDTO>
                .NewConfig()
                .Map(dest => dest.DepartmentName, src => src.Department != null ? src.Department.Name : string.Empty)
                .Map(dest => dest.DesignationTitle, src => src.Designation != null ? src.Designation.Title : string.Empty)
                .Map(dest => dest.ManagerName, src => src.Manager != null ? src.Manager.FullName : string.Empty);

            // Map from CreateEmployeeDTO to Domain.Employee
            TypeAdapterConfig<CreateEmployeeDTO, Domain.Employee>
                .NewConfig()
                .IgnoreNonMapped(true);

            // Map from UpdateEmployeeDTO to Domain.Employee
            TypeAdapterConfig<UpdateEmployeeDTO, Domain.Employee>
                .NewConfig()
                .IgnoreNonMapped(true);
        }
    }
}
