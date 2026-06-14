## 1. Web — Adaptador HistorialNavegacion

- [x] 1.1 Crear `record EntradaHistorial(string Pagina, DateTime Hora)` en `Web/Adaptadores/Sesion/HistorialNavegacion.cs`
- [x] 1.2 Crear interfaz `IHistorialNavegacion` con métodos `RegistrarVisita(string login, string pagina)` y `ObtenerHistorial(string login): IReadOnlyList<EntradaHistorial>` en el mismo archivo
- [x] 1.3 Implementar `HistorialNavegacion` usando `ConcurrentDictionary<string, Queue<EntradaHistorial>>` con `lock` por entrada y límite `MaxEntradas` leído de `IConfiguration` (default 50)

## 2. Web — Registro en Program.cs y appsettings.json

- [x] 2.1 Registrar `IHistorialNavegacion` como Singleton en `Program.cs`
- [x] 2.2 Añadir sección `"HistorialNavegacion": { "MaxEntradas": 50 }` en `appsettings.json`

## 3. Web — Integración en MainLayout

- [x] 3.1 Inyectar `IHistorialNavegacion` en `MainLayout` con `[Inject]` en `MainLayout.razor.cs`
- [x] 3.2 En `MainLayout.Navegacion.cs`, en `OnLocationChangedAsync`, tras llamar a `RegistroSesionesActivas.ActualizarPagina`, llamar a `HistorialNavegacion.RegistrarVisita(loginSSO, urlNormalizada)`

## 4. Web — Traducciones

- [x] 4.1 Añadir clase `HistorialNavegacion` en `TextosApp.Pages` con claves: `Titulo`, `PaginaVisitada`, `Hora`, `SinHistorial`
- [x] 4.2 Añadir las traducciones correspondientes en `app.es.json` bajo `Pages:HistorialNavegacion`

## 5. Web — Página UsuariosConectados

- [x] 5.1 En `UsuariosConectados.razor.cs`, inyectar `IHistorialNavegacion`, añadir `_usuarioSeleccionado` y `_historialUsuario`, e implementar `OnUsuarioSeleccionadoAsync` que carga el historial
- [x] 5.2 En `UsuariosConectados.razor`, añadir `SelectedDataItemChanged` al `DxGrid` existente y renderizar condicionalmente el segundo `DxGrid` con columnas Página visitada y Hora
- [x] 5.3 Actualizar `ActualizarAsync` para recargar también el historial del usuario seleccionado si hay uno

## 6. Web — Correcciones UX e historial

- [x] 6.1 Sustituir `SelectedDataItemChanged` por columna con botón icono `fa-clock-rotate-left` en `UsuariosConectados.razor` para acceder al historial del usuario
- [x] 6.2 Añadir `margin-top: 2rem` a `.usuarios-conectados-historial` en `UsuariosConectados.razor.css`
- [x] 6.3 Excluir la página Home/Index del historial de navegación en `MainLayout.Navegacion.cs`
