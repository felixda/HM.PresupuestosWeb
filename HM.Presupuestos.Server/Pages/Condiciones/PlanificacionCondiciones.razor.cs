using HM.Core.Comun.v6.Seguridad.Interfaces;
using static HM.Presupuestos.Application.Servicios.CondicionesService;

namespace HM.Presupuestos.Server.Pages.Condiciones
{
    public partial class PlanificacionCondiciones
    {
        #region Inyecci�n de Dependencias

        [Inject] protected IJwt Jwt { get; set; } = default!;
        [Inject] protected IPresupuestosService PresupuestosService { get; set; } = default!;
        [Inject] protected ICondicionesService CondicionesService { get; set; } = default!;
        [Inject] protected ILogAccionesService LogAccionesService { get; set; } = default!;

        [Inject] protected ErrorDialogService ErrorService { get; set; } = default!;
        [Inject] protected TraduccionesHelper Traducciones { get; set; } = default!;
        [Inject] protected NavegacionService NavegacionService { get; set; } = default!;

        #endregion
        #region Propiedades privadas
        private bool _desdePaginaImportarMMS = false;


        #region Page

        private bool _componentInitialized = false;
        private string PageTitle { get; set; } = string.Empty;
        private string TextoToolTipAyuda { get; set; } = string.Empty;

        private bool HayCambiosPendientes => _condicionesNoGuardados.Count > 0 || ExcepcionesNoGuardadas.Count > 0;

        #endregion

        #region Filtro

        private string _radioGroupAcuerdoChecked { get; set; } = string.Empty;
        private IEnumerable<string>? _radioGroupAcuerdoButtonList;
        
        private CondicionFiltro _filtroCondiciones = new();

        private List<CodigoDescripcion> _networks = [];
        private CodigoDescripcion? _networkSeleccionado { get; set; } = null;

        private List<CodigoDescripcion> _gruposClientes = [];
        private int? _codigoGrupoSeleccionado { get; set; }

        private List<CodigoDescripcion> _anios = [];
        private CodigoDescripcion? _anioSeleccionado { get; set; } = null;

        private List<VersionResumen> _versiones = [];
        private VersionResumen? _versionSeleccionada { get; set; } = null;

        private List<CodigoDescripcion> _mediosMaster = [];

        #endregion

        #region Vigencias

        List<Vigencia> _vigencias = []; //Todas las vigencias encontradas
        Vigencia? _vigenciaSeleccionada { get; set; } = null;

        private string LeftCaptionVigencias = string.Empty;
        private string RightCaptionVigencias = string.Empty;

        private string LeftCaptionCondiciones = string.Empty;
        private string RightCaptionCondiciones = string.Empty;

        


        #endregion

        #region Grid Condiciones

        private DxGrid GridCondiciones { get; set; } = new DxGrid();
        private List<CondicionDto> _condiciones = [];
        private List<CondicionDto> _condicionesCache = [];

        private List<CodigoDescripcion> _indicadoresDevolucion = [];
        private Dictionary<CondicionDto, DatosCondicionCambiados> _condicionesNoGuardados { get; } = [];

        #endregion

        #region Grid Excepciones

        private DxGrid GridExcepciones { get; set; } = new DxGrid();
        private List<ExcepcionDto> _excepciones = [];
        private List<ExcepcionDto> _excepcionesCache = [];

        private List<CodigoDescripcion> _alcances = [];
        private List<CodigoDescripcion> _disciplinas = [];
        private List<CodigoDescripcion> _diversifieds = [];
        private List<CodigoDescripcion> _objetivos = [];
        private List<CodigoDescripcion> _tiposCompra = [];
        private List<CodigoDescripcion> _tiposDisciplina = [];
        private List<CodigoDescripcion> _disciplinasGrupo = [];
        private Dictionary<ExcepcionDto, DatosExcepcionesCondicionCambiados> ExcepcionesNoGuardadas { get; } = [];

        private string _tituloGridExcepciones = string.Empty;

        // Cach� para optimizar b�squedas de condiciones por medio
        private Dictionary<int, List<CondicionDto>>? _condicionesPorMedio;

        private enum AccionJerarquias
        {
            Subir,
            Bajar,
        }

        bool ActivarBotonNuevaExcepcion = true;

        #endregion


        #region Popup Vigencia

        private bool _popupVigenciaVisible { get; set; } = false;
        private string _tituloPopupVigencia = string.Empty;
        private Vigencia _vigenciaNueva = new();
        private List<CodigoDescripcion> _meses = [];
        private CodigoDescripcion? _mesDesdeVigenciaSeleccionado { get; set; } = null;
        private CodigoDescripcion? _mesHastaVigenciaSeleccionado { get; set; } = null;
        private CodigoDescripcion? _medioSeleccionadoDesdePopup { get; set; } = null;
        private ModoEdicion _modoEdicionVigencia = ModoEdicion.Alta;

        #endregion

        #region Popup Medios

        private bool _popupMediosVisible { get; set; } = false;
        private List<CodigoDescripcion> _medios = [];
        private int? _codigoMedioleccionadoParaFiltro;

        #endregion


        #endregion


        #region Page

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await base.OnAfterRenderAsync(firstRender);
                try
                {
                    PageTitle = T($"Menu:Menu_{(int)CodigosMenu.PlanificacionCondiciones}:label");
                    LayerOverlayService.Start($"{T(AppResources.Common.Loading)} {PageTitle}");
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
            _radioGroupAcuerdoButtonList = [T(AppResources.Pages.PlanificacionCondiciones.Condiciones), 
                T(AppResources.Pages.PlanificacionCondiciones.CondicionesAcuerdos)];
            _radioGroupAcuerdoChecked = T(AppResources.Pages.PlanificacionCondiciones.Condiciones);

            TextoToolTipAyuda = T(AppResources.Pages.PlanificacionCondiciones.ToolTip);
            
            LeftCaptionVigencias = T("Pages: PlanificacionCondiciones: Vigencias: label");
            LeftCaptionCondiciones = T("Pages: PlanificacionCondiciones: Condiciones: label");

            if (!String.IsNullOrEmpty(_radioGroupAcuerdoChecked))
            {
                PageTitle = _radioGroupAcuerdoChecked;
            }

           // Jwt.Usuario = User;

            _anios = await VersionesService.ObtenerAniosConVersiones();
            _networks = await PresupuestosService.ObtenerNetworks();
            _meses = await Traducciones.ObtenerMeses();

            _indicadoresDevolucion.Add(new CodigoDescripcion { Codigo = 0, Descripcion = "" });
            _indicadoresDevolucion.Add(new CodigoDescripcion { Codigo = 1, Descripcion = T(AppResources.Pages.PlanificacionCondiciones.NetoVenta) });
            _indicadoresDevolucion.Add(new CodigoDescripcion { Codigo = 2, Descripcion = T(AppResources.Pages.PlanificacionCondiciones.Sobreprima) });

            _alcances = await PresupuestosService.ObtenerAlcances();
            _alcances.ForEach(m => m.Descripcion = Helper.StringHelper.Capitalize(m.Descripcion));
            InsertarFilaVacia(_alcances);

            _disciplinas = await PresupuestosService.ObtenerDisciplinas();
            _disciplinas.ForEach(m => m.Descripcion = Helper.StringHelper.Capitalize(m.Descripcion));
            InsertarFilaVacia(_disciplinas);

            _diversifieds = await PresupuestosService.ObtenerDiversifiedsNCB();
            _diversifieds.ForEach(m => m.Descripcion = Helper.StringHelper.Capitalize(m.Descripcion));
            InsertarFilaVacia(_diversifieds);

            _objetivos = await PresupuestosService.ObtenerObjetivos();
            _objetivos.ForEach(m => m.Descripcion = Helper.StringHelper.Capitalize(m.Descripcion));
            InsertarFilaVacia(_objetivos);

            _tiposCompra = await PresupuestosService.ObtenerTiposCompra();
            _tiposCompra.ForEach(m => m.Descripcion = Helper.StringHelper.Capitalize(m.Descripcion));
            InsertarFilaVacia(_tiposCompra);

            _disciplinasGrupo = await PresupuestosService.ObtenerDisciplinasGrupos();
            _disciplinasGrupo.ForEach(m => m.Descripcion = Helper.StringHelper.Capitalize(m.Descripcion));
            InsertarFilaVacia(_disciplinasGrupo);

            _tiposDisciplina = await PresupuestosService.ObtenerTiposDisciplinas();
            _tiposDisciplina.ForEach(m => m.Descripcion = Helper.StringHelper.Capitalize(m.Descripcion));
            InsertarFilaVacia(_tiposDisciplina);

            _tituloGridExcepciones = T(AppResources.Pages.PlanificacionCondiciones.Excepciones);

            await ManageRequest();
          
           
        }

