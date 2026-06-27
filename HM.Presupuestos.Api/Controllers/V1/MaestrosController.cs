using HM.Presupuestos.Api.Contracts.V1.Maestros;
using HM.Presupuestos.Application.CasosDeUso.Compartido;
using HM.Core.Comun.v6.Seguridad.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

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
                return false;
            }

            _jwt.Usuario = usuarioToken;
            return true;
        }
        catch
        {
            return false;
        }
    }
}
