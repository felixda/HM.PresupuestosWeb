# .NET Version Upgrade Plan

## Overview

**Target**: Upgrade toda la solucion HM.Presupuestos de .NET 8 a .NET 10.0 (LTS)
**Scope**: 6 proyectos - 3 librerias de clase, 1 aplicacion ASP.NET Core (Blazor Server), 2 proyectos de test

### Selected Strategy
**All-At-Once** - Todos los proyectos actualizados simultaneamente en una sola operacion atomica.
**Rationale**: 6 proyectos, todos en .NET 8, estructura de dependencias clara.

## Tasks

### 01-prerequisites: Validar SDK y configuracion previa

Verificar que el SDK de .NET 10 esta instalado y que no hay conflictos en global.json.

**Done when**: SDK net10.0 confirmado instalado; global.json (si existe) compatible con .NET 10.

---

### 02-update-tfm: Actualizar TargetFramework en todos los proyectos

Cambiar TargetFramework de net8.0 a net10.0 en los seis .csproj: Domain, Application, Infrastructure, Web, UnitTest, E2ETest.

**Done when**: Los seis proyectos referencian net10.0; dotnet restore sin errores.

---

### 03-update-packages: Actualizar paquetes NuGet

Actualizar paquetes desactualizados en Application, Web y E2ETest a versiones compatibles con net10.0.

**Done when**: Todos los paquetes actualizados; dotnet restore limpio; sin vulnerabilidades pendientes.

---

### 04-fix-breaking-changes: Corregir APIs incompatibles y cambios de comportamiento

Resolver Api.0001 (binario incompatible) y Api.0002 (fuente incompatible) en Web, y Api.0003 en Infrastructure y Web. Compilar y corregir todos los errores en un unico pase.

**Done when**: dotnet build completa con 0 errores en todos los proyectos.

---

### 05-run-tests: Validar con tests

Ejecutar UnitTest y E2ETest. Corregir fallos causados por cambios de comportamiento del runtime.

**Done when**: Todos los tests pasan.