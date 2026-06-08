## 1. Domain

- [x] 1.1 Añadir `Auditorias = 26` al enum `CodigosMenu`
- [x] 1.2 Crear entidad `Auditoria` en `HM.Presupuestos.Domain/Entidades/LogAcciones/Auditoria.cs` con propiedades `Descripcion` (string), `FechaInicio` (DateTime) y `Usuario` (string)
- [x] 1.3 Añadir método `Task<List<Auditoria>> ObtenerAuditorias(AccionesLog tipo, DateTime? fechaInicio, DateTime? fechaFin)` a `ILogAccionesRepository` en `HM.Presupuestos.Domain/Puertos/Repositorios/ILogAccionesRepository.cs`

## 2. Application

- [x] 2.1 Añadir método `Task<List<Auditoria>> ObtenerAuditorias(AccionesLog tipo, DateTime? fechaInicio, DateTime? fechaFin)` a `ILogAccionesService` en `HM.Presupuestos.Application/CasosDeUso/LogAcciones/ILogAccionesService.cs`
- [x] 2.2 Implementar el método en `LogAccionesService` delegando en `_logAccionesRepository.ObtenerAuditorias(...)`

## 3. Tests unitarios (RED — deben fallar antes de implementar)

- [x] 3.1 Crear `HM.Presupuestos.UnitTest/LogAcciones/LogAccionesServiceTests.cs` con los tests de `ObtenerAuditorias`
- [x] 3.2 Verificar que los tests fallan en rojo (compilación correcta, tests KO)

## 4. Infrastructure

- [x] 4.1 Implementar `ObtenerAuditorias` en `LogAccionesRepository` siguiendo el Patrón 2 de `guidelines/infrastructure/repositories.md`: query con interpolación `$@"..."`, cláusulas de fecha opcionales con ternario, `dr.GetString` / `dr.GetDateTime` para leer columnas
- [x] 4.2 Añadir método privado estático `ExtraerUsuario(string parametrosJson)` que parsea el JSON de PARAMETROS y devuelve `"Nombre Apellido1"` o `"Sin Usuario especificado"` si los datos no están disponibles

## 5. GREEN — pasar los tests a verde

- [x] 5.1 Verificar que todos los tests de `LogAccionesServiceTests` pasan en verde tras la implementación del servicio

## 6. Web — Recursos y menú

- [x] 6.1 Añadir entrada `Menu_26` en `wwwroot/data/app.es.json` con `label: "Auditorías"`, `url: "/admin/auditorias"`, `icono: "fa-solid fa-magnifying-glass-chart"`, `visible: true`, `code: 26`
- [x] 6.2 Añadir entrada `Menu_26` en `wwwroot/data/app.en.json` con `label: "Audits"`
- [x] 6.3 Añadir entrada `Menu_26` en `wwwroot/data/app.pt.json` con `label: "Auditorias"`
- [x] 6.4 Añadir inyección temporal del menú 26 en `SesionUsuario.CrearUsuarioDesdeRespuestaServicioExterno` con comentario `TODO-TEMPORAL` antes de `FiltrarMenusInvalidosAsync`

## 7. Web — Página Auditorias

- [x] 7.1 Crear `HM.Presupuestos.Web/Pages/Admin/Auditorias.razor` con ruta `@page "/admin/auditorias"`, heredando de `ContextProtegido`, con estructura de filtro (`DxFormLayout`) y grid (`DxGrid`)
- [x] 7.2 Crear `HM.Presupuestos.Web/Pages/Admin/Auditorias.razor.cs` con el code-behind: inyección de `ILogAccionesService`, propiedades de binding para el filtro (`TipoAuditoriaSeleccionado`, `FechaInicio`, `FechaFin`, `ResultadoAuditorias`, `TiposAuditoria`), implementación de `InicializarPaginaAsync` (cargar combo) y métodos `BuscarAuditorias` y `LimpiarFiltro`
- [x] 7.3 Implementar validación en `BuscarAuditorias`: si `TipoAuditoriaSeleccionado` es null, mostrar aviso con `MensajesHelper` y no lanzar la consulta
- [x] 7.4 Implementar `LimpiarFiltro`: resetear `TipoAuditoriaSeleccionado = null`, `FechaInicio = null`, `FechaFin = null`, `ResultadoAuditorias = []`

## 8. Validación

- [x] 8.1 Compilar la solución completa sin errores (`dotnet build HM.Presupuestos.sln`)
- [x] 8.2 Ejecutar `/task-validate` para compilación y tests
- [x] 8.3 Ejecutar `/task-architecture-review` para verificar que no se violan las reglas de capas
- [x] 8.4 Ejecutar `/task-code-review` para verificar calidad del código
- [x] 8.5 Ejecutar `/task-frontend-review` para verificar los patrones Blazor y DevExpress

## 9. Commit

- [x] 9.1 Hacer commit con mensaje convencional: `feat(admin): añadir página de auditorías con consulta filtrada por tipo y fechas`
