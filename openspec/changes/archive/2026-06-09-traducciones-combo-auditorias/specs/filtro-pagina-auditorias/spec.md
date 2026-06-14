## MODIFIED Requirements

### Requirement: Filtro de búsqueda de auditorías

La página SHALL mostrar un formulario de filtro con los siguientes campos:
- **Tipo auditoria** (obligatorio): combo cargado con los valores visibles de `AccionesLog` en el **idioma activo**, desde `IMapaMenu.ObtenerAccionesLog()`
- **Página** (opcional, visible solo cuando tipo = AccesoAPagina 29): combo con las páginas navegables del menú según el idioma activo
- **Fecha inicio** (opcional): selector de fecha
- **Fecha fin** (opcional): selector de fecha
- **Botón Buscar**: siempre habilitado
- **Botón Limpiar**: siempre habilitado, situado a la derecha del botón Buscar

#### Scenario: Combo de tipo muestra labels en el idioma activo

- **WHEN** el usuario accede a la página con idioma `"en"`
- **THEN** el combo de tipo muestra los labels en inglés

#### Scenario: Cambio de idioma recarga ambos combos

- **WHEN** el usuario cambia el idioma de la aplicación mientras está en la página Auditorías
- **THEN** el combo de tipo y el combo de página muestran los labels en el nuevo idioma sin necesidad de navegar a otra página

#### Scenario: Buscar sin tipo de auditoría seleccionado

- **WHEN** el usuario pulsa "Buscar" sin haber seleccionado un tipo de auditoría
- **THEN** el sistema muestra un mensaje de aviso indicando que debe seleccionar los campos obligatorios y no lanza ninguna consulta

#### Scenario: Buscar con tipo de auditoría seleccionado (sin filtro de página)

- **WHEN** el usuario selecciona un tipo de auditoría y pulsa "Buscar"
- **THEN** el sistema consulta `PPT_ACCION_LOG` filtrando por `DES_PROCESO LIKE '[N]%'` y muestra los resultados

#### Scenario: Buscar con tipo AccesoAPagina y página seleccionada

- **WHEN** el usuario selecciona AccesoAPagina (29), una página y pulsa "Buscar"
- **THEN** el sistema filtra por tipo Y por `DES_PROCESO LIKE '%[codigoPagina]'`

#### Scenario: Limpiar filtro y resultados

- **WHEN** el usuario pulsa "Limpiar"
- **THEN** el sistema resetea todos los campos y el grid queda sin datos
