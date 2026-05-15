using HM.Presupuestos.Server.Extensiones;
using HM.Presupuestos.Server.Modelos;
using HM.Presupuestos.Server.Servicios;

namespace HM.Presupuestos.Server.Services
{

    public interface IPermisosService
    {
        bool PuedeAccederA(string url);
    }

    public class PermisosService : IPermisosService
    {
        private HashSet<string> _urlsPermitidas = [];

        private readonly ILocalizadorRecursos _resourceService;
        private readonly ISesionUsuario _usuarioService;

        public PermisosService(ILocalizadorRecursos resourceService, ISesionUsuario usuarioService)
        {
            _resourceService = resourceService;
            _usuarioService = usuarioService;
        }


        private void CargarUrlsPermitidasSiEsNecesario(UsuarioApp usuarioApp)
        {
            if (_urlsPermitidas.Count > 0)
                return;

            var urlsPermitidas = usuarioApp.Usuario.Menus
                .Where(menu => menu.TienePadre())
                .Select(menu => menu.Url(_resourceService))
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