using System;
using System.IO;
using System.Threading.Tasks;
using Npgsql;

class Program
{
    static async Task Main(string[] args)
    {
        string connectionString = "Host=localhost;Database=hrms_v2;Username=postgres;Password=postgres";
        
        try
        {
            Console.WriteLine("Connecting to database...");
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                Console.WriteLine("Connected to database.");
                
                // Read and execute the AddAuditableColumns.sql script
                string auditableColumnsSql = File.ReadAllText("AddAuditableColumns.sql");
                Console.WriteLine("Executing AddAuditableColumns.sql script...");
                
                using (var command = new NpgsqlCommand(auditableColumnsSql, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
                
                Console.WriteLine("AddAuditableColumns.sql script executed successfully!");
            }
            
            Console.WriteLine("All database updates completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}
