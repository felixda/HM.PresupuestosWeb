
namespace HM.Presupuestos.Server.Services
{
    /// <summary>
    /// Clase para controlar al navegar a otra página si hay cambios en los datos de las paginas, 
    /// que muestra pantalla modal para avisar si los hubiera, pudiendo cancelar la navegacion
    /// </summary>
    public class ControlCambiosService
    {
        private readonly IJSRuntime _js;

        public bool HayCambios() => _hayCambios;

        private bool _hayCambios = false;

        public ControlCambiosService(IJSRuntime js)
        {
            _js = js;
        }

        private Func<string, Task<bool>>? _mostrarModal;


        /// <summary>
        /// Metodo para activar la generación de un evento a la hora de cambiar de página mediante navegacion
        /// </summary>
        /// <param name="cambios"></param>
        public async Task MarcarCambios(bool cambios)
        {
            _hayCambios = cambios;
            //Esto esta en controlCambios.js
            await _js.InvokeVoidAsync("setConfirmOnUnload", cambios);
        }


        public void LimpiarCambios()
        {
            _hayCambios = false;
            _js.InvokeVoidAsync("setConfirmOnUnload", false);
        }

        public void RegistrarConfirmador(Func<string, Task<bool>> confirmador)
        {
            _mostrarModal = confirmador;
        }

        /// <summary>
        /// Funcion que se ejecuta al detectar eventos en la navegacion de las paginas
        /// </summary>
        /// <param name="destino"></param>
        /// <returns></returns>
        public async Task<bool> ConfirmarNavegacionAsync(string destino)
        {
            if (!_hayCambios || _mostrarModal == null)
                return true;

            return await _mostrarModal.Invoke(destino);
        }
    }


}