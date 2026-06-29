using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Entidades.LogAcciones;

namespace HM.Presupuestos.Domain.Puertos
{
    public interface ILogsTecnicosRepository
    {
        Task<List<LogTecnico>> ObtenerLogs(FiltroLogsTecnicos filtro);
        Task<List<CodigoDescripcion>> ObtenerNivelesDisponibles();
    }
}