-- Fix specifically for the Status column in Attendance table
DO $$
DECLARE
    status_type TEXT;
    status_exists BOOLEAN;
BEGIN
    -- Check if Status column exists
    SELECT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'attendances' AND column_name = 'status'
    ) INTO status_exists;
    
    IF NOT status_exists THEN
        RAISE NOTICE 'Status column does not exist in Attendances table';
        RETURN;
    END IF;
    
    -- Check the current data type of the Status column
    SELECT data_type INTO status_type 
    FROM information_schema.columns 
    WHERE table_name = 'attendances' AND column_name = 'status';
    
    RAISE NOTICE 'Current Status column type: %', status_type;
    
    -- Fix Status column if it's text type
    IF status_type = 'text' OR status_type = 'character varying' THEN
        -- First drop the default constraint if it exists
        BEGIN
            ALTER TABLE "Attendances" ALTER COLUMN "Status" DROP DEFAULT;
            RAISE NOTICE 'Default constraint dropped from Status column';
        EXCEPTION WHEN OTHERS THEN
            RAISE NOTICE 'Could not drop default constraint: %', SQLERRM;
        END;
        
        -- Convert Status column to integer with proper mapping
        BEGIN
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
            
            RAISE NOTICE 'Status column converted to integer type';
            
            -- Set default value to 0 (Present)
            ALTER TABLE "Attendances" ALTER COLUMN "Status" SET DEFAULT 0;
            RAISE NOTICE 'Default value (0 = Present) set for Status column';
        EXCEPTION WHEN OTHERS THEN
            RAISE NOTICE 'Error converting Status column: %', SQLERRM;
        END;
    ELSE
        RAISE NOTICE 'Status column is already of type: %. No conversion needed.', status_type;
    END IF;
    
    RAISE NOTICE 'Status column fix completed!';
END
$$;
