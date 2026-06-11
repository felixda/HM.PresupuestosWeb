using HM.Presupuestos.Application.CasosDeUso.Compartido;

namespace HM.Presupuestos.Web.Pages.Condiciones
{
    public partial class ImportacionCondiciones
    {
        #region Inyecci?n de Dependencias

        [Inject] protected IJwt Jwt { get; set; } = default!;
        [Inject] protected IMaestrosService PresupuestosService { get; set; } = default!;
        [Inject] protected ICondicionesService CondicionesService { get; set; } = default!;
        [Inject] protected ParametrosNavegacion NavegacionService { get; set; } = default!;

        #endregion
        #region Propiedades privadas

        #region Page

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

        protected override async Task InicializarPaginaAsync()
        {
            TextoToolTipAyuda = ObtenerTexto(TextosApp.Pages.ImportacionCondiciones.ToolTip);

            Networks = await PresupuestosService.ObtenerNetworks();
            Anios = await VersionesService.ObtenerAniosConVersiones();

            FilterInit();
        }

        protected override Task OnPermisoDenegadoAsync() => Task.CompletedTask;

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

            await EjecutarAsync(async () =>
            {
                if (values.Any())
                {
                    string codigosNetwork = ObtenerValoresSeleccionados<CodigoDescripcion, int>(values, x => x.Codigo, ",");
                    GruposClientes = await PresupuestosService.ObtenerGruposClientePorNetworks(codigosNetwork);
                }
            });
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
                await EjecutarAsync(async () =>
                {
                    Versiones = await ObtenerVersionesPorPermisos(anioSeleccionado);
                });
            }
        }

        private async Task ImportarCondicionesMMS()
        {
            if (!ValidarCamposObligatoriosFiltro())
            {
                await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(TextosApp.Mensajes.CamposObligatorios));
                return;
            }

            bool confirmacion = await MensajesHelper.MostrarMensajeParaConfirmacion(TituloPagina, ObtenerTexto(TextosApp.Mensajes.AvisoImportarCondiciones));
            if (!confirmacion) return;

            await EjecutarAsync(async () =>
            {
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

                try
                {
                    await CondicionesService.ImportarCondicionesMMS(_filtro);
                }
                catch (ExcepcionBaseDatos exBd)
                {
                    await TratarExcepcionGeneradaEnBD(exBd, TituloPagina);
                    ImportacionRealizada = false;
                    return;
                }

                await MensajesHelper.MostrarMensajeExito(TituloPagina, ObtenerTexto(TextosApp.Mensajes.ImportacionCondicionesFinalizada));
                ImportacionRealizada = true;
            });
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
            await EjecutarAsync(() =>
            {
                AnioSeleccionado = null;
                VersionSeleccionada = null;
                GruposClientesSeleccionados = null;
                FilterInit();
            }, showOverlay: false);
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
            NavigationManager.NavigateTo("/gestion/planificacion-condiciones");
        }

    }
}



