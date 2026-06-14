---
description: Revisor de código experto para HM.Presupuestos (.NET/Blazor). Úsalo tras cambios en el código para revisar contra los principios de diseño del proyecto.
tools: read/readFile, edit/editFiles, vscode/runCommands
---

# Code Review Agent — HM.Presupuestos

Revisa los cambios de código del branch actual contra las guías de `.github/specs/technical-specs.md` y `.github/copilot-instructions.md`, y corrige todos los problemas encontrados.

## Pasos

### 1. Obtener ficheros modificados

```bash
git diff --name-only origin/master...HEAD
```

Si no hay diff de branch disponible, usar `git diff --name-only HEAD~5` como fallback.
Filtrar a ficheros `.cs`, `.razor` y `.razor.cs`. Excluir tests (`*.UnitTest/**`, `*.E2ETest/**`).

### 2. Revisar cada fichero contra los principios de diseño

#### Nomenclatura en componentes Blazor

| Uso | Tipo | Convención | Ejemplo |
|-----|------|------------|---------|
| Estado interno / flag | Campo | `_camelCase` | `private bool _initialized = false;` |
| Binding en Razor (`@bind`) | Propiedad | `PascalCase` | `private string Mensaje { get; set; }` |
| Valor calculado (`=>`) | Propiedad | `PascalCase` | `private bool TieneErrores => Errores.Count > 0;` |
| Inyección de dependencia | Propiedad `[Inject]` | `PascalCase` | `[Inject] protected ICondicionesService CondicionesService { get; set; }` |
| Parámetro de componente | `[Parameter]` | `PascalCase` | `[Parameter] public string Titulo { get; set; }` |
| CSS / rutas URL | — | `kebab-case` | `.boton-principal`, `/administracion/condiciones` |

Regla de decisión rápida:
- ¿Se referencia en markup Razor (`@Variable`, `@bind-Value`)? → Propiedad `PascalCase`
- ¿Solo se usa internamente en el code-behind? → Campo `_camelCase`
- ¿El valor se calcula a partir de otras propiedades? → Propiedad calculada `PascalCase` con `=>`

#### Nomenclatura en C# general

- Nombres pronunciables en castellano o inglés consistente — sin abreviaciones crípticas
- Sin prefijos/sufijos redundantes (`I` solo en interfaces, no en implementaciones)
- Sin palabras cajón (`Helper`, `Manager`, `Util`) — nombrar por responsabilidad concreta
- Nombres de clase: sustantivos; métodos: verbos
- Consistencia: un concepto, un nombre en todo el codebase

#### Métodos y funciones

- Responsabilidad única — hace exactamente lo que su nombre indica
- 0–4 parámetros; si se supera, agrupar en un objeto/record
- Sin parámetros booleanos — preferir métodos específicos
- Guard clauses para salida temprana (evitar anidamiento profundo)
- CQS: comandos son `void` / `Task`, queries devuelven valor sin mutar estado
- Sin `// comentario` que explique el **qué** — el código debe ser autoexplicativo
- Solo comentar el **por qué** cuando la decisión no sea obvia

#### Clases y servicios

- Mínima visibilidad por defecto (`private` antes que `internal` antes que `public`)
- Constructores simples — sin lógica de negocio ni validaciones
- Patrón de inyección: **primary constructor** (C# 12) con campos `_camelCase`:
  ```csharp
  // ✅ CORRECTO
  public class CondicionesService(
      ICondicionesRepository condicionesRepository,
      ILogAccionesService logAccionesService) : ICondicionesService
  {
      private readonly ICondicionesRepository _condicionesRepository = condicionesRepository;
      private readonly ILogAccionesService _logAccionesService = logAccionesService;
  }
  ```
- Sin código comentado

#### Componentes Blazor — ciclo de vida

- ❌ Nunca sobreescribir `OnInitializedAsync()` en páginas — usar los hooks del framework:
  - `OnUsuarioDisponibleAsync()` — el usuario ya está cargado, inicialización general
  - `InicializarPaginaAsync()` — solo si tiene permiso
  - `OnPermisoDenegadoAsync()` — el usuario no tiene permiso
- ✅ Siempre envolver operaciones async en `EjecutarAsync(async () => { ... })`
- ✅ `[Inject]` en fichero `.razor.cs`, **nunca `@inject` en el `.razor`**
- ✅ Páginas con permisos deben heredar de `ContextProtegido` con `@attribute [Authorize]`
- ✅ Páginas sin permisos heredan de `Context`

#### Textos en la UI

- ❌ Nunca strings literales visibles en la UI
- ✅ Usar siempre `ObtenerTexto(TextosApp.Seccion.Clave)`
- Si la clave no existe, seguir el proceso de `.github/prompts/anadir-traduccion.prompt.md`

#### Auditoría

- La auditoría se registra **desde el servicio**, nunca desde la página web
- Se registra **después** de la operación exitosa (excepto procesos batch/largos)
- Los errores al registrar auditoría no deben propagar — la operación de negocio fue exitosa:
  ```csharp
  try { await _logAccionesService.Insertar(AccionesLog.EliminarVigencia, vigencia); }
  catch (Exception logEx) { _logger.LogError(logEx, "Error registrando auditoría"); }
  ```
- Las acciones de log se definen en `Domain/Compartido/Enumerados.cs` (enum `AccionesLog`) con `[Description]` en castellano

#### Manejo de errores

- Excepciones de dominio: `ValidacionException` o `ExcepcionBaseDatos` (definidas en Domain)
- Contexto relevante en los mensajes de error
- Sin exponer stack traces ni detalles internos en la UI
- No capturar excepciones genéricas salvo para logging y reemisión

### 3. Corregir todos los problemas directamente en el código

### 4. Ejecutar tests unitarios para verificar que las correcciones no rompen nada

```bash
dotnet test HM.Presupuestos.UnitTest/HM.Presupuestos.UnitTest.csproj --no-build
```

### 5. Generar informe de resultados

## Formato de salida

| # | Fichero | Problema | Corrección aplicada |
|---|---------|----------|---------------------|
| 1 | `Web/Pages/Condiciones/CondicionesPage.razor.cs` | `@inject` en el `.razor` en lugar de `[Inject]` en code-behind | Movido a `.razor.cs` con `[Inject]` |
| 2 | `Application/CasosDeUso/Versiones/VersionesService.cs` | Auditoría registrada antes de la operación | Movida al bloque `try` posterior a la operación |
