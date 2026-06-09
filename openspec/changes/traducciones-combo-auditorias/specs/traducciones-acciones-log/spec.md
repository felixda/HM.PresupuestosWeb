## ADDED Requirements

### Requirement: Sección AccionesLog en los recursos JSON

Los ficheros de recursos (`app.es.json`, `app.en.json`, `app.pt.json`) SHALL contener una sección `AccionesLog` con una entrada por cada valor del enum `AccionesLog`. Cada entrada SHALL tener los campos `label` (texto traducido) y `visible` (boolean). La clave de cada entrada SHALL ser el nombre exacto del valor del enum (e.g. `"AccesoAPagina"`, `"ImpersonacionUsuario"`).

#### Scenario: Todos los valores del enum tienen entrada en el JSON

- **WHEN** se carga el JSON de cualquier idioma
- **THEN** cada valor del enum `AccionesLog` tiene una entrada en la sección `AccionesLog` con `label` y `visible`

#### Scenario: Valores no visibles no aparecen en el combo

- **WHEN** se llama a `ObtenerAccionesLog()`
- **THEN** los valores con `visible: false` no están en la lista devuelta

#### Scenario: Valores visibles aparecen en el combo con el label del idioma activo

- **WHEN** se llama a `ObtenerAccionesLog()` con idioma `"en"`
- **THEN** los labels devueltos son los del JSON de inglés, no los del enum en español

### Requirement: Método ObtenerAccionesLog en IMapaMenu

`IMapaMenu` SHALL exponer un método `ObtenerAccionesLog()` que devuelva `List<CodigoDescripcion>` con los valores visibles del idioma activo. `Codigo` SHALL ser el entero del enum; `Descripcion` SHALL ser el `label` del JSON. La lista SHALL estar ordenada alfabéticamente por descripción.

#### Scenario: Lista devuelta en el idioma activo

- **WHEN** el idioma activo es `"pt"` y se llama a `ObtenerAccionesLog()`
- **THEN** los labels son los del JSON portugués

#### Scenario: Error leyendo el JSON devuelve lista vacía

- **WHEN** el JSON no puede leerse
- **THEN** se devuelve lista vacía sin lanzar excepción

#### Scenario: Lista devuelta ordenada alfabéticamente

- **WHEN** el JSON contiene entradas en orden arbitrario
- **THEN** la lista devuelta está ordenada por `Descripcion` ascendente

### Tests requeridos

`HM.Presupuestos.UnitTest/Navegacion/MapaMenuTests.cs`:
- `ObtenerAccionesLog_IdiomaActivo_DevuelveLabelsEnIdiomaActivo`
- `ObtenerAccionesLog_EntradaConVisibleFalse_NoAparece`
- `ObtenerAccionesLog_ResultadoOrdenadoAlfabeticamentePorDescripcion`
- `ObtenerAccionesLog_JsonSinSeccionAccionesLog_DevuelveListaVacia`
- `ObtenerPaginasNavegables_JsonConPaginas_DevuelveSoloPaginasConUrl` _(gap change anterior)_
