using HM.Presupuestos.Web.Adaptadores.Sesion;

namespace HM.Presupuestos.Web.Adaptadores.Navegacion
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



