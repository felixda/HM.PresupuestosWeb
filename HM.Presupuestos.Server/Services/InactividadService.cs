
namespace HM.Presupuestos.Server.Services
{
    /// <summary>
    /// Servicio para comunicarse con el JsInteropHelper (que recibe los eventos de inactividad del inactividad.js) y con el componente MainLayout
    /// para enviarle los eventos correspondientes
    /// </summary>

    public class ControlInactividad
    {
        /// <summary>
        /// Evento para mostrar la advertencia
        /// </summary>
        public event EventHandler? OnInactividadIniciada;

        /// <summary>
        /// Evento para actualizar la cuenta atras
        /// </summary>
        public event EventHandler<int>? OnCuentaRegresiva;

        /// <summary>
        /// Evento para ocultar la advertencia y redirigir a la home
        /// </summary>
        public event EventHandler? OnInactividadFinalizada;

        /// <summary>
        /// Evento para indicar que el usuario ha tenido actividad y ocultar la advertencia en caso de que estuviera mostrando
        /// </summary>
        public event EventHandler? OnAdvertenciaCancelada;

        public void NotificarInactividadIniciada() => OnInactividadIniciada?.Invoke(this, EventArgs.Empty);
        public void NotificarCuentaRegresiva(int tiempoMs) => OnCuentaRegresiva?.Invoke(this, tiempoMs);
        public void NotificarInactividadFinalizada() => OnInactividadFinalizada?.Invoke(this, EventArgs.Empty);
        public void CancelarAdvertencia() => OnAdvertenciaCancelada?.Invoke(this, EventArgs.Empty);
    }
}

