## Why

La página de Usuarios Conectados muestra quién está conectado en tiempo real, pero no permite saber por dónde ha navegado cada usuario durante la sesión. Para el soporte y la administración es útil poder ver las últimas páginas visitadas por un usuario específico en el día actual, sin salir de la página.

## What Changes

- Nuevo adaptador en memoria `IHistorialNavegacion` / `HistorialNavegacion` que almacena las últimas N páginas visitadas por cada usuario (configurable vía `appsettings.json`, por defecto 50).
- Integración en `MainLayout.OnLocationChangedAsync` para registrar cada visita (sin duplicar páginas consecutivas idénticas, ya gestionado por la lógica existente).
- El historial se acumula mientras el servidor esté en ejecución, sin resetearse al desconectarse el usuario.
- La página `UsuariosConectados` añade interacción: al seleccionar una fila del grid de usuarios, se muestra debajo un segundo grid con el historial de páginas visitadas por ese usuario en el día actual.
- Nuevas claves de traducción para los textos del segundo grid.

## Capabilities

### New Capabilities

- `historial-navegacion-ver`: Visualizar el historial de páginas visitadas por un usuario desde la página de Usuarios Conectados.

### Modified Capabilities

- `usuarios-conectados-ver`: La página de Usuarios Conectados incorpora interacción de selección de usuario y panel de historial.

## Impact

- **Web**: nuevo adaptador `HistorialNavegacion`, inyección en `MainLayout`, cambios en `UsuariosConectados.razor` y `.razor.cs`.
- **appsettings.json**: nueva sección `HistorialNavegacion` con clave `MaxEntradas`.
- **TextosApp / app.es.json**: nuevas claves de traducción para columnas y mensajes del historial.
- No requiere cambios en Domain, Application ni Infrastructure.
- No requiere nuevas acciones de auditoría ni nuevos permisos de menú.

## No incluido / Fuera de alcance

- Persistencia del historial en base de datos.
- Historial de días anteriores.
- Filtro por rango horario dentro del día.
- Notificaciones en tiempo real al actualizar el historial.
