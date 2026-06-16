<#
.SYNOPSIS
    Cambia los user-secrets de la aplicacion al entorno indicado.

.DESCRIPTION
    Lee los secretos del fichero 'secrets.local.json' (NO está en git) y los carga
    en el store de dotnet user-secrets del proyecto HM.Presupuestos.Web.
    Usa 'secrets.local.template.json' como plantilla para crear tu propio fichero.

.PARAMETER Env
    Entorno destino: DEV | PRU | PRE | PRO

.EXAMPLE
    .\Switch-Env.ps1 -Env DEV
    .\Switch-Env.ps1 -Env PRU
#>
param(
    [Parameter(Mandatory)]
    [ValidateSet("DEV", "PRU", "PRE", "PRO")]
    [string]$Env
)

$project      = "HM.Presupuestos.Web"
$secretsFile  = Join-Path $PSScriptRoot "secrets.local.json"
$templateFile = Join-Path $PSScriptRoot "secrets.local.template.json"

# Verificar que existe el fichero de secretos
if (-not (Test-Path $secretsFile)) {
    Write-Host ""
    Write-Host "ERROR: No se encontro 'secrets.local.json'." -ForegroundColor Red
    Write-Host "Copia la plantilla y rellena los valores reales:" -ForegroundColor Yellow
    Write-Host "  cp secrets.local.template.json secrets.local.json" -ForegroundColor Cyan
    Write-Host ""
    exit 1
}

# Leer y parsear el fichero de secretos
try {
    $allSecrets = Get-Content $secretsFile -Raw -Encoding UTF8 | ConvertFrom-Json
} catch {
    Write-Host "ERROR: El fichero 'secrets.local.json' no es JSON valido: $_" -ForegroundColor Red
    exit 1
}

# Verificar que existe la clave del entorno solicitado
$envSecrets = $allSecrets.$Env
if ($null -eq $envSecrets) {
    Write-Host "ERROR: El entorno '$Env' no existe en secrets.local.json." -ForegroundColor Red
    Write-Host "Entornos disponibles: DEV, PRU, PRE, PRO" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "Limpiando secretos actuales..." -ForegroundColor Yellow
dotnet user-secrets clear --project $project | Out-Null

Write-Host "Configurando secretos para entorno: $Env" -ForegroundColor Cyan

$count = 0
foreach ($prop in $envSecrets.PSObject.Properties) {
    $result = dotnet user-secrets set $prop.Name $prop.Value --project $project 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  ERROR al establecer '$($prop.Name)': $result" -ForegroundColor Red
        exit 1
    }
    $count++
}

Write-Host ""
Write-Host "OK — $count secretos aplicados para el entorno $Env." -ForegroundColor Green
Write-Host ""
Write-Host "Secretos activos:" -ForegroundColor Gray
dotnet user-secrets list --project $project
Write-Host ""
