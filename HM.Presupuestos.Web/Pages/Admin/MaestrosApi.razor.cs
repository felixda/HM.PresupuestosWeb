using HM.Presupuestos.Web.Adaptadores.Api;
using HM.Presupuestos.Web.Adaptadores.Api.Modelos;

namespace HM.Presupuestos.Web.Pages.Admin;

public partial class MaestrosApi
{
    #region Inyección de Dependencias

    [Inject] protected IMaestrosApiService MaestrosApiService { get; set; } = default!;

    #endregion

    #region Propiedades privadas

    private string TextoToolTipAyuda { get; set; } = string.Empty;
    private List<CodigoDescripcion> OpcionesMaestro { get; set; } = [];
    private int? MaestroSeleccionado { get; set; }
    private List<MaestroApiItem> Datos { get; set; } = [];

    #endregion

    #region Ciclo de Vida

    protected override Task OnPermisoDenegadoAsync() => Task.CompletedTask;

    protected override Task InicializarPaginaAsync()
    {
        TextoToolTipAyuda = "Consulta de maestros mediante API REST";

        OpcionesMaestro =
        [
            new CodigoDescripcion { Codigo = 1, Descripcion = "Tipologías" },
            new CodigoDescripcion { Codigo = 2, Descripcion = "Networks" },
            new CodigoDescripcion { Codigo = 3, Descripcion = "Medios" },
            new CodigoDescripcion { Codigo = 4, Descripcion = "Grupos clientes" }
        ];

        MaestroSeleccionado = 1;
        return CargarMaestroAsync();
    }

    #endregion

    #region Eventos

    private async Task OnComboMaestroSelectedDataItemChangedAsync(SelectedDataItemChangedEventArgs<CodigoDescripcion> e)
    {
        if (e.DataItem == null)
        {
            return;
        }

        MaestroSeleccionado = e.DataItem.Codigo;
        await CargarMaestroAsync();
    }

    private async Task CargarMaestroAsync()
    {
        if (!MaestroSeleccionado.HasValue)
        {
            await MensajesHelper.MostrarMensajeAviso(TituloPagina, "Selecciona un maestro.");
            return;
        }

        await EjecutarAsync(async () =>
        {
            Datos = MaestroSeleccionado.Value switch
            {
                1 => await MaestrosApiService.ObtenerTipologias(),
                2 => await MaestrosApiService.ObtenerNetworks(),
                3 => await MaestrosApiService.ObtenerMedios(),
                4 => await MaestrosApiService.ObtenerGruposClientes(),
                _ => []
            };

            if (Datos.Count == 0)
            {
                await MensajesHelper.MostrarMensajeInfo(TituloPagina, "No hay datos para el maestro seleccionado.");
            }
        });
    }

    #endregion
}
