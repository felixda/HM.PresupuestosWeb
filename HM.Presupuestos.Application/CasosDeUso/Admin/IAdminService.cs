namespace HM.Presupuestos.Application.CasosDeUso
{
    /// <summary>
    /// Interfaz del servicio de administración
    /// </summary>
    public interface IAdminService
    {
        /// <summary>
        /// Obtiene la lista de meses bloqueados para un año concreto
        /// </summary>
        Task<List<int>> ObtenerMesesBloqueados(int anio);

        /// <summary>
        /// Reemplaza los meses bloqueados de un año por la lista proporcionada
        /// </summary>
        Task InsertarMesesBloqueado(int anio, List<int> meses);
    }
}
