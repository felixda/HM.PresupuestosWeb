

namespace HM.Presupuestos.Domain.Puertos
{
    public interface IAdminRepository 
    {
               
        Task<List<int>> ObtenerMesesBloqueados(int anio);
      
        Task InsertarMesBloqueado(int anio, int mes);

      
        Task EliminarMesesBloqueados(int anio);

        ITransaccion ObtenerTransaccion();

    }


}
