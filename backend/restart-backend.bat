@echo off
echo Killing any existing HrmsApi processes...
taskkill /f /im HrmsApi.exe 2>nul
taskkill /f /fi "WINDOWTITLE eq HrmsApi" /im dotnet.exe 2>nul

echo Cleaning project...
dotnet clean .\HrmsApi\HrmsApi.csproj

echo Removing bin and obj folders...
if exist .\HrmsApi\bin rmdir /s /q .\HrmsApi\bin
if exist .\HrmsApi\obj rmdir /s /q .\HrmsApi\obj

echo Building project...
dotnet build .\HrmsApi\HrmsApi.csproj

echo Starting HrmsApi...
dotnet run --project .\HrmsApi\HrmsApi.csproj
