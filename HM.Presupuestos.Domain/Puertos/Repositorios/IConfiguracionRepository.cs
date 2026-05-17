using HM.Presupuestos.Domain.Entidades;

namespace HM.Presupuestos.Domain.Puertos
{
    public interface IConfiguracionRepository
    {
        Task<CodigoDescripcion> ObtenerAnioDiario();

        Task ActualizarAnioDiario(int anio);

    }
}
