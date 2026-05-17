using HM.Presupuestos.Server.Adaptadores.Sesion;
using HM.Core.Comun.v6.Entidades.Logger;
using HM.Presupuestos.Infrastructure.Servicios;
using NLog;
using NLog.Targets;
using System.Runtime.CompilerServices;

namespace HM.Presupuestos.Server.Adaptadores.Ui
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
        Task RegistrarAccesoAPagina( string tituloPagina);

        Task RegistrarIntentoAccesoNoAutorizado( string ruta);
    }

    public class RegistroAplicacion:IRegistroAplicacion
    {
        #region Propiedades privadas

        private readonly IConfiguration _configuration;
        private readonly IAlmacenSesionUsuario _almacenSesionService;
        private readonly ISesionUsuario _sesionUsuario;
        private readonly IRutasNavegacion _rutasNavegacion;
        private readonly ILogAccionesService _logAccionesService;

        #endregion

        #region Constructor

        public RegistroAplicacion(IConfiguration configuracion, 
            IAlmacenSesionUsuario almacenSesionUsuario, 
            ISesionUsuario sesionUsuario, 
            IRutasNavegacion rutasNavegacion, 
            ILogAccionesService logAccionesService )
        {
            _configuration = configuracion;
            _almacenSesionService = almacenSesionUsuario;
            _sesionUsuario = sesionUsuario;
            _rutasNavegacion = rutasNavegacion;
            _logAccionesService = logAccionesService;
        }

        #endregion


        #region Public Methods

        public async Task RegistrarAccesoAPagina(string tituloPagina)
        {
            var urlActual = _rutasNavegacion.ObtenerRutaActual();
            var accion = $"Acceso a página {tituloPagina} [{urlActual}]";
            await _logAccionesService.Insertar(accion);
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
            // Extraer nombre de la clase desde el FilePath
            var category = ExtraerNombreClaseDesdeFilePath(callerFilePath);
            // Llamar al método original
            await RegistrarExcepcion(category, exception, comments, logLevel, insertDBLog, insertFileLog);
        }

        /// <summary>
        /// Extrae el nombre de la clase desde el CallerFilePath
        /// </summary>
        /// <param name="filePath">Ruta completa del archivo (ej: C:\Proyectos\...\MainLayout.razor.cs)</param>
        /// <returns>Nombre de la clase (ej: MainLayout)</returns>
        private static string ExtraerNombreClaseDesdeFilePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return "Unknown";

            // Obtener nombre del archivo sin extensión
            var fileName = Path.GetFileNameWithoutExtension(filePath);

            // Si termina en .razor (archivos .razor.cs), eliminar ese sufijo
            if (fileName.EndsWith(".razor", StringComparison.OrdinalIgnoreCase))
                fileName = fileName[..^6]; // Eliminar ".razor"

            return fileName;
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

            var usuario =  _sesionUsuario.UsuarioApp!.UsuarioActivo;
            var stackTraceText = stackTrace ?? string.Empty;

            if (insertDBLog)
            {
                await RegistrarEnBaseDatos(category, message, stackTraceText, comments, usuario);
            }

            if (insertFileLog)
            {
                RegistrarEnArchivo(logLevel, usuario.Login, category, message, stackTraceText, comments);
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

            await ApiCoreCli.SaveLog(usuario.Jwt, data);
        }


        /// <summary>
        /// Inserta log en archivo según nivel
        /// </summary>
        private void RegistrarEnArchivo(NivelRegistro logLevel, string userName, string category, string message, string stackTraceText, string comments)
        {
            var extension = logLevel == NivelRegistro.Error ? "err" : "log";
            var logger = ObtenerLogger(extension);
            var entrada = $"[{userName}] > {category}\nMessage:\n{message}\n\nStackTrace:\n{stackTraceText}\n\nComments:\n{comments}\n\n";

            switch (logLevel)
            {
                case NivelRegistro.Info:
                    logger.Info(entrada);
                    break;
                case NivelRegistro.Message:
                    logger.Info(message);
                    break;
                case NivelRegistro.Warning:
                    logger.Warn(entrada);
                    break;
                case NivelRegistro.Debug:
                    logger.Debug(entrada);
                    break;
                case NivelRegistro.Error:
                    logger.Error(entrada);
                    break;
                case NivelRegistro.Trace:
                    logger.Trace(entrada);
                    break;
            }
        }


        /// <summary>
        /// Obtener instancia de logger configurada para el tipo de log (log o err)
        /// </summary>
        /// <param name="extension">Extensión del archivo de log (log o err)</param>
        /// <returns>Instancia de Logger configurada</returns>
        private static Logger ObtenerLogger(string extension = "log")
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

        #endregion
    }
}




