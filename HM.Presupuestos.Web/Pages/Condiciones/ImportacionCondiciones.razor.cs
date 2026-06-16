using HM.Presupuestos.Application.CasosDeUso.Compartido;

namespace HM.Presupuestos.Web.Pages.Condiciones
{
    public partial class ImportacionCondiciones
    {
        #region Inyección de Dependencias

        [Inject] protected IJwt Jwt { get; set; } = default!;
        [Inject] protected IMaestrosCacheService PresupuestosService { get; set; } = default!;
        [Inject] protected ICondicionesService CondicionesService { get; set; } = default!;
        [Inject] protected ParametrosNavegacion NavegacionService { get; set; } = default!;

        #endregion

        #region Propiedades

        private string TextoToolTipAyuda { get; set; } = string.Empty;

        private bool ImportacionRealizada { get; set; }

        private string QuerySearchGrupoClientesList { get; set; } = string.Empty;

        private CondicionImportarFiltro? _filtro;

        private List<CodigoDescripcion> Networks { get; set; } = [];
        private object? _networksSeleccionados;

        private object? NetworkSeleccionados
        {
            get => _networksSeleccionados;
            set
            {
                if (_networksSeleccionados != value)
                {
                    _networksSeleccionados = value;
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
        private CodigoDescripcion? AnioSeleccionado { get; set; }

        private List<VersionResumen> Versiones { get; set; } = [];
        private VersionResumen? VersionSeleccionada { get; set; }

        #endregion

        #region Ciclo de Vida del Componente

        protected override async Task InicializarPaginaAsync()
        {
            TextoToolTipAyuda = ObtenerTexto(TextosApp.Pages.ImportacionCondiciones.ToolTip);

            Networks = await PresupuestosService.ObtenerNetworks();
            Anios = await VersionesService.ObtenerAniosConVersiones();

            InicializarFiltro();
        }

        protected override Task OnPermisoDenegadoAsync() => Task.CompletedTask;

        #endregion

        #region Métodos Privados

        private void InicializarFiltro()
        {
            if (Networks.Count == 1)
            {
                _networksSeleccionados = new List<CodigoDescripcion> { Networks[0] };
            }
            else if (Networks.Count > 1)
            {
                _networksSeleccionados = null;
            }
        }

        private async Task OnNetworksValuesChangedAsync(IEnumerable<CodigoDescripcion> values, IDropDownBox dropDownBox)
        {
            dropDownBox.BeginUpdate();
            dropDownBox.Value = values;

            GruposClientesSeleccionados = null;
            GruposClientes = [];

            await EjecutarAsync(async () =>
            {
                if (values.Any())
                {
                    string codigosNetwork = ObtenerValoresSeleccionados<CodigoDescripcion, int>(values, x => x.Codigo, ",");
                    GruposClientes = await PresupuestosService.ObtenerGruposClientePorNetworks(codigosNetwork);
                }
            });
            dropDownBox.EndUpdate();
        }

        private async Task ComboAniosSelectedDataItemChangedAsync(SelectedDataItemChangedEventArgs<CodigoDescripcion> e)
        {
            VersionSeleccionada = null;
            Versiones = [];
            if (e.DataItem != null)
            {
                int anioSeleccionado = e.DataItem.Codigo;
                await EjecutarAsync(async () =>
                {
                    Versiones = await ObtenerVersionesPorPermisos(anioSeleccionado);
                });
            }
        }

        private async Task ImportarCondicionesMMSAsync()
        {
            if (!PuedeAplicarFiltro())
            {
                await MostrarCamposObligatoriosAsync();
                return;
            }

            if (!await ConfirmarImportacionAsync()) return;

            await EjecutarAsync(async () =>
            {
                var filtro = CrearFiltroImportacion();

                await ImportarCondicionesAsync(filtro);

                await MostrarImportacionFinalizadaAsync();
                ImportacionRealizada = true;
            });
        }

        private Task MostrarCamposObligatoriosAsync() =>
            MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(TextosApp.Mensajes.CamposObligatorios));

        private Task<bool> ConfirmarImportacionAsync() =>
            MensajesHelper.MostrarMensajeParaConfirmacion(TituloPagina, ObtenerTexto(TextosApp.Mensajes.AvisoImportarCondiciones));

        private CondicionImportarFiltro CrearFiltroImportacion()
        {
            _filtro = new CondicionImportarFiltro
            {
                CodigosNetwork = _networksSeleccionados == null
                    ? [.. Networks.Select(x => x.Codigo)]
                    : [.. ((List<CodigoDescripcion>)_networksSeleccionados).Select(x => x.Codigo)],
                CodigosGrupoCliente = GruposClientesSeleccionados == null
                    ? [-1]
                    : [.. ((List<CodigoDescripcion>)GruposClientesSeleccionados).Select(x => x.Codigo)],
                Anio = AnioSeleccionado!.Codigo,
                CodigoVersion = VersionSeleccionada!.Codigo
            };
            return _filtro;
        }

        private Task ImportarCondicionesAsync(CondicionImportarFiltro filtro) =>
            CondicionesService.ImportarCondicionesMMS(filtro);

        private Task MostrarImportacionFinalizadaAsync() =>
            MensajesHelper.MostrarMensajeExito(TituloPagina, ObtenerTexto(TextosApp.Mensajes.ImportacionCondicionesFinalizada));

        private bool PuedeAplicarFiltro()
        {
            return AnioSeleccionado != null && VersionSeleccionada != null;
        }

        private async Task LimpiarFiltroAsync()
        {
            await EjecutarAsync(() =>
            {
                AnioSeleccionado = null;
                VersionSeleccionada = null;
                GruposClientesSeleccionados = null;
                InicializarFiltro();
            }, showOverlay: false);
        }

        private void OnListBoxValuesChanged<T>(IEnumerable<T> values, IDropDownBox dropDownBox)
        {
            dropDownBox.BeginUpdate();
            dropDownBox.Value = values;
            dropDownBox.EndUpdate();
        }

        private void IrACondiciones()
        {
            NavegacionService.Guardar(_filtro);
            NavigationManager.NavigateTo("/gestion/planificacion-condiciones");
        }

        #endregion
    }
}



