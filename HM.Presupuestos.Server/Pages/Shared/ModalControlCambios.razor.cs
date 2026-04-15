namespace HM.Presupuestos.Server.Pages.Shared
{
    public partial class ModalControlCambios
    {
        private TaskCompletionSource<bool>? _tcs;
        private bool Mostrar = false;
        private string Mensaje = "";
        private string Si = "Sí";

        public async Task<bool> Show(string mensaje)
        {
            Si = await _TraduccionesHelper.GetResourceValue("Common:Si:label");
            Mensaje = mensaje;
            Mostrar = true;
            StateHasChanged();
            _tcs = new();
            await Task.Yield(); // 👈 IMPORTANTE: deja que Blazor pinte el modal

            return await _tcs.Task;
        }

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
    }
}
