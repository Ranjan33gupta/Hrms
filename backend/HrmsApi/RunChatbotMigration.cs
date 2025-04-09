using System;
using System.IO;
using Npgsql;

namespace HrmsApi
{
    public class RunChatbotMigration
    {
        public static void Run()
        {
            Console.WriteLine("=== Running Chatbot Migration ===");
            string connectionString = "Host=localhost;Database=hrms_v2;Username=postgres;Password=postgres";
            string sqlScript = File.ReadAllText("Migrations/ChatbotTables.sql");

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Connected to database");

                    using (var command = new NpgsqlCommand(sqlScript, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    Console.WriteLine("Chatbot tables migration completed successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error running migration: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
