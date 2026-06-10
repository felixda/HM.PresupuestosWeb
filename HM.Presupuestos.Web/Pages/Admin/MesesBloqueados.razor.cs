using DevExpress.Blazor;
using HM.Core.Comun.v6.Seguridad.Interfaces;
using HM.Presupuestos.Application.CasosDeUso;
using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Web.Adaptadores;

namespace HM.Presupuestos.Web.Pages.Admin
{
    public partial class MesesBloqueados
    {
        #region Inyección de Dependencias

        [Inject] protected IAdminService AdminService { get; set; } = default!;
        [Inject] protected IVersionesService VersionesService { get; set; } = default!;
        [Inject] protected TraduccionesHelper Traducciones { get; set; } = default!;
        [Inject] protected ILogAccionesService LogAccionesService { get; set; } = default!;

        #endregion


        #region Private Properties

        private string PageTitle { get; set; } = string.Empty;
        private string TextoToolTipAyuda { get; set; } = string.Empty;
        private List<CodigoDescripcion> Anios = [];
        private CodigoDescripcion? AnioSeleccionado { get; set; } = new();
        private bool HayAnioSeleccionado => AnioSeleccionado is not null && AnioSeleccionado.Codigo > 0;

        #endregion

        #region Grid Meses

        DxGrid GridMeses { get; set; } = new DxGrid();

        private List<CodigoDescripcion> Meses = [];
        IReadOnlyList<object> MesesSeleccionados { get; set; } = [];

        #endregion

        #region Ciclo de Vida

        protected override Task OnPermisoDenegadoAsync()
        {
            return Task.CompletedTask;
        }

        protected override async Task InicializarPaginaAsync()
        {
            TextoToolTipAyuda = ObtenerTexto(AppResources.Pages.MesesBloqueados.ToolTip);
            Meses = await Traducciones.ObtenerMeses();
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

        private async Task ComboAnios_SelectedDataItemChanged(SelectedDataItemChangedEventArgs<CodigoDescripcion> e)
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

        private async Task Grabar_Click()
        {
            await EjecutarAsync(async () =>
            {
                List<int> mesesSeleccionados = [
                        .. (MesesSeleccionados?.Cast<CodigoDescripcion>()
                                              .Select(x => x.Codigo) ?? [])
                    ];

                await AdminService.InsertarMesesBloqueado(AnioSeleccionado!.Codigo, mesesSeleccionados);
                await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(AppResources.Common.DatosGrabados));
            });
        }

       
        #endregion

    }
}


