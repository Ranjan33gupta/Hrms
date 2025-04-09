using System;
using System.IO;
using System.Threading.Tasks;
using Npgsql;

namespace SqlFix
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== HRMS Database Column Fix Utility ===");
            
            string connectionString = "Host=localhost;Database=hrms_v2;Username=postgres;Password=postgres";
            
            string sql = @"
-- First drop the default constraint
ALTER TABLE ""Attendances"" ALTER COLUMN ""Status"" DROP DEFAULT;

-- Now alter the column type
ALTER TABLE ""Attendances"" 
ALTER COLUMN ""Status"" TYPE integer
USING CASE 
    WHEN ""Status"" = 'Present' THEN 0
    WHEN ""Status"" = 'Absent' THEN 1
    WHEN ""Status"" = 'Leave' THEN 2
    WHEN ""Status"" = 'HalfDay' THEN 3
    WHEN ""Status"" = 'Holiday' THEN 4
    WHEN ""Status"" = 'Weekend' THEN 5
    WHEN ""Status"" = 'WorkFromHome' THEN 6
    ELSE 0 -- Default to Present
END::integer;

-- Set new default value
ALTER TABLE ""Attendances"" ALTER COLUMN ""Status"" SET DEFAULT 0;

-- Ensure ClockIn and ClockOut columns are correct type
ALTER TABLE ""Attendances"" 
ALTER COLUMN ""ClockIn"" TYPE time without time zone 
USING ""ClockIn""::time;

ALTER TABLE ""Attendances"" 
ALTER COLUMN ""ClockOut"" TYPE time without time zone 
USING ""ClockOut""::time;
";
            
            try
            {
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
