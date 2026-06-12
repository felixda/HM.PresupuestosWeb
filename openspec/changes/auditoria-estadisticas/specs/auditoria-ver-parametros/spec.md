## MODIFIED Requirements

### Requirement: Ver parámetros de una entrada de auditoría
El sistema SHALL mostrar el contenido del campo `Parametros` de una entrada de auditoría al pulsar un icono en el grid de la página de Auditorías, mediante un popup modal con el JSON formateado y con scroll. La búsqueda que carga el grid SHALL requerir las fechas FechaInicio y FechaFin como campos obligatorios.

#### Scenario: Columna de parámetros visible en el grid
- **WHEN** el administrador accede a la página de Auditorías y realiza una búsqueda
- **THEN** el grid muestra una columna adicional con un icono por cada fila resultado

#### Scenario: Abrir popup con parámetros
- **WHEN** el administrador pulsa el icono de parámetros en una fila del grid
- **THEN** se abre un popup modal con el título localizado y el contenido del campo `Parametros` de esa fila formateado como texto

#### Scenario: Cerrar popup
- **WHEN** el administrador cierra el popup (botón cerrar o click fuera)
- **THEN** el popup se cierra y el grid queda en el estado anterior

#### Scenario: Entrada sin parámetros significativos
- **WHEN** el campo `Parametros` de una fila contiene únicamente el valor `"-"` (registro sin datos)
- **THEN** el popup muestra ese valor sin errores

#### Scenario: Acceso restringido
- **WHEN** un usuario sin permiso de administrador intenta acceder a la página
- **THEN** el sistema muestra la pantalla de acceso denegado (comportamiento heredado, sin cambio)

#### Scenario: Búsqueda sin fecha no es posible
- **WHEN** el administrador elimina FechaInicio o FechaFin e intenta pulsar Buscar
- **THEN** el sistema muestra un aviso indicando que las fechas son obligatorias y no ejecuta la consulta
