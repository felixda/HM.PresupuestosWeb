
namespace HM.Presupuestos.Web.Layout
{
    public partial class Cabecera : Context
    {
        [Inject] private IConfiguration Configuration { get; set; } = default!;
      
        private string VersionApp { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            SesionUsuario.UsuarioCargado += async () =>
            {
                await InvokeAsync(StateHasChanged);
            };
        }

        protected override async Task OnUsuarioDisponibleAsync()
        {
            var assembly = typeof(MainLayout).Assembly;
            var version = assembly.GetName().Version?.ToString() ?? "1.0.0";
            var fecha = File.GetLastWriteTime(assembly.Location);

            VersionApp = $"{version} ({fecha:dd/MM/yyyy HH:mm})";
            await InvokeAsync(StateHasChanged);
        }

        protected override async Task OnUsuarioImpersonadoDesconectado()
        {
            await base.OnUsuarioImpersonadoDesconectado();
            await InvokeAsync(StateHasChanged);
        }

        public override void Dispose()
        {
            SesionUsuario.UsuarioCargado -= async () =>
            {
                await InvokeAsync(StateHasChanged);
            };

            base.Dispose();
            GC.SuppressFinalize(this);
        }

        async Task OnToolbarItemClick()
        {
            await JSRuntime.InvokeVoidAsync("Menu.ToggleMenuVisibility");
        }
    }
}


