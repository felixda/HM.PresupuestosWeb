using HM.Presupuestos.Application.CasosDeUso.Compartido;
using static HM.Presupuestos.Application.CasosDeUso.CondicionesService;

namespace HM.Presupuestos.Web.Pages.Condiciones
{
    public partial class PlanificacionCondiciones
    {
        #region Inyecci?n de Dependencias

        [Inject] protected IMaestrosCacheService PresupuestosService { get; set; } = default!;
        [Inject] protected ICondicionesService CondicionesService { get; set; } = default!;
        [Inject] protected ParametrosNavegacion NavegacionService { get; set; } = default!;

        #endregion
        #region Propiedades privadas
        private bool _desdePaginaImportarMMS = false;


        #region Page

        private string TextoToolTipAyuda { get; set; } = string.Empty;
        private string TituloActual => string.IsNullOrEmpty(RadioGroupAcuerdoChecked) ? TituloPagina : RadioGroupAcuerdoChecked;

        private bool HayCambiosPendientes => CondicionesNoGuardados.Count > 0 || ExcepcionesNoGuardadas.Count > 0;

        #endregion

        #region Filtro

        private string RadioGroupAcuerdoChecked { get; set; } = string.Empty;
        private IEnumerable<string>? RadioGroupAcuerdoButtonList { get; set; }
        
        private CondicionFiltro _filtroCondiciones = new();

        private List<CodigoDescripcion> Networks { get; set; } = [];
        private CodigoDescripcion? NetworkSeleccionado { get; set; } = null;

        private List<CodigoDescripcion> GruposClientes { get; set; } = [];
        private int? CodigoGrupoSeleccionado { get; set; }

        private List<CodigoDescripcion> Anios { get; set; } = [];
        private CodigoDescripcion? AnioSeleccionado { get; set; } = null;

        private List<VersionResumen> Versiones { get; set; } = [];
        private VersionResumen? VersionSeleccionada { get; set; } = null;

        private List<CodigoDescripcion> MediosMaster { get; set; } = [];

        #endregion

        #region Vigencias

        private List<Vigencia> Vigencias { get; set; } = []; //Todas las vigencias encontradas
        private Vigencia? VigenciaSeleccionada { get; set; } = null;

        private string LeftCaptionVigencias = string.Empty;
        private string RightCaptionVigencias = string.Empty;

        private string LeftCaptionCondiciones = string.Empty;
        private string RightCaptionCondiciones = string.Empty;

        


        #endregion

        #region Grid Condiciones

        private DxGrid GridCondiciones { get; set; } = new DxGrid();
        private List<CondicionViewModel> Condiciones { get; set; } = [];
        private List<CondicionViewModel> _condicionesCache = [];

        private List<CodigoDescripcion> IndicadoresDevolucion { get; set; } = [];
        private Dictionary<CondicionViewModel, DatosCondicionCambiados> CondicionesNoGuardados { get; } = [];

        #endregion

        #region Grid Excepciones

        private DxGrid GridExcepciones { get; set; } = new DxGrid();
        private List<ExcepcionCondicionViewModel> Excepciones { get; set; } = [];
        private List<ExcepcionCondicionViewModel> _excepcionesCache = [];

        private List<CodigoDescripcion> Alcances { get; set; } = [];
        private List<CodigoDescripcion> Disciplinas { get; set; } = [];
        private List<CodigoDescripcion> Diversifieds { get; set; } = [];
        private List<CodigoDescripcion> Objetivos { get; set; } = [];
        private List<CodigoDescripcion> TiposCompra { get; set; } = [];
        private List<CodigoDescripcion> TiposDisciplina { get; set; } = [];
        private List<CodigoDescripcion> DisciplinasGrupo { get; set; } = [];
        private Dictionary<ExcepcionCondicionViewModel, DatosExcepcionesCondicionCambiados> ExcepcionesNoGuardadas { get; } = [];

        private string TituloGridExcepciones { get; set; } = string.Empty;

        // Cach? para optimizar b?squedas de condiciones por medio
        private Dictionary<int, List<CondicionViewModel>>? _condicionesPorMedio;

        private enum AccionJerarquias
        {
            Subir,
            Bajar,
        }

        private bool ActivarBotonNuevaExcepcion { get; set; } = true;

        #endregion


        #region Popup Vigencia

        private bool PopupVigenciaVisible { get; set; } = false;
        private string TituloPopupVigencia { get; set; } = string.Empty;
        private Vigencia _vigenciaNueva = new();
        private List<CodigoDescripcion> Meses { get; set; } = [];
        private CodigoDescripcion? MesDesdeVigenciaSeleccionado { get; set; } = null;
        private CodigoDescripcion? MesHastaVigenciaSeleccionado { get; set; } = null;
        private CodigoDescripcion? MedioSeleccionadoDesdePopup { get; set; } = null;
        private ModoEdicion _modoEdicionVigencia = ModoEdicion.Alta;

        #endregion

        #region Popup Medios

        private bool PopupMediosVisible { get; set; } = false;
        private List<CodigoDescripcion> _medios = [];
        private int? _codigoMedioSeleccionadoParaFiltro;

        #endregion


        #endregion


        #region Ciclo de Vida del Componente

        protected override Task OnPermisoDenegadoAsync()
        {
            return Task.CompletedTask;
        }

        protected override async Task InicializarPaginaAsync()
        {
            InicializarTextos();

            InicializarMeses();

            InicializarIndicadoresDevolucion();

            await CargarCatalogosAsync();

            await ManejarRequest();
        }

