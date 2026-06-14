## MODIFIED Requirements

### Requirement: Ver lista de usuarios SSO activos

El sistema SHALL mostrar una página de administración accesible solo para usuarios con permiso `CodigosMenu.UsuariosConectados` que lista todas las sesiones SSO activas en ese momento, con las columnas: Login, Página actual, Conectado desde (hora UTC), Tiempo en sesión (calculado). Al seleccionar una fila del grid, el sistema SHALL mostrar debajo un segundo grid con el historial de páginas visitadas por el usuario seleccionado.

#### Scenario: Acceso con permiso de administrador

- **WHEN** un administrador navega a `/admin/usuarios-conectados`
- **THEN** el sistema muestra la página con la lista de sesiones activas

#### Scenario: Acceso sin permiso

- **WHEN** un usuario sin permiso `UsuariosConectados` navega a `/admin/usuarios-conectados`
- **THEN** el sistema redirige a la página de acceso denegado

#### Scenario: Lista vacía

- **WHEN** no hay ningún usuario SSO conectado en ese momento
- **THEN** el sistema muestra un mensaje informativo indicando que no hay usuarios conectados

#### Scenario: Lista con usuarios conectados

- **WHEN** hay usuarios SSO autenticados con circuito activo
- **THEN** el sistema muestra una fila por cada usuario con su login, página actual, hora de conexión y tiempo transcurrido

#### Scenario: Selección de usuario muestra historial

- **WHEN** el administrador selecciona una fila del grid de usuarios
- **THEN** el sistema muestra un segundo grid debajo con el historial de páginas visitadas por ese usuario

### Requirement: Refresco manual de la lista

El sistema SHALL permitir actualizar la lista de sesiones activas y el historial del usuario seleccionado mediante un botón explícito. No SHALL actualizarse automáticamente de forma periódica.

#### Scenario: Refrescar la lista

- **WHEN** el administrador pulsa el botón "Actualizar"
- **THEN** el sistema recarga la lista de sesiones activas y, si hay un usuario seleccionado, también su historial

### Requirement: Registro de sesión al autenticarse por SSO

El sistema SHALL registrar en el almacén de sesiones activas el login y la hora de inicio cuando un usuario se autentica mediante SSO.

#### Scenario: Autenticación SSO exitosa

- **WHEN** un usuario completa la autenticación SSO
- **THEN** el sistema registra su login y la hora actual como nueva sesión activa

#### Scenario: Autenticación por impersonación

- **WHEN** un usuario se autentica mediante login/password (impersonación)
- **THEN** el sistema NO registra la sesión en el almacén de sesiones activas

### Requirement: Actualización de página actual en navegación

El sistema SHALL actualizar la página actual registrada para una sesión activa cada vez que el usuario navega a una nueva ruta, y SHALL registrar también la visita en el historial de navegación del usuario.

#### Scenario: Navegación a nueva página

- **WHEN** un usuario SSO autenticado navega a una nueva URL dentro de la aplicación
- **THEN** el sistema actualiza la página actual de su sesión y añade la URL al historial de navegación

#### Scenario: Usuario no autenticado navega

- **WHEN** un usuario no autenticado por SSO navega dentro de la aplicación
- **THEN** el sistema no registra ni actualiza ninguna sesión activa ni historial
