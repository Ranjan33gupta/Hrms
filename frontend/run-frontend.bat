@echo off
echo Starting WorkNest frontend...
cd %~dp0
start "" http://localhost:5174
npm run dev
