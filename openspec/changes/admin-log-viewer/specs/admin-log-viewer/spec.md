## ADDED Requirements

### Requirement: Ver logs tecnicos desde administracion
El sistema SHALL mostrar una pagina de administracion accesible solo para usuarios con el permiso de menu correspondiente que permita consultar logs tecnicos y avisos generados por la aplicacion.

#### Scenario: Acceso con permiso
- **WHEN** un administrador funcional navega a la ruta de logs de administracion
- **THEN** el sistema muestra la pagina con filtros y resultados de consulta

#### Scenario: Acceso sin permiso
- **WHEN** un usuario sin el permiso de menu de logs navega a la ruta de administracion de logs
- **THEN** el sistema muestra la vista de acceso denegado

### Requirement: Filtrar logs por rango de fechas y campos operativos
El sistema SHALL permitir filtrar logs tecnicos por fecha desde, fecha hasta, nivel, usuario y categoria.

#### Scenario: Consulta por rango de fechas
- **WHEN** el administrador indica una fecha desde y una fecha hasta y ejecuta la busqueda
- **THEN** el sistema devuelve solo entradas comprendidas en ese rango

#### Scenario: Consulta por nivel
- **WHEN** el administrador selecciona un nivel de log y ejecuta la busqueda
- **THEN** el sistema devuelve solo entradas con ese nivel

#### Scenario: Consulta por usuario
- **WHEN** el administrador indica un usuario y ejecuta la busqueda
- **THEN** el sistema devuelve solo entradas asociadas a ese usuario

#### Scenario: Consulta por categoria
- **WHEN** el administrador indica una categoria y ejecuta la busqueda
- **THEN** el sistema devuelve solo entradas asociadas a esa categoria

#### Scenario: Categoria basada en metodo origen
- **WHEN** el sistema muestra o filtra la categoria de una entrada de log
- **THEN** la categoria corresponde al nombre del metodo donde se genero el log

#### Scenario: Consulta de todos los niveles
- **WHEN** el administrador no selecciona un nivel concreto
- **THEN** el sistema devuelve entradas de todos los niveles de log actualmente soportados

#### Scenario: Consulta de entradas sin usuario
- **WHEN** el administrador selecciona la opcion `Sin usuario` en el filtro de usuario
- **THEN** el sistema devuelve solo entradas que no tienen usuario asociado

### Requirement: Mostrar resumen y detalle de cada entrada
El sistema SHALL mostrar un listado resumido de resultados y SHALL permitir abrir el detalle completo de una entrada concreta.

#### Scenario: Visualizacion resumida
- **WHEN** la consulta devuelve resultados
- **THEN** el sistema muestra al menos fecha y hora, nivel, usuario, categoria y mensaje resumido por cada entrada

#### Scenario: Mostrar entradas sin usuario
- **WHEN** una entrada de log no tiene usuario asociado
- **THEN** el sistema la muestra igualmente identificada como `Sin usuario`

#### Scenario: Apertura de detalle
- **WHEN** el administrador abre una entrada concreta del listado
- **THEN** el sistema muestra el detalle ampliado de la entrada, incluyendo la informacion tecnica disponible para esa linea de log

### Requirement: Informar cuando no existan resultados
El sistema SHALL informar de forma explicita cuando una busqueda no encuentre entradas de log para los filtros indicados.

#### Scenario: Consulta sin coincidencias
- **WHEN** el administrador ejecuta una busqueda y no existen logs que cumplan los filtros
- **THEN** el sistema muestra un estado vacio informando que no hay resultados

### Requirement: Leer logs diarios estructurados del servidor actual
El sistema SHALL leer las entradas de log desde los ficheros diarios estructurados del servidor del entorno actual, sin depender de almacenamiento centralizado.

#### Scenario: Lectura de un unico dia
- **WHEN** la consulta solo cubre un dia natural
- **THEN** el sistema lee exclusivamente el fichero diario correspondiente a ese dia

#### Scenario: Lectura de varios dias
- **WHEN** la consulta cubre varios dias naturales
- **THEN** el sistema lee los ficheros diarios incluidos en el rango solicitado

### Requirement: Mantener el formato estructurado para consulta
El sistema SHALL persistir cada log tecnico consultable en un formato estructurado JSON por linea dentro del fichero diario.

#### Scenario: Escritura de nueva entrada de log
- **WHEN** la aplicacion registra un error tecnico o un aviso en fichero
- **THEN** la entrada se escribe como una linea JSON independiente dentro del fichero diario del dia correspondiente

#### Scenario: Categoria persistida como campo explicito
- **WHEN** la aplicacion escribe una entrada de log tecnico consultable
- **THEN** la linea JSON incluye un campo explicito con el nombre del metodo origen usado como categoria