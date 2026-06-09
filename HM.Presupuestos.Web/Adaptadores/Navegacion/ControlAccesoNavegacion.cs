using HM.Presupuestos.Web.Adaptadores.Sesion;
using HM.Presupuestos.Web.Adaptadores.Navegacion;
using HM.Presupuestos.Web.Adaptadores.Sesion;

namespace HM.Presupuestos.Web.Adaptadores.Navegacion
{

    public interface IControlAccesoNavegacion
    {
        bool PuedeAccederA(string url);
    }

    public class ControlAccesoNavegacion : IControlAccesoNavegacion
    {
        private HashSet<string> _urlsPermitidas = [];

        private readonly IRecursosApp _mapaMenu;
        private readonly ISesionUsuario _usuarioService;

        public ControlAccesoNavegacion(IRecursosApp RecursosApp, ISesionUsuario usuarioService)
        {
            _mapaMenu = RecursosApp;
            _usuarioService = usuarioService;
        }


        private void CargarUrlsPermitidasSiEsNecesario(ContextoUsuario usuarioApp)
        {
            if (_urlsPermitidas.Count > 0)
                return;

            var urlsPermitidas = usuarioApp.UsuarioActivo.Menus
                .Where(menu => menu.TienePadre())
                .Select(menu => menu.Url(_mapaMenu))
                .Where(url => !string.IsNullOrWhiteSpace(url));

            _urlsPermitidas = new HashSet<string>(
                urlsPermitidas!,
                StringComparer.OrdinalIgnoreCase
            );
        }


        public bool PuedeAccederA(string url)
        {
            CargarUrlsPermitidasSiEsNecesario(_usuarioService.UsuarioApp!);
            return _urlsPermitidas.Contains(url, StringComparer.OrdinalIgnoreCase);
        }

    }
}




