
using HM.Presupuestos.Domain.Comun;
using HM.Presupuestos.Domain.Entidades;

namespace HM.Presupuestos.Domain.Puertos
{
    public interface IPresupuestosRepository 
    {
       
        Task<List<CodigoDescripcion>> ObtenerTipologias();
        
        Task<List<CodigoDescripcion>> ObtenerAlcances();
        
        Task<List<CodigoDescripcion>> ObtenerDiversifiedsNCB();
        
        Task<List<CodigoDescripcion>> ObtenerDisciplinas();
        
        Task<List<CodigoDescripcion>> ObtenerTiposCompra();
        
        Task<List<CodigoDescripcion>> ObtenerObjetivos();


        Task<List<CodigoDescripcion>> ObtenerMedios();

        Task<List<CodigoDescripcion>> ObtenerMediosPorNetWork(string codigosNetwork);

        Task<List<CodigoDescripcion>> ObtenerNetworks();

        Task<List<CodigoDescripcion>> ObtenerGruposClientes();
        Task<List<GrupoClientesConNetwork>> ObtenerGruposClientesConNetwork();

        Task<List<CodigoDescripcion>> ObtenerGruposClientePorNetworks(string codigosNetworks);

        //Task<List<CodigoDescripcion>> ObtenerEditoriales();
        Task<List<CodigoDescripcion>> ObtenerEditoriales(FiltroEditoriales? filtro = null);
        Task<List<CodigoDescripcion>> ObtenerEditorialesPorAgrupacionComercial(int codigoAgrupacionComercial );
        Task<List<EditorialConAgrupacionComercialAndMedio>> ObtenerEditorialesConAgrupacionComercialAndMedio();
        Task<List<CodigoDescripcion>> ObtenerAgrupacionesComerciales(string? codigosMedios = null);

        Task<List<AgrupacionComercialConMedio>> ObtenerAgrupacionesComercialesConMedio();

        Task<List<CodigoDescripcion>> ObtenerTiposDisciplina();
        Task<List<CodigoDescripcion>> ObtenerDisciplinasGrupos();
        Task<List<int>> ObtenerMesCerradoList(int year);
        Task<List<CodigoDescripcion>> GetAgrupacionEditorialListByMedio(int codeMedio);
        Task<List<CodigoDescripcion>> GetEditorialListByMedio(int codeMedio);
        Task<List<CodigoDescripcion>> GetEditorialListByAgrupacionEditorial(int codeAgrupacionEditorial);

        Task<CodigoDescripcion?> ObtenerMedio(int codigoMedio);

        Task<CodigoDescripcion?> ObtenerAgrupacionComercial(int codigoAgrupacionComercial);
        Task<CodigoDescripcion?> ObtenerEditorial(int codigoEditorial);

        Task<List<CodigoDescripcion>> ObtenerConceptosNMD(int codigoMedio, ConceptosCondicionesNMD concepto, ValoresConceptosNMD valores);
    }

}
