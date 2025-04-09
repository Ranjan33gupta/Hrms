using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using HrmsApi.Modules.Attendance.Domain;
using HrmsApi.Modules.Employee.Domain;
using HrmsApi.Modules.Leave.Domain;
using Npgsql;

namespace HrmsApi.Utils
{
    public class SchemaMigrationGenerator
    {
        private readonly string _connectionString;
        private readonly string _migrationOutputPath;
        
        public SchemaMigrationGenerator(string connectionString, string migrationOutputPath)
        {
            _connectionString = connectionString;
            _migrationOutputPath = migrationOutputPath;
        }
        
        public async Task GenerateMigrationScript()
        {
            // Define all entity types that need to be monitored for changes
            var entityTypes = new List<Type>
            {
                typeof(Attendance),
                typeof(Shift),
                typeof(EmployeeShiftAssignment),
                typeof(Employee),
                typeof(LeaveRequest)
                // Add more entity types as needed
            };
            
            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendLine("-- Auto-generated migration script");
            sqlBuilder.AppendLine("-- Generated on: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sqlBuilder.AppendLine();
            sqlBuilder.AppendLine("DO $$");
            sqlBuilder.AppendLine("BEGIN");
            
            var hasChanges = false;
            
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                foreach (var entityType in entityTypes)
                {
                    var tableName = entityType.Name + "s"; // Simple pluralization - might need more sophistication
                    var tableScriptParts = await GenerateTableScript(connection, entityType, tableName);
                    
                    if (tableScriptParts.Count > 0)
                    {
                        hasChanges = true;
                        sqlBuilder.AppendLine($"    -- Changes for table {tableName}");
                        foreach (var part in tableScriptParts)
                        {
                            sqlBuilder.AppendLine($"    {part}");
                        }
                        sqlBuilder.AppendLine();
                    }
                }
            }
            
            sqlBuilder.AppendLine("END");
            sqlBuilder.AppendLine("$$;");
            
            if (hasChanges)
            {
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var filePath = Path.Combine(_migrationOutputPath, $"Migration_{timestamp}.sql");
                await File.WriteAllTextAsync(filePath, sqlBuilder.ToString());
                Console.WriteLine($"Migration script generated at: {filePath}");
            }
            else
            {
                Console.WriteLine("No schema changes detected.");
            }
        }
        
        private async Task<List<string>> GenerateTableScript(NpgsqlConnection connection, Type entityType, string tableName)
        {
            var scriptParts = new List<string>();
            
            // Check if table exists
            var tableExists = await CheckIfTableExists(connection, tableName);
            if (!tableExists)
            {
                scriptParts.Add($"IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = '{tableName.ToLower()}') THEN");
                scriptParts.Add($"    CREATE TABLE \"{tableName}\" (");
                
                var classProperties = entityType.GetProperties()
                    .Where(p => !p.GetCustomAttributes<System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute>().Any())
                    .ToList();
                
                var columnDefinitions = new List<string>();
                foreach (var property in classProperties)
                {
                    var columnName = property.Name;
                    var dataType = GetPostgresDataType(property);
                    var isNullable = IsNullableProperty(property);
                    var defaultValue = GetDefaultValue(property);
                    
                    var columnDefinition = $"        \"{columnName}\" {dataType}";
                    if (!isNullable)
                        columnDefinition += " NOT NULL";
                    if (!string.IsNullOrEmpty(defaultValue))
                        columnDefinition += defaultValue;
                    
                    columnDefinitions.Add(columnDefinition);
                }
                
                scriptParts.Add(string.Join(",\n", columnDefinitions));
                scriptParts.Add("    );");
                scriptParts.Add("END IF;");
                return scriptParts;
            }
            
            // Table exists, check for columns that need to be added or altered
            var properties = entityType.GetProperties()
                .Where(p => !p.GetCustomAttributes<System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute>().Any())
                .ToList();
            
            foreach (var property in properties)
            {
                var columnExists = await CheckIfColumnExists(connection, tableName, property.Name);
                var columnName = property.Name;
                var dataType = GetPostgresDataType(property);
                var isNullable = IsNullableProperty(property);
                
                if (!columnExists)
                {
                    // Column doesn't exist, add it
                    var defaultValue = GetDefaultValue(property);
                    var columnDefinition = $"ALTER TABLE \"{tableName}\" ADD COLUMN \"{columnName}\" {dataType}";
                    if (!isNullable)
                        columnDefinition += " NOT NULL";
                    if (!string.IsNullOrEmpty(defaultValue))
                        columnDefinition += defaultValue;
                    else if (!isNullable)
                        columnDefinition += " DEFAULT ''"; // Provide a default for NOT NULL columns
                    
                    scriptParts.Add($"IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = '{tableName.ToLower()}' AND column_name = '{columnName.ToLower()}') THEN");
                    scriptParts.Add($"    {columnDefinition};");
                    scriptParts.Add("END IF;");
                }
                else
                {
                    // Column exists, check if type needs to be updated
                    var currentType = await GetColumnType(connection, tableName, property.Name);
                    
                    // Compare PostgreSQL type with C# property type and decide if conversion is needed
                    if (!IsTypeCompatible(currentType, dataType))
                    {
                        string typeConversion = GetTypeConversion(currentType, dataType, columnName);
                        
                        scriptParts.Add($"-- Column {columnName} type mismatch: current={currentType}, needed={dataType}");
                        scriptParts.Add($"ALTER TABLE \"{tableName}\" ALTER COLUMN \"{columnName}\" TYPE {dataType} USING {typeConversion};");
                    }
                }
            }
            
            return scriptParts;
        }
        
