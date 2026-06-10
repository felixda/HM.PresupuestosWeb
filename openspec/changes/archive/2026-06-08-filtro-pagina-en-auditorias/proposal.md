## Why

La página de Auditorías permite filtrar por tipo de acción, pero cuando se selecciona el tipo "Acceso a página" (código 29) no hay forma de acotar la búsqueda a una página concreta. Además, el formato con el que se graba este tipo de log en BD tiene el prefijo `(RegistrarAccesoAPagina) -> [29]` que impide que la query de filtrado lo encuentre correctamente.

## What Changes

- **Corrección del formato de grabación** del log de acceso a página: el prefijo `[29]` pasa a encabezar el texto, y el código de menú `[codigoPagina]` se añade al final — alineando el registro con el patrón de los demás tipos.
- **Nuevo combo "Página"** en el filtro de la página Auditorías, visible únicamente cuando el tipo seleccionado es `AccesoAPagina` (29). Se rellena con las entradas del JSON de menú del idioma activo que tienen URL no vacía.
- **Nuevo parámetro opcional `codigoPagina`** en la query de auditorías: si se proporciona, añade la condición `DES_PROCESO LIKE '%[codigoPagina]'`.
- **Mejora de `QuitarPrefijoAccion`** en el repositorio para que elimine también el nombre del método llamador `(Method) -> ` además del prefijo `[N]`, para todos los tipos.

## Capabilities

### New Capabilities

- `filtro-pagina-auditorias`: Filtrado de auditorías de tipo "Acceso a página" por página concreta, con carga dinámica de páginas desde el JSON de menú según el idioma activo.

### Modified Capabilities

- `consulta-auditorias`: Cambio en el contrato de `ObtenerAuditorias` — nuevo parámetro opcional `codigoPagina` y cambio en el patrón de búsqueda en BD.

## Impact

- **Domain**: `ILogAccionesRepository` — firma de `ObtenerAuditorias` con nuevo parámetro
- **Application**: `ILogAccionesService` + `LogAccionesService` — nuevo parámetro delegado
- **Infrastructure**: `LogAccionesRepository` — nueva cláusula AND opcional + `QuitarPrefijoAccion` mejorado
- **Web (Adaptadores)**: `RegistroAplicacion` — nuevo formato de string para AccesoAPagina; `IMapaMenu` + `MapaMenu` — nuevo método `ObtenerPaginasNavegables()`
- **Web (Página)**: `Auditorias.razor` + `Auditorias.razor.cs` — nuevo combo condicional + nueva propiedad + carga en `InicializarPaginaAsync`
- **Tests unitarios**: nuevos casos para `ObtenerAuditorias` con `codigoPagina`
- Requiere nuevas claves de traducción en TextosApp (`Pages.Auditorias.Pagina`)
- No requiere nuevas acciones de auditoría (AccionesLog)
- No requiere nuevos permisos (CodigosMenu)

## No incluido / Fuera de alcance

- Migración / reformateo de registros históricos ya grabados en BD con el formato antiguo
- Cambio de formato de grabación para otros tipos de acción distintos a AccesoAPagina (29)
- Paginación o exportación de resultados del grid
