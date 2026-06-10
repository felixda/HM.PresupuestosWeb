

namespace HM.Presupuestos.Web.Pages.Configuracion
{
    /// <summary>
    /// Página de configuración del año del diario de presupuestos
    /// Permite seleccionar el año activo para operaciones diarias
    /// </summary>
    public partial class AnioDiario : ContextProtegido
    {
        #region Inyección de Dependencias

        [Inject] protected IConfiguracionService ConfiguracionService { get; set; } = default!;

        #endregion

        #region Propiedades Privadas

        private string TextoToolTipAyuda { get; set; } = string.Empty;
        
        /// <summary>
        /// Lista de años disponibles (año anterior, actual y posterior)
        /// </summary>
        private List<CodigoDescripcion> Anios { get; set; } = [];
        
        /// <summary>
        /// Año actualmente seleccionado por el usuario
        /// </summary>
        private CodigoDescripcion? AnioSeleccionado { get; set; }
        
        /// <summary>
        /// Año original guardado en base de datos
        /// </summary>
        private CodigoDescripcion? AnioOriginal { get; set; }
        
        /// <summary>
        /// Indica si el año seleccionado es diferente al año original (hay cambios pendientes)
        /// </summary>
        private bool HayCambiosPendientes => 
            AnioSeleccionado is not null && 
            AnioSeleccionado.Codigo != AnioOriginal?.Codigo;


       // protected override CodigosMenu CodigoMenuPermiso => CodigosMenu.AnioDiario;

        #endregion

        #region Ciclo de Vida del Componente

        /// <summary>
        /// Se ejecuta cuando el usuario no tiene permisos para acceder a la página
        /// </summary>
        protected override Task OnPermisoDenegadoAsync()
        {
            return Task.CompletedTask;
        }

        protected override async Task InicializarPaginaAsync()
        {
            TextoToolTipAyuda = ObtenerTexto(TextosApp.Pages.AnioDiario.ToolTip);

            int añoActual = DateTime.Now.Year;
            Anios.Add(CrearAnio(añoActual + 1));
            Anios.Add(CrearAnio(añoActual));
            Anios.Add(CrearAnio(añoActual - 1));

            AnioOriginal = await ConfiguracionService.ObtenerAnioDiario();

            if (Anios.Count > 0)
            {
                AnioSeleccionado = Anios.Find(c => c.Codigo == AnioOriginal?.Codigo);
            }
        }

        /// <summary>
        /// Crea un objeto CodigoDescripcion para un año específico
        /// </summary>
        /// <param name="año">Año a crear</param>
        /// <returns>Objeto CodigoDescripcion con código y descripción del año</returns>
        private static CodigoDescripcion CrearAnio(int año)
        {
            return new CodigoDescripcion
            {
                Codigo = año,
                Descripcion = año.ToString()
            };
        }
       
        #endregion

        #region Eventos

        /// <summary>
        /// Maneja el evento de clic en el botón Grabar
        /// Actualiza el año del diario en la configuración
        /// </summary>
        private async Task Grabar_Click()
        {
            await EjecutarAsync(async () =>
            {
                await ConfiguracionService.ActualizarAnioDiario(AnioSeleccionado!.Codigo);
                AnioOriginal = AnioSeleccionado;
                await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(TextosApp.Common.DatosGrabados));
            });
        }

        #endregion
    }
}

