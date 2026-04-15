using HM.Core.Servidor.v6.DAL.Interfaces;
using HM.Presupuestos.Contratos.Entidades;

namespace HM.Presupuestos.Application.Repositorios
{
    public interface IAdminRepository : IBaseDAL
    {
               
        Task<List<int>> ObtenerMesesBloqueados(int anio);
      
        Task InsertarMesBloqueado(int anio, int mes);

      
        Task EliminarMesesBloqueados(int anio);

      

    }


}
