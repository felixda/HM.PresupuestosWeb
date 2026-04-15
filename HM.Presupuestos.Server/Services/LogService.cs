using HM.Core.Comun.v6.Entidades.Logger;
using HM.Presupuestos.Contratos;
using HM.Presupuestos.Server.Servicios;
using NLog;
using NLog.Targets;

namespace HM.Presupuestos.Server.Services
{

    public enum LogLevel
    {
        Error = 1,
        Warning = 2,
        Info = 3,
        Debug = 4,
        Trace = 5,
        Message = 6
    }

    public interface ILogService
    {
       Task InsertLog(string category, string message, string? stackTrace, string comments, LogLevel logLevel = LogLevel.Error, bool insertDBLog = true, bool insertFileLog = true);
        Task InsertException(string category, Exception exception, string comments = "", LogLevel logLevel = LogLevel.Error, bool insertDBLog = true, bool insertFileLog = true);

        Task GrabarAccesoAPagina( string tituloPagina);

        Task RegistrarAccesoNoAutorizado( LogAccion logAccion);

    }

    public class LogService:ILogService
    {
        #region Propiedades privadas

        private readonly IConfiguration _configuration;
        private readonly IJSRuntime _JSRuntime;
        private readonly ISessionService _SessionService;
        private readonly IUsuarioServicio _usuarioServicio;
        private readonly INavigationService _navigationService;
        private readonly ILogAccionesService _logAccionesService;

        #endregion

        #region Constructor

        public LogService(IConfiguration configuration, IJSRuntime JSRuntime, ISessionService SessionService, IUsuarioServicio usuarioServicio, INavigationService navigationService, ILogAccionesService logAccionesService )
        {
            _configuration = configuration;
            _JSRuntime = JSRuntime;
            _SessionService = SessionService;
            _usuarioServicio = usuarioServicio;
            _navigationService = navigationService;
            _logAccionesService = logAccionesService;
        }

        #endregion


        #region Public Methods

        public async Task GrabarAccesoAPagina(string tituloPagina)
        {
            var urlActual = _navigationService.ObtenerUrlActual();
            var accion = $"Acceso a página {tituloPagina} [{urlActual}]";
            await _logAccionesService.Insertar(accion);
        }


        /// <summary>
        /// Registrar intento de acceso no autorizado
        /// </summary>
        public async Task RegistrarAccesoNoAutorizado( LogAccion logAccion)
        {
            try
            { 
                var usuario = _usuarioServicio.UsuarioApp!.Usuario;
                if (usuario == null)
                {
                    return;
                }

                await _logAccionesService.Insertar(logAccion);

                Console.WriteLine($"[MainLayout] ⚠️ Acceso denegado registrado: {usuario.Login} -> ({logAccion.Parametros!.ToString()})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MainLayout] ❌ Error al registrar acceso no autorizado: {ex.Message}");
                await InsertException(nameof(ContextProtegido), ex);
            }
        }

