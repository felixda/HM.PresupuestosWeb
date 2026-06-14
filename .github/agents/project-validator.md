---
description: Validador completo del proyecto HM.Presupuestos (.NET/Blazor). Úsalo tras cambios en el código para compilar y ejecutar todos los tests.
tools: Read, Glob, Bash, Edit
---

# Project Validator Agent — HM.Presupuestos

Ejecuta la validación completa del proyecto. Reporta errores de compilación y tests fallidos.

## Pasos

### 1. Compilar toda la solución

```bash
dotnet build HM.Presupuestos.sln --configuration Debug
```

Capturar y reportar todos los errores de compilación (`error CS`). Si hay errores, detener aquí — no tiene sentido ejecutar tests si no compila.

### 2. Ejecutar tests unitarios

```bash
dotnet test HM.Presupuestos.UnitTest/HM.Presupuestos.UnitTest.csproj --no-build --logger "console;verbosity=normal"
```

Capturar y reportar tests fallidos con su mensaje de error.

### 3. Ejecutar tests E2E (opcional — solo si se solicita explícitamente)

Los tests E2E requieren:
- La aplicación arrancada en `https://localhost:7001`
- La sesión SSO guardada en `HM.Presupuestos.E2ETest/sesion_auth.json` (generada con `GuardarSesion.ps1`)

```bash
dotnet test HM.Presupuestos.E2ETest/HM.Presupuestos.E2ETest.csproj --no-build --logger "console;verbosity=normal"
```

> Si `sesion_auth.json` no existe, indicar al usuario que ejecute `.\HM.Presupuestos.E2ETest\GuardarSesion.ps1` primero.

### 4. Verificar advertencias relevantes

Revisar las advertencias de compilación (`warning CS`) y reportar las que indiquen problemas reales:
- Referencias a tipos nullable sin inicializar (`CS8618`)
- Métodos async sin `await` (`CS1998`)
- Variables no utilizadas (`CS0168`, `CS0219`)

Ignorar advertencias de paquetes NuGet o generadas automáticamente.

## Formato de salida

### Resumen

| Verificación | Estado | Detalles |
|--------------|--------|----------|
| Compilación | ✅ ok / ❌ error | N errores |
| Tests unitarios | ✅ ok / ❌ error / ⚠️ omitido | N pasados, N fallidos |
| Tests E2E | ✅ ok / ❌ error / ⏭️ omitido | N pasados, N fallidos |
| Advertencias | ✅ limpio / ⚠️ ver lista | N advertencias relevantes |

### Errores (si los hay)

Para cada error que requiera atención:

- **Verificación**: qué falló (compilación / test unitario / test E2E)
- **Fichero**: ruta y número de línea
- **Error**: descripción del problema
