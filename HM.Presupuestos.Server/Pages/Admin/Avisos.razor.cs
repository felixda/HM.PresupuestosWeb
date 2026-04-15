using HM.Presupuestos.Contratos.Comun;

namespace HM.Presupuestos.Server.Pages.Admin
{
    public partial class Avisos
    {
        private bool _componentInitialized = false;
        private string _pageTitle { get; set; } = string.Empty;
        private string _mensaje = "";
        private string _error = "";
        private TiposDeAviso TipoAviso { get; set; } = TiposDeAviso.Warning;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                   // await InicializarAsync();
                    _pageTitle = T($"Menu:Menu_{(int)CodigosMenu.Avisos}:label");
                    _LayerOverlayService.Start($"{T("Common:loading:label")} {_pageTitle}");
                }
                catch (Exception ex)
                {
                    await _ErrorService.MostrarErrorInicializandoPagina(_pageTitle, ex);
                    return;
                }
                finally
                {
                    _LayerOverlayService.Stop();
                }

                if (!_componentInitialized)
                {
                    _componentInitialized = true;
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        private async Task EnviarAviso()
        {
            try
            {
                _LayerOverlayService.Start();
                await AvisosService.ActivarAvisosAsync(_mensaje, TipoAviso);
                await _LogAccionesService.Insertar(this.GetType().Name + " > " + T("Common:LogActions:LogAction_3:label"));
            }
            catch (Exception ex)
            {
                await _LogAccionesService.Insertar(this.GetType().Name + " > " + T("Common:LogActions:LogAction_4:label"));
                _error = "Error al enviar el aviso: " + ex.Message;
            }
            finally
            {
                _LayerOverlayService.Stop();
            }
        }

        private string ClaseAviso => TipoAviso switch
        {
            TiposDeAviso.Info => "info",
            TiposDeAviso.Success => "success",
            TiposDeAviso.Warning => "warning",
            TiposDeAviso.Error => "error",
            _ => "warning"
        };

        private string ClseTextArea => $"form-control textarea-{ClaseAviso}";

        private string ClaseMarco => TipoAviso switch
        {
            TiposDeAviso.Info => "radio-card-info",
            TiposDeAviso.Success => "radio-card-success",
            TiposDeAviso.Warning => "radio-card-warning",
            TiposDeAviso.Error => "radio-card-error",
            _ => "radio-card-warning"
        };
    }
}
