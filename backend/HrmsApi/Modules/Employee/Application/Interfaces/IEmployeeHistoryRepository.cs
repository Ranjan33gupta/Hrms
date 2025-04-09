using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HrmsApi.Modules.Employee.Domain;
using HrmsApi.Modules.Employee.Application.DTOs;

namespace HrmsApi.Modules.Employee.Application.Interfaces
{
    public interface IEmployeeHistoryRepository
    {
        Task<IEnumerable<EmployeeHistory>> GetAllAsync();
        Task<EmployeeHistory> GetByIdAsync(Guid id);
        Task<IEnumerable<EmployeeHistory>> GetByEmployeeIdAsync(Guid employeeId);
        Task<EmployeeHistory> CreateAsync(EmployeeHistory employeeHistory);
        Task<EmployeeHistory> UpdateAsync(EmployeeHistory employeeHistory);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> AddHistoryLogAsync(Guid employeeId, string employeeName, EmployeeChangeDetail logEntry);
        Task<IEnumerable<FlattenedEmployeeHistoryDTO>> GetFlattenedHistoryByEmployeeIdAsync(Guid employeeId);
        Task<IEnumerable<FlattenedEmployeeHistoryDTO>> SearchHistoryAsync(
            Guid? employeeId = null, 
            DateTime? startDate = null, 
            DateTime? endDate = null, 
            string? actionType = null, 
            string? fieldName = null);
    }
}
