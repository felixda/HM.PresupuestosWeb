
namespace HM.Presupuestos.Web.Pages
{
    public partial class Index
    {
        #region Servicios Inyectados

        [Inject] private DialogoErrores DialogoErrores { get; set; } = default!;

        #endregion

        #region Propiedades

        private List<Menu> Favoritos = [];

        #endregion

        #region Ciclo de Vida

        protected override async Task OnUsuarioDisponibleAsync()
        {
            try
            {
                await base.OnUsuarioDisponibleAsync();

                LayerOverlayService.Start($"{ObtenerTexto(AppResources.Common.Loading)} {TituloPagina}");
                
                ObtenerFavoritos(Usuario);
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Index] ? Error en OnUsuarioDisponibleAsync: {ex.Message}");
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await DialogoErrores.MostrarErrorInicializandoPagina(TituloPagina, ex);
            }
            finally
            {
                LayerOverlayService.Stop();
            }
        }

        protected override async Task OnUsuarioImpersonadoDesconectado()
        {
            try
            {
                LayerOverlayService.Start($"{ObtenerTexto(AppResources.Common.Loading)} {TituloPagina}");
                await base.OnUsuarioImpersonadoDesconectado();
                ObtenerFavoritos(Usuario);
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Index] ? Error en OnUsuarioLoginDesconectado: {ex.Message}");
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await DialogoErrores.MostrarErrorInicializandoPagina(TituloPagina, ex);
            }
            finally
            {
                LayerOverlayService.Stop();
            }
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Actualiza los menús favoritos del usuario
        /// </summary>
        private void ObtenerFavoritos(UsuarioEntidad usuario)
        {
            Favoritos = [.. usuario.Menus.Where(c => c.IsFavorite)];
        }


        #endregion
    }
}

