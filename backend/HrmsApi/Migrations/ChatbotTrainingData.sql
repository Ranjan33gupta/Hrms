-- Create Chatbot Conversations table if it doesn't exist
CREATE TABLE IF NOT EXISTS "ChatbotConversations" (
    "Id" UUID PRIMARY KEY,
    "EmployeeId" UUID NULL,
    "StartedAt" TIMESTAMP NOT NULL,
    "LastMessageAt" TIMESTAMP NOT NULL,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE
);

-- Create Chatbot Messages table if it doesn't exist
CREATE TABLE IF NOT EXISTS "ChatbotMessages" (
    "Id" UUID PRIMARY KEY,
    "ConversationId" UUID NOT NULL,
    "Content" TEXT NOT NULL,
    "Timestamp" TIMESTAMP NOT NULL,
    "IsFromUser" BOOLEAN NOT NULL,
    FOREIGN KEY ("ConversationId") REFERENCES "ChatbotConversations" ("Id") ON DELETE CASCADE
);

-- Create Chatbot Intents table if it doesn't exist
CREATE TABLE IF NOT EXISTS "ChatbotIntents" (
    "Id" UUID PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Description" TEXT NULL,
    "Category" VARCHAR(100) NULL,
    "ResponseTemplate" TEXT NOT NULL,
    "ApiEndpoint" VARCHAR(255) NULL,
    "RouteDestination" VARCHAR(255) NULL,
    "RequiresAuth" BOOLEAN NOT NULL DEFAULT FALSE,
    "CreatedAt" TIMESTAMP NOT NULL,
    "CreatedBy" VARCHAR(100) NOT NULL
);

-- Create Chatbot Training Phrases table if it doesn't exist
CREATE TABLE IF NOT EXISTS "ChatbotTrainingPhrases" (
    "Id" UUID PRIMARY KEY,
    "IntentId" UUID NOT NULL,
    "Phrase" TEXT NOT NULL,
    "CreatedAt" TIMESTAMP NOT NULL,
    FOREIGN KEY ("IntentId") REFERENCES "ChatbotIntents" ("Id") ON DELETE CASCADE
);

-- Create Chatbot Entities table if it doesn't exist
CREATE TABLE IF NOT EXISTS "ChatbotEntities" (
    "Id" UUID PRIMARY KEY,
    "IntentId" UUID NOT NULL,
    "Name" VARCHAR(100) NOT NULL,
    "Type" VARCHAR(50) NOT NULL,
    "Description" TEXT NULL,
    "CreatedAt" TIMESTAMP NOT NULL,
    FOREIGN KEY ("IntentId") REFERENCES "ChatbotIntents" ("Id") ON DELETE CASCADE
);

-- Clear existing data for clean import
DELETE FROM "ChatbotTrainingPhrases";
DELETE FROM "ChatbotEntities";
DELETE FROM "ChatbotIntents";

-- Insert basic intents
INSERT INTO "ChatbotIntents" ("Id", "Name", "Description", "Category", "ResponseTemplate", "ApiEndpoint", "RouteDestination", "RequiresAuth", "CreatedAt", "CreatedBy")
VALUES 
-- General intents
(gen_random_uuid(), 'greeting', 'Greeting intent', 'General', 'Hello! I''m your WorkNest assistant. How can I help you today?', NULL, NULL, FALSE, NOW(), 'System'),
(gen_random_uuid(), 'goodbye', 'Goodbye intent', 'General', 'Goodbye! Have a great day!', NULL, NULL, FALSE, NOW(), 'System'),
(gen_random_uuid(), 'help', 'Help intent', 'General', 'I can help you with attendance, leave management, navigation, performance reviews, and training. What would you like to know?', NULL, NULL, FALSE, NOW(), 'System'),
(gen_random_uuid(), 'thanks', 'Thanks intent', 'General', 'You''re welcome! Is there anything else I can help you with?', NULL, NULL, FALSE, NOW(), 'System'),

