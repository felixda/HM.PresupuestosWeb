using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Entidades.LogAcciones;


namespace HM.Presupuestos.Domain.Puertos
{
    public interface ILogAccionesRepository
    {
        Task Insertar(LogAccion logAccion);
        Task<List<Auditoria>> ObtenerAuditorias(AccionesLog tipo, DateTime? fechaInicio, DateTime? fechaFin, int? codigoPagina = null);
    }
}


