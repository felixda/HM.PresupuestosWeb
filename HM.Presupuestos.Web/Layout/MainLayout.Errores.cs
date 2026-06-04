namespace HM.Presupuestos.Web.Layout
{
    public partial class MainLayout
    {
        #region Errores â€” Propiedades de binding

        private bool MostrarError { get; set; }
        private string MensajeError { get; set; } = string.Empty;
        private string TituloVentanaError { get; set; } = string.Empty;

        #endregion

        #region Errores â€” LÃ³gica

        /// <summary>
        /// Muestra un popup si se ha producido un error en la carga de la pantalla.
        /// Se suscribe al evento DialogoErrores.OnError.
        /// </summary>
        private async Task MostrarPopupError(string nombrePantalla, Exception ex)
        {
            TituloVentanaError = nombrePantalla;
            MensajeError = await TraduccionesHelper.GetResourceValue("mensajes:ErrorAbriendoPantalla:label");
            MostrarError = true;
            await InvokeAsync(StateHasChanged);
            await RegistroAplicacion.RegistrarExcepcion(this.GetType().Name, ex);
        }

        private void CerrarPopupError()
        {
            MostrarError = false;
            Navigation.NavigateTo("/home", forceLoad: false);
        }

        #endregion
    }
}

