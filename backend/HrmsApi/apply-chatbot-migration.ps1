Write-Host "Applying Chatbot migration..." -ForegroundColor Green

# Navigate to the project directory
Set-Location $PSScriptRoot

# Apply the migration
dotnet ef database update 20250410_ApplyChatbotTables

Write-Host "Migration completed successfully!" -ForegroundColor Green
