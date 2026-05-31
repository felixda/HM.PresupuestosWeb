using HM.Core.Comun.v6.Agentes;
using HM.Core.Comun.v6.Agentes.Interfaces;
using HM.Core.Comun.v6.Loggers;
using HM.Core.Comun.v6.Seguridad;
using HM.Core.Comun.v6.Seguridad.Interfaces;
using HM.Core.Servidor.v6;
using HM.Core.Servidor.v6.Entidades.Configuracion;
using HM.Core.Servidor.v6.Pack.Entidades.Configuracion;
using HM.Core.Servidor.v6.Pack.UserProviders;
using HM.Core.Servidor.v6.UserProviders.Interfaces;
using HM.Presupuestos.Application.CasosDeUso;
using HM.Presupuestos.Application.CasosDeUso.Compartido;
using HM.Presupuestos.Domain.Puertos;
using HM.Presupuestos.Infrastructure.Servicios;
using HM.Presupuestos.Infrastructure.Persistencia;
using HM.Presupuestos.Web;
using HM.Presupuestos.Web.Adaptadores.Sesion;
using HM.Presupuestos.Web.ThemeSwitcher;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Localization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.Globalization;
using System.Net;
using WebApplication = Microsoft.AspNetCore.Builder.WebApplication;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>(optional: true)
    .Build();

var urlBaseCore = configuration.GetValue<string>("ServicioCore:Core");
var urlBase = configuration.GetValue<string>("Presupuestos:UrlBaseApi");
var hoursExpired = configuration.GetValue<int>("Presupuestos:CookieJwtExpire");
var puerto = configuration.GetValue<int>("Hosting:PuertoHttp");

var options = new WebApplicationOptions
{
    WebRootPath = "wwwroot",
};

var builder = WebApplication.CreateBuilder(options);

builder.Configuration.AddConfiguration(configuration);

//Kestrel es el servidor web predeterminado en ASP.NET Core.
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, puerto);
});

builder.WebHost.UseConfiguration(configuration);


// SSO authentication - ESTE ORDEN ES CRÍTICO
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AppSettings:AzureAd"));

builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();


InyeccionDependencias inyeccionCoreServidor = new InyeccionDependencias();
inyeccionCoreServidor.Inyectar(builder.Services);

AppCoreConfig configuracionCore = new AppCoreConfig();

configuration.Bind(configuracionCore);

builder.Services.AddTransient<IUserProvider, AzureAdProvider>();

ApiCoreCli.ConfigurarUrlBaseCore(urlBaseCore ?? String.Empty);
ApiCoreCli.ConfigurarLogger(new NLogLogger(new LogUtility()));

builder.Services.AddControllers();


builder.Services.AddRazorComponents()  // Enabling razor components (Blazor)
    .AddInteractiveServerComponents(); // Enabling Blazor Server (SignalR)


builder.Services.AddDevExpressBlazor(options => {
    options.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
    options.SizeMode = DevExpress.Blazor.SizeMode.Medium;
});

//Configure blazor
builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options => 
    {
        options.DetailedErrors = true;
        // Tiempo que el servidor mantiene un circuito desconectado antes de eliminarlo
        // Despues de este tiempo, si el cliente no se reconecta, el circuito se destruye
        options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(6);
        // Numero maximo de circuitos desconectados que se mantienen en memoria
        options.DisconnectedCircuitMaxRetained = 100;
    })
    .AddHubOptions(options =>
    {
        // Tiempo maximo que el servidor espera a recibir un mensaje del cliente
        // Si se excede, se considera que el cliente esta desconectado
        options.ClientTimeoutInterval = TimeSpan.FromMinutes(5);
        // Tiempo maximo para completar el handshake inicial
        options.HandshakeTimeout = TimeSpan.FromSeconds(30);
        // Intervalo en que el servidor envia pings al cliente para mantener viva la conexion
        options.KeepAliveInterval = TimeSpan.FromSeconds(15);
        // Tamano maximo de mensajes (util para prevenir ataques)
        options.MaximumReceiveMessageSize = 32 * 1024; // 32KB
    });

//builder.WebHost.UseStaticWebAssets();
builder.Services.AddLocalization();

builder.Services.Configure<AppConfigServidor>(configuration);

//Havas Core services
builder.Services.AddScoped<IJwt, Jwt>();
builder.Services.AddScoped<IHttpClientHandler, HM.Core.Comun.v6.Agentes.HttpClientHandler>();
builder.Services.AddScoped<IApiCoreClient, ApiCoreClient>(client => new ApiCoreClient(client.GetService<IHttpClientHandler>(), urlBaseCore));
builder.Services.AddScoped<IControlador, Controlador>();
builder.Services.AddHttpContextAccessor();