        #endregion


        #region Filtro

        /// <summary>
        /// Initialize filter values
        /// </summary>
        private async Task FilterInit()
        {
            if (_networks.Count == 1)
            {
                _networkSeleccionado = _networks[0];
                _gruposClientes = await PresupuestosService.ObtenerGruposClientePorNetwork(_networkSeleccionado.Codigo);
                _mediosMaster = await PresupuestosService.ObtenerMediosPorNetWork(_networkSeleccionado.Codigo.ToString());
            }
            if ( _gruposClientes.Count == 1)
            {
                _codigoGrupoSeleccionado = _gruposClientes[0].Codigo;
            }
        }

        private async Task RadioGroupAcuerdo_ValueChanged(string newValue)
        {
            if (!await ComprobarSiHayCambiosPendienteAndSeguir())
            {
                return;
            }
            _radioGroupAcuerdoChecked = newValue;
            if (!String.IsNullOrEmpty(_radioGroupAcuerdoChecked))
            {
                PageTitle = _radioGroupAcuerdoChecked;
            }
        }

        private async Task ComboBoxNetwork_SelectedDataItemChanged(SelectedDataItemChangedEventArgs<CodigoDescripcion> e)
        {
            if (e.ChangeSource == SelectionChangeSource.UserAction)
            {
                _codigoGrupoSeleccionado = null;
                _gruposClientes = [];
                _networkSeleccionado = e.DataItem;
                if (_networkSeleccionado is null) return;

                try
                {
                    LayerOverlayService.Start();
                    _gruposClientes = await PresupuestosService.ObtenerGruposClientePorNetwork(_networkSeleccionado.Codigo);
                    if (_gruposClientes.Count == 1)
                    {
                        _codigoGrupoSeleccionado = _gruposClientes[0].Codigo;
                    }

                    _mediosMaster = await PresupuestosService.ObtenerMediosPorNetWork(_networkSeleccionado.Codigo.ToString());
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
        }

        private async Task ComboAnios_SelectedDataItemChanged(SelectedDataItemChangedEventArgs<CodigoDescripcion> e)
        {
            if (e.ChangeSource == SelectionChangeSource.UserAction)
            {
                _versionSeleccionada = null;
                _versiones = [];
                if (e.DataItem is null) return;

                int anioSeleccionado = e.DataItem.Codigo;
                try
                {
                    LayerOverlayService.Start();
                    _versiones = await ObtenerVersionesPorPermisos(anioSeleccionado);
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
        }

        private async Task ComboBoxVigencias_SelectedDataItemChanged(SelectedDataItemChangedEventArgs<Vigencia> e)
        {
            try
            {
                if (e.DataItem != null)
                {

                    await ObtenerCondicionesExcepciones(e.DataItem.Codigo);
                    RightCaptionCondiciones = $"[{e.DataItem.Descripcion}]";
                }
                else
                {
                    _condiciones.Clear();
                    _condicionesCache.Clear();
                    _excepciones.Clear();
                    _excepcionesCache.Clear();

                    _tituloGridExcepciones = T(AppResources.Pages.PlanificacionCondiciones.Excepciones);
                    _codigoMedioleccionadoParaFiltro = null;
                }
            }
            catch (Exception ex)
            {
                await LogService.InsertException(ex);
                await MensajesHelper.MostrarMensajeError(PageTitle);
            }
        }


        private void ActualizarCondicionesConMedioAccesible()
        {
            var codigosMediosAccesibles = new HashSet<int>(
                   _mediosMaster.Select(m => m.Codigo));

            foreach (var item in _condiciones)
            {
                item.MedioAccesible = codigosMediosAccesibles.Contains(item.CodigoMedio);
            }

            _condiciones.RemoveAll(c => c.PctSAG == 0 && c.PctDevolucion == 0 && c.PctManPower == 0 && c.MedioAccesible == false);
        }

        private void ActulizarExcepcionesConMedioAccesible()
        {
            var codigosMediosAccesibles = new HashSet<int>(
                _mediosMaster.Select(m => m.Codigo));
            foreach (var item in _excepciones)
            {
                item.MedioAccesible = codigosMediosAccesibles.Contains(item.CodigoMedio);
            }
        }
        private async Task ObtenerCondicionesExcepciones(int codigoVigencia)
        {
            try
            {
                LayerOverlayService.Start();
                _condiciones = await CondicionesService.ObtenerCondicionesPorVigencia(codigoVigencia, _networkSeleccionado!.Codigo);
                _condiciones.ForEach(m => m.DescripcionMedio = StringHelper.Capitalize(m.DescripcionMedio));

                ActualizarCondicionesConMedioAccesible();

                _condicionesCache = DatosHelper.ClonarObjeto(_condiciones);
                _condicionesPorMedio = null; // Limpiar cach�

                _medios = [.. _condiciones
                                .Select(x => new CodigoDescripcion
                                {
                                    Codigo = x.CodigoMedio,
                                    Descripcion = x.DescripcionMedio
                                })];

                await ObtenerExcepciones(codigoVigencia);
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

        private async Task ObtenerExcepciones(int codigoVigencia)
        {
            _excepciones = await CondicionesService.ObtenerExcepcionesCondiciones(codigoVigencia);
            _excepciones.ForEach(m => m.DescripcionMedio = StringHelper.Capitalize(m.DescripcionMedio));

            //Actualiza el indicador de Base de calculo de las excepciones a partir del indicador de Base de calculo de la Condicion del mismo Medio
            foreach (CondicionDto condicion in _condiciones)
            {
                int indicadorCalculoDevolucion = condicion.IndicadorCalculoDevolucion;
                _excepciones.Where(c => c.CodigoMedio == condicion.CodigoMedio).ToList().ForEach(c => c.IndicadorCalculoDevolucion = indicadorCalculoDevolucion);
            }

            ActulizarExcepcionesConMedioAccesible();

            _excepcionesCache = DatosHelper.ClonarObjeto(_excepciones);
        }

        private async Task FiltroBuscar()
        {
            if (!await ComprobarSiHayCambiosPendienteAndSeguir())  return;

            if (!ValidarCamposObligatoriosFiltro())
            {
                await MensajesHelper.MostrarMensajeInfo(PageTitle, T(AppResources.Mensajes.CamposObligatorios));
                return;
            }
            try
            {
                LayerOverlayService.Start();
                _filtroCondiciones.CodigoNetwork = _networkSeleccionado!.Codigo;

                var grupoSeleccionado = _gruposClientes.First(c=> c.Codigo == _codigoGrupoSeleccionado!.Value);
                _filtroCondiciones.CodigoGrupoCliente = _codigoGrupoSeleccionado!.Value;
                _filtroCondiciones.Anio = Convert.ToInt32(_anioSeleccionado!.Descripcion);
                _filtroCondiciones.CodigoVersion = _versionSeleccionada!.Codigo;

                if (_radioGroupAcuerdoChecked == T(AppResources.Pages.PlanificacionCondiciones.CondicionesAcuerdos))
                {
                    _filtroCondiciones.IndicadorAcuerdo = Convert.ToInt32(T(AppResources.Pages.PlanificacionCondiciones.CondicionesAcuerdosCodigo));
                }
                else if (_radioGroupAcuerdoChecked == T(AppResources.Pages.PlanificacionCondiciones.Condiciones))
                { 
                    _filtroCondiciones.IndicadorAcuerdo = Convert.ToInt32(T(AppResources.Pages.PlanificacionCondiciones.CondicionesCodigo));
                }

                _vigencias = await CondicionesService.ObtenerVigencias(_filtroCondiciones);
                
                LayerOverlayService.Stop();
                if (_vigencias.Count == 0)
                {
                    _vigenciaSeleccionada = null;
                    if (!_desdePaginaImportarMMS)
                    {
                        await MensajesHelper.MostrarMensajeInfo(PageTitle, T(AppResources.Mensajes.RegistrosNoEncontrados));
                    }
                }
                else
                {
                    await Traducciones.TraducirMesesVigencias(_vigencias);
                    SeleccionarVigencia();
                }

                _tituloGridExcepciones = T(AppResources.Pages.PlanificacionCondiciones.Excepciones);
                _codigoMedioleccionadoParaFiltro = null;

                RightCaptionVigencias = $"[{_networkSeleccionado?.Descripcion ?? ""}, {grupoSeleccionado!.Descripcion}, {Convert.ToInt32(_anioSeleccionado!.Descripcion)}, {_versionSeleccionada!.Descripcion}]";

                ActivarBotonNuevaExcepcion = true;

            }
            catch (Exception ex)
            {
                await LogService.InsertException(ex);
            }
            finally
            {
                LayerOverlayService.Stop();
                _desdePaginaImportarMMS = false;
            }
        }

        private bool ValidarCamposObligatoriosFiltro()
        {
            return _networkSeleccionado != null
                && _codigoGrupoSeleccionado != null
                && _anioSeleccionado != null
                && _versionSeleccionada != null;
        }

        private async Task FiltroLimpiar()
        {
            try
            {
                if (!await ComprobarSiHayCambiosPendienteAndSeguir()) return;

                LayerOverlayService.Start();
                _anioSeleccionado = null;
                _versionSeleccionada = null;
                _codigoGrupoSeleccionado = null;
                _codigoMedioleccionadoParaFiltro = null;

                //Limpiar vigencias. aL poner esta a null, se limpia automaticamente las condiciones y las excepciones
                _vigencias = [];
                _vigenciaSeleccionada = null;

                await FilterInit();
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


        #region Vigencias

        private void SeleccionarVigencia()
        {
            if (_vigencias.Count == 0)
            {
                _vigenciaSeleccionada = null;
                return;
            }

            if (_vigencias.Count == 1)
            {
                _vigenciaSeleccionada = _vigencias[0];
            }
            else
            {
                //Se usa el operador ^ al calcular un �ndice desde el final de una colecci�n.
                _vigenciaSeleccionada = _vigencias[^1];
            }
        }

        private void AñadirVigencia()
        {
            _vigenciaNueva.CodigoNetWork = _filtroCondiciones.CodigoNetwork;
            _vigenciaNueva.CodigoVersion = _filtroCondiciones.CodigoVersion;
            _vigenciaNueva.CodigoGrupoCliente = _filtroCondiciones.CodigoGrupoCliente;
            _vigenciaNueva.IndicadorAcuerdo = _filtroCondiciones.IndicadorAcuerdo;

            _tituloPopupVigencia =  T(AppResources.Pages.PlanificacionCondiciones.NuevaVigencia);

            _modoEdicionVigencia = ModoEdicion.Alta;
            _popupVigenciaVisible = true;
        }

        private async Task EditarVigencia()
        {
            if (_vigenciaSeleccionada == null)
            {
                await MensajesHelper.MostrarMensajeError(PageTitle, T("Pages:PlanificacionCondiciones:VigenciaSeleccion:label"));
                return;
            }

            int codigoVigencia = _vigenciaSeleccionada.Codigo;

            Vigencia? vigencia = _vigencias.Find(c=> c.Codigo == codigoVigencia);

            if (vigencia == null)
            {
                return;
            }

            CodigoDescripcion mesDesde = _meses[vigencia.MesDesde - 1];
            CodigoDescripcion mesHasta = _meses[vigencia.MesHasta - 1];

            _mesDesdeVigenciaSeleccionado = mesDesde;
            _mesHastaVigenciaSeleccionado = mesHasta;

            _tituloPopupVigencia = T(AppResources.Pages.PlanificacionCondiciones.EditarVigencia);

            _modoEdicionVigencia = ModoEdicion.Edicion;
            _popupVigenciaVisible = true;
        }

        private async Task EliminarVigencia()
        {
            bool confirm = await MensajesHelper.MostrarMensajeParaConfirmacion(PageTitle, T(AppResources.Mensajes.ConfirmacionEliminar));
            if (!confirm) return;

            try
            {
                int codigoVigencia = _vigenciaSeleccionada!.Codigo;

                if (await CondicionesService.ExistenCondicionesVigencias(codigoVigencia))
                {
                    confirm = await MensajesHelper.MostrarMensajeParaConfirmacion(PageTitle, T(AppResources.Pages.PlanificacionCondiciones.Mensajes.VigenciaConCondiciones));
                    if (!confirm)  return;
                }
                LayerOverlayService.Start();
                await CondicionesService.EliminarVigencia(_vigenciaSeleccionada);
                _vigencias.Remove(_vigenciaSeleccionada);
                SeleccionarVigencia();
                await MensajesHelper.MostrarMensajeInfo(PageTitle, T(AppResources.Mensajes.RegistroEliminado));
            }
            catch (ExcepcionBaseDatos exBd)
            {
                await TratarExcepcionGeneradaEnBD(exBd, PageTitle);
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


        private async Task GrabarVigencia()
        {
            if (! await ValidarFechasVigencia()) return;

            try
            {
                LayerOverlayService.Start();
                int mesDesde = _mesDesdeVigenciaSeleccionado!.Codigo;
                int mesHasta = _mesHastaVigenciaSeleccionado!.Codigo;

                Vigencia vigencia = new ();

                if (_modoEdicionVigencia == ModoEdicion.Alta)
                {
                    vigencia = DatosHelper.ClonarObjeto(_vigenciaNueva);
                }
                else
                {
                    vigencia = DatosHelper.ClonarObjeto(_vigenciaSeleccionada!);
                }

                vigencia.MesDesde = mesDesde;
                vigencia.MesHasta = mesHasta;

                if (! await CondicionesService.ValidarSolapesVigencia(vigencia))
                {
                    await MensajesHelper.MostrarMensajeError(PageTitle, T(AppResources.Pages.PlanificacionCondiciones.Mensajes.VigenciasSolapadas));
                    return;
                }

                if (_modoEdicionVigencia == ModoEdicion.Alta)
                {

                    await CondicionesService.InsertarVigencia(vigencia );
                    _vigencias.Add(vigencia);
                }
                else
                {
                    await CondicionesService.ActualizarVigencia(vigencia);
                    Vigencia? vigenciamodificada = _vigencias.Find(c => c.Codigo == _vigenciaSeleccionada!.Codigo);
                    if (vigenciamodificada != null)
                    {
                        vigenciamodificada.MesDesde = vigencia.MesDesde;
                        vigenciamodificada.MesHasta = vigencia.MesHasta;
                    }
                }

                await Traducciones.TraducirMesesVigencias(_vigencias);

                _vigenciaSeleccionada = vigencia;

                if (_modoEdicionVigencia == ModoEdicion.Alta)
                {
                    await MensajesHelper.MostrarMensajeInfo(PageTitle, T("Pages:PlanificacionCondiciones:Mensajes:VigenciaA�adida:label"));
                }
                else
                {
                    await MensajesHelper.MostrarMensajeInfo(PageTitle, T(AppResources.Pages.PlanificacionCondiciones.Mensajes.VigenciaModificada));
                }

                _popupVigenciaVisible = false;
                _mesDesdeVigenciaSeleccionado = null;
                _mesHastaVigenciaSeleccionado = null;
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

        private void OcultarPopupVigencia()
        {
            _popupVigenciaVisible = false;
        }

        private async Task<bool> ValidarFechasVigencia()
        {
            if (_mesDesdeVigenciaSeleccionado == null || _mesHastaVigenciaSeleccionado == null)
            {
                await MensajesHelper.MostrarMensajeError(PageTitle, T(AppResources.Pages.PlanificacionCondiciones.Mensajes.VigenciaSinMeses));
                return  false;
            }

            if (_mesDesdeVigenciaSeleccionado.Codigo > _mesHastaVigenciaSeleccionado.Codigo)
            {
                await MensajesHelper.MostrarMensajeError(PageTitle, T(AppResources.Pages.PlanificacionCondiciones.Mensajes.VigenciaValidacion));
                return false;
            }
            return true;
        }

        #endregion


        #region Grid Condiciones

        private async void GridCondiciones_CustomizeElement(GridCustomizeElementEventArgs ea)
        {
            if (ea.ElementType != GridElementType.DataCell) return;

            var column = (IGridDataColumn)ea.Column;
            var condicion = (CondicionDto)GridCondiciones.GetDataItem(ea.VisibleIndex);

            if (condicion == null) return;
          
            var itemCondicionOriginal = _condicionesCache.Find(x => x.Key == condicion.Key);
            if (itemCondicionOriginal == null) return;

            try 
            { 
                //Para utilizar este metodo de comparacion las columnas a comparar deben tener definida la propiedad caption

                // Diccionario: caption ? funci�n que devuelve (original, actual)
                var comparadores = new Dictionary<string, Func<(object? original, object? actual)>>
                {
                    {
                        T(AppResources.Pages.PlanificacionCondiciones.SagTpc),
                        () => (itemCondicionOriginal.PctSAG, condicion.PctSAG)
                    },
                    {
                        T(AppResources.Pages.PlanificacionCondiciones.ManpowerTpc),
                        () => (itemCondicionOriginal.PctManPower, condicion.PctManPower)
                    },
                    {
                        T(AppResources.Pages.PlanificacionCondiciones.BCDevolucion),
                        () => (itemCondicionOriginal.IndicadorCalculoDevolucion, condicion.IndicadorCalculoDevolucion)
                    },
                    {
                        T(AppResources.Pages.PlanificacionCondiciones.DevolucionTpc),
                        () => (itemCondicionOriginal.PctDevolucion, condicion.PctDevolucion)
                    }
                };

                if (comparadores.TryGetValue(column.Caption, out var selector))
                {
                    var (original, actual) = selector();
                    if (!Equals(original, actual))
                    {
                        ea.CssClass = "grid-modified-cell";
                    }
                }
            }
            catch (Exception ex)
            {
                await LogService.InsertException(ex);
                await MensajesHelper.MostrarMensajeError(PageTitle);
            }
        }

        private bool HaCambiadoValorConceptoCondiciones(CondicionDto item, ConceptosCondiciones concepto)
        {
            var medio = _condicionesCache.Find(c => c.CodigoMedio == item.CodigoMedio);
            if (medio == null)
            {
                return true;
            }
            return concepto switch
            {
                ConceptosCondiciones.Sag => item.PctSAG != medio!.PctSAG,
                ConceptosCondiciones.Manpower => item.PctManPower != medio!.PctManPower,
                ConceptosCondiciones.Devolucion => item.PctDevolucion != medio!.PctDevolucion,
                ConceptosCondiciones.BaseCalculoDevolucion => item.IndicadorCalculoDevolucion != medio!.IndicadorCalculoDevolucion,
                _ => false
            };
        }

        private async void Condicion_SetPct(CondicionDto itemEditable, CondicionDto itemDestino, ConceptosCondiciones concepto, decimal? value)
        {
            try 
            { 
                CondicionDto? itemToActualizar = _condiciones.Find(c => c.CodigoMedio == itemEditable.CodigoMedio);

                switch (concepto)
                {
                    case ConceptosCondiciones.Sag:
                        {
                            itemEditable.PctSAG = value;
                            if (itemToActualizar != null)
                            {
                                itemToActualizar.PctSAG = value;
                            }
                            break;
                        }
                        ;
                    case ConceptosCondiciones.Manpower:
                        {
                            itemEditable.PctManPower = value;
                            if (itemToActualizar != null)
                            {
                                itemToActualizar.PctManPower = value;
                            }
                            break;
                        }
                        ;
                    case ConceptosCondiciones.Devolucion:
                        {
                            itemEditable.PctDevolucion = value;
                            if (itemToActualizar != null)
                            {
                                itemToActualizar.PctDevolucion = value;
                            }
                            break;
                        }
                }
                await ControlarCambiosCondiciones(itemEditable, itemDestino);
            }
            catch (Exception ex)
            {
                await LogService.InsertException(ex);
                await MensajesHelper.MostrarMensajeError(PageTitle);
            }
        }

        private async void Condicion_SetDev(CondicionDto itemEditable, CondicionDto itemDestino, int? value)
        {
            try
            {
                int codigoMedio = itemEditable.CodigoMedio;
                int valor = value.HasValue ? value.Value : 0;

                itemEditable.IndicadorCalculoDevolucion = valor;

                _condiciones
                   .Where(c => c.CodigoMedio == codigoMedio)
                   .ToList()
                   .ForEach(c =>
                   {
                       c.IndicadorCalculoDevolucion = valor;
                       if (valor == 0)
                       {
                           c.PctDevolucion = null;
                           itemEditable.PctDevolucion = null;
                       }
                   });

                _excepciones
                    .Where(c => c.CodigoMedio == codigoMedio)
                    .ToList()
                    .ForEach(c =>
                    {
                        c.IndicadorCalculoDevolucion = valor;
                        if (valor == 0)
                        {
                            c.PctDevolucion = null;
                        }
                    });

                await ControlarCambiosCondiciones(itemEditable, itemDestino);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await LogService.InsertException(ex);
                await MensajesHelper.MostrarMensajeError(PageTitle);
            }
        }

        private async Task ControlarCambiosCondiciones(CondicionDto itemEditable, CondicionDto itemDestino)
        {
            var itemOrigen = _condicionesCache.Find(c => c.CodigoMedio == itemEditable.CodigoMedio)!;
            var camposConCambios = DatosHelper.ObtenerCamposModificados(itemEditable, itemOrigen);

            if (camposConCambios.Count > 0)
            {
                DatosHelper.AplicarCambios(itemEditable, itemDestino);
                if (_condicionesNoGuardados.TryGetValue(itemDestino, out var cambios))
                {
                    cambios.CamposCambiados.UnionWith(camposConCambios);
                }
                else
                {
                    _condicionesNoGuardados.Add(itemDestino, new(TiposCambiosdeDatos.Modificados, camposConCambios));
                }
            }
            else
            {
                if (_condicionesNoGuardados.Count > 0)
                {
                    _condicionesNoGuardados.Remove(itemDestino);
                }
            }

            await MarcarCambios(HayCambiosPendientes);
            StateHasChanged();
        }

        #endregion


        #region Grid Excepciones

        private async void GridMediosExcepciones_CustomizeElement(GridCustomizeElementEventArgs ea)
        {
            if (ea.ElementType != GridElementType.DataCell) return;
            try
            {
                var column = (IGridDataColumn)ea.Column;
                var excepcion = (ExcepcionDto)GridExcepciones.GetDataItem(ea.VisibleIndex);

                if (excepcion == null) return;
                var itemOriginal = _excepcionesCache.Find(x => x.Key == excepcion.Key);
                if (itemOriginal == null)
                {
                    ea.CssClass = "grid-modified-cell";
                    return;
                }

                //Para utilizar este metodo de comparacion las columnas a comparar deben tener definida la propiedad caption

                // Diccionario: caption ? funci�n que devuelve (original, actual)
                var comparadores = new Dictionary<string, Func<(object? original, object? actual)>>()
                { 
                    {
                        T(AppResources.Pages.PlanificacionCondiciones.Jerarquia),
                        () => (itemOriginal!.Jerarquia, excepcion.Jerarquia)
                    },
                    {
                        T(AppResources.Pages.PlanificacionCondiciones.SagTpc),
                        () => (itemOriginal!.PctSAG, excepcion.PctSAG)
                    },
                    {
                        T(AppResources.Pages.PlanificacionCondiciones.ManpowerTpc),
                        () => (itemOriginal!.PctManPower, excepcion.PctManPower)
                    },
                    {
                        T(AppResources.Pages.PlanificacionCondiciones.DevolucionTpc),
                        () => (itemOriginal!.PctDevolucion, excepcion.PctDevolucion)
                    },
                    {
                        T(AppResources.Pages.PlanificacionCondiciones.Alcance),
                        () => (itemOriginal!.CodigoAlcance, excepcion.CodigoAlcance)
                    },
                    {
                        T(AppResources.Pages.PlanificacionCondiciones.Disciplina),
                        () => (itemOriginal!.CodigoDisciplina, excepcion.CodigoDisciplina)
                    },
                    {
                        T(AppResources.Pages.PlanificacionCondiciones.Diversified),
                        () => (itemOriginal!.CodigoDiversified, excepcion.CodigoDiversified)
                    },
                    {
                        T(AppResources.Pages.PlanificacionCondiciones.Objetivo),
                        () => (itemOriginal!.CodigoObjetivo, excepcion.CodigoObjetivo)
                    },
                    {
                        T(AppResources.Pages.PlanificacionCondiciones.TipoCompra),
                        () => (itemOriginal!.CodigoTipoCompra, excepcion.CodigoTipoCompra)
                    },
                    {
                        T(AppResources.Pages.PlanificacionCondiciones.TipoDisciplina),
                        () => (itemOriginal!.CodigoTipoDisciplina, excepcion.CodigoTipoDisciplina)
                    },
                    {
                        T(AppResources.Pages.PlanificacionCondiciones.DisciplinaGrupo),
                        () => (itemOriginal!.CodigoDisciplinaGrupo, excepcion.CodigoDisciplinaGrupo)
                    }
                };

                if (comparadores.TryGetValue(column.Caption, out var selector))
                {
                    var (original, actual) = selector();

                    if (!Equals(original, actual))
                    {
                        ea.CssClass = "grid-modified-cell";
                    }
                }
            }
            catch (Exception ex)
            {
                await LogService.InsertException(ex);
                await MensajesHelper.MostrarMensajeError(PageTitle);
            }
        }

        private bool HaCambiadoValorExcepciones(ExcepcionDto item, ConceptosCondicionesNMD concepto)
        {
            var excepcion = _excepcionesCache.Find(c => c.CodigoCondicionMedio == item.CodigoCondicionMedio);

            if (excepcion == null)
            {
                return true;
            }

            return concepto switch
            {
                ConceptosCondicionesNMD.Alcance => item.CodigoAlcance != excepcion!.CodigoAlcance,
                ConceptosCondicionesNMD.Disciplina => item.CodigoDisciplina != excepcion!.CodigoDisciplina,
                ConceptosCondicionesNMD.Diversified => item.CodigoDiversified != excepcion!.CodigoDiversified,
                ConceptosCondicionesNMD.Objetivo => item.CodigoObjetivo != excepcion!.CodigoObjetivo,
                ConceptosCondicionesNMD.TipoCompra => item.CodigoTipoCompra != excepcion!.CodigoTipoCompra,
                ConceptosCondicionesNMD.TipoDisciplina => item.CodigoTipoDisciplina != excepcion!.CodigoTipoDisciplina,
                ConceptosCondicionesNMD.DisciplinaGrupo => item.CodigoDisciplinaGrupo != excepcion!.CodigoDisciplinaGrupo,
                _ => false
            };
        }

        private bool HaCambiadoValorExcepciones(ExcepcionDto item, ConceptosCondiciones concepto)
        {
            var excepcion = _excepcionesCache.Find(c => c.CodigoCondicionMedio == item.CodigoCondicionMedio);
            if (excepcion == null)
            {
                return true;
            }
            return concepto switch
            {
                ConceptosCondiciones.Sag => item.PctSAG != excepcion!.PctSAG,
                ConceptosCondiciones.Manpower => item.PctManPower != excepcion!.PctManPower,
                ConceptosCondiciones.Devolucion => item.PctDevolucion != excepcion!.PctDevolucion,
                _ => false
            };
        }

        private bool HaCambiadoValorJerarquia(ExcepcionDto item)
        {
            var excepcion = _excepcionesCache.Find(c => c.CodigoCondicionMedio == item.CodigoCondicionMedio);
            if (excepcion == null)
            {
                return true;
            }
            return item.Jerarquia != excepcion.Jerarquia;
        }

        private async void ExcepcionMedioCondicion_SetConceptosNMD(ExcepcionDto itemEditable, ExcepcionDto itemDestino, ConceptosCondicionesNMD concepto, int? value)
        {
            try
            { 
                ExcepcionDto? itemToActualizar = _excepciones.Find(c => c.CodigoCondicionMedio == itemEditable.CodigoCondicionMedio);

                int valor = value ?? 0;

                switch (concepto)
                {
                    case ConceptosCondicionesNMD.Alcance:
                        {
                            if (itemToActualizar != null)
                            {
                                itemToActualizar.CodigoAlcance = valor;
                            }
                            itemEditable.CodigoAlcance = valor;
                            break;
                        }
                    case ConceptosCondicionesNMD.Disciplina:
                        {
                            if (itemToActualizar != null)
                            {
                                itemToActualizar.CodigoDisciplina = valor;
                            }
                            itemEditable.CodigoDisciplina = valor;
                            break;
                        }
                    case ConceptosCondicionesNMD.Diversified:
                        {
                            if (itemToActualizar != null)
                            {
                                itemToActualizar.CodigoDiversified = valor;
                            }
                            itemEditable.CodigoDiversified = valor;
                            break;
                        }
                    case ConceptosCondicionesNMD.Objetivo:
                        {
                            if (itemToActualizar != null)
                            {
                                itemToActualizar.CodigoObjetivo = valor;
                            }
                            itemEditable.CodigoObjetivo = valor;
                            break;
                        }
                    case ConceptosCondicionesNMD.TipoCompra:
                        {
                            if (itemToActualizar != null)
                            {
                                itemToActualizar.CodigoTipoCompra = valor;
                            }
                            itemEditable.CodigoTipoCompra = valor;
                            break;
                        }
                    case ConceptosCondicionesNMD.TipoDisciplina:
                        {
                            if (itemToActualizar != null)
                            {
                                itemToActualizar.CodigoTipoDisciplina = valor;
                            }
                            itemEditable.CodigoTipoDisciplina = valor;
                            break;
                        }
                    case ConceptosCondicionesNMD.DisciplinaGrupo:
                        {
                            if (itemToActualizar != null)
                            {
                                itemToActualizar.CodigoDisciplinaGrupo = valor;
                            }
                            itemEditable.CodigoDisciplinaGrupo = valor;
                            break;
                        }
                }
                await ControlarCambiosExcepciones(itemEditable, itemDestino);
            }
            catch (Exception ex)
            {
                await LogService.InsertException(ex);
                await MensajesHelper.MostrarMensajeError(PageTitle);
            }
        }

        private async void MedioCondicionExcepcion_SetPct(ExcepcionDto itemEditable, ExcepcionDto itemDestino, ConceptosCondiciones concepto, decimal? value)
        {
            try
            {
                ExcepcionDto? itemToActualizar = _excepciones.Find(c => c.CodigoCondicionMedio == itemEditable.CodigoCondicionMedio);
                switch (concepto)
                {
                    case ConceptosCondiciones.Sag:
                        {
                            itemEditable.PctSAG = value;

                            if (itemToActualizar != null)
                            {
                                itemToActualizar.PctSAG = value;
                            }
                            break;
                        }
                        ;
                    case ConceptosCondiciones.Manpower:
                        {
                            itemEditable.PctManPower = value;

                            if (itemToActualizar != null)
                            {
                                itemToActualizar.PctManPower = value;
                            }
                            break;
                        }
                        ;
                    case ConceptosCondiciones.Devolucion:
                        {
                            itemEditable.PctDevolucion = value;
                            if (itemToActualizar != null)
                            {
                                itemToActualizar.PctDevolucion = value;
                            }
                            break;
                        }
                }
                await ControlarCambiosExcepciones(itemEditable, itemDestino);
            }
            catch (Exception ex)
            {
                await LogService.InsertException(ex);
                await MensajesHelper.MostrarMensajeError(PageTitle);
            }
        }

        private async Task ControlarCambiosExcepciones(ExcepcionDto itemEditable, ExcepcionDto itemDestino, bool aplicarCambios = true)
        {
            var itemOrigen = _excepcionesCache.Find(c => c.CodigoCondicionMedio == itemEditable.CodigoCondicionMedio)!;

            var camposConCambios = DatosHelper.ObtenerCamposModificados(itemEditable, itemOrigen);

            if (camposConCambios.Count > 0)
            {
                if (aplicarCambios)
                {
                    DatosHelper.AplicarCambios(itemEditable, itemDestino);
                }
                if (ExcepcionesNoGuardadas.TryGetValue(itemDestino, out var cambios))
                {
                    cambios.CamposCambiados.UnionWith(camposConCambios);
                }
                else
                {
                    ExcepcionesNoGuardadas.Add(itemDestino, new(TiposCambiosdeDatos.Modificados, camposConCambios));
                }
            }
            else
            {
                if (ExcepcionesNoGuardadas.Count != 0)
                {
                    ExcepcionesNoGuardadas.Remove(itemDestino);
                }
            }
            await MarcarCambios(HayCambiosPendientes);
            StateHasChanged();
        }

        private async void AñadirExcepcion()
        {
            if (_codigoMedioleccionadoParaFiltro.HasValue)
            {
                try
                {
                    LayerOverlayService.Start();
                    int codigoMedio = _codigoMedioleccionadoParaFiltro!.Value;

                    var nuevaExcepcion = new ExcepcionDto();
                    nuevaExcepcion.MedioAccesible = true;

                    string codigoCondicion = ObtenerNuevoCodigoExcepcion();

                    nuevaExcepcion.CodigoCondicionMedio = codigoCondicion;
                    nuevaExcepcion.CodigoMedio = codigoMedio;

                    string descripcionMedio = _medios.Find(c => c.Codigo == codigoMedio)!.Descripcion;

                    nuevaExcepcion.DescripcionMedio = descripcionMedio;

                    int jerarquia = 1;
                    var excepcionesPorMedio = _excepciones.Where(c => c.CodigoMedio == codigoMedio);

                    if (excepcionesPorMedio.Any())
                    {
                        jerarquia = excepcionesPorMedio.Max(c => c.Jerarquia) + 1;
                    }
                    nuevaExcepcion.Jerarquia = jerarquia;
                    nuevaExcepcion.CodigoAlcance = 0;
                    nuevaExcepcion.CodigoDisciplina = 0;
                    nuevaExcepcion.CodigoDiversified = 0;
                    nuevaExcepcion.CodigoObjetivo = 0;
                    nuevaExcepcion.CodigoTipoCompra = 0;
                    nuevaExcepcion.CodigoTipoDisciplina = 0;
                    nuevaExcepcion.CodigoDisciplinaGrupo = 0;

                    _excepciones.Insert(jerarquia - 1, nuevaExcepcion);

                    ExcepcionesNoGuardadas[nuevaExcepcion] = new(TiposCambiosdeDatos.Añadidos, []);

                    GridExcepciones.Reload();
                    await MarcarCambios(true);
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
                _popupMediosVisible = true;
            }
        }

        private string ObtenerNuevoCodigoExcepcion()
        {
            //Se busca en las excepciones aquella cuyo condigo empiza por -, que son la que ha a�adido el usuario y aun no se han grabado
            var codigosNuevos = _excepciones
                .Select(x => x.CodigoCondicionMedio)
                .Where(c => c.StartsWith("-"))
                .Select(c => int.Parse(c))
                .ToList();

            int nuevoCodigoNumerico;

            // Si ya hay c�digos nuevos, toma el menor y r�stale 1
            if (codigosNuevos.Count > 0)
            {
                nuevoCodigoNumerico = codigosNuevos.Min() - 1;
            }
            else
            {
                nuevoCodigoNumerico = -1;
            }

            return nuevoCodigoNumerico.ToString();
        }

        private async Task MoverJerarquia(ExcepcionDto item, AccionJerarquias accion)
        {
            try
            {
                int jerarquia = item.Jerarquia;
                int codigoMedio = item.CodigoMedio;

                int nuevoValorJerarquia = accion == AccionJerarquias.Subir ? jerarquia - 1 : jerarquia + 1;

                var excepcion = _excepciones.Find(c => c.CodigoMedio == codigoMedio && c.Jerarquia == nuevoValorJerarquia);
                if (excepcion != null)
                {
                    excepcion.Jerarquia = jerarquia;
                    item.Jerarquia = nuevoValorJerarquia;

                    OrdenarExcepciones();

                    //Para indicar que se han cambiado las dos filas
                    await ControlarCambiosExcepciones(item, excepcion, false);
                    await ControlarCambiosExcepciones(excepcion, item, false);

                    GridExcepciones.Reload();
                    StateHasChanged();
                }
             }
            catch (Exception ex)
            {
                await LogService.InsertException(ex);
                await MensajesHelper.MostrarMensajeError(PageTitle);
            }
        }

        private void OrdenarExcepciones()
        {
            _excepciones.Sort((a, b) =>
            {
                int result = a.CodigoMedio.CompareTo(b.CodigoMedio);
                if (result == 0)
                {
                    result = a.Jerarquia.CompareTo(b.Jerarquia);
                }
                return result;
            });
        }

        private async Task EliminarExcepcion(object dataItem)
        {
            try
            {
                LayerOverlayService.Start();
                bool confirmar = await MensajesHelper.MostrarMensajeParaConfirmacion(PageTitle, T(AppResources.Mensajes.ConfirmacionEliminar));
                if (!confirmar) return;

                var item = (ExcepcionDto)dataItem;

                //Si el codigo empieza por -, es que es excepcion nueva y solo lo eliminamos de la lista
                if (item.CodigoCondicionMedio.StartsWith('-'))
                {
                    _excepciones.Remove(item);
                    GridCondiciones.Reload();
                }
                else
                {
                    await CondicionesService.EliminarExcepcionCondicion(item.CodigosConceptosCondiciones, item.Jerarquia, _vigenciaSeleccionada!.Codigo, item.CodigoMedio, Usuario!.CodigoUsuario);

                    //Obtenemos de nuevo las excepciones ya que se ha eliminado y actualizado jerarquias
                    await ObtenerExcepciones(_vigenciaSeleccionada!.Codigo);

                    if (_codigoMedioleccionadoParaFiltro != null)
                    {
                        CondicionDto? medio = _condiciones.Find(c => c.CodigoMedio == _codigoMedioleccionadoParaFiltro.Value);
                        if (medio != null)
                        {
                            await FiltrarExcepcionesPorMedio(medio);
                        }
                    }

                    await MensajesHelper.MostrarMensajeExito(PageTitle, T(AppResources.Mensajes.RegistroEliminado));
                }
                ExcepcionesNoGuardadas.Remove(item);
            }
            catch (Exception ex)
            {
                await LogService.InsertException(ex);
                await MensajesHelper.MostrarMensajeError(PageTitle, T(AppResources.Mensajes.ErrorDelete));
            }
            finally
            {
                LayerOverlayService.Stop();
            }
        }


        #region Filtro excepciones por medio

        private async Task FiltrarExcepcionesPorMedio(CondicionDto item)
        {
            if (_codigoMedioleccionadoParaFiltro == item.CodigoMedio)
            {
                //Hay que a�adir las excepciones que se han quitado al hacer el filtro por medio
                foreach (ExcepcionDto excepcion in _excepcionesCache)
                {
                    ExcepcionDto? exc = _excepciones.Find(c => c.CodigoCondicionMedio == excepcion.CodigoCondicionMedio);
                    if (exc == null)
                    {
                        _excepciones.Add(excepcion);
                    }
                }
                OrdenarExcepciones();
                _tituloGridExcepciones = T(AppResources.Pages.PlanificacionCondiciones.Excepciones);
                _codigoMedioleccionadoParaFiltro = null;
                _medioSeleccionadoDesdePopup = null;
            }
            else
            {
                if (ExcepcionesNoGuardadas.Count != 0)
                {
                    
                    await MensajesHelper.MostrarMensajeInfo(PageTitle, T(AppResources.Pages.PlanificacionCondiciones.Mensajes.AlFiltrarExcepcionesConCambios));
                    return;
                }
                _codigoMedioleccionadoParaFiltro = item.CodigoMedio;
                _excepciones = DatosHelper.ClonarObjeto(_excepcionesCache.Where(x => x.CodigoMedio == _codigoMedioleccionadoParaFiltro).ToList());
                _tituloGridExcepciones = $"{T(AppResources.Pages.PlanificacionCondiciones.ExcepcionesMedio)} {item.DescripcionMedio}";
            }
            ActivarBotonNuevaExcepcion = item.MedioAccesible;

            StateHasChanged();
        }

       
        #endregion


        #endregion


        #region Validar y grabar Datos

        private async Task<bool> ValidarCondiciones()
        {
            bool resultado = true;

            foreach (var datoNoGuardado in _condicionesNoGuardados)
            {
                var itemModificado = datoNoGuardado.Key;

                if (itemModificado.PctDevolucion.HasValue)
                {
                    if (itemModificado.IndicadorCalculoDevolucion < 1)
                    {
                        await MensajesHelper.MostrarMensajeError(PageTitle, T(AppResources.Pages.PlanificacionCondiciones.Mensajes.PctDevCondicionSinBC));
                        resultado = false;
                    }
                }

                if (itemModificado.IndicadorCalculoDevolucion > 0)
                {
                    if (!itemModificado.PctDevolucion.HasValue)
                    {
                        await MensajesHelper.MostrarMensajeError(PageTitle, T(AppResources.Pages.PlanificacionCondiciones.Mensajes.BCCondicionSinPctDevolucion));
                        resultado = false;
                    }
                }


            }
            return resultado;
        }

        private async Task<bool> ValidarExcepciones()
        {
            if (CondicionesCacheTodasSinDatos() && _condicionesNoGuardados.Count == 0)
            {
                await MensajesHelper.MostrarMensajeError(PageTitle, T(AppResources.Pages.PlanificacionCondiciones.Mensajes.ExcepcionesSinCondiciones));
                return false;
            }

            // Validaci�n de duplicados optimizada con HashSet
            var excepcionesVistas = new HashSet<string>();
            ExcepcionDto? primerDuplicado = null;

            foreach (var excepcion in _excepciones)
            {
                // Crear clave �nica basada en los valores que identifican duplicados
                var clave = $"{excepcion.CodigoMedio}|{excepcion.PctManPower}|{excepcion.PctSAG}|{excepcion.PctDevolucion}|" +
                            $"{excepcion.CodigoAlcance}|{excepcion.CodigoDisciplina}|{excepcion.CodigoDiversified}|" +
                            $"{excepcion.CodigoObjetivo}|{excepcion.CodigoTipoCompra}|{excepcion.CodigoTipoDisciplina}|" +
                            $"{excepcion.CodigoDisciplinaGrupo}";

                if (!excepcionesVistas.Add(clave))
                {
                    primerDuplicado = excepcion;
                    break; // Sale inmediatamente al encontrar el primer duplicado
                }
            }

            if (primerDuplicado != null)
            {
                string mensaje = T(AppResources.Pages.PlanificacionCondiciones.Mensajes.MedioExcepcionConDatosDuplicados);
                await MensajesHelper.MostrarMensajeError(PageTitle, string.Format(mensaje, primerDuplicado.DescripcionMedio));
                return false;
            }

            foreach (var datoNoGuardado in ExcepcionesNoGuardadas)
            {
                var itemModificado = datoNoGuardado.Key;

                if (itemModificado.PctSAG == null && itemModificado.PctManPower == null && itemModificado.PctDevolucion == null)
                {
                    await MensajesHelper.MostrarMensajeError(PageTitle, T(AppResources.Pages.PlanificacionCondiciones.Mensajes.PctEnTodasExcepciones));
                    return false;
                }
                if (itemModificado.CodigoAlcance == 0 && itemModificado.CodigoDisciplina == 0 && itemModificado.CodigoDiversified == 0 
                    && itemModificado.CodigoObjetivo == 0 && itemModificado.CodigoTipoCompra == 0
                    && itemModificado.CodigoTipoDisciplina == 0 && itemModificado.CodigoDisciplinaGrupo == 0)
                {
                    await MensajesHelper.MostrarMensajeError(PageTitle, T(AppResources.Pages.PlanificacionCondiciones.Mensajes.ConceptoNMDEnTodasExcepciones));
                    return false;
                }

                //Buscamos por si hay condiciones cambiadas por medio, por si se hubieran dejado a null los porcentajes
                int codigoMedio = itemModificado.CodigoMedio;
                var existeCondicion = _condicionesNoGuardados.Keys.Any(c => c.CodigoMedio == codigoMedio);

                //Si no hay, tenemos que ver si hay datos en las condiciones originales para el medio
                if (!existeCondicion)
                {
                    bool todasNull = _condicionesCache
                        .Where(obj => obj.CodigoMedio == codigoMedio)
                        .All(obj =>
                                obj.PctSAG == null &&
                                obj.PctManPower == null &&
                                obj.PctDevolucion == null
                        );
                    if (todasNull)
                    {
                        await MensajesHelper.MostrarMensajeError(PageTitle, T(AppResources.Pages.PlanificacionCondiciones.Mensajes.ExcepcionesConCondicionesEnMedio));
                        return false;
                    }
                }

                if (itemModificado.PctDevolucion != null)
                {
                    if (itemModificado.IndicadorCalculoDevolucion < 1)
                    {
                        await MensajesHelper.MostrarMensajeError(PageTitle, T(AppResources.Pages.PlanificacionCondiciones.Mensajes.PctDevExcepcionSinBD));
                        return false;
                    }
                }

                // Validaciones de porcentajes optimizadas con cach�
                if (!ValidarPorcentajeExcepcion(itemModificado.PctSAG, codigoMedio, c => c.PctSAG))
                {
                    await MensajesHelper.MostrarMensajeError(PageTitle, T(AppResources.Pages.PlanificacionCondiciones.Mensajes.PctExcepcionSinPctSAGCondicion));
                    return false;
                }

                if (!ValidarPorcentajeExcepcion(itemModificado.PctManPower, codigoMedio, c => c.PctManPower))
                {
                    await MensajesHelper.MostrarMensajeError(PageTitle, T(AppResources.Pages.PlanificacionCondiciones.Mensajes.PctExcepcionSinPctManPowerCondicion));
                    return false;
                }

                if (!ValidarPorcentajeExcepcion(itemModificado.PctDevolucion, codigoMedio, c => c.PctDevolucion))
                {
                    await MensajesHelper.MostrarMensajeError(PageTitle, T(AppResources.Pages.PlanificacionCondiciones.Mensajes.PctExcepcionSinPctDEVCondicion));
                    return false;
                }
            }
            return true;
        }

        private bool CondicionesCacheTodasSinDatos()
        {
            bool todasCero = _condicionesCache.All(obj =>
                obj.PctSAG == null &&
                obj.PctManPower == null &&
                obj.PctDevolucion == null
            );
            return todasCero;
        }

        /// <summary>
        /// Obtiene las condiciones agrupadas por medio usando cach� para optimizar b�squedas
        /// </summary>
        private Dictionary<int, List<CondicionDto>> ObtenerCondicionesPorMedio()
        {
            if (_condicionesPorMedio == null)
            {
                _condicionesPorMedio = _condiciones
                    .GroupBy(c => c.CodigoMedio)
                    .ToDictionary(g => g.Key, g => g.ToList());
            }
            return _condicionesPorMedio;
        }

        /// <summary>
        /// Valida si existe una condici�n con el porcentaje especificado para un medio (O(1) con cach�)
        /// </summary>
        private bool ValidarPorcentajeExcepcion(decimal? porcentaje, int codigoMedio, 
    Func<CondicionDto, decimal?> selector)
        {
            if (!porcentaje.HasValue) return true;

            var condicionesPorMedio = ObtenerCondicionesPorMedio();
            if (!condicionesPorMedio.TryGetValue(codigoMedio, out var condiciones))
                return false;

            return condiciones.Any(c => selector(c).HasValue);
        }

        private async Task Grabar()
        {
            try
            {
                if (_condicionesNoGuardados.Count == 0 && ExcepcionesNoGuardadas.Count == 0) 
                {
                    await MensajesHelper.MostrarMensajeInfo(PageTitle, T(AppResources.Mensajes.SinModificaciones));
                    return;
                }

                if (_condicionesNoGuardados.Count > 0)
                {
                    bool validarCondiciones = await ValidarCondiciones();
                    if (!validarCondiciones)
                    {
                        return;
                    }
                }

                if (ExcepcionesNoGuardadas.Count > 0)
                {
                    bool validarExcepciones = await ValidarExcepciones();
                    if (!validarExcepciones)
                    {
                        return;
                    }
                }
                LayerOverlayService.Start();
                await CondicionesService.GrabarCondicionesExcepciones(_condicionesNoGuardados, ExcepcionesNoGuardadas, _vigenciaSeleccionada!.Codigo);
               
                await ObtenerCondicionesExcepciones(_vigenciaSeleccionada!.Codigo);
                await MensajesHelper.MostrarMensajeInfo(PageTitle, T(AppResources.Common.DatosGrabados));
                await LimpiarCambiosPendientes();
            }
            catch (Exception ex)
            {
                await LogService.InsertException(ex);
                await MensajesHelper.MostrarMensajeError(PageTitle, T(AppResources.Mensajes.ErrorAlGrabar));
            }
            finally
            {
                LayerOverlayService.Stop();
            }
        }

        #endregion


        #region Popup Añadir Excepcion

        private async Task AñadirExcepcionDesdePopup()
        {
            try
            {
                LayerOverlayService.Start();
                int codigoMedio = _medioSeleccionadoDesdePopup!.Codigo;
                string codigoCondicion = ObtenerNuevoCodigoExcepcion();

                var nuevaExcepcion = new ExcepcionDto();
                nuevaExcepcion.CodigoCondicionMedio = codigoCondicion;
                nuevaExcepcion.CodigoMedio = codigoMedio;
                nuevaExcepcion.DescripcionMedio = _medioSeleccionadoDesdePopup.Descripcion;
                nuevaExcepcion.MedioAccesible = true;

                int jerarquia = 1;
                var excepcionesPorMedio = _excepciones.Where(c => c.CodigoMedio == codigoMedio);

                if (excepcionesPorMedio.Any())
                {
                    jerarquia = excepcionesPorMedio.Max(c => c.Jerarquia) + 1;
                }
                nuevaExcepcion.Jerarquia = jerarquia;
                nuevaExcepcion.CodigoAlcance = 0;
                nuevaExcepcion.CodigoDisciplina = 0;
                nuevaExcepcion.CodigoDiversified = 0;
                nuevaExcepcion.CodigoObjetivo = 0;
                nuevaExcepcion.CodigoTipoCompra = 0;
                nuevaExcepcion.CodigoTipoDisciplina = 0;
                nuevaExcepcion.CodigoDisciplinaGrupo = 0;

                //Buscar el indicador de devolucion
                int indicadorCalculoDevolucion = _condiciones.Find(c => c.CodigoMedio == codigoMedio)!.IndicadorCalculoDevolucion;
                nuevaExcepcion.IndicadorCalculoDevolucion = indicadorCalculoDevolucion;

                int indice = _excepciones.IndexOf(_excepciones.Find(c => c.CodigoMedio == codigoMedio && c.Jerarquia == jerarquia - 1)!);

                _excepciones.Insert(indice + 1, nuevaExcepcion);

                OrdenarExcepciones();

                ExcepcionesNoGuardadas[nuevaExcepcion] = new(TiposCambiosdeDatos.Añadidos, []);

                GridExcepciones.Reload(); //Porque si no desplaza la ultima fila y no se ve
                await MarcarCambios(true);


                _medioSeleccionadoDesdePopup = null;
                _popupMediosVisible = false;
            }
            catch (Exception ex)
            {
                await LogService.InsertException(ex);
                await MensajesHelper.MostrarMensajeError(PageTitle, T(AppResources.Mensajes.ErrorAlGrabar));
            }
            finally
            {
                LayerOverlayService.Stop();
            }
        }

        private void OcultarPopupMedios()
        {
            _popupMediosVisible = false;
        }

        #endregion


        #region Limpiar y cancelar cambios

        private async Task CancelarCambiosCondiciones()
        {
            bool confirmar = await MensajesHelper.MostrarMensajeParaConfirmacion(PageTitle, T(AppResources.Pages.PlanificacionCondiciones.Mensajes.CancelarCambiosCondiciones));
            if (confirmar)
            {
                LimpiarCambiosCondicionesPendientes();
            }
        }

        private async Task CancelarCambiosExcepciones()
        {
            bool confirmar = await MensajesHelper.MostrarMensajeParaConfirmacion(PageTitle, T(AppResources.Pages.PlanificacionCondiciones.Mensajes.CancelarCambiosExcepciones));
            if (confirmar)
            {
                await LimpiarCambiosExcepcionesPendientes();
            }
        }

        private async Task<bool> ComprobarSiHayCambiosPendienteAndSeguir()
        {
            if (HayCambiosPendientes)
            {
                bool continuar = await MensajesHelper.MostrarMensajeParaConfirmacion(PageTitle, T(AppResources.Mensajes.AvisoAntesCancelar));
                if (continuar)
                {
                    await LimpiarCambiosPendientes();
                }
                return continuar;

            }
            return true;
        }

        private async Task LimpiarCambiosPendientes()
        {
            LimpiarCambiosCondicionesPendientes();
            await LimpiarCambiosExcepcionesPendientes();
        }

        private void LimpiarCambiosCondicionesPendientes()
        {
            _condiciones = DatosHelper.ClonarObjeto(_condicionesCache);
            _condicionesNoGuardados.Clear();

            if (ExcepcionesNoGuardadas.Count == 0)
            {
                LimpiarCambios();
            }
        }

        private async Task LimpiarCambiosExcepcionesPendientes()
        {
            _excepciones = DatosHelper.ClonarObjeto(_excepcionesCache);
            ExcepcionesNoGuardadas.Clear();

            if (_codigoMedioleccionadoParaFiltro != null)
            {
                CondicionDto? condicion = new CondicionDto();
                condicion = _condiciones.Find(c => c.CodigoMedio == _codigoMedioleccionadoParaFiltro.Value);
                if (condicion != null)
                {
                    await FiltrarExcepcionesPorMedio(condicion);
                }
            }

            if (_condicionesNoGuardados.Count == 0)
            {
                LimpiarCambios();
            }
        }

        private async Task CancelarCambios()
        {
            if (await MensajesHelper.MostrarMensajeParaConfirmacion(PageTitle, T(AppResources.Mensajes.Cancelar)))
            {
                await LimpiarCambiosPendientes();
            }
        }

        #endregion


        #region Varios

        private static void InsertarFilaVacia(List<CodigoDescripcion> lista)
        {
            CodigoDescripcion item = new CodigoDescripcion { Codigo = 0, Descripcion = "" }; //El codigo 0 es el que tienen cuando vienen a null
            lista.Insert(0, item);
        }


        private async Task CargarConceptosNMD(ExcepcionDto fila, ConceptosCondicionesNMD concepto)
        {
            try
            {
                // Verificar si ya est�n cargadas las opciones para esta fila
                var yaEstanCargadas = concepto switch
                {
                    ConceptosCondicionesNMD.Disciplina => fila.DisciplinasDisponibles != null,
                    ConceptosCondicionesNMD.Objetivo => fila.ObjetivosDisponibles != null,
                    ConceptosCondicionesNMD.TipoCompra => fila.TiposCompraDisponibles != null,
                    ConceptosCondicionesNMD.TipoDisciplina => fila.TiposDisciplinaDisponibles != null,
                    ConceptosCondicionesNMD.DisciplinaGrupo => fila.DisciplinasGrupoDisponibles != null,
                    _ => false
                };

                if (yaEstanCargadas) return; // Ya est�n cargadas, no hacer nada

                var datos = await ObtenerConceptosNMD(fila, concepto);
                AsignarConceptosAFila(fila, concepto, datos);
                await InvokeAsync(StateHasChanged);
            }
            catch { }
        }


        private void AsignarConceptosAFila(ExcepcionDto fila, ConceptosCondicionesNMD concepto, List<CodigoDescripcion> datos)
        {
            var listaFinal = new List<CodigoDescripcion>
            {
                new() { Codigo = 0, Descripcion = string.Empty }
            };

            if (datos != null && datos.Count > 0)
            {
                listaFinal.AddRange(datos);
            }

            switch (concepto)
            {
                case ConceptosCondicionesNMD.Disciplina:
                    fila.DisciplinasDisponibles = listaFinal;
                    break;
                case ConceptosCondicionesNMD.Objetivo:
                    fila.ObjetivosDisponibles = listaFinal;
                    break;
                case ConceptosCondicionesNMD.TipoCompra:
                    fila.TiposCompraDisponibles = listaFinal;
                    break;
                case ConceptosCondicionesNMD.TipoDisciplina:
                    fila.TiposDisciplinaDisponibles = listaFinal;
                    break;
                case ConceptosCondicionesNMD.DisciplinaGrupo:
                    fila.DisciplinasGrupoDisponibles = listaFinal;
                    break;
            }
        }
        private async Task<List<CodigoDescripcion>> ObtenerConceptosNMD(ExcepcionDto excepcion, ConceptosCondicionesNMD concepto)
        {
            try
            {
                ValoresConceptosNMD valores = new();

                valores.CodigoDisciplina = excepcion.CodigoDisciplina.NullSiCero();
                valores.CodigoDiversified = excepcion.CodigoDiversified.NullSiCero();
                valores.CodigoObjetivo = excepcion.CodigoObjetivo.NullSiCero();
                valores.CodigoDisciplinaGrupo = excepcion.CodigoDisciplinaGrupo.NullSiCero();    
                valores.CodigoTipoCompra = excepcion.CodigoTipoCompra.NullSiCero();
                valores.CodigoTipoDisciplina = excepcion.CodigoTipoDisciplina.NullSiCero();  

                var datos = await PresupuestosService.ObtenerConceptosNMD(excepcion.CodigoMedio, concepto, valores);
                return datos;
            }
            catch (Exception ex)
            {
                await LogService.InsertException(ex);
                await MensajesHelper.MostrarMensajeError(PageTitle, T(AppResources.Mensajes.ErrorAlGrabar));
                return new List<CodigoDescripcion>();
            }
        }


      
        #endregion


        /// <summary>
        /// Metodo para comprobar si venimos desde la pagina de importacion de condiciones de MMS
        /// </summary>
        /// <returns></returns>
        private async Task ManageRequest()
        {
            try
            {
                var datos = NavegacionService.GetDatos<dynamic>();
                if (datos == null) return;

                NavegacionService.Limpiar();
                if (datos is CondicionImportarFiltro filtro)
                {
                    if (filtro.CodigosNetwork.Length == 1)
                    {
                        _networkSeleccionado = _networks.FirstOrDefault(n => n.Codigo == filtro.CodigosNetwork[0]);

                        if (_networkSeleccionado != null)
                        {
                            _gruposClientes = await PresupuestosService.ObtenerGruposClientePorNetwork(_networkSeleccionado.Codigo);

                            _mediosMaster = await PresupuestosService.ObtenerMediosPorNetWork(_networkSeleccionado.Codigo.ToString());

                            if (filtro.CodigosGrupoCliente.Length == 1)
                            {
                                _codigoGrupoSeleccionado = _gruposClientes.FirstOrDefault(n => n.Codigo == filtro.CodigosGrupoCliente[0])?.Codigo;
                            }
                            else if (_gruposClientes.Count == 1)
                            {
                                _codigoGrupoSeleccionado = _gruposClientes[0].Codigo;
                            }
                        }
                    }
                    
                    _anioSeleccionado = _anios.FirstOrDefault(n => n.Codigo == filtro.Anio);
                    if (_anioSeleccionado != null)
                    {
                        _versiones = await ObtenerVersionesPorPermisos(_anioSeleccionado.Codigo);
                    }
                    _versionSeleccionada = _versiones.FirstOrDefault(n => n.Codigo == filtro.CodigoVersion);

                    if (!String.IsNullOrEmpty(_radioGroupAcuerdoChecked) && _anioSeleccionado != null && _networkSeleccionado != null && _versionSeleccionada != null && _codigoGrupoSeleccionado != null)
                    {
                        _desdePaginaImportarMMS = true;
                        await FiltroBuscar();
                    }
                }
                else
                {
                    await FilterInit();
                }
            }
            catch (Exception ex)
            {
                await LogService.InsertException(ex);
                await MensajesHelper.MostrarMensajeError(PageTitle, T(AppResources.Common.Messages.UndefinedError));
            }
        }
    }
}


