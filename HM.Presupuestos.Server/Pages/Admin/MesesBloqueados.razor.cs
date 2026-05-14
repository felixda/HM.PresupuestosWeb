using DevExpress.Blazor;
using HM.Core.Comun.v6.Seguridad.Interfaces;
using HM.Presupuestos.Application.Servicios;
using HM.Presupuestos.Domain.Comun;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Server.Services;

namespace HM.Presupuestos.Server.Pages.Admin
{
    public partial class MesesBloqueados
    {
        #region Inyección de Dependencias

        [Inject] protected IJwt Jwt { get; set; } = default!;
        [Inject] protected IAdminService AdminService { get; set; } = default!;
        [Inject] protected IVersionesService VersionesService { get; set; } = default!;
        [Inject] protected ITraductorRecursos ResourceService { get; set; } = default!;
        [Inject] protected TraduccionesHelper Traducciones { get; set; } = default!;
        [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] protected ILogService LogService { get; set; } = default!;
        [Inject] protected MensajesHelper MensajesHelper { get; set; } = default!;
        [Inject] protected ErrorDialogService ErrorService { get; set; } = default!;
        [Inject] protected ILogAccionesService LogAccionesService { get; set; } = default!;
        [Inject] protected ILayerOverlayService LayerOverlayService { get; set; } = default!;

        #endregion


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
                    PageTitle = ObtenerTexto($"Menu:Menu_{(int)CodigosMenu.MesesBloqueados}:label");
                    LayerOverlayService.Start($"{ObtenerTexto(AppResources.Common.Loading)} {PageTitle}");
                    await PageInitialize();
                }
                catch (Exception ex)
                {
                    await ErrorService.MostrarErrorInicializandoPagina(PageTitle, ex);
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

        private async Task PageInitialize()
        {
            try
            {
               // _Jwt.Usuario = User;
                TextoToolTipAyuda = ObtenerTexto(AppResources.Pages.MesesBloqueados.ToolTip);

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
                await LogService.InsertException(ex);
                await MensajesHelper.MostrarMensajeError(PageTitle, ObtenerTexto(AppResources.Common.Messages.UndefinedError));
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
                    LayerOverlayService.Start();
                    List<int> mesesBloqueado = await AdminService.ObtenerMesesBloqueados(anioSeleccionado);
                    MesesSeleccionados = Meses
                       .Where(m => mesesBloqueado.Contains(m.Codigo))
                       .ToList();
                }
                catch (Exception ex)
                {
                    await LogService.InsertException(ex);
                    await MensajesHelper.MostrarMensajeError(PageTitle);
                }
                finally
                {
                    LayerOverlayService.Stop();
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
                LayerOverlayService.Start();
                List<int> mesesSeleccionados = [
                        .. (MesesSeleccionados?.Cast<CodigoDescripcion>()
                                              .Select(x => x.Codigo) ?? [])
                    ];

                await AdminService.InsertarMesesBloqueado(AnioSeleccionado!.Codigo, mesesSeleccionados);
                await MensajesHelper.MostrarMensajeInfo(PageTitle, ObtenerTexto(AppResources.Common.DatosGrabados));
            }
            catch (Exception ex)
            {
                await LogService.InsertException(ex);
                await MensajesHelper.MostrarMensajeError(PageTitle);
            }
            finally
            {
                LayerOverlayService.Stop();
            }
        }

       
        #endregion

    }
}