-- Attendance intents
(gen_random_uuid(), 'attendance_status', 'Check current attendance status', 'Attendance', 'Let me check your attendance status.', '/api/Attendance/Status', '/attendance', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'attendance_monthly', 'Check monthly attendance', 'Attendance', 'Here''s your attendance for this month.', '/api/Attendance/Monthly', '/attendance/monthly', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'attendance_today', 'Check if attendance marked today', 'Attendance', 'Let me check if you''ve marked your attendance today.', '/api/Attendance/Today', '/attendance', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'attendance_last_checkin', 'Check last check-in time', 'Attendance', 'Your last check-in was at {time} on {date}.', '/api/Attendance/LastCheckIn', '/attendance', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'attendance_yesterday', 'Check yesterday''s attendance', 'Attendance', 'Here are your in and out times for yesterday.', '/api/Attendance/Date?date={yesterday}', '/attendance', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'attendance_weekly', 'Check weekly attendance', 'Attendance', 'Here''s your attendance for this week.', '/api/Attendance/Weekly', '/attendance/weekly', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'attendance_late_today', 'Check if late today', 'Attendance', 'Let me check if you''re late today.', '/api/Attendance/LateToday', '/attendance', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'attendance_hours_worked', 'Check hours worked this week', 'Attendance', 'You''ve worked {hours} hours this week.', '/api/Attendance/HoursWorked?period=week', '/attendance/weekly', TRUE, NOW(), 'System'),

-- Leave Management intents
(gen_random_uuid(), 'leave_balance', 'Check leave balance', 'Leave', 'Here''s your current leave balance.', '/api/Leave/Balance', '/leave/balance', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'leave_apply', 'Apply for leave', 'Leave', 'I can help you apply for leave. Please provide the dates.', NULL, '/leave/apply', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'leave_apply_dates', 'Apply for leave with dates', 'Leave', 'I''ll help you apply for leave from {startDate} to {endDate}.', '/api/Leave/Apply', '/leave/apply', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'leave_policy', 'Check leave policy', 'Leave', 'Here''s information about our leave policy.', '/api/Leave/Policy', '/leave/policy', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'leave_availability', 'Check if leave possible on specific date', 'Leave', 'Let me check if you can take leave on {date}.', '/api/Leave/Availability?date={date}', '/leave/apply', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'leave_casual_balance', 'Check casual leave balance', 'Leave', 'You have {count} casual leaves available.', '/api/Leave/Balance?type=casual', '/leave/balance', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'leave_cancel', 'Cancel leave request', 'Leave', 'I can help you cancel your leave request.', NULL, '/leave/requests', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'leave_status', 'Check leave request status', 'Leave', 'Here''s the status of your leave requests.', '/api/Leave/Requests', '/leave/requests', TRUE, NOW(), 'System'),

-- Navigation intents
(gen_random_uuid(), 'navigate_dashboard', 'Navigate to dashboard', 'Navigation', 'Taking you to the dashboard.', NULL, '/dashboard', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'navigate_attendance', 'Navigate to attendance page', 'Navigation', 'Taking you to the attendance page.', NULL, '/attendance', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'navigate_leave', 'Navigate to leave section', 'Navigation', 'Taking you to the leave section.', NULL, '/leave', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'navigate_profile', 'Navigate to profile', 'Navigation', 'Taking you to your profile.', NULL, '/profile', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'navigate_performance', 'Navigate to performance review', 'Navigation', 'Taking you to performance reviews.', NULL, '/performance', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'navigate_training', 'Navigate to training portal', 'Navigation', 'Taking you to the training portal.', NULL, '/training', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'navigate_recruitment', 'Navigate to recruitment page', 'Navigation', 'Taking you to the recruitment page.', NULL, '/recruitment', TRUE, NOW(), 'System'),

