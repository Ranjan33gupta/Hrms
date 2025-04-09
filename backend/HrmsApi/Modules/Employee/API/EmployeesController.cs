using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HrmsApi.Data;
using HrmsApi.Modules.Employee.Domain;
using HrmsApi.Modules.Employee.Application.DTOs;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using HrmsApi.Attributes;
using HrmsApi.Modules.Employee.Application.Interfaces;
using System.Text;

namespace HrmsApi.Modules.Employee.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase, IDisposable
    {
        private readonly ILogger<EmployeesController> _logger;
        private readonly HrmsDbContext _context;
        private readonly IEmployeeHistoryRepository _employeeHistoryRepository;

        public EmployeesController(HrmsDbContext context, IEmployeeHistoryRepository employeeHistoryRepository, ILogger<EmployeesController> logger)
        {
            _context = context;
            _employeeHistoryRepository = employeeHistoryRepository;
            _logger = logger;
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDTO>>> GetEmployees()
        {
            Console.WriteLine("GET /api/employees request received");
            try
            {
                var employees = await _context.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Designation)
                    .Include(e => e.Manager)
                    .ToListAsync();

                // Use Mapster to map domain entities to DTOs
                var employeeDtos = employees.Adapt<List<EmployeeDTO>>();

                // Ensure department and designation information is properly set
                foreach (var dto in employeeDtos)
                {
                    var employee = employees.FirstOrDefault(e => e.Id == dto.Id);
                    if (employee != null)
                    {
                        dto.DepartmentName = employee.Department?.Name ?? "N/A";
                        dto.DesignationTitle = employee.Designation?.Title ?? "N/A";
                    }
                }

                Console.WriteLine($"Returning {employeeDtos.Count} employees");
                return employeeDtos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetEmployees: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDTO>> GetEmployee(Guid id)
        {
            try
            {
                var employee = await _context.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Designation)
                    .Include(e => e.Manager)
                    .Include(e => e.BankDetail)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (employee == null)
                {
                    return NotFound();
                }

                // Use Mapster to map domain entity to DTO
                var employeeDto = employee.Adapt<EmployeeDTO>();

                // Ensure department and designation information is properly set
                employeeDto.DepartmentName = employee.Department?.Name ?? "N/A";
                employeeDto.DesignationTitle = employee.Designation?.Title ?? "N/A";
                employeeDto.ManagerName = employee.Manager?.FullName ?? "N/A";

                return employeeDto;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Employees
        [HttpPost]
        public async Task<ActionResult<EmployeeDTO>> PostEmployee(CreateEmployeeDTO createEmployeeDto)
        {
            try
            {
                Console.WriteLine($"POST /api/employees request received with data: {System.Text.Json.JsonSerializer.Serialize(createEmployeeDto)}");

                // Validate department ID
                if (createEmployeeDto.DepartmentId == null)
                {
                    // Get a valid department from the database
                    var department = await _context.Departments.FirstOrDefaultAsync();
                    if (department == null)
                    {
                        // If no departments exist, create one
                        department = new Department
                        {
                            Id = Guid.Parse("01234567-89ab-cdef-0123-456789abcdef"),
                            Name = "IT",
                            Code = "IT"
                        };
                        _context.Departments.Add(department);
                        await _context.SaveChangesAsync();
                    }

                    Console.WriteLine($"Setting null department ID to valid department ID {department.Id}");
                    createEmployeeDto.DepartmentId = department.Id;
                }
                else
                {
                    var departmentExists = await _context.Departments.AnyAsync(d => d.Id == createEmployeeDto.DepartmentId);
                    if (!departmentExists)
                    {
                        // Get a valid department from the database
                        var department = await _context.Departments.FirstOrDefaultAsync();
                        if (department == null)
                        {
                            // If no departments exist, create one
                            department = new Department
                            {
                                Id = Guid.Parse("01234567-89ab-cdef-0123-456789abcdef"),
                                Name = "IT",
                                Code = "IT"
                            };
                            _context.Departments.Add(department);
                            await _context.SaveChangesAsync();
                        }

                        Console.WriteLine($"Replacing invalid department ID {createEmployeeDto.DepartmentId} with valid department ID {department.Id}");
                        createEmployeeDto.DepartmentId = department.Id;
                    }
                }

                // Validate designation ID
                if (createEmployeeDto.DesignationId == null)
                {
                    // Get a valid designation from the database
                    var designation = await _context.Designations.FirstOrDefaultAsync();
                    if (designation == null)
                    {
                        // If no designations exist, create one
                        designation = new Designation
                        {
                            Id = Guid.Parse("34567890-89ab-cdef-0123-456789abcdef"),
                            Title = "Software Engineer"
                        };
                        _context.Designations.Add(designation);
                        await _context.SaveChangesAsync();
                    }

                    Console.WriteLine($"Setting null designation ID to valid designation ID {designation.Id}");
                    createEmployeeDto.DesignationId = designation.Id;
                }
                else
                {
                    var designationExists = await _context.Designations.AnyAsync(d => d.Id == createEmployeeDto.DesignationId);
                    if (!designationExists)
                    {
                        // Get a valid designation from the database
                        var designation = await _context.Designations.FirstOrDefaultAsync();
                        if (designation == null)
                        {
                            // If no designations exist, create one
                            designation = new Designation
                            {
                                Id = Guid.Parse("34567890-89ab-cdef-0123-456789abcdef"),
                                Title = "Software Engineer"
                            };
                            _context.Designations.Add(designation);
                            await _context.SaveChangesAsync();
                        }

                        Console.WriteLine($"Replacing invalid designation ID {createEmployeeDto.DesignationId} with valid designation ID {designation.Id}");
                        createEmployeeDto.DesignationId = designation.Id;
                    }
                }

                // Ensure other required fields have default values if null
                if (string.IsNullOrEmpty(createEmployeeDto.FullName))
                {
                    createEmployeeDto.FullName = "New Employee";
                }

                if (createEmployeeDto.JoiningDate == null)
                {
                    createEmployeeDto.JoiningDate = DateTime.UtcNow;
                }

                // Start a transaction to ensure all operations succeed or fail together
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Step 1: Create and add the employee
                    var employee = new Domain.Employee
                    {
                        Id = Guid.NewGuid(),
                        EmployeeCode = !string.IsNullOrEmpty(createEmployeeDto.EmployeeCode) ? createEmployeeDto.EmployeeCode : $"EMP{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}",
                        FullName = !string.IsNullOrEmpty(createEmployeeDto.FullName) ? createEmployeeDto.FullName : "New Employee",
                        Email = createEmployeeDto.Email ?? string.Empty,
                        ContactNumber = createEmployeeDto.ContactNumber ?? string.Empty,
                        Gender = createEmployeeDto.Gender ?? string.Empty,
                        DateOfBirth = createEmployeeDto.DateOfBirth.HasValue ? DateTime.SpecifyKind(createEmployeeDto.DateOfBirth.Value, DateTimeKind.Utc) : null,
                        MaritalStatus = createEmployeeDto.MaritalStatus ?? string.Empty,
                        NationalIdNumber = createEmployeeDto.NationalIdNumber ?? string.Empty,
                        DepartmentId = createEmployeeDto.DepartmentId.Value, // We've validated this is not null above
                        DesignationId = createEmployeeDto.DesignationId.Value, // We've validated this is not null above
                        ManagerId = createEmployeeDto.ManagerId,
                        JoiningDate = createEmployeeDto.JoiningDate.HasValue ? DateTime.SpecifyKind(createEmployeeDto.JoiningDate.Value, DateTimeKind.Utc) : DateTime.UtcNow,
                        EmploymentType = "Full-Time",
                        IsActive = true
                    };

                    _context.Employees.Add(employee);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Employee added successfully with ID: {employee.Id}");

                    // Step 2: Add bank details if provided
                    if (createEmployeeDto.BankDetail != null)
                    {
                        // Ensure required fields are not null
                        if (string.IsNullOrEmpty(createEmployeeDto.BankDetail.BankName))
                            createEmployeeDto.BankDetail.BankName = "Default Bank";

                        if (string.IsNullOrEmpty(createEmployeeDto.BankDetail.AccountNumber))
                            createEmployeeDto.BankDetail.AccountNumber = "0000000000";

                        if (string.IsNullOrEmpty(createEmployeeDto.BankDetail.AccountHolderName))
                            createEmployeeDto.BankDetail.AccountHolderName = createEmployeeDto.FullName;

                        if (string.IsNullOrEmpty(createEmployeeDto.BankDetail.IFSCCode))
                            createEmployeeDto.BankDetail.IFSCCode = "DEFAULTCODE";

                        if (string.IsNullOrEmpty(createEmployeeDto.BankDetail.BranchName))
                            createEmployeeDto.BankDetail.BranchName = "Main Branch";

                        var bankDetail = new BankDetail
                        {
                            Id = Guid.NewGuid(),
                            EmployeeId = employee.Id,
                            BankName = createEmployeeDto.BankDetail.BankName,
                            AccountHolderName = createEmployeeDto.BankDetail.AccountHolderName,
                            AccountNumber = createEmployeeDto.BankDetail.AccountNumber,
                            IFSCCode = createEmployeeDto.BankDetail.IFSCCode,
                            BranchName = createEmployeeDto.BankDetail.BranchName
                        };

                        _context.BankDetails.Add(bankDetail);
                        await _context.SaveChangesAsync();
                        Console.WriteLine($"Bank details added successfully for employee ID: {employee.Id}");
                    }

                    // Step 3: Add initial salary/payroll if provided
                    if (createEmployeeDto.InitialSalary != null)
                    {
                        var payroll = new Payroll
                        {
                            Id = Guid.NewGuid(),
                            EmployeeId = employee.Id,
                            BasicSalary = createEmployeeDto.InitialSalary.BasicSalary ?? 0,
                            HRA = createEmployeeDto.InitialSalary.HRA ?? 0,
                            Allowances = createEmployeeDto.InitialSalary.Allowances ?? 0,
                            Deductions = createEmployeeDto.InitialSalary.Deductions ?? 0,
                            SalaryMonth = createEmployeeDto.InitialSalary.SalaryMonth.HasValue
                                ? DateTime.SpecifyKind(createEmployeeDto.InitialSalary.SalaryMonth.Value, DateTimeKind.Utc)
                                : DateTime.SpecifyKind(new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1), DateTimeKind.Utc),
                            PaymentDate = createEmployeeDto.InitialSalary.PaymentDate.HasValue
                                ? DateTime.SpecifyKind(createEmployeeDto.InitialSalary.PaymentDate.Value, DateTimeKind.Utc)
                                : DateTime.UtcNow
                        };

                        // Calculate Net Salary
                        payroll.NetSalary = payroll.BasicSalary + payroll.HRA + payroll.Allowances - payroll.Deductions;

                        _context.Payrolls.Add(payroll);
                        await _context.SaveChangesAsync();
                        Console.WriteLine($"Payroll added successfully for employee ID: {employee.Id}");
                    }

                    // Commit the transaction
                    await transaction.CommitAsync();
                    Console.WriteLine("Transaction committed successfully");

                    // Get the employee with all related data for the response
                    var employeeWithDetails = await _context.Employees
                        .Include(e => e.Department)
                        .Include(e => e.Designation)
                        .Include(e => e.Manager)
                        .Include(e => e.BankDetail)
                        .FirstOrDefaultAsync(e => e.Id == employee.Id);

                    if (employeeWithDetails == null)
                    {
                        return NotFound($"Employee with ID {employee.Id} not found after creation");
                    }

                    // Map back to DTO for response
                    var employeeDto = employeeWithDetails.Adapt<EmployeeDTO>();

                    // Handle null references
                    if (employeeWithDetails.Department != null)
                    {
                        employeeDto.DepartmentName = employeeWithDetails.Department.Name;
                    }
                    else
                    {
                        employeeDto.DepartmentName = "N/A";
                    }

                    if (employeeWithDetails.Designation != null)
                    {
                        employeeDto.DesignationTitle = employeeWithDetails.Designation.Title;
                    }
                    else
                    {
                        employeeDto.DesignationTitle = "N/A";
                    }

                    if (employeeWithDetails.Manager != null)
                    {
                        employeeDto.ManagerName = employeeWithDetails.Manager.FullName;
                    }
                    else
                    {
                        employeeDto.ManagerName = "N/A";
                    }

                    return CreatedAtAction("GetEmployee", new { id = employee.Id }, employeeDto);
                }
                catch (Exception ex)
                {
                    // Rollback the transaction if any operation fails
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Error details: {ex.ToString()}");

                    // Log the full exception details including all inner exceptions
                    var currentEx = ex;
                    int exceptionLevel = 0;
                    StringBuilder errorDetails = new StringBuilder();

                    while (currentEx != null)
                    {
                        string message = $"Exception level {exceptionLevel}: {currentEx.Message}";
                        Console.WriteLine(message);
                        errorDetails.AppendLine(message);

                        string stackTrace = $"Stack trace: {currentEx.StackTrace}";
                        Console.WriteLine(stackTrace);
                        errorDetails.AppendLine(stackTrace);

                        currentEx = currentEx.InnerException;
                        exceptionLevel++;
                    }

                    return StatusCode(500, $"Internal server error: {errorDetails.ToString()}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PostEmployee: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Employees/{id}/BankDetails
        [HttpPost("{id}/BankDetails")]
        public async Task<ActionResult<BankDetailDTO>> AddBankDetails(Guid id, BankDetailDTO bankDetailDto)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);

                if (employee == null)
                {
                    return NotFound($"Employee with ID {id} not found");
                }

                // Ensure required fields are not null
                if (string.IsNullOrEmpty(bankDetailDto.BankName))
                    bankDetailDto.BankName = "Default Bank";

                if (string.IsNullOrEmpty(bankDetailDto.AccountNumber))
                    bankDetailDto.AccountNumber = "0000000000";

                if (string.IsNullOrEmpty(bankDetailDto.AccountHolderName))
                    bankDetailDto.AccountHolderName = employee.FullName;

                if (string.IsNullOrEmpty(bankDetailDto.IFSCCode))
                    bankDetailDto.IFSCCode = "DEFAULTCODE";

                if (string.IsNullOrEmpty(bankDetailDto.BranchName))
                    bankDetailDto.BranchName = "Main Branch";

                // Create bank detail directly
                var bankDetail = new BankDetail
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = id,
                    BankName = bankDetailDto.BankName,
                    AccountHolderName = bankDetailDto.AccountHolderName,
                    AccountNumber = bankDetailDto.AccountNumber,
                    IFSCCode = bankDetailDto.IFSCCode,
                    BranchName = bankDetailDto.BranchName
                };

                _context.BankDetails.Add(bankDetail);

                try
                {
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Bank details added successfully for employee ID: {id}");

                    // Return the created bank details
                    return CreatedAtAction("GetBankDetail", new { id = bankDetail.Id }, bankDetailDto);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error details: {ex.ToString()}");

                    // Log the full exception details including all inner exceptions
                    var currentEx = ex;
                    int exceptionLevel = 0;
                    StringBuilder errorDetails = new StringBuilder();

                    while (currentEx != null)
                    {
                        string message = $"Exception level {exceptionLevel}: {currentEx.Message}";
                        Console.WriteLine(message);
                        errorDetails.AppendLine(message);

                        string stackTrace = $"Stack trace: {currentEx.StackTrace}";
                        Console.WriteLine(stackTrace);
                        errorDetails.AppendLine(stackTrace);

                        currentEx = currentEx.InnerException;
                        exceptionLevel++;
                    }

                    return StatusCode(500, $"Internal server error: {errorDetails.ToString()}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddBankDetails: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Employees/{id}/Payroll
        [HttpPost("{id}/Payroll")]
        public async Task<ActionResult<PayrollDTO>> AddPayroll(Guid id, PayrollDTO payrollDto)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);

                if (employee == null)
                {
                    return NotFound($"Employee with ID {id} not found");
                }

                // Create payroll directly
                var payroll = new Payroll
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = id,
                    BasicSalary = payrollDto.BasicSalary ?? 0,
                    HRA = payrollDto.HRA ?? 0,
                    Allowances = payrollDto.Allowances ?? 0,
                    Deductions = payrollDto.Deductions ?? 0,
                    SalaryMonth = payrollDto.SalaryMonth.HasValue
                        ? DateTime.SpecifyKind(payrollDto.SalaryMonth.Value, DateTimeKind.Utc)
                        : DateTime.SpecifyKind(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DateTimeKind.Utc),
                    PaymentDate = payrollDto.PaymentDate.HasValue
                        ? DateTime.SpecifyKind(payrollDto.PaymentDate.Value, DateTimeKind.Utc)
                        : DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
                };

                // Calculate Net Salary
                payroll.NetSalary = payroll.BasicSalary + payroll.HRA + payroll.Allowances - payroll.Deductions;

                _context.Payrolls.Add(payroll);

                try
                {
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Payroll added successfully for employee ID: {id}");

                    // Set the calculated net salary in the response DTO
                    payrollDto.NetSalary = payroll.NetSalary;

                    // Return the created payroll
                    return CreatedAtAction("GetPayroll", new { id = payroll.Id }, payrollDto);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error details: {ex.ToString()}");

                    // Log the full exception details including all inner exceptions
                    var currentEx = ex;
                    int exceptionLevel = 0;
                    StringBuilder errorDetails = new StringBuilder();

                    while (currentEx != null)
                    {
                        string message = $"Exception level {exceptionLevel}: {currentEx.Message}";
                        Console.WriteLine(message);
                        errorDetails.AppendLine(message);

                        string stackTrace = $"Stack trace: {currentEx.StackTrace}";
                        Console.WriteLine(stackTrace);
                        errorDetails.AppendLine(stackTrace);

                        currentEx = currentEx.InnerException;
                        exceptionLevel++;
                    }

                    return StatusCode(500, $"Internal server error: {errorDetails.ToString()}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddPayroll: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Employees/{id}/BankDetails
        [HttpGet("{id}/BankDetails")]
        public async Task<ActionResult<BankDetailDTO>> GetBankDetail(Guid id)
        {
            var bankDetail = await _context.BankDetails.FirstOrDefaultAsync(b => b.EmployeeId == id);

            if (bankDetail == null)
            {
                return NotFound($"Bank details for employee with ID {id} not found");
            }

            return bankDetail.Adapt<BankDetailDTO>();
        }

        // GET: api/Employees/{id}/Payroll
        [HttpGet("{id}/Payroll")]
        public async Task<ActionResult<PayrollDTO>> GetPayroll(Guid id)
        {
            var payroll = await _context.Payrolls.FirstOrDefaultAsync(p => p.EmployeeId == id);

            if (payroll == null)
            {
                return NotFound($"Payroll for employee with ID {id} not found");
            }

            return payroll.Adapt<PayrollDTO>();
        }

        // PUT: api/Employees/5
        [HttpPut("{id}")]
        [HrmsApi.Attributes.Authorize("Admin")]
        public async Task<IActionResult> PutEmployee(Guid id, UpdateEmployeeDTO updateEmployeeDto)
        {
            if (id != updateEmployeeDto.Id)
            {
                return BadRequest();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Manager)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            // Track changes for history
            var changes = new List<EmployeeChangeDetail>();

            // Check for changes in each field
            if (employee.FullName != updateEmployeeDto.FullName)
            {
                changes.Add(new EmployeeChangeDetail
                {
                    Action = "Update",
                    FieldChanged = "FullName",
                    OldValue = employee.FullName ?? "Not set",
                    NewValue = updateEmployeeDto.FullName ?? "Not set"
                });
            }

            if (employee.Email != updateEmployeeDto.Email)
            {
                changes.Add(new EmployeeChangeDetail
                {
                    Action = "Update",
                    FieldChanged = "Email",
                    OldValue = employee.Email ?? "Not set",
                    NewValue = updateEmployeeDto.Email ?? "Not set"
                });
            }

            if (employee.ContactNumber != updateEmployeeDto.ContactNumber)
            {
                changes.Add(new EmployeeChangeDetail
                {
                    Action = "Update",
                    FieldChanged = "ContactNumber",
                    OldValue = employee.ContactNumber ?? "Not set",
                    NewValue = updateEmployeeDto.ContactNumber ?? "Not set"
                });
            }

            if (employee.DepartmentId != updateEmployeeDto.DepartmentId)
            {
                var oldDepartment = employee.Department?.Name ?? "Unknown";
                var newDepartment = await _context.Departments
                    .Where(d => d.Id == updateEmployeeDto.DepartmentId)
                    .Select(d => d.Name)
                    .FirstOrDefaultAsync() ?? "Unknown";

                changes.Add(new EmployeeChangeDetail
                {
                    Action = "Update",
                    FieldChanged = "Department",
                    OldValue = oldDepartment,
                    NewValue = newDepartment
                });
            }

            if (employee.DesignationId != updateEmployeeDto.DesignationId)
            {
                var oldDesignation = employee.Designation?.Title ?? "Unknown";
                var newDesignation = await _context.Designations
                    .Where(d => d.Id == updateEmployeeDto.DesignationId)
                    .Select(d => d.Title)
                    .FirstOrDefaultAsync() ?? "Unknown";

                changes.Add(new EmployeeChangeDetail
                {
                    Action = "Update",
                    FieldChanged = "Designation",
                    OldValue = oldDesignation,
                    NewValue = newDesignation
                });
            }

            if (employee.ManagerId != updateEmployeeDto.ManagerId)
            {
                var oldManager = employee.Manager?.FullName ?? "None";
                var newManager = "None";

                if (updateEmployeeDto.ManagerId.HasValue)
                {
                    newManager = await _context.Employees
                        .Where(e => e.Id == updateEmployeeDto.ManagerId)
                        .Select(e => e.FullName)
                        .FirstOrDefaultAsync() ?? "Unknown";
                }

                changes.Add(new EmployeeChangeDetail
                {
                    Action = "Update",
                    FieldChanged = "Manager",
                    OldValue = oldManager,
                    NewValue = newManager
                });
            }

            // Create a copy of the employee to track changes
            var oldEmployee = new Domain.Employee
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Email = employee.Email,
                ContactNumber = employee.ContactNumber,
                Gender = employee.Gender,
                DateOfBirth = employee.DateOfBirth,
                MaritalStatus = employee.MaritalStatus,
                NationalIdNumber = employee.NationalIdNumber,
                DepartmentId = employee.DepartmentId,
                DesignationId = employee.DesignationId,
                ManagerId = employee.ManagerId,
                JoiningDate = employee.JoiningDate,
                ExitDate = employee.ExitDate,
                IsActive = employee.IsActive
            };

            // Manually update date fields to ensure proper UTC kind
            if (updateEmployeeDto.DateOfBirth.HasValue)
            {
                employee.DateOfBirth = DateTime.SpecifyKind(updateEmployeeDto.DateOfBirth.Value, DateTimeKind.Utc);
            }

            if (updateEmployeeDto.JoiningDate.HasValue)
            {
                employee.JoiningDate = DateTime.SpecifyKind(updateEmployeeDto.JoiningDate.Value, DateTimeKind.Utc);
            }

            if (updateEmployeeDto.ExitDate.HasValue)
            {
                employee.ExitDate = DateTime.SpecifyKind(updateEmployeeDto.ExitDate.Value, DateTimeKind.Utc);
            }

            // Update non-date fields using Mapster
            var config = new TypeAdapterConfig();
            config.ForType<UpdateEmployeeDTO, Domain.Employee>()
                .Ignore(dest => dest.DateOfBirth)
                .Ignore(dest => dest.JoiningDate)
                .Ignore(dest => dest.ExitDate);

            updateEmployeeDto.Adapt(employee, config);

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                // Use the execution strategy to handle transactions properly
                var strategy = _context.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    // Execute all operations in a single transaction
                    using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        // Save employee changes
                        await _context.SaveChangesAsync();

                        // Update or create bank details if provided
                        if (updateEmployeeDto.BankDetail != null)
                        {
                            // Check if bank details already exist
                            var bankDetail = await _context.BankDetails
                                .FirstOrDefaultAsync(bd => bd.EmployeeId == id);

                            if (bankDetail != null)
                            {
                                // Update existing bank details
                                bankDetail.BankName = updateEmployeeDto.BankDetail.BankName;
                                bankDetail.AccountNumber = updateEmployeeDto.BankDetail.AccountNumber;
                                bankDetail.IFSCCode = updateEmployeeDto.BankDetail.IFSCCode;
                                bankDetail.AccountHolderName = updateEmployeeDto.BankDetail.AccountHolderName;
                                bankDetail.BranchName = updateEmployeeDto.BankDetail.BranchName;
                                _context.Entry(bankDetail).State = EntityState.Modified;

                                // Add to change history
                                changes.Add(new EmployeeChangeDetail
                                {
                                    Action = "Update",
                                    FieldChanged = "BankDetails",
                                    OldValue = "Existing bank details",
                                    NewValue = $"{updateEmployeeDto.BankDetail.BankName} - {updateEmployeeDto.BankDetail.AccountNumber}"
                                });
                            }
                            else
                            {
                                // Create new bank details
                                var newBankDetail = new BankDetail
                                {
                                    Id = Guid.NewGuid(),
                                    EmployeeId = id,
                                    BankName = updateEmployeeDto.BankDetail.BankName,
                                    AccountNumber = updateEmployeeDto.BankDetail.AccountNumber,
                                    IFSCCode = updateEmployeeDto.BankDetail.IFSCCode,
                                    AccountHolderName = updateEmployeeDto.BankDetail.AccountHolderName,
                                    BranchName = updateEmployeeDto.BankDetail.BranchName
                                };
                                _context.BankDetails.Add(newBankDetail);

                                // Add to change history
                                changes.Add(new EmployeeChangeDetail
                                {
                                    Action = "Add",
                                    FieldChanged = "BankDetails",
                                    OldValue = "None",
                                    NewValue = $"{updateEmployeeDto.BankDetail.BankName} - {updateEmployeeDto.BankDetail.AccountNumber}"
                                });
                            }

                            await _context.SaveChangesAsync();
                        }

                        // Update or create payroll information if provided
                        if (updateEmployeeDto.InitialSalary != null)
                        {
                            // Create a new payroll entry
                            var payroll = new Payroll
                            {
                                Id = Guid.NewGuid(),
                                EmployeeId = id,
                                // EmployeeName is not in the Payroll entity
                                BasicSalary = updateEmployeeDto.InitialSalary.BasicSalary ?? 0,
                                HRA = updateEmployeeDto.InitialSalary.HRA ?? 0,
                                Allowances = updateEmployeeDto.InitialSalary.Allowances ?? 0,
                                Deductions = updateEmployeeDto.InitialSalary.Deductions ?? 0,
                                NetSalary = updateEmployeeDto.InitialSalary.NetSalary ?? 0,
                                SalaryMonth = updateEmployeeDto.InitialSalary.SalaryMonth ?? DateTime.UtcNow,
                                PaymentDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
                            };

                            _context.Payrolls.Add(payroll);

                            // Add to change history
                            changes.Add(new EmployeeChangeDetail
                            {
                                Action = "Update",
                                FieldChanged = "Salary",
                                OldValue = "Previous salary",
                                NewValue = $"Basic: {updateEmployeeDto.InitialSalary.BasicSalary}, Net: {updateEmployeeDto.InitialSalary.NetSalary}"
                            });

                            await _context.SaveChangesAsync();
                        }

                        // Record history for each change
                        if (changes.Count > 0)
                        {
                            try
                            {
                                // Get or create employee history
                                var employeeHistory = await _context.EmployeeHistories
                                    .FirstOrDefaultAsync(eh => eh.EmployeeId == id);

                                if (employeeHistory == null)
                                {
                                    employeeHistory = new EmployeeHistory
                                    {
                                        Id = Guid.NewGuid(),
                                        EmployeeId = id,
                                        EmployeeName = employee.FullName
                                    };
                                    _context.EmployeeHistories.Add(employeeHistory);
                                }

                                // Ensure all DateTime objects in changes are UTC
                                foreach (var change in changes)
                                {
                                    if (change.Timestamp != default)
                                    {
                                        change.Timestamp = DateTime.SpecifyKind(change.Timestamp, DateTimeKind.Utc);
                                    }
                                }

                                // Use the helper method to add changes with proper DateTime handling
                                employeeHistory.AddChanges(DateTime.UtcNow, changes);
                                await _context.SaveChangesAsync();
                            }
                            catch (Exception ex)
                            {
                                // Log the error but don't fail the update
                                _logger.LogError(ex, "Error updating employee history for employee {EmployeeId}", id);
                            }
                        }

                        // Commit transaction
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction if any operation fails
                        _logger.LogError(ex, "Error updating employee {EmployeeId}", id);
                        await transaction.RollbackAsync();
                        throw;
                    }
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        [HrmsApi.Attributes.Authorize("Admin")]
        public async Task<ActionResult<EmployeeDTO>> DeleteEmployee(Guid id)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Manager)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            // Map to DTO before removing
            var employeeDto = employee.Adapt<EmployeeDTO>();

            // Use the execution strategy to handle transactions properly
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                // Begin transaction
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Record deletion in history
                    EmployeeHistory employeeHistory = null;

                    try
                    {
                        employeeHistory = await _context.EmployeeHistories
                            .FirstOrDefaultAsync(eh => eh.EmployeeId == id);

                        if (employeeHistory == null)
                        {
                            employeeHistory = new EmployeeHistory
                            {
                                Id = Guid.NewGuid(),
                                EmployeeId = id,
                                EmployeeName = employee.FullName
                            };
                            _context.EmployeeHistories.Add(employeeHistory);
                        }

                        // Create the deletion record
                        var deletionChanges = new List<EmployeeChangeDetail>
                        {
                            new EmployeeChangeDetail
                            {
                                Action = "Delete",
                                FieldChanged = "Employee",
                                OldValue = $"Employee: {employee.FullName}, ID: {employee.Id}",
                                NewValue = "Deleted",
                                Timestamp = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
                            }
                        };

                        // Use the helper method to add changes with proper DateTime handling
                        employeeHistory.AddChanges(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc), deletionChanges);
                    }
                    catch (Exception ex)
                    {
                        // Log the error but don't fail the delete operation
                        _logger.LogError(ex, "Error updating employee history for employee {EmployeeId} during deletion", id);

                        // Create a minimal history record if the main one failed
                        employeeHistory = new EmployeeHistory
                        {
                            Id = Guid.NewGuid(),
                            EmployeeId = id,
                            EmployeeName = employee?.FullName ?? "Unknown"
                        };

                        // Create the deletion record
                        var deletionChanges = new List<EmployeeChangeDetail>
                        {
                            new EmployeeChangeDetail
                            {
                                Action = "Delete",
                                FieldChanged = "Employee",
                                OldValue = $"Employee: {employee?.FullName ?? "Unknown"}, ID: {id}",
                                NewValue = "Deleted",
                                Timestamp = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
                            }
                        };

                        // Use the helper method to add changes with proper DateTime handling
                        employeeHistory.AddChanges(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc), deletionChanges);
                        _context.EmployeeHistories.Add(employeeHistory);
                    }

                    // Remove the employee
                    _context.Employees.Remove(employee);

                    // Save all changes
                    await _context.SaveChangesAsync();

                    // Commit transaction
                    await transaction.CommitAsync();
                }
                catch
                {
                    // Rollback transaction if any operation fails
                    await transaction.RollbackAsync();
                    throw;
                }
            });

            return employeeDto;
        }

        // GET: api/Employees/test
        [HttpGet("test")]
        public async Task<ActionResult<object>> GetEmployeesTest()
        {
            try
            {
                var employees = await _context.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Designation)
                    .Include(e => e.Manager)
                    .ToListAsync();

                var result = employees.Select(e => new {
                    Id = e.Id,
                    FullName = e.FullName,
                    Email = e.Email,
                    DepartmentId = e.DepartmentId,
                    DepartmentName = e.Department?.Name ?? "No Department",
                    DesignationId = e.DesignationId,
                    DesignationTitle = e.Designation?.Title ?? "No Position",
                    HasDepartment = e.Department != null,
                    HasDesignation = e.Designation != null
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private bool EmployeeExists(Guid id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }

        // Implement IDisposable to properly dispose of database connections
        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    _context?.Dispose();
                }

                // Free unmanaged resources
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~EmployeesController()
        {
            Dispose(false);
        }
    }
}
