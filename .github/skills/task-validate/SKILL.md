---
name: task-validate
description: Compila la solución y ejecuta todos los tests unitarios. Disparadores: "valida el proyecto", "compila y testea", "ejecuta los tests", "verifica que compila", "task validate".
argument-hint: "[--e2e]"
context: fork
agent: project-validator
allowed-tools: Read, Glob, Bash, Edit
---

# Validate Project

Lanza un subagente para compilar la solución completa y ejecutar los tests.

## Scope

Siempre valida la solución completa:
```bash
dotnet build HM.Presupuestos.sln --configuration Debug
dotnet test HM.Presupuestos.UnitTest/HM.Presupuestos.UnitTest.csproj --no-build
```

Si se proporciona el argumento `--e2e`, ejecutar también los tests E2E:
```bash
dotnet test HM.Presupuestos.E2ETest/HM.Presupuestos.E2ETest.csproj --no-build
```
> Requiere la aplicación arrancada en `https://localhost:7001` y `sesion_auth.json`.

## What the Agent Does

1. Compilar la solución; si hay errores `error CS`, detenerse y reportar
2. Ejecutar tests unitarios y reportar los fallidos con su mensaje de error
3. Ejecutar tests E2E si se solicitó con `--e2e`
4. Revisar advertencias relevantes: `CS8618` (nullable), `CS1998` (async sin await), `CS0168`/`CS0219` (variables no usadas)

## Output

| Verificación | Estado | Detalles |
|---|---|---|
| Compilación | ✅ ok / ❌ error | N errores |
| Tests unitarios | ✅ ok / ❌ error | N pasados, N fallidos |
| Tests E2E | ✅ ok / ❌ error / ⏭️ omitido | N pasados, N fallidos |
| Advertencias | ✅ limpio / ⚠️ ver lista | N advertencias relevantes |
