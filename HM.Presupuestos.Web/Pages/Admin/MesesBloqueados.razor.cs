namespace HM.Presupuestos.Web.Pages.Admin
{
    public partial class MesesBloqueados
    {
        #region Inyección de Dependencias

        [Inject] protected IAdminService AdminService { get; set; } = default!;
        [Inject] protected new IVersionesService VersionesService { get; set; } = default!;

        #endregion

        #region Propiedades privadas

        private string TextoToolTipAyuda { get; set; } = string.Empty;
        private List<CodigoDescripcion> Anios { get; set; } = [];
        private CodigoDescripcion? AnioSeleccionado { get; set; }
        private bool HayAnioSeleccionado => AnioSeleccionado is not null && AnioSeleccionado.Codigo > 0;

        #endregion

        #region Grid Meses

        private DxGrid GridMeses { get; set; } = new DxGrid();
        private List<CodigoDescripcion> Meses { get; set; } = [];
        private IReadOnlyList<object> MesesSeleccionados { get; set; } = [];

        #endregion

        #region Ciclo de Vida

        protected override Task OnPermisoDenegadoAsync() => Task.CompletedTask;

        protected override async Task InicializarPaginaAsync()
        {
            InicializarTextos();
            InicializarMeses();
            await CargarAniosAsync();
        }

        private void InicializarTextos()
        {
            TextoToolTipAyuda = ObtenerTexto(TextosApp.Pages.MesesBloqueados.ToolTip);
        }

        private void InicializarMeses()
        {
            Meses = GestorIdioma.ObtenerMeses();
        }

        private async Task CargarAniosAsync()
        {
            Anios = await VersionesService.ObtenerAniosConVersiones(true);

            if (Anios.Count > 0)
            {
                int anioActual = DateTime.Now.Year;
                AnioSeleccionado = Anios.FirstOrDefault(x => x.Codigo == anioActual)
                                ?? Anios.First();
            }
        }

        #endregion

        #region Eventos

        private async Task ComboAniosSelectedDataItemChangedAsync(SelectedDataItemChangedEventArgs<CodigoDescripcion> e)
        {
            if (e.DataItem != null)
            {
                int anioSeleccionado = e.DataItem.Codigo;
                await EjecutarAsync(async () =>
                {
                    List<int> mesesBloqueado = await AdminService.ObtenerMesesBloqueados(anioSeleccionado);
                    MesesSeleccionados = Meses
                       .Where(m => mesesBloqueado.Contains(m.Codigo))
                       .ToList();
                });
            }
            else
            {
                GridMeses.ClearSelection();
            }
        }

        private async Task GrabarAsync()
        {
            await EjecutarAsync(async () =>
            {
                var mesesSeleccionados = ObtenerCodigosMesesSeleccionados();
                await AdminService.InsertarMesesBloqueado(AnioSeleccionado!.Codigo, mesesSeleccionados);
                await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(TextosApp.Common.DatosGrabados));
            });
        }

        private List<int> ObtenerCodigosMesesSeleccionados() =>
            [.. (MesesSeleccionados?.Cast<CodigoDescripcion>().Select(x => x.Codigo) ?? [])];

        #endregion

    }
}


