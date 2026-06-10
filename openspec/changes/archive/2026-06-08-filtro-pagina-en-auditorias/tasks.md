## 1. Domain

- [x] 1.1 Añadir `int? codigoPagina = null` como parámetro opcional al método `ObtenerAuditorias` en `ILogAccionesRepository`

## 2. Application

- [x] 2.1 Añadir `int? codigoPagina = null` como parámetro opcional al método `ObtenerAuditorias` en `ILogAccionesService`
- [x] 2.2 Actualizar la implementación en `LogAccionesService` para delegar el nuevo parámetro al repositorio

## 3. Tests unitarios (RED — deben fallar antes de implementar)

- [x] 3.1 Añadir tests en `LogAccionesServiceTests` para `ObtenerAuditorias` con `codigoPagina` informado y verificar que delega correctamente al repositorio

## 4. Infrastructure

- [x] 4.1 Añadir la cláusula `AND DES_PROCESO LIKE :PatronPagina` (opcional) en `LogAccionesRepository.ObtenerAuditorias` cuando `codigoPagina` no es null, con el patrón `%[{codigoPagina}]`
- [x] 4.2 Mejorar `QuitarPrefijoAccion` para eliminar también `(Method) -> ` buscando el separador `-> ` en el texto (para todos los tipos)

## 5. GREEN — pasar los tests a verde

- [x] 5.1 Verificar que todos los tests de `LogAccionesServiceTests` pasan en verde tras los cambios

## 6. Web — Adaptadores

- [x] 6.1 Corregir el formato del string en `RegistroAplicacion.RegistrarAccesoAPagina`: cambiar de `(RegistrarAccesoAPagina) -> [29] texto [url] [codigoMenu]` a `[29](RegistrarAccesoAPagina) -> texto [url] [codigoMenu]`
- [x] 6.2 Añadir método `List<CodigoDescripcion> ObtenerPaginasNavegables()` a la interfaz `IMapaMenu`
- [x] 6.3 Implementar `ObtenerPaginasNavegables()` en `MapaMenu`: iterar el JSON de menú del idioma activo, devolver solo las entradas con `url` no vacía, con `Codigo = code` y `Descripcion = label`, ordenadas por descripción

## 7. Web — Recursos

- [x] 7.1 Añadir clave `Pagina` en la sección `Pages.Auditorias` de `app.es.json`, `app.en.json` y `app.pt.json`
- [x] 7.2 Actualizar `TextosApp.cs` con la nueva clave `Pages.Auditorias.Pagina`

## 8. Web — Página Auditorías

- [x] 8.1 Añadir propiedad `PaginaSeleccionada { get; set; }` (int?) y `PaginasNavegables { get; set; }` (List<CodigoDescripcion>) en `Auditorias.razor.cs`
- [x] 8.2 Inyectar `IMapaMenu` en `Auditorias.razor.cs` y cargar `PaginasNavegables` en `InicializarPaginaAsync`
- [x] 8.3 Resetear `PaginaSeleccionada = null` en `LimpiarFiltroAsync`
- [x] 8.4 Pasar `PaginaSeleccionada` como argumento `codigoPagina` en la llamada a `LogAccionesService.ObtenerAuditorias` dentro de `BuscarAuditoriasAsync`
- [x] 8.5 Añadir en `Auditorias.razor` un `DxFormLayoutItem` con `DxComboBox` para "Página", visible condicionalmente con `@if (TipoAuditoriaSeleccionado == (int)AccionesLog.AccesoAPagina)`
- [x] 8.6 Resetear `PaginaSeleccionada = null` cuando el tipo seleccionado cambia a uno distinto de AccesoAPagina

## 9. Validación

- [x] 9.1 Compilar la solución completa sin errores (`dotnet build HM.Presupuestos.sln`)
- [x] 9.2 Ejecutar todos los tests unitarios y verificar que pasan en verde

## 10. Commit

- [x] 10.1 Hacer commit con mensaje: `feat(admin): añadir filtro por página en auditorías de tipo AccesoAPagina`
