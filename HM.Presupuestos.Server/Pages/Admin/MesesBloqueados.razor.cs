using DevExpress.Blazor;
using HM.Presupuestos.Application.Servicios;
using HM.Presupuestos.Contratos.Comun;
using HM.Presupuestos.Contratos.Entidades;


namespace HM.Presupuestos.Server.Pages.Admin
{
    public partial class MesesBloqueados
    {
        #region Private Properties

        private bool _componentInitialized = false;
        private string PageTitle { get; set; } = string.Empty;
        private string TextoToolTipAyuda { get; set; } = string.Empty;
        private List<CodigoDescripcion> Anios = [];
        private CodigoDescripcion? AnioSeleccionado { get; set; } = new();
        private bool HayAnioSeleccionado => AnioSeleccionado is not null && AnioSeleccionado.Codigo > 0;

        #endregion

        #region Grid Meses

        DxGrid GridMeses { get; set; } = new DxGrid();

        private List<CodigoDescripcion> Meses = [];
        IReadOnlyList<object> MesesSeleccionados { get; set; } = [];

        #endregion

        #region Page

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    //await InicializarAsync();
                    PageTitle = T($"Menu:Menu_{(int)CodigosMenu.MesesBloqueados}:label");
                    _LayerOverlayService.Start($"{T("Common:loading:label")} {PageTitle}");
                    await PageInitialize();
                }
                catch (Exception ex)
                {
                    await _ErrorService.MostrarErrorInicializandoPagina(PageTitle, ex);
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

        private async Task PageInitialize()
        {
            try
            {
               // _Jwt.Usuario = User;
                TextoToolTipAyuda = T("Pages:MesesBloqueados:ToolTip:label");

                Meses = await Traducciones.ObtenerMeses();
                Anios = await VersionesService.ObtenerAniosConVersiones(true);

                if (Anios.Count > 0)
                {
                    int anioActual = DateTime.Now.Year;
                    AnioSeleccionado = Anios.FirstOrDefault(x => x.Codigo == anioActual)
                                    ?? Anios.First();

                }
            }
            catch (Exception ex)
            {
                await _LogService.InsertException(this.GetType().Name, ex);
                await _MensajesHelper.MostrarMensajeError(PageTitle, T("Common:Messages:UndefinedError:label"));
            }
        }
       
        #endregion

        #region Eventos

        private async Task ComboAnios_SelectedDataItemChanged(SelectedDataItemChangedEventArgs<CodigoDescripcion> e)
        {
            if (e.DataItem != null)
            {
                int anioSeleccionado = e.DataItem.Codigo;
                try
                {
                    _LayerOverlayService.Start();
                    List<int> mesesBloqueado = await AdminService.ObtenerMesesBloqueados(anioSeleccionado);
                    MesesSeleccionados = Meses
                       .Where(m => mesesBloqueado.Contains(m.Codigo))
                       .ToList();
                }
                catch (Exception ex)
                {
                    await _LogService.InsertException(this.GetType().Name, ex);
                    await _MensajesHelper.MostrarMensajeError(PageTitle);
                }
                finally
                {
                    _LayerOverlayService.Stop();
                }
            }
            else
            {
                GridMeses.ClearSelection();
            }
        }

        private async Task Grabar_Click()
        {
            try
            {
                _LayerOverlayService.Start();
                List<int> mesesSeleccionados = [
                        .. (MesesSeleccionados?.Cast<CodigoDescripcion>()
                                              .Select(x => x.Codigo) ?? [])
                    ];

                await AdminService.InsertarMesesBloqueado(AnioSeleccionado!.Codigo, mesesSeleccionados);
                await _MensajesHelper.MostrarMensajeInfo(PageTitle, T("common:DatosGrabados:label"));
            }
            catch (Exception ex)
            {
                await _LogService.InsertException(this.GetType().Name, ex);
                await _MensajesHelper.MostrarMensajeError(PageTitle);
            }
            finally
            {
                _LayerOverlayService.Stop();
            }
        }

       
        #endregion

    }
}
