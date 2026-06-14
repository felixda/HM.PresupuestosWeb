
namespace HM.Presupuestos.Web.Adaptadores.Sesion
{
    public class ContextoUsuario
    {
        private UsuarioEntidad _usuarioAutenticado = new();
        private UsuarioEntidad? _usuarioImpersonado = null;

        /// <summary>
        /// Devuelve el usuario activo en la aplicaciÃ³n.
        /// Si hay un usuario impersonado, se devuelve ese; de lo contrario, se devuelve el usuario autenticado.
        /// </summary>
        public UsuarioEntidad UsuarioActivo
        {
            get
            {
                return _usuarioImpersonado ?? _usuarioAutenticado;
            }
        }

        public UsuarioEntidad UsuarioAutenticado => _usuarioAutenticado;

        public UsuarioEntidad? UsuarioImpersonado => _usuarioImpersonado;

        public void AsignarUsuarioAutenticado(UsuarioEntidad usuarioAutenticado)
        {
            _usuarioAutenticado = usuarioAutenticado;
        }

        public void AsignarUsuarioImpersonado(UsuarioEntidad? usuarioImpersonado)
        {
            _usuarioImpersonado = usuarioImpersonado;
        }

        public void DesactivarUsuarioImpersonado()
        {
            _usuarioImpersonado = null;
        }

        public UsuarioEntidad? ObtenerUsuarioImpersonado()
        {
            return _usuarioImpersonado;
        }
    }
}



