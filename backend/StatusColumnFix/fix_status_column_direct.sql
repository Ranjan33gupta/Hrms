-- Direct fix for Status column in Attendances table
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

-- Set default value
ALTER TABLE "Attendances" ALTER COLUMN "Status" SET DEFAULT 0;

-- Ensure ClockIn and ClockOut columns are correct type
ALTER TABLE "Attendances" 
ALTER COLUMN "ClockIn" TYPE time without time zone 
USING "ClockIn"::time;

ALTER TABLE "Attendances" 
ALTER COLUMN "ClockOut" TYPE time without time zone 
USING "ClockOut"::time;
