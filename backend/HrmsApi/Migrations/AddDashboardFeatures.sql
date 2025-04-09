-- Migration script to add tables for advanced dashboard features
-- Created: 2025-04-09

-- Create AttendancePhotos table
CREATE TABLE IF NOT EXISTS "AttendancePhotos" (
    "Id" uuid NOT NULL,
    "AttendanceId" uuid NOT NULL,
    "IsClockIn" boolean NOT NULL,
    "PhotoUrl" text NOT NULL,
    "StoragePath" character varying(255),
    "CaptureTime" timestamp with time zone NOT NULL,
    "DeviceInfo" character varying(255),
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" character varying(100),
    CONSTRAINT "PK_AttendancePhotos" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_AttendancePhotos_Attendances_AttendanceId" FOREIGN KEY ("AttendanceId") REFERENCES "Attendances" ("Id") ON DELETE CASCADE
);

-- Create MoodEntries table
CREATE TABLE IF NOT EXISTS "MoodEntries" (
    "Id" uuid NOT NULL,
    "EmployeeId" uuid NOT NULL,
    "EntryDate" timestamp with time zone NOT NULL,
    "Mood" integer NOT NULL,
    "Comment" text,
    "SentimentScore" double precision,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" character varying(100),
    "UpdatedAt" timestamp with time zone,
    "UpdatedBy" character varying(100),
    CONSTRAINT "PK_MoodEntries" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_MoodEntries_Employees_EmployeeId" FOREIGN KEY ("EmployeeId") REFERENCES "Employees" ("Id") ON DELETE CASCADE
);

-- Create ChatbotIntents table
CREATE TABLE IF NOT EXISTS "ChatbotIntents" (
    "Id" uuid NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Description" character varying(255) NOT NULL,
    "ResponseTemplate" text NOT NULL,
    "ApiEndpoint" character varying(255),
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" character varying(100),
    "UpdatedAt" timestamp with time zone,
    "UpdatedBy" character varying(100),
    CONSTRAINT "PK_ChatbotIntents" PRIMARY KEY ("Id")
);

-- Create ChatbotTrainingPhrases table
CREATE TABLE IF NOT EXISTS "ChatbotTrainingPhrases" (
    "Id" uuid NOT NULL,
    "IntentId" uuid NOT NULL,
    "Phrase" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_ChatbotTrainingPhrases" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ChatbotTrainingPhrases_ChatbotIntents_IntentId" FOREIGN KEY ("IntentId") REFERENCES "ChatbotIntents" ("Id") ON DELETE CASCADE
);

-- Create ChatbotEntities table
CREATE TABLE IF NOT EXISTS "ChatbotEntities" (
    "Id" uuid NOT NULL,
    "IntentId" uuid NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Type" character varying(100) NOT NULL,
    "Description" character varying(255),
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_ChatbotEntities" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ChatbotEntities_ChatbotIntents_IntentId" FOREIGN KEY ("IntentId") REFERENCES "ChatbotIntents" ("Id") ON DELETE CASCADE
);

