using HM.Presupuestos.Application.CasosDeUso.LogAcciones;
using HM.Core.Comun.v6.Loggers.Interfaces;
using HM.Presupuestos.Domain.Puertos;
using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Domain.Entidades;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace HM.Presupuestos.Application.CasosDeUso
{
    /// <summary>
    /// Servicio de gestión de sobreprimas comerciales
    /// Gestiona los tres conceptos de sobreprimas: Default, SLA y HVP
    /// </summary>
    public class SobreprimasService(
        ILogger<SobreprimasService> logger,
        ISobreprimasRepository sobreprimasRepository,
        ILogAccionesService logAccionesService) : ISobreprimasService
    {
        private readonly ILogger<SobreprimasService> _logger = logger;
        private readonly ISobreprimasRepository _sobreprimasRepository = sobreprimasRepository;
        private readonly ILogAccionesService _logAccionesService = logAccionesService;

        /// <summary>
        /// Obtiene lista de sobreprimas filtradas
        /// </summary>
        /// <param name="filterSobreprima">Filtro de búsqueda (país, versión, etc.)</param>
        /// <returns>Lista de objetos Sobreprima</returns>
        public async Task<List<Sobreprima>> ObtenerSobreprimas(SobreprimaFiltro filterSobreprima)
        {
            _logger.LogTrace("Llamando método GetSobreprimaList");

            var resultado = await _sobreprimasRepository.ObtenerSobreprimas(filterSobreprima);

            return resultado;
        }

        /// <summary>
        /// Verifica si existen sobreprimas que coincidan con el filtro, excluyendo opcionalmente códigos específicos
        /// </summary>
        /// <param name="filterSobreprima">Filtro para buscar sobreprimas (país, versión, etc.)</param>
        /// <param name="sobreprima">Sobreprima opcional con conceptos (Default, HVP, SLA) a excluir de la búsqueda</param>
        /// <returns>True si existen sobreprimas que coincidan (excluyendo los códigos especificados), false en caso contrario</returns>
        /// <remarks>
        /// Lógica del método:
        /// 1. Si NO se proporciona sobreprima ? busca sin exclusiones
        /// 2. Si se proporciona sobreprima ? construye lista de códigos a excluir de los 3 conceptos (Default, HVP, SLA)
        /// 3. Solo incluye en la exclusión los conceptos con código > 0
        /// Útil para validar duplicados excluyendo el registro actual en ediciones
        /// </remarks>
        public async Task<bool> ExistenSobreprimas(SobreprimaFiltro filterSobreprima, SobreprimaGridModel? sobreprima = null)
        {
            _logger.LogTrace("Llamando método ExistenSobreprimas");

            // Caso 1: Sin sobreprima específica ? buscar sin exclusiones
            if (sobreprima == null)
            {
                return await _sobreprimasRepository.ExistenSobreprimas(filterSobreprima);
            }

            // Caso 2: Con sobreprima ? construir códigos a excluir usando LINQ
            var codigosExcluir = new[]
            {
                sobreprima.ConceptoDefaul.Codigo,
                sobreprima.ConceptoHVP.Codigo,
                sobreprima.ConceptoSLA.Codigo
            }
            .Where(codigo => codigo > 0)           // Solo códigos válidos
            .Select(codigo => codigo.ToString());  // Convertir a string

            // Si no hay códigos válidos, no hay nada que excluir
            if (!codigosExcluir.Any())
            {
                return false;
            }

            // Buscar con exclusión de códigos
            string codigosCSV = string.Join(",", codigosExcluir);
            return await _sobreprimasRepository.ExistenSobreprimas(filterSobreprima, codigosCSV);
        }

        /// <summary>
        /// Elimina los tres conceptos de sobreprimas (Default, SLA, HVP) en una única transacción
        /// </summary>
        /// <param name="sobreprima">Modelo de grid con los códigos de los tres conceptos a eliminar</param>
        /// <remarks>
        /// Este método elimina en una transacción:
        /// - ConceptoDefault (si código != 0)
        /// - ConceptoSLA (si código != 0)
        /// - ConceptoHVP (si código != 0)
        /// Solo elimina los conceptos que tengan código asignado (existen en BD)
        /// Si cualquier eliminación falla, hace rollback de toda la operación
        /// </remarks>
        public async Task EliminarSobreprimas(SobreprimaGridModel sobreprima)
        {
            _logger.LogTrace("Llamando método EliminarSobreprimas");

            using var transaction = _sobreprimasRepository.ObtenerTransaccion();
            try
            {
                // Obtener códigos de los 3 conceptos a eliminar
                var codigosAEliminar = new[]
                {
                    sobreprima.ConceptoDefaul.Codigo,
                    sobreprima.ConceptoSLA.Codigo,
                    sobreprima.ConceptoHVP.Codigo
                }
                .Where(codigo => codigo != 0);  // Solo códigos válidos (existen en BD)

                // Eliminar cada concepto 
                foreach (var codigo in codigosAEliminar)
                {
                    await _sobreprimasRepository.EliminarSobreprima(codigo);
                }

                await transaction.CommitAsync();

                // Registrar auditoría fuera de la transacción
                try
                {
                    await _logAccionesService.Insertar(
                        AccionesLog.EliminarSobreprima,
                        sobreprima);
                }
                catch (Exception logEx)
                {
                    _logger.LogError(logEx, "Error registrando auditoría (eliminación exitosa)");
                    // No propagar - la eliminación fue exitosa
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Guarda una lista de sobreprimas aplicando lógica condicional según código y porcentaje
        /// </summary>
        /// <param name="items">Lista de sobreprimas a procesar</param>
        /// <param name="nombreMetodoLlamador">Nombre del método llamador (se obtiene automáticamente con CallerMemberName)</param>
        /// <remarks>
        /// Este método aplica la siguiente lógica para cada sobreprima en una transacción:
        /// 
        /// 1. Si Codigo == 0 Y Porcentaje > 0 ? INSERTAR (nueva sobreprima)
        /// 2. Si Codigo > 0 Y Porcentaje == 0 ? ELIMINAR (limpiar sobreprima)
        /// 3. Si Codigo > 0 Y Porcentaje > 0 ? ACTUALIZAR (modificar existente)
        /// 4. Si Codigo == 0 Y Porcentaje == 0 ? IGNORAR (no hacer nada)
        /// 
        /// Se registra en auditoría el inicio de la operación antes de la transacción.
        /// Si cualquier operación falla, hace rollback de todos los cambios.
        /// </remarks>
        public async Task GrabarSobreprimas(List<Sobreprima> items, [CallerMemberName] string nombreMetodoLlamador = "")
        {
            _logger.LogInformation("Llamando método GrabarSobreprimas");

            // Registrar auditoría antes de la transacción
            

            using var transaction = _sobreprimasRepository.ObtenerTransaccion();
            try
            {

                foreach (var item in items)
                {
                    // Caso 1: Nueva sobreprima (sin código y con porcentaje)
                    if (item.Codigo == 0 && item.Porcentaje > 0)
                    {
                        await _sobreprimasRepository.InsertSobreprima(item);
                    }
                    // Caso 2: Sobreprima existente
                    else if (item.Codigo > 0)
                    {
                        // Caso 2.1: Eliminar si porcentaje = 0
                        if (item.Porcentaje == 0)
                        {
                            await _sobreprimasRepository.EliminarSobreprima(item.Codigo);
                        }
                        // Caso 2.2: Actualizar si tiene porcentaje
                        else
                        {
                            await _sobreprimasRepository.ActualizarSobreprima(item);
                        }
                    }
                    // Caso 3: Ignorar (código = 0 y porcentaje = 0)
                }

                await transaction.CommitAsync();

                // Registrar auditoría fuera de la transacción
                try
                {
                    await _logAccionesService.Insertar(
                       AccionesLog.ActualizarSobreprimas,
                       items);
                }
                catch (Exception logEx)
                {
                    _logger.LogError(logEx, "Error registrando auditoría (Grabación exitosa)");
                    // No propagar - la grabación fue exitosa
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}


