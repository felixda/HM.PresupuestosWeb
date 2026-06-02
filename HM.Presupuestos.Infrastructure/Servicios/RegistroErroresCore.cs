using HM.Core.Comun.v6.Entidades.Logger;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Puertos;

namespace HM.Presupuestos.Infrastructure.Servicios
{
    public class RegistroErroresCore(IClienteApiCore clienteApiCore) : IRegistroErroresCore
    {
        private readonly IClienteApiCore _clienteApiCore = clienteApiCore;

        public async Task RegistrarErrorSaveLog(string jwtToken, DetalleError error)
        {
            var registroError = new DatosPeticionLogData
            {
                UserName = error.UserName,
                Fecha = error.Fecha,
                Mensaje = error.Mensaje,
                StackTrace = error.StackTrace
            };

            await _clienteApiCore.RegistrarLog(jwtToken, registroError);
        }
    }
}
