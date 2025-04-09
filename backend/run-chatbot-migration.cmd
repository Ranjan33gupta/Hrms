@echo off
echo Running WorkNest Chatbot Database Migration...
cd /d %~dp0
cd HrmsApi
dotnet ef database update --context ChatbotDbContext
echo Migration completed!
pause
