using HM.Presupuestos.Domain.Comun;
using HM.Presupuestos.Domain.Entidades;

namespace HM.Presupuestos.Domain.Puertos
{
    public interface ICondicionesRepository
    {
               
        Task<List<ConceptoCondicion>> ObtenerConceptos();
        Task<List<Vigencia>> ObtenerVigencias(CondicionFiltro filtro);
        Task InsertarVigencia(Vigencia item);

        Task ActualizarVigencia(Vigencia item);

        Task EliminarVigencia(int codigoVigencia);

        Task<bool> ExistenCondicionesVigencias(int codigoVigencia);

        Task<List<CondicionDto>> ObtenerCondicionesPorVigencia(int codigoVigencia);

        Task GrabarCondicion(Condicion medioCondicion);

        Task EliminarCondicion(Condicion condicion);

        Task<List<ExcepcionDto>> ObtenerExcepcionesCondiciones(int codigoVigencia);

        Task EliminarConceptoNMDExcepcionCondicion(int codigoCondicionMeido, ConceptosCondicionesNMD concepto);

        Task EliminarExcepcionCondicion(int codigoCondicionMedio);

        Task ActualizarJerarquiaExcepcion(int codigoCondicionMedio, int jerarquia);

        Task GrabarConceptoNMD(int codigoCondicionMedio, ConceptosCondicionesNMD codigoConceptoNMD, int codigo);

        Task<List<int>> ObtenerCodigosExcepcionesCondiciones(Condicion condicion);

        Task<Condicion?> ObtenerExcepcionOrCondicion(Condicion item);
      
        Task<Condicion?> ObtenerExcepcionOrCondicion(int codigoCondicionMedio);

        Task InsertarCondicion( Condicion condicion);
 
        Task ActualizarCondicion( Condicion condicion);

        Task ImportarCondicionesMMS(CondicionImportarFiltro param);

    }


}
