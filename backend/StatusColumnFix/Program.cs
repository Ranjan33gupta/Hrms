using System;
using System.IO;
using System.Threading.Tasks;
using Npgsql;

namespace StatusColumnFix
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== HRMS Status Column Fix Utility ===");
            Console.WriteLine("This utility will fix the Status column in the Attendance table");
            
            string connectionString = "Host=localhost;Database=hrms_v2;Username=postgres;Password=postgres";
            string sqlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "fix_status_column.sql");
            
            try
            {
                if (!File.Exists(sqlFilePath))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: SQL script file not found at {sqlFilePath}");
                    Console.ResetColor();
                    return;
                }
                
                string sqlScript = File.ReadAllText(sqlFilePath);
                Console.WriteLine("SQL script loaded successfully.");
                
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    Console.WriteLine("Connected to database.");
                    
                    using (var cmd = new NpgsqlCommand(sqlScript, connection))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                    
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nStatus column fix completed successfully!");
                    Console.WriteLine("Please restart the HrmsApi application for changes to take effect.");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nError fixing Status column: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();
            }
            
            Console.WriteLine("\nPress Enter to exit...");
            Console.ReadLine();
        }
    }
}
