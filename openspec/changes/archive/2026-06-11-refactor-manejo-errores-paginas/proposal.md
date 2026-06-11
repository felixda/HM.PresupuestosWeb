## Why

Varias páginas Blazor del proyecto conviven con dos patrones de manejo de errores: el moderno (`EjecutarAsync`) y el antiguo (try/catch manual con `LayerOverlayService` y `RegistroAplicacion.RegistrarExcepcion` directos). El patrón antiguo omite el sistema de logging con respaldo (niveles API → archivo → consola) de `ManejarExcepcion`, duplica código y produce inconsistencia de comportamiento en producción. Adicionalmente, `EjecutarAsync<TResult>` en `Context` tiene una llamada duplicada a `RegistrarExcepcion`.

## What Changes

- **Corregir bug**: eliminar la llamada duplicada a `RegistroAplicacion.RegistrarExcepcion` en `EjecutarAsync<TResult>` en `Context.cs`
- **Migrar `ImportacionCondiciones`**: sustituir los try/catch manuales por `EjecutarAsync` y alinear el ciclo de vida al patrón `ContextProtegido` (`InicializarPaginaAsync`, `TituloPagina`, sin `OnAfterRenderAsync` propio)
- **Migrar `Versiones`**: sustituir los try/catch manuales por `EjecutarAsync`
- **Limpiar código comentado**: eliminar bloques `//` de lógica obsoleta en `Sobreprimas.razor.cs` e `ImportacionCondiciones.razor.cs`

## Capabilities

### New Capabilities

- Ninguna — es un refactor puro, no se añaden funcionalidades nuevas

### Modified Capabilities

- Sin cambios en requisitos de negocio ni en especificaciones de comportamiento

## Impact

- **Capas afectadas**: únicamente Web (páginas Blazor y `Context.cs` base)
- **Archivos**:
  - `HM.Presupuestos.Web/Componentes/Base/Context.cs`
  - `HM.Presupuestos.Web/Pages/Condiciones/ImportacionCondiciones.razor.cs`
  - `HM.Presupuestos.Web/Pages/Mantenimientos/Versiones.razor.cs`
  - `HM.Presupuestos.Web/Pages/GestionSobreprimas/Sobreprimas.razor.cs`
- **Requiere nuevas claves de traducción**: No
- **Requiere nuevas acciones de auditoría**: No
- **Requiere nuevos permisos**: No
- **Breaking changes**: Ninguno — comportamiento externo idéntico

## No incluido / Fuera de alcance

- Migración de `PlanificacionCondiciones` (página muy grande; se abordará por separado si se decide)
- Refactor del ciclo de vida de otras páginas más allá de `ImportacionCondiciones`
- Cambios en la lógica de negocio de ninguna de las páginas afectadas