-- Performance & Appraisal intents
(gen_random_uuid(), 'performance_report', 'View performance report', 'Performance', 'Here''s your performance report.', '/api/Performance/Report', '/performance/report', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'performance_appraisal', 'Check last appraisal', 'Performance', 'Here''s information about your last appraisal.', '/api/Performance/LastAppraisal', '/performance/appraisals', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'performance_feedback', 'View manager feedback', 'Performance', 'Here''s the feedback from your manager.', '/api/Performance/Feedback', '/performance/feedback', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'performance_goals', 'Check performance goals', 'Performance', 'Here are your current performance goals.', '/api/Performance/Goals', '/performance/goals', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'performance_rate_goals', 'Rate performance goals', 'Performance', 'I can help you rate your goals.', NULL, '/performance/goals/rate', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'performance_skill_evaluation', 'Check skill evaluation results', 'Performance', 'Here are your skill evaluation results.', '/api/Performance/SkillEvaluation', '/performance/skills', TRUE, NOW(), 'System'),

-- Training & Learning intents
(gen_random_uuid(), 'training_enrolled', 'List enrolled courses', 'Training', 'Here are the courses you''re currently enrolled in.', '/api/Training/Enrolled', '/training/courses', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'training_start', 'Start training', 'Training', 'I can help you start your training.', NULL, '/training/start', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'training_completed', 'View completed trainings', 'Training', 'Here are the trainings you''ve completed.', '/api/Training/Completed', '/training/completed', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'training_certifications', 'Check certifications', 'Training', 'Here are the certifications you currently have.', '/api/Training/Certifications', '/training/certifications', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'training_new_sessions', 'Check for new training sessions', 'Training', 'Here are the new training sessions available.', '/api/Training/NewSessions', '/training/new', TRUE, NOW(), 'System'),
(gen_random_uuid(), 'training_learning_path', 'View learning path', 'Training', 'Here''s your current learning path.', '/api/Training/LearningPath', '/training/path', TRUE, NOW(), 'System');

-- Add training phrases for Attendance intents
INSERT INTO "ChatbotTrainingPhrases" ("Id", "IntentId", "Phrase", "CreatedAt")
SELECT gen_random_uuid(), "Id", 'What''s my attendance for this month?', NOW() FROM "ChatbotIntents" WHERE "Name" = 'attendance_monthly'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Show my attendance this month', NOW() FROM "ChatbotIntents" WHERE "Name" = 'attendance_monthly'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Monthly attendance report', NOW() FROM "ChatbotIntents" WHERE "Name" = 'attendance_monthly'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Did I mark attendance today?', NOW() FROM "ChatbotIntents" WHERE "Name" = 'attendance_today'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Have I checked in today?', NOW() FROM "ChatbotIntents" WHERE "Name" = 'attendance_today'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Did I clock in today?', NOW() FROM "ChatbotIntents" WHERE "Name" = 'attendance_today'
UNION ALL
SELECT gen_random_uuid(), "Id", 'When was my last check-in?', NOW() FROM "ChatbotIntents" WHERE "Name" = 'attendance_last_checkin'
UNION ALL
SELECT gen_random_uuid(), "Id", 'What time did I last clock in?', NOW() FROM "ChatbotIntents" WHERE "Name" = 'attendance_last_checkin'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Show my in and out time for yesterday', NOW() FROM "ChatbotIntents" WHERE "Name" = 'attendance_yesterday'
UNION ALL
SELECT gen_random_uuid(), "Id", 'What was my attendance yesterday?', NOW() FROM "ChatbotIntents" WHERE "Name" = 'attendance_yesterday'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Check my weekly attendance', NOW() FROM "ChatbotIntents" WHERE "Name" = 'attendance_weekly'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Show my attendance for this week', NOW() FROM "ChatbotIntents" WHERE "Name" = 'attendance_weekly'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Am I late today?', NOW() FROM "ChatbotIntents" WHERE "Name" = 'attendance_late_today'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Did I come late today?', NOW() FROM "ChatbotIntents" WHERE "Name" = 'attendance_late_today'
UNION ALL
SELECT gen_random_uuid(), "Id", 'How many hours did I work this week?', NOW() FROM "ChatbotIntents" WHERE "Name" = 'attendance_hours_worked'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Total hours worked this week', NOW() FROM "ChatbotIntents" WHERE "Name" = 'attendance_hours_worked';

