-- Add only the missing CreatedBy and UpdatedBy columns to Attendances table
DO $$
BEGIN
    -- Check if columns exist before adding them
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'attendances' AND column_name = 'createdby') THEN
        ALTER TABLE "Attendances" ADD COLUMN "CreatedBy" text NULL;
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'attendances' AND column_name = 'updatedby') THEN
        ALTER TABLE "Attendances" ADD COLUMN "UpdatedBy" text NULL;
    END IF;
END
$$;
