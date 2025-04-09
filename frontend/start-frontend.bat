@echo off
echo Starting HRMS Frontend...

REM Navigate to the frontend directory (in case this is run from elsewhere)
cd /d "%~dp0"

REM Check if Node.js is installed
where node >nul 2>nul
if %ERRORLEVEL% neq 0 (
    echo ERROR: Node.js is not installed or not in PATH.
    echo Please install Node.js from https://nodejs.org/
    pause
    exit /b 1
)

REM Run npx directly to avoid PowerShell execution policy issues
echo Starting Vite development server...
npx vite

REM If we get here, the server has stopped
echo Vite development server has stopped.
pause
