using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace HM.Presupuestos.Server.Services
{
    public interface IGestorCookies
    {
        string? Obtener(string clave);
        void Grabar(string clave, string valor, int? DiasExpiracion = null);
        Task GrabarAsync(string clave, string valor, int? DiasExpiracion = null);
        void Eliminar(string clave);
        Task EliminarAsync(string clave);
    }

    public class GestorCookies : IGestorCookies
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GestorCookies> _logger;
        private readonly IJSRuntime? _jsRuntime;
        private readonly Dictionary<string, string> _cookiesPendientes = [];

        public GestorCookies(
            IHttpContextAccessor httpContextAccessor, 
            ILogger<GestorCookies> logger,
            IJSRuntime? jsRuntime = null)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _jsRuntime = jsRuntime;
        }

        public string? Obtener(string clave)
        {
            try
            {
                // Primero verificar cache en memoria
                if (_cookiesPendientes.TryGetValue(clave, out var pendingValue))
                {
                    _logger.LogDebug("Cookie {Key} leída desde caché: {Value}", clave, pendingValue);
                    return pendingValue;
                }

                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    _logger.LogWarning("HttpContext no disponible al leer cookie {Key}", clave);
                    return null;
                }

                var value = httpContext.Request.Cookies[clave];
                _logger.LogDebug("Cookie {Key} leída: {Value}", clave, value ?? "null");
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al leer cookie {Key}", clave);
                return null;
            }
        }

        public void Grabar(string clave, string valor, int? diasExpiracion = null)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    _logger.LogError("❌ HttpContext no disponible al establecer cookie {Key}", clave);
                    _cookiesPendientes[clave] = valor;
                    return;
                }

                // Verificar si la respuesta ya ha comenzado
                if (httpContext.Response.HasStarted)
                {
                    _logger.LogWarning("⚠️ Respuesta HTTP ya iniciada. No se puede establecer cookie {Key} en el servidor.", clave);
                    _cookiesPendientes[clave] = valor;
                    return;
                }

                var options = new CookieOptions
                {
                    HttpOnly = false,
                    Secure = httpContext.Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    IsEssential = true,
                    Path = "/"
                };

                if (diasExpiracion.HasValue)
                {
                    options.Expires = DateTimeOffset.UtcNow.AddDays(diasExpiracion.Value);
                }

                httpContext.Response.Cookies.Append(clave, valor, options);
                _cookiesPendientes[clave] = valor;
                
                _logger.LogInformation("✅ Cookie {Key} establecida en servidor: {Value}", clave, valor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al establecer cookie {Key}", clave);
                _cookiesPendientes[clave] = valor;
            }
        }

        /// <summary>
        /// Establecer cookie usando JavaScript cuando la respuesta HTTP ya ha comenzado
        /// </summary>
        public async Task GrabarAsync(string key, string valor, int? diasExpiracion = null)
        {
            try
            {
                if (_jsRuntime == null)
                {
                    _logger.LogWarning("JSRuntime no disponible, usando SetCookie estándar");
                    Grabar(key, valor, diasExpiracion);
                    return;
                }

                _logger.LogDebug("Estableciendo cookie {Key} mediante JavaScript", key);

                var expiresString = diasExpiracion.HasValue
                    ? $"; expires={DateTimeOffset.UtcNow.AddDays(diasExpiracion.Value):R}"
                    : "";

                var cookieString = $"{key}={valor}; path=/{expiresString}; SameSite=Lax";

                await _jsRuntime.InvokeVoidAsync("eval", $"document.cookie = '{cookieString}'");
                
                _cookiesPendientes[key] = valor;
                _logger.LogInformation("✅ Cookie {Key} establecida mediante JavaScript: {Value}", key, valor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al establecer cookie {Key} mediante JavaScript", key);
            }
        }

        public void Eliminar(string valor)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null || httpContext.Response.HasStarted)
                {
                    _logger.LogWarning("No se puede eliminar cookie {Key} del servidor", valor);
                    _cookiesPendientes.Remove(valor);
                    return;
                }

                httpContext.Response.Cookies.Delete(valor);
                _cookiesPendientes.Remove(valor);
                
                _logger.LogDebug("Cookie {Key} eliminada", valor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar cookie {Key}", valor);
            }
        }

        /// <summary>
        /// Eliminar cookie usando JavaScript cuando la respuesta HTTP ya ha comenzado
        /// </summary>
        public async Task EliminarAsync(string valor)
        {
            try
            {
                if (_jsRuntime == null)
                {
                    Eliminar(valor);
                    return;
                }

                await _jsRuntime.InvokeVoidAsync("eval", 
                    $"document.cookie = '{valor}=; path=/; expires=Thu, 01 Jan 1970 00:00:00 GMT'");
                
                _cookiesPendientes.Remove(valor);
                _logger.LogDebug("Cookie {Key} eliminada mediante JavaScript", valor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar cookie {Key} mediante JavaScript", valor);
            }
        }
    }
}