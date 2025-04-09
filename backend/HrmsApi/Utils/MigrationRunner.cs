using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HrmsApi.Commands;

namespace HrmsApi.Utils
{
    public static class MigrationRunner
    {
        public static async Task RunMigrationGenerator(IServiceProvider serviceProvider)
        {
            Console.WriteLine("=== HRMS Schema Migration Generator ===");
            
            try
            {
                var command = serviceProvider.GetRequiredService<GenerateMigrationCommand>();
                await command.Execute();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Migration failed: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();
            }
        }
    }
}
