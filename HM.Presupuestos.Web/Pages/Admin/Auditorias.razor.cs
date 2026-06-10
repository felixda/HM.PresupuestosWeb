using HM.Presupuestos.Domain.Entidades.LogAcciones;

namespace HM.Presupuestos.Web.Pages.Admin
{
    public partial class Auditorias
    {
        #region Inyección de Dependencias

        [Inject] private ILogAccionesService LogAccionesService { get; set; } = default!;
        [Inject] private IRecursosApp RecursosApp { get; set; } = default!;

        #endregion

        #region Propiedades Privadas

        private List<CodigoDescripcion> TiposAuditoria { get; set; } = [];
        private int? TipoAuditoriaSeleccionado { get; set; }
        private DateTime? FechaInicio { get; set; }
        private DateTime? FechaFin { get; set; }
        private List<Auditoria> ResultadoAuditorias { get; set; } = [];
        private List<CodigoDescripcion> PaginasNavegables { get; set; } = [];
        private int? PaginaSeleccionada { get; set; }

        #endregion

        #region Ciclo de Vida

        protected override Task InicializarPaginaAsync()
        {
            TiposAuditoria = RecursosApp.ObtenerAccionesLog();
            PaginasNavegables = RecursosApp.ObtenerPaginasNavegables();
            return Task.CompletedTask;
        }

        protected override Task OnPermisoDenegadoAsync()
        {
            return Task.CompletedTask;
        }

        protected override Task OnIdiomaActualizadoAsync()
        {
            TiposAuditoria = RecursosApp.ObtenerAccionesLog();
            PaginasNavegables = RecursosApp.ObtenerPaginasNavegables();
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
                    ObtenerTexto(TextosApp.Pages.Auditorias.CamposObligatorios));
                return;
            }

            await EjecutarAsync(async () =>
            {
                AccionesLog tipo = (AccionesLog)TipoAuditoriaSeleccionado.Value;
                ResultadoAuditorias = await LogAccionesService.ObtenerAuditorias(tipo, FechaInicio, FechaFin, PaginaSeleccionada);
            });
        }

        private async Task LimpiarFiltroAsync()
        {
            TipoAuditoriaSeleccionado = null;
            FechaInicio = null;
            FechaFin = null;
            PaginaSeleccionada = null;
            ResultadoAuditorias = [];
            await InvokeAsync(StateHasChanged);
        }

        #endregion
    }
}
