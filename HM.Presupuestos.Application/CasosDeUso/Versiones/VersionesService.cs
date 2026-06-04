using HM.Presupuestos.Domain.Puertos;
using HM.Presupuestos.Domain.Entidades;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Version = HM.Presupuestos.Domain.Entidades.Version;

namespace HM.Presupuestos.Application.CasosDeUso
{
    /// <summary>
    /// Servicio de gestión de versiones de presupuestos
    /// Maneja versiones, indicadores de estado y datos relacionados
    /// </summary>
    public class VersionesService(
        ILogger<VersionesService> logger, 
        IVersionesRepository versionesRepository) : IVersionesService
    {
        private readonly ILogger<VersionesService> _logger = logger;
        private readonly IVersionesRepository _versionesRepository = versionesRepository;

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
        public async Task<List<VersionResumen>> ObtenerVersionesResumen(int? anio = null, int? estadoIncluido = null, int? estadoExcluido = null)
        {
            _logger.LogTrace("Llamando método ObtenerVersionesResumen");
            return await _versionesRepository.ObtenerVersionesResumen(anio, estadoIncluido, estadoExcluido);
        }

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
        public async Task<List<Version>> ObtenerVersiones(int anio, int? estadoIncluido = null)
        {
            _logger.LogTrace("Llamando método ObtenerVersiones");

            // Obtener versiones filtradas
            var versiones = await _versionesRepository.ObtenerVersiones(anio, estadoIncluido);
            
            // Obtener indicadores maestros
            var indicadores = await _versionesRepository.ObtenerEstadosVersiones();
            
            // Calcular estado de indicadores para cada versión
            foreach (var versionItem in versiones)
            {
                foreach (var itemMasterIndicador in indicadores)
                {
                    var itemVersionIndicador = new Version.VersionIndicador
                    {
                        Codigo = itemMasterIndicador.Codigo ?? -1
                    };
                    
                    // Calcular si el indicador está activo mediante operación BitAnd
                    int bitand = itemMasterIndicador.BitAnd;
                    itemVersionIndicador.Estado = ((versionItem.IndEstado & bitand) == bitand);
                    
                    versionItem.IndicadorList.Add(itemVersionIndicador);
                }
                
                // Verificar si tiene datos relacionados
                versionItem.IsDataLinked = await _versionesRepository.IsDataLinked(versionItem.Codigo);
            }
            
            return versiones;
        }

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
        public async Task<List<CodigoDescripcion>> ObtenerAniosConVersiones(bool incluirAnios = false)
        {
            _logger.LogTrace("Llamando método ObtenerAniosConVersiones");

            var resultado = await _versionesRepository.ObtenerAniosConVersiones();
           
            // Añadir años adicionales si se solicita
            if (incluirAnios)
            {
                int anioActual = DateTime.Now.Year;
                int anioAnterior = anioActual - 1;
                int anioPosterior = anioActual + 1;

                // Añadir año anterior si no existe
                if (!resultado.Any(x => x.Codigo == anioAnterior))
                {
                    resultado.Add(new CodigoDescripcion { Codigo = anioAnterior, Descripcion = anioAnterior.ToString() });
                }

                // Añadir año actual si no existe
                if (!resultado.Any(x => x.Codigo == anioActual))
                {
                    resultado.Add(new CodigoDescripcion { Codigo = anioActual, Descripcion = anioActual.ToString() });
                }

                // Añadir año posterior si no existe
                if (!resultado.Any(x => x.Codigo == anioPosterior))
                {
                    resultado.Add(new CodigoDescripcion { Codigo = anioPosterior, Descripcion = anioPosterior.ToString() });
                }
            }

            // Ordenar descendentemente
            return [.. resultado.OrderByDescending(x => x.Codigo)];
        }

        public async Task<bool> EliminarVersion(Version itemVersion)
        {
            var result = true;
            using var transaction = _versionesRepository.ObtenerTransaccion();
            try
            {
                await _versionesRepository.EliminarVersion((int)itemVersion.Codigo);
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                result = false;
                await transaction.RollbackAsync();
                throw;
            }
            return result;
        }

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
        public async Task<bool> GrabarVersiones(List<Version> versionesNuevas, List<Version> versionesModificadas, int codigoPais)
        {
            var result = true;
            using var transaction = _versionesRepository.ObtenerTransaccion();
            try
            {
                // Insertar versiones nuevas
                foreach (var itemNewVersion in versionesNuevas)
                {
                    await _versionesRepository.InsertarVersion(codigoPais, itemNewVersion);
                    // Serializar para registro interno
                    string json = JsonSerializer.Serialize(itemNewVersion, new JsonSerializerOptions { WriteIndented = true });
                }

                // Actualizar versiones modificadas
                foreach (var itemUpdatedVersion in versionesModificadas)
                {
                    await _versionesRepository.ActualizarVersion(itemUpdatedVersion);
                    // Serializar para registro interno
                    string json = JsonSerializer.Serialize(itemUpdatedVersion, new JsonSerializerOptions { WriteIndented = true });
                }
                
                await transaction.CommitAsync();
            }
            catch
            {
                result = false;
                await transaction.RollbackAsync();
                throw;
            }
            return result;
        }

        #region Métodos Deprecated

        // Deprecated - No referenciado
        public async Task<bool> ExistenPrevisionesEnVersion(int codigoVersion)
        {
            _logger.LogTrace("Llamando método ExistenPrevisionesEnVersion");
            return await _versionesRepository.ExistenPrevisionesEnVersion(codigoVersion);
        }

        // Deprecated - No referenciado
        public async Task<bool> ExistenCondicionesEnVersion(int codigoVersion)
        {
            _logger.LogTrace("Llamando método ExistenCondicionesEnVersion");
            return await _versionesRepository.ExistenCondicionesEnVersion(codigoVersion);
        }

        // Deprecated - No referenciado
        public async Task<bool> ExistenSobreprimasEnVersion(int codigoVersion)
        {
            _logger.LogTrace("Llamando método ExistenSobreprimasEnVersion");
            return await _versionesRepository.ExistenSobreprimasEnVersion(codigoVersion);
        }

        // Deprecated - No referenciado
        public async Task<bool> ExistenDatosRelacionadosConVersion(int codigoVersion)
        {
            _logger.LogTrace("Llamando método ExistenDatosRelacionadosConVersion");
            return await _versionesRepository.IsDataLinked(codigoVersion);
        }

        #endregion

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
        public async Task<List<MedioIncremento>> ObtenerImportesMedios(FiltroComprobarNetoVentaOrigenJSON json)
        {
            _logger.LogInformation("Llamando método ObtenerImportesMedios");
            return await _versionesRepository.ObtenerImportesMedios(json);
        }
    }
}
