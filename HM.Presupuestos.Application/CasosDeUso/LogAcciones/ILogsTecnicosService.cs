using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Entidades.LogAcciones;

namespace HM.Presupuestos.Application.CasosDeUso.LogAcciones
{
    public interface ILogsTecnicosService
    {
        Task<List<LogTecnico>> ObtenerLogs(FiltroLogsTecnicos filtro);
        Task<List<CodigoDescripcion>> ObtenerNivelesDisponibles();
        Task<List<CodigoDescripcion>> ObtenerUsuariosDisponibles();
    }
}