        /// <summary>
        /// Insert exception in database and file
        /// </summary>
        /// <param name="category">Category entry</param>
        /// <param name="exception">Exception object</param>
        /// <param name="comments">Comments</param>
        /// <param name="logLevel">Log level</param>
        /// <param name="insertDBLog">Add entry to database</param>
        /// <param name="insertFileLog">Add entry to file log</param> 
        public async Task InsertException(string category, Exception exception, string comments="", LogLevel logLevel = LogLevel.Error, bool insertDBLog = true, bool insertFileLog = true)
        {
            if (insertDBLog || insertFileLog)
            {
                var stackTrace = String.IsNullOrEmpty(exception.StackTrace) ? "" : exception.StackTrace;
                string mensaje = ExcepcionesHelper.ObtenerMensajeCompletoExcepcion(exception);
                await InsertLog(category, mensaje, stackTrace, comments, logLevel, insertDBLog, insertFileLog);
            }
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
        public async Task InsertLog(string category, string message, string? stackTrace, string comments, LogLevel logLevel=LogLevel.Error, bool insertDBLog = true, bool insertFileLog = true)
        {
            if (insertDBLog || insertFileLog)
            {

                var usuario =  _usuarioServicio.UsuarioApp!.Usuario;

                var stackTraceText = String.IsNullOrEmpty(stackTrace) ? "" : stackTrace;
                if (insertDBLog)
                {
                    var data = new DatosPeticionLogData();
                    data.CodigoAplicacion = usuario.CodigoAplicacion;
                    data.CodigoPais = usuario.CodigoPais;
                    data.CodigoCompania = usuario.CodigoCompania;                   
                    data.UserName = usuario.Login;                    
                    data.Fecha = DateTime.Now;
                    data.Categoria = category;
                    data.Mensaje = message.Length > 1500 ? message[..1500] : message ?? string.Empty;
                    data.StackTrace = stackTraceText;
                    data.Observaciones = comments;
                    data.DominioAplicacion = _configuration.GetValue<string>("AppSettings:AppDomain");

                    await ApiCoreCli.SaveLog(usuario.Jwt, data);
                }
                if (insertFileLog)
                {
                    switch (logLevel)
                    {
                        case LogLevel.Info:
                            AddInfo(usuario.Login, category, message ?? string.Empty, stackTraceText, comments);
                            break;
                        case LogLevel.Message:
                            AddMessage(usuario.Login, category, message ?? string.Empty, stackTraceText, comments);
                            break;
                        case LogLevel.Warning:
                            AddWarning(usuario.Login, category, message ?? string.Empty, stackTraceText, comments);
                            break;
                        case LogLevel.Debug:
                            AddDebug(usuario.Login, category, message ?? string.Empty, stackTraceText, comments);
                            break;
                        case LogLevel.Error:
                            AddError(usuario.Login, category, message ?? string.Empty, stackTraceText, comments);
                            break;
                        case LogLevel.Trace:
                            AddTrace(usuario.Login, category, message ?? string.Empty, stackTraceText, comments);
                            break;
                        default:
                            break;
                    }
                    
                }
            }
        }

        #endregion



        #region Private Methods

        /// <summary>
        /// Get Logger object 
        /// </summary>
        /// <param name="extension">File extension</param>
        private Logger GetLogger(string extension = "log")
        {
            LogManager.Setup().LoadConfigurationFromFile("nlog.config");
            var obLogger = LogManager.GetCurrentClassLogger();
            var logDirectory = LogManager.Configuration.Variables["logDirectory"];
            var appName = LogManager.Configuration.Variables["appName"];
            var target = (FileTarget)LogManager.Configuration.FindTargetByName("fileLog");
            target.FileName = logDirectory+"\\" + appName + "_" + DateTime.Now.ToString("yyyy-MM-dd") + "." + extension;
            LogManager.ReconfigExistingLoggers(true);
            return obLogger;
        }

        /// <summary>
        /// Add info message to log
        /// </summary>
        /// <param name="userName">user name</param>
        /// <param name="category">Exception category</param>
        /// <param name="message">Exception message</param> 
        /// <param name="stackTrace">Exception stack trace</param>
        /// <param name="comments">Comments</param>
        private void AddInfo(string userName, string category, string message, string stackTrace, string comments)
        {
            var logger = GetLogger();
            logger.Info("[" + userName + "] > " + category + "\nMessage:\n" + message + "\n\nStackTrace:\n" + stackTrace + "\n\nComments:\n" + comments + "\n\n");
        }


        /// <summary>
        /// Add warning message to log
        /// </summary>
        /// <param name="userName">user name</param>
        /// <param name="category">Exception category</param>
        /// <param name="message">Exception message</param> 
        /// <param name="stackTrace">Exception stack trace</param>
        /// <param name="comments">Comments</param>
        private void AddWarning(string userName, string category, string message, string stackTrace, string comments)
        {
            var logger = GetLogger();
            logger.Warn("[" + userName + "] > " + category + "\nMessage:\n" + message + "\n\nStackTrace:\n" + stackTrace + "\n\nComments:\n" + comments + "\n\n");
        }


        /// <summary>
        /// Add error message to log
        /// </summary>
        /// <param name="userName">user name</param>
        /// <param name="category">Exception category</param>
        /// <param name="message">Exception message</param> 
        /// <param name="stackTrace">Exception stack trace</param>
        /// <param name="comments">Comments</param>
        /// <param name="extension">file extension</param>
        private void AddError(string userName, string category, string message, string stackTrace, string comments, string extension = "err")
        {
            var logger = GetLogger(extension);
            logger.Error("[" + userName +"] > " + category+ "\nMessage:\n" + message+ "\n\nStackTrace:\n" + stackTrace + "\n\nComments:\n" + comments+ "\n\n");
        }


        /// <summary>
        /// Add debug message to log
        /// </summary>
        /// <param name="userName">user name</param>
        /// <param name="category">Exception category</param>
        /// <param name="message">Exception message</param> 
        /// <param name="stackTrace">Exception stack trace</param>
        /// <param name="comments">Comments</param>
        private void AddDebug(string userName, string category, string message, string stackTrace, string comments)
        {
            var logger = GetLogger();
            logger.Debug("[" + userName + "] > " + category + "\nMessage:\n" + message + "\n\nStackTrace:\n" + stackTrace + "\n\nComments:\n" + comments + "\n\n");
        }


        /// <summary>
        /// Add trace message to log
        /// </summary>
        /// <param name="userName">user name</param>
        /// <param name="category">Exception category</param>
        /// <param name="message">Exception message</param> 
        /// <param name="stackTrace">Exception stack trace</param>
        /// <param name="comments">Comments</param>
        private void AddTrace(string userName, string category, string message, string stackTrace, string comments)
        {
            var logger = GetLogger();
            logger.Trace("[" + userName + "] > " + category + "\nMessage:\n" + message + "\n\nStackTrace:\n" + stackTrace + "\n\nComments:\n" + comments + "\n\n");
        }


        /// <summary>
        /// Add text message to log
        /// </summary>
        /// <param name="userName">user name</param>
        /// <param name="category">Exception category</param>
        /// <param name="message">Exception message</param> 
        /// <param name="stackTrace">Exception stack trace</param>
        /// <param name="comments">Comments</param>
        /// <returns></returns>
        private void AddMessage(string userName, string category, string message, string stackTrace, string comments)
        {
            var logger = GetLogger();
            logger.Info(message);
        }

        #endregion
    }
}
