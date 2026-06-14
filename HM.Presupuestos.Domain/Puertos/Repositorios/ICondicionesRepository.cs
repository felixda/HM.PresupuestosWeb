using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Domain.Entidades;

namespace HM.Presupuestos.Domain.Puertos
{
    public interface ICondicionesRepository
    {
               
        Task<List<ConceptoCondicion>> ObtenerConceptosDeCondicion();
        Task<List<Vigencia>> ObtenerVigencias(CondicionFiltro filtro);
        Task InsertarVigencia(Vigencia item);

        Task ActualizarVigencia(Vigencia item);

        Task EliminarVigencia(int codigoVigencia);

        Task<bool> LaVigenciaTieneCondiciones(int codigoVigencia);

        Task<List<CondicionDto>> ObtenerCondicionesDeLaVigencia(int codigoVigencia);

        Task GrabarCondicion(Condicion medioCondicion);

        Task EliminarCondicion(Condicion condicion);

        Task<List<ExcepcionDto>> ObtenerExcepcionesDeLaVigencia(int codigoVigencia);

        Task EliminarConceptoDeExcepcion(int codigoCondicionMeido, ConceptosCondicionesNMD concepto);

        Task EliminarExcepcionCondicion(int codigoCondicionMedio);

        Task ActualizarJerarquiaExcepcion(int codigoCondicionMedio, int jerarquia);

        Task GrabarConceptoNMD(int codigoCondicionMedio, ConceptosCondicionesNMD codigoConceptoNMD, int codigo);

        Task<List<int>> ObtenerCodigosDeExcepcionesDeCondicion(Condicion condicion);

        Task<Condicion?> ObtenerCondicionPorClave(Condicion condicion);
      
        Task<Condicion?> ObtenerCondicionPorCodigo(int codigoCondicionMedio);

        Task InsertarCondicion( Condicion condicion);
 
        Task ActualizarCondicion( Condicion condicion);

        Task ImportarCondicionesMMS(CondicionImportarFiltro param);

        ITransaccion ObtenerTransaccion();

    }


}

