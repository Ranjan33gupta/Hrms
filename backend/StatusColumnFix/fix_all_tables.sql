-- Comprehensive fix for all tables with Status columns
DO $$
BEGIN
    -- Fix Status column in Attendances table
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'attendances' AND column_name = 'status' AND data_type = 'text'
    ) THEN
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
        
        ALTER TABLE "Attendances" ALTER COLUMN "Status" SET DEFAULT 0;
        RAISE NOTICE 'Fixed Status column in Attendances table';
    END IF;
    
    -- Fix ClockIn and ClockOut columns
    BEGIN
        ALTER TABLE "Attendances" 
        ALTER COLUMN "ClockIn" TYPE time without time zone 
        USING "ClockIn"::time;
        
        ALTER TABLE "Attendances" 
        ALTER COLUMN "ClockOut" TYPE time without time zone 
        USING "ClockOut"::time;
        
        RAISE NOTICE 'Fixed ClockIn and ClockOut columns in Attendances table';
    EXCEPTION WHEN OTHERS THEN
        RAISE NOTICE 'Error fixing time columns: %', SQLERRM;
    END;
END
$$;
