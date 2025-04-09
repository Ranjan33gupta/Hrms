-- Add missing columns to Attendances table
ALTER TABLE "Attendances" ADD COLUMN IF NOT EXISTS "CheckInLocation" text NULL;
ALTER TABLE "Attendances" ADD COLUMN IF NOT EXISTS "CheckOutLocation" text NULL;
ALTER TABLE "Attendances" ADD COLUMN IF NOT EXISTS "CheckInDevice" text NULL;
ALTER TABLE "Attendances" ADD COLUMN IF NOT EXISTS "CheckOutDevice" text NULL;
ALTER TABLE "Attendances" ADD COLUMN IF NOT EXISTS "CheckInIpAddress" text NULL;
ALTER TABLE "Attendances" ADD COLUMN IF NOT EXISTS "CheckOutIpAddress" text NULL;
ALTER TABLE "Attendances" ADD COLUMN IF NOT EXISTS "CheckInLatitude" double precision NULL;
ALTER TABLE "Attendances" ADD COLUMN IF NOT EXISTS "CheckInLongitude" double precision NULL;
ALTER TABLE "Attendances" ADD COLUMN IF NOT EXISTS "CheckOutLatitude" double precision NULL;
ALTER TABLE "Attendances" ADD COLUMN IF NOT EXISTS "CheckOutLongitude" double precision NULL;
ALTER TABLE "Attendances" ADD COLUMN IF NOT EXISTS "Status" text NULL DEFAULT 'Present';

-- Create Shifts table if it doesn't exist
CREATE TABLE IF NOT EXISTS "Shifts" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "StartTime" interval NOT NULL,
    "EndTime" interval NOT NULL,
    "GracePeriod" interval NOT NULL DEFAULT '00:15:00',
    "IsNightShift" boolean NOT NULL DEFAULT false,
    "Description" text NULL,
    "IsActive" boolean NOT NULL DEFAULT true,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text NULL,
    "UpdatedAt" timestamp with time zone NULL,
    "UpdatedBy" text NULL,
    CONSTRAINT "PK_Shifts" PRIMARY KEY ("Id")
);

-- Add ShiftId to Attendances if it doesn't exist
ALTER TABLE "Attendances" ADD COLUMN IF NOT EXISTS "ShiftId" uuid NULL;

-- Add foreign key constraint if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'FK_Attendances_Shifts_ShiftId'
    ) THEN
        ALTER TABLE "Attendances" 
        ADD CONSTRAINT "FK_Attendances_Shifts_ShiftId" 
        FOREIGN KEY ("ShiftId") 
        REFERENCES "Shifts" ("Id") 
        ON DELETE RESTRICT;
    END IF;
END
$$;

-- Create EmployeeShiftAssignments table if it doesn't exist
CREATE TABLE IF NOT EXISTS "EmployeeShiftAssignments" (
    "Id" uuid NOT NULL,
    "EmployeeId" uuid NOT NULL,
    "ShiftId" uuid NOT NULL,
    "EffectiveFrom" timestamp with time zone NOT NULL,
    "EffectiveTo" timestamp with time zone NULL,
    "IsActive" boolean NOT NULL DEFAULT true,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" text NULL,
    "UpdatedAt" timestamp with time zone NULL,
    "UpdatedBy" text NULL,
    CONSTRAINT "PK_EmployeeShiftAssignments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_EmployeeShiftAssignments_Employees_EmployeeId" FOREIGN KEY ("EmployeeId") REFERENCES "Employees" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_EmployeeShiftAssignments_Shifts_ShiftId" FOREIGN KEY ("ShiftId") REFERENCES "Shifts" ("Id") ON DELETE CASCADE
);

-- Create index for EmployeeShiftAssignments
CREATE INDEX IF NOT EXISTS "IX_EmployeeShiftAssignments_EmployeeId" ON "EmployeeShiftAssignments" ("EmployeeId");
CREATE INDEX IF NOT EXISTS "IX_EmployeeShiftAssignments_ShiftId" ON "EmployeeShiftAssignments" ("ShiftId");
