using HM.Presupuestos.Domain.Entidades;
using DomainVersion = HM.Presupuestos.Domain.Entidades.Version;
using HM.Presupuestos.Domain.Puertos;

namespace HM.Presupuestos.UnitTest.Fakes;

internal sealed class InMemoryVersionesRepository : IVersionesRepository
{
    private readonly List<DomainVersion> _versions = new();
    private readonly List<Indicador> _indicadores = new();
    private readonly InMemoryTransaccion _trans = new();

    public Task<List<Indicador>> ObtenerEstadosVersiones() => Task.FromResult(_indicadores.ToList());

    public Task<List<DomainVersion>> ObtenerVersiones(int anio, int? estadoIncluido = null)
    {
        return Task.FromResult(_versions.Where(v => anio == 0 || v.Codigo == anio).ToList());
    }

    public Task<List<CodigoDescripcion>> ObtenerAniosConVersiones() => Task.FromResult(new List<CodigoDescripcion>());

    public Task<int> InsertarVersion(int codigoPais, DomainVersion version)
    {
        var next = (_versions.Count == 0) ? 1 : _versions.Max(v => v.Codigo) + 1;
        version.Codigo = next;
        _versions.Add(version);
        return Task.FromResult(next);
    }

    public Task ActualizarVersion(DomainVersion version)
    {
        var ex = _versions.FirstOrDefault(v => v.Codigo == version.Codigo);
        if (ex != null)
        {
            // replace fields
            ex.Descripcion = version.Descripcion;
            ex.IndEstado = version.IndEstado;
        }
        return Task.CompletedTask;
    }

    public Task EliminarVersion(int codigoVersion)
    {
        var ex = _versions.FirstOrDefault(v => v.Codigo == codigoVersion);
        if (ex != null) _versions.Remove(ex);
        return Task.CompletedTask;
    }

    public Task<List<MedioIncremento>> ObtenerImportesMedios(FiltroComprobarNetoVentaOrigenJSON json) => Task.FromResult(new List<MedioIncremento>());

    public Task GrabarCopiasVersiones(DatosCargarVersionDestinoJSON json) => Task.CompletedTask;

    public Task<List<VersionResumen>> ObtenerVersionesResumen(int? anio = null, int? estadoIncluido = null, int? estadoExcluido = null)
    {
        return Task.FromResult(new List<VersionResumen>());
    }

    public Task<bool> TieneDatosVinculados(int codigoVersion) => Task.FromResult(_versions.Any(v => v.Codigo == codigoVersion));

    public ITransaccion ObtenerTransaccion() => _trans;

    // helpers
    public void SeedVersion(DomainVersion v) => _versions.Add(v);
    public void SeedIndicador(Indicador i) => _indicadores.Add(i);
}
