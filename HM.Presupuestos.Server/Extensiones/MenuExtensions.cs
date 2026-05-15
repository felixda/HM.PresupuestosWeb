
namespace HM.Presupuestos.Server.Extensiones
{
    public static class MenuExtensions
    {
        public static string? Url(this Menu menu, ILocalizadorRecursos resourceService)
        {
            int id = menu.Id;
            return resourceService.ObtenerUrlMenu(id);
        }

        public static bool TienePadre(this Menu menu) => menu.IdPadre != null;

    }
}
