
namespace HM.Presupuestos.Web.Layout;

public partial class LoginCabeceraImpersonado
{

    #region Ciclo de vida

    protected override async Task OnUsuarioDisponibleAsync()
    {
        await base.OnUsuarioDisponibleAsync();
        await InvokeAsync(StateHasChanged);
    }

    protected override async Task OnUsuarioImpersonadoDesconectado()
    {
        await base.OnUsuarioImpersonadoDesconectado();
        await InvokeAsync(StateHasChanged);
    }

    #endregion

    #region MÃ©todos privados

    private async Task OnCerrarSesion()
    {
        await SesionUsuario.CerrarSesionLoginAsync();
        NavigationManager.NavigateTo("/Index");
    }

    #endregion
}
