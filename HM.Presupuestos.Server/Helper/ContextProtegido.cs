
namespace HM.Presupuestos.Server.Helper
{
    public class ContextProtegido : Context, IDisposable
    {
        [Inject] protected NavigationManager? NavigationManager { get; set; } = default!;

        [Inject] protected IPermisosService PermisosService { get; set; } = default!;

        [Inject] protected INavigationService NavigationService { get; set; } = default!;



        protected bool TienePermiso { get; private set; } = false;
        protected bool ValidandoPermisos { get; private set; } = true;

        // ✅ Constructor: D
        public ContextProtegido()
        {
        }

        protected override async Task OnUsuarioDisponibleAsync()
        {
            UsuarioApp = UsuarioService.UsuarioApp!;
            if (UsuarioApp is not null)
            {
                PermisosService.EstablecerMenus(UsuarioApp);

                string url = NavigationManager!.ToBaseRelativePath(NavigationManager.Uri);
                var urlNormalizada = NavigationService.NormalizarUrl(url);

                TienePermiso = PermisosService.TienePermiso(urlNormalizada);
                ValidandoPermisos = false;

                if (TienePermiso)
                {
                    await OnPermisoValidadoAsync();
                }
                else
                {
                    await RegistrarAccesoNoAutorizado(url);
                    await OnPermisoDenegadoAsync();
                }
            }
        }


        /// <summary>
        /// Registrar intento de acceso no autorizado
        /// </summary>
        private async Task RegistrarAccesoNoAutorizado(string url)
        {
            try
            {
                var logAccion = CrearLogAccion(
                    codigoUsuario: UsuarioApp!.Usuario.CodigoUsuario,
                    nombreMetodoLlamador: nameof(RegistrarAccesoNoAutorizado),
                    accion: AccionesLog.IntentoAccesoNoAutorizado,
                    objetoConParametros: new
                    {
                        Url = url,
                        Usuario = UsuarioApp.Usuario.Login,
                        FechaHora = DateTime.Now
                    }
                );

                await LogService.RegistrarAccesoNoAutorizado(logAccion);

                Console.WriteLine($"[MainLayout] ⚠️ Acceso denegado registrado: {UsuarioApp.Usuario.Login} -> ({url})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MainLayout] ❌ Error al registrar acceso no autorizado: {ex.Message}");
                await LogService.InsertException(nameof(ContextProtegido), ex);
            }
        }

        /// <summary>
        /// Sobrescribir este método en las páginas que heredan para cargar datos si el usuario tiene permiso
        /// </summary>
        protected virtual Task OnPermisoValidadoAsync() => Task.CompletedTask;
        protected virtual Task OnPermisoDenegadoAsync() => Task.CompletedTask;

    }
}