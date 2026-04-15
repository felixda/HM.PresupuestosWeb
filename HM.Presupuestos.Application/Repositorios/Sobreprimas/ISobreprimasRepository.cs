
using HM.Core.Servidor.v6.DAL.Interfaces;
using HM.Presupuestos.Contratos.Entidades;

namespace HM.Presupuestos.Application.Repositorios
{
    public interface ISobreprimasRepository : IBaseDAL
    {
        Task<List<Sobreprima>> ObtenerSobreprimas(SobreprimaFiltro filterSobreprima);

        Task InsertSobreprima(Sobreprima item);

        Task EliminarSobreprima(int codigoSobreprima);

        Task ActualizarSobreprima(Sobreprima item);


        Task<bool> ExistenSobreprimas(SobreprimaFiltro filterSobreprima, string? codigosSobreprima = null);

    }
}
