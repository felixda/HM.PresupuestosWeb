using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using HM.Core.Comun.v6.Entidades.Logger;
using HM.Core.Comun.v6.Entidades.Configuracion;
using HM.Core.Comun.v6.Loggers.Interfaces;

namespace HM.Presupuestos.Infrastructure.Servicios
{
    public static class ApiCoreCli
    {
        private static ILogger? _logger = default!;
        private static string _urlBaseCore = string.Empty;

        public static void ConfigurarLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static void ConfigurarUrlBaseCore(string urlBase)
        {
            _urlBaseCore = urlBase;
        }


        public static async Task<string> ObtenerFavoritos(string jwtToken)
        {
            string apiUrl = $"{_urlBaseCore}/api/v6/core/Usuario/obtenervalorvariable?variable=MENU_FAVORITOS";

            using HttpClient client = new();
            string retorno;
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    retorno = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Respuesta de la API:");
                    Console.WriteLine(retorno);
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                    Console.WriteLine($"Mensaje: {await response.Content.ReadAsStringAsync()}");
                    throw (new Exception($"Error llamada API -> {response.StatusCode}"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción: {ex.Message}");
                throw;
            }
            return retorno;
        }

        public static async Task GrabarFavoritos(string jwtToken, ElementoConfiguracion postData)
        {
            string apiUrl = $"{_urlBaseCore}/api/v6/core/Usuario/guardarvalorvariable";

            using HttpClient client = new();
           
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                string jsonData = JsonSerializer.Serialize(postData);
                HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                _logger?.Trace($"LLamada a la API: {apiUrl}");

                // Verifica el resultado de la respuesta
                if (response.IsSuccessStatusCode)
                {
                    string retorno = await response.Content.ReadAsStringAsync();
                    _logger?.Trace($"Respuesta de la API:{retorno}");
                }
                else
                {
                    _logger?.Error($"Error: {response.StatusCode}");
                    _logger?.Error($"Mensaje: {await response.Content.ReadAsStringAsync()}");
                    throw (new Exception($"Error llamada API -> {response.StatusCode}"));
                }
            }
            catch (Exception ex)
            {
                _logger?.Error($"Excepción: {ex.Message}");
                throw;
            }
        }


        public static async Task SaveLog(string jwtToken, DatosPeticionLogData postData)
        {
            string apiUrl = $"{_urlBaseCore}/api/v6/core/Log/guardarlog";

            using HttpClient client = new();

            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                string jsonData = JsonSerializer.Serialize(postData);
                HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                _logger?.Trace($"LLamada a la API: {apiUrl}");

                // Verifica el resultado de la respuesta
                if (response.IsSuccessStatusCode)
                {
                    string retorno = await response.Content.ReadAsStringAsync();
                    _logger?.Trace($"Respuesta de la API:{retorno}");
                }
                else
                {
                    _logger?.Error($"Error: {response.StatusCode}");
                    _logger?.Error($"Mensaje: {await response.Content.ReadAsStringAsync()}");
                    throw (new Exception($"Error llamada API -> {response.StatusCode}"));
                }
            }
            catch (Exception ex)
            {
                _logger?.Error($"Excepción: {ex.Message}");
                throw;
            }
        }

    }
}
