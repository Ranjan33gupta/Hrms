-- Comprehensive fix for Attendance table schema issues
DO $$
DECLARE
    status_type TEXT;
BEGIN
    -- Check the current data type of the Status column
    SELECT data_type INTO status_type 
    FROM information_schema.columns 
    WHERE table_name = 'attendances' AND column_name = 'status';
    
    -- Fix Status column if it's text type
    IF status_type = 'text' OR status_type = 'character varying' THEN
        -- First drop the default constraint
        BEGIN
            ALTER TABLE "Attendances" ALTER COLUMN "Status" DROP DEFAULT;
            RAISE NOTICE 'Default constraint dropped from Status column';
        EXCEPTION WHEN OTHERS THEN
            RAISE NOTICE 'Could not drop default constraint: %', SQLERRM;
        END;
        
        -- Convert Status column to integer with proper mapping
        ALTER TABLE "Attendances" 
        ALTER COLUMN "Status" TYPE integer 
        USING CASE 
            WHEN "Status" = 'Present' THEN 0
            WHEN "Status" = 'Absent' THEN 1
            WHEN "Status" = 'Leave' THEN 2
            WHEN "Status" = 'HalfDay' THEN 3
            WHEN "Status" = 'Holiday' THEN 4
            WHEN "Status" = 'Weekend' THEN 5
            WHEN "Status" = 'WorkFromHome' THEN 6
            ELSE 0 -- Default to Present
        END::integer;
        
        -- Set default value to 0 (Present)
        ALTER TABLE "Attendances" ALTER COLUMN "Status" SET DEFAULT 0;
        
        RAISE NOTICE 'Status column converted to integer type with default value 0';
    ELSE
        RAISE NOTICE 'Status column is already of type: %', status_type;
    END IF;
    
    -- Fix ClockIn and ClockOut columns to be time without time zone
    BEGIN
        ALTER TABLE "Attendances" 
        ALTER COLUMN "ClockIn" TYPE time without time zone 
        USING "ClockIn"::time;
        
        RAISE NOTICE 'ClockIn column converted to time without time zone';
    EXCEPTION WHEN OTHERS THEN
        RAISE NOTICE 'ClockIn column error: %', SQLERRM;
    END;
    
    BEGIN
        ALTER TABLE "Attendances" 
        ALTER COLUMN "ClockOut" TYPE time without time zone 
        USING "ClockOut"::time;
        
        RAISE NOTICE 'ClockOut column converted to time without time zone';
    EXCEPTION WHEN OTHERS THEN
        RAISE NOTICE 'ClockOut column error: %', SQLERRM;
    END;
    
    -- Ensure all required columns exist
    -- CreatedBy and UpdatedBy columns
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'attendances' AND column_name = 'createdby') THEN
        ALTER TABLE "Attendances" ADD COLUMN "CreatedBy" text NULL;
        RAISE NOTICE 'Added CreatedBy column';
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'attendances' AND column_name = 'updatedby') THEN
        ALTER TABLE "Attendances" ADD COLUMN "UpdatedBy" text NULL;
        RAISE NOTICE 'Added UpdatedBy column';
    END IF;
    
    -- Ensure location tracking columns exist
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'attendances' AND column_name = 'checkinlocation') THEN
        ALTER TABLE "Attendances" ADD COLUMN "CheckInLocation" text NULL;
        RAISE NOTICE 'Added CheckInLocation column';
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'attendances' AND column_name = 'checkoutlocation') THEN
        ALTER TABLE "Attendances" ADD COLUMN "CheckOutLocation" text NULL;
        RAISE NOTICE 'Added CheckOutLocation column';
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'attendances' AND column_name = 'checkindevice') THEN
        ALTER TABLE "Attendances" ADD COLUMN "CheckInDevice" text NULL;
        RAISE NOTICE 'Added CheckInDevice column';
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'attendances' AND column_name = 'checkoutdevice') THEN
        ALTER TABLE "Attendances" ADD COLUMN "CheckOutDevice" text NULL;
        RAISE NOTICE 'Added CheckOutDevice column';
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'attendances' AND column_name = 'checkinipaddress') THEN
        ALTER TABLE "Attendances" ADD COLUMN "CheckInIpAddress" text NULL;
        RAISE NOTICE 'Added CheckInIpAddress column';
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'attendances' AND column_name = 'checkoutipaddress') THEN
        ALTER TABLE "Attendances" ADD COLUMN "CheckOutIpAddress" text NULL;
        RAISE NOTICE 'Added CheckOutIpAddress column';
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'attendances' AND column_name = 'checkinlatitude') THEN
        ALTER TABLE "Attendances" ADD COLUMN "CheckInLatitude" double precision NULL;
        RAISE NOTICE 'Added CheckInLatitude column';
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'attendances' AND column_name = 'checkinlongitude') THEN
        ALTER TABLE "Attendances" ADD COLUMN "CheckInLongitude" double precision NULL;
        RAISE NOTICE 'Added CheckInLongitude column';
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'attendances' AND column_name = 'checkoutlatitude') THEN
        ALTER TABLE "Attendances" ADD COLUMN "CheckOutLatitude" double precision NULL;
        RAISE NOTICE 'Added CheckOutLatitude column';
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'attendances' AND column_name = 'checkoutlongitude') THEN
        ALTER TABLE "Attendances" ADD COLUMN "CheckOutLongitude" double precision NULL;
        RAISE NOTICE 'Added CheckOutLongitude column';
    END IF;
    
    -- Ensure ShiftId column exists
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'attendances' AND column_name = 'shiftid') THEN
        ALTER TABLE "Attendances" ADD COLUMN "ShiftId" uuid NULL;
        RAISE NOTICE 'Added ShiftId column';
    END IF;
    
    -- Create Shifts table if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'shifts') THEN
        CREATE TABLE "Shifts" (
            "Id" uuid NOT NULL PRIMARY KEY,
            "Name" text NOT NULL,
            "StartTime" time without time zone NOT NULL,
            "EndTime" time without time zone NOT NULL,
            "GracePeriod" interval NOT NULL DEFAULT '00:15:00',
            "IsNightShift" boolean NOT NULL DEFAULT false,
            "Description" text NULL,
            "IsActive" boolean NOT NULL DEFAULT true,
            "CreatedAt" timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
            "CreatedBy" text NULL,
            "UpdatedAt" timestamp with time zone NULL,
            "UpdatedBy" text NULL
        );
        
        RAISE NOTICE 'Created Shifts table';
    END IF;
    
    -- Create EmployeeShiftAssignments table if it doesn't exist
    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'employeeshiftassignments') THEN
        CREATE TABLE "EmployeeShiftAssignments" (
            "Id" uuid NOT NULL PRIMARY KEY,
            "EmployeeId" uuid NOT NULL,
            "ShiftId" uuid NOT NULL,
            "EffectiveFrom" timestamp with time zone NOT NULL,
            "EffectiveTo" timestamp with time zone NULL,
            "IsActive" boolean NOT NULL DEFAULT true,
            "CreatedAt" timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
            "CreatedBy" text NULL,
            "UpdatedAt" timestamp with time zone NULL,
            "UpdatedBy" text NULL,
            CONSTRAINT "FK_EmployeeShiftAssignments_Employees_EmployeeId" FOREIGN KEY ("EmployeeId") REFERENCES "Employees" ("Id") ON DELETE CASCADE,
            CONSTRAINT "FK_EmployeeShiftAssignments_Shifts_ShiftId" FOREIGN KEY ("ShiftId") REFERENCES "Shifts" ("Id") ON DELETE CASCADE
        );
        
        RAISE NOTICE 'Created EmployeeShiftAssignments table';
    END IF;
    
    RAISE NOTICE 'Attendance schema fix completed successfully!';
END
$$;
