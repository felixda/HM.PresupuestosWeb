using HM.Core.Comun.v6.Entidades.Logger;
using HM.Presupuestos.Domain.Extensiones;
using HM.Presupuestos.Infrastructure.Servicios;
using NLog;
using NLog.Targets;
using NLog.Config;
using NLog.Layouts;
using System.Runtime.CompilerServices;

namespace HM.Presupuestos.Web.Adaptadores.Auditoria
{

    public enum NivelRegistro
    {
        Error = 1,
        Warning = 2,
        Info = 3,
        Debug = 4,
        Trace = 5,
        Message = 6
    }

    public interface IRegistroAplicacion
    {
        Task RegistrarEvento(string category, string message, string? stackTrace, string comments, NivelRegistro logLevel = NivelRegistro.Error, bool insertDBLog = true, bool insertFileLog = true);
        Task RegistrarExcepcion(string category, Exception exception, string comments = "", NivelRegistro logLevel = NivelRegistro.Error, bool insertDBLog = true, bool insertFileLog = true);
        Task RegistrarExcepcion(Exception exception, string comments = "", NivelRegistro logLevel = NivelRegistro.Error, bool insertDBLog = true, bool insertFileLog = true, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "");
        Task RegistrarAccesoAPagina(string tituloPagina);
        Task RegistrarIntentoAccesoNoAutorizado(string ruta);
    }

    public class RegistroAplicacion:IRegistroAplicacion
    {
        #region Propiedades privadas

        private readonly IConfiguration _configuration;
        private readonly IAlmacenSesionUsuario _almacenSesionService;
        private readonly ISesionUsuario _sesionUsuario;
        private readonly IRutasNavegacion _rutasNavegacion;
        private readonly ILogAccionesService _logAccionesService;
        private readonly IClienteApiCore _clienteApiCore;

        private readonly IRecursosApp _recursosApp;

        #endregion

        #region Constructor

        public RegistroAplicacion(IConfiguration configuracion, 
            IAlmacenSesionUsuario almacenSesionUsuario, 
            ISesionUsuario sesionUsuario, 
            IRutasNavegacion rutasNavegacion, 
            ILogAccionesService logAccionesService,
            IClienteApiCore clienteApiCore,
            IRecursosApp recursosApp)
        {
            _configuration = configuracion;
            _almacenSesionService = almacenSesionUsuario;
            _sesionUsuario = sesionUsuario;
            _rutasNavegacion = rutasNavegacion;
            _logAccionesService = logAccionesService;
            _clienteApiCore = clienteApiCore;
            _recursosApp = recursosApp;
        }

        #endregion

        #region Public Methods

        public async Task RegistrarAccesoAPagina(string tituloPagina)
        {
            var urlActual = _rutasNavegacion.ObtenerRutaActual();
            int codigoMenu = _recursosApp.ObtenerCodigoMenuPorUrl(urlActual);
            var accion = AccionesLog.AccesoAPagina.ObtenerDescripcion();
            var usuario = _sesionUsuario.UsuarioApp?.UsuarioActivo;

            string accionConDetalle = $"[{(int)AccionesLog.AccesoAPagina}](RegistrarAccesoAPagina) -> {string.Format(accion.ToString(), tituloPagina)} [{urlActual}] [{codigoMenu}]";
            string parametros = usuario != null
                ? $"{{\"Login\":\"{usuario.Login}\",\"Nombre\":\"{usuario.Nombre} {usuario.Apellido1}\"}}"
                : string.Empty;

            await _logAccionesService.Insertar(new LogAccion { Accion = accionConDetalle, Parametros = parametros });
        }


        /// <summary>
        /// Registrar intento de acceso no autorizado
        /// </summary>
        public async Task RegistrarIntentoAccesoNoAutorizado( string ruta)
        {
            try
            { 
                await _logAccionesService.Insertar(
                      AccionesLog.IntentoAccesoNoAutorizado,
                      ruta);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MainLayout] ? Error al registrar acceso no autorizado: {ex.Message}");
                await RegistrarExcepcion(ex);
            }
        }

        /// <summary>
        /// Insert exception in database and file
        /// </summary>
        /// <param name="exception">Exception object</param>
        /// <param name="comments">Comments</param>
        /// <param name="logLevel">Log level</param>
        /// <param name="insertDBLog">Add entry to database</param>
        /// <param name="insertFileLog">Add entry to file log</param>
        /// <param name="callerFilePath"></param>
        /// <param name="callerMemberName"></param> 
        public async Task RegistrarExcepcion(
            Exception exception, 
            string comments = "", 
            NivelRegistro logLevel = NivelRegistro.Error, 
            bool insertDBLog = true, 
            bool insertFileLog = true, 
            [CallerFilePath] string callerFilePath = "", 
            [CallerMemberName] string callerMemberName = "")
        {
            var category = callerMemberName;
            await RegistrarExcepcion(category, exception, comments, logLevel, insertDBLog, insertFileLog);
        }

        public async Task RegistrarExcepcion(
            string category, 
            Exception exception, 
            string comments="", 
            NivelRegistro logLevel = NivelRegistro.Error, 
            bool insertDBLog = true, 
            bool insertFileLog = true)
        {
            if (!insertDBLog && !insertFileLog)
                return;

            var stackTrace = exception.StackTrace ?? string.Empty;
            var mensaje = ExcepcionesHelper.ObtenerMensajeCompletoExcepcion(exception);

            await RegistrarEvento(category, mensaje, stackTrace, comments, logLevel, insertDBLog, insertFileLog);
        }


