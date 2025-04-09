@echo off
echo Starting WorkNest HRMS Frontend...
cd /d %~dp0
echo Frontend will be available at: http://localhost:5173
cmd /c npm run dev
pause
