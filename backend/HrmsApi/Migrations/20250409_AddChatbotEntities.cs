using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HrmsApi.Migrations
{
    public partial class AddChatbotEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if ChatbotIntents table already exists
            if (!TableExists(migrationBuilder, "ChatbotIntents"))
            {
                migrationBuilder.CreateTable(
                    name: "ChatbotIntents",
                    columns: table => new
                    {
                        Id = table.Column<Guid>(type: "uuid", nullable: false),
                        Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                        Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                        RequiredRole = table.Column<string>(type: "text", nullable: true),
                        ApiEndpoint = table.Column<string>(type: "text", nullable: true),
                        RouteDestination = table.Column<string>(type: "text", nullable: true),
                        RequiresAuth = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_ChatbotIntents", x => x.Id);
                    });
            }

            // Check if ChatbotTrainingPhrases table already exists
            if (!TableExists(migrationBuilder, "ChatbotTrainingPhrases"))
            {
                migrationBuilder.CreateTable(
                    name: "ChatbotTrainingPhrases",
                    columns: table => new
                    {
                        Id = table.Column<Guid>(type: "uuid", nullable: false),
                        Phrase = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                        IntentId = table.Column<Guid>(type: "uuid", nullable: false)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_ChatbotTrainingPhrases", x => x.Id);
                        table.ForeignKey(
                            name: "FK_ChatbotTrainingPhrases_ChatbotIntents_IntentId",
                            column: x => x.IntentId,
                            principalTable: "ChatbotIntents",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                    });

                migrationBuilder.CreateIndex(
                    name: "IX_ChatbotTrainingPhrases_IntentId",
                    table: "ChatbotTrainingPhrases",
                    column: "IntentId");
            }

            // Check if ChatbotResponses table already exists
            if (!TableExists(migrationBuilder, "ChatbotResponses"))
            {
                migrationBuilder.CreateTable(
                    name: "ChatbotResponses",
                    columns: table => new
                    {
                        Id = table.Column<Guid>(type: "uuid", nullable: false),
                        Response = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                        IntentId = table.Column<Guid>(type: "uuid", nullable: false),
                        Priority = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_ChatbotResponses", x => x.Id);
                        table.ForeignKey(
                            name: "FK_ChatbotResponses_ChatbotIntents_IntentId",
                            column: x => x.IntentId,
                            principalTable: "ChatbotIntents",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                    });

                migrationBuilder.CreateIndex(
                    name: "IX_ChatbotResponses_IntentId",
                    table: "ChatbotResponses",
                    column: "IntentId");
            }

            // Check if ChatbotConversations table already exists
            if (!TableExists(migrationBuilder, "ChatbotConversations"))
            {
                migrationBuilder.CreateTable(
                    name: "ChatbotConversations",
                    columns: table => new
                    {
                        Id = table.Column<Guid>(type: "uuid", nullable: false),
                        EmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                        StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                        EndedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_ChatbotConversations", x => x.Id);
                        table.ForeignKey(
                            name: "FK_ChatbotConversations_Employees_EmployeeId",
                            column: x => x.EmployeeId,
                            principalTable: "Employees",
                            principalColumn: "Id");
                    });

                migrationBuilder.CreateIndex(
                    name: "IX_ChatbotConversations_EmployeeId",
                    table: "ChatbotConversations",
                    column: "EmployeeId");
            }

            // Check if ChatbotMessages table already exists
            if (!TableExists(migrationBuilder, "ChatbotMessages"))
            {
                migrationBuilder.CreateTable(
                    name: "ChatbotMessages",
                    columns: table => new
                    {
                        Id = table.Column<Guid>(type: "uuid", nullable: false),
                        ConversationId = table.Column<Guid>(type: "uuid", nullable: false),
                        Message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                        IsFromUser = table.Column<bool>(type: "boolean", nullable: false),
                        Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                        IntentId = table.Column<Guid>(type: "uuid", nullable: true),
                        ConfidenceScore = table.Column<double>(type: "double precision", nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_ChatbotMessages", x => x.Id);
                        table.ForeignKey(
                            name: "FK_ChatbotMessages_ChatbotConversations_ConversationId",
                            column: x => x.ConversationId,
                            principalTable: "ChatbotConversations",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                        table.ForeignKey(
                            name: "FK_ChatbotMessages_ChatbotIntents_IntentId",
                            column: x => x.IntentId,
                            principalTable: "ChatbotIntents",
                            principalColumn: "Id");
                    });

                migrationBuilder.CreateIndex(
                    name: "IX_ChatbotMessages_ConversationId",
                    table: "ChatbotMessages",
                    column: "ConversationId");

                migrationBuilder.CreateIndex(
                    name: "IX_ChatbotMessages_IntentId",
                    table: "ChatbotMessages",
                    column: "IntentId");
            }

            // Seed initial intents and training phrases
            SeedInitialData(migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatbotMessages");

            migrationBuilder.DropTable(
                name: "ChatbotResponses");

            migrationBuilder.DropTable(
                name: "ChatbotTrainingPhrases");

            migrationBuilder.DropTable(
                name: "ChatbotConversations");

            migrationBuilder.DropTable(
                name: "ChatbotIntents");
        }

        private bool TableExists(MigrationBuilder migrationBuilder, string tableName)
        {
            var sql = $@"
                SELECT EXISTS (
                    SELECT FROM information_schema.tables 
                    WHERE table_schema = 'public'
                    AND table_name = '{tableName}'
                );";

            return migrationBuilder.Sql(sql) != null;
        }

        private void SeedInitialData(MigrationBuilder migrationBuilder)
        {
            // Seed help intent
            var helpIntentId = Guid.NewGuid();
            migrationBuilder.InsertData(
                table: "ChatbotIntents",
                columns: new[] { "Id", "Name", "Description", "RequiredRole", "ApiEndpoint", "RouteDestination", "RequiresAuth" },
                values: new object[] { helpIntentId, "help", "General help intent", null, null, null, false });

            // Seed help training phrases
            migrationBuilder.InsertData(
                table: "ChatbotTrainingPhrases",
                columns: new[] { "Id", "Phrase", "IntentId" },
                values: new object[] { Guid.NewGuid(), "help", helpIntentId });
            migrationBuilder.InsertData(
                table: "ChatbotTrainingPhrases",
                columns: new[] { "Id", "Phrase", "IntentId" },
                values: new object[] { Guid.NewGuid(), "what can you do", helpIntentId });
            migrationBuilder.InsertData(
                table: "ChatbotTrainingPhrases",
                columns: new[] { "Id", "Phrase", "IntentId" },
                values: new object[] { Guid.NewGuid(), "how do you work", helpIntentId });

            // Seed help responses
            migrationBuilder.InsertData(
                table: "ChatbotResponses",
                columns: new[] { "Id", "Response", "IntentId", "Priority" },
                values: new object[] { Guid.NewGuid(), "I'm your WorkNest assistant! I can help you navigate the system, answer questions about HR policies, and provide information about your attendance, leave, and more.", helpIntentId, 1 });

            // Seed leave policy intent
            var leavePolicyIntentId = Guid.NewGuid();
            migrationBuilder.InsertData(
                table: "ChatbotIntents",
                columns: new[] { "Id", "Name", "Description", "RequiredRole", "ApiEndpoint", "RouteDestination", "RequiresAuth" },
                values: new object[] { leavePolicyIntentId, "leave_policy", "Information about leave policies", null, null, null, false });

            // Seed leave policy training phrases
            migrationBuilder.InsertData(
                table: "ChatbotTrainingPhrases",
                columns: new[] { "Id", "Phrase", "IntentId" },
                values: new object[] { Guid.NewGuid(), "what is the leave policy", leavePolicyIntentId });
            migrationBuilder.InsertData(
                table: "ChatbotTrainingPhrases",
                columns: new[] { "Id", "Phrase", "IntentId" },
                values: new object[] { Guid.NewGuid(), "tell me about leave policy", leavePolicyIntentId });
            migrationBuilder.InsertData(
                table: "ChatbotTrainingPhrases",
                columns: new[] { "Id", "Phrase", "IntentId" },
                values: new object[] { Guid.NewGuid(), "how many leaves do I get", leavePolicyIntentId });

            // Seed leave policy responses
            migrationBuilder.InsertData(
                table: "ChatbotResponses",
                columns: new[] { "Id", "Response", "IntentId", "Priority" },
                values: new object[] { Guid.NewGuid(), "Our standard leave policy provides 20 days of annual leave, 10 days of sick leave, and 5 days of personal leave per year. Special leave types like maternity, paternity, and bereavement leave are also available.", leavePolicyIntentId, 1 });

            // Seed navigation intents
            var dashboardNavIntentId = Guid.NewGuid();
            migrationBuilder.InsertData(
                table: "ChatbotIntents",
                columns: new[] { "Id", "Name", "Description", "RequiredRole", "ApiEndpoint", "RouteDestination", "RequiresAuth" },
                values: new object[] { dashboardNavIntentId, "navigate_dashboard", "Navigate to dashboard", null, null, "/employee-dashboard", true });

            // Seed dashboard navigation training phrases
            migrationBuilder.InsertData(
                table: "ChatbotTrainingPhrases",
                columns: new[] { "Id", "Phrase", "IntentId" },
                values: new object[] { Guid.NewGuid(), "take me to the dashboard", dashboardNavIntentId });
            migrationBuilder.InsertData(
                table: "ChatbotTrainingPhrases",
                columns: new[] { "Id", "Phrase", "IntentId" },
                values: new object[] { Guid.NewGuid(), "go to dashboard", dashboardNavIntentId });
            migrationBuilder.InsertData(
                table: "ChatbotTrainingPhrases",
                columns: new[] { "Id", "Phrase", "IntentId" },
                values: new object[] { Guid.NewGuid(), "show me my dashboard", dashboardNavIntentId });

            // Seed dashboard navigation responses
            migrationBuilder.InsertData(
                table: "ChatbotResponses",
                columns: new[] { "Id", "Response", "IntentId", "Priority" },
                values: new object[] { Guid.NewGuid(), "Taking you to your dashboard now.", dashboardNavIntentId, 1 });

            // Seed attendance navigation intent
            var attendanceNavIntentId = Guid.NewGuid();
            migrationBuilder.InsertData(
                table: "ChatbotIntents",
                columns: new[] { "Id", "Name", "Description", "RequiredRole", "ApiEndpoint", "RouteDestination", "RequiresAuth" },
                values: new object[] { attendanceNavIntentId, "navigate_attendance", "Navigate to attendance", null, null, "/attendance", true });

            // Seed attendance navigation training phrases
            migrationBuilder.InsertData(
                table: "ChatbotTrainingPhrases",
                columns: new[] { "Id", "Phrase", "IntentId" },
                values: new object[] { Guid.NewGuid(), "take me to attendance", attendanceNavIntentId });
            migrationBuilder.InsertData(
                table: "ChatbotTrainingPhrases",
                columns: new[] { "Id", "Phrase", "IntentId" },
                values: new object[] { Guid.NewGuid(), "go to attendance page", attendanceNavIntentId });
            migrationBuilder.InsertData(
                table: "ChatbotTrainingPhrases",
                columns: new[] { "Id", "Phrase", "IntentId" },
                values: new object[] { Guid.NewGuid(), "show me my attendance history", attendanceNavIntentId });

            // Seed attendance navigation responses
            migrationBuilder.InsertData(
                table: "ChatbotResponses",
                columns: new[] { "Id", "Response", "IntentId", "Priority" },
                values: new object[] { Guid.NewGuid(), "Taking you to the attendance page now.", attendanceNavIntentId, 1 });

            // Seed leave request navigation intent
            var leaveRequestNavIntentId = Guid.NewGuid();
            migrationBuilder.InsertData(
                table: "ChatbotIntents",
                columns: new[] { "Id", "Name", "Description", "RequiredRole", "ApiEndpoint", "RouteDestination", "RequiresAuth" },
                values: new object[] { leaveRequestNavIntentId, "navigate_leave_request", "Navigate to leave request form", null, null, "/request-leave", true });

            // Seed leave request navigation training phrases
            migrationBuilder.InsertData(
                table: "ChatbotTrainingPhrases",
                columns: new[] { "Id", "Phrase", "IntentId" },
                values: new object[] { Guid.NewGuid(), "take me to leave request", leaveRequestNavIntentId });
            migrationBuilder.InsertData(
                table: "ChatbotTrainingPhrases",
                columns: new[] { "Id", "Phrase", "IntentId" },
                values: new object[] { Guid.NewGuid(), "go to leave request form", leaveRequestNavIntentId });
            migrationBuilder.InsertData(
                table: "ChatbotTrainingPhrases",
                columns: new[] { "Id", "Phrase", "IntentId" },
                values: new object[] { Guid.NewGuid(), "I want to apply for leave", leaveRequestNavIntentId });

            // Seed leave request navigation responses
            migrationBuilder.InsertData(
                table: "ChatbotResponses",
                columns: new[] { "Id", "Response", "IntentId", "Priority" },
                values: new object[] { Guid.NewGuid(), "Taking you to the leave request form now.", leaveRequestNavIntentId, 1 });

            // Seed admin-specific intent
            var addEmployeeIntentId = Guid.NewGuid();
            migrationBuilder.InsertData(
                table: "ChatbotIntents",
                columns: new[] { "Id", "Name", "Description", "RequiredRole", "ApiEndpoint", "RouteDestination", "RequiresAuth" },
                values: new object[] { addEmployeeIntentId, "add_employee", "Add a new employee", "Admin", null, "/add-employee", true });

            // Seed add employee training phrases
            migrationBuilder.InsertData(
                table: "ChatbotTrainingPhrases",
                columns: new[] { "Id", "Phrase", "IntentId" },
                values: new object[] { Guid.NewGuid(), "add a new employee", addEmployeeIntentId });
            migrationBuilder.InsertData(
                table: "ChatbotTrainingPhrases",
                columns: new[] { "Id", "Phrase", "IntentId" },
                values: new object[] { Guid.NewGuid(), "create employee profile", addEmployeeIntentId });
            migrationBuilder.InsertData(
                table: "ChatbotTrainingPhrases",
                columns: new[] { "Id", "Phrase", "IntentId" },
                values: new object[] { Guid.NewGuid(), "register a new staff member", addEmployeeIntentId });

            // Seed add employee responses
            migrationBuilder.InsertData(
                table: "ChatbotResponses",
                columns: new[] { "Id", "Response", "IntentId", "Priority" },
                values: new object[] { Guid.NewGuid(), "Taking you to the add employee form now.", addEmployeeIntentId, 1 });
        }
    }
}
