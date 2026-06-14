using HM.Presupuestos.E2ETest.Configuracion;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace HM.Presupuestos.E2ETest.Base;

[TestFixture]
public abstract class E2ETestBase : PageTest
{
    // Ruta donde se guarda el estado de sesiÃ³n SSO (generado con GuardarSesion.ps1)
    private static readonly string SesionPath = Path.Combine(
        AppContext.BaseDirectory, "sesion_auth.json");

    protected E2ETestSettings Settings { get; private set; } = new();
    protected string BaseUrl => Settings.BaseUrl;

    [OneTimeSetUp]
    public void CargarConfiguracion()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        Settings = new E2ETestSettings();
        config.GetSection("E2ETest").Bind(Settings);
    }

    public override BrowserNewContextOptions ContextOptions()
    {
        // Si existe sesiÃ³n guardada, se reutiliza para evitar pasar por SSO
        var opciones = new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true
        };

        if (File.Exists(SesionPath))
            opciones.StorageStatePath = SesionPath;

        return opciones;
    }

    protected async Task IrAUrl(string rutaRelativa = "")
    {
        await Page.GotoAsync($"{BaseUrl}/{rutaRelativa.TrimStart('/')}");
    }
}
