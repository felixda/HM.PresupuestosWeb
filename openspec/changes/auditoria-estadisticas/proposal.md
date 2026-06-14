## Why

La página de Auditorías permite consultar el log de acciones una a una, pero no ofrece ninguna visión agregada. Los administradores no pueden responder preguntas como "¿qué acción se ejecuta más?", "¿quién entra más en la aplicación?" o "¿cuáles son las páginas más visitadas?" sin exportar datos y analizarlos manualmente.

## What Changes

- Las fechas de filtro pasan a ser **obligatorias** con valor por defecto = hoy, evitando consultas sin límite temporal.
- Al pulsar Buscar se cargan simultáneamente el grid de detalle (comportamiento actual) y un nuevo panel de resumen estadístico con métricas agregadas del mismo período y tipo.
- El panel de resumen es colapsable y se sitúa debajo del grid.
- Si el rango de fechas supera 90 días, las estadísticas se muestran sin gráfica de tendencia diaria y con un aviso informativo.
- Se utilizan `DxSparkline` (tipo Bar) para la tendencia temporal y `DxProgressBar` para los rankings top-N, ambos ya incluidos en `DevExpress.Blazor` v24.2.5.

## Capabilities

### New Capabilities

- `auditoria-estadisticas`: Panel de métricas agregadas en la página de Auditorías: total de acciones, usuarios únicos, acción/usuario/página más frecuente, sparkline de actividad diaria y top-5 de usuarios, filtrado por el mismo tipo y rango de fechas que el grid.

### Modified Capabilities

- `auditoria-ver-parametros`: Los filtros de fecha cambian de opcionales a obligatorios con valor por defecto = hoy. Cambio de comportamiento en la validación del formulario.

## Impact

- **Domain**: nueva entidad `EstadisticasAuditoria` con métricas agregadas y colección de puntos temporales.
- **Application**: nuevo método en `ILogAccionesService` / `LogAccionesService` para obtener estadísticas con los mismos parámetros que `ObtenerAuditorias`.
- **Infrastructure**: nueva query de agregación en `LogAccionesRepository` (GROUP BY con TRUNC de fecha Oracle).
- **Web**: modificación de `Auditorias.razor` y `Auditorias.razor.cs` para fechas obligatorias con default, panel colapsable de stats con `DxSparkline` y `DxProgressBar`.
- **Traducciones**: nuevas claves en `TextosApp` y `app.es.json` para los literales del panel de estadísticas.
- Sin nuevas acciones de auditoría (`AccionesLog`).
- Sin nuevos permisos (`CodigosMenu`).

## No incluido / Fuera de alcance

- Dashboard independiente con página propia.
- Exportación de estadísticas a Excel.
- Estadísticas por usuario concreto (drill-down).
- Agrupación dinámica por semana/mes para rangos > 90 días (la sparkline simplemente no se muestra).
- Comparativa entre períodos.
