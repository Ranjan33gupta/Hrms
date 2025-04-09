-- Add auditable columns to Attendances table if they don't exist
DO $$
BEGIN
    -- Check if columns exist before adding them
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Attendances' AND column_name = 'CreatedBy') THEN
        ALTER TABLE "Attendances" ADD COLUMN "CreatedBy" text NULL;
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Attendances' AND column_name = 'UpdatedBy') THEN
        ALTER TABLE "Attendances" ADD COLUMN "UpdatedBy" text NULL;
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Attendances' AND column_name = 'CreatedAt') THEN
        ALTER TABLE "Attendances" ADD COLUMN "CreatedAt" timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP;
    END IF;
    
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'Attendances' AND column_name = 'UpdatedAt') THEN
        ALTER TABLE "Attendances" ADD COLUMN "UpdatedAt" timestamp with time zone NULL;
    END IF;
END
$$;
