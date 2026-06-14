using HM.Presupuestos.E2ETest.Base;
using Microsoft.Playwright;

namespace HM.Presupuestos.E2ETest.Tests;

[TestFixture]
[Category("Inicio")]
public class InicioTests : E2ETestBase
{
    [Test]
    [Description("La aplicaciÃ³n responde y devuelve una pÃ¡gina HTML vÃ¡lida")]
    public async Task Aplicacion_Responde_ConHtmlValido()
    {
        var response = await Page.GotoAsync(BaseUrl);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.That(response, Is.Not.Null);
        Assert.That(response!.Status, Is.LessThan(500), "La aplicaciÃ³n devolviÃ³ un error de servidor");

        var content = await Page.ContentAsync();
        Assert.That(content, Is.Not.Empty, "El contenido de la pÃ¡gina estÃ¡ vacÃ­o");
    }

    [Test]
    [Description("La pÃ¡gina tiene un tÃ­tulo definido")]
    public async Task PaginaInicio_TieneTitle()
    {
        await Page.GotoAsync(BaseUrl);

        // Esperar a que Blazor termine de renderizar
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var titulo = await Page.TitleAsync();
        Assert.That(titulo, Is.Not.Empty, "La pÃ¡gina no tiene tÃ­tulo");
    }

    [Test]
    [Description("La URL raÃ­z redirige o carga contenido sin error 5xx")]
    public async Task UrlRaiz_NoDaError500()
    {
        var fallos = new List<string>();

        var dominiosExternos = new[] { "microsoft.com", "google", "analytics", "telemetry", "OneCollector" };

        Page.RequestFailed += (_, req) =>
        {
            if (!dominiosExternos.Any(d => req.Url.Contains(d, StringComparison.OrdinalIgnoreCase)))
                fallos.Add($"Request fallida: {req.Url} - {req.Failure}");
        };

        var response = await Page.GotoAsync(BaseUrl);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.That(response?.Status ?? 0, Is.LessThan(500),
            $"La pÃ¡gina devolviÃ³ HTTP {response?.Status}");
        Assert.That(fallos, Is.Empty,
            $"Se produjeron errores en las peticiones:\n{string.Join("\n", fallos)}");
    }
}
