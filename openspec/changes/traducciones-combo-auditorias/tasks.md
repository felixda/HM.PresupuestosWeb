## 1. Recursos JSON

- [x] 1.1 Añadir sección `AccionesLog` en `app.es.json` con los 29 valores del enum: `label` en español y `visible` (6 con `false`: `CopiarInversionesFinalizado`, `ImportarCondicionesMMSFinalizado`, `CopiarVersionesFinalizado`, `RecuperarSesionDespuesDeF5SSO`, `RecuperarSesionDespuesDeF5Impersonacion`, `ErrorAlEnviarAviso`)
- [x] 1.2 Añadir sección `AccionesLog` en `app.en.json` con los 29 valores del enum: `label` en inglés y `visible` (mismos criterios)
- [x] 1.3 Añadir sección `AccionesLog` en `app.pt.json` con los 29 valores del enum: `label` en portugués y `visible` (mismos criterios)

## 2. Web — Adaptadores

- [x] 2.1 Añadir método `List<CodigoDescripcion> ObtenerAccionesLog()` a la interfaz `IMapaMenu`
- [x] 2.2 Implementar `ObtenerAccionesLog()` en `MapaMenu`: iterar la sección `AccionesLog` del JSON del idioma activo, devolver solo los que tengan `visible: true`, con `Codigo = (int)Enum.Parse<AccionesLog>(key)` y `Descripcion = label`, ordenados por descripción

## 3. Web — Componentes Base

- [x] 3.1 Convertir `ActualizarIdioma()` en `Context.cs` para que llame a `OnIdiomaActualizadoAsync()` antes de `StateHasChanged`
- [x] 3.2 Añadir `protected virtual Task OnIdiomaActualizadoAsync() => Task.CompletedTask;` en `Context.cs`

## 4. Web — Página Auditorías

- [x] 4.1 Cambiar la carga de `TiposAuditoria` en `InicializarPaginaAsync` para usar `MapaMenu.ObtenerAccionesLog()` en lugar de `Enum.GetValues<AccionesLog>()`
- [x] 4.2 Eliminar el using `HM.Presupuestos.Domain.Extensiones` en `Auditorias.razor.cs` (ya no se usa `ObtenerDescripcion()`)
- [x] 4.3 Sobreescribir `OnIdiomaActualizadoAsync()` en `Auditorias.razor.cs` para recargar `TiposAuditoria = MapaMenu.ObtenerAccionesLog()` y `PaginasNavegables = MapaMenu.ObtenerPaginasNavegables()`

## 5. Guidelines

- [x] 5.1 Documentar en `frontend-patterns/SKILL.md` el hook `OnIdiomaActualizadoAsync`: cuándo sobreescribirlo y ejemplo de uso
- [x] 5.2 Añadir en `frontend-patterns/SKILL.md` la regla: "Al añadir, modificar o eliminar un valor en `AccionesLog`, actualizar obligatoriamente la sección `AccionesLog` en `app.es.json`, `app.en.json` y `app.pt.json`"

## 6. Tests unitarios

> El proyecto `HM.Presupuestos.UnitTest` no puede referenciar `HM.Presupuestos.Web` porque Web ya referencia UnitTest (dependencia circular). Se creó un nuevo proyecto `HM.Presupuestos.Web.UnitTest` para los tests de la capa Web. El hook `OnIdiomaActualizadoAsync` en `Context.cs` (componente Blazor) no se testea con NUnit puro — queda fuera del alcance de este change.

- [x] 6.1 Crear proyecto `HM.Presupuestos.Web.UnitTest` referenciando `HM.Presupuestos.Web`, añadido a la solución
- [x] 6.2 Crear `HM.Presupuestos.Web.UnitTest/Navegacion/MapaMenuTests.cs` con 5 tests (`ObtenerAccionesLog` x4 + `ObtenerPaginasNavegables` x1)
- [x] 6.3 Tests marcados con `[Category("MapaMenu")]`

## 7. Validación

- [x] 7.1 Compilar la solución completa sin errores (`dotnet build HM.Presupuestos.sln`) — 0 errores
- [x] 7.2 Ejecutar todos los tests unitarios y verificar que pasan en verde — 22/22 (16 UnitTest + 6 Web.UnitTest)

## 8. Commit

