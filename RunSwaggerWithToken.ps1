param(
    [string]$ApiProject = "HM.Presupuestos.Api/HM.Presupuestos.Api.csproj",
    [string]$SwaggerBaseUrl = "http://localhost:5078",
    [string]$VerifyEndpoint = "api/v1/maestros/tipologias",
    [int]$TokenHours = 8,
    [string]$OutputPath = "HM.Presupuestos.Api/bin/Debug/net10.0/swagger-dev.jwt",
    [int]$StartupTimeoutSeconds = 30,
    [switch]$NoBrowser,
    [switch]$NoStartApi
)

$ErrorActionPreference = "Stop"

function Write-Step {
    param([string]$Message)
    Write-Host "[SwaggerToken] $Message" -ForegroundColor Cyan
}

function To-Base64Url {
    param([byte[]]$Bytes)

    return [Convert]::ToBase64String($Bytes).TrimEnd('=') -replace '\+', '-' -replace '/', '_'
}

function Get-JwtSigningKey {
    param([string]$Project)

    # Misma precedencia que la API: Auth:SigningKey, fallback a Jwt:Clave.
    $raw = dotnet user-secrets list --project $Project

    $authMatch = $raw | Select-String '^Auth:SigningKey\s*=\s*(.+)$'
    if ($authMatch) {
        return $authMatch.Matches[0].Groups[1].Value.Trim()
    }

    $jwtMatch = $raw | Select-String '^Jwt:Clave\s*=\s*(.+)$'
    if ($jwtMatch) {
        # Si Auth:SigningKey no esta en secrets, intentamos resolverla desde appsettings
        # para evitar desalineacion cuando Auth existe solo en config de archivo.
        $projectPath = Resolve-Path $Project
        $projectDir = Split-Path -Parent $projectPath
        $appsettingsPath = Join-Path $projectDir "appsettings.json"

        if (Test-Path $appsettingsPath) {
            try {
                $cfg = Get-Content -Path $appsettingsPath -Raw | ConvertFrom-Json
                if ($cfg.Auth -and $cfg.Auth.SigningKey -and -not [string]::IsNullOrWhiteSpace($cfg.Auth.SigningKey)) {
                    return [string]$cfg.Auth.SigningKey
                }
            }
            catch {
                # Si no se puede parsear appsettings, usamos Jwt:Clave como fallback final.
            }
        }

        return $jwtMatch.Matches[0].Groups[1].Value.Trim()
    }

    throw "No se encontro una clave JWT valida. Define Auth:SigningKey o Jwt:Clave (en secrets o appsettings)."
}

function New-DevJwt {
    param(
        [string]$SigningKey,
        [int]$Hours
    )

    $headerJson = '{"alg":"HS256","typ":"JWT"}'
    $now = [DateTimeOffset]::UtcNow

    $payloadObject = [ordered]@{
        sub = "swagger-dev-user"
        unique_name = "swagger.dev"
        name = "Swagger Dev"
        CodigoUsuario = 1
        CodigoAplicacion = 1
        CodigoPais = 34
        Login = "swagger.dev"
        Nombre = "Swagger"
        Apellido1 = "Dev"
        Companias = "1"
        iat = $now.ToUnixTimeSeconds()
        exp = $now.AddHours($Hours).ToUnixTimeSeconds()
    }

    $payloadJson = $payloadObject | ConvertTo-Json -Compress

    $headerB64 = To-Base64Url([Text.Encoding]::UTF8.GetBytes($headerJson))
    $payloadB64 = To-Base64Url([Text.Encoding]::UTF8.GetBytes($payloadJson))
    $unsignedToken = "$headerB64.$payloadB64"

    $hmac = [System.Security.Cryptography.HMACSHA256]::new([Text.Encoding]::UTF8.GetBytes($SigningKey))
    $signature = To-Base64Url($hmac.ComputeHash([Text.Encoding]::UTF8.GetBytes($unsignedToken)))

    return "$unsignedToken.$signature"
}

function Test-SwaggerAvailable {
    param([string]$BaseUrl)

    try {
        $null = Invoke-WebRequest -UseBasicParsing "$BaseUrl/swagger/v1/swagger.json" -TimeoutSec 2
        return $true
    }
    catch {
        return $false
    }
}

