using HM.Presupuestos.Domain.Puertos;
using HM.Presupuestos.Domain.Comun;
using HM.Presupuestos.Domain.Entidades;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace HM.Presupuestos.Application.Servicios
{ 
    /// <summary>
    /// Servicio de gestión de condiciones comerciales, excepciones y vigencias
    /// Gestiona condiciones SAG, Manpower, Devolución y sus excepciones por medio con conceptos NMD
    /// </summary>
    /// <remarks>
    /// 📊 Arquitectura CORRECTA:
    /// 
    /// CondicionesService 
    /// ├─> condicionesClientesRepository  ✅ OK (su dominio)
    /// ├─> presupuestosService            ✅ OK (otro servicio)
    /// └─> logAccionesService             ✅ OK (otro servicio)
    /// 
    /// Ventajas de usar servicios en lugar de repositorios directos:
    /// - Arquitectura: Respeta capas
    /// - Encapsulación: Encapsula lógica de negocio
    /// - Mantenibilidad: Desacoplado
    /// - Testabilidad: Más simple
    /// - Reutilización: Lógica centralizada
    /// - Evolución: Cambio en un solo lugar
    /// </remarks>
    public class CondicionesService(
        ILogger<CondicionesService> logger, 
        ICondicionesRepository condicionesRepository, 
        IPresupuestosService presupuestosService, 
        ILogAccionesService logAccionesService) : ICondicionesService
    {
        private readonly ILogger<CondicionesService> _logger = logger;
        private readonly ICondicionesRepository _condicionesRepository = condicionesRepository;
        private readonly IPresupuestosService _presupuestosService = presupuestosService;
        private readonly ILogAccionesService _logAccionesService = logAccionesService;

        /// <summary>
        /// Record para rastrear cambios en datos de condiciones
        /// </summary>
        public record DatosCondicionCambiados(TiposCambiosdeDatos TipoCambio, HashSet<string> CamposCambiados);

        /// <summary>
        /// Record para rastrear cambios en datos de excepciones de condiciones
        /// </summary>
        public record DatosExcepcionesCondicionCambiados(TiposCambiosdeDatos TipoCambio, HashSet<string> CamposCambiados);

        #region Vigencias

        public Task<List<Vigencia>> ObtenerVigencias(CondicionFiltro filtro)
        {
            _logger.LogTrace("Llamando método ObtenerVigencias");
            return _condicionesRepository.ObtenerVigencias(filtro);
        }

        public async Task InsertarVigencia(Vigencia item)
        {
            _logger.LogTrace("Llamando método InsertarVigencia");
            await _condicionesRepository.InsertarVigencia(item);
        }

        public async Task ActualizarVigencia( Vigencia vigencia)
        {
            _logger.LogTrace("Llamando método ActualizarVigencia");
            await _condicionesRepository.ActualizarVigencia(vigencia);
        }

        /// <summary>
        /// Valida que una vigencia no se solape con otras vigencias existentes
        /// </summary>
        /// <param name="vigencia">Vigencia a validar con sus fechas desde/hasta</param>
        /// <returns>True si no hay solapamiento, false si existe al menos un solapamiento</returns>
        /// <remarks>
        /// Este método realiza las siguientes validaciones:
        /// 1. Busca vigencias con los mismos criterios (versión, grupo cliente, network, acuerdo)
        /// 2. Excluye la propia vigencia si ya existe (comparación por código)
        /// 3. Verifica solapamiento de rangos: [MesDesde..MesHasta]
        /// La lógica de solapamiento: vigencia.MesDesde &lt;= existente.MesHasta AND existente.MesDesde &lt;= vigencia.MesHasta
        /// </remarks>
        public async Task<bool> ValidarSolapesVigencia(Vigencia vigencia)
        {
            CondicionFiltro filtro = new()
            {
                CodigoVersion = vigencia.CodigoVersion,
                CodigoGrupoCliente = vigencia.CodigoGrupoCliente,
                CodigoNetwork = vigencia.CodigoNetWork,
                IndicadorAcuerdo = vigencia.IndicadorAcuerdo
            };

            List<Vigencia> vigencias = await _condicionesRepository.ObtenerVigencias(filtro);

            if (vigencias.Count == 0)
                return true;

            // Excluir el propio item
            vigencias.RemoveAll(c => c.Codigo == vigencia.Codigo);

            // Verificar solapamiento: [Desde..Hasta]
            var seSolapa = vigencias.Any(r =>
                vigencia.MesDesde <= r.MesHasta &&
                r.MesDesde <= vigencia.MesHasta);

            return !seSolapa;
        }

        /// <summary>
        /// Elimina una vigencia ejecutando un procedimiento almacenado que borra condiciones, excepciones y conceptos asociados
        /// </summary>
        /// <param name="vigencia">Vigencia a eliminar</param>
        /// <remarks>
        /// El PL de base de datos se encarga de eliminar en cascada:
        /// - Condiciones de la vigencia
        /// - Excepciones asociadas
        /// - Conceptos NMD de las excepciones
        /// Se registra la acción de auditoría después de la eliminación exitosa
        /// </remarks>
        public async Task EliminarVigencia(Vigencia vigencia)
        {
            _logger.LogTrace("Llamando método EliminarVigencia");
            await _condicionesRepository.EliminarVigencia(vigencia.Codigo);

            // Registrar auditoría después de eliminación exitosa
            try
            {
                await _logAccionesService.Insertar(
                    AccionesLog.EliminarVigencia,
                    vigencia);
            }
            catch (Exception logEx)
            {
                _logger.LogError(logEx, "Error registrando auditoría (eliminación exitosa)");
                // No propagar - la eliminación fue exitosa
            }
        }

        public Task<bool> ExistenCondicionesVigencias(int codigoVigencia)
        {
            _logger.LogTrace("Llamando método ExistenCondicionesVigencias");
            return _condicionesRepository.ExistenCondicionesVigencias(codigoVigencia);
        }

        #endregion

        /// <summary>
        /// Obtiene las condiciones por vigencia o devuelve una colección vacía basada en medios del network
        /// </summary>
        /// <param name="codigoVigencia">Código de la vigencia</param>
        /// <param name="codigoNetwork">Código del network para obtener medios si no hay condiciones</param>
        /// <returns>Lista de condiciones existentes o estructura vacía con medios del network</returns>
        /// <remarks>
        /// Lógica del método:
        /// 1. Si existen condiciones para la vigencia → las devuelve
        /// 2. Si NO existen condiciones → devuelve una colección con CondicionDto vacíos para cada medio del network
        /// Esto permite que la UI muestre todos los medios disponibles aunque no tengan condiciones configuradas
        /// </remarks>
        public async Task<List<CondicionDto>> ObtenerCondicionesPorVigencia(int codigoVigencia, int codigoNetwork)
        {
            _logger.LogTrace("Llamando método ObtenerCondicionesPorVigencia");

            List<CondicionDto> resultado = await _condicionesRepository.ObtenerCondicionesPorVigencia(codigoVigencia);

            if (resultado.Count > 0) return resultado;

            // Si no hay datos, devolvemos la colección de condiciones para cada medio accesible por network
            List<CodigoDescripcion> medios = await _presupuestosService.ObtenerMediosPorNetWork(codigoNetwork.ToString());

            return [.. medios.Select(m => new CondicionDto
            {
                CodigoMedio = m.Codigo,
                DescripcionMedio = m.Descripcion
            })];
        }

        public Task<List<ExcepcionDto>> ObtenerExcepcionesCondiciones(int codigoVigencia)
        {
            _logger.LogTrace("Llamando método ObtenerExcepcionesCondiciones");
            return _condicionesRepository.ObtenerExcepcionesCondiciones(codigoVigencia);
        }

        /// <summary>
        /// Guarda condiciones y excepciones comerciales con sus conceptos asociados en una única transacción
        /// </summary>
        /// <param name="condicionesNoGuardadas">Diccionario de condiciones modificadas con sus campos cambiados</param>
        /// <param name="excepcionesNoGuardadas">Diccionario de excepciones modificadas con sus campos cambiados</param>
        /// <param name="codigoVigencia">Código de la vigencia a la que pertenecen</param>
        /// <remarks>
        /// Este método realiza un proceso complejo en una transacción:
        /// 
        /// CONDICIONES:
        /// - Para cada condición procesa 3 conceptos: SAG, Manpower, Devolución
        /// - Inserta si es nueva y tiene porcentaje
        /// - Actualiza si cambió el porcentaje o indicador de devolución
        /// - Elimina si el porcentaje pasa a null (también elimina excepciones relacionadas)
        /// 
        /// EXCEPCIONES:
        /// - Pre-tratamiento: Si cambió jerarquía, pone valores negativos temporales para evitar duplicados UK
        /// - Para cada excepción procesa 3 conceptos: SAG, Manpower, Devolución
        /// - Para cada excepción gestiona 7 conceptos NMD: Alcance, Objetivo, Disciplina, Diversified, TipoCompra, TipoDisciplina, DisciplinaGrupo
        /// - Solo actualiza campos que cambiaron (optimización mediante HashSet CamposCambiados)
        /// 
        /// Si cualquier operación falla, hace rollback de toda la transacción
        /// </remarks>
        public async Task GrabarCondicionesExcepciones(
            Dictionary<CondicionDto, DatosCondicionCambiados> condicionesNoGuardadas,
            Dictionary<ExcepcionDto, DatosExcepcionesCondicionCambiados> excepcionesNoGuardadas,
            int codigoVigencia)
        {
            _logger.LogTrace("Llamando método GrabarCondicionesExcepciones");

            List<ConceptoCondicion> conceptos = await _condicionesRepository.ObtenerConceptos();

            using var transaction = _condicionesRepository.ObtenerTransaccion();

            try
            {
                #region Grabar Condiciones

                foreach (var (condicionDto, _) in condicionesNoGuardadas)
                {
                    var condicionBase = new Condicion
                    {
                        CodigoMedio = condicionDto.CodigoMedio,
                        Jerarquia = 0,
                        CodigoVigencia = codigoVigencia
                    };

                    var definiciones = new[]
                    {
                        (ConceptosCondiciones.Sag, condicionDto.PctSAG, ObtenerIndicador(conceptos, ConceptosCondiciones.Sag, 1)),
                        (ConceptosCondiciones.Manpower, condicionDto.PctManPower, ObtenerIndicador(conceptos, ConceptosCondiciones.Manpower, 1)),
                        (ConceptosCondiciones.Devolucion, condicionDto.PctDevolucion, condicionDto.IndicadorCalculoDevolucion)
                    };

                    foreach (var (concepto, porcentaje, indicador) in definiciones)
                    {
                        condicionBase.CodigoConcepto = concepto;
                        condicionBase.Porcentaje = porcentaje;
                        condicionBase.IndicadorCalculo = indicador;

                        await TratarCondicion(condicionBase);
                    }
                }

                #endregion

                #region Grabar Excepciones

                // Pre-tratamiento de jerarquías. Si se han modificado las jerarquías, tenemos que hacer una operación previa para que no dé problemas
                // a la hora de editarlas ya que forma parte de una UK en la tabla
                foreach (var (excepcionDto, datos) in excepcionesNoGuardadas)
                {
                    if (datos.CamposCambiados.Contains(nameof(ExcepcionDto.Jerarquia)))
                    {
                        // Si es > 0 es porque existe en BD, le ponemos la jerarquía en negativo para saber que no se va a duplicar
                        // y luego se modifica con la jerarquía correspondiente
                        foreach (var codigos in excepcionDto.CodigosConceptosCondiciones.Where(c => c.CodigoCondicion > 0))
                        {
                            var jerarquiaNegativa = excepcionDto.Jerarquia * -1;
                            await _condicionesRepository.ActualizarJerarquiaExcepcion(codigos.CodigoCondicion, jerarquiaNegativa);
                        }
                    }
                }

                // Tratamiento de excepciones
                foreach (var (excepcionDto, datos) in excepcionesNoGuardadas)
                {
                    var excepcionBase = new Condicion
                    {
                        CodigoMedio = excepcionDto.CodigoMedio,
                        Jerarquia = excepcionDto.Jerarquia,
                        CodigoVigencia = codigoVigencia
                    };

                    var definiciones = new[]
                    {
                        (ConceptosCondiciones.Sag, excepcionDto.PctSAG, ObtenerIndicador(conceptos, ConceptosCondiciones.Sag, 1)),
                        (ConceptosCondiciones.Manpower, excepcionDto.PctManPower, ObtenerIndicador(conceptos, ConceptosCondiciones.Manpower, 1)),
                        (ConceptosCondiciones.Devolucion, excepcionDto.PctDevolucion, excepcionDto.IndicadorCalculoDevolucion)
                    };

                    foreach (var (concepto, porcentaje, indicador) in definiciones)
                    {
                        excepcionBase.CodigoConcepto = concepto;
                        excepcionBase.Porcentaje = porcentaje;
                        excepcionBase.IndicadorCalculo = indicador;

                        var codigoCondicion = excepcionDto.CodigosConceptosCondiciones
                            .FirstOrDefault(c => c.CodigoConcepto == (int)concepto)?.CodigoCondicion ?? 0;

                        excepcionBase.CodigoCondicion = codigoCondicion;

                        await TratarExcepcion(excepcionBase, excepcionDto, datos.CamposCambiados);
                    }
                }

                #endregion

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static int ObtenerIndicador(IEnumerable<ConceptoCondicion> conceptos, ConceptosCondiciones concepto, int ValorPorDefecto)
        {
            return conceptos.FirstOrDefault(c => c.Codigo == (int)concepto)?.IndicadorCalculo ?? ValorPorDefecto;
        }

        /// <summary>
        /// Elimina una excepción de condición y ajusta las jerarquías de excepciones posteriores
        /// </summary>
        /// <param name="codigosConceptosCondiciones">Lista de códigos de conceptos a eliminar</param>
        /// <param name="jerarquia">Jerarquía de la excepción eliminada</param>
        /// <param name="codigoVigencia">Código de vigencia para buscar excepciones afectadas</param>
        /// <param name="codigoMedio">Código de medio para filtrar excepciones del mismo medio</param>
        /// <param name="codigoUsuario">Código de usuario que realiza la operación</param>
        /// <remarks>
        /// Este método realiza las siguientes operaciones en una transacción:
        /// 1. Elimina los 7 conceptos NMD de cada condición (Alcance, Objetivo, Disciplina, etc.)
        /// 2. Elimina las excepciones de condición
        /// 3. Busca excepciones posteriores del mismo medio (jerarquía mayor)
        /// 4. Decrementa en 1 la jerarquía de todas las excepciones posteriores
        /// 5. Actualiza las jerarquías en base de datos
        /// 
        /// Esto mantiene la integridad de la jerarquía secuencial de excepciones por medio
        /// </remarks>
        public async Task EliminarExcepcionCondicion(
            List<CodigosConceptoCondicion> codigosConceptosCondiciones,
            int jerarquia,
            int codigoVigencia,
            int codigoMedio,
            int codigoUsuario)
        {
            _logger.LogTrace("Llamando método EliminarExcepcionCondicion");

            using var transaction = _condicionesRepository.ObtenerTransaccion();

            try
            {
                // Eliminar conceptos y excepciones existentes
                foreach (var codigos in codigosConceptosCondiciones)
                {
                    foreach (var concepto in Enum.GetValues<ConceptosCondicionesNMD>())
                    {
                        await _condicionesRepository.EliminarConceptoNMDExcepcionCondicion(codigos.CodigoCondicion, concepto);
                    }

                    await _condicionesRepository.EliminarExcepcionCondicion(codigos.CodigoCondicion);
                }

                // Ajustar jerarquías de excepciones posteriores
                var excepciones = await _condicionesRepository.ObtenerExcepcionesCondiciones(codigoVigencia);

                // Filtrar por el medio y cogemos las que tengan la jerarquía mayor que la eliminada
                var excepcionesFiltradas = excepciones
                    .Where(c => c.CodigoMedio == codigoMedio && c.Jerarquia > jerarquia)
                    .ToList();

                // Decrementar jerarquía
                excepcionesFiltradas.ForEach(c => c.Jerarquia--);

                // Actualizar jerarquías en base de datos
                foreach (var excepcion in excepcionesFiltradas)
                {
                    var tareasActualizar = excepcion.CodigosConceptosCondiciones
                        .Select(codigo => _condicionesRepository.ActualizarJerarquiaExcepcion(codigo.CodigoCondicion, excepcion.Jerarquia));

                    await Task.WhenAll(tareasActualizar);
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task TratarConceptoNMD(int codigoCondicionMedio, ConceptosCondicionesNMD conceptoNMD, int? codigoConcepto)
        {
            if (codigoConcepto.HasValue && codigoConcepto.Value > 0)
            {
                await _condicionesRepository.GrabarConceptoNMD(codigoCondicionMedio, conceptoNMD, codigoConcepto.Value);
            }
            else
            {
                await _condicionesRepository.EliminarConceptoNMDExcepcionCondicion(codigoCondicionMedio, conceptoNMD);
            }
        }

        private async Task TratarExcepcion(Condicion excepcion, ExcepcionDto itemModificado, HashSet<string> camposConCambios)
        {
            // Buscar excepción existente
            var excepcionBD = await _condicionesRepository.ObtenerExcepcionOrCondicion(excepcion.CodigoCondicion);

            // Caso 1: No existe → insertar si porcentaje no es null 
            if (excepcionBD == null)
            {
                if (excepcion.Porcentaje == null) return;

                await _condicionesRepository.InsertarCondicion(excepcion);

                int codigoExcepcionMedio = excepcion.CodigoCondicion;

                // Insertar conceptos asociados
                await TratarConceptoNMD(codigoExcepcionMedio, ConceptosCondicionesNMD.Alcance, itemModificado.CodigoAlcance);
                await TratarConceptoNMD(codigoExcepcionMedio, ConceptosCondicionesNMD.Objetivo, itemModificado.CodigoObjetivo);
                await TratarConceptoNMD(codigoExcepcionMedio, ConceptosCondicionesNMD.Disciplina, itemModificado.CodigoDisciplina);
                await TratarConceptoNMD(codigoExcepcionMedio, ConceptosCondicionesNMD.Diversified, itemModificado.CodigoDiversified);
                await TratarConceptoNMD(codigoExcepcionMedio, ConceptosCondicionesNMD.TipoCompra, itemModificado.CodigoTipoCompra);
                await TratarConceptoNMD(codigoExcepcionMedio, ConceptosCondicionesNMD.TipoDisciplina, itemModificado.CodigoTipoDisciplina);
                await TratarConceptoNMD(codigoExcepcionMedio, ConceptosCondicionesNMD.DisciplinaGrupo, itemModificado.CodigoDisciplinaGrupo);

                return;
            }

            // Caso 2: Existe 
            int codigoCondicion = excepcionBD.CodigoCondicion;

            // Si cambió Porcentaje o Jerarquía
            if (excepcionBD.Porcentaje != excepcion.Porcentaje || excepcionBD.Jerarquia != excepcion.Jerarquia)
            {
                if (!excepcion.Porcentaje.HasValue)
                {
                    // Eliminar conceptos y excepción
                    foreach (var concepto in Enum.GetValues<ConceptosCondicionesNMD>())
                    {
                        await _condicionesRepository.EliminarConceptoNMDExcepcionCondicion(codigoCondicion, concepto);
                    }

                    await _condicionesRepository.EliminarExcepcionCondicion(codigoCondicion);
                    return;
                }

                excepcion.CodigoCondicion = codigoCondicion;
                await _condicionesRepository.ActualizarCondicion(excepcion);
            }

            // Actualizar solo los campos modificados 
            var tareas = new List<Task>();

            if (camposConCambios.Contains(nameof(ExcepcionDto.CodigoAlcance)))
                tareas.Add(TratarConceptoNMD(codigoCondicion, ConceptosCondicionesNMD.Alcance, itemModificado.CodigoAlcance));

            if (camposConCambios.Contains(nameof(ExcepcionDto.CodigoObjetivo)))
                tareas.Add(TratarConceptoNMD(codigoCondicion, ConceptosCondicionesNMD.Objetivo, itemModificado.CodigoObjetivo));

            if (camposConCambios.Contains(nameof(ExcepcionDto.CodigoDisciplina)))
                tareas.Add(TratarConceptoNMD(codigoCondicion, ConceptosCondicionesNMD.Disciplina, itemModificado.CodigoDisciplina));

            if (camposConCambios.Contains(nameof(ExcepcionDto.CodigoDiversified)))
                tareas.Add(TratarConceptoNMD(codigoCondicion, ConceptosCondicionesNMD.Diversified, itemModificado.CodigoDiversified));

            if (camposConCambios.Contains(nameof(ExcepcionDto.CodigoTipoCompra)))
                tareas.Add(TratarConceptoNMD(codigoCondicion, ConceptosCondicionesNMD.TipoCompra, itemModificado.CodigoTipoCompra));

            if (camposConCambios.Contains(nameof(ExcepcionDto.CodigoTipoDisciplina)))
                tareas.Add(TratarConceptoNMD(codigoCondicion, ConceptosCondicionesNMD.TipoDisciplina, itemModificado.CodigoTipoDisciplina));

            if (camposConCambios.Contains(nameof(ExcepcionDto.CodigoDisciplinaGrupo)))
                tareas.Add(TratarConceptoNMD(codigoCondicion, ConceptosCondicionesNMD.DisciplinaGrupo, itemModificado.CodigoDisciplinaGrupo));

            if (tareas.Count > 0)
                await Task.WhenAll(tareas);
        }

        private async Task TratarCondicion(Condicion condicion)
        {
            // Buscar condición  
            Condicion? condicionBD = await _condicionesRepository.ObtenerExcepcionOrCondicion(condicion);

            // Caso 1: No existe → grabar solo si tiene porcentaje
            if (condicionBD == null)
            {
                if (condicion.Porcentaje.HasValue)
                {
                    await _condicionesRepository.GrabarCondicion(condicion);
                }
                return;
            }

            // Caso 2.1: Si es Devolución, comprobar si ha cambiado el indicador
            if (condicion.CodigoConcepto == ConceptosCondiciones.Devolucion)
            {
                if (condicion.IndicadorCalculo == condicionBD.IndicadorCalculo)
                {
                    // Existe pero el porcentaje no ha cambiado → no se hace nada
                    if (condicionBD.Porcentaje == condicion.Porcentaje) return;
                }
            }
            // Caso 2.2: Si no es Devolución
            else
            {
                // Existe pero el porcentaje no ha cambiado → no se hace nada
                if (condicionBD.Porcentaje == condicion.Porcentaje) return;
            }

            // Caso 3: Existe y cambió el porcentaje o el tipo de devolución

            // 3.1 Si el nuevo porcentaje es null → eliminar condición (y sus excepciones)
            if (condicion.Porcentaje == null)
            {
                // Buscar códigos de excepciones del medio
                List<int> codigos = await _condicionesRepository.ObtenerCodigosExcepcionesCondiciones(condicion);

                foreach (var codigo in codigos)
                {
                    foreach (var concepto in Enum.GetValues<ConceptosCondicionesNMD>())
                    {
                        await _condicionesRepository.EliminarConceptoNMDExcepcionCondicion(codigo, concepto);
                    }
                    await _condicionesRepository.EliminarExcepcionCondicion(codigo);
                }
                await _condicionesRepository.EliminarCondicion(condicion);
            }
            // 3.2 Si el nuevo porcentaje no es null → grabar condición
            else
            {
                await _condicionesRepository.GrabarCondicion(condicion);
            }
        }

        /// <summary>
        /// Importa condiciones desde MMS (Media Management System) registrando inicio y fin en auditoría
        /// </summary>
        /// <param name="param">Parámetros de filtro para la importación desde MMS</param>
        /// <param name="nombreMetodoLlamador">Nombre del método llamador (se obtiene automáticamente con CallerMemberName)</param>
        /// <remarks>
        /// Flujo del proceso:
        /// 1. Registra en log de auditoría el inicio de la importación
        /// 2. Ejecuta la importación en una transacción
        /// 3. Si tiene éxito: registra en auditoría la finalización
        /// 4. Si falla: hace rollback y propaga la excepción
        /// 
        /// Los logs de auditoría quedan registrados incluso si falla la importación,
        /// lo que permite trazabilidad de intentos fallidos
        /// </remarks>
        public async Task ImportarCondicionesMMS(CondicionImportarFiltro param, [CallerMemberName] string nombreMetodoLlamador = "")
        {

            // Registrar auditoría antes de lanzar el proceso
            try
            {
                await _logAccionesService.Insertar(
                    AccionesLog.ImportarCondicionesMMS,
                    param);
            }
            catch (Exception logEx)
            {
                _logger.LogError(logEx, "Error registrando auditoría (lanzando proceso importacion de condiciones)");
                // No propagar
            }

            using var transaction = _condicionesRepository.ObtenerTransaccion();
            try
            {
                _logger.LogInformation("Llamando método ImportarCondicionesMMS");
                await _condicionesRepository.ImportarCondicionesMMS(param);
                await transaction.CommitAsync();

                // Registrar auditoría después de proceso realizado
                try
                {
                    await _logAccionesService.Insertar(
                        AccionesLog.ImportarCondicionesMMSFinalizado,
                        param);
                }
                catch (Exception logEx)
                {
                    _logger.LogError(logEx, "Error registrando auditoría (Proceso finalizado)");
                    // No propagar - El proceso de importación se ha realizado correctamente, solo ha fallado el registro en auditoría
                }
            }
            catch
            {
                _logger.LogInformation("Hay error y se llama al Rollback");
                await transaction.RollbackAsync();
                _logger.LogInformation("Ha habido error y se ha hecho Rollback");
                throw;
            }
            finally
            {
                _logger.LogInformation("Saliendo del método ImportarCondicionesMMS");
            }
        }
    }
}
