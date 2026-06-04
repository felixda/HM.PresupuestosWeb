using HM.Presupuestos.Domain.Extensiones;
using System.Text.Json;

namespace HM.Presupuestos.Web.Adaptadores.Sesion
{
    public interface ISesionUsuario
    {
        ContextoUsuario? UsuarioApp { get; }

        event Func<Task>? UsuarioCargado;

        Task InicializarUsuarioAsync();
        Task CerrarSesionLoginAsync();
        Task<UsuarioEntidad?> AutenticarUsuarioSSOAsync(bool esRecargaPagina = false);
        Task<bool> AutenticarUsuarioPorLoginAsync(string login, string password, bool esRecargaPagina = false);
    }

    public class SesionUsuario : ISesionUsuario
    {
        private readonly IAlmacenSesionUsuario _sesionUsuario;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILogger<SesionUsuario> _logger;
        private readonly IConfiguration _configuracion;
        private readonly IControlador _servicioAutenticacion;
        private readonly ILogAccionesService _registroAcciones;
        private readonly IJwt _jwt;
        private readonly IValidadorMenusUsuario _validadorMenusUsuario;
 

        public event Func<Task>? UsuarioCargado;

        public ContextoUsuario? UsuarioApp { get; private set; }


        public SesionUsuario(
            IAlmacenSesionUsuario sesionUsuario,
            IControlador servicioAutenticacion,
            IJwt jwt,
            IConfiguration configuracion,
            AuthenticationStateProvider authStateProvider,
            ILogAccionesService registroAcciones,
            ILogger<SesionUsuario> logger,
            IValidadorMenusUsuario validadorMenusUsuario)
        {
            _sesionUsuario = sesionUsuario;
            _registroAcciones = registroAcciones;
            _authStateProvider = authStateProvider;
            _logger = logger;
            _servicioAutenticacion = servicioAutenticacion;
            _jwt = jwt;
            _configuracion = configuracion;
            _validadorMenusUsuario = validadorMenusUsuario;
        }

        public async Task InicializarUsuarioAsync()
        {
            if (UsuarioApp != null)
            {
                return;
            }

            var usuario = new ContextoUsuario();

            UsuarioEntidad usuarioSSO = await _sesionUsuario.ObtenerUsuarioSSO(); 
            UsuarioEntidad? usuarioLogin = await _sesionUsuario.ObtenerUsuarioImpersonado();

            usuario.AsignarUsuarioAutenticado(usuarioSSO);
            usuario.AsignarUsuarioImpersonado(usuarioLogin);

            UsuarioApp = usuario;
            _jwt.Usuario = UsuarioApp.UsuarioActivo;

            if (UsuarioCargado != null)
            {
                await UsuarioCargado.Invoke();
            }
        }