        private void InicializarTextos()
        {
            RadioGroupAcuerdoButtonList = [ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Condiciones),
                ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.CondicionesAcuerdos)];
            RadioGroupAcuerdoChecked = ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Condiciones);

            TextoToolTipAyuda = ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.ToolTip);

            LeftCaptionVigencias = ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Vigencias);
            LeftCaptionCondiciones = ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Condiciones);

            TituloGridExcepciones = ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Excepciones);
        }

        private void InicializarMeses()
        {
            Meses = GestorIdioma.ObtenerMeses();
        }

        private void InicializarIndicadoresDevolucion()
        {
            IndicadoresDevolucion.Add(new CodigoDescripcion { Codigo = 0, Descripcion = "" });
            IndicadoresDevolucion.Add(new CodigoDescripcion { Codigo = 1, Descripcion = ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.NetoVenta) });
            IndicadoresDevolucion.Add(new CodigoDescripcion { Codigo = 2, Descripcion = ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Sobreprima) });
        }

        private async Task CargarCatalogosAsync()
        {
            var taskAnios = VersionesService.ObtenerAniosConVersiones();
            var taskNetworks = PresupuestosService.ObtenerNetworks();
            var taskAlcances = PresupuestosService.ObtenerAlcances();
            var taskDisciplinas = PresupuestosService.ObtenerDisciplinas();
            var taskDiversifieds = PresupuestosService.ObtenerDiversifiedsNCB();
            var taskObjetivos = PresupuestosService.ObtenerObjetivos();
            var taskTiposCompra = PresupuestosService.ObtenerTiposCompra();
            var taskDisciplinasGrupo = PresupuestosService.ObtenerDisciplinasGrupos();
            var taskTiposDisciplina = PresupuestosService.ObtenerTiposDisciplinas();

            await Task.WhenAll(taskAnios, taskNetworks, taskAlcances, taskDisciplinas, taskDiversifieds, taskObjetivos, taskTiposCompra, taskDisciplinasGrupo, taskTiposDisciplina);

            Anios = taskAnios.Result;
            Networks = taskNetworks.Result;

            Alcances         = PrepararCatalogo(taskAlcances.Result);
            Disciplinas      = PrepararCatalogo(taskDisciplinas.Result);
            Diversifieds     = PrepararCatalogo(taskDiversifieds.Result);
            Objetivos        = PrepararCatalogo(taskObjetivos.Result);
            TiposCompra      = PrepararCatalogo(taskTiposCompra.Result);
            DisciplinasGrupo = PrepararCatalogo(taskDisciplinasGrupo.Result);
            TiposDisciplina  = PrepararCatalogo(taskTiposDisciplina.Result);
        }

        #endregion


        #region Filtro

        /// <summary>
        /// Inicializa los valores del filtro
        /// </summary>
        private async Task FilterInitAsync()
        {
            if (Networks.Count == 1)
            {
                NetworkSeleccionado = Networks[0];
                GruposClientes = await PresupuestosService.ObtenerGruposClientePorNetwork(NetworkSeleccionado.Codigo);
                MediosMaster = await PresupuestosService.ObtenerMediosPorNetWork(NetworkSeleccionado.Codigo.ToString());
            }
            if ( GruposClientes.Count == 1)
            {
                CodigoGrupoSeleccionado = GruposClientes[0].Codigo;
            }
        }

        private async Task OnRadioGroupAcuerdoValueChangedAsync(string newValue)
        {
            if (!await ComprobarSiHayCambiosPendienteAndSeguir())
            {
                return;
            }
            RadioGroupAcuerdoChecked = newValue;
        }

        private async Task OnComboNetworkSelectedDataItemChangedAsync(SelectedDataItemChangedEventArgs<CodigoDescripcion> e)
        {
            if (e.ChangeSource == SelectionChangeSource.UserAction)
            {
                CodigoGrupoSeleccionado = null;
                GruposClientes = [];
                NetworkSeleccionado = e.DataItem;
                if (NetworkSeleccionado is null) return;

                try
                {
                    LayerOverlayService.Start();
                    GruposClientes = await PresupuestosService.ObtenerGruposClientePorNetwork(NetworkSeleccionado.Codigo);
                    if (GruposClientes.Count == 1)
                    {
                        CodigoGrupoSeleccionado = GruposClientes[0].Codigo;
                    }

                    MediosMaster = await PresupuestosService.ObtenerMediosPorNetWork(NetworkSeleccionado.Codigo.ToString());
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

        private async Task OnComboAniosSelectedDataItemChangedAsync(SelectedDataItemChangedEventArgs<CodigoDescripcion> e)
        {
            if (e.ChangeSource == SelectionChangeSource.UserAction)
            {
                VersionSeleccionada = null;
                Versiones = [];
                if (e.DataItem is null) return;

                int anioSeleccionado = e.DataItem.Codigo;
                try
                {
                    LayerOverlayService.Start();
                    Versiones = await ObtenerVersionesPorPermisos(anioSeleccionado);
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

        private async Task OnComboVigenciasSelectedDataItemChangedAsync(SelectedDataItemChangedEventArgs<Vigencia> e)
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
                    Condiciones.Clear();
                    _condicionesCache.Clear();
                    Excepciones.Clear();
                    _excepcionesCache.Clear();

                    TituloGridExcepciones = ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Excepciones);
                    _codigoMedioSeleccionadoParaFiltro = null;
                }
            }
            catch (Exception ex)
            {
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina);
            }
        }


        private void ActualizarCondicionesConMedioAccesible()
        {
            var codigosMediosAccesibles = new HashSet<int>(
                   MediosMaster.Select(m => m.Codigo));

            foreach (var item in Condiciones)
            {
                item.MedioAccesible = codigosMediosAccesibles.Contains(item.CodigoMedio);
            }

            Condiciones.RemoveAll(c => c.PctSAG == 0 && c.PctDevolucion == 0 && c.PctManPower == 0 && c.MedioAccesible == false);
        }

        private void ActualizarExcepcionesConMedioAccesible()
        {
            var codigosMediosAccesibles = new HashSet<int>(
                MediosMaster.Select(m => m.Codigo));
            foreach (var item in Excepciones)
            {
                item.MedioAccesible = codigosMediosAccesibles.Contains(item.CodigoMedio);
            }
        }
        private async Task ObtenerCondicionesExcepciones(int codigoVigencia)
        {
            try
            {
                LayerOverlayService.Start();
                var condicionesDto = await CondicionesService.ObtenerCondicionesPorVigencia(codigoVigencia, NetworkSeleccionado!.Codigo);
                Condiciones = [.. condicionesDto.Select(dto => new CondicionViewModel
                {
                    CodigoMedio = dto.CodigoMedio,
                    DescripcionMedio = StringHelper.Capitalize(dto.DescripcionMedio),
                    PctSAG = dto.PctSAG,
                    PctManPower = dto.PctManPower,
                    PctDevolucion = dto.PctDevolucion,
                    IndicadorCalculoDevolucion = dto.IndicadorCalculoDevolucion,
                    NumeroExcepciones = dto.NumeroExcepciones
                })];

                ActualizarCondicionesConMedioAccesible();

                _condicionesCache = DatosHelper.ClonarObjeto(Condiciones);
                _condicionesPorMedio = null; // Limpiar cach?

                _medios = [.. Condiciones
                                .Select(x => new CodigoDescripcion
                                {
                                    Codigo = x.CodigoMedio,
                                    Descripcion = x.DescripcionMedio
                                })];

                await ObtenerExcepciones(codigoVigencia);
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

        private async Task ObtenerExcepciones(int codigoVigencia)
        {
            var excepcionesDto = await CondicionesService.ObtenerExcepcionesCondiciones(codigoVigencia);
            Excepciones = [.. excepcionesDto.Select(dto => new ExcepcionCondicionViewModel
            {
                CodigoMedio = dto.CodigoMedio,
                Jerarquia = dto.Jerarquia,
                CodigoCondicionMedio = dto.CodigoCondicionMedio,
                DescripcionMedio = StringHelper.Capitalize(dto.DescripcionMedio),
                PctSAG = dto.PctSAG,
                PctManPower = dto.PctManPower,
                PctDevolucion = dto.PctDevolucion,
                IndicadorCalculoDevolucion = dto.IndicadorCalculoDevolucion,
                CodigoAlcance = dto.CodigoAlcance,
                CodigoDisciplina = dto.CodigoDisciplina,
                CodigoDiversified = dto.CodigoDiversified,
                CodigoObjetivo = dto.CodigoObjetivo,
                CodigoTipoCompra = dto.CodigoTipoCompra,
                CodigoTipoDisciplina = dto.CodigoTipoDisciplina,
                CodigoDisciplinaGrupo = dto.CodigoDisciplinaGrupo
            })];

            //Actualiza el indicador de Base de calculo de las excepciones a partir del indicador de Base de calculo de la Condicion del mismo Medio
            foreach (CondicionViewModel condicion in Condiciones)
            {
                int indicadorCalculoDevolucion = condicion.IndicadorCalculoDevolucion;
                Excepciones.Where(c => c.CodigoMedio == condicion.CodigoMedio).ToList().ForEach(c => c.IndicadorCalculoDevolucion = indicadorCalculoDevolucion);
            }

            ActualizarExcepcionesConMedioAccesible();

            _excepcionesCache = DatosHelper.ClonarObjeto(Excepciones);
        }

        private async Task FiltroBuscar()
        {
            if (!await ComprobarSiHayCambiosPendienteAndSeguir())  return;

            if (!ValidarCamposObligatoriosFiltro())
            {
                await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(TextosApp.Mensajes.CamposObligatorios));
                return;
            }
            try
            {
                LayerOverlayService.Start();
                _filtroCondiciones.CodigoNetwork = NetworkSeleccionado!.Codigo;

                var grupoSeleccionado = GruposClientes.First(c=> c.Codigo == CodigoGrupoSeleccionado!.Value);
                _filtroCondiciones.CodigoGrupoCliente = CodigoGrupoSeleccionado!.Value;
                _filtroCondiciones.Anio = Convert.ToInt32(AnioSeleccionado!.Descripcion);
                _filtroCondiciones.CodigoVersion = VersionSeleccionada!.Codigo;

                if (RadioGroupAcuerdoChecked == ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.CondicionesAcuerdos))
                {
                    _filtroCondiciones.IndicadorAcuerdo = Convert.ToInt32(ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.CondicionesAcuerdosCodigo));
                }
                else if (RadioGroupAcuerdoChecked == ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Condiciones))
                { 
                    _filtroCondiciones.IndicadorAcuerdo = Convert.ToInt32(ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.CondicionesCodigo));
                }

                Vigencias = await CondicionesService.ObtenerVigencias(_filtroCondiciones);
                
                LayerOverlayService.Stop();
                if (Vigencias.Count == 0)
                {
                    VigenciaSeleccionada = null;
                    if (!_desdePaginaImportarMMS)
                    {
                        await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(TextosApp.Mensajes.RegistrosNoEncontrados));
                    }
                }
                else
                {
                    GestorIdioma.TraducirMesesVigencias(Vigencias);
                    SeleccionarVigencia();
                }

                TituloGridExcepciones = ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Excepciones);
                _codigoMedioSeleccionadoParaFiltro = null;

                RightCaptionVigencias = $"[{NetworkSeleccionado?.Descripcion ?? ""}, {grupoSeleccionado!.Descripcion}, {Convert.ToInt32(AnioSeleccionado!.Descripcion)}, {VersionSeleccionada!.Descripcion}]";

                ActivarBotonNuevaExcepcion = true;

            }
            catch (Exception ex)
            {
                await RegistroAplicacion.RegistrarExcepcion(ex);
            }
            finally
            {
                LayerOverlayService.Stop();
                _desdePaginaImportarMMS = false;
            }
        }

        private bool ValidarCamposObligatoriosFiltro()
        {
            return NetworkSeleccionado != null
                && CodigoGrupoSeleccionado != null
                && AnioSeleccionado != null
                && VersionSeleccionada != null;
        }

        private async Task FiltroLimpiar()
        {
            try
            {
                if (!await ComprobarSiHayCambiosPendienteAndSeguir()) return;

                LayerOverlayService.Start();
                AnioSeleccionado = null;
                VersionSeleccionada = null;
                CodigoGrupoSeleccionado = null;
                _codigoMedioSeleccionadoParaFiltro = null;

                //Limpiar vigencias. aL poner esta a null, se limpia automaticamente las condiciones y las excepciones
                Vigencias = [];
                VigenciaSeleccionada = null;

                await FilterInitAsync();
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


        #region Vigencias

        private void SeleccionarVigencia()
        {
            if (Vigencias.Count == 0)
            {
                VigenciaSeleccionada = null;
                return;
            }

            if (Vigencias.Count == 1)
            {
                VigenciaSeleccionada = Vigencias[0];
            }
            else
            {
                //Se usa el operador ^ al calcular un ?ndice desde el final de una colecci?n.
                VigenciaSeleccionada = Vigencias[^1];
            }
        }

        private void AñadirVigencia()
        {
            _vigenciaNueva.CodigoNetWork = _filtroCondiciones.CodigoNetwork;
            _vigenciaNueva.CodigoVersion = _filtroCondiciones.CodigoVersion;
            _vigenciaNueva.CodigoGrupoCliente = _filtroCondiciones.CodigoGrupoCliente;
            _vigenciaNueva.IndicadorAcuerdo = _filtroCondiciones.IndicadorAcuerdo;

            TituloPopupVigencia =  ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.NuevaVigencia);

            _modoEdicionVigencia = ModoEdicion.Alta;
            PopupVigenciaVisible = true;
        }

        private async Task EditarVigencia()
        {
            if (VigenciaSeleccionada == null)
            {
                await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.VigenciaSeleccion));
                return;
            }

            int codigoVigencia = VigenciaSeleccionada.Codigo;

            Vigencia? vigencia = Vigencias.Find(c=> c.Codigo == codigoVigencia);

            if (vigencia == null)
            {
                return;
            }

            CodigoDescripcion mesDesde = Meses[vigencia.MesDesde - 1];
            CodigoDescripcion mesHasta = Meses[vigencia.MesHasta - 1];

            MesDesdeVigenciaSeleccionado = mesDesde;
            MesHastaVigenciaSeleccionado = mesHasta;

            TituloPopupVigencia = ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.EditarVigencia);

            _modoEdicionVigencia = ModoEdicion.Edicion;
            PopupVigenciaVisible = true;
        }

        private async Task EliminarVigencia()
        {
            bool confirm = await MensajesHelper.MostrarMensajeParaConfirmacion(TituloPagina, ObtenerTexto(TextosApp.Mensajes.ConfirmacionEliminar));
            if (!confirm) return;

            try
            {
                int codigoVigencia = VigenciaSeleccionada!.Codigo;

                if (await CondicionesService.ExistenCondicionesVigencias(codigoVigencia))
                {
                    confirm = await MensajesHelper.MostrarMensajeParaConfirmacion(TituloPagina, ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Mensajes.VigenciaConCondiciones));
                    if (!confirm)  return;
                }
                LayerOverlayService.Start();
                await CondicionesService.EliminarVigencia(VigenciaSeleccionada);
                Vigencias.Remove(VigenciaSeleccionada);
                SeleccionarVigencia();
                await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(TextosApp.Mensajes.RegistroEliminado));
            }
            catch (ExcepcionBaseDatos exBd)
            {
                await TratarExcepcionGeneradaEnBD(exBd, TituloPagina);
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


        private async Task GrabarVigencia()
        {
            if (! await ValidarFechasVigencia()) return;

            try
            {
                LayerOverlayService.Start();
                int mesDesde = MesDesdeVigenciaSeleccionado!.Codigo;
                int mesHasta = MesHastaVigenciaSeleccionado!.Codigo;

                Vigencia vigencia = new ();

                if (_modoEdicionVigencia == ModoEdicion.Alta)
                {
                    vigencia = DatosHelper.ClonarObjeto(_vigenciaNueva);
                }
                else
                {
                    vigencia = DatosHelper.ClonarObjeto(VigenciaSeleccionada!);
                }

                vigencia.MesDesde = mesDesde;
                vigencia.MesHasta = mesHasta;

                if (! await CondicionesService.ValidarSolapesVigencia(vigencia))
                {
                    await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Mensajes.VigenciasSolapadas));
                    return;
                }

                if (_modoEdicionVigencia == ModoEdicion.Alta)
                {

                    await CondicionesService.InsertarVigencia(vigencia );
                    Vigencias.Add(vigencia);
                }
                else
                {
                    await CondicionesService.ActualizarVigencia(vigencia);
                    Vigencia? vigenciamodificada = Vigencias.Find(c => c.Codigo == VigenciaSeleccionada!.Codigo);
                    if (vigenciamodificada != null)
                    {
                        vigenciamodificada.MesDesde = vigencia.MesDesde;
                        vigenciamodificada.MesHasta = vigencia.MesHasta;
                    }
                }

                GestorIdioma.TraducirMesesVigencias(Vigencias);

                VigenciaSeleccionada = vigencia;

                if (_modoEdicionVigencia == ModoEdicion.Alta)
                {
                    await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Mensajes.VigenciaAnadida));
                }
                else
                {
                    await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Mensajes.VigenciaModificada));
                }

                PopupVigenciaVisible = false;
                MesDesdeVigenciaSeleccionado = null;
                MesHastaVigenciaSeleccionado = null;
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

        private void OcultarPopupVigencia()
        {
            PopupVigenciaVisible = false;
        }

        private async Task<bool> ValidarFechasVigencia()
        {
            if (MesDesdeVigenciaSeleccionado == null || MesHastaVigenciaSeleccionado == null)
            {
                await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Mensajes.VigenciaSinMeses));
                return  false;
            }

            if (MesDesdeVigenciaSeleccionado.Codigo > MesHastaVigenciaSeleccionado.Codigo)
            {
                await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Mensajes.VigenciaValidacion));
                return false;
            }
            return true;
        }

        #endregion


        #region Grid Condiciones

        private async void OnGridCondicionesElementCustomized(GridCustomizeElementEventArgs ea)
        {
            if (ea.ElementType != GridElementType.DataCell) return;

            var column = (IGridDataColumn)ea.Column;
            var condicion = (CondicionViewModel)GridCondiciones.GetDataItem(ea.VisibleIndex);

            if (condicion == null) return;
          
            var itemCondicionOriginal = _condicionesCache.Find(x => x.Key == condicion.Key);
            if (itemCondicionOriginal == null) return;

            try 
            { 
                //Para utilizar este metodo de comparacion las columnas a comparar deben tener definida la propiedad caption

                // Diccionario: caption ? funci?n que devuelve (original, actual)
                var comparadores = new Dictionary<string, Func<(object? original, object? actual)>>
                {
                    {
                        ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.SagTpc),
                        () => (itemCondicionOriginal.PctSAG, condicion.PctSAG)
                    },
                    {
                        ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.ManpowerTpc),
                        () => (itemCondicionOriginal.PctManPower, condicion.PctManPower)
                    },
                    {
                        ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.BCDevolucion),
                        () => (itemCondicionOriginal.IndicadorCalculoDevolucion, condicion.IndicadorCalculoDevolucion)
                    },
                    {
                        ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.DevolucionTpc),
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
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina);
            }
        }

        private bool HaCambiadoValorConceptoCondiciones(CondicionViewModel item, ConceptosCondiciones concepto)
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

        private async void OnCondicionSetPctAsync(CondicionViewModel itemEditable, CondicionViewModel itemDestino, ConceptosCondiciones concepto, decimal? value)
        {
            try 
            { 
                CondicionViewModel? itemToActualizar = Condiciones.Find(c => c.CodigoMedio == itemEditable.CodigoMedio);

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
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina);
            }
        }

        private async void OnCondicionSetDevAsync(CondicionViewModel itemEditable, CondicionViewModel itemDestino, int? value)
        {
            try
            {
                int codigoMedio = itemEditable.CodigoMedio;
                int valor = value.HasValue ? value.Value : 0;

                itemEditable.IndicadorCalculoDevolucion = valor;

                Condiciones
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

                Excepciones
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
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina);
            }
        }

        private async Task ControlarCambiosCondiciones(CondicionViewModel itemEditable, CondicionViewModel itemDestino)
        {
            var itemOrigen = _condicionesCache.Find(c => c.CodigoMedio == itemEditable.CodigoMedio)!;
            var camposConCambios = DatosHelper.ObtenerCamposModificados(itemEditable, itemOrigen);

            if (camposConCambios.Count > 0)
            {
                DatosHelper.AplicarCambios(itemEditable, itemDestino);
                if (CondicionesNoGuardados.TryGetValue(itemDestino, out var cambios))
                {
                    cambios.CamposCambiados.UnionWith(camposConCambios);
                }
                else
                {
                    CondicionesNoGuardados.Add(itemDestino, new(TiposCambiosdeDatos.Modificados, camposConCambios));
                }
            }
            else
            {
                if (CondicionesNoGuardados.Count > 0)
                {
                    CondicionesNoGuardados.Remove(itemDestino);
                }
            }

            await ActualizarEstadoCambios(HayCambiosPendientes);
            StateHasChanged();
        }

        #endregion


        #region Grid Excepciones

        private async void OnGridMediosExcepcionesElementCustomized(GridCustomizeElementEventArgs ea)
        {
            if (ea.ElementType != GridElementType.DataCell) return;
            try
            {
                var column = (IGridDataColumn)ea.Column;
                var excepcion = (ExcepcionCondicionViewModel)GridExcepciones.GetDataItem(ea.VisibleIndex);

                if (excepcion == null) return;
                var itemOriginal = _excepcionesCache.Find(x => x.Key == excepcion.Key);
                if (itemOriginal == null)
                {
                    ea.CssClass = "grid-modified-cell";
                    return;
                }

                //Para utilizar este metodo de comparacion las columnas a comparar deben tener definida la propiedad caption

                // Diccionario: caption ? funci?n que devuelve (original, actual)
                var comparadores = new Dictionary<string, Func<(object? original, object? actual)>>()
                { 
                    {
                        ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Jerarquia),
                        () => (itemOriginal!.Jerarquia, excepcion.Jerarquia)
                    },
                    {
                        ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.SagTpc),
                        () => (itemOriginal!.PctSAG, excepcion.PctSAG)
                    },
                    {
                        ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.ManpowerTpc),
                        () => (itemOriginal!.PctManPower, excepcion.PctManPower)
                    },
                    {
                        ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.DevolucionTpc),
                        () => (itemOriginal!.PctDevolucion, excepcion.PctDevolucion)
                    },
                    {
                        ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Alcance),
                        () => (itemOriginal!.CodigoAlcance, excepcion.CodigoAlcance)
                    },
                    {
                        ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Disciplina),
                        () => (itemOriginal!.CodigoDisciplina, excepcion.CodigoDisciplina)
                    },
                    {
                        ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Diversified),
                        () => (itemOriginal!.CodigoDiversified, excepcion.CodigoDiversified)
                    },
                    {
                        ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Objetivo),
                        () => (itemOriginal!.CodigoObjetivo, excepcion.CodigoObjetivo)
                    },
                    {
                        ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.TipoCompra),
                        () => (itemOriginal!.CodigoTipoCompra, excepcion.CodigoTipoCompra)
                    },
                    {
                        ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.TipoDisciplina),
                        () => (itemOriginal!.CodigoTipoDisciplina, excepcion.CodigoTipoDisciplina)
                    },
                    {
                        ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.DisciplinaGrupo),
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
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina);
            }
        }

        private bool HaCambiadoValorExcepciones(ExcepcionCondicionViewModel item, ConceptosCondicionesNMD concepto)
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

        private bool HaCambiadoValorExcepciones(ExcepcionCondicionViewModel item, ConceptosCondiciones concepto)
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

        private bool HaCambiadoValorJerarquia(ExcepcionCondicionViewModel item)
        {
            var excepcion = _excepcionesCache.Find(c => c.CodigoCondicionMedio == item.CodigoCondicionMedio);
            if (excepcion == null)
            {
                return true;
            }
            return item.Jerarquia != excepcion.Jerarquia;
        }

        private async void OnExcepcionConceptosNMDChangedAsync(ExcepcionCondicionViewModel itemEditable, ExcepcionCondicionViewModel itemDestino, ConceptosCondicionesNMD concepto, int? value)
        {
            try
            { 
                ExcepcionCondicionViewModel? itemToActualizar = Excepciones.Find(c => c.CodigoCondicionMedio == itemEditable.CodigoCondicionMedio);

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
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina);
            }
        }

        private async void OnMedioCondicionExcepcionSetPctAsync(ExcepcionCondicionViewModel itemEditable, ExcepcionCondicionViewModel itemDestino, ConceptosCondiciones concepto, decimal? value)
        {
            try
            {
                ExcepcionCondicionViewModel? itemToActualizar = Excepciones.Find(c => c.CodigoCondicionMedio == itemEditable.CodigoCondicionMedio);
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
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina);
            }
        }

        private async Task ControlarCambiosExcepciones(ExcepcionCondicionViewModel itemEditable, ExcepcionCondicionViewModel itemDestino, bool aplicarCambios = true)
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
            await ActualizarEstadoCambios(HayCambiosPendientes);
            StateHasChanged();
        }

        private async void OnAñadirExcepcionAsync()
        {
            if (_codigoMedioSeleccionadoParaFiltro.HasValue)
            {
                try
                {
                    LayerOverlayService.Start();
                    int codigoMedio = _codigoMedioSeleccionadoParaFiltro!.Value;

                    var nuevaExcepcion = new ExcepcionCondicionViewModel();
                    nuevaExcepcion.MedioAccesible = true;

                    string codigoCondicion = ObtenerNuevoCodigoExcepcion();

                    nuevaExcepcion.CodigoCondicionMedio = codigoCondicion;
                    nuevaExcepcion.CodigoMedio = codigoMedio;

                    string descripcionMedio = _medios.Find(c => c.Codigo == codigoMedio)!.Descripcion;

                    nuevaExcepcion.DescripcionMedio = descripcionMedio;

                    int jerarquia = 1;
                    var excepcionesPorMedio = Excepciones.Where(c => c.CodigoMedio == codigoMedio);

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

                    Excepciones.Insert(jerarquia - 1, nuevaExcepcion);

                    ExcepcionesNoGuardadas[nuevaExcepcion] = new(TiposCambiosdeDatos.Añadidos, []);

                    GridExcepciones.Reload();
                    await ActualizarEstadoCambios(true);
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
            else
            {
                PopupMediosVisible = true;
            }
        }

        private string ObtenerNuevoCodigoExcepcion()
        {
            //Se busca en las excepciones aquella cuyo condigo empiza por -, que son la que ha a?adido el usuario y aun no se han grabado
            var codigosNuevos = Excepciones
                .Select(x => x.CodigoCondicionMedio)
                .Where(c => c.StartsWith("-"))
                .Select(c => int.Parse(c))
                .ToList();

            int nuevoCodigoNumerico;

            // Si ya hay c?digos nuevos, toma el menor y r?stale 1
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

        private async Task MoverJerarquia(ExcepcionCondicionViewModel item, AccionJerarquias accion)
        {
            try
            {
                int jerarquia = item.Jerarquia;
                int codigoMedio = item.CodigoMedio;

                int nuevoValorJerarquia = accion == AccionJerarquias.Subir ? jerarquia - 1 : jerarquia + 1;

                var excepcion = Excepciones.Find(c => c.CodigoMedio == codigoMedio && c.Jerarquia == nuevoValorJerarquia);
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
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina);
            }
        }

        private void OrdenarExcepciones()
        {
            Excepciones.Sort((a, b) =>
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
                bool confirmar = await MensajesHelper.MostrarMensajeParaConfirmacion(TituloPagina, ObtenerTexto(TextosApp.Mensajes.ConfirmacionEliminar));
                if (!confirmar) return;

                var item = (ExcepcionCondicionViewModel)dataItem;

                //Si el codigo empieza por -, es que es excepcion nueva y solo lo eliminamos de la lista
                if (item.CodigoCondicionMedio.StartsWith('-'))
                {
                    Excepciones.Remove(item);
                    GridCondiciones.Reload();
                }
                else
                {
                    await CondicionesService.EliminarExcepcionCondicion(item.CodigosConceptosCondiciones, item.Jerarquia, VigenciaSeleccionada!.Codigo, item.CodigoMedio, Usuario!.CodigoUsuario);

                    //Obtenemos de nuevo las excepciones ya que se ha eliminado y actualizado jerarquias
                    await ObtenerExcepciones(VigenciaSeleccionada!.Codigo);

                    if (_codigoMedioSeleccionadoParaFiltro != null)
                    {
                        CondicionViewModel? medio = Condiciones.Find(c => c.CodigoMedio == _codigoMedioSeleccionadoParaFiltro.Value);
                        if (medio != null)
                        {
                            await FiltrarExcepcionesPorMedio(medio);
                        }
                    }

                    await MensajesHelper.MostrarMensajeExito(TituloPagina, ObtenerTexto(TextosApp.Mensajes.RegistroEliminado));
                }
                ExcepcionesNoGuardadas.Remove(item);
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


        #region Filtro excepciones por medio

        private async Task FiltrarExcepcionesPorMedio(CondicionViewModel item)
        {
            if (_codigoMedioSeleccionadoParaFiltro == item.CodigoMedio)
            {
                //Hay que a?adir las excepciones que se han quitado al hacer el filtro por medio
                foreach (ExcepcionCondicionViewModel excepcion in _excepcionesCache)
                {
                    ExcepcionCondicionViewModel? exc = Excepciones.Find(c => c.CodigoCondicionMedio == excepcion.CodigoCondicionMedio);
                    if (exc == null)
                    {
                        Excepciones.Add(excepcion);
                    }
                }
                OrdenarExcepciones();
                TituloGridExcepciones = ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Excepciones);
                _codigoMedioSeleccionadoParaFiltro = null;
                MedioSeleccionadoDesdePopup = null;
            }
            else
            {
                if (ExcepcionesNoGuardadas.Count != 0)
                {
                    
                    await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Mensajes.AlFiltrarExcepcionesConCambios));
                    return;
                }
                _codigoMedioSeleccionadoParaFiltro = item.CodigoMedio;
                Excepciones = DatosHelper.ClonarObjeto(_excepcionesCache.Where(x => x.CodigoMedio == _codigoMedioSeleccionadoParaFiltro).ToList());
                TituloGridExcepciones = $"{ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.ExcepcionesMedio)} {item.DescripcionMedio}";
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

            foreach (var datoNoGuardado in CondicionesNoGuardados)
            {
                var itemModificado = datoNoGuardado.Key;

                if (itemModificado.PctDevolucion.HasValue)
                {
                    if (itemModificado.IndicadorCalculoDevolucion < 1)
                    {
                        await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Mensajes.PctDevCondicionSinBC));
                        resultado = false;
                    }
                }

                if (itemModificado.IndicadorCalculoDevolucion > 0)
                {
                    if (!itemModificado.PctDevolucion.HasValue)
                    {
                        await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Mensajes.BCCondicionSinPctDevolucion));
                        resultado = false;
                    }
                }


            }
            return resultado;
        }

        private async Task<bool> ValidarExcepciones()
        {
            if (CondicionesCacheTodasSinDatos() && CondicionesNoGuardados.Count == 0)
            {
                await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Mensajes.ExcepcionesSinCondiciones));
                return false;
            }

            // Validaci?n de duplicados optimizada con HashSet
            var excepcionesVistas = new HashSet<string>();
            ExcepcionCondicionViewModel? primerDuplicado = null;

            foreach (var excepcion in Excepciones)
            {
                // Crear clave ?nica basada en los valores que identifican duplicados
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
                string mensaje = ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Mensajes.MedioExcepcionConDatosDuplicados);
                await MensajesHelper.MostrarMensajeError(TituloPagina, string.Format(mensaje, primerDuplicado.DescripcionMedio));
                return false;
            }

            foreach (var datoNoGuardado in ExcepcionesNoGuardadas)
            {
                var itemModificado = datoNoGuardado.Key;

                if (itemModificado.PctSAG == null && itemModificado.PctManPower == null && itemModificado.PctDevolucion == null)
                {
                    await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Mensajes.PctEnTodasExcepciones));
                    return false;
                }
                if (itemModificado.CodigoAlcance == 0 && itemModificado.CodigoDisciplina == 0 && itemModificado.CodigoDiversified == 0 
                    && itemModificado.CodigoObjetivo == 0 && itemModificado.CodigoTipoCompra == 0
                    && itemModificado.CodigoTipoDisciplina == 0 && itemModificado.CodigoDisciplinaGrupo == 0)
                {
                    await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Mensajes.ConceptoNMDEnTodasExcepciones));
                    return false;
                }

                //Buscamos por si hay condiciones cambiadas por medio, por si se hubieran dejado a null los porcentajes
                int codigoMedio = itemModificado.CodigoMedio;
                var existeCondicion = CondicionesNoGuardados.Keys.Any(c => c.CodigoMedio == codigoMedio);

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
                        await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Mensajes.ExcepcionesConCondicionesEnMedio));
                        return false;
                    }
                }

                if (itemModificado.PctDevolucion != null)
                {
                    if (itemModificado.IndicadorCalculoDevolucion < 1)
                    {
                        await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Mensajes.PctDevExcepcionSinBD));
                        return false;
                    }
                }

                // Validaciones de porcentajes optimizadas con cach?
                if (!ValidarPorcentajeExcepcion(itemModificado.PctSAG, codigoMedio, c => c.PctSAG))
                {
                    await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Mensajes.PctExcepcionSinPctSAGCondicion));
                    return false;
                }

                if (!ValidarPorcentajeExcepcion(itemModificado.PctManPower, codigoMedio, c => c.PctManPower))
                {
                    await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Mensajes.PctExcepcionSinPctManPowerCondicion));
                    return false;
                }

                if (!ValidarPorcentajeExcepcion(itemModificado.PctDevolucion, codigoMedio, c => c.PctDevolucion))
                {
                    await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Mensajes.PctExcepcionSinPctDEVCondicion));
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
        /// Obtiene las condiciones agrupadas por medio usando cach? para optimizar b?squedas
        /// </summary>
        private Dictionary<int, List<CondicionViewModel>> ObtenerCondicionesPorMedio()
        {
            if (_condicionesPorMedio == null)
            {
                _condicionesPorMedio = Condiciones
                    .GroupBy(c => c.CodigoMedio)
                    .ToDictionary(g => g.Key, g => g.ToList());
            }
            return _condicionesPorMedio;
        }

        /// <summary>
        /// Valida si existe una condici?n con el porcentaje especificado para un medio (O(1) con cach?)
        /// </summary>
        private bool ValidarPorcentajeExcepcion(decimal? porcentaje, int codigoMedio, 
    Func<CondicionViewModel, decimal?> selector)
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
                if (CondicionesNoGuardados.Count == 0 && ExcepcionesNoGuardadas.Count == 0) 
                {
                    await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(TextosApp.Mensajes.SinModificaciones));
                    return;
                }

                if (CondicionesNoGuardados.Count > 0)
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
                var condicionesParaGrabar = CondicionesNoGuardados
                    .ToDictionary(kvp => (CondicionDto)kvp.Key, kvp => kvp.Value);
                var excepcionesParaGrabar = ExcepcionesNoGuardadas
                    .ToDictionary(kvp => (ExcepcionDto)kvp.Key, kvp => kvp.Value);
                await CondicionesService.GrabarCondicionesExcepciones(condicionesParaGrabar, excepcionesParaGrabar, VigenciaSeleccionada!.Codigo);
               
                await ObtenerCondicionesExcepciones(VigenciaSeleccionada!.Codigo);
                await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(TextosApp.Common.DatosGrabados));
                await LimpiarCambiosPendientes();
            }
            catch (Exception ex)
            {
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Mensajes.ErrorAlGrabar));
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
                int codigoMedio = MedioSeleccionadoDesdePopup!.Codigo;
                string codigoCondicion = ObtenerNuevoCodigoExcepcion();

                var nuevaExcepcion = new ExcepcionCondicionViewModel();
                nuevaExcepcion.CodigoCondicionMedio = codigoCondicion;
                nuevaExcepcion.CodigoMedio = codigoMedio;
                nuevaExcepcion.DescripcionMedio = MedioSeleccionadoDesdePopup.Descripcion;
                nuevaExcepcion.MedioAccesible = true;

                int jerarquia = 1;
                var excepcionesPorMedio = Excepciones.Where(c => c.CodigoMedio == codigoMedio);

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
                int indicadorCalculoDevolucion = Condiciones.Find(c => c.CodigoMedio == codigoMedio)!.IndicadorCalculoDevolucion;
                nuevaExcepcion.IndicadorCalculoDevolucion = indicadorCalculoDevolucion;

                int indice = Excepciones.IndexOf(Excepciones.Find(c => c.CodigoMedio == codigoMedio && c.Jerarquia == jerarquia - 1)!);

                Excepciones.Insert(indice + 1, nuevaExcepcion);

                OrdenarExcepciones();

                ExcepcionesNoGuardadas[nuevaExcepcion] = new(TiposCambiosdeDatos.Añadidos, []);

                GridExcepciones.Reload(); //Porque si no desplaza la ultima fila y no se ve
                await ActualizarEstadoCambios(true);


                MedioSeleccionadoDesdePopup = null;
                PopupMediosVisible = false;
            }
            catch (Exception ex)
            {
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Mensajes.ErrorAlGrabar));
            }
            finally
            {
                LayerOverlayService.Stop();
            }
        }

        private void OcultarPopupMedios()
        {
            PopupMediosVisible = false;
        }

        #endregion


        #region Limpiar y cancelar cambios

        private async Task CancelarCambiosCondiciones()
        {
            bool confirmar = await MensajesHelper.MostrarMensajeParaConfirmacion(TituloPagina, ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Mensajes.CancelarCambiosCondiciones));
            if (confirmar)
            {
                LimpiarCambiosCondicionesPendientes();
            }
        }

        private async Task CancelarCambiosExcepciones()
        {
            bool confirmar = await MensajesHelper.MostrarMensajeParaConfirmacion(TituloPagina, ObtenerTexto(TextosApp.Pages.PlanificacionCondiciones.Mensajes.CancelarCambiosExcepciones));
            if (confirmar)
            {
                await LimpiarCambiosExcepcionesPendientes();
            }
        }

        private async Task<bool> ComprobarSiHayCambiosPendienteAndSeguir()
        {
            if (HayCambiosPendientes)
            {
                bool continuar = await MensajesHelper.MostrarMensajeParaConfirmacion(TituloPagina, ObtenerTexto(TextosApp.Mensajes.AvisoAntesCancelar));
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
            Condiciones = DatosHelper.ClonarObjeto(_condicionesCache);
            CondicionesNoGuardados.Clear();

            if (ExcepcionesNoGuardadas.Count == 0)
            {
                base.LimpiarCambiosPendientes();
            }
        }

        private async Task LimpiarCambiosExcepcionesPendientes()
        {
            Excepciones = DatosHelper.ClonarObjeto(_excepcionesCache);
            ExcepcionesNoGuardadas.Clear();

            if (_codigoMedioSeleccionadoParaFiltro != null)
            {
                CondicionViewModel? condicion = Condiciones.Find(c => c.CodigoMedio == _codigoMedioSeleccionadoParaFiltro.Value);
                if (condicion != null)
                {
                    await FiltrarExcepcionesPorMedio(condicion);
                }
            }

            if (CondicionesNoGuardados.Count == 0)
            {
                base.LimpiarCambiosPendientes();
            }
        }

        private async Task CancelarCambios()
        {
            if (await MensajesHelper.MostrarMensajeParaConfirmacion(TituloPagina, ObtenerTexto(TextosApp.Mensajes.Cancelar)))
            {
                await LimpiarCambiosPendientes();
            }
        }

        #endregion


        #region Varios

        private static List<CodigoDescripcion> PrepararCatalogo(List<CodigoDescripcion> origen)
        {
            var lista = new List<CodigoDescripcion>(origen);
            lista.ForEach(m => m.Descripcion = StringHelper.Capitalize(m.Descripcion));
            InsertarFilaVacia(lista);
            return lista;
        }

        private static void InsertarFilaVacia(List<CodigoDescripcion> lista)
        {
            CodigoDescripcion item = new CodigoDescripcion { Codigo = 0, Descripcion = "" }; //El codigo 0 es el que tienen cuando vienen a null
            lista.Insert(0, item);
        }


        private async Task CargarConceptosNMD(ExcepcionCondicionViewModel fila, ConceptosCondicionesNMD concepto)
        {
            try
            {
                // Verificar si ya est?n cargadas las opciones para esta fila
                var yaEstanCargadas = concepto switch
                {
                    ConceptosCondicionesNMD.Disciplina => fila.DisciplinasDisponibles != null,
                    ConceptosCondicionesNMD.Objetivo => fila.ObjetivosDisponibles != null,
                    ConceptosCondicionesNMD.TipoCompra => fila.TiposCompraDisponibles != null,
                    ConceptosCondicionesNMD.TipoDisciplina => fila.TiposDisciplinaDisponibles != null,
                    ConceptosCondicionesNMD.DisciplinaGrupo => fila.DisciplinasGrupoDisponibles != null,
                    _ => false
                };

                if (yaEstanCargadas) return; // Ya est?n cargadas, no hacer nada

                var datos = await ObtenerConceptosNMD(fila, concepto);
                AsignarConceptosAFila(fila, concepto, datos);
                await InvokeAsync(StateHasChanged);
            }
            catch { }
        }


        private void AsignarConceptosAFila(ExcepcionCondicionViewModel fila, ConceptosCondicionesNMD concepto, List<CodigoDescripcion> datos)
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
        private async Task<List<CodigoDescripcion>> ObtenerConceptosNMD(ExcepcionCondicionViewModel excepcion, ConceptosCondicionesNMD concepto)
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
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Mensajes.ErrorAlGrabar));
                return new List<CodigoDescripcion>();
            }
        }


      
        #endregion


        /// <summary>
        /// Metodo para comprobar si venimos desde la pagina de importacion de condiciones de MMS
        /// </summary>
        /// <returns></returns>
        private async Task ManejarRequest()
        {
            try
            {
                var datos = NavegacionService.Obtener<dynamic>();
                if (datos == null) return;

                NavegacionService.Limpiar();
                if (datos is CondicionImportarFiltro filtro)
                {
                    if (filtro.CodigosNetwork.Length == 1)
                    {
                        NetworkSeleccionado = Networks.FirstOrDefault(n => n.Codigo == filtro.CodigosNetwork[0]);

                        if (NetworkSeleccionado != null)
                        {
                            GruposClientes = await PresupuestosService.ObtenerGruposClientePorNetwork(NetworkSeleccionado.Codigo);

                            MediosMaster = await PresupuestosService.ObtenerMediosPorNetWork(NetworkSeleccionado.Codigo.ToString());

                            if (filtro.CodigosGrupoCliente.Length == 1)
                            {
                                CodigoGrupoSeleccionado = GruposClientes.FirstOrDefault(n => n.Codigo == filtro.CodigosGrupoCliente[0])?.Codigo;
                            }
                            else if (GruposClientes.Count == 1)
                            {
                                CodigoGrupoSeleccionado = GruposClientes[0].Codigo;
                            }
                        }
                    }
                    
                    AnioSeleccionado = Anios.FirstOrDefault(n => n.Codigo == filtro.Anio);
                    if (AnioSeleccionado != null)
                    {
                        Versiones = await ObtenerVersionesPorPermisos(AnioSeleccionado.Codigo);
                    }
                    VersionSeleccionada = Versiones.FirstOrDefault(n => n.Codigo == filtro.CodigoVersion);

                    if (!String.IsNullOrEmpty(RadioGroupAcuerdoChecked) && AnioSeleccionado != null && NetworkSeleccionado != null && VersionSeleccionada != null && CodigoGrupoSeleccionado != null)
                    {
                        _desdePaginaImportarMMS = true;
                        await FiltroBuscar();
                    }
                }
                else
                {
                    await FilterInitAsync();
                }
            }
            catch (Exception ex)
            {
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Common.Messages.UndefinedError));
            }
        }
    }
}




