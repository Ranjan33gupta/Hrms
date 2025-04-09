@echo off
echo Starting WorkNest HRMS React Application with alternative method...
cd %~dp0

:: Kill any existing Node processes
taskkill /F /IM node.exe >nul 2>&1

:: Open browser to the login page
start "" http://localhost:5173

:: Run the alternative development script
node run-dev.js
