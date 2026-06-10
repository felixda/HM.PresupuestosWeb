

using HM.Presupuestos.Application.CasosDeUso.Compartido;

namespace HM.Presupuestos.Web.Pages.GestionSobreprimas
{
    /// <summary>
    /// Página de gestión de sobreprimas comerciales
    /// Gestiona los tres conceptos de sobreprimas: Default, SLA y HVP
    /// </summary>
    public partial class Sobreprimas : ContextProtegido
    {
        #region Inyección de Dependencias

        [Inject] protected ISobreprimasService SobreprimasService { get; set; } = default!;
        [Inject] protected IMaestrosService PresupuestosService { get; set; } = default!;
        [Inject] protected IVersionesService VersionesService { get; set; } = default!;
        [Inject] protected MensajesHelper MensajesHelper { get; set; } = default!;
        [Inject] protected DialogoErrores ErrorService { get; set; } = default!;
        [Inject] protected ILayerOverlayService LayerOverlayService { get; set; } = default!;
        [Inject] protected ParametrosNavegacion NavegacionService { get; set; } = default!;

        #endregion

        #region Propiedades Privadas

        #region Página

        private string CaptionIzquierda { get; set; } = string.Empty;
        private string CaptionDerecha { get; set; } = string.Empty;

       

       // protected override CodigosMenu CodigoMenuPermiso => CodigosMenu.Sobreprimas;

       // protected override string ObtenerTituloPagina() =>
       //     ObtenerTexto(TextosApp.Menu.ObtenerEtiqueta((int)CodigosMenu.Sobreprimas));

        #endregion

        #region Filtro

        private bool _desdePaginaImportarMMS = false;
        private SobreprimaFiltro _filtroSobreprima = new();

        private List<CodigoDescripcion> AñosMaestros { get; set; } = [];
        private CodigoDescripcion? AñoSeleccionado { get; set; }
        private List<VersionResumen> VersionesMaestras { get; set; } = [];
        private VersionResumen? VersionSeleccionada { get; set; }
        private List<CodigoDescripcion> NetworksMaestros { get; set; } = [];
        private List<CodigoDescripcion> MediosFiltrados { get; set; } = [];
        private List<CodigoDescripcion> AgrupacionesComercialesMaestras { get; set; } = [];
        private List<CodigoDescripcion> EditorialesMaestras { get; set; } = [];
        private string TextoBusquedaAgrupacionesComerciales { get; set; } = string.Empty;
        private string TextoBusquedaEditoriales { get; set; } = string.Empty;

        // Backing fields para DxDropDownBox (object? requerido)
        private object? _networksSeleccionados;
        private object? _mediosSeleccionados;
        private object? _agrupacionesComercialesSeleccionadas;


        private object? NetworksSeleccionados
        {
            get => _networksSeleccionados;
            set
            {
                if (_networksSeleccionados != value)
                {
                    _networksSeleccionados = value;
                    var codigosNetwork = ObtenerValoresSeleccionados<CodigoDescripcion, int>(value, x => x.Codigo);

                    if (string.IsNullOrEmpty(codigosNetwork))
                    {
                        _ = ActualizarMediosCuandoQuitamosNetworks();
                    }
                    else
                    {
                        _ = ActualizarMediosCuandoModificamosNetworks(codigosNetwork);
                    }
                }
            }
        }

        private object? MediosSeleccionados
        {
            get => _mediosSeleccionados;
            set
            {
                if (_mediosSeleccionados != value)
                {
                    _mediosSeleccionados = value;
                }

                if (value == null)
                {
                    _ = ActualizarEditorialesCuandoQuitamosMedios();
                }
            }
        }

        private object? AgrupacionesComercialesSeleccionadas
        {
            get => _agrupacionesComercialesSeleccionadas;
            set
            {
                if (_agrupacionesComercialesSeleccionadas != value)
                {
                    _agrupacionesComercialesSeleccionadas = value;
                }

                if (value == null)
                {
                    ActualizarEditorialesCuandoQuitamosAgrupaciones();
                }
            }
        }

        private object? EditorialesSeleccionadas { get; set; }

        // Listas internas del popup 
        private List<CodigoDescripcion> _mediosMaestros = [];
        private List<CodigoDescripcion> _mediosPopup = [];
        private List<CodigoDescripcion> _agrupacionesComercialesPopup = [];
        private List<CodigoDescripcion> _editorialesPopup = [];

        #endregion

        #region Grid Sobreprimas

        private DxGrid GridSobreprimas { get; set; } = new DxGrid();
        private List<SobreprimaGridModel> SobreprimasGrid { get; set; } = [];

        private bool _popupEdicionVisible = false;
        private SobreprimaGridModel _sobreprimaEnEdicion = new();
        private SobreprimaGridModel _sobreprimaOriginal = new();
        private DxPopup? _popupSobreprimas;
        private string _tituloPopupEdicion = string.Empty;

        #endregion

        #endregion

        #region Ciclo de Vida del Componente

        /// <summary>
        /// Se ejecuta cuando el usuario no tiene permisos para acceder a la página
        /// </summary>
        protected override Task OnPermisoDenegadoAsync()
        {
            Console.WriteLine("[Sobreprimas] ? Permiso denegado");
            return Task.CompletedTask;
        }


        protected override async Task InicializarPaginaAsync()
        {
           // TituloPagina = ObtenerTexto(TextosApp.Menu.ObtenerEtiqueta((int)CodigosMenu.Sobreprimas));
            LayerOverlayService.Start($"{ObtenerTexto(TextosApp.Common.Loading)} {TituloPagina}");


            CaptionIzquierda = ObtenerTexto(TextosApp.Pages.Sobreprimas.Titulo);

            AñosMaestros = await VersionesService.ObtenerAniosConVersiones();
            NetworksMaestros = await PresupuestosService.ObtenerNetworks();

            string codigosNetwork = ObtenerValoresSeleccionados<CodigoDescripcion, int>(NetworksMaestros, x => x.Codigo, ",");
            _mediosMaestros = await PresupuestosService.ObtenerMediosPorNetWork(codigosNetwork);

            MediosFiltrados = DatosHelper.ClonarObjeto(_mediosMaestros);
            string codigosMedios = ObtenerValoresSeleccionados<CodigoDescripcion, int>(MediosFiltrados, x => x.Codigo, ",");

            AgrupacionesComercialesMaestras = await PresupuestosService.ObtenerAgrupacionesComerciales(codigosMedios);

            FiltroEditoriales filtro = new();
            filtro.CodigosMedios = codigosMedios;
            EditorialesMaestras = await PresupuestosService.ObtenerEditoriales(filtro);

            await ManajarRequest();
        }

