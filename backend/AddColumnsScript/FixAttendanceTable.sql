-- Fix the Attendance table structure to match the C# model
DO $$
BEGIN
    -- 1. Fix ClockIn and ClockOut columns to be time without time zone
    ALTER TABLE "Attendances" 
    ALTER COLUMN "ClockIn" TYPE time without time zone 
    USING "ClockIn"::time;

    ALTER TABLE "Attendances" 
    ALTER COLUMN "ClockOut" TYPE time without time zone 
    USING "ClockOut"::time;

    -- 2. Ensure Status column exists with default value
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'attendances' AND column_name = 'status') THEN
        ALTER TABLE "Attendances" ADD COLUMN "Status" text NOT NULL DEFAULT 'Present';
    END IF;

    -- 3. Ensure all location tracking columns exist
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'attendances' AND column_name = 'checkinlocation') THEN
        ALTER TABLE "Attendances" ADD COLUMN "CheckInLocation" text NULL;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'attendances' AND column_name = 'checkoutlocation') THEN
        ALTER TABLE "Attendances" ADD COLUMN "CheckOutLocation" text NULL;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'attendances' AND column_name = 'checkindevice') THEN
        ALTER TABLE "Attendances" ADD COLUMN "CheckInDevice" text NULL;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'attendances' AND column_name = 'checkoutdevice') THEN
        ALTER TABLE "Attendances" ADD COLUMN "CheckOutDevice" text NULL;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'attendances' AND column_name = 'checkinipaddress') THEN
        ALTER TABLE "Attendances" ADD COLUMN "CheckInIpAddress" text NULL;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'attendances' AND column_name = 'checkoutipaddress') THEN
        ALTER TABLE "Attendances" ADD COLUMN "CheckOutIpAddress" text NULL;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'attendances' AND column_name = 'checkinlatitude') THEN
        ALTER TABLE "Attendances" ADD COLUMN "CheckInLatitude" double precision NULL;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'attendances' AND column_name = 'checkinlongitude') THEN
        ALTER TABLE "Attendances" ADD COLUMN "CheckInLongitude" double precision NULL;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'attendances' AND column_name = 'checkoutlatitude') THEN
        ALTER TABLE "Attendances" ADD COLUMN "CheckOutLatitude" double precision NULL;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'attendances' AND column_name = 'checkoutlongitude') THEN
        ALTER TABLE "Attendances" ADD COLUMN "CheckOutLongitude" double precision NULL;
    END IF;

    -- 4. Ensure ShiftId column exists
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_name = 'attendances' AND column_name = 'shiftid') THEN
        ALTER TABLE "Attendances" ADD COLUMN "ShiftId" uuid NULL;
    END IF;
END
$$;
