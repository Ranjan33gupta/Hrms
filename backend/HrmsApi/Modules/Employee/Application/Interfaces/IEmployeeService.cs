using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HrmsApi.Modules.Employee.Application.DTOs;

namespace HrmsApi.Modules.Employee.Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync();
        Task<EmployeeDTO?> GetEmployeeByIdAsync(Guid id);
        Task<EmployeeDTO?> CreateEmployeeAsync(CreateEmployeeDTO employeeDto);
        Task<EmployeeDTO?> UpdateEmployeeAsync(UpdateEmployeeDTO employeeDto);
        Task DeleteEmployeeAsync(Guid id);
        Task<IEnumerable<EmployeeDTO>> GetEmployeesByDepartmentAsync(Guid departmentId);
        Task<IEnumerable<EmployeeDTO>> GetEmployeesByManagerAsync(Guid managerId);
    }
}
