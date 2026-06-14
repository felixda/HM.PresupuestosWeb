## Context

La clase base `Context` provee `EjecutarAsync` como barrera centralizada de overlay + logging + manejo de errores para todas las páginas Blazor. Sin embargo, varias páginas antiguas (`ImportacionCondiciones`, `Versiones`) siguen usando el patrón previo: try/catch manual con llamadas directas a `LayerOverlayService` y `RegistroAplicacion.RegistrarExcepcion`. Esto provoca:

1. **Bypass del sistema de logging con respaldo** — `EjecutarAsync` usa `ManejarExcepcion` que implementa 3 niveles (API → archivo NLog → consola). El patrón antiguo llama directamente a `RegistrarExcepcion(ex)` sin respaldo.
2. **Duplicación de boilerplate** — cada método repite Start/Stop overlay y catch.
3. **Bug en `EjecutarAsync<TResult>`** — `RegistrarExcepcion` se invoca dos veces seguidas.
4. **Ruido de código comentado** — bloques `//` de lógica obsoleta en `Sobreprimas` e `ImportacionCondiciones`.

## Goals / Non-Goals

**Goals:**
- Un único patrón de manejo de errores en todas las páginas Web
- Corregir la doble llamada a `RegistrarExcepcion` en `Context`
- Migrar `ImportacionCondiciones` al ciclo de vida de `ContextProtegido`
- Eliminar código comentado obsoleto

**Non-Goals:**
- Cambiar la lógica de negocio de ninguna página
- Migrar `PlanificacionCondiciones` (fuera de alcance por tamaño)
- Modificar capas Domain, Application o Infrastructure

## Decisions

### D1: Migración try/catch → `EjecutarAsync`

**Decisión**: sustituir cada bloque try/catch manual que sigue el patrón:
```
LayerOverlayService.Start()  →  await EjecutarAsync(async () =>
try { ... }                  →  {
catch(ex) {                  →      // lógica
    RegistrarExcepcion(ex)   →  });
    MostrarMensajeError(...)
}
finally { LayerOverlayService.Stop() }
```

**Alternativa descartada**: crear un nuevo overload de `EjecutarAsync` con mensaje personalizado. No es necesario — el overload con `mensajePersonalizado` ya existe.

### D2: Migración del ciclo de vida de `ImportacionCondiciones`

**Decisión**: eliminar el `OnAfterRenderAsync` propio y la propiedad `PageTitle` (inglés), reemplazándolos por el patrón estándar `ContextProtegido`:
- `InicializarPaginaAsync()` para la carga inicial
- `TituloPagina` (heredado de `Context`) en lugar de `PageTitle`
- `OnPermisoDenegadoAsync()` si aplica

**Alternativa descartada**: mantener `OnAfterRenderAsync` propio. Genera inconsistencia y duplica la gestión del ciclo de vida que ya maneja `ContextProtegido`.

### D3: Corrección bug `EjecutarAsync<TResult>`

**Decisión**: eliminar la segunda llamada duplicada a `RegistrarExcepcion` y unificar el catch con `ManejarExcepcion` igual que el overload `EjecutarAsync(Func<Task>)`.

**Alternativa descartada**: dejar el overload separado. `EjecutarAsync<TResult>` debería tener la misma robustez de logging que `EjecutarAsync`.

## Risks / Trade-offs

- **[Riesgo] Cambio de comportamiento en mensajes de error** — El mensaje mostrado al usuario puede variar levemente si `ImportacionCondiciones` usaba `ErrorService.MostrarErrorInicializandoPagina` con un texto específico. → Mitigación: verificar visualmente el flujo de error en QA antes de merge.
- **[Trade-off] `EjecutarAsync<TResult>` unificado** — Al usar `ManejarExcepcion`, la excepción ya queda registrada antes de retornar `defaultValue`. En el patrón anterior se registraba y se retornaba. Comportamiento equivalente.
- **[Riesgo menor] Código comentado** — Algunos bloques comentados pueden contener lógica de referencia útil. → Mitigación: revisar cada bloque antes de eliminar.
