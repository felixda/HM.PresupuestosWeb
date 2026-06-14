using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Entidades.LogAcciones;
using System.Runtime.CompilerServices;

namespace HM.Presupuestos.Application.CasosDeUso.LogAcciones
{
    public interface ILogAccionesService
    {
        Task Insertar(string accion, object? parametros = null, [CallerMemberName] string nombreMetodoLlamador = "");
        Task Insertar(AccionesLog accion, object? parametros = null, [CallerMemberName] string nombreMetodoLlamador = "");
        Task Insertar(LogAccion logAccion);
        Task<List<Auditoria>> ObtenerAuditorias(AccionesLog tipo, DateTime? fechaInicio, DateTime? fechaFin, int? codigoPagina = null);
        Task<EstadisticasAuditoria> ObtenerEstadisticas(AccionesLog tipo, DateTime fechaInicio, DateTime fechaFin, int? codigoPagina = null);
    }
}
