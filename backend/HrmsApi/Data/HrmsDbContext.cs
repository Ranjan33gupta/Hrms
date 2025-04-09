using Microsoft.EntityFrameworkCore;
using HrmsApi.Modules.Employee.Domain;
using HrmsApi.Modules.Leave.Domain;
using HrmsApi.Modules.Auth.Domain;
using HrmsApi.Modules.Settings.Domain;
using HrmsApi.Modules.Attendance.Domain;
using HrmsApi.Modules.Dashboard.Domain;
using HrmsApi.Modules.Chatbot.Domain;

namespace HrmsApi.Data
{
    public class HrmsDbContext : DbContext
    {
        public HrmsDbContext(DbContextOptions<HrmsDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<LeavePolicy> LeavePolicies { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<HrmsApi.Modules.Attendance.Domain.Attendance> Attendances { get; set; } = null!;
        public DbSet<Shift> Shifts { get; set; } = null!;
        public DbSet<BankDetail> BankDetails { get; set; }
        public DbSet<Payroll> Payrolls { get; set; }
        public DbSet<EmployeeHistory> EmployeeHistories { get; set; }
        public DbSet<PayrollHistory> PayrollHistories { get; set; }
        public DbSet<EmployeeShiftAssignment> EmployeeShiftAssignments { get; set; }
        public DbSet<AttendancePhoto> AttendancePhotos { get; set; } = null!;
        
        // Dashboard module
        public DbSet<MoodEntry> MoodEntries { get; set; } = null!;
        public DbSet<HrmsApi.Modules.Dashboard.Domain.ChatbotIntent> DashboardChatbotIntents { get; set; } = null!;
        public DbSet<HrmsApi.Modules.Dashboard.Domain.ChatbotTrainingPhrase> DashboardChatbotTrainingPhrases { get; set; } = null!;
        public DbSet<HrmsApi.Modules.Dashboard.Domain.ChatbotEntity> DashboardChatbotEntities { get; set; } = null!;
        public DbSet<MotivationalQuote> MotivationalQuotes { get; set; } = null!;
        
        // Chatbot module
        public DbSet<HrmsApi.Modules.Chatbot.Domain.ChatbotIntent> ChatbotIntents { get; set; } = null!;
        public DbSet<HrmsApi.Modules.Chatbot.Domain.ChatbotTrainingPhrase> ChatbotTrainingPhrases { get; set; } = null!;
        public DbSet<HrmsApi.Modules.Chatbot.Domain.ChatbotResponse> ChatbotResponses { get; set; } = null!;
        public DbSet<HrmsApi.Modules.Chatbot.Domain.ChatbotConversation> ChatbotConversations { get; set; } = null!;
        public DbSet<HrmsApi.Modules.Chatbot.Domain.ChatbotMessage> ChatbotMessages { get; set; } = null!;
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships for Employee, BankDetail, and Payroll
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.BankDetail)
                .WithOne(b => b.Employee)
                .HasForeignKey<BankDetail>(b => b.EmployeeId);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Payrolls)
                .WithOne(p => p.Employee)
                .HasForeignKey(p => p.EmployeeId);
                
            // Configure EmployeeHistory entity
            modelBuilder.Entity<EmployeeHistory>()
                .HasOne(eh => eh.Employee)
                .WithMany()
                .HasForeignKey(eh => eh.EmployeeId);
                
            // Configure jsonb column for EmployeeHistory
            modelBuilder.Entity<EmployeeHistory>()
                .Property(e => e.EmployeeChangeDetails)
                .HasColumnType("jsonb");
                
            // Configure PayrollHistory entity
            modelBuilder.Entity<PayrollHistory>()
                .HasOne(ph => ph.Employee)
                .WithMany()
                .HasForeignKey(ph => ph.EmployeeId);
                
            modelBuilder.Entity<PayrollHistory>()
                .HasOne(ph => ph.Payroll)
                .WithMany()
                .HasForeignKey(ph => ph.PayrollId);
                
            // Configure jsonb column for PayrollHistory
            modelBuilder.Entity<PayrollHistory>()
                .Property(ph => ph.PayrollChanges)
                .HasColumnType("jsonb");
                
