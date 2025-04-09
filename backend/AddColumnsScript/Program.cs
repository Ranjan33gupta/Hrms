using System;
using System.IO;
using System.Threading.Tasks;
using Npgsql;

class Program
{
    static async Task Main(string[] args)
    {
        string connectionString = "Host=localhost;Database=hrms_v2;Username=postgres;Password=postgres";
        string sqlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "FixStatusColumnRevised.sql");
        
        try
        {
            Console.WriteLine("Connecting to database...");
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                Console.WriteLine("Connected to database. Executing SQL script...");
                
                string sqlContent = File.ReadAllText(sqlFilePath);
                using (var command = new NpgsqlCommand(sqlContent, connection))
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
        
        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
    }
}
