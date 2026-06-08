## ADDED Requirements

### Requirement: Combo "Página" condicional en el filtro de Auditorías

La página Auditorías SHALL mostrar un campo adicional "Página" en el panel de filtros únicamente cuando el tipo de auditoría seleccionado sea `AccesoAPagina` (código 29). El combo SHALL cargarse con la lista de páginas navegables del JSON de menú del idioma activo, incluyendo solo las entradas que tengan el campo `url` no vacío. Cada opción mostrará el `label` del menú como descripción y usará el `code` como valor.

#### Scenario: Combo Página oculto cuando no es tipo AccesoAPagina

- **WHEN** el usuario selecciona un tipo de auditoría distinto de AccesoAPagina (29) o no ha seleccionado ningún tipo
- **THEN** el combo "Página" no es visible en el panel de filtros

#### Scenario: Combo Página visible cuando se selecciona AccesoAPagina

- **WHEN** el usuario selecciona el tipo de auditoría AccesoAPagina (29)
- **THEN** el combo "Página" aparece en el panel de filtros con la lista de páginas navegables del idioma activo

#### Scenario: Combo Página vacío al seleccionar otro tipo tras AccesoAPagina

- **WHEN** el usuario cambia el tipo de auditoría de AccesoAPagina a otro tipo
- **THEN** el combo "Página" desaparece y la selección de página queda reseteada a null

#### Scenario: Limpiar filtro resetea también la página seleccionada

- **WHEN** el usuario pulsa "Limpiar"
- **THEN** el combo Página queda sin selección (null), además del resto de campos ya existentes
