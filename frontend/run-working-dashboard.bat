@echo off
echo Starting WorkNest HRMS Frontend with Working Dashboard...
cd %~dp0
start "" "http://localhost:5175/working-dashboard"
npx vite --port 5175
