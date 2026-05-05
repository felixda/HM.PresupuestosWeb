using HM.Presupuestos.Domain.Entidades;

namespace HM.Presupuestos.Domain.Puertos
{
    public interface ICoreLoggerService
    {
        Task SaveLog(string jwtToken, ErrorLogData data);
    }
}
