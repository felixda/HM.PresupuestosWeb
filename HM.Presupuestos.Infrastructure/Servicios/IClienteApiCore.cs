using HM.Core.Comun.v6.Entidades.Configuracion;
using HM.Core.Comun.v6.Entidades.Logger;

namespace HM.Presupuestos.Infrastructure.Servicios
{
    public interface IClienteApiCore
    {
        /// <summary>
        /// Obtiene los códigos de menús favoritos del usuario desde la API de HM.CORE.
        /// </summary>
        /// <param name="jwtToken">Token JWT del usuario autenticado.</param>
        /// <returns>Cadena con los códigos de menú favoritos separados por coma.</returns>
        Task<string> ObtenerCodigosDeMenusFavoritos(string jwtToken);

        /// <summary>
        /// Persiste en la API de HM.CORE los códigos de menús favoritos del usuario.
        /// </summary>
        /// <param name="jwtToken">Token JWT del usuario autenticado.</param>
        /// <param name="codigosFavoritos">Objeto con los códigos favoritos a guardar.</param>
        Task GuardarCodigosDeMenusFavoritos(string jwtToken, ElementoConfiguracion configuracionFavoritos);

        /// <summary>
        /// Envía una entrada de auditoría/log a la API de HM.CORE para su persistencia.
        /// </summary>
        /// <param name="jwtToken">Token JWT del usuario autenticado.</param>
        /// <param name="datosLog">Datos de la acción a registrar (acción, parámetros, usuario, etc.).</param>
        Task RegistrarLog(string jwtToken, DatosPeticionLogData datosLog);
    }
}
