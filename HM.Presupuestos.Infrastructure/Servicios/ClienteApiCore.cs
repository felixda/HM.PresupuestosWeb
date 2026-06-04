ï»¿
using HM.Core.Comun.v6.Entidades.Configuracion;
using HM.Core.Comun.v6.Entidades.Logger;
using HM.Core.Comun.v6.Seguridad.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HM.Presupuestos.Infrastructure.Servicios
{
    public sealed class ClienteApiCore(
        HttpClient httpClient,
        ILogger<ClienteApiCore> logger,
        IConfiguration configuration,
        IJwt jwt) : IClienteApiCore
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<ClienteApiCore> _logger = logger;
        private readonly IJwt _jwt = jwt;
        private readonly string _urlBaseApi = configuration["ServicioCore:UrlBase"] ?? string.Empty;

        private string JwtUsuarioActual =>
            _jwt.Usuario?.Jwt
            ?? throw new InvalidOperationException("No hay un usuario autenticado con token JWT vÃ¡lido.");

        /// <summary>
        /// Obtiene los codigos de menus favoritos del usuario desde la API de HM.CORE.
        /// El token JWT se obtiene del usuario autenticado en el circuito actual.
        /// </summary>
        public async Task<string> ObtenerCodigosDeMenusFavoritos()
        {
            string urlEndpoint = $"{_urlBaseApi}/api/v6/core/Usuario/obtenervalorvariable?variable=MENU_FAVORITOS";

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, urlEndpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JwtUsuarioActual);

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string contenidoRespuesta = await response.Content.ReadAsStringAsync();
                    _logger.LogTrace("Respuesta de la API: {Contenido}", contenidoRespuesta);
                    return contenidoRespuesta;
                }

                _logger.LogError("Error: {StatusCode}", response.StatusCode);
                _logger.LogError("Mensaje: {Mensaje}", await response.Content.ReadAsStringAsync());
                throw new Exception($"Error llamada API -> {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepcion en ObtenerCodigosDeMenusFavoritos");
                throw;
            }
        }

        /// <summary>
        /// Persiste en la API de HM.CORE los codigos de menus favoritos del usuario.
        /// El token JWT se obtiene del usuario autenticado en el circuito actual.
        /// </summary>
        public async Task GuardarCodigosDeMenusFavoritos(ElementoConfiguracion configuracionFavoritos)
        {
            string urlEndpoint = $"{_urlBaseApi}/api/v6/core/Usuario/guardarvalorvariable";
            await EnviarPostJsonAsync(urlEndpoint, configuracionFavoritos);
        }

        /// <summary>
        /// Envia una entrada de auditoria/log a la API de HM.CORE para su persistencia.
        /// </summary>
        /// <param name="jwtUsuario">
        /// Token JWT del usuario autenticado. Se recibe explÃ­citamente en lugar de obtenerlo
        /// del servicio <see cref="IJwt"/> inyectado porque este mÃ©todo puede ser invocado durante
        /// el flujo de autenticaciÃ³n inicial (ej: desde <c>RegistroAplicacion</c>), momento en el
        /// que <c>IJwt.Usuario</c> todavÃ­a es null. Pasar el token desde el caller â€”que ya dispone
        /// del usuario cargadoâ€” evita la <see cref="InvalidOperationException"/> que de otro modo
        /// redirigirÃ­a a <c>/Unauthorized</c>.
        /// </param>
        /// <param name="datosLog">Datos de la acciÃ³n a registrar (acciÃ³n, parÃ¡metros, usuario, etc.).</param>
        public async Task RegistrarLog(string jwtUsuario, DatosPeticionLogData datosLog)
        {
            string urlEndpoint = $"{_urlBaseApi}/api/v6/core/Log/guardarlog";
            await EnviarPostJsonAsync(urlEndpoint, datosLog, jwtUsuario);
        }

        /// <summary>
        /// Realiza una peticion POST autenticada a la API de HM.CORE serializando el cuerpo como JSON.
        /// Si no se proporciona token explÃ­cito, lo obtiene del usuario autenticado en el circuito actual.
        /// El header de autorizaciÃ³n se establece por peticiÃ³n para evitar problemas con HttpClient compartido.
        /// </summary>
        private async Task EnviarPostJsonAsync<T>(string urlEndpoint, T cuerpo, string? jwtUsuario = null)
        {
            string tokenAutenticacion = jwtUsuario ?? JwtUsuarioActual;
            try
            {
                string json = JsonSerializer.Serialize(cuerpo);
                var request = new HttpRequestMessage(HttpMethod.Post, urlEndpoint)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenAutenticacion);

                _logger.LogTrace("Llamada a la API: {Url}", urlEndpoint);

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string contenidoRespuesta = await response.Content.ReadAsStringAsync();
                    _logger.LogTrace("Respuesta de la API: {Contenido}", contenidoRespuesta);
                    return;
                }

                _logger.LogError("Error: {StatusCode}", response.StatusCode);
                _logger.LogError("Mensaje: {Mensaje}", await response.Content.ReadAsStringAsync());
                throw new Exception($"Error llamada API -> {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepcion en EnviarPostJsonAsync hacia {Url}", urlEndpoint);
                throw;
            }
        }
    }
}
