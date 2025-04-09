@echo off
echo Starting WorkNest HRMS System...

echo.
echo === Starting Backend ===
start cmd /k "cd backend && run-fast.bat"

echo.
echo === Starting Frontend ===
start cmd /k "cd frontend && npm run dev"

echo.
echo === Opening Test Dashboard ===
timeout /t 5
start http://localhost:5175/test-dashboard

echo.
echo WorkNest HRMS System started successfully!
echo Backend: http://localhost:5171
echo Frontend: http://localhost:5175
echo Test Dashboard: http://localhost:5175/test-dashboard
