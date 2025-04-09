@echo off
echo Starting WorkNest HRMS Frontend Application...
cd %~dp0

:: Kill any existing Node processes to free up the port
taskkill /F /IM node.exe >nul 2>&1

:: Open the browser to the application
start "" http://localhost:5173

:: Use direct command to run Vite without relying on npm scripts
echo Starting Vite development server...
npx --yes vite
