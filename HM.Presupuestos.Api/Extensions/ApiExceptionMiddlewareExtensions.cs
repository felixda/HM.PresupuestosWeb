using HM.Presupuestos.Api.Middleware;

namespace HM.Presupuestos.Api.Extensions;

public static class ApiExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseApiExceptionMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ApiExceptionMiddleware>();
    }
}
