
using HM.Presupuestos.Domain.Entidades;

namespace HM.Presupuestos.Domain.Puertos
{
    public interface ISobreprimasRepository 
    {
        Task<List<Sobreprima>> ObtenerSobreprimas(SobreprimaFiltro filterSobreprima);

        Task InsertSobreprima(Sobreprima item);

        Task EliminarSobreprima(int codigoSobreprima);

        Task ActualizarSobreprima(Sobreprima item);


        Task<bool> ExistenSobreprimas(SobreprimaFiltro filterSobreprima, string? codigosSobreprima = null);

    }
}
