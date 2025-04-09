using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HrmsApi.Modules.Employee.Domain;

namespace HrmsApi.Modules.Employee.Application.Interfaces
{
    public interface IPayrollHistoryRepository
    {
        Task<IEnumerable<PayrollHistory>> GetAllPayrollHistoriesAsync();
        Task<PayrollHistory> GetPayrollHistoryByIdAsync(Guid id);
        Task<IEnumerable<PayrollHistory>> GetPayrollHistoriesByEmployeeIdAsync(Guid employeeId);
        Task<IEnumerable<PayrollHistory>> GetPayrollHistoriesByPayrollIdAsync(Guid payrollId);
        Task<PayrollHistory> AddPayrollHistoryAsync(PayrollHistory payrollHistory);
        Task<PayrollHistory> UpdatePayrollHistoryAsync(PayrollHistory payrollHistory);
        Task<bool> DeletePayrollHistoryAsync(Guid id);
    }
}
