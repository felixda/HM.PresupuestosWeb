

namespace HM.Presupuestos.Server.Pages.Configuracion
{
    /// <summary>
    /// Página de configuración del año del diario de presupuestos
    /// Permite seleccionar el año activo para operaciones diarias
    /// </summary>
    public partial class AnioDiario : ContextProtegido
    {
        #region Inyección de Dependencias

        [Inject] protected MensajesHelper MensajesHelper { get; set; } = default!;
        [Inject] protected ErrorDialogService ErrorService { get; set; } = default!;
        [Inject] protected ILayerOverlayService LayerOverlayService { get; set; } = default!;
        [Inject] protected IConfiguracionService ConfiguracionService { get; set; } = default!;

        #endregion

        #region Propiedades Privadas

        private string TituloPagina { get; set; } = string.Empty;
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
            Console.WriteLine("[AnioDiario] ❌ Permiso denegado");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Se ejecuta cuando el usuario tiene permisos válidos para acceder
        /// Inicializa la página y carga los datos necesarios
        /// </summary>
        //protected override async Task OnPermisoValidadoAsync()
        //{
        //    try
        //    {
        //        TituloPagina = ObtenerTexto(AppResources.Menu.ObtenerEtiqueta((int)CodigosMenu.AnioDiario));
        //        LayerOverlayService.Start($"{ObtenerTexto(AppResources.Common.Loading)} {TituloPagina}");

        //        await InicializarPaginaAsync();

        //        Console.WriteLine("[AnioDiario] ✅ OnPermisoValidadoAsync completado");

        //        await InvokeAsync(StateHasChanged);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"[AnioDiario] ❌ Error en OnPermisoValidadoAsync: {ex.Message}");
        //        Console.WriteLine($"[AnioDiario] StackTrace: {ex.StackTrace}");
        //        await LogService.InsertException(ex);

        //        await ErrorService.MostrarErrorInicializandoPagina(TituloPagina, ex);
        //    }
        //    finally
        //    {
        //        LayerOverlayService.Stop();
        //    }
        //}


        /// <summary>
        /// ✅ Solo implementas la lógica específica de inicialización
        /// El try-catch-finally, overlay y logging se manejan automáticamente
        /// </summary>
        protected override async Task InicializarPaginaAsync()
        {
            TextoToolTipAyuda = ObtenerTexto(AppResources.Pages.AnioDiario.ToolTip);

            // Generar lista de años: año anterior, actual y posterior
            int añoActual = DateTime.Now.Year;
            Anios.Add(CrearAnio(añoActual + 1));
            Anios.Add(CrearAnio(añoActual));
            Anios.Add(CrearAnio(añoActual - 1));

            // Cargar año guardado en configuración
            AnioOriginal = await ConfiguracionService.ObtenerAnioDiario();

            // Seleccionar el año original por defecto
            if (Anios.Count > 0)
            {
                AnioSeleccionado = Anios.Find(c => c.Codigo == AnioOriginal?.Codigo);
            }
        }


        /// <summary>
        /// Inicializa la página cargando el año actual desde la configuración
        /// y generando la lista de años disponibles (anterior, actual, posterior)
        /// </summary>      
        //private async Task InicializarPaginaAsync()
        //{
        //    try
        //    {
        //        TextoToolTipAyuda = ObtenerTexto(AppResources.Pages.AnioDiario.ToolTip);

        //        // Generar lista de años: año anterior, actual y posterior
        //        int añoActual = DateTime.Now.Year;
        //        Anios.Add(CrearAnio(añoActual + 1));
        //        Anios.Add(CrearAnio(añoActual));
        //        Anios.Add(CrearAnio(añoActual - 1));

        //        // Cargar año guardado en configuración
        //        AnioOriginal = await ConfiguracionService.ObtenerAnioDiario();

        //        // Seleccionar el año original por defecto
        //        if (Anios.Count > 0)
        //        {
        //            AnioSeleccionado = Anios.Find(c => c.Codigo == AnioOriginal?.Codigo);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"[AnioDiario] ❌ Error en InicializarPaginaAsync: {ex.Message}");
        //        await LogService.InsertException(ex);
        //        throw;
        //    }
        //}

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
            try
            {
                LayerOverlayService.Start();
                
                await ConfiguracionService.ActualizarAnioDiario(AnioSeleccionado!.Codigo);
                
                // Actualizar año original después del guardado exitoso
                AnioOriginal = AnioSeleccionado;
                
                await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(AppResources.Common.DatosGrabados));
            }
            catch (Exception ex)
            {
                await LogService.InsertException(ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina);
            }
            finally
            {
                LayerOverlayService.Stop();
            }
        }

        #endregion
    }
}
