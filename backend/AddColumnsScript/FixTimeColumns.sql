-- Fix the data type mismatch for ClockIn and ClockOut columns
DO $$
BEGIN
    -- Alter ClockIn column to time type
    ALTER TABLE "Attendances" 
    ALTER COLUMN "ClockIn" TYPE time without time zone 
    USING "ClockIn"::time;

    -- Alter ClockOut column to time type
    ALTER TABLE "Attendances" 
    ALTER COLUMN "ClockOut" TYPE time without time zone 
    USING "ClockOut"::time;
END
$$;
