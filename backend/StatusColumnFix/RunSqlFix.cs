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
            Console.WriteLine("=== HRMS Database Column Fix Utility ===");
            
            string connectionString = "Host=localhost;Database=hrms_v2;Username=postgres;Password=postgres";
            string sqlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "fix_status_column_direct.sql");
            
            try
            {
                Console.WriteLine("Reading SQL script from: " + sqlFilePath);
                if (!File.Exists(sqlFilePath))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ERROR: SQL file not found!");
                    Console.ResetColor();
                    return;
                }
                
                string sql = File.ReadAllText(sqlFilePath);
                Console.WriteLine("SQL script loaded successfully");
                
                Console.WriteLine("Connecting to database...");
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    Console.WriteLine("Connected to database successfully");
                    
                    Console.WriteLine("Executing SQL fix script...");
                    using (var cmd = new NpgsqlCommand(sql, connection))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                    
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Database fix completed successfully!");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();
            }
            
            Console.WriteLine("\nPress Enter to exit...");
            Console.ReadLine();
        }
    }
}
