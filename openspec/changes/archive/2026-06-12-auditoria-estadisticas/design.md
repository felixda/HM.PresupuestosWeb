## Context

La página de Auditorías (`/admin/auditorias`) muestra un grid de registros de `PPT_ACCION_LOG` filtrados por tipo de acción y rango de fechas. Actualmente las fechas son opcionales y no existe ninguna vista agregada de los datos.

La tabla tiene ~80.000 registros. Las queries de agregación (`GROUP BY` + `COUNT`) sobre este volumen son eficientes en Oracle sin índices adicionales.

## Goals / Non-Goals

**Goals:**
- Mostrar métricas agregadas del período filtrado junto al grid existente.
- Hacer las fechas obligatorias con default = hoy para evitar consultas sin límite temporal.
- Implementar todo con `DevExpress.Blazor` ya referenciado (sin nuevas dependencias).

**Non-Goals:**
- Dashboard independiente con página propia.
- Agrupación dinámica por semana/mes para rangos > 90 días.
- Exportación de estadísticas.
- Comparativa entre períodos.

## Decisions

### D1 — Una query adicional por búsqueda, no reutilización del grid

**Elección**: nueva query `ObtenerEstadisticas` en el repositorio con el mismo `WHERE` que `ObtenerAuditorias` pero con `GROUP BY` y `COUNT`.

**Alternativa descartada**: calcular stats en memoria a partir de `List<Auditoria>` ya cargada.

**Rationale**: el grid puede estar paginado o limitado en el futuro. Calcular desde el grid rompería la consistencia si los datos del grid se limitan. Una query de agregación en Oracle es más eficiente que traer todas las filas al servidor .NET.

---

### D2 — Entidad `EstadisticasAuditoria` en Domain

**Elección**: nueva clase en `Domain/Entidades/LogAcciones/EstadisticasAuditoria.cs` con propiedades:
```
int TotalAcciones
int UsuariosUnicos
string UsuarioMasActivo
int UsuarioMasActivoTotal
string PaginaMasVisitada        // solo si tipo == AccesoAPagina
int PaginaMasVisitadaTotal      // solo si tipo == AccesoAPagina
List<PuntoTemporal> ActividadPorDia
List<UsuarioContador> TopUsuarios
```

Con clases auxiliares `PuntoTemporal { DateTime Fecha; int Total }` y `UsuarioContador { string Login; int Total }`.

**Rationale**: mantener el modelo en Domain siguiendo el patrón de `Auditoria.cs`. El servicio y el repositorio trabajan con este tipo.

---

### D3 — Puerto y servicio existente extendidos, no nuevos

**Elección**: añadir `ObtenerEstadisticas(AccionesLog tipo, DateTime fechaInicio, DateTime fechaFin)` a `ILogAccionesService` y su implementación en `LogAccionesService`, y el método correspondiente a `ILogAccionesRepository`.

**Rationale**: `EstadisticasAuditoria` pertenece al mismo módulo de negocio que `Auditoria`. Crear un servicio nuevo solo para stats violaría el principio de cohesión.

**Nota**: las fechas son **obligatorias** en este método (no nullable), al contrario que en `ObtenerAuditorias`.

---

### D4 — Dos queries SQL independientes

La agregación se divide en dos queries para legibilidad y para facilitar el testing:

**Query 1 — Métricas generales + top usuarios:**
```sql
SELECT COD_USUARIO,
       JSON_VALUE(PARAMETROS, '$.Login') AS LOGIN,
       COUNT(*) AS TOTAL
  FROM PPT_ACCION_LOG
 WHERE DES_PROCESO LIKE :Patron
   AND FECHA_INICIO >= :FechaInicio
   AND FECHA_INICIO < :FechaFin
 GROUP BY COD_USUARIO, JSON_VALUE(PARAMETROS, '$.Login')
 ORDER BY TOTAL DESC
 FETCH FIRST 5 ROWS ONLY
```

**Query 2 — Actividad por día (sparkline):**
```sql
SELECT TRUNC(FECHA_INICIO) AS DIA, COUNT(*) AS TOTAL
  FROM PPT_ACCION_LOG
 WHERE DES_PROCESO LIKE :Patron
   AND FECHA_INICIO >= :FechaInicio
   AND FECHA_INICIO < :FechaFin
 GROUP BY TRUNC(FECHA_INICIO)
 ORDER BY DIA
```

`FechaFin` se trata como `FechaFin.Date.AddDays(1)` en el repositorio para incluir todo el día seleccionado.

---

### D5 — Panel colapsable con `DxFormLayoutGroup`

**Elección**: usar `DxFormLayoutGroup` con `ExpandButtonDisplayMode` dentro del `DxFormLayout` existente para el panel de stats.

**Rationale**: consistente con el patrón ya usado para los filtros en la misma página. No requiere componente personalizado.

---

### D6 — Sparkline condicional según rango

**Elección**: si `(FechaFin - FechaInicio).TotalDays > 90`, se oculta el `DxSparkline` y se muestra un `<span>` con texto localizado informativo. Las demás métricas (cards y top-N) se muestran siempre.

**Rationale**: más de 90 puntos en una sparkline compacta resulta ilegible.

---

### D7 — Fechas obligatorias con default hoy

**Elección**: en `InicializarPaginaAsync()`, `FechaInicio = DateTime.Today` y `FechaFin = DateTime.Today`.

La validación en `BuscarAuditoriasAsync` pasa a comprobar también que ambas fechas estén informadas (aunque en la práctica siempre lo estarán por el default).

**Rationale**: evita consultas sin límite temporal y da un resultado útil por defecto ("¿qué ha pasado hoy?").

## Risks / Trade-offs

- **[Riesgo] Rendimiento de queries de agregación sin índice en FECHA_INICIO** → Si el rendimiento es inaceptable en producción, añadir índice `CREATE INDEX IDX_PPT_ACCION_LOG_FECHA ON PPT_ACCION_LOG(FECHA_INICIO)`. No se requiere en el sprint inicial dado el volumen actual.

- **[Trade-off] Dos queries por búsqueda en lugar de una** → Ligero incremento de round-trips a BD. Aceptable dado el volumen y la separación de concerns entre detalle y agregado.

- **[Riesgo] `JSON_VALUE` en Oracle puede devolver NULL si PARAMETROS no tiene campo Login** → El repositorio trata NULL como string vacío y usa `COD_USUARIO` como fallback para mostrar en el top.

- **[Trade-off] Fechas obligatorias cambian comportamiento existente** → La página es nueva y sin usuarios establecidos, por lo que el impacto es nulo.
