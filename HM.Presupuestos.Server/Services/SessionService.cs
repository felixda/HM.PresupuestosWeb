using HM.Core.Comun.v6.Entidades.Seguridad;
using HM.Presupuestos.Domain.Comun;
using HM.Presupuestos.Server.Helper;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;
using System.Text.Json;


namespace HM.Presupuestos.Server.Services
{

    public interface ISessionService
    {
        Task EstablecerUsuarioSesion(UsuarioEntidad userEntity);
        Task<UsuarioEntidad> ObtenerUsuarioSesion();
        Task EliminarUsuarioSesion();
        Task EstablecerCodigoIdioma(string codigoIdioma); 
        Task<string> ObtenerCodigoIdimoa();


        Task<UsuarioEntidad> ObtenerUsuarioSesionSSO();
        Task EstablecerUsuarioSesionSSO(UsuarioEntidad usuario);

        Task EstablecerUsuarioSesionLogin(UsuarioEntidad usuario);
        Task<UsuarioEntidad?> ObtenerUsuarioSesionLogin();

        Task EliminarUsuarioSesionLogin();


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
    public class SessionService : ISessionService
    {
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly ILogger<SessionService> _logger;

        public SessionService(
            ProtectedSessionStorage sessionStorage,
            ILogger<SessionService> logger)
        {
            _sessionStorage = sessionStorage;
            _logger = logger;
        }

        /// <summary>
        /// Guarda el usuario con cifrado automático.
        /// Persiste entre recargas (F5) pero se limpia al cerrar navegador.
        /// </summary>
        public async Task EstablecerUsuarioSesion(UsuarioEntidad userEntity)
        {
            try
            {
                if (userEntity == null)
                {
                    await EliminarUsuarioSesion();
                    return;
                }

                var json = JsonSerializer.Serialize(userEntity);
                await _sessionStorage.SetAsync(Constantes.Session.USER, json);
                
                _logger.LogDebug("Usuario guardado: {Login}", userEntity.Login);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar sesión");
            }
        }

        /// <summary>
        /// Recupera el usuario con descifrado automático.
        /// </summary>
        public async Task<UsuarioEntidad> ObtenerUsuarioSesion()
        {
            try
            {
                var result = await _sessionStorage.GetAsync<string>(Constantes.Session.USER);

                if (!result.Success || string.IsNullOrEmpty(result.Value))
                {
                    return new UsuarioEntidad();
                }

                var usuario = JsonSerializer.Deserialize<UsuarioEntidad>(result.Value);
                return usuario ?? new UsuarioEntidad();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al recuperar sesión");
                return new UsuarioEntidad();
            }
        }

        public async Task EliminarUsuarioSesion()
        {
            try
            {
                await _sessionStorage.DeleteAsync(Constantes.Session.USER);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar sesión");
            }
        }

        /// <summary>
        /// Guarda el código de idioma del usuario con cifrado automático.
        /// Persiste entre recargas (F5) pero se limpia al cerrar navegador.
        /// </summary>
        public async Task EstablecerCodigoIdioma(string languageCode)
        {
            try
            {
                if (string.IsNullOrEmpty(languageCode))
                {
                    _logger.LogWarning("Intento de guardar idioma vacío");
                    return;
                }

                await _sessionStorage.SetAsync(Constantes.LocalStorage.LANGUAGE_CODE, languageCode);

                _logger.LogDebug("Idioma guardado: {LanguageCode}", languageCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar idioma");
            }
        }

        /// <summary>
        /// Recupera el código de idioma del usuario con descifrado automático.
        /// </summary>
        public async Task<string> ObtenerCodigoIdimoa()
        {
            try
            {
                var result = await _sessionStorage.GetAsync<string>(Constantes.LocalStorage.LANGUAGE_CODE);

                if (!result.Success || string.IsNullOrEmpty(result.Value))
                {
                    _logger.LogDebug("No se encontró idioma en sesión");
                    return string.Empty;
                }

                _logger.LogDebug("Idioma recuperado: {LanguageCode}", result.Value);
                return result.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al recuperar idioma");
                return string.Empty;
            }
        }



        public async Task EstablecerUsuarioSesionLogin(UsuarioEntidad userEntity)
        {
            await SetItemAsync("UsuarioLogin", JsonSerializer.Serialize(userEntity));
        }

        public async Task EstablecerUsuarioSesionSSO(UsuarioEntidad userEntity)
        {
            await SetItemAsync("UsuarioSSO", JsonSerializer.Serialize(userEntity));
        }

        /// <summary>
        /// Get user Session value
        /// </summary>
        /// <returns>UsuarioEntidad object</returns>
        public async Task<UsuarioEntidad> ObtenerUsuarioSesionSSO()
        {
            var result = new UsuarioEntidad();
            try
            {
                var userJson = await GetItemAsync("UsuarioSSO");
                if (!String.IsNullOrEmpty(userJson))
                    result = JsonSerializer.Deserialize<UsuarioEntidad>(userJson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario SSO de sesión");
            }
            return result;
        }


        public async Task<UsuarioEntidad> ObtenerUsuarioSesionLogin()
        {
            UsuarioEntidad? result = null;
            try
            {
                var userJson = await GetItemAsync("UsuarioLogin");
                if (!String.IsNullOrEmpty(userJson))
                {
                    result = JsonSerializer.Deserialize<UsuarioEntidad>(userJson);
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }


        public async Task EliminarUsuarioSesionLogin()
        {
            await _sessionStorage.DeleteAsync("UsuarioLogin");
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
        private async Task<string> GetItemAsync(string key)
        {
            var result = await _sessionStorage.GetAsync<string>(key);
            return result.Success ? result.Value : null;
        }


        #endregion
    }
}
