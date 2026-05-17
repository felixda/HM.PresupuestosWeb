using HM.Presupuestos.Server.Extensiones;
using HM.Presupuestos.Server.Sesion;

namespace HM.Presupuestos.Server.Services
{

    public interface IControlAccesoNavegacion
    {
        bool PuedeAccederA(string url);
    }

    public class ControlAccesoNavegacion : IControlAccesoNavegacion
    {
        private HashSet<string> _urlsPermitidas = [];

        private readonly IMapaMenu _mapaMenu;
        private readonly ISesionUsuario _usuarioService;

        public ControlAccesoNavegacion(IMapaMenu mapaMenu, ISesionUsuario usuarioService)
        {
            _mapaMenu = mapaMenu;
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