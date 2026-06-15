namespace HM.Presupuestos.Web.Pages.Admin
{
    public partial class Avisos
    {
        #region Inyección de Dependencias

        [Inject] protected IAvisosService AvisosService { get; set; } = default!;
        [Inject] protected ILogAccionesService LogAccionesService { get; set; } = default!;

        #endregion

        #region Propiedades Privadas

        private string Mensaje { get; set; } = string.Empty;
        private TiposDeAviso TipoAviso { get; set; } = TiposDeAviso.Warning;

        #endregion

        #region Ciclo de Vida

        protected override Task InicializarPaginaAsync()
        {
            return Task.CompletedTask;
        }

        protected override Task OnPermisoDenegadoAsync()
        {
            return Task.CompletedTask;
        }

        #endregion

        #region Métodos Privados

        private async Task EnviarAvisoAsync()
        {
            await EjecutarAsync(async () =>
            {
                await AvisosService.ActivarAvisosAsync(Mensaje, TipoAviso);
                await LogAccionesService.Insertar(AccionesLog.EnviarAviso, Mensaje);
            });
        }

        #endregion

        #region Propiedades Calculadas

        private string ClaseAviso => TipoAviso switch
        {
            TiposDeAviso.Info => "info",
            TiposDeAviso.Success => "success",
            TiposDeAviso.Warning => "warning",
            TiposDeAviso.Error => "error",
            _ => "warning"
        };

        private string ClaseTextArea => $"form-control textarea-{ClaseAviso}";

        private string ClaseMarco => TipoAviso switch
        {
            TiposDeAviso.Info => "radio-card-info",
            TiposDeAviso.Success => "radio-card-success",
            TiposDeAviso.Warning => "radio-card-warning",
            TiposDeAviso.Error => "radio-card-error",
            _ => "radio-card-warning"
        };

        #endregion
    }
}


