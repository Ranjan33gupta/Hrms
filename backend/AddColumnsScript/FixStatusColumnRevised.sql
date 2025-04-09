-- Fix the Status column in the Attendances table to be an integer
DO $$
BEGIN
    -- First drop the default constraint
    ALTER TABLE "Attendances" 
    ALTER COLUMN "Status" DROP DEFAULT;

    -- Then alter the column type
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

    -- Finally, set the default value to 0 (Present)
    ALTER TABLE "Attendances" 
    ALTER COLUMN "Status" SET DEFAULT 0;
END
$$;
