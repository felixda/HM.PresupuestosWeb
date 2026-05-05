using HM.Presupuestos.Domain.Comun;
using HM.Presupuestos.Domain.Entidades;

namespace HM.Presupuestos.Application.Repositorios
{
    public interface IConfiguracionRepository
    {
        Task<CodigoDescripcion> ObtenerAnioDiario();

        Task ActualizarAnioDiario(int anio);

    }
}
