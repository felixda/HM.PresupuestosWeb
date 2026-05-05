using HM.Core.Comun.v6.Entidades.Logger;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Puertos;

namespace HM.Presupuestos.Infraestructure
{
    public class CoreLoggerService : ICoreLoggerService
    {
        public async Task SaveLog(string jwtToken, ErrorLogData data)
        {
            var datosPeticion = new DatosPeticionLogData
            {
                CodigoAplicacion = 0,
                CodigoPais = 0,
                CodigoCompania = 0,
                UserName = data.UserName,
                Fecha = data.Fecha,
                Categoria = string.Empty,
                Mensaje = data.Mensaje,
                StackTrace = data.StackTrace,
                Observaciones = string.Empty,
                DominioAplicacion = string.Empty
            };

            await ApiCoreCli.SaveLog(jwtToken, datosPeticion);
        }
    }
}
