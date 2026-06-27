# GuardarSesion.ps1
$outputPath = "$PSScriptRoot\bin\Debug\net10.0\sesion_auth.json"
Write-Host 'Abriendo navegador para login SSO...' -ForegroundColor Cyan
Write-Host 'Haz login y cierra el inspector cuando termines.' -ForegroundColor Yellow
$playwrightScript = "$PSScriptRoot\bin\Debug\net10.0\playwright.ps1"
if (-not (Test-Path $playwrightScript)) { Write-Host 'Primero ejecuta: dotnet build' -ForegroundColor Red; exit 1 }
$url = 'http://localhost:8080'
& powershell -ExecutionPolicy Bypass $playwrightScript codegen --save-storage=$outputPath $url
Write-Host 'Sesion guardada en:' $outputPath -ForegroundColor Green