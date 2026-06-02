
using HM.Core.Comun.v6.Entidades.Configuracion;
using HM.Core.Comun.v6.Entidades.Logger;
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
        IConfiguration configuration) : IClienteApiCore
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<ClienteApiCore> _logger = logger;
        private readonly string _urlBaseApi = configuration["ServicioCore:UrlBase"] ?? string.Empty;

        /// <summary>
        /// Obtiene los codigos de menus favoritos del usuario desde la API de HM.CORE.
        /// </summary>
        public async Task<string> ObtenerCodigosDeMenusFavoritos(string jwtToken)
        {
            string urlEndpoint = $"{_urlBaseApi}/api/v6/core/Usuario/obtenervalorvariable?variable=MENU_FAVORITOS";

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, urlEndpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

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
        /// </summary>
        public async Task GuardarCodigosDeMenusFavoritos(string jwtToken, ElementoConfiguracion configuracionFavoritos)
        {
            string urlEndpoint = $"{_urlBaseApi}/api/v6/core/Usuario/guardarvalorvariable";
            await EnviarPostJsonAsync(jwtToken, urlEndpoint, configuracionFavoritos);
        }

        /// <summary>
        /// Envia una entrada de auditoria/log a la API de HM.CORE para su persistencia.
        /// </summary>
        public async Task RegistrarLog(string jwtToken, DatosPeticionLogData datosLog)
        {
            string urlEndpoint = $"{_urlBaseApi}/api/v6/core/Log/guardarlog";
            await EnviarPostJsonAsync(jwtToken, urlEndpoint, datosLog);
        }

        /// <summary>
        /// Realiza una peticion POST autenticada a la API de HM.CORE serializando el cuerpo como JSON.
        /// El header de autorización se establece por petición para evitar problemas con HttpClient compartido.
        /// </summary>
        private async Task EnviarPostJsonAsync<T>(string jwtToken, string urlEndpoint, T cuerpo)
        {
            try
            {
                string json = JsonSerializer.Serialize(cuerpo);
                var request = new HttpRequestMessage(HttpMethod.Post, urlEndpoint)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

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
