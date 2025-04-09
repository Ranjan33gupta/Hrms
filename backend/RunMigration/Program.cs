using System;
using System.IO;
using System.Threading.Tasks;
using Npgsql;

class Program
{
    static async Task Main(string[] args)
    {
        string connectionString = "Host=localhost;Database=hrms_v2;Username=postgres;Password=postgres";
        string sqlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "migration.sql");
        
        // Create the SQL file content
        string sqlContent = @"
-- Add missing columns to Attendances table
DO $$
BEGIN
    -- Check if columns exist before adding them
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Attendances' AND column_name = 'CheckInLocation') THEN
        ALTER TABLE ""Attendances"" ADD COLUMN ""CheckInLocation"" text NULL;
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Attendances' AND column_name = 'CheckOutLocation') THEN
        ALTER TABLE ""Attendances"" ADD COLUMN ""CheckOutLocation"" text NULL;
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Attendances' AND column_name = 'CheckInDevice') THEN
        ALTER TABLE ""Attendances"" ADD COLUMN ""CheckInDevice"" text NULL;
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Attendances' AND column_name = 'CheckOutDevice') THEN
        ALTER TABLE ""Attendances"" ADD COLUMN ""CheckOutDevice"" text NULL;
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Attendances' AND column_name = 'CheckInIpAddress') THEN
        ALTER TABLE ""Attendances"" ADD COLUMN ""CheckInIpAddress"" text NULL;
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Attendances' AND column_name = 'CheckOutIpAddress') THEN
        ALTER TABLE ""Attendances"" ADD COLUMN ""CheckOutIpAddress"" text NULL;
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Attendances' AND column_name = 'CheckInLatitude') THEN
        ALTER TABLE ""Attendances"" ADD COLUMN ""CheckInLatitude"" double precision NULL;
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Attendances' AND column_name = 'CheckInLongitude') THEN
        ALTER TABLE ""Attendances"" ADD COLUMN ""CheckInLongitude"" double precision NULL;
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Attendances' AND column_name = 'CheckOutLatitude') THEN
        ALTER TABLE ""Attendances"" ADD COLUMN ""CheckOutLatitude"" double precision NULL;
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Attendances' AND column_name = 'CheckOutLongitude') THEN
        ALTER TABLE ""Attendances"" ADD COLUMN ""CheckOutLongitude"" double precision NULL;
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Attendances' AND column_name = 'Status') THEN
        ALTER TABLE ""Attendances"" ADD COLUMN ""Status"" text NULL DEFAULT 'Present';
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Attendances' AND column_name = 'ShiftId') THEN
        ALTER TABLE ""Attendances"" ADD COLUMN ""ShiftId"" uuid NULL;
    END IF;
END
$$;

-- Create Shifts table if it doesn't exist
CREATE TABLE IF NOT EXISTS ""Shifts"" (
    ""Id"" uuid NOT NULL,
    ""Name"" text NOT NULL,
    ""StartTime"" interval NOT NULL,
    ""EndTime"" interval NOT NULL,
    ""GracePeriod"" interval NOT NULL DEFAULT '00:15:00',
    ""IsNightShift"" boolean NOT NULL DEFAULT false,
    ""Description"" text NULL,
    ""IsActive"" boolean NOT NULL DEFAULT true,
    ""CreatedAt"" timestamp with time zone NOT NULL,
    ""CreatedBy"" text NULL,
    ""UpdatedAt"" timestamp with time zone NULL,
    ""UpdatedBy"" text NULL,
    CONSTRAINT ""PK_Shifts"" PRIMARY KEY (""Id"")
);

-- Add foreign key constraint if it doesn't exist and if both tables exist
DO $$
BEGIN
    -- Check if both tables exist
    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'Attendances') 
       AND EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'Shifts') THEN
        
        -- Check if the constraint doesn't already exist
        IF NOT EXISTS (
            SELECT 1 FROM information_schema.table_constraints 
            WHERE constraint_name = 'FK_Attendances_Shifts_ShiftId'
        ) THEN
            -- Add the foreign key constraint
            ALTER TABLE ""Attendances"" 
            ADD CONSTRAINT ""FK_Attendances_Shifts_ShiftId"" 
            FOREIGN KEY (""ShiftId"") 
            REFERENCES ""Shifts"" (""Id"") 
            ON DELETE RESTRICT;
        END IF;
    END IF;
