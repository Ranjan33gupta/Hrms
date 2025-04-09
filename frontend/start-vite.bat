@echo off
echo Starting WorkNest HRMS Frontend...
cd %~dp0
start http://localhost:5175
npx vite --port 5175
