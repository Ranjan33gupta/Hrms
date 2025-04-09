using System;
using System.Threading.Tasks;
using Npgsql;

namespace HrmsApi.Utils
{
    public class FixStatusColumn
    {
        public static async Task Run(string connectionString)
        {
            try
            {
                Console.WriteLine("Starting Status column fix utility...");
                
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    Console.WriteLine("Connected to database.");
                    
                    // Check if the Status column exists and is of text type
                    bool isTextColumn = await IsStatusColumnText(connection);
                    if (!isTextColumn)
                    {
                        Console.WriteLine("Status column is already properly configured (integer type). No fix needed.");
                        return;
                    }
                    
                    // Execute the fix
                    await ExecuteFix(connection);
                    
                    Console.WriteLine("Status column fix completed successfully!");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error fixing Status column: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();
            }
        }
        
        private static async Task<bool> IsStatusColumnText(NpgsqlConnection connection)
        {
            var sql = "SELECT data_type FROM information_schema.columns " +
                     "WHERE table_name = 'attendances' AND column_name = 'status'";
                     
            using var cmd = new NpgsqlCommand(sql, connection);
            var result = await cmd.ExecuteScalarAsync();
            
            if (result == null)
            {
                Console.WriteLine("Status column not found in Attendances table.");
                return false;
            }
            
            string dataType = result.ToString();
            Console.WriteLine($"Current Status column data type: {dataType}");
            
            return dataType.ToLower() == "text" || dataType.ToLower() == "character varying";
        }
        
        private static async Task ExecuteFix(NpgsqlConnection connection)
        {
            // First drop the default constraint
            Console.WriteLine("Dropping default constraint from Status column...");
            var dropDefaultSql = "ALTER TABLE \"Attendances\" ALTER COLUMN \"Status\" DROP DEFAULT";
            
            using (var cmd = new NpgsqlCommand(dropDefaultSql, connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }
            
            // Now alter the column type with proper CASE statement for conversion
            Console.WriteLine("Converting Status column to integer type...");
            var alterSql = @"
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
                END::integer
            ";
            
            using (var cmd = new NpgsqlCommand(alterSql, connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }
            
            // Set default value to 0 (Present)
            Console.WriteLine("Setting default value for Status column...");
            var setDefaultSql = "ALTER TABLE \"Attendances\" ALTER COLUMN \"Status\" SET DEFAULT 0";
            
            using (var cmd = new NpgsqlCommand(setDefaultSql, connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
