using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HrmsApi.Modules.Chatbot.Infrastructure.Data
{
    public class ChatbotDbInitializer
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ChatbotDbInitializer> _logger;

        public ChatbotDbInitializer(
            IServiceProvider serviceProvider,
            ILogger<ChatbotDbInitializer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                _logger.LogInformation("Starting Chatbot database initialization");
                
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ChatbotDbContext>();
                
                // Ensure database is created
                await dbContext.Database.EnsureCreatedAsync();
                
                // Run the SQL migration script
                string migrationScript = File.ReadAllText("Migrations/ChatbotTrainingData.sql");
                await dbContext.Database.ExecuteSqlRawAsync(migrationScript);
                
                _logger.LogInformation("Chatbot database initialization completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the Chatbot database");
                throw;
            }
        }
    }
}
