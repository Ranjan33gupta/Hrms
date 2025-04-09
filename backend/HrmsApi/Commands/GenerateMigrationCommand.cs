using System;
using System.IO;
using System.Threading.Tasks;
using HrmsApi.Utils;
using Microsoft.Extensions.Configuration;

namespace HrmsApi.Commands
{
    public class GenerateMigrationCommand
    {
        private readonly IConfiguration _configuration;
        
        public GenerateMigrationCommand(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public async Task Execute()
        {
            try
            {
                Console.WriteLine("Starting schema migration generator...");
                
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                
                // Create migrations directory if it doesn't exist
                var migrationsPath = Path.Combine(Directory.GetCurrentDirectory(), "Migrations");
                if (!Directory.Exists(migrationsPath))
                {
                    Directory.CreateDirectory(migrationsPath);
                }
                
                var generator = new SchemaMigrationGenerator(connectionString, migrationsPath);
                await generator.GenerateMigrationScript();
                
                Console.WriteLine("Schema migration generation completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating migration: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
