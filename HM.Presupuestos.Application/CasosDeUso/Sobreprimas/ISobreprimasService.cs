using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Domain.Entidades;

namespace HM.Presupuestos.Application.CasosDeUso
{
    /// <summary>
    /// Interfaz del servicio de gestión de sobreprimas comerciales
    /// </summary>
    public interface ISobreprimasService
    {
        Task<List<Sobreprima>> ObtenerSobreprimas(SobreprimaFiltro filterSobreprima);

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
        Task<bool> ExistenSobreprimas(SobreprimaFiltro filterSobreprima, SobreprimaGridModel? sobreprima = null);

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
        Task EliminarSobreprimas(SobreprimaGridModel sobreprima);

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
        Task GrabarSobreprimas(List<Sobreprima> items, string nombreMetodoLlamador = "");
    }
}

