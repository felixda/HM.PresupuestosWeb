

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
        [Inject] protected IMaestrosCacheService PresupuestosService { get; set; } = default!;
        [Inject] protected ParametrosNavegacion NavegacionService { get; set; } = default!;

        #endregion

        #region Propiedades Privadas

        #region Página

        private string CaptionIzquierda { get; set; } = string.Empty;
        private string CaptionDerecha { get; set; } = string.Empty;

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
        private object? NetworksSeleccionadosBacking;
        private object? MediosSeleccionadosBacking;
        private object? AgrupacionesComercialesSeleccionadasBacking;


        private object? NetworksSeleccionados
        {
            get => NetworksSeleccionadosBacking;
            set
            {
                if (NetworksSeleccionadosBacking != value)
                {
                    NetworksSeleccionadosBacking = value;
                    var codigosNetwork = ObtenerValoresSeleccionados<CodigoDescripcion, int>(value, x => x.Codigo);

                    if (string.IsNullOrEmpty(codigosNetwork))
                    {
                        _ = ActualizarMediosCuandoQuitamosNetworksAsync();
                    }
                    else
                    {
                        _ = ActualizarMediosCuandoModificamosNetworksAsync(codigosNetwork);
                    }
                }
            }
        }

        private object? MediosSeleccionados
        {
            get => MediosSeleccionadosBacking;
            set
            {
                if (MediosSeleccionadosBacking != value)
                {
                    MediosSeleccionadosBacking = value;
                }

                if (value == null)
                {
                    _ = ActualizarEditorialesCuandoQuitamosMediosAsync();
                }
            }
        }

        private object? AgrupacionesComercialesSeleccionadas
        {
            get => AgrupacionesComercialesSeleccionadasBacking;
            set
            {
                if (AgrupacionesComercialesSeleccionadasBacking != value)
                {
                    AgrupacionesComercialesSeleccionadasBacking = value;
                }

                if (value == null)
                {
                    _ = ActualizarEditorialesCuandoQuitamosAgrupacionesAsync();
                }
            }
        }

        private object? EditorialesSeleccionadas { get; set; }

        // Listas internas del popup 
        private List<CodigoDescripcion> MediosMaestros { get; set; } = [];
        private List<CodigoDescripcion> MediosPopup { get; set; } = [];
        private List<CodigoDescripcion> AgrupacionesComercialesPopup { get; set; } = [];
        private List<CodigoDescripcion> EditorialesPopup { get; set; } = [];

        #endregion

        #region Grid Sobreprimas

        private DxGrid GridSobreprimas { get; set; } = new DxGrid();
        private List<SobreprimaGridModel> SobreprimasGrid { get; set; } = [];

        private bool PopupEdicionVisible { get; set; } = false;
        private SobreprimaGridModel SobreprimaEnEdicion { get; set; } = new();
        private SobreprimaGridModel SobreprimaOriginal { get; set; } = new();
        private DxPopup? PopupSobreprimas { get; set; }
        private string TituloPopupEdicion { get; set; } = string.Empty;

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
            CaptionIzquierda = ObtenerTexto(TextosApp.Pages.Sobreprimas.Titulo);

            // A: Fase 1 — años y networks en paralelo (repositorios distintos → seguro)
            var añosTask    = VersionesService.ObtenerAniosConVersiones();
            var networksTask = PresupuestosService.ObtenerNetworks();
            await Task.WhenAll(añosTask, networksTask);

            AñosMaestros    = añosTask.Result;
            NetworksMaestros = networksTask.Result;

            // C: El overlay se detiene al salir de InicializarPaginaAsync (lo hace ContextProtegido).
            //    Fase 2 se dispara en background — la página ya es visible para el usuario.
            _ = CargarMaestrosDependientesAsync();
        }

        private async Task CargarMaestrosDependientesAsync()
        {
            try
            {
                string codigosNetwork = ObtenerValoresSeleccionados<CodigoDescripcion, int>(NetworksMaestros, x => x.Codigo, ",");
                MediosMaestros = await PresupuestosService.ObtenerMediosPorNetWork(codigosNetwork);
                MediosFiltrados = DatosHelper.ClonarObjeto(MediosMaestros);

                await InvokeAsync(StateHasChanged);

                string codigosMedios = ObtenerValoresSeleccionados<CodigoDescripcion, int>(MediosFiltrados, x => x.Codigo, ",");

                // B: AgrupacionesComerciales + Editoriales en una sola query Oracle
                var (agrupaciones, editoriales) = await PresupuestosService.ObtenerAgrupacionesYEditoriales(codigosMedios);
                AgrupacionesComercialesMaestras = agrupaciones;
                EditorialesMaestras = editoriales;

                await InvokeAsync(StateHasChanged);

                await ManejarRequestAsync();
            }
            catch (Exception ex)
            {
                await ManejarExcepcion(ex, null);
            }
        }

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
        private async Task ComboBoxAño_CambioSeleccionAsync(SelectedDataItemChangedEventArgs<CodigoDescripcion> e)
        {
            if (e.DataItem == null) return;
            if (_filtroSobreprima == null || _filtroSobreprima.Anio == e.DataItem.Codigo) return;

            VersionesMaestras = [];
            _filtroSobreprima.Anio = e.DataItem.Codigo;
            _filtroSobreprima.CodigoVersion = null;
            VersionSeleccionada = null;

            await EjecutarAsync(async () =>
            {
                VersionesMaestras = await ObtenerVersionesPorPermisos(_filtroSobreprima.Anio);
            });
        }

        /// <summary>
        /// Actualiza la lista de medios cuando se modifican los networks seleccionados
        /// </summary>
        private async Task ActualizarMediosCuandoModificamosNetworksAsync(string codigosNetwork)
        {
            await EjecutarAsync(async () =>
            {
                MediosFiltrados = await PresupuestosService.ObtenerMediosPorNetWork(codigosNetwork);
                MediosSeleccionados = null;
            }, showOverlay: false);
        }

        /// <summary>
        /// Actualiza la lista de medios cuando se quitan todos los networks
        /// Restaura la lista completa de medios maestros
        /// </summary>
        private async Task ActualizarMediosCuandoQuitamosNetworksAsync()
        {
            await EjecutarAsync(() =>
            {
                MediosFiltrados = DatosHelper.ClonarObjeto(MediosMaestros);
                MediosSeleccionados = null;
            }, showOverlay: false);
        }

        /// <summary>
        /// Actualiza las editoriales cuando se quitan todos los medios
        /// </summary>
        private async Task ActualizarEditorialesCuandoQuitamosMediosAsync()
        {
            await EjecutarAsync(async () =>
            {
                string codigosMedios = ObtenerValoresSeleccionados<CodigoDescripcion, int>(MediosFiltrados, x => x.Codigo, ",");
                AgrupacionesComercialesMaestras = await PresupuestosService.ObtenerAgrupacionesComerciales(codigosMedios);
                await ComprobarAgrupacionesYEditorialesAsync(codigosMedios);
            }, showOverlay: false);
        }

        /// <summary>
        /// Actualiza las editoriales cuando se quitan todas las agrupaciones comerciales
        /// </summary>
        private async Task ActualizarEditorialesCuandoQuitamosAgrupacionesAsync()
        {
            await EjecutarAsync(async () =>
            {
                FiltroEditoriales filtro = new();
                if (MediosSeleccionadosBacking != null)
                {
                    filtro.CodigosMedios = ObtenerValoresSeleccionados<CodigoDescripcion, int>(MediosSeleccionadosBacking, x => x.Codigo, ",");
                }
                else
                {
                    filtro.CodigosMedios = ObtenerValoresSeleccionados<CodigoDescripcion, int>(MediosFiltrados, x => x.Codigo, ",");
                }

                EditorialesMaestras = await PresupuestosService.ObtenerEditoriales(filtro);
                EditorialesSeleccionadas = null;
                StateHasChanged();
            }, showOverlay: false);
        }

        /// <summary>
        /// Maneja el cambio de networks seleccionados en el ListBox
        /// Actualiza la lista de medios disponibles
        /// </summary>
        private async Task ListBoxNetworks_CambioValoresAsync(IEnumerable<CodigoDescripcion> values, IDropDownBox dropDownBox, bool esSingle)
        {
            dropDownBox.BeginUpdate();
            dropDownBox.Value = values;

            if (!values.Any())
            {
                await ActualizarMediosCuandoQuitamosNetworksAsync();
            }

            dropDownBox.EndUpdate();
            if (esSingle)
                dropDownBox.HideDropDown();
        }

        private async Task OnMediosChangedAsync(
            IEnumerable<CodigoDescripcion> medios,
            IDropDownBox dropDownBox)
        {
            dropDownBox.BeginUpdate();

            try
            {
                dropDownBox.Value = medios;

                await ActualizarSeleccionMediosAsync(medios);
            }
            finally
            {
                dropDownBox.EndUpdate();
            }
        }

        private async Task OnMedioSeleccionadoAsync(
            IEnumerable<CodigoDescripcion> medios,
            IDropDownBox dropDownBox)
        {
            await OnMediosChangedAsync(medios, dropDownBox);

            dropDownBox.HideDropDown();
        }

        private async Task ActualizarSeleccionMediosAsync(
            IEnumerable<CodigoDescripcion> medios)
        {
            if (!medios.Any())
            {
                LimpiarSeleccionMedios();
                return;
            }

            await EjecutarAsync(() =>
                ActualizarAgrupacionesComercialesAsync(medios));
        }

        private async Task ActualizarAgrupacionesComercialesAsync(
            IEnumerable<CodigoDescripcion> medios)
        {
            string codigosMedios = ObtenerCodigosMedios(medios);

            AgrupacionesComercialesMaestras =
                await PresupuestosService.ObtenerAgrupacionesComerciales(codigosMedios);

            await ComprobarAgrupacionesYEditorialesAsync(codigosMedios);
        }

        private string ObtenerCodigosMedios(
            IEnumerable<CodigoDescripcion> medios)
        {
            return ObtenerValoresSeleccionados<CodigoDescripcion, int>(
                medios,
                medio => medio.Codigo,
                ",");
        }

        private void LimpiarSeleccionMedios()
        {
            MediosSeleccionados = null;
            AgrupacionesComercialesMaestras = [];
        }

        /// <summary>
        /// Comprueba y ajusta las agrupaciones y editoriales seleccionadas según los medios filtrados
        /// </summary>
        private async Task ComprobarAgrupacionesYEditorialesAsync(string codigosMedios)
        {
            FiltroEditoriales filtro = new() { CodigosMedios = codigosMedios };

            if (AgrupacionesComercialesSeleccionadas != null)
            {
                var lista = FiltrarPorSeleccionadas(
                    AgrupacionesComercialesMaestras,
                    AgrupacionesComercialesSeleccionadas as List<CodigoDescripcion> ?? []
                );

                AgrupacionesComercialesSeleccionadas = new List<CodigoDescripcion>(lista);

                filtro.CodigosAgrupacionesComerciales = ObtenerValoresSeleccionados<CodigoDescripcion, int>(
                    AgrupacionesComercialesSeleccionadas,
                    x => x.Codigo,
                    ","
                );
            }

            EditorialesMaestras = await PresupuestosService.ObtenerEditoriales(filtro);

            if (EditorialesSeleccionadas != null)
            {
                var lista = FiltrarPorSeleccionadas(
                    EditorialesMaestras,
                    EditorialesSeleccionadas as List<CodigoDescripcion> ?? []
                );

                EditorialesSeleccionadas = new List<CodigoDescripcion>(lista);
            }

            StateHasChanged();
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
        private List<CodigoDescripcion> FiltrarPorSeleccionadas(List<CodigoDescripcion> listaDondeBuscar, List<CodigoDescripcion> listaSeleccionadas)
        {
            return listaDondeBuscar.Where(item => listaSeleccionadas.Any(sel => sel.Codigo == item.Codigo)).ToList();
        }

        /// <summary>
        /// Maneja el cambio de agrupaciones comerciales seleccionadas en el ListBox
        /// Actualiza las editoriales disponibles
        /// </summary>
        private async Task ListBoxAgrupaciones_CambioValoresAsync(IEnumerable<CodigoDescripcion> values, IDropDownBox dropDownBox, bool esSingle)
        {
            dropDownBox.BeginUpdate();
            dropDownBox.Value = values;

            if (!values.Any())
            {
                FiltroEditoriales filtro = new() { CodigosMedios = ObtenerValoresSeleccionados<CodigoDescripcion, int>(MediosSeleccionadosBacking, x => x.Codigo, ",") };
                EditorialesMaestras = await PresupuestosService.ObtenerEditoriales(filtro);
            }
            else
            {
                await EjecutarAsync(async () =>
                {
                    FiltroEditoriales filtro = new()
                    {
                        CodigosAgrupacionesComerciales = ObtenerValoresSeleccionados<CodigoDescripcion, int>(values, x => x.Codigo, ","),
                        CodigosMedios = ObtenerValoresSeleccionados<CodigoDescripcion, int>(MediosSeleccionadosBacking, x => x.Codigo, ",")
                    };
                    EditorialesMaestras = await PresupuestosService.ObtenerEditoriales(filtro);
                    EditorialesSeleccionadas = null;
                });
            }

            dropDownBox.EndUpdate();
            if (esSingle)
                dropDownBox.HideDropDown();
        }

        private async Task OnAgrupacionesChangedAsync(
            IEnumerable<CodigoDescripcion> values,
            IDropDownBox dropDownBox)
        {
            dropDownBox.BeginUpdate();

            try
            {
                dropDownBox.Value = values;

                await ActualizarEditorialesPorAgrupacionesAsync(values);
            }
            finally
            {
                dropDownBox.EndUpdate();
            }
        }

        private async Task ActualizarEditorialesPorAgrupacionesAsync(IEnumerable<CodigoDescripcion> agrupaciones)
        {
            await EjecutarAsync(async () =>
            {
                var codigosMedios = ObtenerValoresSeleccionados<CodigoDescripcion, int>(
                    MediosSeleccionadosBacking,
                    x => x.Codigo,
                    ","
                );

                var codigosAgrupaciones = ObtenerValoresSeleccionados<CodigoDescripcion, int>(
                    agrupaciones,
                    x => x.Codigo,
                    ","
                );

                var filtro = new FiltroEditoriales
                {
                    CodigosMedios = codigosMedios,
                    CodigosAgrupacionesComerciales = codigosAgrupaciones
                };

                EditorialesMaestras = await PresupuestosService.ObtenerEditoriales(filtro);
                EditorialesSeleccionadas = null;
            }, showOverlay: false);
        }

        /// <summary>
        /// Maneja el cambio de editoriales seleccionadas en el ListBox
        /// </summary>
        private static void OnEditorialesChanged(IEnumerable<CodigoDescripcion> values, IDropDownBox dropDownBox)
        {
            dropDownBox.BeginUpdate();
            dropDownBox.Value = values;
            dropDownBox.EndUpdate();
        }


        /// <summary>
        /// Obtiene la lista de sobreprimas según los filtros aplicados
        /// Convierte la lista a SobreprimaGridModel para mostrar en el grid
        /// </summary>
        private async Task AplicarFiltroAsync()
        {
            if (!HayVersionSeleccionada())
            {
                await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(TextosApp.Mensajes.CamposObligatorios));
                return;
            }

            await EjecutarAsync(async () =>
            {
                SobreprimasGrid = [];

                _filtroSobreprima.Anio = AñoSeleccionado!.Codigo;
                _filtroSobreprima.CodigoVersion = VersionSeleccionada!.Codigo;
                _filtroSobreprima.CodigoNetworkList = ObtenerValoresSeleccionados<CodigoDescripcion, int>(NetworksSeleccionadosBacking, x => x.Codigo, ",");
                _filtroSobreprima.CodigoMedioList = ObtenerValoresSeleccionados<CodigoDescripcion, int>(MediosSeleccionadosBacking, x => x.Codigo, ",");
                _filtroSobreprima.CodigoAgrupacionComercialList = ObtenerValoresSeleccionados<CodigoDescripcion, int>(AgrupacionesComercialesSeleccionadasBacking, x => x.Codigo, ",");
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
                    SobreprimasGrid = await ConvertirSobreprimasEnModeloGridAsync(listaSobreprimas);
                    GridSobreprimas.SetFocusedRowIndex(0);
                }

                CaptionDerecha = $"[{AñoSeleccionado?.Descripcion ?? ""}, {VersionSeleccionada!.Descripcion}]";
            });

            _desdePaginaImportarMMS = false;
        }

        /// <summary>
        /// Limpia todos los filtros y reinicia el estado de la página
        /// </summary>
        private async Task LimpiarFiltroAsync()
        {
            await EjecutarAsync(() =>
            {
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
            });
        }
        #endregion

        #region Grid Sobreprimas - Eventos

        /// <summary>
        /// Maneja el doble clic en una fila del grid
        /// Abre el popup de edición si el medio es accesible
        /// </summary>
        private async Task GridSobreprimas_DobleClickAsync(GridRowClickEventArgs e)
        {
            var sobreprima = (SobreprimaGridModel?)GridSobreprimas.GetDataItem(e.VisibleIndex);

            if (sobreprima == null) return;

            if (sobreprima.MedioAccesible)
            {
                await MostrarPopupEdicionAsync(sobreprima, ModoOperacion.Modificar);
            }
        }

        /// <summary>
        /// Crea una nueva sobreprima y abre el popup de edición
        /// </summary>
        private async Task NuevaSobreprimaAsync()
        {
            await EjecutarAsync(async () =>
            {
                var nuevaSobreprima = new SobreprimaGridModel
                {
                    Anio = _filtroSobreprima!.Anio,
                    CodigoVersion = VersionSeleccionada!.Codigo
                };

                await MostrarPopupEdicionAsync(nuevaSobreprima, ModoOperacion.Insertar);
            }, showOverlay: false);
        }

        /// <summary>
        /// Muestra el popup de edición de sobreprima
        /// Inicializa las listas maestras del popup según el modo de operación
        /// </summary>
        private async Task MostrarPopupEdicionAsync(SobreprimaGridModel sobreprima, ModoOperacion modoOperacion)
        {
            await EjecutarAsync(async () =>
            {
                AgrupacionesComercialesPopup.Clear();
                EditorialesPopup.Clear();

                SobreprimaEnEdicion = DatosHelper.ClonarObjeto(sobreprima);
                SobreprimaOriginal = DatosHelper.ClonarObjeto(SobreprimaEnEdicion);
                SobreprimaEnEdicion.ModoOperacion = modoOperacion;

                if (modoOperacion == ModoOperacion.Insertar)
                {
                    if (NetworksMaestros != null && NetworksMaestros.Count == 1)
                    {
                        SobreprimaEnEdicion.CodigoNetwork = NetworksMaestros[0].Codigo;
                        MediosPopup = await PresupuestosService.ObtenerMediosPorNetWork(SobreprimaEnEdicion.CodigoNetwork.ToString());
                    }
                }
                else if (modoOperacion == ModoOperacion.Modificar)
                {
                    string codigosMedios = sobreprima.CodigoMedio?.ToString() ?? string.Empty;
                    string codigosAgrupaciones = sobreprima.CodigoAgrupacionComercial?.ToString() ?? string.Empty;

                    var mediosTask = PresupuestosService.ObtenerMediosPorNetWork(SobreprimaEnEdicion.CodigoNetwork.ToString());
                    var agrupacionesTask = PresupuestosService.ObtenerAgrupacionesComerciales(codigosMedios);
                    var editorialesTask = PresupuestosService.ObtenerEditoriales(new FiltroEditoriales
                    {
                        CodigosMedios = codigosMedios,
                        CodigosAgrupacionesComerciales = codigosAgrupaciones
                    });

                    await Task.WhenAll(mediosTask, agrupacionesTask, editorialesTask);

                    MediosPopup = mediosTask.Result;
                    AgrupacionesComercialesPopup = agrupacionesTask.Result;
                    EditorialesPopup = editorialesTask.Result;
                }

                TituloPopupEdicion = modoOperacion == ModoOperacion.Insertar
                    ? ObtenerTexto(TextosApp.Common.Nuevo)
                    : ObtenerTexto(TextosApp.Common.Edit);

                PopupEdicionVisible = true;
            });
        }

        /// <summary>
        /// Elimina una sobreprima previa confirmación del usuario
        /// </summary>
        private async Task EliminarSobreprimaAsync(SobreprimaGridModel sobreprima)
        {
            if (sobreprima == null) return;

            if (!await MensajesHelper.MostrarMensajeParaConfirmacion(TituloPagina, ObtenerTexto(TextosApp.Mensajes.ConfirmacionEliminar))) return;

            await EjecutarAsync(async () =>
            {
                await SobreprimasService.EliminarSobreprimas(sobreprima);
                SobreprimasGrid.Remove(sobreprima);
                await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(TextosApp.Mensajes.RegistroEliminado));
            });
        }

        /// <summary>
        /// Oculta el popup de edición de sobreprima
        /// </summary>
        private void OcultarPopupEdicion()
        {
            PopupEdicionVisible = false;
        }

        #endregion

        #region Popup Edición - Eventos ComboBox

        /// <summary>
        /// Maneja el cambio de agrupación comercial en el popup
        /// Actualiza la lista de editoriales disponibles
        /// </summary>
        private async Task PopupComboBoxAgrupacion_CambioSeleccionAsync(SelectedDataItemChangedEventArgs<CodigoDescripcion> e)
        {
            if (e.ChangeSource != SelectionChangeSource.UserAction) return;

            await EjecutarAsync(async () =>
            {
                FiltroEditoriales filtro = new() { CodigosMedios = SobreprimaEnEdicion.CodigoMedio?.ToString() ?? string.Empty };

                if (e.DataItem != null)
                {
                    filtro.CodigosAgrupacionesComerciales = e.DataItem.Codigo.ToString();
                    EditorialesPopup = await PresupuestosService.ObtenerEditoriales(filtro);
                    SobreprimaEnEdicion.DescripcionAgrupacionComercial = e.DataItem.Descripcion;
                    SobreprimaEnEdicion.CodigoEditorial = null;
                }
                else
                {
                    EditorialesPopup = await PresupuestosService.ObtenerEditoriales(filtro);
                    SobreprimaEnEdicion.DescripcionAgrupacionComercial = string.Empty;
                }
            });
        }

        /// <summary>
        /// Maneja el cambio de network en el popup
        /// Actualiza la lista de medios disponibles
        /// </summary>
        private async Task PopupComboBoxNetwork_CambioSeleccionAsync(SelectedDataItemChangedEventArgs<CodigoDescripcion> e)
        {
            if (e.ChangeSource != SelectionChangeSource.UserAction || e.DataItem == null) return;

            await EjecutarAsync(async () =>
            {
                MediosPopup = await PresupuestosService.ObtenerMediosPorNetWork(e.DataItem.Codigo.ToString());
            });
        }

        /// <summary>
        /// Maneja el cambio de medio en el popup
        /// Actualiza las listas de agrupaciones comerciales y editoriales disponibles
        /// </summary>
        private async Task PopupComboBoxMedio_CambioSeleccionAsync(SelectedDataItemChangedEventArgs<CodigoDescripcion> e)
        {
            if (e.ChangeSource != SelectionChangeSource.UserAction || e.DataItem == null) return;

            await EjecutarAsync(async () =>
            {
                string codigoMedio = e.DataItem.Codigo.ToString();
                AgrupacionesComercialesPopup = await PresupuestosService.ObtenerAgrupacionesComerciales(codigoMedio);

                FiltroEditoriales filtro = new() { CodigosMedios = codigoMedio };
                EditorialesPopup = await PresupuestosService.ObtenerEditoriales(filtro);
            });
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
            if (sobreprimaGrid.KeyGrid != SobreprimaOriginal.KeyGrid)
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
                if (sobreprimaGrid.ConceptoDefaul.Porcentaje != SobreprimaOriginal.ConceptoDefaul.Porcentaje)
                {
                    var sobreprimaDefault = CrearSobreprimaBase(sobreprimaGrid);
                    sobreprimaDefault.Codigo = sobreprimaGrid.ConceptoDefaul.Codigo;
                    sobreprimaDefault.Porcentaje = sobreprimaGrid.ConceptoDefaul.Porcentaje;
                    sobreprimaDefault.CodigoConcepto = sobreprimaGrid.ConceptoDefaul.CodigoConcepto;
                    lista.Add(sobreprimaDefault);
                }

                if (sobreprimaGrid.ConceptoSLA.Porcentaje != SobreprimaOriginal.ConceptoSLA.Porcentaje)
                {
                    var sobreprimaSLA = CrearSobreprimaBase(sobreprimaGrid);
                    sobreprimaSLA.Codigo = sobreprimaGrid.ConceptoSLA.Codigo;
                    sobreprimaSLA.Porcentaje = sobreprimaGrid.ConceptoSLA.Porcentaje;
                    sobreprimaSLA.CodigoConcepto = sobreprimaGrid.ConceptoSLA.CodigoConcepto;
                    lista.Add(sobreprimaSLA);
                }

                if (sobreprimaGrid.ConceptoHVP.Porcentaje != SobreprimaOriginal.ConceptoHVP.Porcentaje)
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
        private async Task<bool> SobreprimaEstaDuplicadaAsync(SobreprimaGridModel sobreprima)
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
        private async Task GuardarSobreprimaAsync()
        {
            bool grabacionExitosa = false;
            try
            {
                bool noHayCambios = DatosHelper.SonIguales(SobreprimaEnEdicion, SobreprimaOriginal);

                if (noHayCambios)
                {
                    await MensajesHelper.MostrarMensajeAviso(TituloPagina, ObtenerTexto(TextosApp.Mensajes.SinCambios));
                    return;
                }

                LayerOverlayService.Start();

                // Validar campos obligatorios
                var validaciones = new List<(bool Condicion, string ResourceKey)>
                {
                    (SobreprimaEnEdicion.CodigoNetwork == 0, TextosApp.Common.Network),
                    (SobreprimaEnEdicion.CodigoMedio == null, TextosApp.Common.Medio),
                    (SobreprimaEnEdicion.CodigoEditorial == null, TextosApp.Common.Editorial),
                    (SobreprimaEnEdicion.ModoOperacion == ModoOperacion.Insertar
                        && SobreprimaEnEdicion.ConceptoDefaul.Porcentaje == 0
                        && SobreprimaEnEdicion.ConceptoSLA.Porcentaje == 0
                        && SobreprimaEnEdicion.ConceptoHVP.Porcentaje == 0,
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
                if (SobreprimaEnEdicion.ModoOperacion == ModoOperacion.Insertar
                    && SobreprimasGrid.Find(x => x.KeyGrid == SobreprimaEnEdicion.KeyGrid) != null)
                {
                    await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Mensajes.SobreprimaDuplicated));
                    return;
                }

                // Validar duplicados en lista local (edición)
                if (SobreprimaEnEdicion.ModoOperacion != ModoOperacion.Insertar
                    && SobreprimasGrid.Find(x => x.KeyGrid == SobreprimaEnEdicion.KeyGrid
                                                && x.Codigo != SobreprimaEnEdicion.Codigo) != null)
                {
                    await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Mensajes.SobreprimaDuplicated));
                    return;
                }

                // Validar duplicados en base de datos
                if (await SobreprimaEstaDuplicadaAsync(SobreprimaEnEdicion))
                {
                    await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Mensajes.SobreprimaDuplicated));
                    return;
                }

                // Actualizar descripciones para mostrar en el grid
                var network = NetworksMaestros.FirstOrDefault(x => x.Codigo == SobreprimaEnEdicion.CodigoNetwork);
                SobreprimaEnEdicion.DescripcionNetwork = network?.Descripcion ?? "";

                var medio = MediosFiltrados.FirstOrDefault(x => x.Codigo == SobreprimaEnEdicion.CodigoMedio);
                SobreprimaEnEdicion.DescripcionMedio = medio?.Descripcion ?? "";

                var agrupacion = AgrupacionesComercialesMaestras.FirstOrDefault(x => x.Codigo == SobreprimaEnEdicion.CodigoAgrupacionComercial);
                SobreprimaEnEdicion.DescripcionAgrupacionComercial = agrupacion?.Descripcion ?? "";

                var editorial = EditorialesMaestras.FirstOrDefault(x => x.Codigo == SobreprimaEnEdicion.CodigoEditorial);
                SobreprimaEnEdicion.DescripcionEditorial = editorial?.Descripcion ?? "";

                SobreprimaEnEdicion.CodigoPais = Usuario!.CodigoPais;

                // Convertir y guardar
                List<Sobreprima> sobreprimas = ConvertirModeloGridEnSobreprimas(SobreprimaEnEdicion);
                await SobreprimasService.GrabarSobreprimas(sobreprimas);

                grabacionExitosa = true; // Marcamos que la grabación fue correcta

                // Actualizar códigos de conceptos insertados (devueltos por el servicio)
                foreach (var sobreprima in sobreprimas)
                {
                    switch (sobreprima.CodigoConcepto)
                    {
                        case (int)ConceptosSobreprimas.Sobreprima:
                            SobreprimaEnEdicion.ConceptoDefaul.Codigo = sobreprima.Codigo;
                            break;
                        case (int)ConceptosSobreprimas.SLA:
                            SobreprimaEnEdicion.ConceptoSLA.Codigo = sobreprima.Codigo;
                            break;
                        case (int)ConceptosSobreprimas.HVP:
                            SobreprimaEnEdicion.ConceptoHVP.Codigo = sobreprima.Codigo;
                            break;
                    }
                }

                // Actualizar grid según modo de operación
                if (SobreprimaEnEdicion.ModoOperacion == ModoOperacion.Insertar)
                {
                    // Obtener el código mayor para asignar a la nueva fila
                    int codigoMayor = 0;
                    if (SobreprimasGrid.Count > 0)
                    {
                        codigoMayor = SobreprimasGrid.Max(x => x.Codigo);
                    }

                    SobreprimaEnEdicion.Codigo = codigoMayor + 1;

                    SobreprimasGrid.Insert(0, SobreprimaEnEdicion);
                    GridSobreprimas.SetFocusedRowIndex(0);
                    await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(TextosApp.Common.DatosGrabados));
                }
                else
                {
                    // Actualizar la lista del grid
                    var indice = SobreprimasGrid.FindIndex(x => x.Codigo == SobreprimaEnEdicion.Codigo);
                    if (indice >= 0)
                    {
                        // Si todos los porcentajes son 0, se habrá eliminado de BD
                        bool todosLosConceptosEnCero =
                            SobreprimaEnEdicion.ConceptoDefaul.Porcentaje == 0 &&
                            SobreprimaEnEdicion.ConceptoSLA.Porcentaje == 0 &&
                            SobreprimaEnEdicion.ConceptoHVP.Porcentaje == 0;

                        if (todosLosConceptosEnCero)
                        {
                            SobreprimasGrid.RemoveAt(indice);
                        }
                        else
                        {
                            SobreprimasGrid[indice] = SobreprimaEnEdicion;
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
                    await AplicarFiltroAsync();
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
        private async Task<List<SobreprimaGridModel>> ConvertirSobreprimasEnModeloGridAsync(List<Sobreprima> listaSobreprimas)
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
                        DescripcionMedio = MediosMaestros.FirstOrDefault(o => o.Codigo == g.Key.CodigoMedio)?.Descripcion ?? string.Empty,
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
        private async Task ManejarRequestAsync()
        {
            await EjecutarAsync(async () =>
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

                    var codigosListaNetworks = listaNetworks.Select(n => n.Codigo).OrderBy(c => c);
                    var codigosMasterNetwork = NetworksMaestros.Select(n => n.Codigo).OrderBy(c => c);
                    bool sonIguales = codigosListaNetworks.SequenceEqual(codigosMasterNetwork);

                    NetworksSeleccionados = sonIguales
                        ? Enumerable.Empty<CodigoDescripcion>()
                        : listaNetworks;

                    var listaMedios = MediosFiltrados.Where(n => filtro.CodigosMedio.Contains(n.Codigo)).ToList();

                    var codigosListaMedios = listaMedios.Select(n => n.Codigo).OrderBy(c => c);
                    var codigosMedioFiltro = MediosFiltrados.Select(n => n.Codigo).OrderBy(c => c);
                    bool sonIgualesMedios = codigosListaMedios.SequenceEqual(codigosMedioFiltro);

                    MediosSeleccionados = sonIgualesMedios
                        ? Enumerable.Empty<CodigoDescripcion>()
                        : listaMedios;

                    StateHasChanged();

                    if (AñoSeleccionado != null && NetworksSeleccionados != null && VersionSeleccionada != null && MediosSeleccionados != null)
                    {
                        _desdePaginaImportarMMS = true;
                        await AplicarFiltroAsync();
                    }
                }
                else
                {
                    InicializarFiltro();
                }
            }, showOverlay: false);
        }
        #endregion
    }
}
