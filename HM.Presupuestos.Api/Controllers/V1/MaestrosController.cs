using HM.Presupuestos.Api.Contracts.V1.Maestros;
using HM.Presupuestos.Application.CasosDeUso.Compartido;
using HM.Core.Comun.v6.Entidades.Seguridad;
using HM.Core.Comun.v6.Seguridad.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
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
    public async Task<ActionResult<IReadOnlyList<CodigoDescripcionResponse>>> ObtenerTipologias()
    {
        InicializarUsuarioTecnicoSiNoExiste();

        var resultado = await _maestrosService.ObtenerTipologias();

        var response = resultado
            .Select(x => new CodigoDescripcionResponse(x.Codigo, x.Descripcion))
            .ToList();

        return Ok(response);
    }

    [HttpGet("networks")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CodigoDescripcionResponse>>> ObtenerNetworks()
    {
        InicializarUsuarioTecnicoSiNoExiste();

        var resultado = await _maestrosService.ObtenerNetworks();

        var response = resultado
            .Select(x => new CodigoDescripcionResponse(x.Codigo, x.Descripcion))
            .ToList();

        return Ok(response);
    }

    [HttpGet("medios")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CodigoDescripcionResponse>>> ObtenerMedios()
    {
        InicializarUsuarioTecnicoSiNoExiste();

        var resultado = await _maestrosService.ObtenerMedios();

        var response = resultado
            .Select(x => new CodigoDescripcionResponse(x.Codigo, x.Descripcion))
            .ToList();

        return Ok(response);
    }

    [HttpGet("grupos-clientes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CodigoDescripcionResponse>>> ObtenerGruposClientes()
    {
        InicializarUsuarioTecnicoSiNoExiste();

        var resultado = await _maestrosService.ObtenerGruposClientes();

        var response = resultado
            .Select(x => new CodigoDescripcionResponse(x.Codigo, x.Descripcion))
            .ToList();

        return Ok(response);
    }

    private void InicializarUsuarioTecnicoSiNoExiste()
    {
        if (_jwt.Usuario is not null)
        {
            return;
        }

        if (Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
        {
            var bearerHeader = authorizationHeader.ToString();

            if (AuthenticationHeaderValue.TryParse(bearerHeader, out var authValue)
                && "Bearer".Equals(authValue.Scheme, StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(authValue.Parameter))
            {
                try
                {
                    var usuarioToken = _jwt.ObtenerDatosUsuarioJwt(authValue.Parameter);
                    if (usuarioToken is not null)
                    {
                        _jwt.Usuario = usuarioToken;
                        return;
                    }
                }
                catch
                {
                    // Si el token no se puede parsear, aplicamos fallback técnico.
                }
            }
        }

        var usuario = new UsuarioEntidad
        {
            CodigoUsuario = 1,
            CodigoAplicacion = 3,
            CodigoPais = 1,
            Login = "api-local"
        };

        var companiasProperty = typeof(UsuarioEntidad).GetProperty("Companias");
        if (companiasProperty is not null)
        {
            var value = companiasProperty.GetValue(usuario);
            if (value is null)
            {
                object? emptyCompanias = null;

                try
                {
                    emptyCompanias = Activator.CreateInstance(companiasProperty.PropertyType);
                }
                catch
                {
                    emptyCompanias = new ArrayList();
                }

                if (emptyCompanias is not null)
                {
                    companiasProperty.SetValue(usuario, emptyCompanias);
                }
            }
        }

        _jwt.Usuario = usuario;
    }
}