            // Configure Attendance and Shift relationships
            modelBuilder.Entity<HrmsApi.Modules.Attendance.Domain.Attendance>()
                .HasOne(a => a.Shift)
                .WithMany()
                .HasForeignKey(a => a.ShiftId);
                
            modelBuilder.Entity<EmployeeShiftAssignment>()
                .HasOne(esa => esa.Shift)
                .WithMany()
                .HasForeignKey(esa => esa.ShiftId);
                
            // Seed departments
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = Guid.Parse("01234567-89ab-cdef-0123-456789abcdef"), Name = "IT", Code = "IT" },
                new Department { Id = Guid.Parse("12345678-89ab-cdef-0123-456789abcdef"), Name = "HR", Code = "HR" },
                new Department { Id = Guid.Parse("23456789-89ab-cdef-0123-456789abcdef"), Name = "Finance", Code = "FIN" }
            );

            // Seed designations
            modelBuilder.Entity<Designation>().HasData(
                new Designation { Id = Guid.Parse("34567890-89ab-cdef-0123-456789abcdef"), Title = "Software Engineer" },
                new Designation { Id = Guid.Parse("45678901-89ab-cdef-0123-456789abcdef"), Title = "HR Manager" },
                new Designation { Id = Guid.Parse("56789012-89ab-cdef-0123-456789abcdef"), Title = "Financial Analyst" }
            );

            // Seed some initial employees
            modelBuilder.Entity<Employee>().HasData(
                new Employee { 
                    Id = Guid.Parse("67890123-89ab-cdef-0123-456789abcdef"),
                    EmployeeCode = "EMP001",
                    FullName = "John Doe",
                    Email = "john.doe@example.com",
                    ContactNumber = "1234567890",
                    Gender = "Male",
                    DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    MaritalStatus = "Single",
                    NationalIdNumber = "ABC123456",
                    DepartmentId = Guid.Parse("01234567-89ab-cdef-0123-456789abcdef"),
                    DesignationId = Guid.Parse("34567890-89ab-cdef-0123-456789abcdef"),
                    JoiningDate = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    EmploymentType = "Full-Time",
                    IsActive = true
                },
                new Employee { 
                    Id = Guid.Parse("78901234-89ab-cdef-0123-456789abcdef"),
                    EmployeeCode = "EMP002",
                    FullName = "Jane Smith",
                    Email = "jane.smith@example.com",
                    ContactNumber = "0987654321",
                    Gender = "Female",
                    DateOfBirth = new DateTime(1992, 5, 15, 0, 0, 0, DateTimeKind.Utc),
                    MaritalStatus = "Married",
                    NationalIdNumber = "XYZ987654",
                    DepartmentId = Guid.Parse("12345678-89ab-cdef-0123-456789abcdef"),
                    DesignationId = Guid.Parse("45678901-89ab-cdef-0123-456789abcdef"),
                    JoiningDate = new DateTime(2019, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                    EmploymentType = "Full-Time",
                    IsActive = true
                },
                new Employee { 
                    Id = Guid.Parse("89012345-89ab-cdef-0123-456789abcdef"),
                    EmployeeCode = "EMP003",
                    FullName = "Bob Johnson",
                    Email = "bob.johnson@example.com",
                    ContactNumber = "5556667777",
                    Gender = "Male",
                    DateOfBirth = new DateTime(1985, 10, 20, 0, 0, 0, DateTimeKind.Utc),
                    MaritalStatus = "Divorced",
                    NationalIdNumber = "PQR456789",
                    DepartmentId = Guid.Parse("23456789-89ab-cdef-0123-456789abcdef"),
                    DesignationId = Guid.Parse("56789012-89ab-cdef-0123-456789abcdef"),
                    JoiningDate = new DateTime(2018, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                    EmploymentType = "Full-Time",
                    IsActive = true
                }
            );

            // Seed an admin user (password: admin123)
            // Using a pre-computed hash value for "admin123"
            modelBuilder.Entity<User>().HasData(
                new User 
                { 
                    Id = 1, 
                    Username = "admin", 
                    Email = "admin@hrms.com", 
                    PasswordHash = "$2a$11$ej7Hx5XCUUvG4FjmOKjI8.UY6LqUu5VQmmQxqGbzsiLRCRfZTnxDW", // pre-computed hash for "admin123"
                    Role = "Admin",
                    CreatedAt = new DateTime(2025, 4, 8, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
