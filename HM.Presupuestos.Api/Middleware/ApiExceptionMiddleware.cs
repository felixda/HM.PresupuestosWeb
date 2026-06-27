using HM.Presupuestos.Domain.Compartido;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HM.Presupuestos.Api.Middleware;

public sealed class ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ApiExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidacionException ex)
        {
            _logger.LogWarning(ex, "Error de validacion en la API");
            await WriteProblemDetailsAsync(context, StatusCodes.Status400BadRequest, "Error de validacion", ex.Message, new Dictionary<string, object?>
            {
                ["campoValidado"] = ex.CampoValidado.ToString(),
                ["valor"] = ex.Valor
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado en la API");
            var environment = context.RequestServices.GetService<IHostEnvironment>();
            var isDevelopment = environment?.IsDevelopment() == true;

            var detail = isDevelopment
                ? $"{ex.Message}{(ex.InnerException is not null ? $" | Inner: {ex.InnerException.Message}" : string.Empty)}"
                : "Se ha producido un error no controlado.";

            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Error interno",
                detail,
                isDevelopment
                    ? new Dictionary<string, object?>
                    {
                        ["exceptionType"] = ex.GetType().FullName,
                        ["stackTrace"] = ex.StackTrace
                    }
                    : null);
        }
    }

    private static Task WriteProblemDetailsAsync(
        HttpContext context,
        int statusCode,
        string title,
        string detail,
        IDictionary<string, object?>? extensions = null)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        if (extensions is not null)
        {
            foreach (var extension in extensions)
            {
                problem.Extensions[extension.Key] = extension.Value;
            }
        }

        return context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}
