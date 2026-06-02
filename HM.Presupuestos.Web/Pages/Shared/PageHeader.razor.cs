using HM.Core.Comun.v6.Entidades.Configuracion;
using HM.Presupuestos.Infrastructure.Servicios;

namespace HM.Presupuestos.Web.Pages.Shared
{
    public partial class PageHeader  
    {
        #region Inyecciones de Dependencias
        [Inject] protected NavigationManager Navigation { get; set; } = default!;
        [Inject] protected IAlmacenSesionUsuario AlmacenSesionUsuario { get; set; } = default!;
        [Inject] protected IClienteApiCore ClienteApiCore { get; set; } = default!;
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

        /// <summary>
        /// Se ejecuta cuando el usuario está disponible y cargado
        /// Inicializa el estado de favorito del menú actual
        /// </summary>
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

        /// <summary>
        /// Maneja el clic en el botón de favorito
        /// Alterna el estado y actualiza la interfaz
        /// </summary>
        private async Task OnFavorito()
        {
            EsFavorito = !EsFavorito;
            await JSRuntime.InvokeVoidAsync("Page.SetMenuFavorite", "iconFavorite", EsFavorito, "star-fill", "star");
            await GestionarFavorito(EsFavorito);
        }

        /// <summary>
        /// Navega a la página anterior usando el historial del navegador
        /// </summary>
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
        /// <param name="esFavorito">True para marcar como favorito, False para desmarcar</param>
        public async Task GestionarFavorito(bool esFavorito)
        {
            if (Usuario?.Jwt == null) return;

            var codigoMenuActual = ((int)CodigoMenu).ToString();

            // Obtener lista actual de favoritos
            var favoritos = await ObtenerListaFavoritos();

            // Actualizar lista según la acción
            if (esFavorito)
            {
                favoritos.Add(codigoMenuActual);
            }
            else
            {
                favoritos.Remove(codigoMenuActual);
            }

            // Guardar en base de datos
            await GuardarListaFavoritos(favoritos);

            // Actualizar estado del menú en sesión
            ActualizarEstadoMenu(codigoMenuActual, esFavorito);

            // Persistir cambios en sesión del usuario SSO. Del impersonado no se pueden modificar los favoritos
            await AlmacenSesionUsuario.GuardarUsuarioSSO(Usuario);
        }

        /// <summary>
        /// Obtiene la lista de códigos de menús favoritos del usuario
        /// </summary>
        /// <returns>HashSet con los códigos de menús favoritos</returns>
        private async Task<HashSet<string>> ObtenerListaFavoritos()
        {
            var favoritosTexto = await ClienteApiCore.ObtenerCodigosDeMenusFavoritos();

            if (string.IsNullOrWhiteSpace(favoritosTexto))
            {
                return [];
            }

            return favoritosTexto
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToHashSet<string>(StringComparer.Ordinal);
        }

        /// <summary>
        /// Guarda la lista de favoritos en el perfil del usuario en base de datos
        /// </summary>
        /// <param name="favoritos">HashSet con códigos de menús favoritos</param>
        private async Task GuardarListaFavoritos(HashSet<string> favoritos)
        {
            var configuracion = new ElementoConfiguracion
            {
                Nombre = Constantes.UserConfiguration.MENU_FAVORITES,
                Valor = string.Join(",", favoritos)
            };

            await ClienteApiCore.GuardarCodigosDeMenusFavoritos(configuracion);
        }

        /// <summary>
        /// Actualiza el estado de favorito del menú actual en la colección de menús del usuario
        /// </summary>
        /// <param name="codigoMenu">Código del menú a actualizar</param>
        /// <param name="esFavorito">Nuevo estado de favorito</param>
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

