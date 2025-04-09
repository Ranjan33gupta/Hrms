# Kill any existing HrmsApi processes
Get-Process | Where-Object {$_.ProcessName -eq "HrmsApi" -or ($_.ProcessName -eq "dotnet" -and $_.MainWindowTitle -like "*HrmsApi*")} | ForEach-Object { 
    Write-Host "Stopping process: $($_.ProcessName) (ID: $($_.Id))"
    Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue 
}

# Clean the project
Write-Host "Cleaning project..."
dotnet clean .\HrmsApi\HrmsApi.csproj

# Remove bin and obj folders to ensure clean build
Write-Host "Removing bin and obj folders..."
if (Test-Path .\HrmsApi\bin) { Remove-Item -Path .\HrmsApi\bin -Recurse -Force }
if (Test-Path .\HrmsApi\obj) { Remove-Item -Path .\HrmsApi\obj -Recurse -Force }

# Build the project
Write-Host "Building project..."
dotnet build .\HrmsApi\HrmsApi.csproj

# Run the project
Write-Host "Starting HrmsApi..."
dotnet run --project .\HrmsApi\HrmsApi.csproj
