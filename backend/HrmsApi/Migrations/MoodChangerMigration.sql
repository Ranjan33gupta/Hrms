-- Create MoodEntries table if it doesn't exist
CREATE TABLE IF NOT EXISTS "MoodEntries" (
    "Id" UUID PRIMARY KEY,
    "EmployeeId" UUID NULL,
    "UserInput" TEXT NOT NULL,
    "DetectedMood" VARCHAR(50) NOT NULL,
    "ResponseContent" TEXT NOT NULL,
    "CreatedAt" TIMESTAMP NOT NULL,
    "IsAnonymous" BOOLEAN NOT NULL DEFAULT FALSE
);
