using Microsoft.AspNetCore.Components.Server.Circuits;

namespace HM.Presupuestos.Web.Adaptadores.Sesion
{
    public class CircuitSesionTracker(
        IRegistroSesionesActivas registroSesiones,
        ISesionUsuario sesionUsuario) : CircuitHandler
    {
        public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            var login = sesionUsuario.UsuarioApp?.UsuarioAutenticado?.Login;
            if (!string.IsNullOrEmpty(login))
            {
                registroSesiones.Eliminar(login);
            }

            return Task.CompletedTask;
        }
    }
}
