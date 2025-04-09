using System;
using System.IO;
using System.Threading.Tasks;
using HrmsApi.Utils;
using Npgsql;

class RunStatusFix
{
    static async Task Main()
    {
        Console.WriteLine("=== HRMS Status Column Fix Utility ===");
        
        try
        {
            string connectionString = "Host=localhost;Database=hrms_v2;Username=postgres;Password=postgres";
            
            // Run the utility to fix the Status column
            await FixStatusColumn.Run(connectionString);
            
            Console.WriteLine("Fix completed! The Status column has been properly updated.");
            Console.WriteLine("Please restart the application for changes to take effect.");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            Console.ResetColor();
        }
        
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
