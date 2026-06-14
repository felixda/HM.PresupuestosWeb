## Why

Los administradores no tienen visibilidad de quién está usando la aplicación en un momento dado. Para soporte y monitorización operativa necesitan saber qué usuarios SSO están conectados, en qué página se encuentran y cuánto tiempo llevan en sesión, sin tener que revisar logs.

## What Changes

- Nueva página de administración `/admin/usuarios-conectados` visible solo para administradores.
- El servidor registra en memoria (Singleton) cada sesión SSO activa en el momento de autenticación.
- Cada cambio de página actualiza la URL registrada para esa sesión.
- Al destruirse el circuito SignalR (cierre de navegador + expiración de retención), la sesión se elimina del registro.
- Los usuarios autenticados por impersonación (login manual) no se registran ni aparecen en la lista.
- Si el mismo usuario abre varias pestañas, solo se mantiene la información de la pestaña más recientemente activa (last-write-wins por login).
- Nueva entrada de menú `UsuariosConectados = 27` en `CodigosMenu`, gestionada temporalmente de forma hardcoded hasta que se registre en BD.

## Capabilities

### New Capabilities

- `usuarios-conectados-ver`: Página de administración que muestra la lista de sesiones SSO activas con login, página actual y tiempo de conexión. Refresco manual mediante botón.

### Modified Capabilities

_(ninguna)_

## Impact

- **Domain**: sin cambios.
- **Application**: sin cambios.
- **Infrastructure**: sin cambios.
- **Web**:
  - Nueva interfaz y clase Singleton `IRegistroSesionesActivas` / `RegistroSesionesActivas` en `Adaptadores/Sesion/`.
  - Nuevo `ICircuitHandler` (`CircuitSesionTracker`) en `Adaptadores/Sesion/` para detectar destrucción de circuito.
  - Modificación de `SesionUsuario` para registrar la sesión al autenticar (solo SSO).
  - Modificación de `MainLayout` para actualizar la página actual en cada navegación.
  - Nueva página `Pages/Admin/UsuariosConectados.razor` + `.razor.cs` + `.razor.css`.
  - Nueva entrada `CodigosMenu.UsuariosConectados = 27` en `Domain/Compartido/Enumerados.cs`.
  - Bloque temporal hardcoded en `SesionUsuario` para añadir el menú 27 como hijo de Administración (20).
- **Traducciones**: nuevas claves en `TextosApp.Pages` para los literales de la nueva página.
- Sin nuevas acciones de auditoría (`AccionesLog`).
- Nuevo permiso `CodigosMenu.UsuariosConectados = 27`.

## No incluido / Fuera de alcance

- Actualización automática en tiempo real (sin polling ni SignalR push desde la página).
- Soporte multi-tab (se muestra solo la última pestaña activa del usuario).
- Histórico o log de sesiones pasadas.
- Capacidad de forzar el cierre de sesión de un usuario desde esta página.
- Persistencia entre reinicios del servidor.
- Mostrar usuarios autenticados por impersonación.
- Tiempo de inactividad (última acción realizada).
