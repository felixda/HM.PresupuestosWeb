## Why

La página de Auditorías muestra el grid con Descripción, Fecha y Usuario, pero el campo `Parametros` — que contiene el JSON completo de la operación auditada — no es visible para el administrador. Actualmente, para depurar o investigar una acción concreta, no hay forma de consultar los parámetros desde la UI.

## What Changes

- Se añade una nueva columna en el grid de Auditorías con un icono que, al pulsarlo, abre un `DxPopup` con el contenido del campo `Parametros` formateado como JSON.
- La entidad `Auditoria` se amplía con la propiedad `Parametros`.
- El repositorio `LogAccionesRepository` mapea el campo `PARAMETROS` ya seleccionado en la query al nuevo campo de la entidad.

## Capabilities

### New Capabilities

- `auditoria-ver-parametros`: Visualización del campo Parametros de una entrada de auditoría mediante popup en la página de Auditorías.

### Modified Capabilities

*(ninguna — no cambia ningún requisito existente)*

## Impact

- **Domain**: `Auditoria.cs` — nueva propiedad `Parametros`
- **Infrastructure**: `LogAccionesRepository.cs` — mapeo del campo ya seleccionado en la query
- **Web**: `Auditorias.razor` + `Auditorias.razor.cs` — nueva columna con icono y `DxPopup`
- **Traducciones**: requiere nueva clave para el caption de la columna (`TextosApp.Pages.Auditorias.Parametros`) y el título del popup
- **Auditoría**: no requiere nuevas acciones de log
- **Permisos**: no requiere nuevos permisos (`CodigosMenu`)

## No incluido / Fuera de alcance

- Formateo o visualización especial del JSON (coloreado de sintaxis, árbol expandible)
- Filtrado por contenido de parámetros
- Exportación del campo `Parametros`
- El campo `RESULTADO_STR` (no se muestra en este cambio)
