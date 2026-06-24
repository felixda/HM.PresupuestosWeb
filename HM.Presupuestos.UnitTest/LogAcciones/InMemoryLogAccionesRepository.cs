using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Entidades.LogAcciones;
using HM.Presupuestos.Domain.Puertos;

namespace HM.Presupuestos.UnitTest.LogAcciones;

internal sealed class InMemoryLogAccionesRepository : ILogAccionesRepository
{
    private readonly List<LogAccion> _logsInsertados = [];
    private readonly List<AuditoriaSeed> _auditorias = [];

    public IReadOnlyList<LogAccion> LogsInsertados => _logsInsertados;

    public void SembrarAuditorias(params AuditoriaSeed[] auditorias)
    {
        _auditorias.AddRange(auditorias);
    }

    public Task Insertar(LogAccion logAccion)
    {
        _logsInsertados.Add(new LogAccion
        {
            CodigoUsuario = logAccion.CodigoUsuario,
            Accion = logAccion.Accion,
            Parametros = logAccion.Parametros
        });

        return Task.CompletedTask;
    }

    public Task<List<Auditoria>> ObtenerAuditorias(AccionesLog tipo, DateTime? fechaInicio, DateTime? fechaFin, int? codigoPagina = null)
    {
        var query = _auditorias.Where(a => a.Tipo == tipo);

        if (fechaInicio.HasValue)
        {
            query = query.Where(a => a.FechaInicio >= fechaInicio.Value);
        }

        if (fechaFin.HasValue)
        {
            query = query.Where(a => a.FechaInicio <= fechaFin.Value);
        }

        if (codigoPagina.HasValue)
        {
            query = query.Where(a => a.CodigoPagina == codigoPagina.Value);
        }

        var resultado = query
            .OrderBy(a => a.FechaInicio)
            .Select(a => new Auditoria
            {
                Descripcion = a.Descripcion,
                FechaInicio = a.FechaInicio,
                Usuario = a.Usuario,
                Parametros = a.Parametros
            })
            .ToList();

        return Task.FromResult(resultado);
    }

    public Task<EstadisticasAuditoria> ObtenerEstadisticas(AccionesLog tipo, DateTime fechaInicio, DateTime fechaFin, int? codigoPagina = null)
    {
        var filtradas = _auditorias
            .Where(a => a.Tipo == tipo)
            .Where(a => a.FechaInicio >= fechaInicio && a.FechaInicio <= fechaFin)
            .Where(a => !codigoPagina.HasValue || a.CodigoPagina == codigoPagina.Value)
            .ToList();

        var topUsuarios = filtradas
            .GroupBy(a => a.Usuario)
            .Select(g => new UsuarioContador { Login = g.Key, Total = g.Count() })
            .OrderByDescending(x => x.Total)
            .ThenBy(x => x.Login)
            .ToList();

        var actividadPorDia = filtradas
            .GroupBy(a => a.FechaInicio.Date)
            .Select(g => new PuntoTemporal { Fecha = g.Key, Total = g.Count() })
            .OrderBy(x => x.Fecha)
            .ToList();

        var usuarioMasActivo = topUsuarios.FirstOrDefault();
        var paginaMasVisitada = filtradas
            .GroupBy(a => a.Descripcion)
            .Select(g => new { Pagina = g.Key, Total = g.Count() })
            .OrderByDescending(x => x.Total)
            .ThenBy(x => x.Pagina)
            .FirstOrDefault();

        var estadisticas = new EstadisticasAuditoria
        {
            TotalAcciones = filtradas.Count,
            UsuariosUnicos = topUsuarios.Count,
            UsuarioMasActivo = usuarioMasActivo?.Login ?? string.Empty,
            UsuarioMasActivoTotal = usuarioMasActivo?.Total ?? 0,
            PaginaMasVisitada = paginaMasVisitada?.Pagina ?? string.Empty,
            PaginaMasVisitadaTotal = paginaMasVisitada?.Total ?? 0,
            TopUsuarios = topUsuarios,
            ActividadPorDia = actividadPorDia
        };

        return Task.FromResult(estadisticas);
    }
}

internal readonly record struct AuditoriaSeed(
    AccionesLog Tipo,
    DateTime FechaInicio,
    string Usuario,
    string Descripcion,
    string Parametros,
    int? CodigoPagina);
