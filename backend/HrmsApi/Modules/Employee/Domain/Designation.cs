using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HrmsApi.Modules.Employee.Domain
{
    public class Designation
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        
        // Navigation property
        [JsonIgnore]
        public ICollection<Employee>? Employees { get; set; }
    }
}
