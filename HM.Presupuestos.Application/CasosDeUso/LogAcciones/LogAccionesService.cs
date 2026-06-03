using HM.Presupuestos.Application.CasosDeUso.LogAcciones;
using HM.Core.Comun.v6.Seguridad.Interfaces;
using HM.Core.Comun.v6.Loggers.Interfaces;
using HM.Presupuestos.Domain.Puertos;
using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Domain.Entidades;
using System.Runtime.CompilerServices;
using HM.Presupuestos.Domain.Extensiones;

namespace HM.Presupuestos.Application.CasosDeUso.LogAcciones
{
    public class LogAccionesService(ILogger logger, IJwt jwt, ILogAccionesRepository logAccionesRepository, 
        IRegistroErroresCore registroErroresCore) : ILogAccionesService
    {
        private readonly ILogger _logger = logger;
        private readonly IJwt _jwt = jwt;
        private readonly ILogAccionesRepository _logAccionesRepository = logAccionesRepository;
        private readonly IRegistroErroresCore _registroErroresCore = registroErroresCore;

        private int CodigoUsuario => _jwt.Usuario.CodigoUsuario;

        /// <summary>
        /// Registra una acción de auditoría con un mensaje personalizado
        /// </summary>
        /// <param name="accion">Descripción textual de la acción realizada</param>
        /// <param name="parametros">Objeto opcional con parámetros adicionales que se serializarán a JSON</param>
        /// <param name="nombreMetodoLlamador">Nombre del método que invoca este log (se obtiene automáticamente con CallerMemberName)</param>
        /// <remarks>
        /// Este método serializa los parámetros a JSON y construye un mensaje de log con el formato:
        /// (NombreMetodo) -> Accion
        /// Si ocurre un error durante la inserción, se registra el error sin propagarlo
        /// </remarks>
        public async Task Insertar(string accion, object? parametros = null, [CallerMemberName] string nombreMetodoLlamador = "")
        {
            _logger.Trace($"Llamando método Insertar");
            try
            {
                string parametrosJson = parametros != null
                    ? System.Text.Json.JsonSerializer.Serialize(parametros)
                    : string.Empty;

                LogAccion logAccion = new()
                {
                    CodigoUsuario = CodigoUsuario,
                    Accion = $"({nombreMetodoLlamador}) -> {accion} ",
                    Parametros = parametrosJson
                };

                await _logAccionesRepository.Insertar(logAccion);
            }
            catch (Exception ex)
            {
                await InsertErrorLog(this.GetType().Name,ex, CodigoUsuario);
            }
        }


        /// <summary>
        /// Registra una acción de auditoría utilizando un enum de acciones predefinidas
        /// </summary>
        /// <param name="accion">Acción predefinida del enum AccionesLog</param>
        /// <param name="parametros">Objeto opcional con parámetros adicionales que se serializarán a JSON</param>
        /// <param name="nombreMetodoLlamador">Nombre del método que invoca este log (se obtiene automáticamente con CallerMemberName)</param>
        /// <remarks>
        /// Este método es similar a la sobrecarga con string pero usa un enum AccionesLog.
        /// La descripción se obtiene mediante el método de extensión ObtenerDescripcion() del enum.
        /// Formato del mensaje: (NombreMetodo) -> DescripcionAccion
        /// </remarks>
        public async Task Insertar(AccionesLog accion, object? parametros = null, [CallerMemberName] string nombreMetodoLlamador = "")
        {
            await Insertar(accion.ObtenerDescripcion(), parametros, nombreMetodoLlamador);
        }


        public async Task Insertar(LogAccion logAccion)
        {
            _logger.Trace($"Llamando método Insertar");
            try
            {
                await _logAccionesRepository.Insertar(logAccion);
            }
            catch (Exception ex)
            {
                await InsertErrorLog(this.GetType().Name, ex, logAccion.CodigoUsuario);
            }
        }


        private async Task InsertErrorLog(string methodName, Exception exception, int codigoUsuario)
        {
            var data = new DetalleError
            {
                UserName = codigoUsuario.ToString(),
                Mensaje = $"{methodName} > {exception.Message[..Math.Min(exception.Message.Length, 1500)]}",
                StackTrace = exception.StackTrace ?? string.Empty
            };

            await _registroErroresCore.RegistrarErrorSaveLog(data);
        }
    }
}



