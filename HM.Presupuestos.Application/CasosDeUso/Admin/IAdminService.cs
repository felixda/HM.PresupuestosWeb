namespace HM.Presupuestos.Application.CasosDeUso
{
    /// <summary>
    /// Interfaz del servicio de administraciÃ³n
    /// </summary>
    public interface IAdminService
    {
        /// <summary>
        /// Obtiene la lista de meses bloqueados para un aÃ±o concreto
        /// </summary>
        Task<List<int>> ObtenerMesesBloqueados(int anio);

        /// <summary>
        /// Reemplaza los meses bloqueados de un aÃ±o por la lista proporcionada
        /// </summary>
        Task InsertarMesesBloqueado(int anio, List<int> meses);
    }
}
