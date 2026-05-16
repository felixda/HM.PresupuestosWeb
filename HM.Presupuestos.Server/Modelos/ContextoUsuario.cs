
namespace HM.Presupuestos.Server.Modelos
{ 
    public class ContextoUsuario
    {
        private UsuarioEntidad _usarioAutenticado = new();
        private UsuarioEntidad? _usuarioImpersonado = null;

        /// <summary>
        /// Devuelve el usuario activo en la aplicación. 
        /// Si hay un usuario impersonado, se devuelve ese; de lo contrario, se devuelve el usuario SSO.
        /// </summary>
        public UsuarioEntidad UsuarioActivo
        {
            get
            {
                if (_usuarioImpersonado != null)
                {
                    return _usuarioImpersonado;
                }
                else
                {
                    return _usarioAutenticado;
                }
            }
        }

        public void AsignarUsuarioAutenticado(UsuarioEntidad usuarioAutenticado)
        {
            _usarioAutenticado = usuarioAutenticado;
        }

        public void AsignarUsuarioImpersonado(UsuarioEntidad? usuarioImpersonado)
        {
            _usuarioImpersonado = usuarioImpersonado;
        }

    

        public void DesactivarUsuarioImpersonado()
        {
            _usuarioImpersonado = null;
          
        }


        public UsuarioEntidad UsuarioAutenticado
        {
            get { return _usarioAutenticado; }
        }

        public UsuarioEntidad? UsuarioImpersonado
        {
            get { return _usuarioImpersonado; }
        }
        public UsuarioEntidad? ObtenerUsuarioImpersonado ()
        {
            return _usuarioImpersonado;
        }


     

    }
}
