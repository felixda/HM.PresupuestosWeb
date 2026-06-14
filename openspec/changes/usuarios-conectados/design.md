## Context

La aplicación HM.Presupuestos es una Blazor Server app. Cada usuario conectado mantiene un circuito SignalR en el servidor (`AddServerSideBlazor`). El servidor ya tiene `DisconnectedCircuitRetentionPeriod = 6 min` y `DisconnectedCircuitMaxRetained = 100`, lo que significa que un circuito persiste hasta 6 minutos tras desconectarse.

No existe actualmente ningún mecanismo que agregue información global sobre sesiones activas. `SesionUsuario` es Scoped (una instancia por circuito) y `ContextoUsuario` también, por lo que no son accesibles globalmente.

La autenticación tiene dos orígenes: SSO (Azure AD) e impersonación (login manual). Solo los usuarios SSO deben aparecer en la lista.

## Goals / Non-Goals

**Goals:**
- Registrar en memoria cada sesión SSO activa al autenticarse.
- Actualizar la página actual de la sesión en cada navegación.
- Eliminar la sesión del registro cuando el circuito SignalR se destruye.
- Exponer la lista a una página de administración con refresco manual.
- Añadir menú `UsuariosConectados = 27` bajo Administración.

**Non-Goals:**
- Actualización automática/push en tiempo real en la página.
- Soporte multi-tab (last-write-wins por login).
- Persistencia entre reinicios.
- Gestión de sesiones de usuarios impersonados.

## Decisions

### D1 — Singleton `IRegistroSesionesActivas` como almacén central

**Elección**: nueva interfaz `IRegistroSesionesActivas` con implementación `RegistroSesionesActivas` registrada como Singleton en `Web/Adaptadores/Sesion/`.

```csharp
public interface IRegistroSesionesActivas
{
    void Registrar(string login, DateTime inicio);
    void ActualizarPagina(string login, string pagina);
    void Eliminar(string login);
    IReadOnlyList<SesionActivaInfo> ObtenerTodas();
}

public record SesionActivaInfo(string Login, string PaginaActual, DateTime Inicio);
```

Internamente usa `ConcurrentDictionary<string, SesionActivaInfo>` para thread-safety sin locks explícitos.

**Alternativa descartada**: usar `IMemoryCache` — añade complejidad de expiración innecesaria para un dato que se gestiona explícitamente.

**Alternativa descartada**: usar el Login como parte del CircuitId — no hay API pública de Blazor Server para obtener el CircuitId en un Scoped service.

### D2 — Clave por Login (last-write-wins)

**Elección**: la clave del diccionario es el `Login` del usuario SSO. Si el mismo usuario abre varias pestañas, la pestaña más recientemente activa sobreescribe la anterior.

**Rationale**: simplicidad. El caso multi-tab es infrecuente y la consecuencia (ver solo la última página activa) es aceptable. Una clave por circuito requeriría un mecanismo adicional para correlacionar circuito con usuario antes de que el usuario se autentique.

### D3 — `ICircuitHandler` para detectar destrucción de circuito

**Elección**: nueva clase `CircuitSesionTracker : CircuitHandler` registrada como Scoped. Al instanciarse recibe `IRegistroSesionesActivas` (Singleton) y `ISesionUsuario` (Scoped). Cuando `OnCircuitClosedAsync` se invoca, elimina la sesión del registro usando el login del usuario.

```
OnCircuitClosedAsync()
  → si SesionUsuario.UsuarioApp?.UsuarioActivo.Login no nulo
  → RegistroSesionesActivas.Eliminar(login)
```

**Alternativa descartada**: `IHostedService` con timer de limpieza — añade complejidad y latencia. El `CircuitHandler` es el mecanismo semántico correcto.

### D4 — Registro en `SesionUsuario` solo para SSO

**Elección**: en el método que finaliza la autenticación SSO (`AutenticarUsuarioSSOAsync`), tras asignar el usuario, llamar a `IRegistroSesionesActivas.Registrar(login, DateTime.UtcNow)`. El método de impersonación (`AutenticarUsuarioPorLoginAsync`) no llama a este método.

### D5 — Actualización de página en `MainLayout`

**Elección**: en `OnLocationChangedAsync` (ya existente en `MainLayout.Navegacion.cs`), tras registrar el log de acceso, llamar a `IRegistroSesionesActivas.ActualizarPagina(login, urlNormalizada)` solo si el usuario está autenticado por SSO. `MainLayout` inyecta `IRegistroSesionesActivas` como dependencia adicional.

### D6 — Nueva página `UsuariosConectados`

**Elección**: `Pages/Admin/UsuariosConectados.razor` hereda de `ContextProtegido`. Inyecta `IRegistroSesionesActivas` directamente (es Web, no viola arquitectura hexagonal). Muestra un `DxGrid` con columnas Login, Página actual, Conectado desde (hora), Tiempo en sesión (calculado). Botón "Actualizar" llama a `StateHasChanged`.

### D7 — Menú temporal hardcoded

**Elección**: añadir `CodigosMenu.UsuariosConectados = 27` al enum en `Domain/Compartido/Enumerados.cs`. En `SesionUsuario.cs`, junto al bloque TODO-TEMPORAL de Auditorías, añadir bloque equivalente para el menú 27 bajo Administración (20). Se eliminará cuando se registre en BD.

### D8 — Claves de traducción

Nuevas claves en `TextosApp.Pages` bajo una clase `UsuariosConectados`:
- `Titulo`, `Login`, `PaginaActual`, `ConectadoDesde`, `TiempoEnSesion`, `Actualizar`, `SinUsuariosConectados`

## Risks / Trade-offs

- **Delay de baja de hasta 6 min**: cuando un usuario cierra el navegador, el circuito permanece retenido hasta 6 minutos antes de destruirse. La sesión seguirá apareciendo en la lista durante ese tiempo. → Mitigación: documentado en la UI con un aviso informativo.

- **Multi-tab**: si el usuario abre dos pestañas, solo la más recientemente activa se refleja. → Aceptado y documentado en propuesta.

- **Reinicio del servidor**: la lista se vacía. → Aceptado, comportamiento esperado para datos in-memory.

- **Thread-safety**: `ConcurrentDictionary` garantiza operaciones atómicas, pero la combinación lectura-escritura en `ActualizarPagina` (reemplazar `record`) es atómica con `AddOrUpdate`. Sin riesgo de corrupción.
