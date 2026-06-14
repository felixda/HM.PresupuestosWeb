
namespace HM.Presupuestos.Web.Pages.Shared
{
    public partial class ModalControlCambios
    {
        #region Inyección de Dependencias

        [Inject] protected ILocalizadorRecursos LocalizadorRecursos { get; set; } = default!;

        #endregion

        #region Propiedades Privadas

        private TaskCompletionSource<bool>? _tcs;
        private bool Mostrar = false;
        private string Mensaje = "";
        private string Si = "Sí";

        #endregion

        #region Métodos Públicos

        public async Task<bool> Show(string mensaje)
        {
            Si = LocalizadorRecursos.ObtenerTexto(TextosApp.Common.Si);
            Mensaje = mensaje;
            Mostrar = true;
            StateHasChanged();
            _tcs = new();
            await Task.Yield(); // ?? IMPORTANTE: deja que Blazor pinte el modal

            return await _tcs.Task;
        }

        #endregion

        #region Métodos Privados

        private void Confirmar()
        {
            Mostrar = false;
            StateHasChanged();
            _tcs?.SetResult(true);
            //   StateHasChanged();
        }

        private void Cancelar()
        {
            Mostrar = false;
            StateHasChanged();
            _tcs?.SetResult(false);
            //   StateHasChanged();
        }

        #endregion
    }
}

