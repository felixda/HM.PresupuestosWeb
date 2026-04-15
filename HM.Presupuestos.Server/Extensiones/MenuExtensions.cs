using HM.Core.Comun.v6.Entidades.Seguridad;
using HM.Presupuestos.Contratos.Comun;
using HM.Presupuestos.Server.Services;


namespace HM.Presupuestos.Server.Extensiones
{
    public static class MenuExtensions
    {
        public static string? Url(this Menu menu, IResourceService resourceService)
        {
            int id = menu.Id;
            return resourceService.BuscarUrlMenuPorCodeMenuEnJson(id);
        }

    }
}
