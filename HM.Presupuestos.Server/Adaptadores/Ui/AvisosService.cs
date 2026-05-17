

using HM.Presupuestos.Domain.Compartido;

namespace HM.Presupuestos.Server.Adaptadores.Ui
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



