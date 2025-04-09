using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HrmsApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CheckIn = table.Column<TimeSpan>(type: "interval", nullable: false),
                    CheckOut = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Designations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Designations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeCode = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    ContactNumber = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MaritalStatus = table.Column<string>(type: "text", nullable: false),
                    NationalIdNumber = table.Column<string>(type: "text", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    DesignationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ManagerId = table.Column<Guid>(type: "uuid", nullable: true),
                    JoiningDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExitDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EmploymentType = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeaveRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LeaveType = table.Column<string>(type: "text", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApprovedBy = table.Column<string>(type: "text", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Comments = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { new Guid("01234567-89ab-cdef-0123-456789abcdef"), "IT", "IT" },
                    { new Guid("12345678-89ab-cdef-0123-456789abcdef"), "HR", "HR" },
                    { new Guid("23456789-89ab-cdef-0123-456789abcdef"), "FIN", "Finance" }
                });

            migrationBuilder.InsertData(
                table: "Designations",
                columns: new[] { "Id", "Title" },
                values: new object[,]
                {
                    { new Guid("34567890-89ab-cdef-0123-456789abcdef"), "Software Engineer" },
                    { new Guid("45678901-89ab-cdef-0123-456789abcdef"), "HR Manager" },
                    { new Guid("56789012-89ab-cdef-0123-456789abcdef"), "Financial Analyst" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "ContactNumber", "DateOfBirth", "DepartmentId", "DesignationId", "Email", "EmployeeCode", "EmploymentType", "ExitDate", "FullName", "Gender", "IsActive", "JoiningDate", "ManagerId", "MaritalStatus", "NationalIdNumber" },
                values: new object[,]
                {
                    { new Guid("67890123-89ab-cdef-0123-456789abcdef"), "1234567890", new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("01234567-89ab-cdef-0123-456789abcdef"), new Guid("34567890-89ab-cdef-0123-456789abcdef"), "john.doe@example.com", "EMP001", "Full-Time", null, "John Doe", "Male", true, new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Single", "ABC123456" },
                    { new Guid("78901234-89ab-cdef-0123-456789abcdef"), "0987654321", new DateTime(1992, 5, 15, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("12345678-89ab-cdef-0123-456789abcdef"), new Guid("45678901-89ab-cdef-0123-456789abcdef"), "jane.smith@example.com", "EMP002", "Full-Time", null, "Jane Smith", "Female", true, new DateTime(2019, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Married", "XYZ987654" },
                    { new Guid("89012345-89ab-cdef-0123-456789abcdef"), "5556667777", new DateTime(1985, 10, 20, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("23456789-89ab-cdef-0123-456789abcdef"), new Guid("56789012-89ab-cdef-0123-456789abcdef"), "bob.johnson@example.com", "EMP003", "Full-Time", null, "Bob Johnson", "Male", true, new DateTime(2018, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc), null, "Divorced", "PQR456789" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "PasswordHash", "Role", "Username" },
                values: new object[] { 1, new DateTime(2025, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "admin@hrms.com", "$2a$11$ej7Hx5XCUUvG4FjmOKjI8.UY6LqUu5VQmmQxqGbzsiLRCRfZTnxDW", "Admin", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attendances");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Designations");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "LeaveRequests");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
