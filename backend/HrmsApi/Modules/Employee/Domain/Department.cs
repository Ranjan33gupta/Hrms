using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HrmsApi.Modules.Employee.Domain
{
    public class Department
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        
        // Navigation property
        [JsonIgnore]
        public ICollection<Employee>? Employees { get; set; }
    }
}
