using System;
using System.IO;
using System.Threading.Tasks;
using Npgsql;

namespace RunMigration
{
    class RunDashboardMigration
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== HRMS Dashboard Features Migration Utility ===");
            
            string connectionString = "Host=localhost;Database=hrms_v2;Username=postgres;Password=postgres";
            string migrationFilePath = Path.Combine(
                Directory.GetParent(Directory.GetCurrentDirectory()).FullName, 
                "HrmsApi", "Migrations", "AddDashboardFeatures.sql");
            
            try
            {
                Console.WriteLine("Reading SQL migration script from: " + migrationFilePath);
                if (!File.Exists(migrationFilePath))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ERROR: Migration SQL file not found!");
                    Console.ResetColor();
                    return;
                }
                
                string sql = File.ReadAllText(migrationFilePath);
                Console.WriteLine("SQL migration script loaded successfully");
                
                Console.WriteLine("Connecting to database...");
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    Console.WriteLine("Connected to database successfully");
                    
                    Console.WriteLine("Executing migration script...");
                    using (var cmd = new NpgsqlCommand(sql, connection))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                    
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Migration completed successfully!");
                    Console.ResetColor();
                    
                    // Create uploads directory for attendance photos
                    string uploadsDir = Path.Combine(
                        Directory.GetParent(Directory.GetCurrentDirectory()).FullName,
                        "HrmsApi", "Uploads", "AttendancePhotos");
                    
                    if (!Directory.Exists(uploadsDir))
                    {
                        Directory.CreateDirectory(uploadsDir);
                        Console.WriteLine($"Created uploads directory at: {uploadsDir}");
                    }
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
