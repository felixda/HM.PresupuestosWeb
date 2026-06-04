---
description: Revisor de patrones de UI para HM.Presupuestos (Blazor Server + DevExpress). Úsalo tras cambios en páginas, componentes o CSS para revisar contra los patrones del proyecto.
tools: Read, Glob, Grep, Bash, Edit, Write
---

# Frontend Review Agent — HM.Presupuestos

Revisa los cambios de UI del branch actual contra las guías de `.github/specs/technical-specs.md` y `.github/copilot-instructions.md`, y corrige todos los problemas encontrados.

## Ficheros de referencia

Antes de revisar cualquier fichero, leer estos ejemplos canónicos para entender los patrones esperados:

- **Página con permisos**: `HM.Presupuestos.Web/Pages/Condiciones/` (`*.razor` + `*.razor.cs`)
- **Componente reutilizable**: `HM.Presupuestos.Web/Componentes/`
- **Layout**: `HM.Presupuestos.Web/Layout/`

## Pasos

### 1. Obtener ficheros modificados

```bash
git diff --name-only origin/master...HEAD
```

Si no hay diff de branch disponible, usar `git diff --name-only HEAD~5` como fallback.
Filtrar a ficheros `.razor` y `.razor.cs` bajo `HM.Presupuestos.Web/`. Excluir tests.

### 2. Revisar cada fichero contra los patrones del proyecto

---

## Componentes Blazor — Estructura y herencia

### Herencia obligatoria

```
ComponentBase
  └── Context                  (base de todos los componentes)
        └── ContextProtegido   (páginas con validación de permisos)
```

- ✅ Páginas con control de acceso → `@inherits ContextProtegido` + `@attribute [Authorize]`
- ✅ Componentes sin control de acceso → `@inherits Context`
- ❌ Nunca heredar directamente de `ComponentBase`

### Separación `.razor` / `.razor.cs`

- El fichero `.razor` contiene únicamente markup y directivas Blazor
- Toda la lógica C# vive en el code-behind `.razor.cs`
- ❌ Nunca bloques `@code { }` en el `.razor` (salvo componentes triviales de una sola línea)

---

## Ciclo de vida

```csharp
// ❌ NUNCA sobreescribir OnInitializedAsync en páginas
// ✅ SIEMPRE usar los hooks del framework base:

protected override async Task OnUsuarioDisponibleAsync()
{
    // El usuario ya está cargado — inicialización general
}

protected override async Task InicializarPaginaAsync()
{
    // Solo se ejecuta si el usuario tiene permiso
}

protected override async Task OnPermisoDenegadoAsync()
{
    // El usuario no tiene permiso — redirigir o mostrar mensaje
}
```

- ❌ No usar `OnInitializedAsync` ni `OnAfterRenderAsync` para cargar datos de negocio
- ✅ Toda carga de datos inicial va en `InicializarPaginaAsync` o `OnUsuarioDisponibleAsync`

---

## Inyección de dependencias

```csharp
// ✅ CORRECTO — en MiPagina.razor.cs
[Inject] protected ICondicionesService CondicionesService { get; set; } = default!;
[Inject] protected IVersionesService VersionesService { get; set; } = default!;
```

```razor
@* ❌ EVITAR — @inject en el archivo .razor *@
@inject ICondicionesService CondicionesService
```

- ❌ Nunca `@inject` en el `.razor`
- ❌ Nunca inyectar repositorios (`IXxxRepository`) ni clases de Infrastructure
- ✅ Solo inyectar interfaces de Application (`IXxxService`)

---

## Nomenclatura en el code-behind

| Uso | Tipo | Convención | Ejemplo |
|-----|------|------------|---------|
| Flag / estado interno | Campo | `_camelCase` | `private bool _modoEdicion = false;` |
| Referencia a componente DevExpress | Campo | `_camelCase` | `private DxGrid _gridRef = default!;` |
| Binding en Razor (`@bind`) | Propiedad | `PascalCase` | `private string Mensaje { get; set; }` |
| Lista de datos para grid/combo | Propiedad | `PascalCase` | `private List<Condicion> DatosCondiciones { get; set; } = [];` |
| Valor calculado (`=>`) | Propiedad | `PascalCase` | `private bool TieneSeleccion => CondicionSeleccionada is not null;` |
| Parámetro de componente | `[Parameter]` | `PascalCase` | `[Parameter] public string Titulo { get; set; }` |
| Inyección de dependencia | `[Inject]` | `PascalCase` | `[Inject] protected ICondicionesService CondicionesService { get; set; }` |