END
$$;

-- Create EmployeeShiftAssignments table if it doesn't exist
CREATE TABLE IF NOT EXISTS ""EmployeeShiftAssignments"" (
    ""Id"" uuid NOT NULL,
    ""EmployeeId"" uuid NOT NULL,
    ""ShiftId"" uuid NOT NULL,
    ""EffectiveFrom"" timestamp with time zone NOT NULL,
    ""EffectiveTo"" timestamp with time zone NULL,
    ""IsActive"" boolean NOT NULL DEFAULT true,
    ""CreatedAt"" timestamp with time zone NOT NULL,
    ""CreatedBy"" text NULL,
    ""UpdatedAt"" timestamp with time zone NULL,
    ""UpdatedBy"" text NULL,
    CONSTRAINT ""PK_EmployeeShiftAssignments"" PRIMARY KEY (""Id"")
);

-- Add foreign key constraints for EmployeeShiftAssignments if they don't exist
DO $$
BEGIN
    -- Check if all necessary tables exist
    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'EmployeeShiftAssignments') 
       AND EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'Employees')
       AND EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'Shifts') THEN
        
        -- Check if the employee constraint doesn't already exist
        IF NOT EXISTS (
            SELECT 1 FROM information_schema.table_constraints 
            WHERE constraint_name = 'FK_EmployeeShiftAssignments_Employees_EmployeeId'
        ) THEN
            -- Add the foreign key constraint for EmployeeId
            ALTER TABLE ""EmployeeShiftAssignments"" 
            ADD CONSTRAINT ""FK_EmployeeShiftAssignments_Employees_EmployeeId"" 
            FOREIGN KEY (""EmployeeId"") 
            REFERENCES ""Employees"" (""Id"") 
            ON DELETE CASCADE;
        END IF;
        
        -- Check if the shift constraint doesn't already exist
        IF NOT EXISTS (
            SELECT 1 FROM information_schema.table_constraints 
            WHERE constraint_name = 'FK_EmployeeShiftAssignments_Shifts_ShiftId'
        ) THEN
            -- Add the foreign key constraint for ShiftId
            ALTER TABLE ""EmployeeShiftAssignments"" 
            ADD CONSTRAINT ""FK_EmployeeShiftAssignments_Shifts_ShiftId"" 
            FOREIGN KEY (""ShiftId"") 
            REFERENCES ""Shifts"" (""Id"") 
            ON DELETE CASCADE;
        END IF;
    END IF;
END
$$;

-- Create indexes for EmployeeShiftAssignments if they don't exist
DO $$
BEGIN
    -- Check if the table exists
    IF EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'EmployeeShiftAssignments') THEN
        
        -- Create index for EmployeeId if it doesn't exist
        IF NOT EXISTS (
            SELECT 1 FROM pg_indexes 
            WHERE tablename = 'EmployeeShiftAssignments' AND indexname = 'IX_EmployeeShiftAssignments_EmployeeId'
        ) THEN
            CREATE INDEX ""IX_EmployeeShiftAssignments_EmployeeId"" ON ""EmployeeShiftAssignments"" (""EmployeeId"");
        END IF;
        
        -- Create index for ShiftId if it doesn't exist
        IF NOT EXISTS (
            SELECT 1 FROM pg_indexes 
            WHERE tablename = 'EmployeeShiftAssignments' AND indexname = 'IX_EmployeeShiftAssignments_ShiftId'
        ) THEN
            CREATE INDEX ""IX_EmployeeShiftAssignments_ShiftId"" ON ""EmployeeShiftAssignments"" (""ShiftId"");
        END IF;
    END IF;
END
$$;
";

        // Write the SQL file
        File.WriteAllText(sqlFilePath, sqlContent);
        Console.WriteLine($"Created SQL file at: {sqlFilePath}");
        
        try
        {
            Console.WriteLine("Connecting to database...");
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                Console.WriteLine("Connected to database. Executing SQL script...");
                
                using (var command = new NpgsqlCommand(File.ReadAllText(sqlFilePath), connection))
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
