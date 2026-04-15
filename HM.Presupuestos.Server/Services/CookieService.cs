using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace HM.Presupuestos.Server.Services
{
    public interface ICookieService
    {
        string? GetCookie(string key);
        void SetCookie(string key, string value, int? expireDays = null);
        Task SetCookieAsync(string key, string value, int? expireDays = null);
        void RemoveCookie(string key);
        Task RemoveCookieAsync(string key);
    }

    public class CookieService : ICookieService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CookieService> _logger;
        private readonly IJSRuntime? _jsRuntime;
        private readonly Dictionary<string, string> _pendingCookies = new Dictionary<string, string>();

        public CookieService(
            IHttpContextAccessor httpContextAccessor, 
            ILogger<CookieService> logger,
            IJSRuntime? jsRuntime = null)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _jsRuntime = jsRuntime;
        }

        public string? GetCookie(string key)
        {
            try
            {
                // Primero verificar cache en memoria
                if (_pendingCookies.TryGetValue(key, out var pendingValue))
                {
                    _logger.LogDebug("Cookie {Key} leída desde caché: {Value}", key, pendingValue);
                    return pendingValue;
                }

                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    _logger.LogWarning("HttpContext no disponible al leer cookie {Key}", key);
                    return null;
                }

                var value = httpContext.Request.Cookies[key];
                _logger.LogDebug("Cookie {Key} leída: {Value}", key, value ?? "null");
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al leer cookie {Key}", key);
                return null;
            }
        }

        public void SetCookie(string key, string value, int? expireDays = null)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    _logger.LogError("❌ HttpContext no disponible al establecer cookie {Key}", key);
                    _pendingCookies[key] = value;
                    return;
                }

                // Verificar si la respuesta ya ha comenzado
                if (httpContext.Response.HasStarted)
                {
                    _logger.LogWarning("⚠️ Respuesta HTTP ya iniciada. No se puede establecer cookie {Key} en el servidor.", key);
                    _pendingCookies[key] = value;
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

                if (expireDays.HasValue)
                {
                    options.Expires = DateTimeOffset.UtcNow.AddDays(expireDays.Value);
                }

                httpContext.Response.Cookies.Append(key, value, options);
                _pendingCookies[key] = value;
                
                _logger.LogInformation("✅ Cookie {Key} establecida en servidor: {Value}", key, value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al establecer cookie {Key}", key);
                _pendingCookies[key] = value;
            }
        }

        /// <summary>
        /// Establecer cookie usando JavaScript cuando la respuesta HTTP ya ha comenzado
        /// </summary>
        public async Task SetCookieAsync(string key, string value, int? expireDays = null)
        {
            try
            {
                if (_jsRuntime == null)
                {
                    _logger.LogWarning("JSRuntime no disponible, usando SetCookie estándar");
                    SetCookie(key, value, expireDays);
                    return;
                }

                _logger.LogDebug("Estableciendo cookie {Key} mediante JavaScript", key);

                var expiresString = expireDays.HasValue
                    ? $"; expires={DateTimeOffset.UtcNow.AddDays(expireDays.Value):R}"
                    : "";

                var cookieString = $"{key}={value}; path=/{expiresString}; SameSite=Lax";

                await _jsRuntime.InvokeVoidAsync("eval", $"document.cookie = '{cookieString}'");
                
                _pendingCookies[key] = value;
                _logger.LogInformation("✅ Cookie {Key} establecida mediante JavaScript: {Value}", key, value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al establecer cookie {Key} mediante JavaScript", key);
            }
        }

        public void RemoveCookie(string key)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null || httpContext.Response.HasStarted)
                {
                    _logger.LogWarning("No se puede eliminar cookie {Key} del servidor", key);
                    _pendingCookies.Remove(key);
                    return;
                }

                httpContext.Response.Cookies.Delete(key);
                _pendingCookies.Remove(key);
                
                _logger.LogDebug("Cookie {Key} eliminada", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar cookie {Key}", key);
            }
        }

        /// <summary>
        /// Eliminar cookie usando JavaScript cuando la respuesta HTTP ya ha comenzado
        /// </summary>
        public async Task RemoveCookieAsync(string key)
        {
            try
            {
                if (_jsRuntime == null)
                {
                    RemoveCookie(key);
                    return;
                }

                await _jsRuntime.InvokeVoidAsync("eval", 
                    $"document.cookie = '{key}=; path=/; expires=Thu, 01 Jan 1970 00:00:00 GMT'");
                
                _pendingCookies.Remove(key);
                _logger.LogDebug("Cookie {Key} eliminada mediante JavaScript", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar cookie {Key} mediante JavaScript", key);
            }
        }
    }
}