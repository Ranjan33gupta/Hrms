@echo off
echo Starting HRMS Applications...

echo.
echo ===================================
echo Starting Backend API (ASP.NET Core)
echo ===================================
echo.
start cmd /k "cd backend\HrmsApi && dotnet run"

echo.
echo ===================================
echo Starting Frontend (React)
echo ===================================
echo.
start cmd /k "cd frontend && npm install && npm run dev"

echo.
echo Both applications are starting in separate windows.
echo Backend API: http://localhost:5171
echo Frontend: http://localhost:5173
echo.
echo Press any key to exit this window...
pause > nul
