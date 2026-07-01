using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Puertos;

namespace HM.Presupuestos.UnitTest.Fakes;

internal sealed class InMemoryIndicadoresRepository : IIndicadoresRepository
{
    private readonly Dictionary<int, Indicador> _indicadores = new();
    private readonly Dictionary<int, IdiomaIndicador> _idiomas = new();
    private readonly Dictionary<int, int> _bitAndVersionesActualizado = new();
    private int _nextCodigoIndicador = 1;
    private int _nextCodigoIdioma = 1;

    public InMemoryTransaccion UltimaTransaccion { get; private set; } = new();

    public IReadOnlyDictionary<int, Indicador> Indicadores => _indicadores;

    public IReadOnlyDictionary<int, IdiomaIndicador> Idiomas => _idiomas;

    public IReadOnlyDictionary<int, int> BitAndVersionesActualizado => _bitAndVersionesActualizado;

    public void SembrarIndicadores(params Indicador[] indicadores)
    {
        foreach (var indicador in indicadores)
        {
            var codigo = indicador.Codigo ?? _nextCodigoIndicador++;
            indicador.Codigo = codigo;
            _nextCodigoIndicador = Math.Max(_nextCodigoIndicador, codigo + 1);
            _indicadores[codigo] = ClonarIndicador(indicador);
        }
    }

    public Task<List<Indicador>> ObtenerIndicadoresConIdiomas(string? descripcion = null)
    {
        var query = _indicadores.Values.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(descripcion))
        {
            query = query.Where(i => i.Descripcion.Contains(descripcion, StringComparison.OrdinalIgnoreCase));
        }

        var resultado = query
            .OrderBy(i => i.Orden)
            .Select(ClonarIndicadorConIdiomas)
            .ToList();

        return Task.FromResult(resultado);
    }

    public Task EliminarIndicador(int codigoIndicador)
    {
        _indicadores.Remove(codigoIndicador);
        return Task.CompletedTask;
    }

    public Task EliminarIdiomasIndicador(int codigoIndicador)
    {
        var codigosAEliminar = _idiomas
            .Where(x => x.Value.CodigoIndicador == codigoIndicador)
            .Select(x => x.Key)
            .ToList();

        foreach (var codigo in codigosAEliminar)
        {
            _idiomas.Remove(codigo);
        }

        return Task.CompletedTask;
    }

    public Task<bool> ExisteIndicador(Indicador indicador)
    {
        var existe = _indicadores.Values.Any(i =>
            i.Codigo != indicador.Codigo &&
            string.Equals(i.Descripcion, indicador.Descripcion, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(existe);
    }

    public Task<bool> ExisteOrden(Indicador indicador)
    {
        var existe = _indicadores.Values.Any(i =>
            i.Codigo != indicador.Codigo &&
            i.Orden == indicador.Orden);

        return Task.FromResult(existe);
    }

    public Task<bool> ExisteBitAnd(Indicador indicador)
    {
        var existe = _indicadores.Values.Any(i =>
            i.Codigo != indicador.Codigo &&
            i.BitAnd == indicador.BitAnd);

        return Task.FromResult(existe);
    }

    public Task<int> InsertarIndicador(Indicador indicador)
    {
        var codigo = _nextCodigoIndicador++;
        indicador.Codigo = codigo;
        _indicadores[codigo] = ClonarIndicador(indicador);
        return Task.FromResult(codigo);
    }

    public Task ActualizarIndicador(Indicador indicador)
    {
        if (indicador.Codigo is null)
        {
            throw new InvalidOperationException("El indicador debe tener código para actualizar.");
        }

        _indicadores[indicador.Codigo.Value] = ClonarIndicador(indicador);
        return Task.CompletedTask;
    }

    public Task<int> ObtenerUltimoBitAnd()
    {
        var ultimo = _indicadores.Count == 0 ? 0 : _indicadores.Values.Max(i => i.BitAnd);
        return Task.FromResult(ultimo);
    }

    public Task<int> ObtenerUltimoOrden()
    {
        var ultimo = _indicadores.Count == 0 ? 0 : _indicadores.Values.Max(i => i.Orden);
        return Task.FromResult(ultimo);
    }

    public Task InsertarIdiomaIndicador(IdiomaIndicador idiomaIndicador)
    {
        var codigo = idiomaIndicador.Codigo ?? _nextCodigoIdioma++;
        idiomaIndicador.Codigo = codigo;
        _nextCodigoIdioma = Math.Max(_nextCodigoIdioma, codigo + 1);
        _idiomas[codigo] = ClonarIdioma(idiomaIndicador);
        return Task.CompletedTask;
    }

    public Task ActualizarIdiomaIndicador(IdiomaIndicador idiomaIndicador)
    {
        if (idiomaIndicador.Codigo is null)
        {
            throw new InvalidOperationException("El idioma debe tener código para actualizar.");
        }

        _idiomas[idiomaIndicador.Codigo.Value] = ClonarIdioma(idiomaIndicador);
        return Task.CompletedTask;
    }

    public ITransaccion ObtenerTransaccion()
    {
        UltimaTransaccion = new InMemoryTransaccion();
        return UltimaTransaccion;
    }

    public Task EliminarIdiomaIndicador(int codigo)
    {
        _idiomas.Remove(codigo);
        return Task.CompletedTask;
    }

    public Task<int> ObtenerBitAndIndicador(int codigoIndicador)
    {
        return Task.FromResult(_indicadores[codigoIndicador].BitAnd);
    }

    public Task Actualizar1BitAndVersion(int codigoVersion, int bitAnd)
    {
        _bitAndVersionesActualizado[codigoVersion] = bitAnd;
        return Task.CompletedTask;
    }

    private Indicador ClonarIndicadorConIdiomas(Indicador indicador)
    {
        var clonado = ClonarIndicador(indicador);
        clonado.Idiomas = _idiomas.Values
            .Where(i => i.CodigoIndicador == clonado.Codigo)
            .Select(ClonarIdioma)
            .ToList();
        return clonado;
    }

    private static Indicador ClonarIndicador(Indicador indicador)
    {
        return new Indicador
        {
            Codigo = indicador.Codigo,
            Indice = indicador.Indice,
            Orden = indicador.Orden,
            CodigoIdioma = indicador.CodigoIdioma,
            Descripcion = indicador.Descripcion,
            BitAnd = indicador.BitAnd,
            IndMostrar = indicador.IndMostrar,
            IndVersionUnica = indicador.IndVersionUnica,
            Estado = indicador.Estado,
            Idiomas = indicador.Idiomas.Select(ClonarIdioma).ToList()
        };
    }

    private static IdiomaIndicador ClonarIdioma(IdiomaIndicador idioma)
    {
        return new IdiomaIndicador
        {
            Codigo = idioma.Codigo,
            CodigoIndicador = idioma.CodigoIndicador,
            CodigoIdioma = idioma.CodigoIdioma,
            Descripcion = idioma.Descripcion,
            DescripcionAbreviada = idioma.DescripcionAbreviada,
            Leyenda = idioma.Leyenda
        };
    }
}
