using HM.Core.Comun.v6.Entidades.Logger;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Puertos;

namespace HM.Presupuestos.Infrastructure.Servicios
{
    public class RegistroErroresCore : IRegistroErroresCore
    {
        public async Task RegistrarErrorSaveLog(string jwtToken, DetalleError error)
        {
            var registroError = new DatosPeticionLogData
            {
                UserName = error.UserName,
                Fecha = error.Fecha,
                Mensaje = error.Mensaje,
                StackTrace = error.StackTrace
            };


            //CodigoAplicacion = 0,
            //    CodigoPais = 0,
            //    CodigoCompania = 0,
            //    UserName = error.UserName,
            //    Fecha = error.Fecha,
            //    Categoria = string.Empty,
            //    Mensaje = error.Mensaje,
            //    StackTrace = error.StackTrace,
            //    Observaciones = string.Empty,
            //    DominioAplicacion = string.Empty

            await ClienteApiCore.RegistrarLog(jwtToken, registroError);
        }
    }
}
