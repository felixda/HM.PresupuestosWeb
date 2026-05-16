using HM.Core.Comun.v6.Seguridad.Interfaces;

namespace HM.Presupuestos.Server.Pages.Condiciones
{
    public partial class ImportacionCondiciones
    {
        #region Inyecci�n de Dependencias

        [Inject] protected IJwt Jwt { get; set; } = default!;
        [Inject] protected IPresupuestosService PresupuestosService { get; set; } = default!;
        [Inject] protected ICondicionesService CondicionesService { get; set; } = default!;
        [Inject] protected DialogoErrores ErrorService { get; set; } = default!;
        [Inject] protected TraduccionesHelper Traducciones { get; set; } = default!;
        [Inject] protected ILogAccionesService LogAccionesService { get; set; } = default!;
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

        private string _querySearchGrupoClientesList { get; set; } = String.Empty;

        private CondicionImportarFiltro _filtro;

        private List<CodigoDescripcion> _networks = [];
        private object? _networskSeleccionados { get; set; } = null;

        private object? NetworkSeleccionados
        {
            get => _networskSeleccionados;
            set
            {
                if (_networskSeleccionados != value)
                {
                    _networskSeleccionados = value;
                    if (value == null)
                    {
                        _gruposClientesSeleccionados = null;
                        _gruposClientes = [];
                    }
                }
            }
        }



        private List<CodigoDescripcion> _gruposClientes = [];

        private object? _gruposClientesSeleccionados { get; set; }

        private List<CodigoDescripcion> _anios = [];
        private CodigoDescripcion? _anioSeleccionado { get; set; } = null;

        private List<VersionResumen> _versiones = [];
        private VersionResumen? _versionSeleccionada { get; set; } = null;


        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await base.OnAfterRenderAsync(firstRender);
                try
                {
                    PageTitle = ObtenerTexto($"Menu:Menu_{(int)CodigosMenu.ImportacionCondiciones}:label");
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
            TextoToolTipAyuda = ObtenerTexto(AppResources.Pages.ImportacionCondiciones.ToolTip);
         //   Jwt.Usuario = Usuario;

            _networks = await PresupuestosService.ObtenerNetworks();
            _anios = await VersionesService.ObtenerAniosConVersiones();

            FilterInit();
        }

        private void FilterInit()
        {
            if (_networks.Count == 1)
            {
                _networskSeleccionados = new List<CodigoDescripcion> { _networks[0] };
            }
            else if (_networks.Count > 1)
            {
                _networskSeleccionados = null;
            }
        }

        private async Task DxListBoxNetwoks_ValuesChanged(IEnumerable<CodigoDescripcion> values, IDropDownBox dropDownBox, bool esSingle)
        {
            dropDownBox.BeginUpdate();
            dropDownBox.Value = values;

            _gruposClientesSeleccionados = null;
            _gruposClientes = [];

            try
            {
                LayerOverlayService.Start();
                if (values.Any())
                {
                    string codigosNetwork = ObtenerValoresSeleccionados<CodigoDescripcion, int>(values, x => x.Codigo, ",");
                    _gruposClientes = await PresupuestosService.ObtenerGruposClientePorNetworks(codigosNetwork);
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
            _versionSeleccionada = null;
            _versiones = [];
            if (e.DataItem != null)
            {
                int anioSeleccionado = e.DataItem.Codigo;
                try
                {
                    LayerOverlayService.Start();
                    _versiones = await ObtenerVersionesPorPermisos(anioSeleccionado);
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
                await MensajesHelper.MostrarMensajeInfo(PageTitle, ObtenerTexto(AppResources.Mensajes.CamposObligatorios));
                return;
            }
            try
            {
                
                bool confirmacion = await MensajesHelper.MostrarMensajeParaConfirmacion(PageTitle, ObtenerTexto(AppResources.Mensajes.AvisoImportarCondiciones));
                if (!confirmacion) return;


                LayerOverlayService.Start();

                _filtro = new CondicionImportarFiltro()
                {
                    CodigosNetwork = _networskSeleccionados == null
                        ? [.. (_networks).Select(x => x.Codigo)]
                        : [.. ((List<CodigoDescripcion>)_networskSeleccionados).Select(x => x.Codigo)],
                    CodigosGrupoCliente = _gruposClientesSeleccionados == null
                        ? [-1]
                        : [.. ((List<CodigoDescripcion>)_gruposClientesSeleccionados).Select(x => x.Codigo)],
                    Anio = Convert.ToInt32(_anioSeleccionado!.Descripcion),
                    CodigoVersion = _versionSeleccionada!.Codigo
                };

                await CondicionesService.ImportarCondicionesMMS(_filtro);

                await MensajesHelper.MostrarMensajeExito(PageTitle, ObtenerTexto(AppResources.Mensajes.ImportacionCondicionesFinalizada));

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

            if ( _anioSeleccionado == null || _versionSeleccionada == null)
            {
                validacion = false;
            }

            return validacion;
        }

        private async Task FiltroLimpiar()
        {
            try
            {
                _anioSeleccionado = null;
                _versionSeleccionada = null;
                _gruposClientesSeleccionados = null;
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


