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

-- Insert some basic intents
INSERT INTO "ChatbotIntents" ("Id", "Name", "Description", "ResponseTemplate", "ApiEndpoint", "RouteDestination", "RequiresAuth", "CreatedAt", "CreatedBy")
VALUES 
(gen_random_uuid(), 'greeting', 'Greeting intent', 'Hello! I''m your WorkNest assistant. How can I help you today?', NULL, NULL, FALSE, NOW(), 'System'),
(gen_random_uuid(), 'goodbye', 'Goodbye intent', 'Goodbye! Have a great day!', NULL, NULL, FALSE, NOW(), 'System'),
(gen_random_uuid(), 'help', 'Help intent', 'I can help you with leave requests, attendance, and employee information. What would you like to know?', NULL, NULL, FALSE, NOW(), 'System'),
(gen_random_uuid(), 'leave_balance', 'Leave balance intent', 'I''ll check your leave balance for you. Please wait a moment.', '/api/Leave/Balance', '/leave/balance', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'attendance_status', 'Attendance status intent', 'Let me check your attendance status.', '/api/Attendance/Status', '/attendance/status', TRUE, NOW(), 'System');

-- Add training phrases for greeting intent
INSERT INTO "ChatbotTrainingPhrases" ("Id", "IntentId", "Phrase", "CreatedAt")
SELECT gen_random_uuid(), "Id", 'hello', NOW() FROM "ChatbotIntents" WHERE "Name" = 'greeting'
UNION ALL
SELECT gen_random_uuid(), "Id", 'hi', NOW() FROM "ChatbotIntents" WHERE "Name" = 'greeting'
UNION ALL
SELECT gen_random_uuid(), "Id", 'hey there', NOW() FROM "ChatbotIntents" WHERE "Name" = 'greeting'
UNION ALL
SELECT gen_random_uuid(), "Id", 'good morning', NOW() FROM "ChatbotIntents" WHERE "Name" = 'greeting'
UNION ALL
SELECT gen_random_uuid(), "Id", 'good afternoon', NOW() FROM "ChatbotIntents" WHERE "Name" = 'greeting';

-- Add training phrases for goodbye intent
INSERT INTO "ChatbotTrainingPhrases" ("Id", "IntentId", "Phrase", "CreatedAt")
SELECT gen_random_uuid(), "Id", 'goodbye', NOW() FROM "ChatbotIntents" WHERE "Name" = 'goodbye'
UNION ALL
SELECT gen_random_uuid(), "Id", 'bye', NOW() FROM "ChatbotIntents" WHERE "Name" = 'goodbye'
UNION ALL
SELECT gen_random_uuid(), "Id", 'see you later', NOW() FROM "ChatbotIntents" WHERE "Name" = 'goodbye'
UNION ALL
SELECT gen_random_uuid(), "Id", 'have a nice day', NOW() FROM "ChatbotIntents" WHERE "Name" = 'goodbye';

-- Add training phrases for help intent
INSERT INTO "ChatbotTrainingPhrases" ("Id", "IntentId", "Phrase", "CreatedAt")
SELECT gen_random_uuid(), "Id", 'help', NOW() FROM "ChatbotIntents" WHERE "Name" = 'help'
UNION ALL
SELECT gen_random_uuid(), "Id", 'what can you do', NOW() FROM "ChatbotIntents" WHERE "Name" = 'help'
UNION ALL
SELECT gen_random_uuid(), "Id", 'how can you help me', NOW() FROM "ChatbotIntents" WHERE "Name" = 'help'
UNION ALL
SELECT gen_random_uuid(), "Id", 'show me what you can do', NOW() FROM "ChatbotIntents" WHERE "Name" = 'help';

-- Add training phrases for leave balance intent
INSERT INTO "ChatbotTrainingPhrases" ("Id", "IntentId", "Phrase", "CreatedAt")
SELECT gen_random_uuid(), "Id", 'leave balance', NOW() FROM "ChatbotIntents" WHERE "Name" = 'leave_balance'
UNION ALL
SELECT gen_random_uuid(), "Id", 'how many leaves do i have', NOW() FROM "ChatbotIntents" WHERE "Name" = 'leave_balance'
UNION ALL
SELECT gen_random_uuid(), "Id", 'check my leave balance', NOW() FROM "ChatbotIntents" WHERE "Name" = 'leave_balance'
UNION ALL
SELECT gen_random_uuid(), "Id", 'how many days off do i have left', NOW() FROM "ChatbotIntents" WHERE "Name" = 'leave_balance';

-- Add training phrases for attendance status intent
INSERT INTO "ChatbotTrainingPhrases" ("Id", "IntentId", "Phrase", "CreatedAt")
SELECT gen_random_uuid(), "Id", 'attendance status', NOW() FROM "ChatbotIntents" WHERE "Name" = 'attendance_status'
UNION ALL
SELECT gen_random_uuid(), "Id", 'am i clocked in', NOW() FROM "ChatbotIntents" WHERE "Name" = 'attendance_status'
UNION ALL
SELECT gen_random_uuid(), "Id", 'check my attendance', NOW() FROM "ChatbotIntents" WHERE "Name" = 'attendance_status'
UNION ALL
SELECT gen_random_uuid(), "Id", 'what is my attendance status', NOW() FROM "ChatbotIntents" WHERE "Name" = 'attendance_status';