-- Add training phrases for Leave Management intents
INSERT INTO "ChatbotTrainingPhrases" ("Id", "IntentId", "Phrase", "CreatedAt")
SELECT gen_random_uuid(), "Id", 'How many leaves do I have left?', NOW() FROM "ChatbotIntents" WHERE "Name" = 'leave_balance'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Show me my leave balance', NOW() FROM "ChatbotIntents" WHERE "Name" = 'leave_balance'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Check my remaining leaves', NOW() FROM "ChatbotIntents" WHERE "Name" = 'leave_balance'
UNION ALL
SELECT gen_random_uuid(), "Id", 'I want to apply for leave', NOW() FROM "ChatbotIntents" WHERE "Name" = 'leave_apply'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Help me request time off', NOW() FROM "ChatbotIntents" WHERE "Name" = 'leave_apply'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Apply for leave from 10th to 12th April', NOW() FROM "ChatbotIntents" WHERE "Name" = 'leave_apply_dates'
UNION ALL
SELECT gen_random_uuid(), "Id", 'I need leave from Monday to Wednesday', NOW() FROM "ChatbotIntents" WHERE "Name" = 'leave_apply_dates'
UNION ALL
SELECT gen_random_uuid(), "Id", 'What''s the leave policy?', NOW() FROM "ChatbotIntents" WHERE "Name" = 'leave_policy'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Tell me about leave rules', NOW() FROM "ChatbotIntents" WHERE "Name" = 'leave_policy'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Can I take leave on Friday?', NOW() FROM "ChatbotIntents" WHERE "Name" = 'leave_availability'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Is it possible to take leave next Monday?', NOW() FROM "ChatbotIntents" WHERE "Name" = 'leave_availability'
UNION ALL
SELECT gen_random_uuid(), "Id", 'How many casual leaves are available?', NOW() FROM "ChatbotIntents" WHERE "Name" = 'leave_casual_balance'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Check my casual leave balance', NOW() FROM "ChatbotIntents" WHERE "Name" = 'leave_casual_balance'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Cancel my leave request', NOW() FROM "ChatbotIntents" WHERE "Name" = 'leave_cancel'
UNION ALL
SELECT gen_random_uuid(), "Id", 'I want to withdraw my leave application', NOW() FROM "ChatbotIntents" WHERE "Name" = 'leave_cancel'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Check leave request status', NOW() FROM "ChatbotIntents" WHERE "Name" = 'leave_status'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Has my leave been approved?', NOW() FROM "ChatbotIntents" WHERE "Name" = 'leave_status';

