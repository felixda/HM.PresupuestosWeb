using HM.Presupuestos.Contratos.Comun;
using HM.Presupuestos.Contratos.Entidades;

namespace HM.Presupuestos.Application.Repositorios
{
    public interface IConfiguracionRepository
    {
        Task<CodigoDescripcion> ObtenerAnioDiario();

        Task ActualizarAnioDiario(int anio);

    }
}
