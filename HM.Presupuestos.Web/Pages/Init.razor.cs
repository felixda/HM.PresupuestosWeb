using HM.Core.Comun.v6.Entidades.Seguridad;
using HM.Core.Comun.v6.Seguridad.Interfaces;
using HM.Presupuestos.Web.Adaptadores.Sesion;
using HM.Presupuestos.Web.Adaptadores.Ui;

using HM.Presupuestos.Web.Componentes.Ui;
using Microsoft.AspNetCore.Components;

namespace HM.Presupuestos.Web.Pages
{
    public partial class Init
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IRegistroAplicacion LogService { get; set; } = default!;
        [Inject] private IAlmacenSesionUsuario SessionService { get; set; } = default!;
        [Inject] private ISesionUsuario SesionUsuario { get; set; } = default!;
        [Inject] private IGestorCookies GestorCookies { get; set; } = default!;
        [Inject] private IJwt Jwt { get; set; } = default!;

        private const string IDIOMA_COOKIE_KEY = "app_idioma";
        private const int IDIOMA_COOKIE_EXPIRE_DAYS = 365;
        private const string IDIOMA_POR_DEFECTO = "es";

        private bool _componentInitialized = false;
        private UsuarioEntidad _usuario = new UsuarioEntidad();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            InicializarIdiomaDesdeCookie();
        }

        private void InicializarIdiomaDesdeCookie()
        {
            try
            {
                var languageCode = GestorCookies.Obtener(IDIOMA_COOKIE_KEY);

                if (string.IsNullOrEmpty(languageCode))
                {
                    languageCode = IDIOMA_POR_DEFECTO;
                    GestorCookies.Grabar(IDIOMA_COOKIE_KEY, languageCode, IDIOMA_COOKIE_EXPIRE_DAYS);
                    Console.WriteLine($"[Init] Cookie de idioma creada con valor por defecto: {languageCode}");
                }
                else
                {
                    Console.WriteLine($"[Init] Cookie de idioma leÃ­da: {languageCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Init] Error al inicializar idioma desde cookie: {ex.Message}");
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                if (!_componentInitialized)
                {
                    _componentInitialized = true;
                    await InvokeAsync(StateHasChanged);
                }
            }
            else
            {
                var authorized = false;

                try
                {
                    authorized = await CrearPerfilUsuario();
                    if (authorized)
                    {
                        NavigationManager.NavigateTo("/home", forceLoad: false);
                    }
                    else
                    {
                        NavigationManager.NavigateTo("/Unauthorized", forceLoad: false);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Init] ? Exception: {ex.Message}");
                    NavigationManager.NavigateTo("/Unauthorized", true);
                    await LogService.RegistrarExcepcion(ex);
                }
            }
        }

        private async Task<bool> CrearPerfilUsuario()
        {
            var authorized = false;
            var usuarioSSO = await SesionUsuario.AutenticarUsuarioSSOAsync();

            if (usuarioSSO != null)
            {
                _usuario = usuarioSSO;
                Jwt.Usuario = _usuario;
                authorized = true;
            }

            return authorized;
        }

        
    }
}



