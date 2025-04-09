using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using HrmsApi.Modules.Leave.Domain;

namespace HrmsApi.Modules.Employee.Domain
{
    public class Employee
    {
        public Guid Id { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CountryCode { get; set; } = "+91"; // Default country code for India
        public string ContactNumber { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string MaritalStatus { get; set; } = string.Empty;
        public string NationalIdNumber { get; set; } = string.Empty;

        public Guid DepartmentId { get; set; }
        public Guid DesignationId { get; set; }
        public Guid? ManagerId { get; set; }

        // Navigation properties
        [JsonIgnore]
        public Department? Department { get; set; }
        [JsonIgnore]
        public Designation? Designation { get; set; }
        [JsonIgnore]
        public Employee? Manager { get; set; }
        [JsonIgnore]
        public ICollection<Employee>? Subordinates { get; set; }
        [JsonIgnore]
        public ICollection<LeaveRequest>? LeaveRequests { get; set; }
        [JsonIgnore]
        public BankDetail? BankDetail { get; set; }
        [JsonIgnore]
        public ICollection<Payroll>? Payrolls { get; set; }

        public DateTime JoiningDate { get; set; } = DateTime.Now;
        public DateTime? ExitDate { get; set; }

        public string EmploymentType { get; set; } = "Full-Time"; // Full-Time, Part-Time, Contract
        public bool IsActive { get; set; } = true;
    }
}
