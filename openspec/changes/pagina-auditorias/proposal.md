## Why

El equipo de administración necesita visibilidad sobre las acciones registradas en la tabla `PPT_ACCION_LOG` para poder auditar operaciones críticas del sistema. Actualmente no existe ninguna interfaz que permita consultar estos registros de forma filtrada.

## What Changes

- Nueva página Blazor `Pages/Admin/Auditorias.razor` con ruta `/admin/auditorias`
- Nueva entidad de dominio `Auditoria` para representar cada registro de auditoría
- Nuevo valor `Auditorias = 26` en el enum `CodigosMenu`
- Nuevo método `ObtenerAuditorias` en `ILogAccionesRepository`, `ILogAccionesService` y sus implementaciones
- Nuevas entradas `Menu_26` en los ficheros de recursos JSON (`app.es.json`, `app.en.json`, `app.pt.json`)

## Capabilities

### New Capabilities

- `consulta-auditorias`: Consulta filtrada de registros de auditoría por tipo (obligatorio), fecha inicio y fecha fin (opcionales), con visualización en grid de descripción, fecha y usuario

### Modified Capabilities

*(ninguna)*

## Impact

- **Domain**: nueva entidad `Auditoria`, nuevo valor en `CodigosMenu`, nuevo método en `ILogAccionesRepository`
- **Application**: nuevo método en `ILogAccionesService` y `LogAccionesService`
- **Infrastructure**: nuevo método en `LogAccionesRepository` (query Oracle con Patrón 2 — interpolación)
- **Web**: nueva página `Auditorias.razor` + `.razor.cs`, nuevas entradas en JSON de recursos
- **Sin breaking changes** — se añaden artefactos nuevos sin modificar los existentes

## No incluido / Fuera de alcance

- Alta del menú 26 en HM.CORE (pendiente del equipo de core; se inyecta temporalmente en código)
- Paginación del grid de resultados
- Exportación de resultados a Excel u otros formatos
- Nuevas acciones de auditoría (`AccionesLog`) — la consulta usa las existentes
