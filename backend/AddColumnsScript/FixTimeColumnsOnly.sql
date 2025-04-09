-- Fix only the ClockIn and ClockOut columns to be time without time zone
DO $$
BEGIN
    -- Fix ClockIn and ClockOut columns to be time without time zone
    ALTER TABLE "Attendances" 
    ALTER COLUMN "ClockIn" TYPE time without time zone 
    USING "ClockIn"::time;

    ALTER TABLE "Attendances" 
    ALTER COLUMN "ClockOut" TYPE time without time zone 
    USING "ClockOut"::time;
END
$$;
