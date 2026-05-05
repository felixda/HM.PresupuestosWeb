using HM.Core.Comun.v6.Entidades.Seguridad;
using HM.Presupuestos.Domain.Comun;
using HM.Presupuestos.Server.Helper;
using HM.Presupuestos.Server.Services;
using Microsoft.AspNetCore.Components;

namespace HM.Presupuestos.Server.Pages
{
    public partial class Index
    {
        #region Servicios Inyectados

        [Inject] private ILogService _logService { get; set; } = default!;
        [Inject] private IConfiguration _configuration { get; set; } = default!;
        [Inject] private ILayerOverlayService _LayerOverlayService { get; set; } = default!;
        [Inject] private ErrorDialogService _ErrorService { get; set; } = default!;

        #endregion

        #region Propiedades

        private string _pageTitle { get; set; } = string.Empty;
        private List<Menu> _menu = new();

        #endregion

        #region Ciclo de Vida

        protected override async Task OnUsuarioDisponibleAsync()
        {
            try
            {
                // ? Usar AppResources para el mensaje de carga
                _LayerOverlayService.Start($"{T(AppResources.Common.Loading)} {_pageTitle}");
                
                ActualizarMenu(Usuario);
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Index] ? Error en OnUsuarioDisponibleAsync: {ex.Message}");
                await _logService.InsertException(ex);
                await _ErrorService.MostrarErrorInicializandoPagina(_pageTitle, ex);
            }
            finally
            {
                _LayerOverlayService.Stop();
            }
        }

        protected override async Task OnUsuarioLoginDesconectado()
        {
            try
            {
                _LayerOverlayService.Start($"{T(AppResources.Common.Loading)} {_pageTitle}");
                await base.OnUsuarioLoginDesconectado();
                ActualizarMenu(Usuario);
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Index] ? Error en OnUsuarioLoginDesconectado: {ex.Message}");
                await _logService.InsertException(ex);
                await _ErrorService.MostrarErrorInicializandoPagina(_pageTitle, ex);
            }
            finally
            {
                _LayerOverlayService.Stop();
            }
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Actualiza los menús favoritos del usuario
        /// </summary>
        private void ActualizarMenu(UsuarioEntidad user)
        {
            _menu = user.Menus.Where(c => c.IsFavorite).ToList();
        }


        #endregion
    }
}