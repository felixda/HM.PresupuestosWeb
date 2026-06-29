using HM.Presupuestos.Domain.Entidades.LogAcciones;

namespace HM.Presupuestos.Web.Pages.Admin
{
    public partial class Logs
    {
        #region Inyección de Dependencias

        [Inject] private ILogsTecnicosService LogsTecnicosService { get; set; } = default!;

        #endregion

        #region Propiedades privadas

        private DateTime? FechaDesde { get; set; }
        private DateTime? FechaHasta { get; set; }
        private string? NivelSeleccionado { get; set; }
        private string? UsuarioSeleccionado { get; set; }
        private string? CategoriaSeleccionada { get; set; }
        private string? MensajeSeleccionado { get; set; }
        private List<CodigoDescripcion> NivelesDisponibles { get; set; } = [];
        private List<CodigoDescripcion> UsuariosDisponibles { get; set; } = [];
        private List<LogTecnico> _logs = [];
        private LogTecnico? _logSeleccionado;
        private bool _popupDetalleVisible;
        private bool _busquedaEjecutada;

        #endregion

        #region Ciclo de Vida

        protected override async Task InicializarPaginaAsync()
        {
            FechaDesde = DateTime.Today;
            FechaHasta = DateTime.Today;

            await EjecutarAsync(async () =>
            {
                NivelesDisponibles = await LogsTecnicosService.ObtenerNivelesDisponibles();
                UsuariosDisponibles = await LogsTecnicosService.ObtenerUsuariosDisponibles();
            });
        }

        protected override Task OnPermisoDenegadoAsync()
        {
            return Task.CompletedTask;
        }

        #endregion

        #region Métodos privados

        private async Task BuscarLogsAsync()
        {
            await EjecutarAsync(async () =>
            {
                var fechaHasta = FechaHasta?.Date.AddDays(1).AddTicks(-1);

                _logs = await LogsTecnicosService.ObtenerLogs(new FiltroLogsTecnicos
                {
                    FechaDesde = FechaDesde?.Date,
                    FechaHasta = fechaHasta,
                    Nivel = NivelSeleccionado,
                    Usuario = UsuarioSeleccionado,
                    Categoria = CategoriaSeleccionada,
                    Mensaje = MensajeSeleccionado
                });

                _busquedaEjecutada = true;
            });
        }

        private async Task LimpiarFiltroAsync()
        {
            FechaDesde = DateTime.Today;
            FechaHasta = DateTime.Today;
            NivelSeleccionado = null;
            UsuarioSeleccionado = null;
            CategoriaSeleccionada = null;
            MensajeSeleccionado = null;
            _logs = [];
            _logSeleccionado = null;
            _popupDetalleVisible = false;
            _busquedaEjecutada = false;
            await InvokeAsync(StateHasChanged);
        }

        private string ObtenerUsuarioVisible(LogTecnico? log)
        {
            return log == null || string.IsNullOrWhiteSpace(log.Usuario)
                ? ObtenerTexto(TextosApp.Pages.Logs.SinUsuario)
                : log.Usuario;
        }

        private void AbrirPopupDetalle(LogTecnico log)
        {
            _logSeleccionado = log;
            _popupDetalleVisible = true;
        }

        private string ConstruirDetalleTecnico()
        {
            if (_logSeleccionado == null)
                return string.Empty;

            return $"{_logSeleccionado.Excepcion}\n\n{_logSeleccionado.StackTrace}\n\n{_logSeleccionado.Comentarios}";
        }

        #endregion
    }
}