-- Add training phrases for Navigation intents
INSERT INTO "ChatbotTrainingPhrases" ("Id", "IntentId", "Phrase", "CreatedAt")
SELECT gen_random_uuid(), "Id", 'Take me to dashboard', NOW() FROM "ChatbotIntents" WHERE "Name" = 'navigate_dashboard'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Go to main page', NOW() FROM "ChatbotIntents" WHERE "Name" = 'navigate_dashboard'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Show dashboard', NOW() FROM "ChatbotIntents" WHERE "Name" = 'navigate_dashboard'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Go to attendance page', NOW() FROM "ChatbotIntents" WHERE "Name" = 'navigate_attendance'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Take me to attendance', NOW() FROM "ChatbotIntents" WHERE "Name" = 'navigate_attendance'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Open leave section', NOW() FROM "ChatbotIntents" WHERE "Name" = 'navigate_leave'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Go to leave management', NOW() FROM "ChatbotIntents" WHERE "Name" = 'navigate_leave'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Show my profile', NOW() FROM "ChatbotIntents" WHERE "Name" = 'navigate_profile'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Take me to my account', NOW() FROM "ChatbotIntents" WHERE "Name" = 'navigate_profile'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Navigate to performance review', NOW() FROM "ChatbotIntents" WHERE "Name" = 'navigate_performance'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Go to performance section', NOW() FROM "ChatbotIntents" WHERE "Name" = 'navigate_performance'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Open training portal', NOW() FROM "ChatbotIntents" WHERE "Name" = 'navigate_training'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Show me training options', NOW() FROM "ChatbotIntents" WHERE "Name" = 'navigate_training'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Take me to recruitment page', NOW() FROM "ChatbotIntents" WHERE "Name" = 'navigate_recruitment'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Go to hiring section', NOW() FROM "ChatbotIntents" WHERE "Name" = 'navigate_recruitment';

-- Add training phrases for Performance & Appraisal intents
INSERT INTO "ChatbotTrainingPhrases" ("Id", "IntentId", "Phrase", "CreatedAt")
SELECT gen_random_uuid(), "Id", 'Show my performance report', NOW() FROM "ChatbotIntents" WHERE "Name" = 'performance_report'
UNION ALL
SELECT gen_random_uuid(), "Id", 'View my performance metrics', NOW() FROM "ChatbotIntents" WHERE "Name" = 'performance_report'
UNION ALL
SELECT gen_random_uuid(), "Id", 'How was my last appraisal?', NOW() FROM "ChatbotIntents" WHERE "Name" = 'performance_appraisal'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Show my recent performance review', NOW() FROM "ChatbotIntents" WHERE "Name" = 'performance_appraisal'
UNION ALL
SELECT gen_random_uuid(), "Id", 'View feedback from manager', NOW() FROM "ChatbotIntents" WHERE "Name" = 'performance_feedback'
UNION ALL
SELECT gen_random_uuid(), "Id", 'What feedback did I receive?', NOW() FROM "ChatbotIntents" WHERE "Name" = 'performance_feedback'
UNION ALL
SELECT gen_random_uuid(), "Id", 'What are my performance goals?', NOW() FROM "ChatbotIntents" WHERE "Name" = 'performance_goals'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Show my objectives', NOW() FROM "ChatbotIntents" WHERE "Name" = 'performance_goals'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Rate my goals', NOW() FROM "ChatbotIntents" WHERE "Name" = 'performance_rate_goals'
UNION ALL
SELECT gen_random_uuid(), "Id", 'I want to update my goal progress', NOW() FROM "ChatbotIntents" WHERE "Name" = 'performance_rate_goals'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Check skill evaluation results', NOW() FROM "ChatbotIntents" WHERE "Name" = 'performance_skill_evaluation'
UNION ALL
SELECT gen_random_uuid(), "Id", 'How did I do in my skills assessment?', NOW() FROM "ChatbotIntents" WHERE "Name" = 'performance_skill_evaluation';

