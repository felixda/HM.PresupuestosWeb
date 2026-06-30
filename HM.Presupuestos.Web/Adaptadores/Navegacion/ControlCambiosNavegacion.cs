namespace HM.Presupuestos.Web.Adaptadores.Navegacion
{
    /// <summary>
    /// Interfaz para controlar al navegar a otra página si hay cambios en los datos de las paginas, 
    /// que muestra pantalla modal para avisar si los hubiera, pudiendo cancelar la navegacion
    /// </summary>
    public interface IControlCambiosNavegacion
    {
        bool TieneCambiosPendientes();
        Task ActualizarEstadoCambios(bool cambios);
        void LimpiarCambiosPendientes();
        void RegistrarConfirmacionSalida(Func<string, Task<bool>> confirmador);
        Task<bool> PuedeAbandonarPagina(string destino);
    }

    public class ControlCambiosNavegacion : IControlCambiosNavegacion
    {
        private readonly IJSRuntime _js;

        public bool TieneCambiosPendientes() => _tieneCambiosPendientes;

        private bool _tieneCambiosPendientes = false;

        public ControlCambiosNavegacion(IJSRuntime js)
        {
            _js = js;
        }

        private Func<string, Task<bool>>? _confirmarSalida;


        /// <summary>
        /// Metodo para activar la generación de un evento a la hora de cambiar de página mediante navegacion
        /// </summary>
        /// <param name="cambios"></param>
        public async Task ActualizarEstadoCambios(bool cambios)
        {
            _tieneCambiosPendientes = cambios;
            //Esto esta en controlCambios.js
            await _js.InvokeVoidAsync("setConfirmOnUnload", cambios);
        }


        public void LimpiarCambiosPendientes()
        {
            _tieneCambiosPendientes = false;
            _js.InvokeVoidAsync("setConfirmOnUnload", false);
        }

        public void RegistrarConfirmacionSalida(Func<string, Task<bool>> confirmador)
        {
            _confirmarSalida = confirmador;
        }

        /// <summary>
        /// Funcion que se ejecuta al detectar eventos en la navegacion de las paginas
        /// </summary>
        /// <param name="destino"></param>
        /// <returns></returns>
        public async Task<bool> PuedeAbandonarPagina(string destino)
        {
            if (!_tieneCambiosPendientes || _confirmarSalida == null)
                return true;

            return await _confirmarSalida.Invoke(destino);
        }
    }


}


