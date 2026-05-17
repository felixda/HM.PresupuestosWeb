namespace HM.Presupuestos.Web.Adaptadores.Ui
{
    public class DialogoErrores
    {
        public event Func<string, Exception, Task>? OnError;

        public async Task MostrarErrorInicializandoPagina(string mensaje, Exception ex)
        {
            if (OnError != null)
                await OnError.Invoke(mensaje, ex);
        }
    }
}



