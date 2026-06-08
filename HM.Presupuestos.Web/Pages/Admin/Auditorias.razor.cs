using HM.Presupuestos.Application.CasosDeUso.LogAcciones;
using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Entidades.LogAcciones;
using HM.Presupuestos.Domain.Extensiones;
using HM.Presupuestos.Web.Adaptadores;

namespace HM.Presupuestos.Web.Pages.Admin
{
    public partial class Auditorias
    {
        #region Inyección de Dependencias

        [Inject] protected ILogAccionesService LogAccionesService { get; set; } = default!;
        [Inject] protected DialogoErrores ErrorService { get; set; } = default!;

        #endregion

        #region Propiedades Privadas

        private bool _componentInitialized = false;
        private string _pageTitle = string.Empty;
        private List<CodigoDescripcion> _tiposAuditoria { get; set; } = [];
        private int? _tipoAuditoriaSeleccionado { get; set; }
        private DateTime? _fechaInicio { get; set; }
        private DateTime? _fechaFin { get; set; }
        private List<Auditoria> _resultadoAuditorias { get; set; } = [];

        #endregion

        #region Ciclo de Vida

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    _pageTitle = ObtenerTexto($"Menu:Menu_{(int)CodigosMenu.Auditorias}:label");
                    LayerOverlayService.Start($"{ObtenerTexto(AppResources.Common.Loading)} {_pageTitle}");
                    InicializarPagina();
                }
                catch (Exception ex)
                {
                    await ErrorService.MostrarErrorInicializandoPagina(_pageTitle, ex);
                    return;
                }
                finally
                {
                    LayerOverlayService.Stop();
                }

                if (!_componentInitialized)
                {
                    _componentInitialized = true;
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        #endregion

        #region Métodos Privados

        private void InicializarPagina()
        {
            _tiposAuditoria = Enum.GetValues<AccionesLog>()
                .Select(a => new CodigoDescripcion
                {
                    Codigo = (int)a,
                    Descripcion = a.ObtenerDescripcion()
                })
                .OrderBy(x => x.Descripcion)
                .ToList();
        }

        private async Task BuscarAuditoriasAsync()
        {
            if (_tipoAuditoriaSeleccionado is null)
            {
                await MensajesHelper.MostrarMensajeAviso(
                    _pageTitle,
                    ObtenerTexto(AppResources.Pages.Auditorias.CamposObligatorios));
                return;
            }

            await EjecutarAsync(async () =>
            {
                AccionesLog tipo = (AccionesLog)_tipoAuditoriaSeleccionado.Value;
                _resultadoAuditorias = await LogAccionesService.ObtenerAuditorias(tipo, _fechaInicio, _fechaFin);
            });
        }

        private async Task LimpiarFiltroAsync()
        {
            _tipoAuditoriaSeleccionado = null;
            _fechaInicio = null;
            _fechaFin = null;
            _resultadoAuditorias = [];
            await InvokeAsync(StateHasChanged);
        }

        #endregion
    }
}
