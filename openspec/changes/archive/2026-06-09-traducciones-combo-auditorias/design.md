## Context

La página Auditorías carga los combos de tipo de acción y páginas en `InicializarPaginaAsync()`, que se ejecuta una sola vez. El evento `IdiomaCambiado` de `IGestorIdioma` ya llega a `Context.cs` y dispara `StateHasChanged`, pero no recarga los datos — los combos quedan con los valores del idioma inicial. Además, los labels del enum `AccionesLog` están hardcoded en español mediante `[Description]`.

`MapaMenu` ya encapsula la lectura de los JSON por idioma activo y tiene el precedente de `ObtenerPaginasNavegables()`, lo que lo hace el punto natural para añadir `ObtenerAccionesLog()`.

## Goals / Non-Goals

**Goals:**
- Labels de `AccionesLog` traducibles por idioma, sin tocar el enum ni Domain/Application
- Control de visibilidad de acciones en el combo (`visible: true/false` en JSON)
- Recarga automática de combos al cambiar idioma, sin romper páginas existentes
- Regla documentada: cambio en `AccionesLog` → actualizar JSON

**Non-Goals:**
- Migrar los logs ya grabados en BD
- Aplicar el hook a todas las páginas existentes
- Renombrar `IMapaMenu`

## Decisions

### D1 — Lookup por nombre del enum, no por código numérico

**Decisión**: La clave en el JSON es el nombre del valor del enum (e.g. `"AccesoAPagina"`), no el entero (`29`).

**Rationale**: El entero puede reasignarse en el futuro; el nombre del enum es estable y hace el JSON autoexplicativo. El lookup se hace con `tipo.ToString()`.

---

### D2 — `ObtenerAccionesLog()` en `IMapaMenu` / `MapaMenu` (Opción C)

**Decisión**: Añadir el método a la interfaz y clase existente, en lugar de crear un nuevo servicio.

**Rationale**: `MapaMenu` ya tiene `IProveedorRecursosJson`, `IGestorIdioma` y el patrón de iteración del JSON. Reutilizar infraestructura existente minimiza fricción. El nombre `MapaMenu` queda técnicamente impreciso, pero el renombrado se aplaza a un change separado.

---

### D3 — Hook `OnIdiomaActualizadoAsync()` en `Context.cs`

**Decisión**: Convertir `ActualizarIdioma` en un método que llama al hook antes de `StateHasChanged`.

```csharp
// Antes
private async Task ActualizarIdioma() => await InvokeAsync(StateHasChanged);

// Después
private async Task ActualizarIdioma()
{
    await OnIdiomaActualizadoAsync();
    await InvokeAsync(StateHasChanged);
}

protected virtual Task OnIdiomaActualizadoAsync() => Task.CompletedTask;
```

**Rationale**: Las 20+ páginas existentes no se ven afectadas (el noop por defecto es transparente). Solo las páginas con combos dependientes del idioma sobreescriben el hook.

---

### D4 — `visible: true/false` en JSON para filtrar acciones del combo

**Decisión**: 6 valores con `visible: false` (procesos internos y eventos técnicos de sesión):
- `CopiarInversionesFinalizado`, `ImportarCondicionesMMSFinalizado`, `CopiarVersionesFinalizado`
- `RecuperarSesionDespuesDeF5SSO`, `RecuperarSesionDespuesDeF5Impersonacion`
- `ErrorAlEnviarAviso`

Los 23 restantes son `visible: true`.

**Rationale**: Los procesos automáticos y eventos técnicos de sesión no son relevantes para la búsqueda de auditorías por parte del usuario. El campo `visible` en JSON permite ajustar la lista sin tocar código.

## Risks / Trade-offs

- **Desincronía JSON ↔ enum**: Si se añade un valor a `AccionesLog` sin actualizar los JSON, `ObtenerAccionesLog()` no lo mostrará (silencioso). Mitigación: regla en guideline + el método puede loguear una advertencia si detecta un valor del enum sin entrada en el JSON.
- **`MapaMenu` con responsabilidades crecientes**: Cada nuevo método hace el nombre más impreciso. Mitigación: renombrado aplazado a change separado.

## Migration Plan

No requiere migración de datos ni cambios de esquema. Despliegue directo.

## Open Questions

_(ninguna)_
