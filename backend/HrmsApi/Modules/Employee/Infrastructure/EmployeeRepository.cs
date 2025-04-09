using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HrmsApi.Data;
using HrmsApi.Modules.Employee.Domain;
using HrmsApi.Modules.Employee.Domain.Interfaces;

namespace HrmsApi.Modules.Employee.Infrastructure
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly HrmsDbContext _context;

        public EmployeeRepository(HrmsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Domain.Employee>> GetAllAsync()
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .ToListAsync();
        }

        public async Task<Domain.Employee?> GetByIdAsync(Guid? id)
        {
            if (id == null)
            {
                return null;
            }
            
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Domain.Employee> CreateAsync(Domain.Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task UpdateAsync(Domain.Employee employee)
        {
            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Employees.AnyAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Domain.Employee>> GetByDepartmentAsync(Guid departmentId)
        {
            return await _context.Employees
                .Where(e => e.DepartmentId == departmentId)
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .ToListAsync();
        }

        public async Task<IEnumerable<Domain.Employee>> GetByManagerAsync(Guid? managerId)
        {
            if (managerId == null)
            {
                return new List<Domain.Employee>();
            }
            
            return await _context.Employees
                .Where(e => e.ManagerId == managerId)
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .ToListAsync();
        }
    }
}
