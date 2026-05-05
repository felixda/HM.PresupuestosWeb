using HM.Presupuestos.Domain.Entidades;

namespace HM.Presupuestos.Application.Servicios
{
    /// <summary>
    /// Interfaz del servicio de gestión de indicadores de versiones de presupuestos
    /// </summary>
    public interface IIndicadoresService
    {
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
        Task<List<Indicador>> ObtenerIndicadoresConIdiomas(string? descripcion = null);

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
        Task Grabar(Indicador indicador,
            List<IdiomaIndicador> idiomasNuevos,
            List<IdiomaIndicador> idiomasActualizar,
            List<IdiomaIndicador> idiomasEliminar);

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
        Task Eliminar(Indicador indicador);

        Task<int> ObtenerUltimoBitAnd();
        
        Task<int> ObtenerUltimoOrden();
    }
}