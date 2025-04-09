using System;
using System.Text.Json.Serialization;

namespace HrmsApi.Modules.Auth.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        [JsonIgnore]
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User"; // Default role is User, can be Admin
        public string CountryCode { get; set; } = "+91"; // Default country code for India
        public string ContactNumber { get; set; } = string.Empty;
        public Guid? EmployeeId { get; set; } // Link to Employee record
        public DateTime CreatedAt { get; set; } = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}
