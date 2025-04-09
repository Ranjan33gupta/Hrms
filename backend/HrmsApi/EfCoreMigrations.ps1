# PowerShell script to generate and apply EF Core migrations
param (
    [string]$MigrationName = "InitialCreate",
    [switch]$Apply = $false
)

Write-Host "=== HRMS EF Core Migrations Tool ===" -ForegroundColor Cyan

# Ensure the EF Core tools are installed
try {
    dotnet tool list --global | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Error checking for dotnet tools. Make sure .NET SDK is installed." -ForegroundColor Red
        exit 1
    }
    
    $efInstalled = dotnet tool list --global | Select-String "dotnet-ef"
    if (-not $efInstalled) {
        Write-Host "Installing Entity Framework Core tools..." -ForegroundColor Yellow
        dotnet tool install --global dotnet-ef
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Failed to install EF Core tools." -ForegroundColor Red
            exit 1
        }
    }
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
    exit 1
}

# Generate the migration
try {
    Write-Host "Generating migration: $MigrationName" -ForegroundColor Green
    dotnet ef migrations add $MigrationName --project HrmsApi.csproj --context HrmsDbContext
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to generate migration." -ForegroundColor Red
        exit 1
    }
    
    Write-Host "Migration generated successfully!" -ForegroundColor Green
    
    # Apply the migration if requested
    if ($Apply) {
        Write-Host "Applying migration to database..." -ForegroundColor Yellow
        dotnet ef database update --project HrmsApi.csproj --context HrmsDbContext
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Failed to apply migration." -ForegroundColor Red
            exit 1
        }
        
        Write-Host "Migration applied successfully!" -ForegroundColor Green
    } else {
        Write-Host "To apply this migration, run: .\EfCoreMigrations.ps1 -Apply" -ForegroundColor Cyan
    }
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
    exit 1
}