- [ ] 8.1 Hacer commit con mensaje: `feat(i18n): traducciones multiidioma para combo de tipos de auditoría con recarga al cambiar idioma`
- [ ] 1.2 Añadir sección `AccionesLog` en `app.en.json` con los 29 valores del enum: `label` en inglés y `visible` (mismos criterios)
- [ ] 1.3 Añadir sección `AccionesLog` en `app.pt.json` con los 29 valores del enum: `label` en portugués y `visible` (mismos criterios)

## 2. Web — Adaptadores

- [ ] 2.1 Añadir método `List<CodigoDescripcion> ObtenerAccionesLog()` a la interfaz `IMapaMenu`
- [ ] 2.2 Implementar `ObtenerAccionesLog()` en `MapaMenu`: iterar la sección `AccionesLog` del JSON del idioma activo, devolver solo los que tengan `visible: true`, con `Codigo = (int)Enum.Parse<AccionesLog>(key)` y `Descripcion = label`, ordenados por descripción

## 3. Web — Componentes Base

- [ ] 3.1 Convertir `ActualizarIdioma()` en `Context.cs` para que llame a `OnIdiomaActualizadoAsync()` antes de `StateHasChanged`
- [ ] 3.2 Añadir `protected virtual Task OnIdiomaActualizadoAsync() => Task.CompletedTask;` en `Context.cs`

## 4. Web — Página Auditorías

- [ ] 4.1 Cambiar la carga de `TiposAuditoria` en `InicializarPaginaAsync` para usar `MapaMenu.ObtenerAccionesLog()` en lugar de `Enum.GetValues<AccionesLog>()`
- [ ] 4.2 Eliminar el using `HM.Presupuestos.Domain.Extensiones` en `Auditorias.razor.cs` si ya no se usa `ObtenerDescripcion()`
- [ ] 4.3 Sobreescribir `OnIdiomaActualizadoAsync()` en `Auditorias.razor.cs` para recargar `TiposAuditoria = MapaMenu.ObtenerAccionesLog()` y `PaginasNavegables = MapaMenu.ObtenerPaginasNavegables()`

## 5. Guidelines

- [ ] 5.1 Documentar en `frontend-patterns/SKILL.md` el hook `OnIdiomaActualizadoAsync`: cuándo sobreescribirlo y ejemplo de uso
- [ ] 5.2 Añadir en `frontend-patterns/SKILL.md` (o en una guideline apropiada) la regla: "Al añadir, modificar o eliminar un valor en `AccionesLog`, actualizar obligatoriamente la sección `AccionesLog` en `app.es.json`, `app.en.json` y `app.pt.json`"

## 6. Tests unitarios

> El proyecto `HM.Presupuestos.UnitTest` no referencia `HM.Presupuestos.Web`. Para testear `MapaMenu` hay que añadir esa referencia. El hook `OnIdiomaActualizadoAsync` en `Context.cs` (componente Blazor) no se testea con NUnit puro — queda fuera del alcance de este change.

- [ ] 6.1 Añadir `<ProjectReference Include="..\HM.Presupuestos.Web\HM.Presupuestos.Web.csproj" />` en `HM.Presupuestos.UnitTest.csproj`
- [ ] 6.2 Crear `HM.Presupuestos.UnitTest/Navegacion/MapaMenuTests.cs` con:
  - `ObtenerAccionesLog_IdiomaActivo_DevuelveLabelsEnIdiomaActivo`: mock del JSON con 3 entradas visibles en inglés → `Descripcion` = label inglés
  - `ObtenerAccionesLog_EntradaConVisibleFalse_NoAparece`: mock con 1 visible=true y 1 visible=false → solo devuelve 1
  - `ObtenerAccionesLog_ResultadoOrdenadoAlfabeticamentePorDescripcion`: 3 entradas desordenadas → devueltas en orden A-Z
  - `ObtenerAccionesLog_JsonSinSeccionAccionesLog_DevuelveListaVacia`: JSON sin sección `AccionesLog` → lista vacía sin excepción
  - `ObtenerPaginasNavegables_JsonConPaginas_DevuelveSoloPaginasConUrl`: retro-añadido (gap del change anterior)
- [ ] 6.3 Marcar los tests como `[Category("MapaMenu")]`

## 7. Validación

- [ ] 7.1 Compilar la solución completa sin errores (`dotnet build HM.Presupuestos.sln`)
- [ ] 7.2 Ejecutar todos los tests unitarios y verificar que pasan en verde

## 8. Commit

- [ ] 8.1 Hacer commit con mensaje: `feat(i18n): traducciones multiidioma para combo de tipos de auditoría con recarga al cambiar idioma`
