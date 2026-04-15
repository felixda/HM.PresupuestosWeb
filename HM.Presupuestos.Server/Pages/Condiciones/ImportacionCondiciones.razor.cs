


namespace HM.Presupuestos.Server.Pages.Condiciones
{
    public partial class ImportacionCondiciones
    {
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
                    PageTitle = T($"Menu:Menu_{(int)CodigosMenu.ImportacionCondiciones}:label");
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
            TextoToolTipAyuda = T("Pages:ImportacionCondiciones:ToolTip:label");
         //   _Jwt.Usuario = Usuario;

            _networks = await _PresupuestosService.ObtenerNetworks();
            _anios = await _VersionesService.ObtenerAniosConVersiones();

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
                _LayerOverlayService.Start();
                if (values.Any())
                {
                    string codigosNetwork = GetValoresSeleccionados<CodigoDescripcion, int>(values, x => x.Codigo, ",");
                    _gruposClientes = await _PresupuestosService.ObtenerGruposClientePorNetworks(codigosNetwork);
                }
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
                    _LayerOverlayService.Start();
                    _versiones = await ObtenerVersionesPorPermisos(anioSeleccionado);
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
        }

        private async Task ImportarCondicionesMMS()
        {
            if (!ValidarCamposObligatoriosFiltro())
            {
                await _MensajesHelper.MostrarMensajeInfo(PageTitle, T("mensajes:CamposObligatorios:label"));
                return;
            }
            try
            {
                
                bool confirmacion = await _MensajesHelper.MostrarMensajeParaConfirmacion(PageTitle, T("mensajes:AvisoImportarCondiciones:label"));
                if (!confirmacion) return;


                _LayerOverlayService.Start();

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

                await _CondicionesService.ImportarCondicionesMMS(_filtro);

                await _MensajesHelper.MostrarMensajeExito(PageTitle, T("mensajes:ImportacionCondicionesFinalizada:label"));

                ImportacionRealizada = true;
            }
            catch (ExcepcionBaseDatos exBd)
            {
                await TratarExcepcionGeneradaEnBD(exBd, PageTitle);
                ImportacionRealizada = false;
            }
            catch (Exception ex)
            {
                await _LogService.InsertException(this.GetType().Name, ex);
                await _MensajesHelper.MostrarMensajeError(PageTitle);
                ImportacionRealizada = false;
            }
            finally
            {
                _LayerOverlayService.Stop();
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
                await _LogService.InsertException(this.GetType().Name, ex);
                await _MensajesHelper.MostrarMensajeError(PageTitle);
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
            NavegacionService.SetDatos(_filtro);
            Navigation.NavigateTo("/gestion/planificacion-condiciones");
        }

    }
}


