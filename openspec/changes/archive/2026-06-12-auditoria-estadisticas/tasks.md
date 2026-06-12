## 1. Domain

- [x] 1.1 Crear clase auxiliar `PuntoTemporal` en `Domain/Entidades/LogAcciones/` con propiedades `Fecha` (DateTime) y `Total` (int)
- [x] 1.2 Crear clase auxiliar `UsuarioContador` en `Domain/Entidades/LogAcciones/` con propiedades `Login` (string) y `Total` (int)
- [x] 1.3 Crear entidad `EstadisticasAuditoria` en `Domain/Entidades/LogAcciones/` con propiedades: `TotalAcciones`, `UsuariosUnicos`, `UsuarioMasActivo`, `UsuarioMasActivoTotal`, `PaginaMasVisitada`, `PaginaMasVisitadaTotal`, `ActividadPorDia` (List<PuntoTemporal>), `TopUsuarios` (List<UsuarioContador>)

## 2. Puerto y Application

- [x] 2.1 Añadir método `ObtenerEstadisticas(AccionesLog tipo, DateTime fechaInicio, DateTime fechaFin, int? codigoPagina = null)` a `ILogAccionesRepository` en `Domain/Puertos/`
- [x] 2.2 Añadir método `ObtenerEstadisticas(AccionesLog tipo, DateTime fechaInicio, DateTime fechaFin, int? codigoPagina = null)` a `ILogAccionesService` en `Application/CasosDeUso/LogAcciones/`
- [x] 2.3 Implementar `ObtenerEstadisticas` en `LogAccionesService` delegando en el repositorio
- [x] 2.4 Escribir tests unitarios para `ObtenerEstadisticas` en `LogAccionesService` (delega correctamente, maneja lista vacía)

## 3. Infrastructure

- [x] 3.1 Implementar `ObtenerEstadisticas` en `LogAccionesRepository` con query de top usuarios + `JSON_VALUE` para Login
- [x] 3.2 Implementar query de actividad por día con `TRUNC(FECHA_INICIO)` y `GROUP BY`
- [x] 3.3 Tratar `FechaFin` como `FechaFin.Date.AddDays(1)` para incluir el día completo
- [x] 3.4 Usar Login vacío como fallback cuando `JSON_VALUE` devuelve NULL (mostrar `COD_USUARIO`)

## 4. Traduciones

- [x] 4.1 Añadir claves en `TextosApp.Pages.Auditorias`: `ResumenPeriodo`, `TotalAcciones`, `UsuariosUnicos`, `UsuarioMasActivo`, `PaginaMasVisitada`, `TopUsuarios`, `ActividadDiaria`, `RangoDemasiadoAmplio`, `FechasObligatorias`
- [x] 4.2 Añadir traducciones correspondientes en `wwwroot/data/app.es.json`

## 5. Web — Filtros obligatorios

- [x] 5.1 Inicializar `FechaInicio = DateTime.Today` y `FechaFin = DateTime.Today` en `InicializarPaginaAsync()` de `Auditorias.razor.cs`
- [x] 5.2 Añadir validación de fechas obligatorias en `BuscarAuditoriasAsync` mostrando aviso si alguna es null
- [x] 5.3 Marcar visualmente ambos campos de fecha como obligatorios en `Auditorias.razor` (añadir `<span class="mandatory"></span>`)

## 6. Web — Panel de estadísticas

- [x] 6.1 Añadir propiedad `EstadisticasAuditoria? _estadisticas` en `Auditorias.razor.cs`
- [x] 6.2 Llamar a `LogAccionesService.ObtenerEstadisticas` en `BuscarAuditoriasAsync` en paralelo con `ObtenerAuditorias` (usando `Task.WhenAll`)
- [x] 6.3 Añadir `DxFormLayoutGroup` colapsable en `Auditorias.razor` para el panel de resumen, con los 4 cards de métricas generales
- [x] 6.4 Añadir lista de top-5 usuarios con `DxProgressBar` por fila dentro del panel
- [x] 6.5 Añadir `DxSparkline` de tipo Bar con datos de `ActividadPorDia`, visible solo cuando el rango ≤ 90 días
- [x] 6.6 Añadir texto informativo localizado cuando el rango supera 90 días (en lugar de la sparkline)
- [x] 6.7 Ocultar la métrica `PaginaMasVisitada` cuando el tipo seleccionado no es `AccesoAPagina`
- [x] 6.8 Añadir estilos CSS isolation en `Auditorias.razor.css` para los cards, la lista top y la sparkline
