using HM.Presupuestos.Application.CasosDeUso.LogAcciones;
using HM.Presupuestos.Domain.Puertos;
using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Domain.Entidades;
using Microsoft.Extensions.Logging;

namespace HM.Presupuestos.Application.CasosDeUso
{
    /// <summary>
    /// Servicio de gestión de indicadores de versiones de presupuestos
    /// Gestiona indicadores y sus traducciones en múltiples idiomas
    /// </summary>
    public class IndicadoresService(
        ILogger<IndicadoresService> logger,
        IIndicadoresRepository indicadoresRepository,
        IVersionesService versionesService,
        ILogAccionesService logAccionesService) : IIndicadoresService
    {
        private readonly ILogger<IndicadoresService> _logger = logger;
        private readonly IIndicadoresRepository _indicadoresRepository = indicadoresRepository;
        private readonly IVersionesService _versionesService = versionesService;
        private readonly ILogAccionesService _logAccionesService = logAccionesService;


        /// <summary>
        /// Obtiene la lista completa de indicadores con sus traducciones en todos los idiomas disponibles
        /// </summary>
        /// <param name="descripcion">Filtro opcional por descripción del indicador (búsqueda parcial)</param>
        /// <returns>Lista de objetos Indicador con sus traducciones (IdiomaIndicador) asociadas</returns>
        /// <remarks>
        /// Cada indicador incluye:
        /// - Datos base: Código, Descripción, BitAnd, Orden, IndMostrar, IndVersionUnica
        /// - Lista de traducciones (Idiomas): para cada idioma disponible con descripción, abreviatura y leyenda
        /// Si se proporciona un filtro de descripción, solo devuelve indicadores que coincidan parcialmente
        /// </remarks>
        public async Task<List<Indicador>> ObtenerIndicadoresConIdiomas(string? descripcion = null)
        {
            _logger.LogTrace("Comenzando ObtenerIndicadoresConIdiomas");

            var resultado = await _indicadoresRepository.ObtenerIndicadoresConIdiomas(descripcion);

            _logger.LogTrace("Terminando ObtenerIndicadoresConIdiomas - {IndicadoresObtenidos} indicadores obtenidos", resultado.Count);

            return resultado;
        }

        /// <summary>
        /// Guarda un indicador con sus traducciones en idiomas, realizando inserción o actualización según el estado
        /// </summary>
        /// <param name="indicador">Indicador a guardar (puede ser nuevo o existente)</param>
        /// <param name="idiomasNuevos">Lista de idiomas a insertar asociados al indicador</param>
        /// <param name="idiomasActualizar">Lista de idiomas existentes a actualizar</param>
        /// <param name="idiomasEliminar">Lista de idiomas a eliminar del indicador</param>
        /// <exception cref="ValidacionException">Si el indicador tiene valores duplicados (Descripción, Orden o BitAnd)</exception>
        /// <exception cref="Exception">Si ocurre un error durante el guardado o el rollback de la transacción</exception>
        /// <remarks>
        /// Este método realiza las siguientes operaciones en una transacción:
        /// 1. Valida que no existan duplicados de Descripción, Orden o BitAnd
        /// 2. Inserta o actualiza el indicador según su estado (Nuevo/Modificado)
        /// 3. Elimina los idiomas marcados para eliminación
        /// 4. Inserta los nuevos idiomas asociados
        /// 5. Actualiza los idiomas existentes modificados
        /// 6. Registra la acción de auditoría después del commit exitoso
        /// </remarks>
        public async Task Grabar(Indicador indicador, 
            List<IdiomaIndicador> idiomasNuevos, 
            List<IdiomaIndicador> idiomasActualizar, 
            List<IdiomaIndicador> idiomasEliminar)
        {
            _logger.LogTrace("Llamando método Grabar");

            // Validar unicidad de campos clave
            if (indicador.Estado != EstadoEntidad.SinCambios)
            {
                bool existe = await _indicadoresRepository.ExisteIndicador(indicador);
                if (existe)
                {
                    throw new ValidacionException(CampoErrorValidacion.Descripcion, indicador.Descripcion);
                }

                existe = await _indicadoresRepository.ExisteOrden(indicador);
                if (existe)
                {
                    throw new ValidacionException(CampoErrorValidacion.Orden, indicador.Orden.ToString());
                }

                existe = await _indicadoresRepository.ExisteBitAnd(indicador);
                if (existe)
                {
                    throw new ValidacionException(CampoErrorValidacion.BitAnd, indicador.BitAnd.ToString());
                }
            }

            using var transaction = _indicadoresRepository.ObtenerTransaccion();
            try
            {
                // Insertar o actualizar indicador principal
                if (indicador.Estado == EstadoEntidad.Nuevo)
                {
                    indicador.Codigo = await _indicadoresRepository.InsertarIndicador(indicador);
                }
                else if (indicador.Estado == EstadoEntidad.Modificado)
                {
                    await _indicadoresRepository.ActualizarIndicador(indicador);
                }

                // Eliminar idiomas marcados
                foreach (var idioma in idiomasEliminar)
                {
                    if (idioma != null && idioma.Codigo != null)
                        await _indicadoresRepository.EliminarIdiomaIndicador((int)idioma.Codigo);
                }

                // Insertar nuevos idiomas
                foreach (var idioma in idiomasNuevos)
                {
                    idioma.CodigoIndicador = indicador.Codigo;
                    await _indicadoresRepository.InsertarIdiomaIndicador(idioma);
                }

                // Actualizar idiomas existentes
                foreach (var idioma in idiomasActualizar)
                {
                    await _indicadoresRepository.ActualizarIdiomaIndicador(idioma);
                }

                await transaction.CommitAsync();

                // Registrar auditoría fuera de la transacción
                try
                {
                    await _logAccionesService.Insertar(
                        AccionesLog.GrabarIndicador,
                        indicador);
                }
                catch (Exception logEx)
                {
                    _logger.LogError(logEx, "Error registrando auditoría (grabado exitoso)");
                    // No propagar - el guardado fue exitoso
                }

                _logger.LogTrace("Indicador grabado correctamente. Código: {CodigoIndicador}", indicador.Codigo);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        /// <summary>
        /// Elimina un indicador y actualiza el estado de todas las versiones relacionadas
        /// </summary>
        /// <param name="indicador">Indicador a eliminar con su código y datos asociados</param>
        /// <exception cref="Exception">Si ocurre un error durante la eliminación o el rollback de la transacción</exception>
        /// <remarks>
        /// Este método realiza las siguientes operaciones en una transacción:
        /// 1. Elimina los idiomas asociados al indicador
        /// 2. Elimina el indicador de la base de datos
        /// 3. Actualiza el campo IndEstado de todas las versiones restando el BitAnd del indicador eliminado
        /// 4. Registra la acción de auditoría después del commit exitoso
        /// Nota: La obtención de versiones se realiza fuera de la transacción principal por limitaciones del repositorio
        /// </remarks>
        public async Task Eliminar(Indicador indicador)
        {
            _logger.LogTrace("Llamando método Eliminar");

            // Obtener versiones fuera de la transacción
            // No puede ir dentro del commit porque es de otro repositorio distinto y ahora mismo no lo admite
            List<VersionResumen> versiones = await _versionesService.ObtenerVersionesResumen();

            using var transaction = _indicadoresRepository.ObtenerTransaccion();
            try
            {
                int codigoIndicador = indicador.Codigo!.Value;
                int bitAnd = await _indicadoresRepository.ObtenerBitAndIndicador(codigoIndicador);

                // Eliminar idiomas y el indicador
                await _indicadoresRepository.EliminarIdiomasIndicador(codigoIndicador);
                await _indicadoresRepository.EliminarIndicador(codigoIndicador);

                // Actualizar el estado de las versiones
                // Hay que restar el valor del BITAND del indicador eliminado en todas las versiones
                // (siempre que sea menor o igual que el actual de la versión)
                foreach (var version in versiones)
                {
                    if (bitAnd <= version.IndEstado)
                    {
                        version.IndEstado = version.IndEstado - bitAnd;
                        // Utilizamos version.Codigo!.Value porque sabemos con seguridad que nunca será null
                        await _indicadoresRepository.Actualizar1BitAndVersion((int)version.Codigo, version.IndEstado);
                    }
                }

                await transaction.CommitAsync();

                // Registrar auditoría fuera de la transacción
                try
                { 
                    await _logAccionesService.Insertar(
                        AccionesLog.EliminarIndicador,
                        indicador);
                }
                catch (Exception logEx)
                {
                    _logger.LogError(logEx, "Error registrando auditoría (eliminación exitosa)");
                    // No propagar - la eliminación fue exitosa
                }

                _logger.LogTrace("Indicador eliminado correctamente. Código: {CodigoIndicador}", codigoIndicador);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<int> ObtenerUltimoBitAnd()
        {
           // _logger.LogTrace("Llamando método ObtenerUltimoBitAnd");
           // return await _indicadoresRepository.ObtenerUltimoBitAnd();

            _logger.LogTrace("Llamando método ObtenerUltimoBitAnd");
            var resultado = await _indicadoresRepository.ObtenerUltimoBitAnd();
            return resultado;// * 2; // BUG provocado para probar test: multiplicación incorrecta
        }

        public async Task<int> ObtenerUltimoOrden()
        {
            _logger.LogTrace("Llamando método ObtenerUltimoOrden");
            return await _indicadoresRepository.ObtenerUltimoOrden();
        }
    }
}


