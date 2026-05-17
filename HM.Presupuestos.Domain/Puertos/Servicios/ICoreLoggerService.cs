using HM.Presupuestos.Domain.Entidades;

namespace HM.Presupuestos.Domain.Puertos
{
    public interface IRegistroErroresCore
    {
        Task RegistrarErrorSaveLog(string jwtToken, DetalleError error);
        Task RegistrarErrorSaveLog(DetalleError error) => RegistrarErrorSaveLog(string.Empty, error);
    }
}
