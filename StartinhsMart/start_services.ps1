Write-Host "Starting StartinhsMart Services..." -ForegroundColor Green
Write-Host ""

Write-Host "Starting Prediction Service..." -ForegroundColor Yellow
Set-Location "StartinhsMart.PredictionService"
Start-Process -FilePath "python" -ArgumentList "server.py" -WindowStyle Normal

Write-Host ""
Write-Host "Starting Core Service..." -ForegroundColor Yellow
Set-Location "..\StartinhsMart.CoreService"
Start-Process -FilePath "dotnet" -ArgumentList "run", "--project", "src\StartinhsMart.CoreService.Blazor" -WindowStyle Normal

Write-Host ""
Write-Host "Services are starting..." -ForegroundColor Green
Write-Host "Prediction Service: http://127.0.0.1:8080" -ForegroundColor Cyan
Write-Host "Core Service: http://localhost:44300" -ForegroundColor Cyan
Write-Host ""
Write-Host "Press any key to exit..." -ForegroundColor Gray
Read-Host 