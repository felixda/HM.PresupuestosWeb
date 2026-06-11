namespace HM.Presupuestos.Web.Adaptadores.Navegacion
{

    public interface IControlAccesoNavegacion
    {
        bool PuedeAccederA(string url);
    }

    public class ControlAccesoNavegacion(
        IRecursosApp recursosApp,
        ISesionUsuario usuarioService) : IControlAccesoNavegacion
    {
        private HashSet<string> _urlsPermitidas = [];

        private readonly IRecursosApp _recursosApp = recursosApp;
        private readonly ISesionUsuario _usuarioService = usuarioService;


        private void CargarUrlsPermitidasSiEsNecesario(ContextoUsuario usuarioApp)
        {
            if (_urlsPermitidas.Count > 0)
                return;

            var urlsPermitidas = usuarioApp.UsuarioActivo.Menus
                .Where(menu => menu.TienePadre())
                .Select(menu => menu.Url(_recursosApp))
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




