-- Create Chatbot Conversations table
CREATE TABLE IF NOT EXISTS "ChatbotConversations" (
    "Id" UUID PRIMARY KEY,
    "EmployeeId" UUID NULL,
    "StartedAt" TIMESTAMP NOT NULL,
    "LastMessageAt" TIMESTAMP NOT NULL,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE
);

-- Create Chatbot Messages table
CREATE TABLE IF NOT EXISTS "ChatbotMessages" (
    "Id" UUID PRIMARY KEY,
    "ConversationId" UUID NOT NULL,
    "Content" TEXT NOT NULL,
    "Timestamp" TIMESTAMP NOT NULL,
    "IsFromUser" BOOLEAN NOT NULL,
    FOREIGN KEY ("ConversationId") REFERENCES "ChatbotConversations" ("Id") ON DELETE CASCADE
);

-- Create Chatbot Intents table
CREATE TABLE IF NOT EXISTS "ChatbotIntents" (
    "Id" UUID PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Description" TEXT NULL,
    "ResponseTemplate" TEXT NOT NULL,
    "ApiEndpoint" VARCHAR(255) NULL,
    "RouteDestination" VARCHAR(255) NULL,
    "RequiresAuth" BOOLEAN NOT NULL DEFAULT FALSE,
    "CreatedAt" TIMESTAMP NOT NULL,
    "CreatedBy" VARCHAR(100) NOT NULL
);

-- Create Chatbot Training Phrases table
CREATE TABLE IF NOT EXISTS "ChatbotTrainingPhrases" (
    "Id" UUID PRIMARY KEY,
    "IntentId" UUID NOT NULL,
    "Phrase" TEXT NOT NULL,
    "CreatedAt" TIMESTAMP NOT NULL,
    FOREIGN KEY ("IntentId") REFERENCES "ChatbotIntents" ("Id") ON DELETE CASCADE
);

-- Create Chatbot Entities table
CREATE TABLE IF NOT EXISTS "ChatbotEntities" (
    "Id" UUID PRIMARY KEY,
    "IntentId" UUID NOT NULL,
    "Name" VARCHAR(100) NOT NULL,
    "Type" VARCHAR(50) NOT NULL,
    "Description" TEXT NULL,
    "CreatedAt" TIMESTAMP NOT NULL,
    FOREIGN KEY ("IntentId") REFERENCES "ChatbotIntents" ("Id") ON DELETE CASCADE
);

-- Insert basic intents
INSERT INTO "ChatbotIntents" ("Id", "Name", "Description", "ResponseTemplate", "ApiEndpoint", "RouteDestination", "RequiresAuth", "CreatedAt", "CreatedBy")
VALUES 
(gen_random_uuid(), 'greeting', 'Greeting intent', 'Hello! I''m your WorkNest assistant. How can I help you today?', NULL, NULL, FALSE, NOW(), 'System'),
(gen_random_uuid(), 'goodbye', 'Goodbye intent', 'Goodbye! Have a great day!', NULL, NULL, FALSE, NOW(), 'System'),
(gen_random_uuid(), 'help', 'Help intent', 'I can help you with leave requests, attendance, and employee information. What would you like to know?', NULL, NULL, FALSE, NOW(), 'System'),
(gen_random_uuid(), 'leave_balance', 'Leave balance intent', 'I''ll check your leave balance for you. Please wait a moment.', '/api/Leave/Balance', '/leave/balance', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'attendance_status', 'Attendance status intent', 'Let me check your attendance status.', '/api/Attendance/Status', '/attendance/status', TRUE, NOW(), 'System');

-- Add training phrases for intents
DO $$
DECLARE
    greeting_id UUID;
    goodbye_id UUID;
    help_id UUID;
    leave_balance_id UUID;
    attendance_status_id UUID;
