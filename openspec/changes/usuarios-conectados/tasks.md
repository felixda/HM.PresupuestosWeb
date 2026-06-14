## 1. Domain — Enumerado de menú

- [ ] 1.1 Añadir valor `UsuariosConectados = 27` al enum `CodigosMenu` en `Domain/Compartido/Enumerados.cs`

## 2. Web — Registro de sesiones activas (Singleton)

- [ ] 2.1 Crear `record SesionActivaInfo(string Login, string PaginaActual, DateTime Inicio)` en `Web/Adaptadores/Sesion/`
- [ ] 2.2 Crear interfaz `IRegistroSesionesActivas` con métodos `Registrar`, `ActualizarPagina`, `Eliminar`, `ObtenerTodas` en `Web/Adaptadores/Sesion/`
- [ ] 2.3 Implementar `RegistroSesionesActivas` usando `ConcurrentDictionary<string, SesionActivaInfo>` en `Web/Adaptadores/Sesion/`
- [ ] 2.4 Registrar `IRegistroSesionesActivas` como Singleton en `Program.cs`

## 3. Web — CircuitHandler para detectar cierre de circuito

- [ ] 3.1 Crear `CircuitSesionTracker : CircuitHandler` en `Web/Adaptadores/Sesion/` que en `OnCircuitClosedAsync` llama a `IRegistroSesionesActivas.Eliminar(login)` si el usuario SSO tiene sesión registrada
- [ ] 3.2 Registrar `CircuitSesionTracker` como Scoped CircuitHandler en `Program.cs`

## 4. Web — Integración en SesionUsuario

- [ ] 4.1 Inyectar `IRegistroSesionesActivas` en `SesionUsuario` mediante primary constructor
- [ ] 4.2 Llamar a `IRegistroSesionesActivas.Registrar(login, DateTime.UtcNow)` al final de `AutenticarUsuarioSSOAsync`, antes de retornar
- [ ] 4.3 Añadir bloque TODO-TEMPORAL en `SesionUsuario` para el menú `UsuariosConectados = 27` como hijo de `Administracion = 20`

## 5. Web — Integración en MainLayout

- [ ] 5.1 Inyectar `IRegistroSesionesActivas` en `MainLayout` con `[Inject]`
- [ ] 5.2 En `OnLocationChangedAsync`, tras registrar el log de acceso, llamar a `IRegistroSesionesActivas.ActualizarPagina(login, urlNormalizada)` solo si el usuario está autenticado (SSO)

## 6. Web — Traducciones

- [ ] 6.1 Añadir clase `UsuariosConectados` en `TextosApp.Pages` con claves: `Titulo`, `Login`, `PaginaActual`, `ConectadoDesde`, `TiempoEnSesion`, `Actualizar`, `SinUsuariosConectados`
- [ ] 6.2 Añadir las traducciones correspondientes en `app.es.json`

## 7. Web — Página UsuariosConectados

- [ ] 7.1 Crear `Pages/Admin/UsuariosConectados.razor` con ruta `/admin/usuarios-conectados`, hereda de `ContextProtegido`, con `DxGrid` de columnas Login, Página actual, Conectado desde, Tiempo en sesión y botón Actualizar
- [ ] 7.2 Crear `Pages/Admin/UsuariosConectados.razor.cs` con `CodigoMenuPermiso = CodigosMenu.UsuariosConectados`, inyecta `IRegistroSesionesActivas`, implementa `InicializarPaginaAsync` y método `ActualizarAsync`
- [ ] 7.3 Crear `Pages/Admin/UsuariosConectados.razor.css` con estilos básicos de la página
