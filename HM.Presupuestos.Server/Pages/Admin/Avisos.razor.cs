using HM.Presupuestos.Domain.Comun;
using HM.Presupuestos.Application.Servicios;
using HM.Presupuestos.Server.Services;

namespace HM.Presupuestos.Server.Pages.Admin
{
    public partial class Avisos
    {
        #region Inyección de Dependencias

        [Inject] protected IAvisosService AvisosService { get; set; } = default!;
        [Inject] protected ILogAccionesService LogAccionesService { get; set; } = default!;
        [Inject] protected DialogoErrores ErrorService { get; set; } = default!;
      

        #endregion

        #region Propiedades Privadas

        private bool _componentInitialized = false;
        private string _pageTitle { get; set; } = string.Empty;
        private string _mensaje = "";
        private string _error = "";
        private TiposDeAviso TipoAviso { get; set; } = TiposDeAviso.Warning;

        #endregion

        #region Ciclo de Vida

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                   // await InicializarAsync();
                    _pageTitle = ObtenerTexto($"Menu:Menu_{(int)CodigosMenu.Avisos}:label");
                    LayerOverlayService.Start($"{ObtenerTexto(AppResources.Common.Loading)} {_pageTitle}");
                }
                catch (Exception ex)
                {
                    await ErrorService.MostrarErrorInicializandoPagina(_pageTitle, ex);
                    return;
                }
                finally
                {
                    LayerOverlayService.Stop();
                }

                if (!_componentInitialized)
                {
                    _componentInitialized = true;
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        #endregion

        #region Métodos Privados

        private async Task EnviarAviso()
        {
            try
            {
                LayerOverlayService.Start();
                await AvisosService.ActivarAvisosAsync(_mensaje, TipoAviso);
                await LogAccionesService.Insertar(this.GetType().Name + " > " + ObtenerTexto("Common:LogActions:LogAction_3:label"));
            }
            catch (Exception ex)
            {
                await LogAccionesService.Insertar(this.GetType().Name + " > " + ObtenerTexto("Common:LogActions:LogAction_4:label"));
                _error = "Error al enviar el aviso: " + ex.Message;
            }
            finally
            {
                LayerOverlayService.Stop();
            }
        }

        #endregion

        #region Propiedades Calculadas

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

        #endregion
    }
}
