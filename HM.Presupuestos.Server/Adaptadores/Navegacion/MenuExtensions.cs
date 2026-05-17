using HM.Presupuestos.Server.Adaptadores.Sesion;

namespace HM.Presupuestos.Server.Adaptadores.Navegacion
{
    public static class MenuExtensions
    {
        public static string? Url(this Menu menu, IMapaMenu mapaMenu)
        {
            return mapaMenu.ObtenerUrlMenu(menu.Id);
        }

        public static bool TienePadre(this Menu menu) => menu.IdPadre != null;
    }
}


