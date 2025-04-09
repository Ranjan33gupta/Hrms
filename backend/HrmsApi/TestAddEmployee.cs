using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HrmsApi.Modules.Employee.Domain;
using HrmsApi.Modules.Auth.Domain;

namespace HrmsApi
{
    public class TestAddEmployee
    {
        public static async Task AddEmployeeTest()
        {
            try
            {
                // First, get an auth token
                var loginResponse = await GetAuthToken();
                if (string.IsNullOrEmpty(loginResponse.Token))
                {
                    Console.WriteLine("Failed to get auth token");
                    return;
                }

                Console.WriteLine($"Got auth token: {loginResponse.Token}");

                // Now add an employee
                var employee = new Employee
                {
                    EmployeeCode = $"EMP{DateTime.Now.ToString("yyyyMMddHHmmss")}",
                    FullName = "Test Employee",
                    Email = "test.employee@example.com",
                    ContactNumber = "1234567890",
                    Gender = "Male",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    MaritalStatus = "Single",
                    NationalIdNumber = "TEST12345",
                    DepartmentId = Guid.Parse("01234567-89ab-cdef-0123-456789abcdef"), // IT Department
                    DesignationId = Guid.Parse("34567890-89ab-cdef-0123-456789abcdef"), // Software Engineer
                    JoiningDate = DateTime.Now,
                    EmploymentType = "Full-Time",
                    IsActive = true
                };

                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse.Token);

                var employeeJson = JsonSerializer.Serialize(employee);
                var content = new StringContent(employeeJson, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("http://localhost:5171/api/employees", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Employee added successfully!");
                    Console.WriteLine($"Response: {responseContent}");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to add employee. Status: {response.StatusCode}");
                    Console.WriteLine($"Error: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private static async Task<AuthResponse> GetAuthToken()
        {
            var client = new HttpClient();
            var loginData = new
            {
                Username = "admin",
                Password = "admin123"
            };

            var loginJson = JsonSerializer.Serialize(loginData);
            var content = new StringContent(loginJson, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("http://localhost:5171/api/auth/login", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<AuthResponse>(responseContent) ?? new AuthResponse();
            }
            
            return new AuthResponse();
        }
    }

    public class AuthResponse
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? Token { get; set; }
    }
}
