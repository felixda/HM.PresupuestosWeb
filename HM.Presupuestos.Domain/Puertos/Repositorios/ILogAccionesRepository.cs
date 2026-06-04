ï»¿using HM.Presupuestos.Domain.Entidades;


namespace HM.Presupuestos.Domain.Puertos
{
    public interface ILogAccionesRepository
    {
        Task Insertar(LogAccion logAccion);
    }
}