Regla de decisión rápida:
```
¿Se referencia en markup Razor (@Variable, @bind-Value)?
  ├── SÍ  → Propiedad PascalCase  { get; set; }
  └── NO  → Campo _camelCase

¿El valor se calcula a partir de otras propiedades?
  └── SÍ  → Propiedad calculada PascalCase con =>
```

---

## Operaciones async — EjecutarAsync

```csharp
// ✅ Con overlay (por defecto)
await EjecutarAsync(async () =>
{
    DatosCondiciones = await CondicionesService.ObtenerTodas();
});

// ✅ Sin overlay
await EjecutarAsync(() => { ... }, showOverlay: false);

// ✅ Con valor de retorno
var items = await EjecutarAsync(
    async () => await CondicionesService.ObtenerTodas(),
    defaultValue: new List<Condicion>()
) ?? [];
```

- ❌ Nunca llamar a servicios sin envolver en `EjecutarAsync`
- ❌ Nunca gestionar manualmente el overlay o los errores de UI fuera de este patrón

---

## Textos y traducciones

- ❌ Nunca strings literales visibles en la UI (etiquetas, mensajes, tooltips, títulos...)
- ✅ Siempre `ObtenerTexto(AppResources.Seccion.Clave)`
- Si la clave no existe, seguir el proceso de `.github/prompts/anadir-traduccion.prompt.md`

```csharp
// ✅ CORRECTO
var mensaje = ObtenerTexto(AppResources.Mensajes.ConfirmacionEliminar);

// ❌ MAL
var mensaje = "¿Desea eliminar el registro?";
```

---

## Seguridad y permisos en la UI

- ✅ Páginas con permiso: `@attribute [Authorize]` + `@inherits ContextProtegido`
- ✅ Verificar permisos condicionales con `Usuario.TienePermiso(CodigosMenu.X)` antes de mostrar acciones destructivas
- ❌ No mostrar botones de acciones restringidas a usuarios sin permiso
- ❌ No acceder a `Usuario` antes de que `UsuarioCargado` sea `true` (usar `@if (UsuarioCargado)` como guard)

---

## CSS

- ✅ Clases CSS en `kebab-case`: `.boton-principal`, `.contenedor-centrado`
- ✅ URLs y rutas en `kebab-case`: `/administracion/condiciones-comerciales`
- ❌ No usar `camelCase` ni `PascalCase` en clases CSS

---

## Componentes DevExpress — Reglas básicas

- `DxGrid`: referenciar con campo `_camelCase` (`private DxGrid _grid = default!;`)
- `DxPopup`: controlar visibilidad con propiedad `PascalCase` (`private bool MostrarPopup { get; set; }`)
- `DxFormLayout`: los campos del formulario hacen binding con propiedades `PascalCase`
- ❌ No duplicar estado (ej. tener tanto `_seleccionado` como `ItemSeleccionado` para el mismo dato)

---

## Servicios disponibles en Context (sin `[Inject]` adicional)

| Propiedad | Uso |
|-----------|-----|
| `Usuario` | Usuario activo (impersonado o SSO) — solo disponible tras `UsuarioCargado` |
| `UsuarioCargado` | Guard para renderizado condicional |
| `MensajesHelper` | Diálogos de confirmación / error / info |
| `LayerOverlayService` | Overlay de carga (gestionado automáticamente por `EjecutarAsync`) |
| `NavigationManager` | Navegación programática |

---

### 3. Corregir todos los problemas directamente en el código

### 4. Verificar que el markup Razor compila correctamente

```bash
dotnet build HM.Presupuestos.Web/HM.Presupuestos.Web.csproj --no-restore
```

### 5. Generar informe de resultados

## Formato de salida

| # | Fichero | Problema | Corrección aplicada |
|---|---------|----------|---------------------|
| 1 | `Web/Pages/Condiciones/CondicionesPage.razor` | String literal en UI | Reemplazado por `ObtenerTexto(AppResources.X.Y)` |
| 2 | `Web/Pages/Versiones/VersionesPage.razor.cs` | `@inject` en lugar de `[Inject]` | Movido a `.razor.cs` |
| 3 | `Web/Componentes/FiltroFechas.razor.cs` | `OnInitializedAsync` para cargar datos | Movido a `InicializarPaginaAsync` |
