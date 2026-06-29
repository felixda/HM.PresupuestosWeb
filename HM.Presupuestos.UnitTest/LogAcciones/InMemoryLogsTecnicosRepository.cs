using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Entidades.LogAcciones;
using HM.Presupuestos.Domain.Puertos;
using HM.Presupuestos.Application.CasosDeUso.LogAcciones;

namespace HM.Presupuestos.UnitTest.LogAcciones;

internal sealed class InMemoryLogsTecnicosRepository : ILogsTecnicosRepository
{
    private readonly List<LogTecnico> _logs = [];
    private readonly List<CodigoDescripcion> _niveles = [];

    public void SembrarLogs(params LogTecnico[] logs)
    {
        _logs.AddRange(logs);
    }

    public void SembrarNiveles(params CodigoDescripcion[] niveles)
    {
        _niveles.AddRange(niveles);
    }

    public Task<List<LogTecnico>> ObtenerLogs(FiltroLogsTecnicos filtro)
    {
        var query = _logs.AsEnumerable();

        if (filtro.FechaDesde.HasValue)
            query = query.Where(x => x.Fecha >= filtro.FechaDesde.Value);

        if (filtro.FechaHasta.HasValue)
            query = query.Where(x => x.Fecha <= filtro.FechaHasta.Value);

        if (!string.IsNullOrWhiteSpace(filtro.Nivel))
            query = query.Where(x => string.Equals(x.Nivel, filtro.Nivel, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(filtro.Usuario))
        {
            if (string.Equals(filtro.Usuario, LogsTecnicosConstantes.UsuarioSinUsuario, StringComparison.OrdinalIgnoreCase))
                query = query.Where(x => string.IsNullOrWhiteSpace(x.Usuario));
            else
                query = query.Where(x => string.Equals(x.Usuario, filtro.Usuario, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(filtro.Categoria))
            query = query.Where(x => string.Equals(x.Categoria, filtro.Categoria, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(query.OrderByDescending(x => x.Fecha).ToList());
    }

    public Task<List<CodigoDescripcion>> ObtenerNivelesDisponibles()
    {
        return Task.FromResult(_niveles.ToList());
    }
}