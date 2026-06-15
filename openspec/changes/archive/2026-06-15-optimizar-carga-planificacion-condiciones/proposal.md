## Why

La carga inicial de la página `PlanificacionCondiciones` es lenta porque realiza 9 llamadas secuenciales a base de datos para obtener datos maestros de catálogo (networks, alcances, disciplinas, diversifieds, objetivos, tipos de compra, disciplinas grupo, tipos de disciplina, años con versiones). Al ser llamadas independientes y sobre datos cuasi-estáticos, el coste se puede reducir drásticamente con paralelismo y caché de sesión.

## What Changes

- **`MaestrosCacheService`**: añadir caché a los 7 métodos que actualmente delegan directamente a `inner` sin cachear (`ObtenerAlcances`, `ObtenerDisciplinas`, `ObtenerDiversifiedsNCB`, `ObtenerObjetivos`, `ObtenerTiposCompra`, `ObtenerDisciplinasGrupos`, `ObtenerTiposDisciplinas`).
- **`PlanificacionCondiciones.razor.cs`**: sustituir la inyección de `IMaestrosService` por `IMaestrosCacheService` y paralelizar las llamadas a maestros en `InicializarPaginaAsync` mediante `Task.WhenAll`.

## Capabilities

### New Capabilities

_Ninguna. No se introduce comportamiento nuevo observable por el usuario._

### Modified Capabilities

_Ninguna. No cambian requisitos ni contratos de ninguna spec existente; se trata de una optimización interna de rendimiento._

## Impact

- **Application**: `MaestrosCacheService.cs` — 7 métodos pasan de delegar directamente a cachearse.
- **Web**: `PlanificacionCondiciones.razor.cs` — cambio de inyectable y paralelización de carga.
- Sin cambios en Domain, Infrastructure ni contratos de interfaces.
- No requiere nuevas claves de traducción (`TextosApp`).
- No requiere nuevas acciones de auditoría (`AccionesLog`).
- No requiere nuevos permisos (`CodigosMenu`).

## No incluido / Fuera de alcance

- No se cachean métodos con parámetros variables de usuario (p.ej. `ObtenerGruposClientePorNetwork`) ya que su resultado depende de entrada dinámica y el beneficio es marginal.
- No se toca la lógica de negocio de `InicializarPaginaAsync` más allá del paralelismo de la carga de maestros.
- No se modifican otras páginas que pudieran beneficiarse de los nuevos métodos cacheados (se puede hacer en un cambio posterior).
