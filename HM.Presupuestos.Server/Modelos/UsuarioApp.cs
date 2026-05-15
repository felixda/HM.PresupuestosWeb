
namespace HM.Presupuestos.Server.Modelos
{ 
    public class UsuarioApp
    {
        private UsuarioEntidad _usarioSSO = new();
        private UsuarioEntidad? _usuarioImpersonado = null;

        /// <summary>
        /// Devuelve el usuario activo en la aplicación. 
        /// Si hay un usuario impersonado, se devuelve ese; de lo contrario, se devuelve el usuario SSO.
        /// </summary>
        public UsuarioEntidad Usuario
        {
            get
            {
                if (_usuarioImpersonado != null)
                {
                    return _usuarioImpersonado;
                }
                else
                {
                    return _usarioSSO;
                }
            }
        }

        public void AsociarUsuarioSSO(UsuarioEntidad usuarioSSO)
        {
            _usarioSSO = usuarioSSO;
        }

        public void AsociarUsuarioImpersonado(UsuarioEntidad? usuarioImpersonado)
        {
            _usuarioImpersonado = usuarioImpersonado;
        }

    

        public void DesconectarUsuarioImpersonado()
        {
            _usuarioImpersonado = null;
          
        }

        public UsuarioEntidad ObtenerUsuarioSSO()
        {
            return _usarioSSO;
        }

        public UsuarioEntidad? ObtenerUsuarioImpersonado ()
        {
            return _usuarioImpersonado;
        }


     

    }
}
