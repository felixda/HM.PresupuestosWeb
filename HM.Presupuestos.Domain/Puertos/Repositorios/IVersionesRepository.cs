ï»¿using HM.Presupuestos.Domain.Entidades;
using Version = HM.Presupuestos.Domain.Entidades.Version;

namespace HM.Presupuestos.Domain.Puertos
{
    public interface IVersionesRepository 
    {
        Task<List<Indicador>> ObtenerEstadosVersiones();

        /// <summary>
        /// Devuelve una lista de versiones filtrada
        /// </summary>
        /// <param name="anio">Filtro para el aÃ±o</param>
        /// <param name="estadoIncluido">Filtro para buscar por el BitAnd (Indicador de estado). Para mas de un indicador hay que sumarlos en binario</param>
        /// <returns>Lista de versiones</returns>
        Task<List<Version>> ObtenerVersiones(int anio, int? estadoIncluido = null);
        Task<List<CodigoDescripcion>> ObtenerAniosConVersiones();
        Task<int> InsertarVersion(int codigoPais, Version version);
        Task ActualizarVersion(Version version);
        Task EliminarVersion(int codigoVersion);

        Task<bool> ExistenPrevisionesEnVersion(int codigoVersion);

        Task<bool> ExistenSobreprimasEnVersion(int codigoVersion);
        Task<bool> ExistenCondicionesEnVersion(int codigoVersion);


        /// <summary>
        /// Obtener importes de los medios
        /// </summary>
        /// <param name="json">Filtro con todos los datos necesarios para buscar estos importes (origen, medios y otros)</param>
        Task<List<MedioIncremento>> ObtenerImportesMedios(FiltroComprobarNetoVentaOrigenJSON json);

        Task GrabarCopiasVersiones( DatosCargarVersionDestinoJSON json);

        /// <summary>
        /// Devuelve una lista de versiones resumen filtrada
        /// </summary>
        /// <param name="anio">Filtro opcional para el aÃ±o (null = todos los aÃ±os)</param>
        /// <param name="estadoIncluido">Filtro para buscar por el BitAnd (Indicador de estado). Para mas de un indicador hay que sumarlos</param>
        /// <param name="estadoExcluido">Filtro para buscar excluyendo por el BitAnd (Indicador de estado). Para mas de un indicador hay que sumarlos</param>
        /// <returns>Lista de versiones</returns>
        Task<List<VersionResumen>> ObtenerVersionesResumen(int? anio = null, int? estadoIncluido = null, int? estadoExcluido = null);


        Task<bool> IsDataLinked(int codigoVersion);

        ITransaccion ObtenerTransaccion();
    }
}


