@echo off
echo Starting WorkNest HRMS Frontend in Debug Mode...
cd /d %~dp0
echo Frontend will be available at: http://localhost:5173
echo.
echo === DEBUG OUTPUT BELOW ===
echo.
cmd /c npm run dev
pause
