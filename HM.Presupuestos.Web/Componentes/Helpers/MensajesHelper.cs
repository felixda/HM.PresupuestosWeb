
namespace HM.Presupuestos.Web.Componentes.Helpers;

public class MensajesHelper(IDialogService dialogService, ILocalizadorRecursos resourceService, ILayerOverlayService layerOverlayService)
{

    private readonly IDialogService _dialogService = dialogService;
    private readonly ILocalizadorRecursos _resourceService = resourceService;
    private readonly ILayerOverlayService _LayerOverlayService = layerOverlayService;


    /// <summary>
    /// Muestra un mensaje con formato general con un unico botón para cerrarlo (OK)
    /// </summary>
    /// <param name="titulo">Texto a mostrar en el titulo del mensaje</param>
    /// <param name="mensaje">Texto a mostrara en el cuerpo del mensaje</param>
    /// <param name="estilo">Estilo con el que se muestra (cambia el icono y su color)</param>
    /// <returns></returns>
    public async Task MostrarMensajeGeneral(string titulo, string mensaje, MessageBoxRenderStyle estilo)
    {
        _LayerOverlayService.Stop();
        await _dialogService.AlertAsync(new MessageBoxOptions()
        {
            Title = titulo,
            Text = mensaje,
            OkButtonText = "OK",
            ShowIcon = true,
            ShowCloseButton = false,
            ThemeMode = MessageBoxThemeMode.Light,
            RenderStyle = estilo,
        });
    }

    public async Task MostrarMensajeError(string titulo, string? mensaje = null)
    {
        _LayerOverlayService.Stop();
        if (string.IsNullOrEmpty(mensaje))
        {
            mensaje = _resourceService.ObtenerTexto("mensajes:ErrorGeneral:label");
        }
        await MostrarMensajeGeneral(titulo, mensaje, MessageBoxRenderStyle.Danger);
    }

    public async Task MostrarMensajeInfo(string titulo, string mensaje)
    {
        await MostrarMensajeGeneral(titulo, mensaje, MessageBoxRenderStyle.Info);
    }

    public async Task MostrarMensajeExito(string titulo, string mensaje)
    {
        await MostrarMensajeGeneral(titulo, mensaje, MessageBoxRenderStyle.Success);
    }

    public async Task MostrarMensajeAviso(string titulo, string mensaje)
    {
        await MostrarMensajeGeneral(titulo, mensaje, MessageBoxRenderStyle.Warning);
    }

    public async Task<bool> MostrarMensajeParaConfirmacion(string titulo, string mensaje)
    {
        _LayerOverlayService.Stop();
        bool confirmacionCancelar = await _dialogService.ConfirmAsync(new MessageBoxOptions()
        {
            Title = titulo,
            Text = mensaje,
            OkButtonText = _resourceService.ObtenerTexto("Common:Si:label"),
            CancelButtonText = _resourceService.ObtenerTexto("Common:No:label"), 
            ShowIcon = true,
            ShowCloseButton = false,
            ThemeMode = MessageBoxThemeMode.Light,
            RenderStyle = MessageBoxRenderStyle.Info,
        }); 
        return confirmacionCancelar;
    }

   
}





