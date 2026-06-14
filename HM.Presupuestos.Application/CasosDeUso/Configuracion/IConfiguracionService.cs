using HM.Presupuestos.Domain.Entidades;

namespace HM.Presupuestos.Application.CasosDeUso
{
    /// <summary>
    /// Interfaz del servicio de gestión de configuración de la aplicación
    /// </summary>
    public interface IConfiguracionService
    {
        Task<CodigoDescripcion> ObtenerAnioDiario();

        Task ActualizarAnioDiario(int anio);
    }
}
