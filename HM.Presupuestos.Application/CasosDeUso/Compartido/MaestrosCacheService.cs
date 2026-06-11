using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Domain.Entidades;
using Microsoft.Extensions.Options;

namespace HM.Presupuestos.Application.CasosDeUso.Compartido
{
    /// <summary>
    /// Decorator sobre IMaestrosService que cachea datos maestros cuasi-estáticos
    /// durante la sesión del usuario. Al ser AddScoped, cada circuito Blazor dispone
    /// de su propia instancia, garantizando aislamiento entre usuarios sin necesidad
    /// de incluir el identificador de usuario en las claves de caché.
    /// </summary>
    public class MaestrosCacheService(
        IMaestrosService inner,
        IOptions<MaestrosCacheOptions> opciones) : IMaestrosCacheService
    {
        private readonly Dictionary<string, (object? Valor, DateTimeOffset ExpiraEn)> _cache = [];
        private readonly TimeSpan _ttl = TimeSpan.FromMinutes(opciones.Value.TtlMinutos);

        // Claves de caché para los 6 métodos cuasi-estáticos
        private const string ClaveNetworks = "networks";
        private const string ClaveMedios = "medios";
        private const string PrefijoClaveMediosPorNetwork = "medios-por-network:";
        private const string PrefijoClaveAgrupaciones = "agrupaciones:";
        private const string PrefijoClaveEditoriales = "editoriales:";
        private const string PrefijoClaveAgrupacionesYEditoriales = "agrupaciones-editoriales:";

        #region Invalidación

        public void Invalidar(string recurso) => _cache.Remove(recurso);

        public void InvalidarTodo() => _cache.Clear();

        #endregion

        #region Helpers

        private async Task<T> ObtenerOCachear<T>(string clave, Func<Task<T>> obtener)
        {
            if (_cache.TryGetValue(clave, out var entrada) && DateTimeOffset.UtcNow < entrada.ExpiraEn)
                return (T)entrada.Valor!;

            var valor = await obtener();
            _cache[clave] = (valor, DateTimeOffset.UtcNow.Add(_ttl));
            return valor;
        }

        #endregion

        #region Métodos cacheados (6)

        public Task<List<CodigoDescripcion>> ObtenerNetworks() =>
            ObtenerOCachear(ClaveNetworks, inner.ObtenerNetworks);

        public Task<List<CodigoDescripcion>> ObtenerMedios() =>
            ObtenerOCachear(ClaveMedios, inner.ObtenerMedios);

        public Task<List<CodigoDescripcion>> ObtenerMediosPorNetWork(string codigosNetwork) =>
            ObtenerOCachear($"{PrefijoClaveMediosPorNetwork}{codigosNetwork}", () => inner.ObtenerMediosPorNetWork(codigosNetwork));

        public Task<List<CodigoDescripcion>> ObtenerAgrupacionesComerciales(string? codigosMedios = null) =>
            ObtenerOCachear($"{PrefijoClaveAgrupaciones}{codigosMedios ?? string.Empty}", () => inner.ObtenerAgrupacionesComerciales(codigosMedios));

        public Task<List<CodigoDescripcion>> ObtenerEditoriales(FiltroEditoriales? filtro = null)
        {
            string claveParams = $"{filtro?.CodigosMedios ?? string.Empty}:{filtro?.CodigosAgrupacionesComerciales ?? string.Empty}";
            return ObtenerOCachear($"{PrefijoClaveEditoriales}{claveParams}", () => inner.ObtenerEditoriales(filtro));
        }

        public Task<(List<CodigoDescripcion> Agrupaciones, List<CodigoDescripcion> Editoriales)> ObtenerAgrupacionesYEditoriales(string codigosMedios) =>
            ObtenerOCachear($"{PrefijoClaveAgrupacionesYEditoriales}{codigosMedios}", () => inner.ObtenerAgrupacionesYEditoriales(codigosMedios));

        #endregion

        #region Métodos delegados directamente (sin caché)

        public Task<List<CodigoDescripcion>> ObtenerTipologias() => inner.ObtenerTipologias();
        public Task<List<CodigoDescripcion>> ObtenerAlcances() => inner.ObtenerAlcances();
        public Task<List<CodigoDescripcion>> ObtenerDiversifiedsNCB() => inner.ObtenerDiversifiedsNCB();
        public Task<List<CodigoDescripcion>> ObtenerDisciplinas() => inner.ObtenerDisciplinas();
        public Task<List<CodigoDescripcion>> ObtenerTiposCompra() => inner.ObtenerTiposCompra();
        public Task<List<CodigoDescripcion>> ObtenerObjetivos() => inner.ObtenerObjetivos();
        public Task<List<CodigoDescripcion>> ObtenerGruposClientes() => inner.ObtenerGruposClientes();
        public Task<List<CodigoDescripcion>> ObtenerGruposClientePorNetwork(int codigoNetwork) => inner.ObtenerGruposClientePorNetwork(codigoNetwork);
        public Task<List<CodigoDescripcion>> ObtenerGruposClientePorNetworks(string codigosNetworks) => inner.ObtenerGruposClientePorNetworks(codigosNetworks);
        public Task<List<GrupoClientesConNetwork>> ObtenerGruposClientesConNetwork() => inner.ObtenerGruposClientesConNetwork();
        public Task<List<AgrupacionComercialConMedio>> ObtenerAgrupacionesComercialesConMedio() => inner.ObtenerAgrupacionesComercialesConMedio();
        public Task<List<CodigoDescripcion>> ObtenerEditorialesPorAgrupacionComercial(int codigoAgrupacionComercial) => inner.ObtenerEditorialesPorAgrupacionComercial(codigoAgrupacionComercial);
        public Task<List<EditorialConAgrupacionComercialAndMedio>> ObtenerEditorialesConAgrupacionComercialAndMedio() => inner.ObtenerEditorialesConAgrupacionComercialAndMedio();
        public Task<List<CodigoDescripcion>> ObtenerTiposDisciplinas() => inner.ObtenerTiposDisciplinas();
        public Task<List<CodigoDescripcion>> ObtenerDisciplinasGrupos() => inner.ObtenerDisciplinasGrupos();
        public Task<List<int>> ObtenerMesCerradoList(int year) => inner.ObtenerMesCerradoList(year);
        public Task<List<CodigoDescripcion>> GetAgrupacionEditorialListByMedio(int codeMedio) => inner.GetAgrupacionEditorialListByMedio(codeMedio);
        public Task<List<CodigoDescripcion>> GetEditorialListByMedio(int codeMedio) => inner.GetEditorialListByMedio(codeMedio);
        public Task<List<CodigoDescripcion>> GetEditorialListByAgrupacionEditorial(int codeAgrupacionEditorial) => inner.GetEditorialListByAgrupacionEditorial(codeAgrupacionEditorial);
        public Task<CodigoDescripcion?> ObtenerMedio(int codigoMedio) => inner.ObtenerMedio(codigoMedio);
        public Task<CodigoDescripcion?> ObtenerAgrupacionComercial(int codigoAgrupacionComercial) => inner.ObtenerAgrupacionComercial(codigoAgrupacionComercial);
        public Task<CodigoDescripcion?> ObtenerEditorial(int codigoEditorial) => inner.ObtenerEditorial(codigoEditorial);
        public Task<List<CodigoDescripcion>> ObtenerConceptosNMD(int codigoMedio, ConceptosCondicionesNMD concepto, ValoresConceptosNMD valores) => inner.ObtenerConceptosNMD(codigoMedio, concepto, valores);

        #endregion
    }
}
