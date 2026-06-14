# historial-navegacion-ver Specification

## Purpose
TBD - created by archiving change historial-paginas-usuario. Update Purpose after archive.
## Requirements
### Requirement: Ver historial de páginas visitadas por un usuario conectado

El sistema SHALL mostrar, al seleccionar un usuario en el grid de Usuarios Conectados, un segundo grid con las últimas páginas visitadas por ese usuario durante el día actual, con columnas: Página visitada y Hora.

#### Scenario: Seleccionar un usuario del grid

- **WHEN** el administrador selecciona una fila del grid de usuarios conectados
- **THEN** el sistema muestra debajo un segundo grid con el historial de páginas visitadas por ese usuario en el día actual

#### Scenario: Usuario sin historial

- **WHEN** el administrador selecciona un usuario que no ha navegado a ninguna página registrada aún
- **THEN** el sistema muestra el segundo grid vacío con un mensaje informativo

#### Scenario: Historial con entradas

- **WHEN** el usuario seleccionado ha visitado páginas durante la sesión
- **THEN** el sistema muestra hasta `MaxEntradas` entradas ordenadas de más antigua a más reciente, con la página visitada y la hora de visita

#### Scenario: Límite máximo de entradas

- **WHEN** el número de visitas registradas supera el valor configurado en `HistorialNavegacion:MaxEntradas`
- **THEN** el sistema conserva únicamente las últimas `MaxEntradas` entradas, descartando las más antiguas

#### Scenario: No se registran recargas de página

- **WHEN** un usuario navega a la misma URL que ya tenía activa
- **THEN** el sistema NO añade una nueva entrada al historial

### Requirement: Historial acumulativo entre reconexiones

El sistema SHALL acumular el historial de navegación de un usuario a lo largo del día aunque el usuario se desconecte y vuelva a conectar.

#### Scenario: Reconexión del usuario

- **WHEN** un usuario SSO que ya tenía historial se desconecta y vuelve a autenticarse
- **THEN** el historial previo del día se mantiene y las nuevas visitas se acumulan a continuación

### Requirement: Historial configurable

El sistema SHALL permitir configurar el número máximo de entradas de historial por usuario mediante la clave `HistorialNavegacion:MaxEntradas` en `appsettings.json`.

#### Scenario: Configuración por defecto

- **WHEN** la clave `HistorialNavegacion:MaxEntradas` no está definida en `appsettings.json`
- **THEN** el sistema usa el valor por defecto de 50 entradas

#### Scenario: Configuración personalizada

- **WHEN** la clave `HistorialNavegacion:MaxEntradas` está definida con un valor entero positivo
- **THEN** el sistema usa ese valor como límite máximo de entradas por usuario

