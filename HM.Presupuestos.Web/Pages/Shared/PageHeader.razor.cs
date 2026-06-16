using HM.Presupuestos.Application.CasosDeUso;

namespace HM.Presupuestos.Web.Pages.Shared
{
    public partial class PageHeader  
    {
        #region Inyecciones de Dependencias
        [Inject] protected NavigationManager Navigation { get; set; } = default!;
        [Inject] protected IAlmacenSesionUsuario AlmacenSesionUsuario { get; set; } = default!;
        [Inject] protected IMenuFavoritosService MenuFavoritosService { get; set; } = default!;
        #endregion

        #region Campos Privados

        private bool EsFavorito { get; set; } = false;

        #endregion

        #region Parámetros

        [Parameter]
        public CodigosMenu CodigoMenu { get; set; }

        [Parameter]
        public string Titulo { get; set; } = string.Empty;

        [Parameter]
        public string TextoToolTipAyuda { get; set; } = string.Empty;

        #endregion

        #region Ciclo de Vida del Componente

        protected override async Task OnUsuarioDisponibleAsync()
        {
            var menuActual = Usuario.Menus.FirstOrDefault(x => x.Id == (int)CodigoMenu);
            if (menuActual != null)
            {
                EsFavorito = menuActual.IsFavorite;
            }
            await InvokeAsync(StateHasChanged);
        }

        #endregion

        #region Eventos de Botones

        private async Task OnFavorito()
        {
            EsFavorito = !EsFavorito;
            await JSRuntime.InvokeVoidAsync("Page.SetMenuFavorite", "iconFavorite", EsFavorito, "star-fill", "star");
            await GestionarFavorito(EsFavorito);
        }

        private async Task OnVolver()
        {
            Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
            await JSRuntime.InvokeVoidAsync("history.back");
        }

        #endregion

        #region Gestión de Favoritos

        /// <summary>
        /// Gestiona los favoritos del usuario: agrega o elimina el menú actual
        /// Actualiza tanto la base de datos como la sesión del usuario
        /// </summary>
        public async Task GestionarFavorito(bool esFavorito)
        {
            if (Usuario?.Jwt == null) return;

            var codigoMenuActual = ((int)CodigoMenu).ToString();

            var favoritos = await ObtenerListaFavoritos();

            if (esFavorito)
            {
                favoritos.Add(codigoMenuActual);
            }
            else
            {
                favoritos.Remove(codigoMenuActual);
            }

            await GuardarListaFavoritos(favoritos);

            ActualizarEstadoMenu(codigoMenuActual, esFavorito);

            // Persistir cambios en sesión del usuario SSO. Del impersonado no se pueden modificar los favoritos
            await AlmacenSesionUsuario.GuardarUsuarioSSO(Usuario);
        }

        private async Task<HashSet<string>> ObtenerListaFavoritos()
        {
            return await MenuFavoritosService.ObtenerFavoritos();
        }

        private async Task GuardarListaFavoritos(HashSet<string> favoritos)
        {
            await MenuFavoritosService.GuardarFavoritos(favoritos);
        }

        private void ActualizarEstadoMenu(string codigoMenu, bool esFavorito)
        {
            var menuActual = Usuario!.Menus.FirstOrDefault(x => x.Id == int.Parse(codigoMenu));
            if (menuActual != null)
            {
                menuActual.IsFavorite = esFavorito;
            }
        }
        #endregion
    }
}

