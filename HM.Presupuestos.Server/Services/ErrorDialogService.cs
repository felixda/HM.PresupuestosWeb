namespace HM.Presupuestos.Server.Services
{
    public class ErrorDialogService
    {
        public event Func<string, Exception, Task>? OnError;

        public async Task MostrarErrorInicializandoPagina(string mensaje, Exception ex)
        {
            if (OnError != null)
                await OnError.Invoke(mensaje, ex);
        }
    }
}
