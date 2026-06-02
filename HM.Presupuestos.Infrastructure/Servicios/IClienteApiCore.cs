using HM.Core.Comun.v6.Entidades.Configuracion;
using HM.Core.Comun.v6.Entidades.Logger;

namespace HM.Presupuestos.Infrastructure.Servicios
{
    public interface IClienteApiCore
    {
        /// <summary>
        /// Obtiene los códigos de menús favoritos del usuario desde la API de HM.CORE.
        /// El token JWT se obtiene internamente del usuario autenticado en el circuito actual.
        /// </summary>
        /// <returns>Cadena con los códigos de menú favoritos separados por coma.</returns>
        Task<string> ObtenerCodigosDeMenusFavoritos();

        /// <summary>
        /// Persiste en la API de HM.CORE los códigos de menús favoritos del usuario.
        /// El token JWT se obtiene internamente del usuario autenticado en el circuito actual.
        /// </summary>
        /// <param name="configuracionFavoritos">Objeto con los códigos favoritos a guardar.</param>
        Task GuardarCodigosDeMenusFavoritos(ElementoConfiguracion configuracionFavoritos);

        /// <summary>
        /// Envía una entrada de auditoría/log a la API de HM.CORE para su persistencia.
        /// </summary>
        /// <param name="jwtUsuario">
        /// Token JWT del usuario autenticado. Se recibe explícitamente en lugar de obtenerlo
        /// del servicio <see cref="IJwt"/> inyectado porque este método puede invocarse durante
        /// el flujo de autenticación inicial (ej: desde <c>RegistroAplicacion</c>), momento en el
        /// que <c>IJwt.Usuario</c> todavía es null. Pasar el token desde el caller —que ya dispone
        /// del usuario cargado— evita una <see cref="InvalidOperationException"/> que redirigiría a
        /// <c>/Unauthorized</c>.
        /// </param>
        /// <param name="datosLog">Datos de la acción a registrar (acción, parámetros, usuario, etc.).</param>
        Task RegistrarLog(string jwtUsuario, DatosPeticionLogData datosLog);
    }
}
