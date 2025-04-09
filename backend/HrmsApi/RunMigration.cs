using System;
using System.IO;
using System.Threading.Tasks;
using Npgsql;

class RunMigration
{
    static async Task Main(string[] args)
    {
        string connectionString = "Host=localhost;Database=hrms_v2;Username=postgres;Password=postgres";
        string sqlFilePath = "Migrations/AddAttendanceColumnsManually.sql";
        
        try
        {
            Console.WriteLine("Reading SQL script...");
            string sql = File.ReadAllText(sqlFilePath);
            
            Console.WriteLine("Connecting to database...");
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                Console.WriteLine("Connected to database. Executing SQL script...");
                
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
                
                Console.WriteLine("SQL script executed successfully!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
        
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
