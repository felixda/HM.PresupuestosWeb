using System.Globalization;

namespace HM.Presupuestos.Web.Layout;

public partial class SelectorDeIdioma
{

    #region Servicios inyectados

    [Inject] private ILogger<SelectorDeIdioma> Logger { get; set; } = default!;

    #endregion

    #region Propiedades Bindeaedas a UI

    private string IdiomaSeleccionado { get; set; } = "";

    #endregion

    #region Ciclo de vida

    protected override async Task OnUsuarioDisponibleAsync()
    {
        CargarIdiomaActual();
        await RefrescarVista();
    }

    #endregion

    #region MÃ©todos privados

    private void CargarIdiomaActual()
    {
        IdiomaSeleccionado = GestorIdioma.IdiomaActual;

        AplicarCulturaDeIdioma(IdiomaSeleccionado);
    }

    private static void AplicarCulturaDeIdioma(string codigoIso)
    {
        if (string.IsNullOrWhiteSpace(codigoIso))
        {
            return;
        }

        var cultura = new CultureInfo(codigoIso);

        CultureInfo.DefaultThreadCurrentCulture = cultura;
        CultureInfo.DefaultThreadCurrentUICulture = cultura;
    }

    private async Task CambiarIdioma(string codigoIso)
    {
        try
        {
            RegistrarCambioDeIdioma(codigoIso);

            await GestorIdioma.CambiarIdioma(codigoIso);

            IdiomaSeleccionado = codigoIso;

            await RefrescarVista();
        }
        catch (Exception excepcion)
        {
            RegistrarErrorCambioDeIdioma(excepcion);
        }
    }

    private void RegistrarCambioDeIdioma(string codigoIso)
    {
        Logger.LogInformation("Cambiando idioma a: {Idioma}", codigoIso);
    }

    private void RegistrarErrorCambioDeIdioma(Exception excepcion)
    {
        Logger.LogError(excepcion, "Error al cambiar idioma");
    }

    private Task RefrescarVista()
    {
        return InvokeAsync(StateHasChanged);
    }

    private string ObtenerEtiquetaDeIdioma(string codigoIso)
    {
        return ObtenerTexto($"Language:{codigoIso}:label");
    }


    #endregion
}
