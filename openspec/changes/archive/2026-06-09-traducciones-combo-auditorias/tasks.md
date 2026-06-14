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

- [x] 8.1 Hacer commit con mensaje: `feat(i18n): traducciones multiidioma para combo de tipos de auditoría con recarga al cambiar idioma`
