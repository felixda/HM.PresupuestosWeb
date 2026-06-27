using HM.Presupuestos.E2ETest.Configuracion;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace HM.Presupuestos.E2ETest.Base;

[TestFixture]
public abstract class E2ETestBase : PageTest
{
    // Ruta donde se guarda el estado de sesion SSO (generado con GuardarSesion.ps1)
    // Se admiten ambas rutas para evitar friccion entre compilaciones net8/net10.
    private static readonly string[] SesionCandidates =
    [
        Path.Combine(AppContext.BaseDirectory, "sesion_auth.json"),
        Path.Combine(AppContext.BaseDirectory.Replace("net10.0", "net8.0"), "sesion_auth.json")
    ];

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
        // Si existe sesion guardada, se reutiliza para evitar pasar por SSO
        var opciones = new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true
        };

        var sesionPath = SesionCandidates.FirstOrDefault(File.Exists);
        if (!string.IsNullOrWhiteSpace(sesionPath))
            opciones.StorageStatePath = sesionPath;

        return opciones;
    }

    protected async Task IrAUrl(string rutaRelativa = "")
    {
        await Page.GotoAsync($"{BaseUrl}/{rutaRelativa.TrimStart('/')}");
    }
}
