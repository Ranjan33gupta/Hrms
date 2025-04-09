@echo off
echo Starting WorkNest HRMS Dashboard...
cd %~dp0

:: Kill any existing Node processes
taskkill /F /IM node.exe >nul 2>&1

:: Start the frontend with Vite directly
echo Opening browser to direct dashboard...
start "" http://localhost:5173/direct-dashboard
npx vite --port 5173
