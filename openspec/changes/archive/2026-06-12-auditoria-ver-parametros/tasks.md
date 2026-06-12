## 1. Domain

- [x] 1.1 Añadir propiedad `Parametros` (string) a `Auditoria.cs`

## 2. Infrastructure

- [x] 2.1 Asignar `Parametros` al mapear la fila en `LogAccionesRepository.ObtenerAuditorias`

## 3. Traducciones

- [x] 3.1 Añadir clave `TextosApp.Pages.Auditorias.Parametros` (caption columna grid)
- [x] 3.2 Añadir clave `TextosApp.Pages.Auditorias.TituloPopupParametros` (título del popup)

## 4. Web — UI

- [x] 4.1 Añadir columna con icono en el grid de `Auditorias.razor` usando `CellDisplayTemplate`
- [x] 4.2 Añadir `DxPopup` en `Auditorias.razor` con el contenido de `Parametros` en `<pre>`
- [x] 4.3 Añadir en `Auditorias.razor.cs` los campos `_auditoriaSeleccionada` y `_popupParametrosVisible` y el método para abrir el popup
- [x] 4.4 Añadir estilos del popup (altura máxima, scroll) en `Auditorias.razor.css`
