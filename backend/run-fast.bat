@echo off
echo Starting WorkNest backend in fast mode (in-memory database)...
cd %~dp0HrmsApi
set ASPNETCORE_ENVIRONMENT=Development
set ConnectionStrings__DefaultConnection=
dotnet run --no-build
