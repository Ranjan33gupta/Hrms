using System;

namespace HrmsApi.Modules.Auth.Application.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public Guid? EmployeeId { get; set; }
    }

    public class LoginDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string CountryCode { get; set; } = "+91";
        public string ContactNumber { get; set; } = string.Empty;
    }

    public class AuthResponseDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public Guid? EmployeeId { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
