using HM.Presupuestos.Contratos.Comun;
using HM.Presupuestos.Contratos.Entidades;


namespace HM.Presupuestos.Application.Repositorios
{
    public interface ILogAccionesRepository
    {
        Task Insertar(LogAccion logAccion);
    }
}


