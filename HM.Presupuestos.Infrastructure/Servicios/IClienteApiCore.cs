using HM.Core.Comun.v6.Entidades.Configuracion;
using HM.Core.Comun.v6.Entidades.Logger;

namespace HM.Presupuestos.Infrastructure.Servicios
{
    public interface IClienteApiCore
    {
        /// <summary>
        /// Obtiene los cÃ³digos de menÃºs favoritos del usuario desde la API de HM.CORE.
        /// El token JWT se obtiene internamente del usuario autenticado en el circuito actual.
        /// </summary>
        /// <returns>Cadena con los cÃ³digos de menÃº favoritos separados por coma.</returns>
        Task<string> ObtenerCodigosDeMenusFavoritos();

        /// <summary>
        /// Persiste en la API de HM.CORE los cÃ³digos de menÃºs favoritos del usuario.
        /// El token JWT se obtiene internamente del usuario autenticado en el circuito actual.
        /// </summary>
        /// <param name="configuracionFavoritos">Objeto con los cÃ³digos favoritos a guardar.</param>
        Task GuardarCodigosDeMenusFavoritos(ElementoConfiguracion configuracionFavoritos);

        /// <summary>
        /// EnvÃ­a una entrada de auditorÃ­a/log a la API de HM.CORE para su persistencia.
        /// </summary>
        /// <param name="jwtUsuario">
        /// Token JWT del usuario autenticado. Se recibe explÃ­citamente en lugar de obtenerlo
        /// del servicio <see cref="IJwt"/> inyectado porque este mÃ©todo puede invocarse durante
        /// el flujo de autenticaciÃ³n inicial (ej: desde <c>RegistroAplicacion</c>), momento en el
        /// que <c>IJwt.Usuario</c> todavÃ­a es null. Pasar el token desde el caller â€”que ya dispone
        /// del usuario cargadoâ€” evita una <see cref="InvalidOperationException"/> que redirigirÃ­a a
        /// <c>/Unauthorized</c>.
        /// </param>
        /// <param name="datosLog">Datos de la acciÃ³n a registrar (acciÃ³n, parÃ¡metros, usuario, etc.).</param>
        Task RegistrarLog(string jwtUsuario, DatosPeticionLogData datosLog);
    }
}
