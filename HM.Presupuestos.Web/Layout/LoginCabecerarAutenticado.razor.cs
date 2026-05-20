
using HM.Core.Comun.v6.Entidades.Seguridad;

namespace HM.Presupuestos.Web.Layout;

public partial class LoginCabecerarAutenticado
{

    protected override async Task OnUsuarioDisponibleAsync()
    {
        await base.OnUsuarioDisponibleAsync();
        await InvokeAsync(StateHasChanged);
    }

}
