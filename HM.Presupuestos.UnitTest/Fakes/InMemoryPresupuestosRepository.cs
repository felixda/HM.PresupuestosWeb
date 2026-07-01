using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Puertos;

namespace HM.Presupuestos.UnitTest.Fakes;

internal sealed class InMemoryPresupuestosRepository : IPresupuestosRepository
{
    private readonly List<CodigoDescripcion> _tipologias = new();
    private readonly List<CodigoDescripcion> _medios = new();

    public Task<List<CodigoDescripcion>> ObtenerTipologias() => Task.FromResult(_tipologias.ToList());
    public Task<List<CodigoDescripcion>> ObtenerAlcances() => Task.FromResult(new List<CodigoDescripcion>());
    public Task<List<CodigoDescripcion>> ObtenerDiversifiedsNCB() => Task.FromResult(new List<CodigoDescripcion>());
    public Task<List<CodigoDescripcion>> ObtenerDisciplinas() => Task.FromResult(new List<CodigoDescripcion>());
    public Task<List<CodigoDescripcion>> ObtenerTiposCompra() => Task.FromResult(new List<CodigoDescripcion>());
    public Task<List<CodigoDescripcion>> ObtenerObjetivos() => Task.FromResult(new List<CodigoDescripcion>());
    public Task<List<CodigoDescripcion>> ObtenerMedios() => Task.FromResult(_medios.ToList());
    public Task<List<CodigoDescripcion>> ObtenerMediosPorNetWork(string codigosNetwork) => Task.FromResult(new List<CodigoDescripcion>());
    public Task<List<CodigoDescripcion>> ObtenerNetworks() => Task.FromResult(new List<CodigoDescripcion>());
    public Task<List<CodigoDescripcion>> ObtenerGruposClientes() => Task.FromResult(new List<CodigoDescripcion>());
    public Task<List<CodigoDescripcion>> ObtenerGruposClientePorNetworks(string codigosNetworks) => Task.FromResult(new List<CodigoDescripcion>());
    public Task<List<GrupoClientesConNetwork>> ObtenerGruposClientesConNetwork() => Task.FromResult(new List<GrupoClientesConNetwork>());
    public Task<List<CodigoDescripcion>> ObtenerEditoriales(FiltroEditoriales? filtro = null) => Task.FromResult(new List<CodigoDescripcion>());
    public Task<List<CodigoDescripcion>> ObtenerAgrupacionesComerciales(string? codigosMedios = null) => Task.FromResult(new List<CodigoDescripcion>());
    public Task<(List<CodigoDescripcion>, List<CodigoDescripcion>)> ObtenerAgrupacionesYEditoriales(string codigosMedios) => Task.FromResult((new List<CodigoDescripcion>(), new List<CodigoDescripcion>()));
    public Task<List<AgrupacionComercialConMedio>> ObtenerAgrupacionesComercialesConMedio() => Task.FromResult(new List<AgrupacionComercialConMedio>());
    public Task<List<CodigoDescripcion>> ObtenerEditorialesPorAgrupacionComercial(int codigoAgrupacionComercial) => Task.FromResult(new List<CodigoDescripcion>());
    public Task<List<EditorialConAgrupacionComercialAndMedio>> ObtenerEditorialesConAgrupacionComercialAndMedio() => Task.FromResult(new List<EditorialConAgrupacionComercialAndMedio>());
    public Task<List<CodigoDescripcion>> ObtenerTiposDisciplina() => Task.FromResult(new List<CodigoDescripcion>());
    public Task<List<CodigoDescripcion>> ObtenerDisciplinasGrupos() => Task.FromResult(new List<CodigoDescripcion>());
    public Task<List<int>> ObtenerMesCerradoList(int year) => Task.FromResult(new List<int>());

    public Task<List<CodigoDescripcion>> GetAgrupacionEditorialListByMedio(int codeMedio) => Task.FromResult(new List<CodigoDescripcion>());
    public Task<List<CodigoDescripcion>> GetEditorialListByMedio(int codeMedio) => Task.FromResult(new List<CodigoDescripcion>());
    public Task<List<CodigoDescripcion>> GetEditorialListByAgrupacionEditorial(int codeAgrupacionEditorial) => Task.FromResult(new List<CodigoDescripcion>());
    public Task<CodigoDescripcion?> ObtenerMedio(int codigoMedio) => Task.FromResult<CodigoDescripcion?>(null);
    public Task<CodigoDescripcion?> ObtenerAgrupacionComercial(int codigoAgrupacionComercial) => Task.FromResult<CodigoDescripcion?>(null);
    public Task<CodigoDescripcion?> ObtenerEditorial(int codigoEditorial) => Task.FromResult<CodigoDescripcion?>(null);
    public Task<List<CodigoDescripcion>> ObtenerConceptosNMD(int codigoMedio, ConceptosCondicionesNMD concepto, ValoresConceptosNMD valores) => Task.FromResult(new List<CodigoDescripcion>());

    // Helpers
    public void SeedTipologia(CodigoDescripcion cd)
    {
        _tipologias.Add(cd);
    }
}
