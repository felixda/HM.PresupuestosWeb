using HM.Presupuestos.Web.Adaptadores.Api.Modelos;
using HM.Core.Comun.v6.Seguridad.Interfaces;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace HM.Presupuestos.Web.Adaptadores.Api;

public class MaestrosApiService(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    IJwt jwt,
    ILogger<MaestrosApiService> logger) : IMaestrosApiService
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IConfiguration _configuration = configuration;
    private readonly IJwt _jwt = jwt;
    private readonly ILogger<MaestrosApiService> _logger = logger;

    public Task<List<MaestroApiItem>> ObtenerTipologias()
        => ObtenerAsync("api/v1/maestros/tipologias");

    public Task<List<MaestroApiItem>> ObtenerNetworks()
        => ObtenerAsync("api/v1/maestros/networks");

    public Task<List<MaestroApiItem>> ObtenerMedios()
        => ObtenerAsync("api/v1/maestros/medios");

    public Task<List<MaestroApiItem>> ObtenerGruposClientes()
        => ObtenerAsync("api/v1/maestros/grupos-clientes");

    private async Task<List<MaestroApiItem>> ObtenerAsync(string endpoint)
    {
        try
        {
            using var client = CrearClient();
            using var response = await client.GetAsync(endpoint);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Error llamando al endpoint {Endpoint}. StatusCode: {StatusCode}. Response: {ResponseBody}",
                    endpoint,
                    (int)response.StatusCode,
                    responseBody);

                throw new HttpRequestException(
                    $"La API respondió {(int)response.StatusCode} ({response.ReasonPhrase}) en '{endpoint}'. Detalle: {responseBody}",
                    null,
                    response.StatusCode);
            }

            if (string.IsNullOrWhiteSpace(responseBody))
            {
                return [];
            }

            var resultado = JsonSerializer.Deserialize<List<MaestroApiItem>>(
                responseBody,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return resultado ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error llamando al endpoint {Endpoint}", endpoint);
            throw;
        }
    }

    private HttpClient CrearClient()
    {
        var baseUrl = _configuration["ApiRest:BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException("Falta configurar ApiRest:BaseUrl en appsettings.");
        }

        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(baseUrl);

        var token = _jwt.Usuario?.Jwt;
        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }
}
