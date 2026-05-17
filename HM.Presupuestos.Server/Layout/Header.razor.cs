using HM.Presupuestos.Server.Componentes.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace HM.Presupuestos.Server.Layout
{
    public partial class Header : Context
    {
        [Inject] private IConfiguration Configuration { get; set; } = default!;
      
        private string VersionApp = string.Empty;

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

        protected override async Task OnUsuarioLoginDesconectado()
        {
            await base.OnUsuarioLoginDesconectado();
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

