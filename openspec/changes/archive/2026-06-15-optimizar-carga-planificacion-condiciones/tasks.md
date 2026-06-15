## 1. Tests (TDD red)

- [x] 1.1 En `MaestrosCacheServiceTests`: añadir tests que verifican que la segunda llamada a `ObtenerAlcances`, `ObtenerDisciplinas`, `ObtenerDiversifiedsNCB`, `ObtenerObjetivos`, `ObtenerTiposCompra`, `ObtenerDisciplinasGrupos` y `ObtenerTiposDisciplinas` no invoca al inner service (un test por método, siguiendo el patrón de `ObtenerNetworks_SegundaLlamada_NoInvocaInnerService`)

## 2. Application — Ampliar caché de maestros

- [x] 2.1 En `MaestrosCacheService`: añadir claves de caché privadas (`const string`) para los 7 métodos nuevos a cachear (`ClaveAlcances`, `ClaveDisciplinas`, `ClaveDiversifiedsNCB`, `ClaveObjetivos`, `ClaveTiposCompra`, `ClaveDisciplinasGrupos`, `ClaveTiposDisciplinas`)
- [x] 2.2 En `MaestrosCacheService`: sustituir las 7 delegaciones directas a `inner` por llamadas a `ObtenerOCachear` con su clave correspondiente

## 3. Web — Optimizar carga de PlanificacionCondiciones

- [x] 3.1 En `PlanificacionCondiciones.razor.cs`: cambiar la inyección de `IMaestrosService` por `IMaestrosCacheService` para `PresupuestosService`
- [x] 3.2 En `PlanificacionCondiciones.razor.cs`: refactorizar `InicializarPaginaAsync` para ejecutar con `Task.WhenAll` las llamadas independientes a maestros (`ObtenerAniosConVersiones`, `ObtenerNetworks`, `ObtenerAlcances`, `ObtenerDisciplinas`, `ObtenerDiversifiedsNCB`, `ObtenerObjetivos`, `ObtenerTiposCompra`, `ObtenerDisciplinasGrupos`, `ObtenerTiposDisciplinas`)
