using System;

namespace HrmsApi.Modules.Employee.Application.DTOs
{
    public class EmployeeDTO
    {
        public Guid? Id { get; set; }
        public string? EmployeeCode { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? ContactNumber { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? MaritalStatus { get; set; }
        public string? NationalIdNumber { get; set; }
        public Guid? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public Guid? DesignationId { get; set; }
        public string? DesignationTitle { get; set; }
        public Guid? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public DateTime? JoiningDate { get; set; }
        public DateTime? ExitDate { get; set; }
        public bool IsActive { get; set; } = true;
        public BankDetailDTO? BankDetail { get; set; }
    }

    public class CreateEmployeeDTO
    {
        public string? EmployeeCode { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? ContactNumber { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? MaritalStatus { get; set; }
        public string? NationalIdNumber { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? DesignationId { get; set; }
        public Guid? ManagerId { get; set; }
        public DateTime? JoiningDate { get; set; }
        public BankDetailDTO? BankDetail { get; set; }
        public PayrollDTO? InitialSalary { get; set; }
    }

    public class UpdateEmployeeDTO
    {
        public Guid? Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? ContactNumber { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? MaritalStatus { get; set; }
        public string? NationalIdNumber { get; set; }
        public Guid? DepartmentId { get; set; }
        public Guid? DesignationId { get; set; }
        public Guid? ManagerId { get; set; }
        public DateTime? JoiningDate { get; set; }
        public DateTime? ExitDate { get; set; }
        public bool? IsActive { get; set; } = true;
        public BankDetailDTO? BankDetail { get; set; }
        public PayrollDTO? InitialSalary { get; set; }
    }
}
