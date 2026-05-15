using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Text.Json;


namespace HM.Presupuestos.Server.Services
{
    public interface IAlmacenSesionUsuario
    {
        /// <summary>
        /// Obtiene la información del usuario autenticado mediante SSO de Azure AD de la sesión protegida del navegador.
        /// </summary>
        /// <remarks>Utilice este método para acceder a los detalles del usuario actual tras una
        /// autenticación SSO exitosa. El resultado puede incluir información como identificadores, roles y permisos
        /// asociados al usuario.</remarks>
        /// <returns>Un objeto <see cref="UsuarioEntidad"/> que contiene los datos del usuario autenticado.</returns>
        Task<UsuarioEntidad> ObtenerUsuarioSSO();

        /// <summary>
        /// Guarda la información de un usuario autenticado mediante SSO en la sesion protegida del navegador. 
        /// Este método se utiliza para almacenar los datos del usuario después de una autenticación SSO exitosa, permitiendo que la aplicación acceda a esta información durante la sesión del usuario.
        /// </summary>
        /// <param name="usuario">La entidad de usuario que contiene los datos a almacenar. No puede ser null.</param>
        /// <returns>Una tarea que representa la operación asincrónica de guardado.</returns>
        Task GuardarUsuarioSSO(UsuarioEntidad usuario);
        
        /// <summary>
        /// Guarda la información del usuario que se está suplantando para su uso en la sesión actual.
        /// </summary>
        /// <param name="usuario">La entidad de usuario que se va a guardar como usuario suplantado. No puede ser null.</param>
        /// <returns>Una tarea que representa la operación asincrónica de guardado.</returns>
        Task GuardarUsuarioImpersonado(UsuarioEntidad usuario);

        /// <summary>
        /// Obtiene la información del usuario actualmente impersonado de la sesión protegida del navegador. 
        /// Este método se utiliza para acceder a los detalles del usuario que ha sido impersonado, 
        /// lo que puede ser útil en escenarios donde un administrador o un sistema necesita actuar en nombre de otro usuario.
        /// </summary>
        /// <returns>Un objeto <see cref="UsuarioEntidad"/> que contiene los datos del usuario impersonado, o null si no hay usuario impersonado.</returns>
        Task<UsuarioEntidad?> ObtenerUsuarioImpersonado();

        /// <summary>
        /// Elimina la información del usuario impersonado de la sesión protegida del navegador.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica de eliminación.</returns>
        Task EliminarUsuarioImpersonado();
    }

    /// <summary>
    /// Servicio para gestionar la sesión del usuario usando ProtectedSessionStorage.
    /// 
    /// VENTAJAS vs IMemoryCache:
    /// ? Persiste con F5 (NO se pierde al recargar)
    /// ? Más simple (menos código)
    /// ? Cifrado automático
    /// ? Bajo consumo de memoria
    /// ? Escala mejor
    /// </summary>
    public class AlmacenSesionUsuario : IAlmacenSesionUsuario
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly ILogger<AlmacenSesionUsuario> _logger;

        public AlmacenSesionUsuario(
            ProtectedSessionStorage sessionStorage,
            ILogger<AlmacenSesionUsuario> logger)
        {
            _sessionStorage = sessionStorage;
            _logger = logger;
        }

        public async Task GuardarUsuarioImpersonado(UsuarioEntidad usuario)
        {
            await SetItemAsync(Constantes.Session.USER_LOGIN, JsonSerializer.Serialize(usuario));
        }

        public async Task GuardarUsuarioSSO(UsuarioEntidad usuario)
        {
            await SetItemAsync(Constantes.Session.USER_SSO, JsonSerializer.Serialize(usuario));
        }

        public async Task<UsuarioEntidad> ObtenerUsuarioSSO()
        {
            var result = new UsuarioEntidad();
            try
            {
                var userJson = await GetItemAsync(Constantes.Session.USER_SSO);
                if (!String.IsNullOrEmpty(userJson))
                    result = JsonSerializer.Deserialize<UsuarioEntidad>(userJson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario SSO de sesión");
            }
            return result;
        }


        public async Task<UsuarioEntidad?> ObtenerUsuarioImpersonado()
        {
            UsuarioEntidad? result = null;
            try
            {
                var userJson = await GetItemAsync(Constantes.Session.USER_LOGIN);
                if (!String.IsNullOrEmpty(userJson))
                {
                    result = JsonSerializer.Deserialize<UsuarioEntidad>(userJson);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario Impersonado de sesión");

            }
            return result;
        }


        public async Task EliminarUsuarioImpersonado()
        {
            await _sessionStorage.DeleteAsync(Constantes.Session.USER_LOGIN);
        }


        #region Private Methods

        /// <summary>
        /// Set item Session Storage value
        /// </summary>
        /// <param name="key">Session key</param>
        /// <param name="value">Session value</param>
        private async Task SetItemAsync(string key, string value)
        {
            await _sessionStorage.SetAsync(key, value);
        }

        /// <summary>
        /// Get item Session Storage value
        /// </summary>
        /// <param name="key">Session key</param>
        /// <returns>Session value</returns>
        private async Task<string?> GetItemAsync(string key)
        {
            var result = await _sessionStorage.GetAsync<string>(key);
            return result.Success ? result.Value : null;
        }


        #endregion
    }
}
