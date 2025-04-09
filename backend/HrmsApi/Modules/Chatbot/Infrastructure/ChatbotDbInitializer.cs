using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using HrmsApi.Data;

namespace HrmsApi.Modules.Chatbot.Infrastructure
{
    public static class ChatbotDbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider, ILogger logger)
        {
            try
            {
                logger.LogInformation("Initializing Chatbot database tables...");

                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<HrmsDbContext>();

                    // Create tables if they don't exist
                    EnsureChatbotTablesExist(dbContext, logger);

                    // Seed initial data
                    SeedInitialData(dbContext, logger);
                }

                logger.LogInformation("Chatbot database initialization completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the Chatbot database.");
            }
        }

        private static void EnsureChatbotTablesExist(HrmsDbContext dbContext, ILogger logger)
        {
            try
            {
                // Always try to create the tables with IF NOT EXISTS
                logger.LogInformation("Creating Chatbot tables if they don't exist...");

                // Create Chatbot Conversations table
                dbContext.Database.ExecuteSqlRaw(@"
                    CREATE TABLE IF NOT EXISTS ""ChatbotConversations"" (
                        ""Id"" UUID PRIMARY KEY,
                        ""EmployeeId"" UUID NULL,
                        ""StartedAt"" TIMESTAMP NOT NULL,
                        ""LastMessageAt"" TIMESTAMP NOT NULL,
                        ""IsActive"" BOOLEAN NOT NULL DEFAULT TRUE
                    );
                ");

                // Create Chatbot Messages table
                dbContext.Database.ExecuteSqlRaw(@"
                    CREATE TABLE IF NOT EXISTS ""ChatbotMessages"" (
                        ""Id"" UUID PRIMARY KEY,
                        ""ConversationId"" UUID NOT NULL,
                        ""Content"" TEXT NOT NULL,
                        ""Timestamp"" TIMESTAMP NOT NULL,
                        ""IsFromUser"" BOOLEAN NOT NULL,
                        FOREIGN KEY (""ConversationId"") REFERENCES ""ChatbotConversations"" (""Id"") ON DELETE CASCADE
                    );
                ");

                // Create Chatbot Intents table
                dbContext.Database.ExecuteSqlRaw(@"
                    CREATE TABLE IF NOT EXISTS ""ChatbotIntents"" (
                        ""Id"" UUID PRIMARY KEY,
                        ""Name"" VARCHAR(100) NOT NULL,
                        ""Description"" TEXT NULL,
                        ""ResponseTemplate"" TEXT NOT NULL,
                        ""ApiEndpoint"" VARCHAR(255) NULL,
                        ""RouteDestination"" VARCHAR(255) NULL,
                        ""RequiresAuth"" BOOLEAN NOT NULL DEFAULT FALSE,
                        ""CreatedAt"" TIMESTAMP NOT NULL,
                        ""CreatedBy"" VARCHAR(100) NOT NULL
                    );
                ");

                // Create Chatbot Training Phrases table
                dbContext.Database.ExecuteSqlRaw(@"
                    CREATE TABLE IF NOT EXISTS ""ChatbotTrainingPhrases"" (
                        ""Id"" UUID PRIMARY KEY,
                        ""IntentId"" UUID NOT NULL,
                        ""Phrase"" TEXT NOT NULL,
                        ""CreatedAt"" TIMESTAMP NOT NULL,
                        FOREIGN KEY (""IntentId"") REFERENCES ""ChatbotIntents"" (""Id"") ON DELETE CASCADE
                    );
                ");

                // Create Chatbot Entities table
                dbContext.Database.ExecuteSqlRaw(@"
                    CREATE TABLE IF NOT EXISTS ""ChatbotEntities"" (
                        ""Id"" UUID PRIMARY KEY,
                        ""IntentId"" UUID NOT NULL,
                        ""Name"" VARCHAR(100) NOT NULL,
                        ""Type"" VARCHAR(50) NOT NULL,
                        ""Description"" TEXT NULL,
                        ""CreatedAt"" TIMESTAMP NOT NULL,
                        FOREIGN KEY (""IntentId"") REFERENCES ""ChatbotIntents"" (""Id"") ON DELETE CASCADE
                    );
                ");

                logger.LogInformation("Chatbot tables created successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating Chatbot tables.");
                throw;
            }
        }

        private static void SeedInitialData(HrmsDbContext dbContext, ILogger logger)
        {
            try
            {
                // Check if ChatbotIntents table exists before trying to access it
                var tableExists = dbContext.Database.ExecuteSqlRaw(
                    "SELECT EXISTS (SELECT FROM information_schema.tables WHERE table_name = 'ChatbotIntents')");

                if (tableExists == 0)
                {
                    logger.LogInformation("Chatbot tables don't exist yet. Skipping seed data.");
                    return;
                }

                // Check if we already have intents
                var intentCount = 0;
                try {
                    intentCount = dbContext.Database.ExecuteSqlRaw(
                        "SELECT COUNT(*) FROM \"ChatbotIntents\"");
                } catch (Exception ex) {
                    logger.LogWarning(ex, "Could not check ChatbotIntents count. Skipping seed data.");
                    return;
                }

                if (intentCount == 0)
                {
                    logger.LogInformation("Seeding initial Chatbot data...");

                    // Insert basic intents
                    dbContext.Database.ExecuteSqlRaw(@"
                        INSERT INTO ""ChatbotIntents"" (""Id"", ""Name"", ""Description"", ""ResponseTemplate"", ""ApiEndpoint"", ""RouteDestination"", ""RequiresAuth"", ""CreatedAt"", ""CreatedBy"")
                        VALUES
                        (gen_random_uuid(), 'greeting', 'Greeting intent', 'Hello! I''m your WorkNest assistant. How can I help you today?', NULL, NULL, FALSE, NOW(), 'System'),
                        (gen_random_uuid(), 'goodbye', 'Goodbye intent', 'Goodbye! Have a great day!', NULL, NULL, FALSE, NOW(), 'System'),
                        (gen_random_uuid(), 'help', 'Help intent', 'I can help you with leave requests, attendance, and employee information. What would you like to know?', NULL, NULL, FALSE, NOW(), 'System'),
                        (gen_random_uuid(), 'leave_balance', 'Leave balance intent', 'I''ll check your leave balance for you. Please wait a moment.', '/api/Leave/Balance', '/leave/balance', TRUE, NOW(), 'System'),
                        (gen_random_uuid(), 'attendance_status', 'Attendance status intent', 'Let me check your attendance status.', '/api/Attendance/Status', '/attendance/status', TRUE, NOW(), 'System');
                    ");

                    // Add training phrases for greeting intent
                    dbContext.Database.ExecuteSqlRaw(@"
                        INSERT INTO ""ChatbotTrainingPhrases"" (""Id"", ""IntentId"", ""Phrase"", ""CreatedAt"")
                        SELECT gen_random_uuid(), ""Id"", 'hello', NOW() FROM ""ChatbotIntents"" WHERE ""Name"" = 'greeting'
                        UNION ALL
                        SELECT gen_random_uuid(), ""Id"", 'hi', NOW() FROM ""ChatbotIntents"" WHERE ""Name"" = 'greeting'
                        UNION ALL
                        SELECT gen_random_uuid(), ""Id"", 'hey there', NOW() FROM ""ChatbotIntents"" WHERE ""Name"" = 'greeting'
                        UNION ALL
                        SELECT gen_random_uuid(), ""Id"", 'good morning', NOW() FROM ""ChatbotIntents"" WHERE ""Name"" = 'greeting'
                        UNION ALL
                        SELECT gen_random_uuid(), ""Id"", 'good afternoon', NOW() FROM ""ChatbotIntents"" WHERE ""Name"" = 'greeting';
                    ");

                    // Add training phrases for goodbye intent
                    dbContext.Database.ExecuteSqlRaw(@"
                        INSERT INTO ""ChatbotTrainingPhrases"" (""Id"", ""IntentId"", ""Phrase"", ""CreatedAt"")
                        SELECT gen_random_uuid(), ""Id"", 'goodbye', NOW() FROM ""ChatbotIntents"" WHERE ""Name"" = 'goodbye'
                        UNION ALL
                        SELECT gen_random_uuid(), ""Id"", 'bye', NOW() FROM ""ChatbotIntents"" WHERE ""Name"" = 'goodbye'
                        UNION ALL
                        SELECT gen_random_uuid(), ""Id"", 'see you later', NOW() FROM ""ChatbotIntents"" WHERE ""Name"" = 'goodbye'
                        UNION ALL
                        SELECT gen_random_uuid(), ""Id"", 'have a nice day', NOW() FROM ""ChatbotIntents"" WHERE ""Name"" = 'goodbye';
                    ");

                    // Add training phrases for help intent
                    dbContext.Database.ExecuteSqlRaw(@"
                        INSERT INTO ""ChatbotTrainingPhrases"" (""Id"", ""IntentId"", ""Phrase"", ""CreatedAt"")
                        SELECT gen_random_uuid(), ""Id"", 'help', NOW() FROM ""ChatbotIntents"" WHERE ""Name"" = 'help'
                        UNION ALL
                        SELECT gen_random_uuid(), ""Id"", 'what can you do', NOW() FROM ""ChatbotIntents"" WHERE ""Name"" = 'help'
                        UNION ALL
                        SELECT gen_random_uuid(), ""Id"", 'how can you help me', NOW() FROM ""ChatbotIntents"" WHERE ""Name"" = 'help'
                        UNION ALL
                        SELECT gen_random_uuid(), ""Id"", 'show me what you can do', NOW() FROM ""ChatbotIntents"" WHERE ""Name"" = 'help';
                    ");

                    logger.LogInformation("Initial Chatbot data seeded successfully.");
                }
                else
                {
                    logger.LogInformation("Chatbot data already exists.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error seeding Chatbot data.");
                throw;
            }
        }
    }
}
