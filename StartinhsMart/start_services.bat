@echo off
echo Starting StartinhsMart Services...
echo.

echo Starting Prediction Service...
cd StartinhsMart.PredictionService
start "Prediction Service" python server.py

echo.
echo Starting Core Service...
cd ..\StartinhsMart.CoreService
start "Core Service" dotnet run --project src\StartinhsMart.CoreService.Blazor

echo.
echo Services are starting...
echo Prediction Service: http://127.0.0.1:8080
echo Core Service: http://localhost:44300
echo.
echo Press any key to exit...
pause 