        public async Task<bool> AutenticarUsuarioPorLoginAsync(string login, string password, bool esRecargaPagina = false)
        {
            try
            {
                var nombreUsuarioNormalizado = login.Replace(" ", "");
                var usuarioName = nombreUsuarioNormalizado.Contains("@")
                    ? nombreUsuarioNormalizado[..nombreUsuarioNormalizado.IndexOf('@')]
                    : nombreUsuarioNormalizado;

                _logger.LogDebug("Validando usuario Login {UserName} con servicio externo...", usuarioName);

                var applicationCode = _configuracion.GetValue<int>("AppSettings:AppCode");

                RespuestaLogin respuestaLogin = await _servicioAutenticacion.ValidarUsuario(applicationCode, usuarioName, password);
                if (respuestaLogin.LoginStatus != LoginStatusEnum.Correcto)
                {
                    _logger.LogWarning("Validación de usuario falló: {Status}", respuestaLogin.LoginStatus);
                    return false;
                }

                UsuarioEntidad usuario = await CrearUsuarioDesdeRespuestaServicioExterno(respuestaLogin, applicationCode, usuarioName, OrigenValidacionUsuario.Login, esRecargaPagina);

                await _sesionUsuario.GuardarUsuarioImpersonado(usuario);

                UsuarioApp ??= new ContextoUsuario();
                UsuarioApp.AsignarUsuarioImpersonado(usuario);

                // Disparar evento de forma asíncrona SIN esperar (fire-and-forget)
                // Esto permite que el método retorne inmediatamente sin bloquearse
                if (UsuarioCargado != null)
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await UsuarioCargado.Invoke();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error al ejecutar OnUsuarioCargado en CargarUsuarioLoginDesdeServicioExterno");
                        }
                    });
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar usuario Login desde servicio externo");
                throw;
            }
        }


        public async Task<UsuarioEntidad?> AutenticarUsuarioSSOAsync(bool esRecargaPagina = false)
        {
            try
            {
                // Obtener usuario autenticado desde Azure AD/Windows
                var authState = await _authStateProvider.GetAuthenticationStateAsync();
                var userSSO = authState?.User;

                if (authState == null ||
                    userSSO == null ||
                    userSSO.Identity == null ||
                    !userSSO.Identity.IsAuthenticated ||
                    string.IsNullOrEmpty(userSSO.Identity.Name))
                {
                    _logger.LogWarning("Usuario no autenticado o datos incompletos");
                    return null;
                }

                var username = userSSO.Identity.Name;
                var nombreUsuarioSinDominio = username.Contains("@")
                    ? username[..username.IndexOf('@')]
                    : username;

                _logger.LogDebug("Validando usuario {UserName} con servicio externo...", nombreUsuarioSinDominio);

                var codigoAplicacion = _configuracion.GetValue<int>("AppSettings:AppCode");
                var tokenInternalAuthentication = _configuracion.GetValue<string>("AppSettings:Session:TokenInternalAuthentication");

                RespuestaLogin respuestaLogin = await _servicioAutenticacion.ValidarUsuario(
                        codigoAplicacion,
                        nombreUsuarioSinDominio,
                        tokenInternalAuthentication);


                if (respuestaLogin.LoginStatus != LoginStatusEnum.Correcto)
                {
                    _logger.LogWarning("Validación de usuario falló: {Status}", respuestaLogin.LoginStatus);
                    return null;
                }

                UsuarioEntidad usuario = await CrearUsuarioDesdeRespuestaServicioExterno(respuestaLogin, codigoAplicacion , nombreUsuarioSinDominio, OrigenValidacionUsuario.SSO, esRecargaPagina);

                await _sesionUsuario.GuardarUsuarioSSO(usuario);

                UsuarioApp ??= new ContextoUsuario();
                UsuarioApp!.AsignarUsuarioAutenticado(usuario);

                return usuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar usuario SSO desde servicio externo");
                return null;
            }
        }

        public async Task CerrarSesionLoginAsync()
        {
            if (UsuarioApp == null)
            {
                Console.WriteLine("[SesionUsuario] ?? Usuario no cargado, no se puede eliminar");
                return;
            }
            
            var usuarioLogin = UsuarioApp.UsuarioImpersonado;
            if (usuarioLogin == null)
            {
                _logger.LogWarning("[UsuarioServicio] No hay usuario login para eliminar");
                return;
            }

            LogAccion logAccionLogin = CrearRegistroAccionUsuario(usuarioLogin, AccionesLog.CerrarSesionUsuarioLogin);

            await _sesionUsuario.EliminarUsuarioImpersonado(); 
            UsuarioApp.DesactivarUsuarioImpersonado();

            await _registroAcciones.Insertar(logAccionLogin);

            _logger.LogInformation("? Usuario {UserName} descargado exitosamente ", usuarioLogin.Nombre);

            var usuarioCopiaActivo = UsuarioApp.UsuarioActivo;
            LogAccion logAccionSSO = CrearRegistroAccionUsuario(usuarioCopiaActivo, AccionesLog.VolverEntrarEnPresupuestosWebSSO);
            await _registroAcciones.Insertar(logAccionSSO);

            _logger.LogInformation("? Usuario Autenticado {UserName} cargado de nuevo exitosamente ", usuarioCopiaActivo.Nombre);

            if (UsuarioCargado != null)
            {
                await UsuarioCargado.Invoke();
            }
        }

        #region Private Methods

        /// <summary>
        /// Crea un LogAccion basado en una copia segura del usuario (sin JWT ni Token)
        /// </summary>
        /// <param name="usuario">Usuario original</param>
        /// <param name="accion">Acción del log</param>
        /// <returns>LogAccion configurado y listo para insertar</returns>
        private LogAccion CrearRegistroAccionUsuario(UsuarioEntidad usuario, AccionesLog accion)
        {
            // Crear copia del usuario sin datos sensibles para el log
            var usuarioCopia = DatosHelper.ClonarObjeto(usuario);
            usuarioCopia.Jwt = string.Empty;
            usuarioCopia.Token = Guid.Empty;

            return new LogAccion
            {
                CodigoUsuario = usuario.CodigoUsuario,
                Accion = $"(UsuarioServicio) -> {accion.ObtenerDescripcion()}",
                Parametros = JsonSerializer.Serialize(usuarioCopia, new JsonSerializerOptions { WriteIndented = true })
            };
        }

        private async Task<UsuarioEntidad> CrearUsuarioDesdeRespuestaServicioExterno(
            RespuestaLogin respuestaLogin,
            int codigoAplicacion,
            string nombreUsuario,
            OrigenValidacionUsuario origen,
            bool esRecargaPagina = false)
        {
            UsuarioEntidad usuario = _jwt.ObtenerDatosUsuarioJwt(respuestaLogin.Jwt);

            // Convertir reglas
            List<Regla> reglas = respuestaLogin.Reglas.Select(reglaEntidad => new Regla
            {
                Id = reglaEntidad.Id,
                Descripcion = reglaEntidad.Descripcion,
                Password = reglaEntidad.Password
            }).ToList();

            usuario.CodigoAplicacion = codigoAplicacion;
            usuario.Reglas = reglas;
            usuario.Roles = respuestaLogin.Roles;
            usuario.Jwt = respuestaLogin.Jwt;
            usuario.Login = nombreUsuario;

            // TODO: Eliminar esta línea cuando ya no sea necesario
            usuario.Menus.RemoveAll(m => m.Id == 1);

            // ? VALIDAR Y FILTRAR MENÚS CON URLs INVÁLIDAS
            await FiltrarMenusInvalidosAsync(usuario);


            // ? Determinar la descripción de la acción del log según el origen de validación y si es una recuperación de sesión (F5)
            // Este switch utiliza pattern matching con tuplas (característica de C# 8.0+)
            // Evalúa la combinación de dos valores: 'origen' (enum) y 'esRecargaPagina' (bool)
            AccionesLog accionLog = (origen, esRecargaPagina) switch
            {
                // Caso 1: Usuario autenticado por SSO que recarga la página (F5)
                (OrigenValidacionUsuario.SSO, true) => AccionesLog.RecuperarSesionDespuesDeF5SSO,

                // Caso 2: Usuario autenticado por SSO que entra por primera vez
                (OrigenValidacionUsuario.SSO, false) => AccionesLog.EntrarEnPresupuestosWebSSO,

                // Caso 3: Usuario con login manual (impersonación) que recarga la página (F5)
                (OrigenValidacionUsuario.Login, true) => AccionesLog.RecuperarSesionDespuesDeF5Impersonacion,

                // Caso 4: Usuario con login manual (impersonación) que entra por primera vez
                (OrigenValidacionUsuario.Login, false) => AccionesLog.EntrarEnPresupuestosWebImpersonacion,

                // Caso por defecto: Si no coincide ninguna combinación, usar SSO como predeterminado
                _ => AccionesLog.EntrarEnPresupuestosWebSSO
            };


            LogAccion logAccion = CrearRegistroAccionUsuario(usuario, accionLog);

            await _registroAcciones.Insertar(logAccion);

            _logger.LogInformation("? Usuario {UserName} cargado exitosamente desde servicio externo", nombreUsuario);

            return usuario;
        }
     


        /// <summary>
        /// Valida y filtra los menús del usuario, dejando solo aquellos cuya URL existe como página Blazor
        /// </summary>
        /// <param name="usuario">Usuario a validar</param>
        private async Task FiltrarMenusInvalidosAsync(UsuarioEntidad usuario)
        {
            try
            {
                if (usuario?.Menus == null || !usuario.Menus.Any())
                {
                    _logger.LogWarning("[UsuarioServicio] Usuario sin menús para validar");
                    return;
                }

                var totalMenusOriginales = usuario.Menus.Count;
                var menusHijosOriginales = usuario.Menus.Count(m => m.IdPadre != null);

                _logger.LogDebug("[UsuarioServicio] ?? Iniciando validación de menús para usuario {Login}", usuario.Login);
                _logger.LogDebug("[UsuarioServicio] Total menús: {Total}, Menús hijos: {Hijos}",
                    totalMenusOriginales, menusHijosOriginales);

                // Validar menús hijos (los que tienen URL)
                var resultadosValidacion = await _validadorMenusUsuario.ValidarSubmenusDe(usuario);

                // Obtener IDs de menús inválidos
                var menusInvalidosIds = resultadosValidacion
                    .Where(r => !r.Existe)
                    .Select(r => r.CodigoMenu)
                    .ToHashSet();

                if (menusInvalidosIds.Count != 0)
                {
                    _logger.LogWarning("[UsuarioServicio] ?? Se encontraron {Count} menús con URLs inválidas que serán eliminados:",
                        menusInvalidosIds.Count);

                    // Loggear menús que serán eliminados
                    foreach (var resultado in resultadosValidacion.Where(r => !r.Existe))
                    {
                        _logger.LogWarning("[UsuarioServicio]   ? Menú ID: {Id}, Nombre: {Nombre}, URL: {Url}",
                            resultado.CodigoMenu,
                            resultado.NombreMenu,
                            resultado.UrlOriginal);

                        if (resultado.UrlsSimilares.Count != 0)
                        {
                            _logger.LogInformation("[UsuarioServicio]      ?? Sugerencias: {Similares}",
                                string.Join(", ", resultado.UrlsSimilares));
                        }
                    }

                    // Eliminar menús inválidos de la colección
                    var menusEliminados = usuario.Menus.RemoveAll(m => menusInvalidosIds.Contains(m.Id));

                    _logger.LogWarning("[UsuarioServicio] ??? Se eliminaron {Count} menús inválidos", menusEliminados);
                    _logger.LogInformation("[UsuarioServicio] ? Menús restantes: {Total} (Hijos: {Hijos})",
                        usuario.Menus.Count,
                        usuario.Menus.Count(m => m.IdPadre != null));

                    // También eliminar menús padres que se quedaron sin hijos
                    var menusPadresIdsConHijos = usuario.Menus
                        .Where(m => m.IdPadre != null)
                        .Select(m => m.IdPadre)
                        .Distinct()
                        .ToHashSet();

                    var menusPadresSinHijos = usuario.Menus
                        .Where(m => m.IdPadre == null && !menusPadresIdsConHijos.Contains(m.Id))
                        .Select(m => m.Id)
                        .ToList();

                    if (menusPadresSinHijos.Count != 0)
                    {
                        var padresEliminados = usuario.Menus.RemoveAll(m => menusPadresSinHijos.Contains(m.Id));
                        _logger.LogWarning("[UsuarioServicio] ??? Se eliminaron {Count} menús padres sin hijos: {Ids}",
                            padresEliminados,
                            string.Join(", ", menusPadresSinHijos));
                    }
                }
                else
                {
                    _logger.LogInformation("[UsuarioServicio] ? Todos los menús ({Count}) tienen URLs válidas",
                        menusHijosOriginales);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[UsuarioServicio] ? Error al validar y filtrar menús del usuario {Login}",
                    usuario.Login);
                // No lanzamos la excepción para no bloquear el login, solo registramos el error
                // El usuario seguirá con sus menús originales si hay error en la validación
            }
        }

    }
}

        #endregion
    







