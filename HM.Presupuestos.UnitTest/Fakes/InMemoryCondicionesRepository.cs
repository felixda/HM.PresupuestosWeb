using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Domain.Puertos;

namespace HM.Presupuestos.UnitTest.Fakes;

internal sealed class InMemoryCondicionesRepository : ICondicionesRepository
{
    private readonly List<Vigencia> _vigencias = new();
    private readonly List<CondicionDto> _condiciones = new();
    private readonly List<ConceptoCondicion> _conceptos = new() { new ConceptoCondicion { Codigo = (int)ConceptosCondiciones.Sag, IndicadorCalculo = 1 } };

    public InMemoryCondicionesRepository()
    {
    }

    public Task<List<ConceptoCondicion>> ObtenerConceptosDeCondicion() => Task.FromResult(_conceptos.ToList());

    public Task<List<Vigencia>> ObtenerVigencias(CondicionFiltro filtro)
    {
        var resultado = _vigencias.Where(v =>
            (filtro.CodigoVersion == 0 || v.CodigoVersion == filtro.CodigoVersion) &&
            (filtro.CodigoGrupoCliente == 0 || v.CodigoGrupoCliente == filtro.CodigoGrupoCliente) &&
            (filtro.CodigoNetwork == 0 || v.CodigoNetWork == filtro.CodigoNetwork) &&
            (filtro.IndicadorAcuerdo == v.IndicadorAcuerdo)
        ).ToList();

        return Task.FromResult(resultado);
    }

    public Task InsertarVigencia(Vigencia item)
    {
        item.Codigo = _vigencias.Count == 0 ? 1 : _vigencias.Max(x => x.Codigo) + 1;
        _vigencias.Add(item);
        return Task.CompletedTask;
    }

    public Task ActualizarVigencia(Vigencia item)
    {
        var idx = _vigencias.FindIndex(x => x.Codigo == item.Codigo);
        if (idx >= 0) _vigencias[idx] = item;
        return Task.CompletedTask;
    }

    public Task EliminarVigencia(int codigoVigencia)
    {
        _vigencias.RemoveAll(x => x.Codigo == codigoVigencia);
        return Task.CompletedTask;
    }

    public Task<bool> LaVigenciaTieneCondiciones(int codigoVigencia)
    {
        return Task.FromResult(_condiciones.Any(c => c.CodigoMedio == codigoVigencia));
    }

    public Task<List<CondicionDto>> ObtenerCondicionesDeLaVigencia(int codigoVigencia)
    {
        return Task.FromResult(_condiciones.Where(c => c.CodigoMedio == codigoVigencia).ToList());
    }

    public Task GrabarCondicion(Condicion medioCondicion)
    {
        return Task.CompletedTask;
    }

    public Task EliminarCondicion(Condicion condicion)
    {
        return Task.CompletedTask;
    }

    public Task<List<ExcepcionDto>> ObtenerExcepcionesDeLaVigencia(int codigoVigencia)
    {
        return Task.FromResult(new List<ExcepcionDto>());
    }

    public Task EliminarConceptoDeExcepcion(int codigoCondicionMeido, ConceptosCondicionesNMD concepto)
    {
        return Task.CompletedTask;
    }

    public Task EliminarExcepcionCondicion(int codigoCondicionMedio)
    {
        return Task.CompletedTask;
    }

    public Task ActualizarJerarquiaExcepcion(int codigoCondicionMedio, int jerarquia)
    {
        return Task.CompletedTask;
    }

    public Task GrabarConceptoNMD(int codigoCondicionMedio, ConceptosCondicionesNMD codigoConceptoNMD, int codigo)
    {
        return Task.CompletedTask;
    }

    public Task<List<int>> ObtenerCodigosDeExcepcionesDeCondicion(Condicion condicion)
    {
        return Task.FromResult(new List<int>());
    }

    public Task<Condicion?> ObtenerCondicionPorClave(Condicion condicion)
    {
        return Task.FromResult<Condicion?>(null);
    }

    public Task<Condicion?> ObtenerCondicionPorCodigo(int codigoCondicionMedio)
    {
        return Task.FromResult<Condicion?>(null);
    }

    public Task InsertarCondicion(Condicion condicion)
    {
        return Task.CompletedTask;
    }

    public Task ActualizarCondicion(Condicion condicion)
    {
        return Task.CompletedTask;
    }

    public Task ImportarCondicionesMMS(CondicionImportarFiltro param)
    {
        return Task.CompletedTask;
    }

    public ITransaccion ObtenerTransaccion()
    {
        return new InMemoryTransaccion();
    }
}
