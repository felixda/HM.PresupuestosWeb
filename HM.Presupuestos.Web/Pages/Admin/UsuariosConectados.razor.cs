using HM.Presupuestos.Web.Adaptadores.Sesion;

namespace HM.Presupuestos.Web.Pages.Admin
{
    public partial class UsuariosConectados
    {
        #region Inyección de Dependencias

        [Inject] private IRegistroSesionesActivas RegistroSesiones { get; set; } = default!;

        #endregion

        #region Propiedades Privadas

        private IReadOnlyList<SesionActivaInfo> _sesiones = [];

        #endregion

        #region Ciclo de Vida

        protected override Task InicializarPaginaAsync()
        {
            _sesiones = RegistroSesiones.ObtenerTodas();
            return Task.CompletedTask;
        }

        protected override Task OnPermisoDenegadoAsync()
        {
            return Task.CompletedTask;
        }

        #endregion

        #region Métodos Privados

        private Task ActualizarAsync()
        {
            _sesiones = RegistroSesiones.ObtenerTodas();
            return Task.CompletedTask;
        }

        private static string FormatearTiempo(SesionActivaInfo sesion)
        {
            var duracion = DateTime.UtcNow - sesion.Inicio;
            if (duracion.TotalHours >= 1)
                return $"{(int)duracion.TotalHours}h {duracion.Minutes}m";
            if (duracion.TotalMinutes >= 1)
                return $"{(int)duracion.TotalMinutes}m {duracion.Seconds}s";
            return $"{duracion.Seconds}s";
        }

        #endregion
    }
}
