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
        private Auditoria? _auditoriaSeleccionada;
        private bool _popupParametrosVisible;
        private EstadisticasAuditoria? _estadisticas;

        private bool RangoSuperaLimite =>
            FechaInicio.HasValue && FechaFin.HasValue &&
            (FechaFin.Value - FechaInicio.Value).TotalDays > 90;

        #endregion

        #region Ciclo de Vida

        protected override Task InicializarPaginaAsync()
        {
            TiposAuditoria = RecursosApp.ObtenerAccionesLog();
            PaginasNavegables = RecursosApp.ObtenerPaginasNavegables();
            FechaInicio = DateTime.Today;
            FechaFin = DateTime.Today;
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

            if (!FechaInicio.HasValue || !FechaFin.HasValue)
            {
                await MensajesHelper.MostrarMensajeAviso(
                    TituloPagina,
                    ObtenerTexto(TextosApp.Pages.Auditorias.FechasObligatorias));
                return;
            }

            await EjecutarAsync(async () =>
            {
                AccionesLog tipo = (AccionesLog)TipoAuditoriaSeleccionado.Value;
                var fechaFinInclusiva = FechaFin.Value.Date.AddDays(1);
                ResultadoAuditorias = await LogAccionesService.ObtenerAuditorias(tipo, FechaInicio, fechaFinInclusiva, PaginaSeleccionada);
                _estadisticas = await LogAccionesService.ObtenerEstadisticas(tipo, FechaInicio.Value, FechaFin.Value, PaginaSeleccionada);
            });
        }

        private async Task LimpiarFiltroAsync()
        {
            TipoAuditoriaSeleccionado = null;
            FechaInicio = DateTime.Today;
            FechaFin = DateTime.Today;
            PaginaSeleccionada = null;
            ResultadoAuditorias = [];
            _estadisticas = null;
            await InvokeAsync(StateHasChanged);
        }

        private void AbrirPopupParametros(Auditoria auditoria)
        {
            _auditoriaSeleccionada = auditoria;
            _popupParametrosVisible = true;
        }

        #endregion
    }
}
