
namespace HM.Presupuestos.Web.Componentes.Helpers
{

    /// <summary>
    /// Clase para comunicar las operaciones de js para controlar la inactividad del usuario con el servicio 
    /// que geneara los eventos que se mandan al MainLayout para mostrar el aviso, la cuenta atras y redirigir a la home
    /// </summary>
    public class JsInteropHelper : IAsyncDisposable
    {
        private readonly IJSRuntime js;
        private readonly DotNetObjectReference<JsInteropHelper> objRef;
        private readonly ControlInactividad service;

        public JsInteropHelper(IJSRuntime js, ControlInactividad service)
        {
            this.js = js;
            this.service = service;
            objRef = DotNetObjectReference.Create(this);
        }

        /// <summary>
        /// Iniciar el control de evntos del usuario suscribiendose a los eventos necesarios, asi como inicializacion de los 
        /// contadores
        /// </summary>
        /// <param name="tiempoInactividadMs">Tiempo que el usuario permacene inactivo hasta que se redirige a la home</param>
        /// <param name="tiempoAdvertenciaMs">Tiempo antes de redirigir en el que mostrara la advertencia</param>
        /// <returns></returns>
        public async Task Iniciar(int tiempoInactividadMs, int tiempoAdvertenciaMs)
        {
            // Prefer to call a safe wrapper via invokeIfExists to avoid throwing if the wrapper isn't registered yet.
            try
            {
                var invoked = await js.InvokeAsync<bool>("invokeIfExists", "startInactividadWhenReady", objRef, tiempoInactividadMs, tiempoAdvertenciaMs);
                if (!invoked)
                {
                    // fallback to original
                    await js.InvokeVoidAsync("inactividad.iniciar", objRef, tiempoInactividadMs, tiempoAdvertenciaMs);
                }
            }
            catch (JSException ex)
            {
                Console.WriteLine($"[JsInteropHelper] invokeIfExists/startInactividadWhenReady failed: {ex.Message}. Falling back to inactivity.iniciar.");
                await js.InvokeVoidAsync("inactividad.iniciar", objRef, tiempoInactividadMs, tiempoAdvertenciaMs);
            }
        }

        /// <summary>
        /// Finaliza el control de la inactividad y se desuscribe a los eventos que lo controlan
        /// </summary>
        /// <returns></returns>
        public async Task Finalizar()
        {
            try
            {
                var invoked = await js.InvokeAsync<bool>("invokeIfExists", "finalizarInactividadWhenReady", objRef);
                if (!invoked)
                {
                    await js.InvokeVoidAsync("inactividad.finalizar", objRef);
                }
            }
            catch (JSException ex)
            {
                Console.WriteLine($"[JsInteropHelper] invokeIfExists/finalizarInactividadWhenReady failed: {ex.Message}. Falling back to inactivity.finalizar.");
                await js.InvokeVoidAsync("inactividad.finalizar", objRef);
            }
        }

        /// <summary>
        /// Metodo llamado desde Js para avisar al servicio de que tiene que generar el evento para mostrar la advertencia
        /// </summary>
        [JSInvokable]
        public void InactividadIniciada() => service.NotificarInactividadIniciada();

        /// <summary>
        /// Metodo llamado desde Js para avisar al servicio de que tiene que genear el evento para actualizar la cuenta atras
        /// </summary>
        /// <param name="tiempoMs">Indica el numero de segundos que hay que mostrar en la cuenta atras</param>
        [JSInvokable]
        public void CuentaRegresiva(int tiempoMs) => service.NotificarCuentaRegresiva(tiempoMs);

        /// <summary>
        /// Metodo llamado desde Js para avisar al servicio de que tiene que generar el evento para ocultar la advertencia
        /// y redirigir a la home
        /// </summary>
        [JSInvokable]
        public void InactividadFinalizada() => service.NotificarInactividadFinalizada();

        /// <summary>
        /// Metodo llamado desde Js para avisar al servicio de que tiene que generar el evento para indicar que el usuario
        /// ha tenido actividad y ocultar la advertencia en caso de que estuviera mostrando
        /// </summary>
        [JSInvokable]
        public void CancelarAdvertencia() => service.CancelarAdvertencia();

        public ValueTask DisposeAsync()
        {
            objRef.Dispose();
            return ValueTask.CompletedTask;
        }
    }

}




