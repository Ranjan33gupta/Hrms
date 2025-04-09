using Microsoft.AspNetCore.Mvc;
using HrmsApi.Services;
using HrmsApi.Modules.Auth.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using HrmsApi.Modules.Employee.Domain.Interfaces;
using System.Linq;

namespace HrmsApi.Modules.Auth.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IEmployeeRepository _employeeRepository;

        public AuthController(AuthService authService, IEmployeeRepository employeeRepository)
        {
            _authService = authService;
            _employeeRepository = employeeRepository;
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDTO>> Register([FromBody] RegisterDTO registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if there's an employee with the provided contact number
            var employees = await _employeeRepository.GetAllAsync();
            var matchingEmployee = employees.FirstOrDefault(e => 
                e.CountryCode == registerDto.CountryCode && 
                e.ContactNumber == registerDto.ContactNumber);

            if (matchingEmployee == null)
            {
                return BadRequest(new { message = "No employee found with this contact number. Please contact HR." });
            }

            // Set the employee ID in the registration process
            var response = await _authService.Register(registerDto, matchingEmployee.Id);
            
            if (response == null)
            {
                return BadRequest(new { message = "Username already exists" });
            }

            return Ok(response);
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.Login(loginDto);
            
            if (response == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            return Ok(response);
        }

        // GET: api/Auth/profile
        [HttpGet("profile")]
        [HrmsApi.Attributes.Authorize]
        public IActionResult GetProfile()
        {
            var user = HttpContext.Items["User"] as HrmsApi.Modules.Auth.Domain.User;
            
            if (user == null)
            {
                return Unauthorized(new { message = "Unauthorized" });
            }

            return Ok(new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                CountryCode = user.CountryCode,
                ContactNumber = user.ContactNumber,
                EmployeeId = user.EmployeeId
            });
        }
    }
}