        /// <summary>
        /// Insert log entry in database and file
        /// </summary>
        /// <param name="category">Category entry</param>
        /// <param name="message">Message description</param>
        /// <param name="stackTrace">Stack trace error</param>
        /// <param name="comments">Comments</param>
        /// <param name="logLevel">Log level</param>
        /// <param name="insertDBLog">Add entry to database</param>
        /// <param name="insertFileLog">Add entry to file log</param> 
        public async Task RegistrarEvento(
            string category, 
            string message, 
            string? stackTrace, 
            string comments, 
            NivelRegistro logLevel=NivelRegistro.Error,
            bool insertDBLog = true, 
            bool insertFileLog = true)
        {
            if (!insertDBLog && !insertFileLog)
                return;

            var usuario = _sesionUsuario.UsuarioApp?.UsuarioActivo;
            var stackTraceText = stackTrace ?? string.Empty;

            if (insertDBLog && usuario != null)
            {
                await RegistrarEnBaseDatos(category, message, stackTraceText, comments, usuario);
            }

            if (insertFileLog)
            {
                RegistrarEnArchivo(logLevel, usuario?.Login, category, message, stackTraceText, comments);
            }
        }

        #endregion

        #region Private Methods


        /// <summary>
        /// Inserta registro en la base de datos
        /// </summary>
        private async Task RegistrarEnBaseDatos(string category, string message, string stackTraceText, string comments, UsuarioEntidad usuario)
        {
            const int MaxMensajeLength = 1500;

            var data = new DatosPeticionLogData
            {
                CodigoAplicacion = usuario.CodigoAplicacion,
                CodigoPais = usuario.CodigoPais,
                CodigoCompania = usuario.CodigoCompania,
                UserName = usuario.Login,
                Fecha = DateTime.Now,
                Categoria = category,
                Mensaje = message.Length > MaxMensajeLength
                    ? message[..MaxMensajeLength]
                    : message,
                StackTrace = stackTraceText,
                Observaciones = comments,
                DominioAplicacion = _configuration.GetValue<string>("AppSettings:AppDomain")
            };

            await _clienteApiCore.RegistrarLog(usuario.Jwt, data);
        }


        /// <summary>
        /// Inserta log en archivo según nivel
        /// </summary>
        private void RegistrarEnArchivo(NivelRegistro logLevel, string? userName, string category, string message, string stackTraceText, string comments)
        {
            var logger = ObtenerLogger();
            var logEvent = new LogEventInfo(ConvertirNivel(logLevel), logger.Name, message);

            logEvent.Properties["category"] = string.IsNullOrWhiteSpace(category) ? "Unknown" : category;
            logEvent.Properties["login"] = string.IsNullOrWhiteSpace(userName) ? string.Empty : userName;
            logEvent.Properties["exception"] = message;
            logEvent.Properties["stackTrace"] = stackTraceText;
            logEvent.Properties["comments"] = comments;

            logger.Log(logEvent);
        }


        /// <summary>
        /// Obtener instancia de logger configurada para el tipo de log (log o err)
        /// </summary>
        /// <param name="extension">Extensión del archivo de log (log o err)</param>
        /// <returns>Instancia de Logger configurada</returns>
        private static Logger ObtenerLogger()
        {
            LogManager.Setup().LoadConfigurationFromFile("nlog.config");
            AsegurarTargetJson();
            return LogManager.GetCurrentClassLogger();
        }

        private static NLog.LogLevel ConvertirNivel(NivelRegistro logLevel)
        {
            return logLevel switch
            {
                NivelRegistro.Trace => NLog.LogLevel.Trace,
                NivelRegistro.Debug => NLog.LogLevel.Debug,
                NivelRegistro.Info => NLog.LogLevel.Info,
                NivelRegistro.Warning => NLog.LogLevel.Warn,
                NivelRegistro.Error => NLog.LogLevel.Error,
                NivelRegistro.Message => NLog.LogLevel.Info,
                _ => NLog.LogLevel.Info
            };
        }

        private static void AsegurarTargetJson()
        {
            if (LogManager.Configuration?.FindTargetByName<FileTarget>("FileLog") is not { } fileTarget)
                return;

            fileTarget.FileName = "${logDirectory}\\${appName}_${shortdate}.jsonl";
            fileTarget.Layout = new JsonLayout
            {
                Attributes =
                {
                    new JsonAttribute("timestamp", "${date:format=o}"),
                    new JsonAttribute("level", "${level}"),
                    new JsonAttribute("category", "${event-properties:item=category}"),
                    new JsonAttribute("login", "${event-properties:item=login}"),
                    new JsonAttribute("logger", "${logger}"),
                    new JsonAttribute("message", "${message}"),
                    new JsonAttribute("exception", "${event-properties:item=exception}"),
                    new JsonAttribute("stackTrace", "${event-properties:item=stackTrace}"),
                    new JsonAttribute("comments", "${event-properties:item=comments}")
                },
                IncludeAllProperties = false,
                SuppressSpaces = true
            };

            LogManager.ReconfigExistingLoggers(true);
        }

        #endregion
    }
}





