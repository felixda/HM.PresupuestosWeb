using HM.Presupuestos.Application.CasosDeUso.Compartido;

namespace HM.Presupuestos.Web.Pages.Condiciones
{
    public partial class ImportacionCondiciones
    {
        #region Inyecci?n de Dependencias

        [Inject] protected IJwt Jwt { get; set; } = default!;
        [Inject] protected IMaestrosService PresupuestosService { get; set; } = default!;
        [Inject] protected ICondicionesService CondicionesService { get; set; } = default!;
        [Inject] protected DialogoErrores ErrorService { get; set; } = default!;
        [Inject] protected NavigationManager Navigation { get; set; } = default!;
        [Inject] protected ParametrosNavegacion NavegacionService { get; set; } = default!;

        #endregion
        #region Propiedades privadas

        #region Page

        private bool _componentInitialized = false;
        private string PageTitle { get; set; } = string.Empty;
        private string TextoToolTipAyuda { get; set; } = string.Empty;



        #endregion
        #endregion

        #region Filtro

        bool ImportacionRealizada { get; set; } = false;

        private string QuerySearchGrupoClientesList { get; set; } = String.Empty;

        private CondicionImportarFiltro _filtro;

        private List<CodigoDescripcion> Networks { get; set; } = [];
        private object? NetworksSeleccionadosBackingField { get; set; } = null;

        private object? NetworkSeleccionados
        {
            get => NetworksSeleccionadosBackingField;
            set
            {
                if (NetworksSeleccionadosBackingField != value)
                {
                    NetworksSeleccionadosBackingField = value;
                    if (value == null)
                    {
                        GruposClientesSeleccionados = null;
                        GruposClientes = [];
                    }
                }
            }
        }



        private List<CodigoDescripcion> GruposClientes { get; set; } = [];

        private object? GruposClientesSeleccionados { get; set; }

        private List<CodigoDescripcion> Anios { get; set; } = [];
        private CodigoDescripcion? AnioSeleccionado { get; set; } = null;

        private List<VersionResumen> Versiones { get; set; } = [];
        private VersionResumen? VersionSeleccionada { get; set; } = null;


        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await base.OnAfterRenderAsync(firstRender);
                try
                {
                    PageTitle = ObtenerTexto($"Menu:Menu_{(int)CodigosMenu.ImportacionCondiciones}:label");
                    LayerOverlayService.Start($"{ObtenerTexto(TextosApp.Common.Loading)} {PageTitle}");
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
            TextoToolTipAyuda = ObtenerTexto(TextosApp.Pages.ImportacionCondiciones.ToolTip);
         //   Jwt.Usuario = Usuario;

            Networks = await PresupuestosService.ObtenerNetworks();
            Anios = await VersionesService.ObtenerAniosConVersiones();

            FilterInit();
        }

        private void FilterInit()
        {
            if (Networks.Count == 1)
            {
                NetworksSeleccionadosBackingField = new List<CodigoDescripcion> { Networks[0] };
            }
            else if (Networks.Count > 1)
            {
                NetworksSeleccionadosBackingField = null;
            }
        }

        private async Task DxListBoxNetwoks_ValuesChanged(IEnumerable<CodigoDescripcion> values, IDropDownBox dropDownBox, bool esSingle)
        {
            dropDownBox.BeginUpdate();
            dropDownBox.Value = values;

            GruposClientesSeleccionados = null;
            GruposClientes = [];

            try
            {
                LayerOverlayService.Start();
                if (values.Any())
                {
                    string codigosNetwork = ObtenerValoresSeleccionados<CodigoDescripcion, int>(values, x => x.Codigo, ",");
                    GruposClientes = await PresupuestosService.ObtenerGruposClientePorNetworks(codigosNetwork);
                }
            }
            catch (Exception ex)
            {
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(PageTitle);
            }
            finally
            {
                LayerOverlayService.Stop();
            }
            dropDownBox.EndUpdate();
            if (esSingle)
                dropDownBox.HideDropDown();
        }

       
        private async Task ComboAnios_SelectedDataItemChanged(SelectedDataItemChangedEventArgs<CodigoDescripcion> e)
        {
            VersionSeleccionada = null;
            Versiones = [];
            if (e.DataItem != null)
            {
                int anioSeleccionado = e.DataItem.Codigo;
                try
                {
                    LayerOverlayService.Start();
                    Versiones = await ObtenerVersionesPorPermisos(anioSeleccionado);
                }
                catch (Exception ex)
                {
                    await RegistroAplicacion.RegistrarExcepcion(ex);
                    await MensajesHelper.MostrarMensajeError(PageTitle);
                }
                finally
                {
                    LayerOverlayService.Stop();
                }
            }
        }

        private async Task ImportarCondicionesMMS()
        {
            if (!ValidarCamposObligatoriosFiltro())
            {
                await MensajesHelper.MostrarMensajeInfo(PageTitle, ObtenerTexto(TextosApp.Mensajes.CamposObligatorios));
                return;
            }
            try
            {
                
                bool confirmacion = await MensajesHelper.MostrarMensajeParaConfirmacion(PageTitle, ObtenerTexto(TextosApp.Mensajes.AvisoImportarCondiciones));
                if (!confirmacion) return;


                LayerOverlayService.Start();

                _filtro = new CondicionImportarFiltro()
                {
                    CodigosNetwork = NetworksSeleccionadosBackingField == null
                        ? [.. (Networks).Select(x => x.Codigo)]
                        : [.. ((List<CodigoDescripcion>)NetworksSeleccionadosBackingField).Select(x => x.Codigo)],
                    CodigosGrupoCliente = GruposClientesSeleccionados == null
                        ? [-1]
                        : [.. ((List<CodigoDescripcion>)GruposClientesSeleccionados).Select(x => x.Codigo)],
                    Anio = Convert.ToInt32(AnioSeleccionado!.Descripcion),
                    CodigoVersion = VersionSeleccionada!.Codigo
                };

                await CondicionesService.ImportarCondicionesMMS(_filtro);

                await MensajesHelper.MostrarMensajeExito(PageTitle, ObtenerTexto(TextosApp.Mensajes.ImportacionCondicionesFinalizada));

                ImportacionRealizada = true;
            }
            catch (ExcepcionBaseDatos exBd)
            {
                await TratarExcepcionGeneradaEnBD(exBd, PageTitle);
                ImportacionRealizada = false;
            }
            catch (Exception ex)
            {
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(PageTitle);
                ImportacionRealizada = false;
            }
            finally
            {
                LayerOverlayService.Stop();
            }
        }

        private bool ValidarCamposObligatoriosFiltro()
        {
            bool validacion = true;

            if ( AnioSeleccionado == null || VersionSeleccionada == null)
            {
                validacion = false;
            }

            return validacion;
        }

        private async Task FiltroLimpiar()
        {
            try
            {
                AnioSeleccionado = null;
                VersionSeleccionada = null;
                GruposClientesSeleccionados = null;
                FilterInit();
            }
            catch (Exception ex)
            {
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(PageTitle);
            }
        }


        private void DxListBox_ValuesChanged<T>(IEnumerable<T> values, IDropDownBox dropDownBox)
        {
            dropDownBox.BeginUpdate();
            dropDownBox.Value = values;
            dropDownBox.EndUpdate();
        }

        private void IrACondiciones()
        {
            NavegacionService.Guardar(_filtro);
            Navigation.NavigateTo("/gestion/planificacion-condiciones");
        }

    }
}



