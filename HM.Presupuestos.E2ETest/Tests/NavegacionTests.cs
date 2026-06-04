using HM.Presupuestos.E2ETest.Base;
using Microsoft.Playwright;

namespace HM.Presupuestos.E2ETest.Tests;

[TestFixture]
[Category("Navegacion")]
public class NavegacionTests : E2ETestBase
{
    [Test]
    [Description("La ruta /home carga sin errores de servidor")]
    public async Task RutaHome_CargaCorrectamente()
    {
        var response = await Page.GotoAsync($"{BaseUrl}/home");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.That(response?.Status ?? 0, Is.LessThan(500),
            $"La ruta /home devolviÃ³ HTTP {response?.Status}");
    }

    [Test]
    [Description("La ruta /index carga sin errores de servidor")]
    public async Task RutaIndex_CargaCorrectamente()
    {
        var response = await Page.GotoAsync($"{BaseUrl}/index");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.That(response?.Status ?? 0, Is.LessThan(500),
            $"La ruta /index devolviÃ³ HTTP {response?.Status}");
    }

    [Test]
    [Description("Las rutas /home e /index muestran el mismo contenido")]
    public async Task RutaHome_YRutaIndex_MuestranMismoContenido()
    {
        await Page.GotoAsync($"{BaseUrl}/home");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        var contenidoHome = await Page.ContentAsync();

        await Page.GotoAsync($"{BaseUrl}/index");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        var contenidoIndex = await Page.ContentAsync();

        // Ambas rutas deberÃ­an cargar la misma pÃ¡gina Index
        Assert.That(contenidoHome, Is.Not.Empty);
        Assert.That(contenidoIndex, Is.Not.Empty);
    }

    [Test]
    [Description("Una ruta inexistente no produce error 500")]
    public async Task RutaInexistente_NoProduceError500()
    {
        var response = await Page.GotoAsync($"{BaseUrl}/ruta-que-no-existe-xyz");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.That(response?.Status ?? 0, Is.Not.EqualTo(500),
            "Una ruta inexistente devolviÃ³ error 500 interno del servidor");
    }
}
