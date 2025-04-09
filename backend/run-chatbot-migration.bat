@echo off
echo Running Chatbot Migration...

set PGPASSWORD=postgres
"C:\Program Files\PostgreSQL\15\bin\psql.exe" -U postgres -d hrms_v2 -f "C:\Users\ranja\OneDrive\Documents\Hrms\backend\HrmsApi\Migrations\ChatbotTables.sql"

if %ERRORLEVEL% == 0 (
    echo Migration completed successfully!
) else (
    echo Migration failed with error code %ERRORLEVEL%
)

pause
