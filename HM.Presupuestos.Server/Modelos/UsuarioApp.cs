
namespace HM.Presupuestos.Server.Modelos
{ 
    public class UsuarioApp
    {
        private UsuarioEntidad _usarioSSO = new();
        private UsuarioEntidad? _usuarioLogin = null;

        public UsuarioEntidad Usuario
        {
            get
            {
                if (_usuarioLogin != null)
                {
                    return _usuarioLogin;
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

        public void AsociarUsuarioLogin(UsuarioEntidad? usuarioLogin)
        {
            _usuarioLogin = usuarioLogin;
        }

    

        public void DesconectarUsuarioLogin()
        {
            _usuarioLogin = null;
          
        }

        public UsuarioEntidad ObtenerUsuarioSSO()
        {
            return _usarioSSO;
        }

        public UsuarioEntidad? ObtenerUsuarioLogin ()
        {
            return _usuarioLogin;
        }


     

    }
}
