param(
    [string]$WebProject = "HM.Presupuestos.Web/HM.Presupuestos.Web.csproj",
    [string]$E2eProject = "HM.Presupuestos.E2ETest/HM.Presupuestos.E2ETest.csproj",
    [string]$Urls = "http://localhost:8080",
    [int]$TimeoutSeconds = 90
)

$ErrorActionPreference = "Stop"

function Wait-ForHttpReady {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Url,
        [int]$TimeoutSeconds = 90,
        [Parameter(Mandatory = $true)]
        [System.Diagnostics.Process]$Process,
        [string]$StdOutLog,
        [string]$StdErrLog
    )

    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)

    while ((Get-Date) -lt $deadline) {
        if ($Process.HasExited) {
            $stdOutTail = if (Test-Path $StdOutLog) { (Get-Content -Path $StdOutLog -Tail 40) -join [Environment]::NewLine } else { "(sin salida estándar)" }
            $stdErrTail = if (Test-Path $StdErrLog) { (Get-Content -Path $StdErrLog -Tail 40) -join [Environment]::NewLine } else { "(sin salida de error)" }

            throw "La web terminó antes de arrancar (exit code $($Process.ExitCode)).`n--- STDOUT ---`n$stdOutTail`n--- STDERR ---`n$stdErrTail"
        }

        try {
            $response = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 5
            if ($response.StatusCode -ge 200 -and $response.StatusCode -lt 500) {
                return
            }
        }
        catch {
            # Sigue esperando hasta que la web arranque.
        }

        Start-Sleep -Seconds 1
    }

    $stdOutTail = if (Test-Path $StdOutLog) { (Get-Content -Path $StdOutLog -Tail 40) -join [Environment]::NewLine } else { "(sin salida estándar)" }
    $stdErrTail = if (Test-Path $StdErrLog) { (Get-Content -Path $StdErrLog -Tail 40) -join [Environment]::NewLine } else { "(sin salida de error)" }

    throw "La web no respondió en $Url dentro de $TimeoutSeconds segundos.`n--- STDOUT ---`n$stdOutTail`n--- STDERR ---`n$stdErrTail"
}

$webArgs = @(
    "run",
    "--project", $WebProject,
    "--no-build",
    "--urls", $Urls
)

$webProcess = $null
$stdOutLog = Join-Path $env:TEMP "hm-presupuestos-web-stdout.log"
$stdErrLog = Join-Path $env:TEMP "hm-presupuestos-web-stderr.log"

try {
    Write-Host "Iniciando web en $Urls (sin build)..."

    if (Test-Path $stdOutLog) { Remove-Item $stdOutLog -Force }
    if (Test-Path $stdErrLog) { Remove-Item $stdErrLog -Force }

    $webProcess = Start-Process `
        -FilePath "dotnet" `
        -ArgumentList $webArgs `
        -PassThru `
        -WindowStyle Hidden `
        -RedirectStandardOutput $stdOutLog `
        -RedirectStandardError $stdErrLog

    Wait-ForHttpReady -Url $Urls -TimeoutSeconds $TimeoutSeconds -Process $webProcess -StdOutLog $stdOutLog -StdErrLog $stdErrLog

    Write-Host "Web iniciada. Ejecutando E2E..."

    $testArgs = @("test", $E2eProject, "--no-build")

    & dotnet @testArgs
    $exitCode = $LASTEXITCODE

    if ($exitCode -eq 1) {
        Write-Error "Tests E2E fallaron. Revisa los resultados antes de continuar."
        exit 1
    }

    if ($exitCode -ne 0) {
        Write-Error "dotnet test devolvió código $exitCode."
        exit $exitCode
    }

    Write-Host "Tests E2E completados correctamente."
    exit 0
}
finally {
    if ($null -ne $webProcess -and -not $webProcess.HasExited) {
        Write-Host "Deteniendo web (PID $($webProcess.Id))..."
        Stop-Process -Id $webProcess.Id -Force
    }
}