-- Add training phrases for Training & Learning intents
INSERT INTO "ChatbotTrainingPhrases" ("Id", "IntentId", "Phrase", "CreatedAt")
SELECT gen_random_uuid(), "Id", 'List my enrolled courses', NOW() FROM "ChatbotIntents" WHERE "Name" = 'training_enrolled'
UNION ALL
SELECT gen_random_uuid(), "Id", 'What courses am I taking?', NOW() FROM "ChatbotIntents" WHERE "Name" = 'training_enrolled'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Start my training', NOW() FROM "ChatbotIntents" WHERE "Name" = 'training_start'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Begin my course', NOW() FROM "ChatbotIntents" WHERE "Name" = 'training_start'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Show completed trainings', NOW() FROM "ChatbotIntents" WHERE "Name" = 'training_completed'
UNION ALL
SELECT gen_random_uuid(), "Id", 'What courses have I finished?', NOW() FROM "ChatbotIntents" WHERE "Name" = 'training_completed'
UNION ALL
SELECT gen_random_uuid(), "Id", 'What certifications do I have?', NOW() FROM "ChatbotIntents" WHERE "Name" = 'training_certifications'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Show my certificates', NOW() FROM "ChatbotIntents" WHERE "Name" = 'training_certifications'
UNION ALL
SELECT gen_random_uuid(), "Id", 'Are there any new training sessions?', NOW() FROM "ChatbotIntents" WHERE "Name" = 'training_new_sessions'
UNION ALL
SELECT gen_random_uuid(), "Id", 'What new courses are available?', NOW() FROM "ChatbotIntents" WHERE "Name" = 'training_new_sessions'
UNION ALL
SELECT gen_random_uuid(), "Id", 'View my learning path', NOW() FROM "ChatbotIntents" WHERE "Name" = 'training_learning_path'
UNION ALL
SELECT gen_random_uuid(), "Id", 'What should I learn next?', NOW() FROM "ChatbotIntents" WHERE "Name" = 'training_learning_path';

-- Add training phrases for General intents
INSERT INTO "ChatbotTrainingPhrases" ("Id", "IntentId", "Phrase", "CreatedAt")
SELECT gen_random_uuid(), "Id", 'hello', NOW() FROM "ChatbotIntents" WHERE "Name" = 'greeting'
UNION ALL
SELECT gen_random_uuid(), "Id", 'hi', NOW() FROM "ChatbotIntents" WHERE "Name" = 'greeting'
UNION ALL
SELECT gen_random_uuid(), "Id", 'hey there', NOW() FROM "ChatbotIntents" WHERE "Name" = 'greeting'
UNION ALL
SELECT gen_random_uuid(), "Id", 'good morning', NOW() FROM "ChatbotIntents" WHERE "Name" = 'greeting'
UNION ALL
SELECT gen_random_uuid(), "Id", 'good afternoon', NOW() FROM "ChatbotIntents" WHERE "Name" = 'greeting'
UNION ALL
SELECT gen_random_uuid(), "Id", 'goodbye', NOW() FROM "ChatbotIntents" WHERE "Name" = 'goodbye'
UNION ALL
SELECT gen_random_uuid(), "Id", 'bye', NOW() FROM "ChatbotIntents" WHERE "Name" = 'goodbye'
UNION ALL
SELECT gen_random_uuid(), "Id", 'see you later', NOW() FROM "ChatbotIntents" WHERE "Name" = 'goodbye'
UNION ALL
SELECT gen_random_uuid(), "Id", 'have a nice day', NOW() FROM "ChatbotIntents" WHERE "Name" = 'goodbye'
UNION ALL
SELECT gen_random_uuid(), "Id", 'help', NOW() FROM "ChatbotIntents" WHERE "Name" = 'help'
UNION ALL
SELECT gen_random_uuid(), "Id", 'what can you do', NOW() FROM "ChatbotIntents" WHERE "Name" = 'help'
UNION ALL
SELECT gen_random_uuid(), "Id", 'how can you help me', NOW() FROM "ChatbotIntents" WHERE "Name" = 'help'
UNION ALL
SELECT gen_random_uuid(), "Id", 'show me what you can do', NOW() FROM "ChatbotIntents" WHERE "Name" = 'help'
UNION ALL
SELECT gen_random_uuid(), "Id", 'thanks', NOW() FROM "ChatbotIntents" WHERE "Name" = 'thanks'
UNION ALL
SELECT gen_random_uuid(), "Id", 'thank you', NOW() FROM "ChatbotIntents" WHERE "Name" = 'thanks'
UNION ALL
SELECT gen_random_uuid(), "Id", 'appreciate it', NOW() FROM "ChatbotIntents" WHERE "Name" = 'thanks';
