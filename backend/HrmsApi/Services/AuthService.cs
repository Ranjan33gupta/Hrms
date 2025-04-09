using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HrmsApi.Data;
using HrmsApi.Modules.Auth.Domain;
using HrmsApi.Modules.Auth.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HrmsApi.Services
{
    public class AuthService
    {
        private readonly HrmsDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(HrmsDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDTO?> Register(RegisterDTO registerDto, Guid employeeId)
        {
            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
            {
                return null;
            }

            // Create new user
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                Role = "User", // Default role
                CountryCode = registerDto.CountryCode,
                ContactNumber = registerDto.ContactNumber,
                EmployeeId = employeeId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate token
            var token = GenerateJwtToken(user);

            return new AuthResponseDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                CountryCode = user.CountryCode,
                ContactNumber = user.ContactNumber,
                EmployeeId = user.EmployeeId,
                Token = token
            };
        }

        public async Task<AuthResponseDTO?> Login(LoginDTO loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null)
            {
                return null;
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return null;
            }

            // Generate token
            var token = GenerateJwtToken(user);

            return new AuthResponseDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                CountryCode = user.CountryCode,
                ContactNumber = user.ContactNumber,
                EmployeeId = user.EmployeeId,
                Token = token
            };
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? "HrmsDefaultSecretKeyForDevelopment12345"));
            
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("role", user.Role), // Use a simple claim name for role to match our middleware
                new Claim("employeeId", user.EmployeeId?.ToString() ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "HrmsApi",
                audience: _configuration["Jwt:Audience"] ?? "HrmsClient",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
