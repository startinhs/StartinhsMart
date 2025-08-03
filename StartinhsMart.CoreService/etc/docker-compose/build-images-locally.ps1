param ($version='latest')

$currentFolder = $PSScriptRoot
$slnFolder = Join-Path $currentFolder "../../"

Write-Host "********* BUILDING DbMigrator *********" -ForegroundColor Green
$dbMigratorFolder = Join-Path $slnFolder "src/StartinhsMart.CoreService.DbMigrator"
Set-Location $dbMigratorFolder
dotnet publish -c Release
docker build -f Dockerfile.local -t startinhsmart/coreservice-db-migrator:$version .





Write-Host "********* BUILDING Blazor Web Application *********" -ForegroundColor Green
$blazorWebAppFolder = Join-Path $slnFolder "src/StartinhsMart.CoreService.Blazor"
Set-Location $blazorWebAppFolder
dotnet publish -c Release
docker build -f Dockerfile.local -t startinhsmart/coreservice-blazor:$version .



### ALL COMPLETED
Write-Host "COMPLETED" -ForegroundColor Green
Set-Location $currentFolder