BEGIN
    -- Get the IDs
    SELECT "Id" INTO greeting_id FROM "ChatbotIntents" WHERE "Name" = 'greeting' LIMIT 1;
    SELECT "Id" INTO goodbye_id FROM "ChatbotIntents" WHERE "Name" = 'goodbye' LIMIT 1;
    SELECT "Id" INTO help_id FROM "ChatbotIntents" WHERE "Name" = 'help' LIMIT 1;
    SELECT "Id" INTO leave_balance_id FROM "ChatbotIntents" WHERE "Name" = 'leave_balance' LIMIT 1;
    SELECT "Id" INTO attendance_status_id FROM "ChatbotIntents" WHERE "Name" = 'attendance_status' LIMIT 1;
    
    -- Add training phrases for greeting intent
    IF greeting_id IS NOT NULL THEN
        INSERT INTO "ChatbotTrainingPhrases" ("Id", "IntentId", "Phrase", "CreatedAt")
        VALUES 
        (gen_random_uuid(), greeting_id, 'hello', NOW()),
        (gen_random_uuid(), greeting_id, 'hi', NOW()),
        (gen_random_uuid(), greeting_id, 'hey', NOW()),
        (gen_random_uuid(), greeting_id, 'good morning', NOW()),
        (gen_random_uuid(), greeting_id, 'good afternoon', NOW()),
        (gen_random_uuid(), greeting_id, 'good evening', NOW());
    END IF;
    
    -- Add training phrases for goodbye intent
    IF goodbye_id IS NOT NULL THEN
        INSERT INTO "ChatbotTrainingPhrases" ("Id", "IntentId", "Phrase", "CreatedAt")
        VALUES 
        (gen_random_uuid(), goodbye_id, 'goodbye', NOW()),
        (gen_random_uuid(), goodbye_id, 'bye', NOW()),
        (gen_random_uuid(), goodbye_id, 'see you later', NOW()),
        (gen_random_uuid(), goodbye_id, 'have a nice day', NOW()),
        (gen_random_uuid(), goodbye_id, 'talk to you later', NOW());
    END IF;
    
    -- Add training phrases for help intent
    IF help_id IS NOT NULL THEN
        INSERT INTO "ChatbotTrainingPhrases" ("Id", "IntentId", "Phrase", "CreatedAt")
        VALUES 
        (gen_random_uuid(), help_id, 'help', NOW()),
        (gen_random_uuid(), help_id, 'I need help', NOW()),
        (gen_random_uuid(), help_id, 'what can you do', NOW()),
        (gen_random_uuid(), help_id, 'how can you help me', NOW()),
        (gen_random_uuid(), help_id, 'what are your features', NOW());
    END IF;
    
    -- Add training phrases for leave balance intent
    IF leave_balance_id IS NOT NULL THEN
        INSERT INTO "ChatbotTrainingPhrases" ("Id", "IntentId", "Phrase", "CreatedAt")
        VALUES 
        (gen_random_uuid(), leave_balance_id, 'leave balance', NOW()),
        (gen_random_uuid(), leave_balance_id, 'how many leaves do I have', NOW()),
        (gen_random_uuid(), leave_balance_id, 'check my leave balance', NOW()),
        (gen_random_uuid(), leave_balance_id, 'remaining leaves', NOW()),
        (gen_random_uuid(), leave_balance_id, 'how many days off do I have left', NOW());
    END IF;
    
    -- Add training phrases for attendance status intent
    IF attendance_status_id IS NOT NULL THEN
        INSERT INTO "ChatbotTrainingPhrases" ("Id", "IntentId", "Phrase", "CreatedAt")
        VALUES 
        (gen_random_uuid(), attendance_status_id, 'attendance status', NOW()),
        (gen_random_uuid(), attendance_status_id, 'check my attendance', NOW()),
        (gen_random_uuid(), attendance_status_id, 'am I marked present today', NOW()),
        (gen_random_uuid(), attendance_status_id, 'show my attendance', NOW()),
        (gen_random_uuid(), attendance_status_id, 'attendance record', NOW());
    END IF;
END $$;
