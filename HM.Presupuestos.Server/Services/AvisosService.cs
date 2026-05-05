

using HM.Presupuestos.Domain.Comun;

namespace HM.Presupuestos.Server.Services
{

    public class AvisosService : IAvisosService
    {

        public event Func<string, TiposDeAviso,Task>? OnAvisoActivado;

        public async Task ActivarAvisosAsync(string mensaje, TiposDeAviso tipo)
        {
            if (OnAvisoActivado is not null)
            {
                await OnAvisoActivado.Invoke(mensaje, tipo);
            }
        }
    }

    public interface IAvisosService
    {
        event Func<string,TiposDeAviso, Task>? OnAvisoActivado;

        Task ActivarAvisosAsync(string mensaje, TiposDeAviso tipo);
    }
}