        /// <summary>
        /// Se ejecuta cuando el usuario tiene permisos válidos para acceder
        /// Inicializa la página y carga los datos necesarios
        /// </summary>
        //protected override async Task OnPermisoValidadoAsync()
        //{
        //    try
        //    {
        //        TituloPagina = ObtenerTexto(TextosApp.Menu.ObtenerEtiqueta((int)CodigosMenu.CargarSobreprimas));
        //        LayerOverlayService.Start($"{ObtenerTexto(TextosApp.Common.Loading)} {TituloPagina}");

        //        await InicializarPaginaAsync();

        //        await InvokeAsync(StateHasChanged);
        //    }
        //    catch (Exception ex)
        //    {
        //        await LogService.InsertException(ex);
        //        await ErrorService.MostrarErrorInicializandoPagina(TituloPagina, ex);
        //    }
        //    finally
        //    {
        //        LayerOverlayService.Stop();
        //    }
        //}

        /// <summary>
        /// Inicializa la página cargando datos maestros
        /// </summary>
        //private async Task InicializarPaginaAsync()
        //{
        //    CaptionIzquierda = ObtenerTexto(TextosApp.Pages.Sobreprimas.Titulo);

        //    AñosMaestros = await VersionesService.ObtenerAniosConVersiones();
        //    NetworksMaestros = await PresupuestosService.ObtenerNetworks();

        //    string codigosNetwork = GetValoresSeleccionados<CodigoDescripcion, int>(NetworksMaestros, x => x.Codigo, ",");
        //    _mediosMaestros = await PresupuestosService.ObtenerMediosPorNetWork(codigosNetwork);

        //    MediosFiltrados = DatosHelper.ClonarObjeto(_mediosMaestros);
        //    string codigosMedios = GetValoresSeleccionados<CodigoDescripcion, int>(MediosFiltrados, x => x.Codigo, ",");

        //    AgrupacionesComercialesMaestras = await PresupuestosService.ObtenerAgrupacionesComerciales(codigosMedios);

        //    FiltroEditoriales filtro = new();
        //    filtro.CodigosMedios = codigosMedios;
        //    EditorialesMaestras = await PresupuestosService.ObtenerEditoriales(filtro);

        //    await ManajarRequest();
        //}

        #endregion

        #region Filtro

        /// <summary>
        /// Inicializa los valores del filtro
        /// Preselecciona el único network si solo hay uno disponible
        /// </summary>
        private void InicializarFiltro()
        {
            if (NetworksMaestros != null && NetworksMaestros.Count == 1)
            {
                NetworksSeleccionados = new List<CodigoDescripcion> { NetworksMaestros[0] };
            }
        }

        /// <summary>
        /// Maneja el cambio de año seleccionado
        /// Carga las versiones del año seleccionado
        /// </summary>
        private async Task ComboBoxAño_CambioSeleccion(SelectedDataItemChangedEventArgs<CodigoDescripcion> e)
        {
            if (e.DataItem != null)
            {
                if (_filtroSobreprima != null && _filtroSobreprima.Anio != e.DataItem.Codigo)
                {
                    VersionesMaestras = [];
                    _filtroSobreprima.Anio = e.DataItem.Codigo;
                    _filtroSobreprima.CodigoVersion = null;
                    VersionSeleccionada = null;
                    try
                    {
                        LayerOverlayService.Start();
                        VersionesMaestras = await ObtenerVersionesPorPermisos(_filtroSobreprima.Anio);
                    }
                    catch (Exception ex)
                    {
                        await RegistroAplicacion.RegistrarExcepcion(ex);
                        await MensajesHelper.MostrarMensajeError(TituloPagina);
                    }
                    finally
                    {
                        LayerOverlayService.Stop();
                    }
                }
            }
        }

        /// <summary>
        /// Actualiza la lista de medios cuando se modifican los networks seleccionados
        /// </summary>
        private async Task ActualizarMediosCuandoModificamosNetworks(string codigosNetwork)
        {
            try
            {
                MediosFiltrados = await PresupuestosService.ObtenerMediosPorNetWork(codigosNetwork);
                MediosSeleccionados = null;
            }
            catch (Exception ex)
            {
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina);
            }
        }

