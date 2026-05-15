using HM.Presupuestos.Server.Extensiones;

namespace HM.Presupuestos.Server.Layout
{
    public partial class NavMenu
    {

        #region Campos Bindeados a UI

        private List<ConfiguracionMenu> Menu { get; set; } = [];

        #endregion

        #region Ciclo de Vida del Componente

        protected override async Task OnUsuarioDisponibleAsync()
        {
            CrearMenu(Usuario);
            await EstablecerMenuActivo((int)CodigosMenu.Home);
            await InvokeAsync(StateHasChanged);
        }

        #endregion

        #region Métodos Privados


        /// <summary>
        /// Build ConfiguracionMenu object from user object
        /// </summary>
        private void CrearMenu(UsuarioEntidad user)
        {
            Menu = [];

            List<Menu> menusPadres = [.. user.Menus.Where(c => !c.TienePadre())];
            List<Menu> menusHijos = [.. user.Menus.Where(c => c.TienePadre())];

            Console.WriteLine($"[NavMenu] 📋 Creando menú - Padres: {menusPadres.Count}, Hijos: {menusHijos.Count}");

            foreach (var menu in menusPadres)
            {
                var menuPadre = new ConfiguracionMenu
                {
                    CodigoMenu = menu.Id,
                    CodigoMenuPadre = menu.IdPadre,
                    DescripcionMenu = menu.NombreMenu,
                    IndiceOrdenacion = menu.IndOrdenacion
                };

                Menu.Add(menuPadre);
                Console.WriteLine($"[NavMenu] 📁 Menú padre creado: {menu.Id} - {menu.NombreMenu}");
            }

            foreach (var menu in menusHijos)
            {
                var url = menu.Url(MapaMenu) ?? "";

                ConfiguracionMenu menuHijo = new ConfiguracionMenu
                {
                    CodigoMenu = menu.Id,
                    CodigoMenuPadre = menu.IdPadre,
                    DescripcionMenu = menu.NombreMenu,
                    IndiceOrdenacion = menu.IndOrdenacion,
                    Url = url
                };

                var menuPadre = Menu.FirstOrDefault(o => o.CodigoMenu == menu.IdPadre);

                if (menuPadre != null)
                {
                    menuPadre.Submenus.Add(menuHijo);
                    Console.WriteLine($"[NavMenu] ✅ Submenú agregado: {menu.Id} - {menu.NombreMenu} (Padre: {menu.IdPadre}, URL: {url})");
                }
                else
                {
                    Console.WriteLine($"[NavMenu] ⚠️ Padre no encontrado para submenú: {menu.Id} - IdPadre: {menu.IdPadre}");
                }
            }

            foreach (var menu in Menu)
            {
                Console.WriteLine($"[NavMenu] 🔍 Menú final: {menu.CodigoMenu} tiene {menu.Submenus.Count} submenús");
            }
        }

        private async void OnMenuExpandido(TreeViewNodeEventArgs e)
        {
            var urlNormalizada = RutasNavegacion.ObtenerRutaActualNormalizada();

            if (!string.IsNullOrEmpty(urlNormalizada))
            {
                var codigoMenuActivo = MapaMenu.ObtenerCodigoMenuPorUrl(urlNormalizada);

                if (codigoMenuActivo > 0)
                {
                    await EstablecerMenuActivo(codigoMenuActivo);
                }
            }
        }

        private async Task OnNavegarA(string url)
        {
            try
            {
                if (!string.IsNullOrEmpty(url))
                {
                    string urlActual = RutasNavegacion.ObtenerRutaActual();

                    if (url == urlActual)
                    {
                        return;
                    }
                    NavigationManager.NavigateTo(url);
                }
            }
            catch (Exception ex)
            {
                await LogService.RegistrarExcepcion(GetType().Name, ex);
            }
        }

        #endregion
    }
}
