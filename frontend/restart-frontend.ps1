Write-Host "Restarting WorkNest Frontend..." -ForegroundColor Green

# Kill any existing node processes
try {
    Get-Process -Name "node" -ErrorAction SilentlyContinue | Stop-Process -Force
    Write-Host "Stopped existing node processes" -ForegroundColor Yellow
} catch {
    Write-Host "No existing node processes found" -ForegroundColor Yellow
}

# Clean Vite cache
Write-Host "Cleaning Vite cache..." -ForegroundColor Yellow
if (Test-Path -Path "node_modules\.vite") {
    Remove-Item -Recurse -Force "node_modules\.vite" -ErrorAction SilentlyContinue
}

# Start the frontend server
Write-Host "Starting frontend server..." -ForegroundColor Green
Start-Process -FilePath "powershell" -ArgumentList "-ExecutionPolicy Bypass -Command `"cd '$PSScriptRoot'; npm run dev`""

# Open browser
Write-Host "Opening browser..." -ForegroundColor Green
Start-Sleep -Seconds 3
Start-Process "http://localhost:5173"

Write-Host "Frontend restarted successfully!" -ForegroundColor Green
