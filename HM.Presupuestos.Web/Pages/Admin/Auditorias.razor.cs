using HM.Presupuestos.Domain.Entidades.LogAcciones;
using HM.Presupuestos.Domain.Extensiones;

namespace HM.Presupuestos.Web.Pages.Admin
{
    public partial class Auditorias
    {
        #region Inyección de Dependencias

        [Inject] protected ILogAccionesService LogAccionesService { get; set; } = default!;

        #endregion

        #region Propiedades Privadas

        private List<CodigoDescripcion> TiposAuditoria { get; set; } = [];
        private int? TipoAuditoriaSeleccionado { get; set; }
        private DateTime? FechaInicio { get; set; }
        private DateTime? FechaFin { get; set; }
        private List<Auditoria> ResultadoAuditorias { get; set; } = [];

        #endregion

        #region Ciclo de Vida

        protected override async Task InicializarPaginaAsync()
        {
            TiposAuditoria = Enum.GetValues<AccionesLog>()
                .Select(a => new CodigoDescripcion
                {
                    Codigo = (int)a,
                    Descripcion = a.ObtenerDescripcion()
                })
                .OrderBy(x => x.Descripcion)
                .ToList();
        }

        protected override Task OnPermisoDenegadoAsync()
        {
            return Task.CompletedTask;
        }

        #endregion

        #region Métodos Privados

        private async Task BuscarAuditoriasAsync()
        {
            if (TipoAuditoriaSeleccionado is null)
            {
                await MensajesHelper.MostrarMensajeAviso(
                    TituloPagina,
                    ObtenerTexto(AppResources.Pages.Auditorias.CamposObligatorios));
                return;
            }

            await EjecutarAsync(async () =>
            {
                AccionesLog tipo = (AccionesLog)TipoAuditoriaSeleccionado.Value;
                ResultadoAuditorias = await LogAccionesService.ObtenerAuditorias(tipo, FechaInicio, FechaFin);
            });
        }

        private async Task LimpiarFiltroAsync()
        {
            TipoAuditoriaSeleccionado = null;
            FechaInicio = null;
            FechaFin = null;
            ResultadoAuditorias = [];
            await InvokeAsync(StateHasChanged);
        }

        #endregion
    }
}
