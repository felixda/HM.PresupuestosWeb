## Why

Los combos de la página Auditorías (tipo de acción y página) muestran sus opciones siempre en español, independientemente del idioma activo de la aplicación. Los tipos de acción proceden del atributo `[Description]` del enum `AccionesLog`, que está hardcoded en castellano. Las páginas navegables sí se cargan del JSON correcto, pero no se recargan cuando el usuario cambia de idioma durante la sesión.

## What Changes

- **Nueva sección `AccionesLog` en los JSON de recursos** (`app.es.json`, `app.en.json`, `app.pt.json`) con `label` y `visible` por cada valor del enum. Solo los valores con `visible: true` aparecen en el combo de la página Auditorías.
- **Nuevo método `ObtenerAccionesLog()`** en `IMapaMenu` / `MapaMenu`: devuelve `List<CodigoDescripcion>` con los valores visibles del idioma activo. La clave de lookup es el nombre del valor del enum (`"AccesoAPagina"`, etc.).
- **Nuevo hook virtual `OnIdiomaActualizadoAsync()`** en `Context.cs`: se invoca desde `ActualizarIdioma` antes del `StateHasChanged`. Las páginas con combos dependientes del idioma lo sobreescriben para recargar datos.
- **`Auditorias.razor.cs`** sobreescribe `OnIdiomaActualizadoAsync()` para recargar `TiposAuditoria` y `PaginasNavegables` cuando cambia el idioma.
- **Eliminación del `{0}` del label de `AccesoAPagina`** en el JSON: el label del combo muestra "Acceso a página" sin el placeholder de formato que se usa solo al grabar el log.

## Capabilities

### New Capabilities

- `traducciones-acciones-log`: Labels multiidioma para los valores de `AccionesLog` con control de visibilidad en el combo.

### Modified Capabilities

- `filtro-pagina-auditorias`: Los combos de tipo de acción y página se recargan al cambiar de idioma.

## Impact

- **Web (Adaptadores)**: `IMapaMenu` + `MapaMenu` — nuevo método `ObtenerAccionesLog()`
- **Web (Componentes Base)**: `Context.cs` — nuevo hook `OnIdiomaActualizadoAsync()`
- **Web (Página)**: `Auditorias.razor.cs` — override del nuevo hook
- **Recursos**: `app.es.json`, `app.en.json`, `app.pt.json` — nueva sección `AccionesLog` (29 entradas)
- **Guidelines**: `frontend-patterns` — documentar `OnIdiomaActualizadoAsync`; nueva regla de sincronía `AccionesLog` ↔ JSON
- No requiere cambios en Domain, Application ni Infrastructure
- No requiere nuevas claves de `AppResources.cs` (lookup dinámico por nombre del enum)
- No requiere nuevos permisos (CodigosMenu)
- No requiere nuevas acciones de auditoría (AccionesLog)

## No incluido / Fuera de alcance

- Traducción de los textos grabados en BD (los logs ya grabados quedan en el idioma que se grabaron)
- Aplicar el hook `OnIdiomaActualizadoAsync` a otras páginas existentes que no tengan combos con datos del JSON
- Renombrar `MapaMenu` / `IMapaMenu` a un nombre más representativo