-- Create MotivationalQuotes table
CREATE TABLE IF NOT EXISTS "MotivationalQuotes" (
    "Id" uuid NOT NULL,
    "QuoteText" text NOT NULL,
    "Author" character varying(255),
    "Category" character varying(100),
    "IsActive" boolean NOT NULL DEFAULT true,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedBy" character varying(100),
    "UpdatedAt" timestamp with time zone,
    "UpdatedBy" character varying(100),
    CONSTRAINT "PK_MotivationalQuotes" PRIMARY KEY ("Id")
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS "IX_AttendancePhotos_AttendanceId" ON "AttendancePhotos" ("AttendanceId");
CREATE INDEX IF NOT EXISTS "IX_MoodEntries_EmployeeId" ON "MoodEntries" ("EmployeeId");
CREATE INDEX IF NOT EXISTS "IX_MoodEntries_EntryDate" ON "MoodEntries" ("EntryDate");
CREATE INDEX IF NOT EXISTS "IX_ChatbotTrainingPhrases_IntentId" ON "ChatbotTrainingPhrases" ("IntentId");
CREATE INDEX IF NOT EXISTS "IX_ChatbotEntities_IntentId" ON "ChatbotEntities" ("IntentId");

-- Insert some sample motivational quotes
INSERT INTO "MotivationalQuotes" ("Id", "QuoteText", "Author", "Category", "IsActive", "CreatedAt", "CreatedBy")
VALUES 
    (gen_random_uuid(), 'Your attitude, not your aptitude, will determine your altitude.', 'Zig Ziglar', 'Motivation', true, NOW(), 'System'),
    (gen_random_uuid(), 'Success is not final, failure is not fatal: It is the courage to continue that counts.', 'Winston Churchill', 'Success', true, NOW(), 'System'),
    (gen_random_uuid(), 'The only way to do great work is to love what you do.', 'Steve Jobs', 'Work', true, NOW(), 'System'),
    (gen_random_uuid(), 'Believe you can and you''re halfway there.', 'Theodore Roosevelt', 'Motivation', true, NOW(), 'System'),
    (gen_random_uuid(), 'The future depends on what you do today.', 'Mahatma Gandhi', 'Productivity', true, NOW(), 'System');

-- Insert sample chatbot intents
INSERT INTO "ChatbotIntents" ("Id", "Name", "Description", "ResponseTemplate", "ApiEndpoint", "CreatedAt", "CreatedBy")
VALUES 
    ('11111111-1111-1111-1111-111111111111', 'greeting', 'Greeting intent', 'Hello! How can I help you today?', NULL, NOW(), 'System'),
    ('22222222-2222-2222-2222-222222222222', 'leave_balance', 'Check leave balance', 'Let me check your leave balance for you.', '/api/Leave/Employee/{employeeId}/Balance', NOW(), 'System'),
    ('33333333-3333-3333-3333-333333333333', 'attendance_today', 'Check today''s attendance', 'Let me check your attendance for today.', '/api/Attendance/Employee/{employeeId}/Today', NOW(), 'System'),
    ('44444444-4444-4444-4444-444444444444', 'help', 'Help intent', 'I can help you with: checking leave balance, attendance status, or company policies. What would you like to know?', NULL, NOW(), 'System');

-- Insert sample training phrases
INSERT INTO "ChatbotTrainingPhrases" ("Id", "IntentId", "Phrase", "CreatedAt")
VALUES 
    (gen_random_uuid(), '11111111-1111-1111-1111-111111111111', 'hello', NOW()),
    (gen_random_uuid(), '11111111-1111-1111-1111-111111111111', 'hi there', NOW()),
    (gen_random_uuid(), '11111111-1111-1111-1111-111111111111', 'good morning', NOW()),
    (gen_random_uuid(), '11111111-1111-1111-1111-111111111111', 'hey', NOW()),
    
    (gen_random_uuid(), '22222222-2222-2222-2222-222222222222', 'how many leaves do I have', NOW()),
    (gen_random_uuid(), '22222222-2222-2222-2222-222222222222', 'check my leave balance', NOW()),
    (gen_random_uuid(), '22222222-2222-2222-2222-222222222222', 'leave balance', NOW()),
    (gen_random_uuid(), '22222222-2222-2222-2222-222222222222', 'how many days off do I have left', NOW()),
    
    (gen_random_uuid(), '33333333-3333-3333-3333-333333333333', 'am I clocked in today', NOW()),
    (gen_random_uuid(), '33333333-3333-3333-3333-333333333333', 'check my attendance', NOW()),
    (gen_random_uuid(), '33333333-3333-3333-3333-333333333333', 'did I clock in', NOW()),
    (gen_random_uuid(), '33333333-3333-3333-3333-333333333333', 'what time did I arrive today', NOW()),
    
    (gen_random_uuid(), '44444444-4444-4444-4444-444444444444', 'help', NOW()),
    (gen_random_uuid(), '44444444-4444-4444-4444-444444444444', 'what can you do', NOW()),
    (gen_random_uuid(), '44444444-4444-4444-4444-444444444444', 'how does this work', NOW()),
    (gen_random_uuid(), '44444444-4444-4444-4444-444444444444', 'I need assistance', NOW());

-- Create directory for attendance photos if it doesn't exist
-- Note: This is a comment only as SQL cannot create directories
-- The application will handle this when it starts up