function Start-ApiIfNeeded {
    param(
        [string]$Project,
        [string]$BaseUrl,
        [int]$TimeoutSeconds,
        [switch]$SkipStart
    )

    if (Test-SwaggerAvailable -BaseUrl $BaseUrl) {
        Write-Step "La API ya esta activa en $BaseUrl"
        return $null
    }

    if ($SkipStart) {
        throw "La API no esta activa y se solicito -NoStartApi. Arrancala manualmente y vuelve a ejecutar el script."
    }

    Write-Step "Arrancando API en $BaseUrl ..."
    $apiProcess = Start-Process dotnet -ArgumentList "run --project $Project --no-build --urls $BaseUrl" -PassThru

    $deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSeconds)
    while ([DateTime]::UtcNow -lt $deadline) {
        if (Test-SwaggerAvailable -BaseUrl $BaseUrl) {
            Write-Step "API arrancada correctamente"
            return $apiProcess
        }
        Start-Sleep -Milliseconds 500
    }

    throw "La API no estuvo disponible en $BaseUrl dentro de $TimeoutSeconds segundos"
}

Write-Step "Obteniendo Jwt:Clave desde user-secrets"
$signingKey = Get-JwtSigningKey -Project $ApiProject

Write-Step "Generando JWT DEV"
$token = New-DevJwt -SigningKey $signingKey -Hours $TokenHours

$fullOutputPath = Join-Path (Get-Location) $OutputPath
$tokenDir = Split-Path -Parent $fullOutputPath
if (-not (Test-Path $tokenDir)) {
    New-Item -Path $tokenDir -ItemType Directory -Force | Out-Null
}

Set-Content -Path $fullOutputPath -Value $token -Encoding ASCII
Write-Step "Token guardado en $OutputPath"

Set-Clipboard -Value $token
Write-Step "Token copiado al portapapeles"

$apiProcess = Start-ApiIfNeeded -Project $ApiProject -BaseUrl $SwaggerBaseUrl -TimeoutSeconds $StartupTimeoutSeconds -SkipStart:$NoStartApi

$swaggerUrl = "$SwaggerBaseUrl/swagger"
if (-not $NoBrowser) {
    Start-Process $swaggerUrl | Out-Null
    Write-Step "Swagger abierto en $swaggerUrl"
}
else {
    Write-Step "Swagger disponible en $swaggerUrl"
}

Write-Host "" 
Write-Host "Usa este valor en Authorize (pega solo el token, sin 'Bearer '):" -ForegroundColor Yellow
Write-Host "<token-copiado-en-portapapeles>" -ForegroundColor Yellow
Write-Host "" 

if ($apiProcess -ne $null) {
    Write-Host "PID API iniciado por el script: $($apiProcess.Id)" -ForegroundColor DarkYellow
}

function Test-AuthWithGeneratedToken {
    param(
        [string]$BaseUrl,
        [string]$Endpoint,
        [string]$Token
    )

    $normalizedBase = $BaseUrl.TrimEnd('/')
    $normalizedEndpoint = $Endpoint.TrimStart('/')
    $url = "$normalizedBase/$normalizedEndpoint"

    try {
        $response = Invoke-WebRequest -UseBasicParsing -Method Get -Uri $url -Headers @{ Authorization = "Bearer $Token" } -TimeoutSec 10
        Write-Step "Verificacion auth OK ($($response.StatusCode)) en $url"
        return $true
    }
    catch {
        $statusCode = $null
        $reason = $null

        if ($_.Exception.Response) {
            try {
                $statusCode = [int]$_.Exception.Response.StatusCode
                $reason = $_.Exception.Response.ReasonPhrase
            }
            catch {
                # Ignorar errores de lectura de respuesta.
            }
        }

        if ($statusCode) {
            Write-Host "[SwaggerToken] Verificacion auth KO ($statusCode $reason) en $url" -ForegroundColor Red
        }
        else {
            Write-Host ("[SwaggerToken] Verificacion auth KO en {0}: {1}" -f $url, $_.Exception.Message) -ForegroundColor Red
        }

        return $false
    }
}

$null = Test-AuthWithGeneratedToken -BaseUrl $SwaggerBaseUrl -Endpoint $VerifyEndpoint -Token $token