// ===== SERVICIOS DE APLICACIÓN =====
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<MaestrosService>();
builder.Services.AddScoped<VersionesService>();
builder.Services.AddScoped<ISobreprimasService, SobreprimasService>();
builder.Services.AddScoped<CondicionesService>();
builder.Services.AddScoped<IndicadoresService>();
builder.Services.AddSingleton<IProveedorRecursosJson, ProveedorRecursosJson>();
builder.Services.AddScoped<ILocalizadorRecursos, LocalizadorRecursos>();
builder.Services.AddScoped<IMapaMenu, MapaMenu>();
builder.Services.AddScoped<IGestorIdioma, GestorIdioma>();
builder.Services.AddScoped<IAlmacenSesionUsuario, AlmacenSesionUsuario>();
builder.Services.AddScoped<IRegistroAplicacion, RegistroAplicacion>();
builder.Services.AddScoped<MensajesHelper>();
builder.Services.AddScoped<TraduccionesHelper>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<DialogoErrores>();
builder.Services.AddScoped<ParametrosNavegacion>();
builder.Services.AddScoped<ControlInactividad>();
builder.Services.AddScoped<JsInteropHelper>();
builder.Services.AddScoped<IControlCambiosNavegacion, ControlCambiosNavegacion>();
builder.Services.AddScoped<ModalControlCambios>();
builder.Services.AddSingleton<IAvisosService, AvisosService>();
builder.Services.AddScoped<ConfiguracionService>();
builder.Services.AddScoped<ILayerOverlayService, LayerOverlayService>();
builder.Services.AddScoped<IGestorCookies, GestorCookies>(); 
builder.Services.AddScoped<ISesionUsuario, SesionUsuario>();
builder.Services.AddScoped<IControlAccesoNavegacion, ControlAccesoNavegacion>();
builder.Services.AddScoped<IRutasNavegacion, RutasNavegacion>();
builder.Services.AddScoped<IValidadorMenusUsuario, ValidadorMenusUsuario>();


// ===== SERVICIOS DATOS =====
builder.Services.AddScoped<IMaestrosService, MaestrosService>();
builder.Services.AddScoped<IVersionesService, VersionesService>();
builder.Services.AddScoped<IIndicadoresService, IndicadoresService>();
builder.Services.AddScoped<ICondicionesService, CondicionesService>();
builder.Services.AddScoped<IConfiguracionService, ConfiguracionService>();
builder.Services.AddScoped<ILogAccionesService, LogAccionesService>();
builder.Services.AddScoped<IRegistroErroresCore, RegistroErroresCore>();



// ===== REPOSITORIOS =====
builder.Services.AddScoped<IPresupuestosRepository, PresupuestosRepository>();
builder.Services.AddScoped<IVersionesRepository, VersionesRepository>();
builder.Services.AddScoped<ISobreprimasRepository, SobreprimasRepository>();
builder.Services.AddScoped<ICondicionesRepository, CondicionesRepository>();
builder.Services.AddScoped<IIndicadoresRepository, IndicadoresRepository>();
builder.Services.AddScoped<IConfiguracionRepository, ConfiguracionRepository>();
builder.Services.AddScoped<ILogAccionesRepository, LogAccionesRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();


// ===== HTTP CLIENT =====
builder.Services.AddScoped(sp => {
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(navigationManager.BaseUri) };
});

builder.WebHost.UseWebRoot("wwwroot");



// Agregar servicios de sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(3600); // Session expiration time
    options.Cookie.HttpOnly = true; // Prevent JavaScript access to the cookie
    options.Cookie.IsEssential = true; // Set cookie using
});

// ===== LOCALIZACIÓN =====
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-US"),
        new CultureInfo("es-ES"),
        new CultureInfo("pt-PT")
    };

    // Configura la cultura por defecto.
    options.DefaultRequestCulture = new RequestCulture("es-ES");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});



var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/HttpError", createScopeForErrors: true); // Redirige a la página de error en caso de una excepción no controlada
    app.UseHsts();  //Fuerza el uso de HTTPS en el navegador
    app.UseStatusCodePagesWithReExecute("/HttpError", "?StatusCode={0}");
}
else
{
    app.UseExceptionHandler("/HttpError", createScopeForErrors: true);
    app.UseStatusCodePagesWithReExecute("/HttpError", "?StatusCode={0}");
    //app.UseDeveloperExceptionPage();
}

//Metodos DE ASP.NET Core
app.UseHttpsRedirection();  // Redirige automáticamente HTTP a HTTPS
app.UseRouting();           // Habilita el enrutamiento
app.UseAntiforgery();       // Protege los formularios contra CSRF (Cross-Site Request Forgery).
app.UseSession();           // Habilita el soporte de sesiones en la aplicación
app.UseAuthentication();    // Habilita autenticación y autorización
app.UseAuthorization();



// Por defecto, ASP.NET Core solo permite servir archivos estáticos desde la carpeta wwwroot.
// No se puede acceder a archivos fuera de esa carpeta sin configuraciones adicionales.
// Como hay archivos estáticos que no cambian con frecuencia (por ejemplo, bibliotecas JavaScript o imágenes),
// se configura el control de caché para mejorar el rendimiento.
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = context =>
    {
        context.Context.Response.Headers.Append("Cache-Control", "public,max-age=31536000");
    }
});


// Definimos los endpoints de la aplicación, o puntos
// en los que se procesarán peticiones:
app.MapControllers(); // Mapea los controladores API
app.MapRazorComponents<App>()  // Mapea Blazor en toda la aplicación
.AddInteractiveServerRenderMode(); // Habilita la interactividad en Blazor Server

// Endpoint para que JavaScript obtenga textos localizados sin depender de interop estático
app.MapGet("/api/recursos/{expresion}/{idioma}", (string expresion, string idioma, ILocalizadorRecursos localizador) =>
    Results.Ok(localizador.ObtenerTexto(Uri.UnescapeDataString(expresion), idioma)))
    .AllowAnonymous();

app.Run();




