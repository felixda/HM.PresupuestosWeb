## Context

La página `PlanificacionCondiciones` realiza en `InicializarPaginaAsync` 9 llamadas `await` secuenciales para cargar datos maestros de catálogo (años, networks, alcances, disciplinas, diversifieds, objetivos, tipos de compra, disciplinas grupo, tipos de disciplina). Cada llamada espera el resultado de la anterior antes de iniciar la siguiente, aunque todas son independientes entre sí.

El proyecto ya dispone de `MaestrosCacheService` (patrón Decorator sobre `IMaestrosService`), introducido en el change `2026-06-11-cache-maestros-sesion`, que cachea en memoria los datos maestros cuasi-estáticos durante la sesión del usuario. Sin embargo, solo tiene caché activa sobre 6 métodos; los 7 usados por `PlanificacionCondiciones` (`ObtenerAlcances`, `ObtenerDisciplinas`, `ObtenerDiversifiedsNCB`, `ObtenerObjetivos`, `ObtenerTiposCompra`, `ObtenerDisciplinasGrupos`, `ObtenerTiposDisciplinas`) delegan directamente a `inner` sin caché.

## Goals / Non-Goals

**Goals:**
- Reducir el tiempo de carga inicial de `PlanificacionCondiciones` paralelizando las llamadas independientes.
- Evitar llamadas redundantes a base de datos en visitas posteriores cacheando los 7 métodos maestros que actualmente no tienen caché.
- Mantener la coherencia con el patrón existente (`MaestrosCacheService` como decorator scoped).

**Non-Goals:**
- No se modifica el comportamiento funcional de la página.
- No se cachean métodos con parámetros dinámicos de usuario (p.ej. `ObtenerGruposClientePorNetwork`).
- No se extiende la optimización a otras páginas en este change.

## Decisions

### D1 — Ampliar `MaestrosCacheService` con los 7 métodos faltantes

Los 7 métodos (`ObtenerAlcances`, `ObtenerDisciplinas`, `ObtenerDiversifiedsNCB`, `ObtenerObjetivos`, `ObtenerTiposCompra`, `ObtenerDisciplinasGrupos`, `ObtenerTiposDisciplinas`) devuelven catálogos de configuración que no cambian durante la sesión del usuario. Son candidatos idóneos para la misma estrategia ya aplicada a `ObtenerNetworks` y `ObtenerMedios`.

**Alternativa descartada**: cachear solo en la página (campo privado). Rompe el principio DRY: otras páginas podrían beneficiarse del mismo caché y habría que repetir el patrón. `MaestrosCacheService` es el lugar correcto según el diseño existente.

### D2 — Paralelizar con `Task.WhenAll` en `InicializarPaginaAsync`

Las 9 llamadas a maestros son completamente independientes. Se agrupan en un único `await Task.WhenAll(...)` para que se ejecuten en paralelo y el tiempo total sea el de la llamada más lenta, no la suma de todas.

```
// Antes: secuencial
Anios = await VersionesService.ObtenerAniosConVersiones();
Networks = await PresupuestosService.ObtenerNetworks();
Alcances = await PresupuestosService.ObtenerAlcances();
...

// Después: paralelo
var (anios, networks, alcances, ...) = await (
    VersionesService.ObtenerAniosConVersiones(),
    PresupuestosService.ObtenerNetworks(),
    PresupuestosService.ObtenerAlcances(),
    ...
);
```

**Nota**: En la primera visita (caché fría) el paralelismo es el mayor beneficio. En visitas posteriores (caché caliente) ambas mejoras se combinan y la carga es virtualmente instantánea.

### D3 — Inyectar `IMaestrosCacheService` en lugar de `IMaestrosService`

`PlanificacionCondiciones` actualmente inyecta `IMaestrosService`. Para aprovechar la caché se sustituye por `IMaestrosCacheService`, que extiende `IMaestrosService` y es compatible en uso. El cambio es mínimo y consistente con `ImportacionCondiciones` y `Sobreprimas`.

## Risks / Trade-offs

- **[Riesgo] `Task.WhenAll` y excepciones**: si varias tareas fallan simultáneamente, `Task.WhenAll` lanza la primera excepción y las demás quedan silenciadas en `AggregateException`. Mitigación: `EjecutarAsync` en la capa Web ya actúa como barrera de error y registra la excepción; el comportamiento de usuario no cambia (overlay de error visible).
- **[Trade-off] Caché de alcances/disciplinas**: si un administrador modifica estos catálogos en base de datos, el usuario no verá los cambios hasta que expire el TTL de la caché o navegue fuera y vuelva con una nueva sesión. Es el mismo trade-off aceptado para `Networks` y `Medios` y es coherente con la naturaleza cuasi-estática de estos datos.
