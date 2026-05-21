param([string]$TestDll)

dotnet test $TestDll --verbosity quiet
$exitCode = $LASTEXITCODE

# dotnet test devuelve 1 cuando hay tests fallidos
# Otros codigos (-1, 2, etc.) son problemas del adaptador, no tests fallidos
if ($exitCode -eq 1) {
    Write-Error "Tests unitarios fallaron. Revisa los resultados antes de continuar."
    exit 1
}

exit 0
