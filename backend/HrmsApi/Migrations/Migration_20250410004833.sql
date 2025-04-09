-- Auto-generated migration script
-- Generated on: 2025-04-10 00:48:32

DO $$
BEGIN
    -- Changes for table Attendances
    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'attendances') THEN
        CREATE TABLE "Attendances" (
            "Id" uuid NOT NULL,
        "EmployeeId" uuid NOT NULL,
        "Date" timestamp with time zone NOT NULL,
        "ClockIn" time without time zone NOT NULL,
        "ClockOut" time without time zone,
        "CheckInLocation" text,
        "CheckOutLocation" text,
        "CheckInDevice" text,
        "CheckOutDevice" text,
        "CheckInIpAddress" text,
        "CheckOutIpAddress" text,
        "CheckInLatitude" double precision,
        "CheckInLongitude" double precision,
        "CheckOutLatitude" double precision,
        "CheckOutLongitude" double precision,
        "Notes" text,
        "ShiftId" uuid,
        "Shift" text,
        "Status" integer NOT NULL DEFAULT 0,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
        "CreatedBy" text,
        "UpdatedAt" timestamp with time zone,
        "UpdatedBy" text
        );
    END IF;

    -- Changes for table Shifts
    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'shifts') THEN
        CREATE TABLE "Shifts" (
            "Id" uuid NOT NULL,
        "Name" text,
        "StartTime" time without time zone NOT NULL,
        "EndTime" time without time zone NOT NULL,
        "GracePeriod" time without time zone NOT NULL,
        "IsNightShift" boolean NOT NULL DEFAULT false,
        "Description" text,
        "IsActive" boolean NOT NULL DEFAULT false,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
        "CreatedBy" text,
        "UpdatedAt" timestamp with time zone,
        "UpdatedBy" text
        );
    END IF;

    -- Changes for table EmployeeShiftAssignments
    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'employeeshiftassignments') THEN
        CREATE TABLE "EmployeeShiftAssignments" (
            "Id" uuid NOT NULL,
        "EmployeeId" uuid NOT NULL,
        "ShiftId" uuid NOT NULL,
        "EffectiveFrom" timestamp with time zone NOT NULL,
        "EffectiveTo" timestamp with time zone,
        "IsActive" boolean NOT NULL DEFAULT false,
        "Shift" text,
        "CreatedAt" timestamp with time zone NOT NULL DEFAULT CURRENT_TIMESTAMP,
        "CreatedBy" text,
        "UpdatedAt" timestamp with time zone,
        "UpdatedBy" text
        );
    END IF;

    -- Changes for table Employees
    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'employees') THEN
        CREATE TABLE "Employees" (
            "Id" uuid NOT NULL,
        "EmployeeCode" text,
        "FullName" text,
        "Email" text,
        "CountryCode" text,
        "ContactNumber" text,
        "Gender" text,
        "DateOfBirth" timestamp with time zone,
        "MaritalStatus" text,
        "NationalIdNumber" text,
        "DepartmentId" uuid NOT NULL,
        "DesignationId" uuid NOT NULL,
        "ManagerId" uuid,
        "Department" text,
        "Designation" text,
        "Manager" text,
        "Subordinates" text,
        "LeaveRequests" text,
        "BankDetail" text,
        "Payrolls" text,
        "JoiningDate" timestamp with time zone NOT NULL,
        "ExitDate" timestamp with time zone,
        "EmploymentType" text,
        "IsActive" boolean NOT NULL DEFAULT false
        );
    END IF;

    -- Changes for table LeaveRequests
    IF NOT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_name = 'leaverequests') THEN
        CREATE TABLE "LeaveRequests" (
            "Id" uuid NOT NULL,
        "EmployeeId" uuid NOT NULL,
        "Employee" text,
        "StartDate" timestamp with time zone NOT NULL,
        "EndDate" timestamp with time zone NOT NULL,
        "LeaveType" text,
        "Reason" text,
        "Status" text,
        "RequestDate" timestamp with time zone NOT NULL,
        "ApprovedBy" text,
        "ApprovalDate" timestamp with time zone,
        "Comments" text,
        "DurationInDays" integer NOT NULL
        );
    END IF;

END
$$;
