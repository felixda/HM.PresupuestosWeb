using HM.Presupuestos.Domain.Entidades;


namespace HM.Presupuestos.Application.Repositorios
{
    public interface ILogAccionesRepository
    {
        Task Insertar(LogAccion logAccion);
    }
}


