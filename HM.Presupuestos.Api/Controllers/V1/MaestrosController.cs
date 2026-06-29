using HM.Presupuestos.Api.Contracts.V1.Maestros;
using HM.Presupuestos.Application.CasosDeUso.Compartido;
using HM.Core.Comun.v6.Seguridad.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace HM.Presupuestos.Api.Controllers.V1;

[ApiController]
[Route("api/v1/maestros")]
[AllowAnonymous]
public class MaestrosController(IMaestrosService maestrosService, IJwt jwt) : ControllerBase
{
    private readonly IMaestrosService _maestrosService = maestrosService;
    private readonly IJwt _jwt = jwt;

    [HttpGet("tipologias")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<CodigoDescripcionResponse>>> ObtenerTipologias()
    {
        if (!IntentarInicializarUsuarioDesdeToken())
        {
            return Unauthorized();
        }

        var resultado = await _maestrosService.ObtenerTipologias();

        var response = resultado
            .Select(x => new CodigoDescripcionResponse(x.Codigo, x.Descripcion))
            .ToList();

        return Ok(response);
    }

    [HttpGet("networks")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<CodigoDescripcionResponse>>> ObtenerNetworks()
    {
        if (!IntentarInicializarUsuarioDesdeToken())
        {
            return Unauthorized();
        }

        var resultado = await _maestrosService.ObtenerNetworks();

        var response = resultado
            .Select(x => new CodigoDescripcionResponse(x.Codigo, x.Descripcion))
            .ToList();

        return Ok(response);
    }

    [HttpGet("medios")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<CodigoDescripcionResponse>>> ObtenerMedios()
    {
        if (!IntentarInicializarUsuarioDesdeToken())
        {
            return Unauthorized();
        }

        var resultado = await _maestrosService.ObtenerMedios();

        var response = resultado
            .Select(x => new CodigoDescripcionResponse(x.Codigo, x.Descripcion))
            .ToList();

        return Ok(response);
    }

    [HttpGet("grupos-clientes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<CodigoDescripcionResponse>>> ObtenerGruposClientes()
    {
        if (!IntentarInicializarUsuarioDesdeToken())
        {
            return Unauthorized();
        }

        var resultado = await _maestrosService.ObtenerGruposClientes();

        var response = resultado
            .Select(x => new CodigoDescripcionResponse(x.Codigo, x.Descripcion))
            .ToList();

        return Ok(response);
    }

    private bool IntentarInicializarUsuarioDesdeToken()
    {
        if (_jwt.Usuario is not null)
        {
            return true;
        }

        if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
        {
            return false;
        }

        var bearerHeader = authorizationHeader.ToString();
        if (!AuthenticationHeaderValue.TryParse(bearerHeader, out var authValue)
            || !"Bearer".Equals(authValue.Scheme, StringComparison.OrdinalIgnoreCase)
            || string.IsNullOrWhiteSpace(authValue.Parameter))
        {
            return false;
        }

        try
        {
            var usuarioToken = _jwt.ObtenerDatosUsuarioJwt(authValue.Parameter);
            if (usuarioToken is null)
            {
                return IntentarInicializarUsuarioDesdeClaims(authValue.Parameter);
            }

            _jwt.Usuario = usuarioToken;
            return true;
        }
        catch
        {
            return IntentarInicializarUsuarioDesdeClaims(authValue.Parameter);
        }
    }

    private bool IntentarInicializarUsuarioDesdeClaims(string jwtToken)
    {
        if (User?.Identity?.IsAuthenticated != true)
        {
            return false;
        }

        var usuarioProperty = _jwt.GetType().GetProperty("Usuario");
        var usuarioType = usuarioProperty?.PropertyType;

        if (usuarioProperty is null || usuarioType is null)
        {
            return false;
        }

        var usuario = Activator.CreateInstance(usuarioType);
        if (usuario is null)
        {
            return false;
        }

        AsignarSiExiste(usuarioType, usuario, "CodigoUsuario", ObtenerClaimInt("CodigoUsuario", 1));
        AsignarSiExiste(usuarioType, usuario, "CodigoAplicacion", ObtenerClaimInt("CodigoAplicacion", 1));
        AsignarSiExiste(usuarioType, usuario, "CodigoPais", ObtenerClaimInt("CodigoPais", 34));
        AsignarSiExiste(usuarioType, usuario, "Companias", ObtenerClaimString("Companias", "1"));
        AsignarSiExiste(usuarioType, usuario, "Login", ObtenerClaimString("Login", User.Identity?.Name ?? "swagger.dev"));
        AsignarSiExiste(usuarioType, usuario, "Nombre", ObtenerClaimString("Nombre", "Swagger"));
        AsignarSiExiste(usuarioType, usuario, "Apellido1", ObtenerClaimString("Apellido1", "Dev"));
        AsignarSiExiste(usuarioType, usuario, "Jwt", jwtToken);

        usuarioProperty.SetValue(_jwt, usuario);
        return true;
    }

    private int ObtenerClaimInt(string claimType, int valorPorDefecto)
    {
        var claim = User.FindFirst(claimType)?.Value;
        return int.TryParse(claim, out var valor) ? valor : valorPorDefecto;
    }

    private string ObtenerClaimString(string claimType, string valorPorDefecto)
    {
        var claim = User.FindFirst(claimType)?.Value;
        return string.IsNullOrWhiteSpace(claim) ? valorPorDefecto : claim;
    }

    private static void AsignarSiExiste(Type targetType, object target, string nombrePropiedad, object? valor)
    {
        var property = targetType.GetProperty(nombrePropiedad);
        if (property is null || !property.CanWrite)
        {
            return;
        }

        if (valor is null)
        {
            return;
        }

        var targetTypeFinal = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
        if (!targetTypeFinal.IsAssignableFrom(valor.GetType()))
        {
            try
            {
                valor = Convert.ChangeType(valor, targetTypeFinal);
            }
            catch
            {
                return;
            }
        }

        property.SetValue(target, valor);
    }
}