        /// <summary>
        /// Actualiza la lista de medios cuando se quitan todos los networks
        /// Restaura la lista completa de medios maestros
        /// </summary>
        private async Task ActualizarMediosCuandoQuitamosNetworks()
        {
            try
            {
                MediosFiltrados = DatosHelper.ClonarObjeto(_mediosMaestros);
                MediosSeleccionados = null;
            }
            catch (Exception ex)
            {
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina);
            }
        }

        /// <summary>
        /// Actualiza las editoriales cuando se quitan todos los medios
        /// </summary>
        private async Task ActualizarEditorialesCuandoQuitamosMedios()
        {
            string codigosMedios = ObtenerValoresSeleccionados<CodigoDescripcion, int>(MediosFiltrados, x => x.Codigo, ",");
            AgrupacionesComercialesMaestras = await PresupuestosService.ObtenerAgrupacionesComerciales(codigosMedios);
            await ComprobarAgrupacionesYEditoriales(codigosMedios);
        }

        /// <summary>
        /// Actualiza las editoriales cuando se quitan todas las agrupaciones comerciales
        /// </summary>
        private async void ActualizarEditorialesCuandoQuitamosAgrupaciones()
        {
            FiltroEditoriales filtro = new();
            if (_mediosSeleccionados != null)
            {
                filtro.CodigosMedios = ObtenerValoresSeleccionados<CodigoDescripcion, int>(_mediosSeleccionados, x => x.Codigo, ",");
            }
            else
            {
                filtro.CodigosMedios = ObtenerValoresSeleccionados<CodigoDescripcion, int>(MediosFiltrados, x => x.Codigo, ",");
            }

            EditorialesMaestras = await PresupuestosService.ObtenerEditoriales(filtro);
            EditorialesSeleccionadas = null;
            StateHasChanged();
        }

        /// <summary>
        /// Maneja el cambio de networks seleccionados en el ListBox
        /// Actualiza la lista de medios disponibles
        /// </summary>
        private async Task ListBoxNetworks_CambioValores(IEnumerable<CodigoDescripcion> values, IDropDownBox dropDownBox, bool esSingle)
        {
            dropDownBox.BeginUpdate();
            dropDownBox.Value = values;
            try
            {
                if (!values.Any())
                {
                    LayerOverlayService.Start();
                    await ActualizarMediosCuandoQuitamosNetworks();
                }
            }
            catch (Exception ex)
            {
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina);
            }
            finally
            {
                LayerOverlayService.Stop();
            }

            dropDownBox.EndUpdate();
            if (esSingle)
                dropDownBox.HideDropDown();
        }

        /// <summary>
        /// Maneja el cambio de medios seleccionados en el ListBox
        /// Actualiza las agrupaciones comerciales y editoriales disponibles
        /// </summary>
        private async Task ListBoxMedios_CambioValores(IEnumerable<CodigoDescripcion> values, IDropDownBox dropDownBox, bool esSingle)
        {
            dropDownBox.BeginUpdate();
            dropDownBox.Value = values;
            try
            {
                if (values.Count() == 0)
                {
                    MediosSeleccionados = null; // Ejecuta el set de la propiedad
                }
                else
                {
                    LayerOverlayService.Start();
                    string codigosMedios = ObtenerValoresSeleccionados<CodigoDescripcion, int>(values, x => x.Codigo, ",");

                    // Obtener las agrupaciones en función de los medios seleccionados
                    AgrupacionesComercialesMaestras = await PresupuestosService.ObtenerAgrupacionesComerciales(codigosMedios);
                    await ComprobarAgrupacionesYEditoriales(codigosMedios);
                }
            }
            catch (Exception ex)
            {
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina);
            }
            finally
            {
                LayerOverlayService.Stop();
            }

            dropDownBox.EndUpdate();
            if (esSingle)
                dropDownBox.HideDropDown();
        }

        /// <summary>
        /// Comprueba y ajusta las agrupaciones y editoriales seleccionadas según los medios filtrados
        /// </summary>
        private async Task ComprobarAgrupacionesYEditoriales(string codigosMedios)
        {
            FiltroEditoriales filtro = new();
            filtro.CodigosMedios = codigosMedios;

            // ? Casting seguro con patrón as + ??
            if (AgrupacionesComercialesSeleccionadas != null)
            {
                var lista = ComprobarListaSeleccionadas(
                    AgrupacionesComercialesMaestras, 
                    AgrupacionesComercialesSeleccionadas as List<CodigoDescripcion> ?? []
                );

                if (lista.Count > 0)
                {
                    AgrupacionesComercialesSeleccionadas = new List<CodigoDescripcion>(lista);
                }
                else
                {
                    AgrupacionesComercialesSeleccionadas = new List<CodigoDescripcion>();
                }
                StateHasChanged();

                filtro.CodigosAgrupacionesComerciales = ObtenerValoresSeleccionados<CodigoDescripcion, int>(
                    AgrupacionesComercialesSeleccionadas, 
                    x => x.Codigo, 
                    ","
                );
            }

            EditorialesMaestras = await PresupuestosService.ObtenerEditoriales(filtro);

            if (EditorialesSeleccionadas != null)
            {
                var lista = ComprobarListaSeleccionadas(
                    EditorialesMaestras, 
                    EditorialesSeleccionadas as List<CodigoDescripcion> ?? []
                );

                if (lista.Count > 0)
                {
                    EditorialesSeleccionadas = new List<CodigoDescripcion>(lista);
                }
                else
                {
                    EditorialesSeleccionadas = new List<CodigoDescripcion>();
                }
                StateHasChanged();
            }
        }

        /// <summary>
        /// Comprueba los items de una lista en otra y retorna solo los que existen en ambas
        /// </summary>
        /// <param name="listaDondeBuscar">Lista donde buscar</param>
        /// <param name="listaSeleccionadas">Lista de items seleccionados para buscar</param>
        /// <returns>Lista con solo los items que existen en ambas listas</returns>
        /// <remarks>
        /// Para que el combo mantenga la selección, los objetos devueltos deben ser 
        /// los mismos que los de la lista maestra asignada
        /// </remarks>
        private List<CodigoDescripcion> ComprobarListaSeleccionadas(List<CodigoDescripcion> listaDondeBuscar, List<CodigoDescripcion> listaSeleccionadas)
        {
            return [.. listaDondeBuscar.Where(item => listaSeleccionadas.Any(sel => sel.Codigo == item.Codigo))];
        }

        /// <summary>
        /// Maneja el cambio de agrupaciones comerciales seleccionadas en el ListBox
        /// Actualiza las editoriales disponibles
        /// </summary>
        private async Task ListBoxAgrupaciones_CambioValores(IEnumerable<CodigoDescripcion> values, IDropDownBox dropDownBox, bool esSingle)
        {
            dropDownBox.BeginUpdate();
            dropDownBox.Value = values;

            if (values.Count() == 0)
            {
                if (_mediosSeleccionados != null)
                {
                    FiltroEditoriales filtro = new();
                    filtro.CodigosMedios = ObtenerValoresSeleccionados<CodigoDescripcion, int>(_mediosSeleccionados, x => x.Codigo, ",");
                    EditorialesMaestras = await PresupuestosService.ObtenerEditoriales(filtro);
                }
                else
                {
                    EditorialesMaestras = await PresupuestosService.ObtenerEditoriales();
                }
            }
            else
            {
                try
                {
                    LayerOverlayService.Start();
                    FiltroEditoriales filtro = new();
                    filtro.CodigosAgrupacionesComerciales = ObtenerValoresSeleccionados<CodigoDescripcion, int>(values, x => x.Codigo, ",");
                    filtro.CodigosMedios = ObtenerValoresSeleccionados<CodigoDescripcion, int>(_mediosSeleccionados, x => x.Codigo, ",");

                    EditorialesMaestras = await PresupuestosService.ObtenerEditoriales(filtro);
                    EditorialesSeleccionadas = null;
                }
                catch (Exception ex)
                {
                    await RegistroAplicacion.RegistrarExcepcion(ex);
                    await MensajesHelper.MostrarMensajeError(TituloPagina);
                }
                finally
                {
                    LayerOverlayService.Stop();
                }
            }
            dropDownBox.EndUpdate();
            if (esSingle)
                dropDownBox.HideDropDown();
        }

        /// <summary>
        /// Maneja el cambio de editoriales seleccionadas en el ListBox
        /// </summary>
        private void ListBoxEditoriales_CambioValores(IEnumerable<CodigoDescripcion> values, IDropDownBox dropDownBox, bool esSingle)
        {
            dropDownBox.BeginUpdate();
            dropDownBox.Value = values;
            dropDownBox.EndUpdate();
            if (esSingle)
            {
                dropDownBox.HideDropDown();
            }
        }

        /// <summary>
        /// Valida que se haya seleccionado una versión (campo obligatorio)
        /// </summary>
        private bool ValidarCamposObligatoriosFiltro()
        {
            return HayVersionSeleccionada();
        }

        /// <summary>
        /// Obtiene la lista de sobreprimas según los filtros aplicados
        /// Convierte la lista a SobreprimaGridModel para mostrar en el grid
        /// </summary>
        private async Task AplicarFiltro()
        {
            if (!ValidarCamposObligatoriosFiltro())
            {
                await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(TextosApp.Mensajes.CamposObligatorios));
                return;
            }
            try
            {
                LayerOverlayService.Start();
                SobreprimasGrid = [];

                _filtroSobreprima.Anio = AñoSeleccionado!.Codigo;
                _filtroSobreprima.CodigoVersion = VersionSeleccionada!.Codigo;
                _filtroSobreprima.CodigoNetworkList = ObtenerValoresSeleccionados<CodigoDescripcion, int>(_networksSeleccionados, x => x.Codigo, ",");
                _filtroSobreprima.CodigoMedioList = ObtenerValoresSeleccionados<CodigoDescripcion, int>(_mediosSeleccionados, x => x.Codigo, ",");
                _filtroSobreprima.CodigoAgrupacionComercialList = ObtenerValoresSeleccionados<CodigoDescripcion, int>(_agrupacionesComercialesSeleccionadas, x => x.Codigo, ",");
                _filtroSobreprima.CodigoEditorialList = ObtenerValoresSeleccionados<CodigoDescripcion, int>(EditorialesSeleccionadas, x => x.Codigo, ",");

                var listaSobreprimas = await SobreprimasService.ObtenerSobreprimas(_filtroSobreprima);

                if (listaSobreprimas.Count == 0)
                {
                    if (!_desdePaginaImportarMMS)
                    {
                        await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(TextosApp.Mensajes.RegistrosNoEncontrados));
                    }
                    GridSobreprimas.SetFocusedRowIndex(-1);
                }
                else
                {
                    SobreprimasGrid = await ConvertirSobreprimasEnModeloGrid(listaSobreprimas);
                    GridSobreprimas.SetFocusedRowIndex(0);
                }

                CaptionDerecha = $"[{AñoSeleccionado?.Descripcion ?? ""}, {VersionSeleccionada!.Descripcion}]";
            }
            catch (Exception ex)
            {
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina);
            }
            finally
            {
                LayerOverlayService.Stop();
                _desdePaginaImportarMMS = false;
            }
        }

        /// <summary>
        /// Limpia todos los filtros y reinicia el estado de la página
        /// </summary>
        private async Task LimpiarFiltro()
        {
            try
            {
                LayerOverlayService.Start();
                _filtroSobreprima = new();
                AñoSeleccionado = null;
                VersionSeleccionada = null;
                NetworksSeleccionados = null;
                MediosSeleccionados = null;
                AgrupacionesComercialesSeleccionadas = null;
                EditorialesSeleccionadas = null;

                SobreprimasGrid.Clear();
                VersionesMaestras.Clear();

                CaptionIzquierda = string.Empty;
                CaptionDerecha = string.Empty;

                InicializarFiltro();
            }
            catch (Exception ex)
            {
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina);
            }
            finally
            {
                LayerOverlayService.Stop();
            }
        }

        #endregion

        #region Grid Sobreprimas - Eventos

        /// <summary>
        /// Maneja el doble clic en una fila del grid
        /// Abre el popup de edición si el medio es accesible
        /// </summary>
        private async Task GridSobreprimas_DobleClick(GridRowClickEventArgs e)
        {
            var sobreprima = (SobreprimaGridModel?)GridSobreprimas.GetDataItem(e.VisibleIndex);

            if (sobreprima == null) return;

            if (sobreprima.MedioAccesible)
            {
                await MostrarPopupEdicion(sobreprima, ModoOperacion.Modificar);
            }
        }

        /// <summary>
        /// Crea una nueva sobreprima y abre el popup de edición
        /// </summary>
        private async Task NuevaSobreprima()
        {
            try
            {
                var nuevaSobreprima = new SobreprimaGridModel
                {
                    Anio = _filtroSobreprima!.Anio,
                    CodigoVersion = VersionSeleccionada!.Codigo
                };

                await MostrarPopupEdicion(nuevaSobreprima, ModoOperacion.Insertar);
            }
            catch (Exception ex)
            {
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina);
            }
        }

        /// <summary>
        /// Muestra el popup de edición de sobreprima
        /// Inicializa las listas maestras del popup según el modo de operación
        /// </summary>
        private async Task MostrarPopupEdicion(SobreprimaGridModel sobreprima, ModoOperacion modoOperacion)
        {
            try
            {
                LayerOverlayService.Start();
                _agrupacionesComercialesPopup.Clear();
                _editorialesPopup.Clear();

                _sobreprimaEnEdicion = DatosHelper.ClonarObjeto(sobreprima);

                if (modoOperacion == ModoOperacion.Insertar)
                {
                    if (NetworksMaestros != null && NetworksMaestros.Count == 1)
                    {
                        _sobreprimaEnEdicion.CodigoNetwork = NetworksMaestros[0].Codigo;
                        _mediosPopup = await PresupuestosService.ObtenerMediosPorNetWork(_sobreprimaEnEdicion.CodigoNetwork.ToString());
                    }
                }
                else if (modoOperacion == ModoOperacion.Modificar)
                {
                    _mediosPopup = await PresupuestosService.ObtenerMediosPorNetWork(_sobreprimaEnEdicion.CodigoNetwork.ToString());
                }

                _sobreprimaOriginal = DatosHelper.ClonarObjeto(_sobreprimaEnEdicion);
                _sobreprimaEnEdicion.ModoOperacion = modoOperacion;

                if (modoOperacion == ModoOperacion.Modificar)
                {
                    FiltroEditoriales filtro = new();

                    filtro.CodigosMedios = sobreprima.CodigoMedio?.ToString() ?? string.Empty;
                    _agrupacionesComercialesPopup = await PresupuestosService.ObtenerAgrupacionesComerciales(filtro.CodigosMedios);

                    filtro.CodigosAgrupacionesComerciales = sobreprima.CodigoAgrupacionComercial?.ToString() ?? string.Empty;
                    _editorialesPopup = await PresupuestosService.ObtenerEditoriales(filtro);
                }

                _popupEdicionVisible = true;
                if (_popupSobreprimas != null)
                {
                    _tituloPopupEdicion = modoOperacion == ModoOperacion.Insertar
                        ? ObtenerTexto(TextosApp.Common.Nuevo)
                        : ObtenerTexto(TextosApp.Common.Edit);
                }
            }
            catch (Exception ex)
            {
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina);
            }
            finally
            {
                LayerOverlayService.Stop();
            }
        }

        /// <summary>
        /// Elimina una sobreprima previa confirmación del usuario
        /// </summary>
        private async Task EliminarSobreprima(SobreprimaGridModel sobreprima)
        {
            if (sobreprima == null) return;

            if (!await MensajesHelper.MostrarMensajeParaConfirmacion(TituloPagina, ObtenerTexto(TextosApp.Mensajes.ConfirmacionEliminar)))
            {
                return;
            }

            try
            {
                LayerOverlayService.Start();
                await SobreprimasService.EliminarSobreprimas(sobreprima);
                SobreprimasGrid.Remove(sobreprima);
                await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(TextosApp.Mensajes.RegistroEliminado));
            }
            catch (Exception ex)
            {
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Mensajes.ErrorDelete));
            }
            finally
            {
                LayerOverlayService.Stop();
            }
        }

        /// <summary>
        /// Oculta el popup de edición de sobreprima
        /// </summary>
        private void OcultarPopupEdicion()
        {
            _popupEdicionVisible = false;
        }

        #endregion

        #region Popup Edición - Eventos ComboBox

        /// <summary>
        /// Maneja el cambio de agrupación comercial en el popup
        /// Actualiza la lista de editoriales disponibles
        /// </summary>
        private async Task PopupComboBoxAgrupacion_CambioSeleccion(SelectedDataItemChangedEventArgs<CodigoDescripcion> e)
        {
            try
            {
                if (e.ChangeSource == SelectionChangeSource.UserAction)
                {
                    LayerOverlayService.Start();
                    FiltroEditoriales filtro = new();
                    if (e.DataItem != null)
                    {
                        int codigoAgrupacion = e.DataItem.Codigo;

                        filtro.CodigosAgrupacionesComerciales = codigoAgrupacion.ToString();
                        filtro.CodigosMedios = _sobreprimaEnEdicion.CodigoMedio?.ToString() ?? string.Empty;

                        _editorialesPopup = await PresupuestosService.ObtenerEditoriales(filtro);

                        _sobreprimaEnEdicion.DescripcionAgrupacionComercial = e.DataItem.Descripcion;
                        _sobreprimaEnEdicion.CodigoEditorial = null;
                    }
                    else
                    {
                        filtro.CodigosMedios = _sobreprimaEnEdicion.CodigoMedio?.ToString() ?? string.Empty;
                        _editorialesPopup = await PresupuestosService.ObtenerEditoriales(filtro);

                        _sobreprimaEnEdicion.DescripcionAgrupacionComercial = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina);
            }
            finally
            {
                LayerOverlayService.Stop();
            }
        }

        /// <summary>
        /// Maneja el cambio de network en el popup
        /// Actualiza la lista de medios disponibles
        /// </summary>
        private async Task PopupComboBoxNetwork_CambioSeleccion(SelectedDataItemChangedEventArgs<CodigoDescripcion> e)
        {
            if (e.ChangeSource == SelectionChangeSource.UserAction)
            {
                if (e.DataItem != null)
                {
                    try
                    {
                        LayerOverlayService.Start();
                        string codigoNetwork = e.DataItem.Codigo.ToString();

                        _mediosPopup = await PresupuestosService.ObtenerMediosPorNetWork(codigoNetwork);
                    }
                    catch (Exception ex)
                    {
                        await RegistroAplicacion.RegistrarExcepcion(ex);
                        await MensajesHelper.MostrarMensajeError(TituloPagina);
                    }
                    finally
                    {
                        LayerOverlayService.Stop();
                    }
                }
            }
        }

        /// <summary>
        /// Maneja el cambio de medio en el popup
        /// Actualiza las listas de agrupaciones comerciales y editoriales disponibles
        /// </summary>
        private async Task PopupComboBoxMedio_CambioSeleccion(SelectedDataItemChangedEventArgs<CodigoDescripcion> e)
        {
            if (e.ChangeSource == SelectionChangeSource.UserAction)
            {
                if (e.DataItem != null)
                {
                    try
                    {
                        LayerOverlayService.Start();
                        string codigoMedio = e.DataItem.Codigo.ToString();
                        _agrupacionesComercialesPopup = await PresupuestosService.ObtenerAgrupacionesComerciales(codigoMedio);

                        FiltroEditoriales filtro = new();
                        filtro.CodigosMedios = codigoMedio;
                        _editorialesPopup = await PresupuestosService.ObtenerEditoriales(filtro);
                    }
                    catch (Exception ex)
                    {
                        await RegistroAplicacion.RegistrarExcepcion(ex);
                        await MensajesHelper.MostrarMensajeError(TituloPagina);
                    }
                    finally
                    {
                        LayerOverlayService.Stop();
                    }
                }
            }
        }

        #endregion

        #region Grid Sobreprimas - CRUD

        /// <summary>
        /// Crea un objeto Sobreprima base a partir de un SobreprimaGridModel
        /// </summary>
        private Sobreprima CrearSobreprimaBase(SobreprimaGridModel sobreprimaGrid)
        {
            return new Sobreprima
            {
                CodigoVersion = sobreprimaGrid.CodigoVersion,
                CodigoNetwork = sobreprimaGrid.CodigoNetwork,
                CodigoMedio = sobreprimaGrid.CodigoMedio!.Value,
                CodigoEditorial = sobreprimaGrid.CodigoEditorial!.Value,
                CodigoPais = sobreprimaGrid.CodigoPais
            };
        }

        /// <summary>
        /// Convierte un SobreprimaGridModel en una lista de Sobreprimas (una por concepto)
        /// Solo incluye los conceptos que han cambiado (optimización)
        /// </summary>
        public List<Sobreprima> ConvertirModeloGridEnSobreprimas(SobreprimaGridModel sobreprimaGrid)
        {
            List<Sobreprima> lista = [];

            // Si cambió la clave compuesta (Network, Medio, Agrupación, Editorial) ? enviar todos los conceptos
            if (sobreprimaGrid.KeyGrid != _sobreprimaOriginal.KeyGrid)
            {
                var sobreprimaDefault = CrearSobreprimaBase(sobreprimaGrid);
                sobreprimaDefault.Codigo = sobreprimaGrid.ConceptoDefaul.Codigo;
                sobreprimaDefault.Porcentaje = sobreprimaGrid.ConceptoDefaul.Porcentaje;
                sobreprimaDefault.CodigoConcepto = sobreprimaGrid.ConceptoDefaul.CodigoConcepto;
                lista.Add(sobreprimaDefault);

                var sobreprimaSLA = CrearSobreprimaBase(sobreprimaGrid);
                sobreprimaSLA.Codigo = sobreprimaGrid.ConceptoSLA.Codigo;
                sobreprimaSLA.Porcentaje = sobreprimaGrid.ConceptoSLA.Porcentaje;
                sobreprimaSLA.CodigoConcepto = sobreprimaGrid.ConceptoSLA.CodigoConcepto;
                lista.Add(sobreprimaSLA);

                var sobreprimaHVP = CrearSobreprimaBase(sobreprimaGrid);
                sobreprimaHVP.Codigo = sobreprimaGrid.ConceptoHVP.Codigo;
                sobreprimaHVP.Porcentaje = sobreprimaGrid.ConceptoHVP.Porcentaje;
                sobreprimaHVP.CodigoConcepto = sobreprimaGrid.ConceptoHVP.CodigoConcepto;
                lista.Add(sobreprimaHVP);
            }
            else
            {
                // Solo agregar los conceptos que cambiaron
                if (sobreprimaGrid.ConceptoDefaul.Porcentaje != _sobreprimaOriginal.ConceptoDefaul.Porcentaje)
                {
                    var sobreprimaDefault = CrearSobreprimaBase(sobreprimaGrid);
                    sobreprimaDefault.Codigo = sobreprimaGrid.ConceptoDefaul.Codigo;
                    sobreprimaDefault.Porcentaje = sobreprimaGrid.ConceptoDefaul.Porcentaje;
                    sobreprimaDefault.CodigoConcepto = sobreprimaGrid.ConceptoDefaul.CodigoConcepto;
                    lista.Add(sobreprimaDefault);
                }

                if (sobreprimaGrid.ConceptoSLA.Porcentaje != _sobreprimaOriginal.ConceptoSLA.Porcentaje)
                {
                    var sobreprimaSLA = CrearSobreprimaBase(sobreprimaGrid);
                    sobreprimaSLA.Codigo = sobreprimaGrid.ConceptoSLA.Codigo;
                    sobreprimaSLA.Porcentaje = sobreprimaGrid.ConceptoSLA.Porcentaje;
                    sobreprimaSLA.CodigoConcepto = sobreprimaGrid.ConceptoSLA.CodigoConcepto;
                    lista.Add(sobreprimaSLA);
                }

                if (sobreprimaGrid.ConceptoHVP.Porcentaje != _sobreprimaOriginal.ConceptoHVP.Porcentaje)
                {
                    var sobreprimaHVP = CrearSobreprimaBase(sobreprimaGrid);
                    sobreprimaHVP.Codigo = sobreprimaGrid.ConceptoHVP.Codigo;
                    sobreprimaHVP.Porcentaje = sobreprimaGrid.ConceptoHVP.Porcentaje;
                    sobreprimaHVP.CodigoConcepto = sobreprimaGrid.ConceptoHVP.CodigoConcepto;
                    lista.Add(sobreprimaHVP);
                }
            }

            return lista;
        }

        /// <summary>
        /// Verifica si la sobreprima está duplicada en base de datos
        /// </summary>
        private async Task<bool> SobreprimaEstaDuplicada(SobreprimaGridModel sobreprima)
        {
            var filtro = new SobreprimaFiltro
            {
                Anio = sobreprima.Anio,
                CodigoVersion = sobreprima.CodigoVersion,
                CodigoNetworkList = sobreprima.CodigoNetwork > 0 ? sobreprima.CodigoNetwork.ToString() : "",
                CodigoMedioList = (sobreprima.CodigoMedio.HasValue && sobreprima.CodigoMedio > 0) ? sobreprima.CodigoMedio.Value.ToString() : "",
                CodigoAgrupacionComercialList = (sobreprima.CodigoAgrupacionComercial.HasValue && sobreprima.CodigoAgrupacionComercial > 0) ? sobreprima.CodigoAgrupacionComercial.Value.ToString() : "",
                CodigoEditorialList = (sobreprima.CodigoEditorial.HasValue && sobreprima.CodigoEditorial > 0) ? sobreprima.CodigoEditorial.Value.ToString() : ""
            };

            if (sobreprima.ModoOperacion == ModoOperacion.Modificar)
            {
                return await SobreprimasService.ExistenSobreprimas(filtro, sobreprima);
            }
            else
            {
                return await SobreprimasService.ExistenSobreprimas(filtro);
            }
        }

        /// <summary>
        /// Inserta o actualiza una sobreprima en el grid y en base de datos
        /// Realiza validaciones completas antes de guardar
        /// </summary>
        private async Task GuardarSobreprima()
        {
            bool grabacionExitosa = false;
            try
            {
                bool noHayCambios = DatosHelper.SonIguales(_sobreprimaEnEdicion, _sobreprimaOriginal);

                if (noHayCambios)
                {
                    await MensajesHelper.MostrarMensajeAviso(TituloPagina, ObtenerTexto(TextosApp.Mensajes.SinCambios));
                    return;
                }

                LayerOverlayService.Start();

                // Validar campos obligatorios
                var validaciones = new List<(bool Condicion, string ResourceKey)>
                {
                    (_sobreprimaEnEdicion.CodigoNetwork == 0, TextosApp.Common.Network),
                    (_sobreprimaEnEdicion.CodigoMedio == null, TextosApp.Common.Medio),
                    (_sobreprimaEnEdicion.CodigoEditorial == null, TextosApp.Common.Editorial),
                    (_sobreprimaEnEdicion.ModoOperacion == ModoOperacion.Insertar
                        && _sobreprimaEnEdicion.ConceptoDefaul.Porcentaje == 0
                        && _sobreprimaEnEdicion.ConceptoSLA.Porcentaje == 0
                        && _sobreprimaEnEdicion.ConceptoHVP.Porcentaje == 0,
                        TextosApp.Common.Porcentaje)
                };

                var campoError = validaciones
                    .Where(v => v.Condicion)
                    .Select(v => ObtenerTexto(v.ResourceKey))
                    .FirstOrDefault();

                if (!string.IsNullOrEmpty(campoError))
                {
                    await MensajesHelper.MostrarMensajeError(
                        TituloPagina,
                        $"{ObtenerTexto(TextosApp.Mensajes.MandatoryField)}: {campoError}"
                    );
                    return;
                }

                // Validar duplicados en lista local (inserción)
                if (_sobreprimaEnEdicion.ModoOperacion == ModoOperacion.Insertar
                    && SobreprimasGrid.Find(x => x.KeyGrid == _sobreprimaEnEdicion.KeyGrid) != null)
                {
                    await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Mensajes.SobreprimaDuplicated));
                    return;
                }

                // Validar duplicados en lista local (edición)
                if (_sobreprimaEnEdicion.ModoOperacion != ModoOperacion.Insertar
                    && SobreprimasGrid.Find(x => x.KeyGrid == _sobreprimaEnEdicion.KeyGrid
                                                && x.Codigo != _sobreprimaEnEdicion.Codigo) != null)
                {
                    await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Mensajes.SobreprimaDuplicated));
                    return;
                }

                // Validar duplicados en base de datos
                if (await SobreprimaEstaDuplicada(_sobreprimaEnEdicion))
                {
                    await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Mensajes.SobreprimaDuplicated));
                    return;
                }

                // Actualizar descripciones para mostrar en el grid
                var network = NetworksMaestros.FirstOrDefault(x => x.Codigo == _sobreprimaEnEdicion.CodigoNetwork);
                _sobreprimaEnEdicion.DescripcionNetwork = network?.Descripcion ?? "";

                var medio = MediosFiltrados.FirstOrDefault(x => x.Codigo == _sobreprimaEnEdicion.CodigoMedio);
                _sobreprimaEnEdicion.DescripcionMedio = medio?.Descripcion ?? "";

                var agrupacion = AgrupacionesComercialesMaestras.FirstOrDefault(x => x.Codigo == _sobreprimaEnEdicion.CodigoAgrupacionComercial);
                _sobreprimaEnEdicion.DescripcionAgrupacionComercial = agrupacion?.Descripcion ?? "";

                var editorial = EditorialesMaestras.FirstOrDefault(x => x.Codigo == _sobreprimaEnEdicion.CodigoEditorial);
                _sobreprimaEnEdicion.DescripcionEditorial = editorial?.Descripcion ?? "";

                _sobreprimaEnEdicion.CodigoPais = Usuario!.CodigoPais;

                // Convertir y guardar
                List<Sobreprima> sobreprimas = ConvertirModeloGridEnSobreprimas(_sobreprimaEnEdicion);
                await SobreprimasService.GrabarSobreprimas(sobreprimas);

                grabacionExitosa = true; // Marcamos que la grabación fue correcta

                // Actualizar códigos de conceptos insertados (devueltos por el servicio)
                foreach (var sobreprima in sobreprimas)
                {
                    switch (sobreprima.CodigoConcepto)
                    {
                        case (int)ConceptosSobreprimas.Sobreprima:
                            _sobreprimaEnEdicion.ConceptoDefaul.Codigo = sobreprima.Codigo;
                            break;
                        case (int)ConceptosSobreprimas.SLA:
                            _sobreprimaEnEdicion.ConceptoSLA.Codigo = sobreprima.Codigo;
                            break;
                        case (int)ConceptosSobreprimas.HVP:
                            _sobreprimaEnEdicion.ConceptoHVP.Codigo = sobreprima.Codigo;
                            break;
                    }
                }

                // Actualizar grid según modo de operación
                if (_sobreprimaEnEdicion.ModoOperacion == ModoOperacion.Insertar)
                {
                    // Obtener el código mayor para asignar a la nueva fila
                    int codigoMayor = 0;
                    if (SobreprimasGrid.Count > 0)
                    {
                        codigoMayor = SobreprimasGrid.Max(x => x.Codigo);
                    }

                    _sobreprimaEnEdicion.Codigo = codigoMayor + 1;

                    SobreprimasGrid.Insert(0, _sobreprimaEnEdicion);
                    GridSobreprimas.SetFocusedRowIndex(0);
                    await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(TextosApp.Common.DatosGrabados));
                }
                else
                {
                    // Actualizar la lista del grid
                    var indice = SobreprimasGrid.FindIndex(x => x.Codigo == _sobreprimaEnEdicion.Codigo);
                    if (indice >= 0)
                    {
                        // Si todos los porcentajes son 0, se habrá eliminado de BD
                        bool todosLosConceptosEnCero =
                            _sobreprimaEnEdicion.ConceptoDefaul.Porcentaje == 0 &&
                            _sobreprimaEnEdicion.ConceptoSLA.Porcentaje == 0 &&
                            _sobreprimaEnEdicion.ConceptoHVP.Porcentaje == 0;

                        if (todosLosConceptosEnCero)
                        {
                            SobreprimasGrid.RemoveAt(indice);
                        }
                        else
                        {
                            SobreprimasGrid[indice] = _sobreprimaEnEdicion;
                        }
                    }

                    await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(TextosApp.Common.DatosGrabados));
                    GridSobreprimas.Reload();
                }

                OcultarPopupEdicion();
            }
            catch (Exception ex)
            {
                await RegistroAplicacion.RegistrarExcepcion(ex);

                if (grabacionExitosa)
                {
                    // Error después de grabar: mensaje especial y recargar
                    await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(TextosApp.Mensajes.ErrorDespuesDeGrabar));
                    OcultarPopupEdicion();
                    await AplicarFiltro();
                }
                else
                {
                    // Error antes de grabar: mensaje normal
                    OcultarPopupEdicion();
                    await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Mensajes.ErrorAlGrabar));
                }
            }
            finally
            {
                LayerOverlayService.Stop();
            }
        }

        /// <summary>
        /// Convierte una lista de Sobreprima en SobreprimaGridModel agrupando por clave compuesta
        /// Obtiene descripciones de entidades no accesibles por permisos
        /// </summary>
        private async Task<List<SobreprimaGridModel>> ConvertirSobreprimasEnModeloGrid(List<Sobreprima> listaSobreprimas)
        {
            var agrupados = listaSobreprimas
                .GroupBy(x => new
                {
                    x.CodigoPais,
                    x.CodigoVersion,
                    x.CodigoNetwork,
                    x.CodigoMedio,
                    x.CodigoAgrupacionComercial,
                    x.CodigoEditorial
                })
                .Select(g =>
                {
                    var resultado = new SobreprimaGridModel
                    {
                        CodigoVersion = g.Key.CodigoVersion,
                        CodigoNetwork = g.Key.CodigoNetwork,
                        CodigoMedio = g.Key.CodigoMedio,
                        CodigoAgrupacionComercial = g.Key.CodigoAgrupacionComercial,
                        CodigoEditorial = g.Key.CodigoEditorial,
                        DescripcionNetwork = NetworksMaestros.FirstOrDefault(o => o.Codigo == g.Key.CodigoNetwork)?.Descripcion ?? string.Empty,
                        DescripcionMedio = _mediosMaestros.FirstOrDefault(o => o.Codigo == g.Key.CodigoMedio)?.Descripcion ?? string.Empty,
                        DescripcionAgrupacionComercial = AgrupacionesComercialesMaestras.FirstOrDefault(o => o.Codigo == g.Key.CodigoAgrupacionComercial)?.Descripcion ?? string.Empty,
                        DescripcionEditorial = EditorialesMaestras.FirstOrDefault(o => o.Codigo == g.Key.CodigoEditorial)?.Descripcion ?? string.Empty,
                        CodigoPais = g.Key.CodigoPais,
                        ModoOperacion = ModoOperacion.Ninguna
                    };

                    // Asignar concepto Default
                    var conceptoDefault = g.FirstOrDefault(x => x.CodigoConcepto == (int)ConceptosSobreprimas.Sobreprima);
                    if (conceptoDefault != null)
                    {
                        resultado.ConceptoDefaul = new RegistroConcepto
                        {
                            Codigo = conceptoDefault.Codigo,
                            CodigoConcepto = conceptoDefault.CodigoConcepto,
                            Porcentaje = conceptoDefault.Porcentaje
                        };
                    }

                    // Asignar concepto SLA
                    var conceptoSLA = g.FirstOrDefault(x => x.CodigoConcepto == (int)ConceptosSobreprimas.SLA);
                    if (conceptoSLA != null)
                    {
                        resultado.ConceptoSLA = new RegistroConcepto
                        {
                            Codigo = conceptoSLA.Codigo,
                            CodigoConcepto = conceptoSLA.CodigoConcepto,
                            Porcentaje = conceptoSLA.Porcentaje
                        };
                    }

                    // Asignar concepto HVP
                    var conceptoHVP = g.FirstOrDefault(x => x.CodigoConcepto == (int)ConceptosSobreprimas.HVP);
                    if (conceptoHVP != null)
                    {
                        resultado.ConceptoHVP = new RegistroConcepto
                        {
                            Codigo = conceptoHVP.Codigo,
                            CodigoConcepto = conceptoHVP.CodigoConcepto,
                            Porcentaje = conceptoHVP.Porcentaje
                        };
                    }
                    return resultado;
                })
                .OrderBy(x => x.DescripcionNetwork)
                .ThenBy(x => x.DescripcionMedio)
                .ThenBy(x => x.DescripcionAgrupacionComercial)
                .ThenBy(x => x.DescripcionEditorial)
                .ToList();

            // Asignar códigos secuenciales para identificación en el grid
            for (int i = 0; i < agrupados.Count; i++)
            {
                agrupados[i].Codigo = i + 1;
            }

            // Validar y obtener descripciones para sobreprimas sin acceso al medio
            foreach (var item in agrupados.Where(x => string.IsNullOrEmpty(x.DescripcionMedio)))
            {
                item.MedioAccesible = false;

                // Obtener descripción del medio
                if (item.CodigoMedio.HasValue)
                {
                    var medio = await PresupuestosService.ObtenerMedio(item.CodigoMedio.Value);
                    item.DescripcionMedio = medio?.Descripcion ?? "Sin Medio";
                }
                else
                {
                    item.DescripcionMedio = "Sin Medio";
                }

                // Obtener descripción de agrupación comercial
                if (item.CodigoAgrupacionComercial.HasValue)
                {
                    var agrupacion = await PresupuestosService.ObtenerAgrupacionComercial(item.CodigoAgrupacionComercial.Value);
                    item.DescripcionAgrupacionComercial = agrupacion?.Descripcion ?? "Sin Agrupación comercial";
                }
                else
                {
                    item.DescripcionAgrupacionComercial = "Sin Agrupación comercial";
                }

                // Obtener descripción de editorial
                if (item.CodigoEditorial.HasValue)
                {
                    var editorial = await PresupuestosService.ObtenerEditorial(item.CodigoEditorial.Value);
                    item.DescripcionEditorial = editorial?.Descripcion ?? "Sin Editorial";
                }
                else
                {
                    item.DescripcionEditorial = "Sin Editorial";
                }
            }

            return agrupados;
        }

        /// <summary>
        /// Verifica si hay una versión seleccionada en el filtro
        /// </summary>
        private bool HayVersionSeleccionada()
        {
            return VersionSeleccionada != null;
        }

        #endregion

        #region Navegación

        /// <summary>
        /// Maneja la navegación desde la página de importación de sobreprimas de MMS
        /// Aplica automáticamente los filtros recibidos
        /// </summary>
        private async Task ManajarRequest()
        {
            try
            {
                var datos = NavegacionService.Obtener<dynamic>();
                if (datos == null) return;

                NavegacionService.Limpiar();

                if (datos is SobreprimaImportarFiltro filtro)
                {
                    AñoSeleccionado = AñosMaestros.FirstOrDefault(n => n.Codigo == filtro.Anio);
                    if (AñoSeleccionado != null)
                    {
                        VersionesMaestras = await ObtenerVersionesPorPermisos(AñoSeleccionado.Codigo);
                    }
                    VersionSeleccionada = VersionesMaestras.FirstOrDefault(n => n.Codigo == filtro.CodigoVersion);

                    var listaNetworks = NetworksMaestros.Where(n => filtro.CodigosNetwork.Contains(n.Codigo)).ToList();

                    // Comprobar si se seleccionaron todos los networks
                    var codigosListaNetworks = listaNetworks.Select(n => n.Codigo).OrderBy(c => c);
                    var codigosMasterNetwork = NetworksMaestros.Select(n => n.Codigo).OrderBy(c => c);
                    bool sonIguales = codigosListaNetworks.SequenceEqual(codigosMasterNetwork);

                    // ? Ya no usa object?, usa IEnumerable<CodigoDescripcion> directamente
                    NetworksSeleccionados = sonIguales
                        ? Enumerable.Empty<CodigoDescripcion>()
                        : listaNetworks;

                    var listaMedios = MediosFiltrados.Where(n => filtro.CodigosMedio.Contains(n.Codigo)).ToList();

                    // Comprobar si se seleccionaron todos los medios
                    var codigosListaMedios = listaMedios.Select(n => n.Codigo).OrderBy(c => c);
                    var codigosMedioFiltro = MediosFiltrados.Select(n => n.Codigo).OrderBy(c => c);
                    bool sonIgualesMedios = codigosListaMedios.SequenceEqual(codigosMedioFiltro);

                    MediosSeleccionados = sonIgualesMedios
                        ? Enumerable.Empty<CodigoDescripcion>()
                        : listaMedios;

                    StateHasChanged();

                    // Aplicar filtro automáticamente si todos los campos están completos
                    if (AñoSeleccionado != null && NetworksSeleccionados != null && VersionSeleccionada != null && MediosSeleccionados != null)
                    {
                        _desdePaginaImportarMMS = true;
                        await AplicarFiltro();
                    }
                }
                else
                {
                    InicializarFiltro();
                }
            }
            catch (Exception ex)
            {
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Common.Messages.UndefinedError));
            }
        }
        #endregion
    }
}
