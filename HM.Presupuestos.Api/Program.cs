using HM.Core.Comun.v6.Agentes;
using HM.Core.Comun.v6.Agentes.Interfaces;
using HM.Core.Comun.v6.Seguridad;
using HM.Core.Comun.v6.Seguridad.Interfaces;
using HM.Core.Servidor.v6;
using HM.Core.Servidor.v6.Entidades.Configuracion;
using HM.Core.Servidor.v6.Pack.UserProviders;
using HM.Core.Servidor.v6.UserProviders.Interfaces;
using HM.Presupuestos.Api.Extensions;
using HM.Presupuestos.Application.CasosDeUso;
using HM.Presupuestos.Application.CasosDeUso.Compartido;
using HM.Presupuestos.Domain.Puertos;
using HM.Presupuestos.Infrastructure.Persistencia;
using HM.Presupuestos.Infrastructure.Servicios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>(optional: true)
    .Build();

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddConfiguration(configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var authSection = builder.Configuration.GetSection("Auth");
var jwtSection = builder.Configuration.GetSection("Jwt");

var issuer = authSection["Issuer"] ?? jwtSection["IssuerToken"] ?? "HAVAS";
var audience = authSection["Audience"] ?? jwtSection["AudienceToken"] ?? "HAVAS";
var signingKey = authSection["SigningKey"] ?? jwtSection["Clave"];

if (string.IsNullOrWhiteSpace(signingKey))
{
    throw new InvalidOperationException("No hay clave JWT configurada. Define Auth:SigningKey o Jwt:Clave.");
}
var urlBaseCore = builder.Configuration.GetValue<string>("ServicioCore:UrlBase") ?? string.Empty;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
        };
    });

builder.Services.AddAuthorization();

// Registro base HM.Core compartido con el resto de aplicaciones.
InyeccionDependencias inyeccionCoreServidor = new();
inyeccionCoreServidor.Inyectar(builder.Services);

builder.Services.Configure<AppConfigServidor>(builder.Configuration);

builder.Services.AddScoped<IJwt, Jwt>();
builder.Services.AddScoped<IHttpClientHandler, HM.Core.Comun.v6.Agentes.HttpClientHandler>();
builder.Services.AddScoped<IApiCoreClient, ApiCoreClient>(client => new ApiCoreClient(client.GetService<IHttpClientHandler>(), urlBaseCore));
builder.Services.AddScoped<IControlador, Controlador>();
builder.Services.AddTransient<IUserProvider, AzureAdProvider>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<IClienteApiCore, ClienteApiCore>();

// Servicios de Application.
builder.Services.AddScoped<IMaestrosService, MaestrosService>();

// Puertos de Domain implementados en Infrastructure.
builder.Services.AddScoped<IPresupuestosRepository, PresupuestosRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseApiExceptionMiddleware();
app.UseHttpsRedirection();
app.UseCors("DefaultCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
