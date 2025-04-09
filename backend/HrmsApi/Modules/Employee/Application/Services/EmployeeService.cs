using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HrmsApi.Modules.Employee.Application.DTOs;
using HrmsApi.Modules.Employee.Application.Interfaces;
using HrmsApi.Modules.Employee.Domain;
using HrmsApi.Modules.Employee.Domain.Interfaces;

namespace HrmsApi.Modules.Employee.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();
            return employees.Select(e => MapToDto(e)).Where(dto => dto != null).ToList();
        }

        public async Task<EmployeeDTO?> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            return MapToDto(employee);
        }

        public async Task<EmployeeDTO?> CreateEmployeeAsync(CreateEmployeeDTO employeeDto)
        {
            var employee = new Domain.Employee
            {
                Id = Guid.NewGuid(),
                EmployeeCode = employeeDto.EmployeeCode,
                FullName = employeeDto.FullName ?? "New Employee",
                Email = employeeDto.Email,
                ContactNumber = employeeDto.ContactNumber,
                Gender = employeeDto.Gender,
                DateOfBirth = employeeDto.DateOfBirth,
                MaritalStatus = employeeDto.MaritalStatus,
                NationalIdNumber = employeeDto.NationalIdNumber,
                DepartmentId = employeeDto.DepartmentId ?? Guid.Empty,
                DesignationId = employeeDto.DesignationId ?? Guid.Empty,
                ManagerId = employeeDto.ManagerId,
                JoiningDate = employeeDto.JoiningDate ?? DateTime.Now,
                IsActive = true
            };

            var createdEmployee = await _employeeRepository.CreateAsync(employee);
            return MapToDto(createdEmployee);
        }

        public async Task<EmployeeDTO?> UpdateEmployeeAsync(UpdateEmployeeDTO employeeDto)
        {
            var existingEmployee = await _employeeRepository.GetByIdAsync(employeeDto.Id);
            
            if (existingEmployee == null)
            {
                throw new Exception($"Employee with ID {employeeDto.Id} not found");
            }
            
            existingEmployee.FullName = employeeDto.FullName ?? existingEmployee.FullName;
            existingEmployee.Email = employeeDto.Email ?? existingEmployee.Email;
            existingEmployee.ContactNumber = employeeDto.ContactNumber ?? existingEmployee.ContactNumber;
            existingEmployee.Gender = employeeDto.Gender ?? existingEmployee.Gender;
            existingEmployee.DateOfBirth = employeeDto.DateOfBirth ?? existingEmployee.DateOfBirth;
            existingEmployee.MaritalStatus = employeeDto.MaritalStatus ?? existingEmployee.MaritalStatus;
            existingEmployee.NationalIdNumber = employeeDto.NationalIdNumber ?? existingEmployee.NationalIdNumber;
            existingEmployee.DepartmentId = employeeDto.DepartmentId ?? existingEmployee.DepartmentId;
            existingEmployee.DesignationId = employeeDto.DesignationId ?? existingEmployee.DesignationId;
            existingEmployee.ManagerId = employeeDto.ManagerId ?? existingEmployee.ManagerId;
            existingEmployee.JoiningDate = employeeDto.JoiningDate ?? existingEmployee.JoiningDate;
            existingEmployee.ExitDate = employeeDto.ExitDate ?? existingEmployee.ExitDate;
            existingEmployee.IsActive = employeeDto.IsActive ?? existingEmployee.IsActive;

            await _employeeRepository.UpdateAsync(existingEmployee);
            return MapToDto(existingEmployee);
        }

        public async Task DeleteEmployeeAsync(Guid id)
        {
            await _employeeRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<EmployeeDTO>> GetEmployeesByDepartmentAsync(Guid departmentId)
        {
            var employees = await _employeeRepository.GetByDepartmentAsync(departmentId);
            return employees.Select(e => MapToDto(e)).Where(dto => dto != null).ToList();
        }

        public async Task<IEnumerable<EmployeeDTO>> GetEmployeesByManagerAsync(Guid managerId)
        {
            var employees = await _employeeRepository.GetByManagerAsync(managerId);
            return employees.Select(e => MapToDto(e)).Where(dto => dto != null).ToList();
        }

        private EmployeeDTO? MapToDto(Domain.Employee? employee)
        {
            if (employee == null)
            {
                return null;
            }
            
            return new EmployeeDTO
            {
                Id = employee.Id,
                EmployeeCode = employee.EmployeeCode,
                FullName = employee.FullName,
                Email = employee.Email,
                ContactNumber = employee.ContactNumber,
                Gender = employee.Gender,
                DateOfBirth = employee.DateOfBirth,
                MaritalStatus = employee.MaritalStatus,
                NationalIdNumber = employee.NationalIdNumber,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.Name ?? "N/A",
                DesignationId = employee.DesignationId,
                DesignationTitle = employee.Designation?.Title ?? "N/A",
                ManagerId = employee.ManagerId,
                ManagerName = employee.Manager?.FullName ?? "N/A",
                JoiningDate = employee.JoiningDate,
                ExitDate = employee.ExitDate,
                IsActive = employee.IsActive
            };
        }
    }
}
