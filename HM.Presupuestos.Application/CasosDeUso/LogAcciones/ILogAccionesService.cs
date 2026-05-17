using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Domain.Entidades;
using System.Runtime.CompilerServices;

namespace HM.Presupuestos.Application.CasosDeUso.LogAcciones
{
    public interface ILogAccionesService
    {
        Task Insertar(string accion, object? parametros = null, [CallerMemberName] string nombreMetodoLlamador = "");
        Task Insertar(AccionesLog accion, object? parametros = null, [CallerMemberName] string nombreMetodoLlamador = "");
        Task Insertar(LogAccion logAccion);
    }
}
