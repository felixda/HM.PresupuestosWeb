using HM.Presupuestos.Domain.Entidades;
using Version = HM.Presupuestos.Domain.Entidades.Version;

namespace HM.Presupuestos.Application.Servicios
{
    /// <summary>
    /// Interfaz del servicio de gestión de versiones de presupuestos
    /// </summary>
    public interface IVersionesService
    {
        /// <summary>
        /// Obtiene una lista resumida de versiones filtrada por año y estado de indicadores
        /// </summary>
        /// <param name="anio">Año para filtrar las versiones</param>
        /// <param name="estadoIncluido">Filtro para buscar versiones que incluyan indicadores específicos mediante BitAnd. 
        /// Para múltiples indicadores, sumar sus valores BitAnd (ej: BitAnd1 + BitAnd2)</param>
        /// <param name="estadoExcluido">Filtro para excluir versiones que contengan indicadores específicos mediante BitAnd. 
        /// Para múltiples indicadores, sumar sus valores BitAnd</param>
        /// <returns>Lista de versiones resumidas que cumplen los criterios de filtrado</returns>
        /// <remarks>
        /// Los filtros de estado utilizan operaciones BitAnd para verificar la presencia o ausencia de indicadores.
        /// Ejemplo: Si un indicador tiene BitAnd=1 y otro BitAnd=2, para buscar versiones con ambos usar estadoIncluido=3
        /// </remarks>
        /// 

        Task<List<VersionResumen>> ObtenerVersionesResumen(int? anio = null, int? estadoIncluido = null, int? estadoExcluido = null);

        /// <summary>
        /// Obtiene una lista completa de versiones con sus indicadores calculados y estado de vinculación de datos
        /// </summary>
        /// <param name="anio">Año para filtrar las versiones</param>
        /// <param name="estadoIncluido">Filtro opcional para buscar versiones que incluyan indicadores específicos mediante BitAnd. 
        /// Para múltiples indicadores, sumar sus valores BitAnd en binario</param>
        /// <returns>Lista de versiones con su lista de indicadores (VersionIndicador) calculados y el estado IsDataLinked</returns>
        /// <remarks>
        /// Este método realiza las siguientes operaciones:
        /// 1. Obtiene las versiones filtradas por año y estado opcional
        /// 2. Obtiene todos los indicadores maestros (estados de versiones)
        /// 3. Para cada versión, calcula qué indicadores están activos usando operaciones BitAnd
        /// 4. Verifica si la versión tiene datos relacionados (previsiones, condiciones, sobreprimas)
        /// El cálculo de indicadores: (versionItem.IndEstado &amp; bitand) == bitand determina si el indicador está activo
        /// </remarks>
        Task<List<Version>> ObtenerVersiones(int anio, int? estadoIncluido = null);

        /// <summary>
        /// Obtiene una lista de años que tienen versiones asociadas, con opción de incluir años adicionales
        /// </summary>
        /// <param name="incluirAnios">Si es true, incluye el año anterior, actual y posterior aunque no tengan versiones en BD</param>
        /// <returns>Lista de años con versiones, ordenada descendentemente</returns>
        /// <remarks>
        /// Si incluirAnios=true y alguno de estos años no existe en BD, se añade automáticamente:
        /// - Año anterior al actual
        /// - Año actual
        /// - Año posterior al actual
        /// Útil para formularios donde se necesita seleccionar años incluso sin versiones creadas
        /// </remarks>
        Task<List<CodigoDescripcion>> ObtenerAniosConVersiones(bool incluirAnios = false);

        Task<bool> EliminarVersion(Version itemVersion);

        /// <summary>
        /// Guarda listas de versiones nuevas y modificadas en una única transacción
        /// </summary>
        /// <param name="versionesNuevas">Lista de versiones nuevas a insertar</param>
        /// <param name="versionesModificadas">Lista de versiones existentes a actualizar</param>
        /// <param name="codigoPais">Código del país asociado a las versiones</param>
        /// <returns>True si la operación fue exitosa, false en caso contrario</returns>
        /// <remarks>
        /// Este método procesa ambas listas en una única transacción:
        /// 1. Inserta todas las versiones nuevas
        /// 2. Actualiza todas las versiones modificadas
        /// 3. Si cualquier operación falla, hace rollback de todas las operaciones
        /// Las versiones se serializan a JSON para registro interno
        /// </remarks>
        Task<bool> GrabarVersiones(List<Version> versionesNuevas, List<Version> versionesModificadas, int codigoPais);

        Task<bool> ExistenPrevisionesEnVersion(int codigoVersion);
        
        Task<bool> ExistenCondicionesEnVersion(int codigoVersion);
        
        Task<bool> ExistenSobreprimasEnVersion(int codigoVersion);
        
        Task<bool> ExistenDatosRelacionadosConVersion(int codigoVersion);

        /// <summary>
        /// Obtiene los importes de medios calculados según criterios de origen y filtros específicos
        /// </summary>
        /// <param name="json">Objeto JSON con filtros complejos que incluye: origen de datos, lista de medios, 
        /// periodos, tipos de compra y otros criterios de filtrado para el cálculo de importes</param>
        /// <returns>Lista de medios con sus incrementos calculados basados en los criterios especificados</returns>
        /// <remarks>
        /// Este método realiza cálculos complejos de importes considerando:
        /// - Origen de datos (previsiones, reales, etc.)
        /// - Filtros por medios específicos
        /// - Periodos temporales
        /// - Otros criterios de negocio definidos en el objeto JSON
        /// Utilizado principalmente para análisis de neto venta y comparativas de presupuestos
        /// </remarks>
        Task<List<MedioIncremento>> ObtenerImportesMedios(FiltroComprobarNetoVentaOrigenJSON json);
    }
}