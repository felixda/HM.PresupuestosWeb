## ADDED Requirements

### Requirement: Acceso a la página de auditorías

La página `/admin/auditorias` SHALL estar protegida por el permiso `CodigosMenu.Auditorias` (código 26). Solo los usuarios con ese menú asignado podrán acceder.

#### Scenario: Usuario con permiso accede a la página

- **WHEN** un usuario con `CodigosMenu.Auditorias` en su lista de menús navega a `/admin/auditorias`
- **THEN** el sistema muestra la página con el filtro y el grid vacío

#### Scenario: Usuario sin permiso intenta acceder

- **WHEN** un usuario sin `CodigosMenu.Auditorias` navega a `/admin/auditorias`
- **THEN** el sistema muestra la pantalla de acceso denegado estándar

---

### Requirement: Filtro de búsqueda de auditorías

La página SHALL mostrar un formulario de filtro con los siguientes campos:
- **Tipo auditoria** (obligatorio): combo cargado con todos los valores del enum `AccionesLog` usando sus descripciones
- **Fecha inicio** (opcional): selector de fecha
- **Fecha fin** (opcional): selector de fecha
- **Botón Buscar**: siempre habilitado
- **Botón Limpiar**: siempre habilitado, situado a la izquierda del botón Buscar

#### Scenario: Buscar sin tipo de auditoría seleccionado

- **WHEN** el usuario pulsa "Buscar" sin haber seleccionado un tipo de auditoría
- **THEN** el sistema muestra un mensaje de aviso indicando que debe seleccionar los campos obligatorios y no lanza ninguna consulta

#### Scenario: Buscar con tipo de auditoría seleccionado

- **WHEN** el usuario selecciona un tipo de auditoría y pulsa "Buscar"
- **THEN** el sistema consulta `PPT_ACCION_LOG` filtrando por registros cuyo `DES_PROCESO` comience por `[N]` (donde N es el código del tipo seleccionado) y muestra los resultados en el grid

#### Scenario: Buscar con tipo y rango de fechas

- **WHEN** el usuario selecciona un tipo de auditoría, una fecha inicio y una fecha fin, y pulsa "Buscar"
- **THEN** el sistema consulta `PPT_ACCION_LOG` filtrando por tipo y por `FECHA_INICIO` dentro del rango indicado (inclusivo)

#### Scenario: Limpiar filtro y resultados

- **WHEN** el usuario pulsa "Limpiar"
- **THEN** el sistema resetea el combo de tipo a vacío, las fechas a vacío, y el grid queda sin datos

---

### Requirement: Grid de resultados de auditorías

El grid SHALL estar siempre visible en la página. Muestra las columnas:
- **Descripción**: contenido de `DES_PROCESO`
- **Fecha Inicio**: `FECHA_INICIO` formateada como `dd/MM/yyyy HH:mm:ss`
- **Usuario**: nombre extraído del campo `PARAMETROS` (JSON) con formato `Nombre Apellido1`

#### Scenario: Grid vacío antes de la primera búsqueda

- **WHEN** el usuario accede a la página por primera vez
- **THEN** el grid está visible pero sin filas

#### Scenario: Grid con resultados tras búsqueda

- **WHEN** la búsqueda devuelve registros
- **THEN** el grid muestra una fila por cada registro con los campos Descripción, Fecha Inicio y Usuario

#### Scenario: Grid sin resultados tras búsqueda

- **WHEN** la búsqueda no devuelve ningún registro con los filtros aplicados
- **THEN** el grid muestra el mensaje estándar de "sin datos" de DevExpress

#### Scenario: Usuario sin datos en PARAMETROS

- **WHEN** un registro tiene el campo `PARAMETROS` vacío, con valor `-`, o sin los campos `Nombre`/`Apellido1`
- **THEN** la columna Usuario muestra el texto `"Sin Usuario especificado"`
