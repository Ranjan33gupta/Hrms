using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HrmsApi.Modules.Employee.Domain;

namespace HrmsApi.Modules.Employee.Domain.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee?> GetByIdAsync(Guid? id);
        Task<Employee> CreateAsync(Employee employee);
        Task UpdateAsync(Employee employee);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<IEnumerable<Employee>> GetByDepartmentAsync(Guid departmentId);
        Task<IEnumerable<Employee>> GetByManagerAsync(Guid? managerId);
    }
}
