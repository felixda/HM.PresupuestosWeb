---
name: action-refactor
description: Refactoriza código, tests o renombra artefactos siguiendo los principios de diseño del proyecto, tras tener los tests en verde. Disparadores: "refactoriza", "renombra", "limpia el código".
argument-hint: "[code | tests | rename | frontend]"
allowed-tools: Read, Write, Edit, Glob, Grep, Bash
---

# Refactor — HM.Presupuestos

Aplica los estándares de diseño y reglas de refactorización del proyecto.

## Uso

```
/action-refactor            → Refactoriza código de producción (por defecto)
/action-refactor code       → Refactoriza código de producción (.cs)
/action-refactor tests      → Refactoriza ficheros de test (NUnit/Moq)
/action-refactor rename     → Renombra el último artefacto creado
/action-refactor frontend   → Refactoriza componentes Blazor contra los patrones del proyecto
```

## Modo: Code (por defecto)

Aplica los principios de diseño de `.github/specs/technical-specs.md` y `.github/copilot-instructions.md`:

- Nomenclatura (nombres pronunciables, concretos, autoexplicativos)
- Diseño de métodos (responsabilidad única, guard clauses, CQS)
- Diseño de clases/servicios (encapsulación, primary constructor C# 12, inyección correcta)

Pasos:
1. Revisar el código contra los principios de diseño del proyecto
2. Aplicar mejoras
3. Ejecutar tests tras el refactor para verificar que nada se rompe:
   ```bash
   dotnet test HM.Presupuestos.UnitTest/HM.Presupuestos.UnitTest.csproj --no-build
   ```

## Modo: Tests

Aplica los estándares de testing de `.github/agents/tests-reviewer.md`:

- Nombres de test orientados al negocio: `Metodo_Contexto_ResultadoEsperado`
- Estructura AAA (Arrange-Act-Assert) con líneas en blanco entre secciones
- Eliminar TODOs pendientes tras la implementación
- Principios FIRST

Pasos:
1. Revisar los tests contra los estándares del proyecto (NUnit + Moq)
2. Aplicar mejoras
3. Ejecutar tests para verificar que nada se rompe:
   ```bash
   dotnet test HM.Presupuestos.UnitTest/HM.Presupuestos.UnitTest.csproj --no-build
   ```

## Modo: Frontend

Aplica los patrones de componentes Blazor de `.github/specs/technical-specs.md` al fichero o módulo actual.

### Ficheros de referencia (leer primero)

Antes de refactorizar, leer la implementación canónica en el módulo de Condiciones:
- **Página con permisos**: `HM.Presupuestos.Web/Pages/Condiciones/`
- **Componente reutilizable**: `HM.Presupuestos.Web/Componentes/`

### Qué refactorizar

#### Componentes Blazor

- `@inject` en `.razor` → `[Inject]` en `.razor.cs`
- `@code { }` en `.razor` → mover lógica al `.razor.cs`
- `OnInitializedAsync` para cargar datos → `InicializarPaginaAsync` o `OnUsuarioDisponibleAsync`
- Herencia de `ComponentBase` → herencia de `ContextProtegido` o `Context`
- Llamadas a servicios sin `EjecutarAsync` → envolver en `EjecutarAsync(async () => { ... })`

#### Nomenclatura en el code-behind

- Campo que se referencia en Razor (`@Variable`, `@bind`) definido como `_camelCase` → cambiar a propiedad `PascalCase { get; set; }`
- Propiedad que solo se usa internamente (sin binding Razor) → cambiar a campo `_camelCase`
- Valor calculado con lógica simple → propiedad calculada `PascalCase` con `=>`
- `@inject` → `[Inject] protected IXxxService XxxService { get; set; } = default!;`

#### Textos y traducciones

- String literal visible en la UI → `ObtenerTexto(TextosApp.Seccion.Clave)`
  - Si la clave no existe, seguir `.github/prompts/anadir-traduccion.prompt.md`

#### Arquitectura

- Inyección de `IXxxRepository` en una página → reemplazar por `IXxxService`
- Lógica de negocio en una página → mover al `XxxService` correspondiente
- Auditoría registrada desde la página → mover al servicio

#### CSS

- Clases CSS en `camelCase` o `PascalCase` → `kebab-case`
- Rutas en mayúsculas → `kebab-case`

### Pasos

1. Leer los ficheros de referencia listados arriba
2. Revisar el código objetivo contra los patrones Blazor del proyecto
3. Aplicar mejoras siguiendo los patrones de referencia
4. Compilar para verificar que nada se rompe:
   ```bash
   dotnet build HM.Presupuestos.Web/HM.Presupuestos.Web.csproj --no-restore
   ```

## Modo: Rename

Renombra el último artefacto creado siguiendo los estándares del proyecto:

- Autoexplicativo — el nombre expresa claramente la intención
- Pronunciable — sin abreviaciones crípticas
- Concreto — sin palabras cajón (`Helper`, `Manager`, `Util`)
- Consistente — un concepto, un nombre en todo el codebase

| Artefacto | Convención |
|-----------|------------|
| Clase de servicio | `XxxService` (sustantivo + Service) |
| Interfaz de servicio | `IXxxService` |
| Interfaz de repositorio | `IXxxRepository` |
| Página Blazor | `NombrePagina.razor` + `NombrePagina.razor.cs` |
| Componente Blazor | `NombreComponente.razor` + `NombreComponente.razor.cs` |
| Campo privado interno | `_camelCase` |
| Propiedad con binding Razor | `PascalCase` |

Pasos:
1. Identificar el artefacto a renombrar
2. Aplicar los estándares de nomenclatura
3. Actualizar todas las referencias (usar renombrado del IDE cuando sea posible)
4. Ejecutar tests para verificar que nada se rompe