        private async Task<bool> CheckIfTableExists(NpgsqlConnection connection, string tableName)
        {
            var sql = $"SELECT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = '{tableName.ToLower()}')";
            using var cmd = new NpgsqlCommand(sql, connection);
            return (bool)await cmd.ExecuteScalarAsync();
        }
        
        private async Task<bool> CheckIfColumnExists(NpgsqlConnection connection, string tableName, string columnName)
        {
            var sql = $"SELECT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = '{tableName.ToLower()}' AND column_name = '{columnName.ToLower()}')";
            using var cmd = new NpgsqlCommand(sql, connection);
            return (bool)await cmd.ExecuteScalarAsync();
        }
        
        private async Task<string> GetColumnType(NpgsqlConnection connection, string tableName, string columnName)
        {
            var sql = $"SELECT data_type FROM information_schema.columns WHERE table_name = '{tableName.ToLower()}' AND column_name = '{columnName.ToLower()}'";
            using var cmd = new NpgsqlCommand(sql, connection);
            return (string)await cmd.ExecuteScalarAsync();
        }
        
        private string GetPostgresDataType(PropertyInfo property)
        {
            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            
            if (type == typeof(string))
                return "text";
            else if (type == typeof(int) || type == typeof(int?))
                return "integer";
            else if (type == typeof(long) || type == typeof(long?))
                return "bigint";
            else if (type == typeof(bool) || type == typeof(bool?))
                return "boolean";
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
                return "timestamp with time zone";
            else if (type == typeof(TimeSpan) || type == typeof(TimeSpan?))
                return "time without time zone";
            else if (type == typeof(decimal) || type == typeof(decimal?))
                return "numeric";
            else if (type == typeof(double) || type == typeof(double?))
                return "double precision";
            else if (type == typeof(Guid) || type == typeof(Guid?))
                return "uuid";
            else if (type.IsEnum)
                return "integer";
            
            return "text"; // Default fallback
        }
        
        private bool IsNullableProperty(PropertyInfo property)
        {
            // Check if the type itself is nullable
            if (Nullable.GetUnderlyingType(property.PropertyType) != null)
                return true;
            
            // Check for nullable reference types (string)
            if (property.PropertyType == typeof(string))
                return true;
            
            // Check if it has [Required] attribute
            if (property.GetCustomAttributes<System.ComponentModel.DataAnnotations.RequiredAttribute>().Any())
                return false;
            
            // Non-value type references (classes) are nullable by default
            return !property.PropertyType.IsValueType;
        }
        
        private string GetDefaultValue(PropertyInfo property)
        {
            // Check for default value attributes or conventions
            // This is a simple implementation that could be extended
            if (property.Name == "CreatedAt")
                return " DEFAULT CURRENT_TIMESTAMP";
            
            if (property.PropertyType == typeof(bool))
                return " DEFAULT false";
                
            if (property.Name == "Status" && property.DeclaringType == typeof(Attendance))
                return " DEFAULT 0"; // Present status
                
            return "";
        }
        
        private bool IsTypeCompatible(string dbType, string modelType)
        {
            // Simple compatibility check - could be extended with more mappings
            if (dbType == modelType)
                return true;
                
            // Common compatible types
            if (dbType == "character varying" && modelType == "text")
                return true;
                
            if (dbType == "timestamp without time zone" && modelType == "timestamp with time zone")
                return true;
                
            // Special cases for time-related types
            if (dbType.Contains("timestamp") && modelType.Contains("time"))
                return false; // Force conversion between timestamp and time
                
            return false;
        }
        
        private string GetTypeConversion(string currentType, string targetType, string columnName)
        {
            // Special handling for enum conversions
            if (currentType == "text" && targetType == "integer")
                return $"CASE WHEN \"{columnName}\" = 'Present' THEN 0 " +
                       $"WHEN \"{columnName}\" = 'Absent' THEN 1 " +
                       $"WHEN \"{columnName}\" = 'Leave' THEN 2 " +
                       $"WHEN \"{columnName}\" = 'HalfDay' THEN 3 " +
                       $"WHEN \"{columnName}\" = 'Holiday' THEN 4 " +
                       $"WHEN \"{columnName}\" = 'Weekend' THEN 5 " +
                       $"WHEN \"{columnName}\" = 'WorkFromHome' THEN 6 " +
                       $"ELSE 0 END";
                
            // Time-related conversions
            if (currentType.Contains("timestamp") && targetType == "time without time zone")
                return $"\"{columnName}\"::time";
                
            // Default conversion
            return $"\"{columnName}\"::{targetType}";
        }
    }
}
