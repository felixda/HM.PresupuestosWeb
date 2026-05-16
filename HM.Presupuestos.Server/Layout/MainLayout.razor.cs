
namespace HM.Presupuestos.Server.Layout
{
    public partial class MainLayout
    {
        #region Servicios Inyectados

        [Inject] private ILocalizadorRecursos LocalizadorRecursos { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private ControlInactividad ControlInactividad { get; set; } = default!;
        [Inject] private JsInteropHelper InteropInactividad { get; set; } = default!;
        [Inject] private IConfiguration Configuracion { get; set; } = default!;
        [Inject] private TraduccionesHelper TraduccionesHelper { get; set; } = default!;
        [Inject] private IControlCambiosNavegacion ControlCambiosNavegacion { get; set; } = default!;
        [Inject] private ISesionUsuario SesionUsuario { get; set; } = default!;
        [Inject] private DialogoErrores DialogoErrores { get; set; } = default!;
        [Inject] private IRegistroAplicacion RegistroAplicacion { get; set; } = default!;
        [Inject] private IGestorIdioma GestorIdioma { get; set; } = default!;
        [Inject] private IRutasNavegacion RutasNavegacion { get; set; } = default!;
        [Inject] private IMapaMenu MapaMenu { get; set; } = default!;

        #endregion

        #region Propiedades para Binding (PascalCase)

        /// <summary>
        /// Popup de error - Se utiliza en el razor
        /// </summary>
        private DxPopup? ErrorPopup { get; set; }
        
        private bool MostrarError { get; set; }
        private string MensajeError { get; set; } = string.Empty;
        private string TituloVentanaError { get; set; } = string.Empty;

        private bool MenuAbierto { get; set; } = true;
        private DrawerMode ModoMenu { get; set; } = DrawerMode.Shrink;
        private DrawerPosition PosicionMenu { get; set; } = DrawerPosition.Left;

        /// <summary>
        /// Controla la visibilidad del modal de advertencia de inactividad
        /// </summary>
        private bool MostrarAvisoInactividad { get; set; } = false;
        
        /// <summary>
        /// Tiempo restante en milisegundos para la cuenta atrás
        /// </summary>
        private int TiempoRestante { get; set; }

        /// <summary>
        /// Título del modal de advertencia
        /// </summary>
        private string TituloAvisoInactividad { get; set; } = string.Empty;
        
        /// <summary>
        /// Texto para la cuenta atrás
        /// </summary>
        private string TextoCuentaAtras { get; set; } = string.Empty;

        /// <summary>
        /// Referencia al componente modal de control de cambios
        /// </summary>
        private ModalControlCambios? ModalConfirmacionCambios { get; set; }

        #endregion

        #region Propiedades Internas (camelCase con _)

        private bool EsHome => Navigation.Uri.ToLower().EndsWith("/home");
        private bool _eventosJavascriptSuscritos = false;
        private string _ultimaUrlRegistrada = "";
        private bool _layoutInicializado = false;
        private string _ultimaRutaVisitada = "";

        #endregion

        #region Popup para mostrar error en la inicialización de las páginas

        /// <summary>
        /// Evento para mostrar un popup si se ha producido un error en la carga de la pantalla
        /// </summary>
        private async Task MostrarPopupError(string nombrePantalla, Exception ex)
        {
            TituloVentanaError = $"{nombrePantalla}";
            MensajeError = await TraduccionesHelper.GetResourceValue("mensajes:ErrorAbriendoPantalla:label");
            MostrarError = true;
            await InvokeAsync(StateHasChanged);
            await RegistroAplicacion.RegistrarExcepcion(this.GetType().Name, ex);
        }

        private void CerrarPopupError()
        {
            MostrarError = false;
            Navigation.NavigateTo("/home", forceLoad: false);
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Se ejecuta ANTES de renderizar - solo inicialización básica
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            try
            {
                Navigation.LocationChanged -= OnLocationChangedAsync;
                Navigation.LocationChanged += OnLocationChangedAsync;

                DialogoErrores.OnError += MostrarPopupError;

                // Para indicar la función que se mostrará al cambiar de páginas en la navegación y se ha indicado que hay cambios
                ControlCambiosNavegacion.RegistrarConfirmacionSalida(MostrarConfirmacionCambiosPendientes);

                GestorIdioma.IdiomaCambiado += async () =>
                {
                    await InvokeAsync(StateHasChanged);
                };

                Console.WriteLine("[MainLayout] ✅ OnInitializedAsync completado");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MainLayout] ❌ Error en OnInitializedAsync: {ex.Message}");
                await RegistroAplicacion.RegistrarExcepcion(nameof(MainLayout), ex);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Console.WriteLine("[MainLayout] 🔄 OnAfterRenderAsync (firstRender: true)");
                
                try
                {
                    await SesionUsuario.InicializarUsuarioAsync();

                    TituloAvisoInactividad = AppResources.Pages.ModalInactividad.Titulo; //  LocalizadorRecursos.ObtenerTexto("Pages:ModalInactividad:Titulo:label");
                    TextoCuentaAtras = AppResources.Pages.ModalInactividad.CuentaAtras; //  LocalizadorRecursos.ObtenerTexto("Pages:ModalInactividad:CuentaAtras:label");

                    await ActualizarSubscripcionesInactividad();
                }
                catch (Exception ex)
                {
                    await RegistroAplicacion.RegistrarExcepcion(this.GetType().Name, ex);
                }
            }
        }

        #endregion

        #region Navigation

        private async void OnLocationChangedAsync(object? sender, LocationChangedEventArgs e)
        {
            Console.WriteLine($"[MainLayout] 🔄 OnLocationChangedAsync: {e.Location}");
            
            try
            {
                var urlNormalizada = RutasNavegacion.NormalizarRuta(e.Location);
                if (urlNormalizada == _ultimaRutaVisitada)
                {
                    Console.WriteLine($"[MainLayout] ⏭️ URL duplicada en navegación, saltando procesamiento: {urlNormalizada}");
                    return;
                }

                _ultimaRutaVisitada = urlNormalizada;

                // ✅ NO registrar log si estamos en Index/Home
                if (!EsPaginaIndex(urlNormalizada))
                {
                    await RegistrarAccesoPagina(urlNormalizada);
                }
                else
                {
                    Console.WriteLine($"[MainLayout] ⏭️ Página Index/Home detectada, no se registra log de acceso");
                }

                await ActualizarSubscripcionesInactividad();
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MainLayout] ❌ Error en OnLocationChangedAsync: {ex.Message}");
                await RegistroAplicacion.RegistrarExcepcion(nameof(MainLayout), ex);
            }
        }

        /// <summary>
        /// Verifica si la URL corresponde a la principal (Index/Home) 
        /// para evitar registrar logs de acceso a la página de inicio
        /// </summary>
        private static bool EsPaginaIndex(string urlNormalizada)
        {
            if (string.IsNullOrEmpty(urlNormalizada))
            {
                return true;
            }

            var urlLower = urlNormalizada.ToLower().Trim('/');

            return urlLower == string.Empty ||
                   urlLower == "home" ||
                   urlLower == "index";
        }

        /// <summary>
        /// Controla la navegación interna dentro de la aplicación, 
        /// mostrando un modal de confirmación si hay cambios pendientes
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task OnBeforeInternalNavigation(LocationChangingContext context)
        {
            bool permitir = await ControlCambiosNavegacion.PuedeAbandonarPagina(context.TargetLocation);

            if (!permitir)
            {
                context.PreventNavigation();
            }
            else
            {
                ControlCambiosNavegacion.LimpiarCambiosPendientes();
            }
        }

        private async Task<bool> MostrarConfirmacionCambiosPendientes(string destino)
        {
            if (ModalConfirmacionCambios is not null)
            {
                string mensaje = AppResources.Mensajes.AvisoCambiosPendientes;// await TraduccionesHelper.GetResourceValue("Mensajes:AvisoCambiosPendientes:label");
                return await ModalConfirmacionCambios.Show(mensaje);
            }

            return true;
        }

        private async Task RegistrarAccesoPagina(string urlNormalizada)
        {
            try
            {
                // Evitar registrar duplicados consecutivos
                if (urlNormalizada == _ultimaUrlRegistrada)
                {
                    Console.WriteLine($"[MainLayout] ⏭️ URL duplicada, no se registra: {urlNormalizada}");
                    return;
                }

                // Obtener label del menú para la URL
                string tituloParaLog = MapaMenu.ObtenerEtiquetaMenuPorUrl(urlNormalizada);

                await RegistroAplicacion.RegistrarAccesoAPagina(tituloParaLog);

                // ✅ Actualizar última URL DESPUÉS de registrar exitosamente
                _ultimaUrlRegistrada = urlNormalizada;

                Console.WriteLine($"[MainLayout] ✅ Log registrado: {tituloParaLog} ({urlNormalizada})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MainLayout] ⚠️ Error al registrar log: {ex.Message}");
                await RegistroAplicacion.RegistrarExcepcion(nameof(MainLayout), ex);
            }
        }

        #endregion

        #region Mensaje Inactividad

        private async Task ActualizarSubscripcionesInactividad()
        {
            if (EsHome)
            {
                ControlInactividad.OnInactividadIniciada -= MostrarModalAdvertenciaInactividad;
                ControlInactividad.OnCuentaRegresiva -= ActualizarCuentaAtras;
                ControlInactividad.OnInactividadFinalizada -= CerrarPorInactividad;
                ControlInactividad.OnAdvertenciaCancelada -= OcultarModalAdvertenciaInactividad;
                await DesactivarEventosJavascriptInactividad();
            }
            else
            {
                if (!_eventosJavascriptSuscritos)
                {
                    ControlInactividad.OnInactividadIniciada -= MostrarModalAdvertenciaInactividad;
                    ControlInactividad.OnInactividadIniciada += MostrarModalAdvertenciaInactividad;
                    ControlInactividad.OnCuentaRegresiva -= ActualizarCuentaAtras;
                    ControlInactividad.OnCuentaRegresiva += ActualizarCuentaAtras;
                    ControlInactividad.OnInactividadFinalizada -= CerrarPorInactividad;
                    ControlInactividad.OnInactividadFinalizada += CerrarPorInactividad;
                    ControlInactividad.OnAdvertenciaCancelada -= OcultarModalAdvertenciaInactividad;
                    ControlInactividad.OnAdvertenciaCancelada += OcultarModalAdvertenciaInactividad;
                    await ActivarEventosJavascriptInactividad();
                }
            }
        }

        private async Task ActivarEventosJavascriptInactividad()
        {
            var inactividadTimeMinutos = Configuracion.GetValue<int>("AppSettings:Session:InactividadMinutos", 10);
            var tiempoMostrarAvisoSegundos = Configuracion.GetValue<int>("AppSettings:Session:TiempoVisualizacionAvisoInactividadSegundos", 30);

            var tiempoInactividad = inactividadTimeMinutos * 60 * 1000;
            TiempoRestante = tiempoMostrarAvisoSegundos * 1000;

            await InteropInactividad.Iniciar(tiempoInactividad, TiempoRestante);
            _eventosJavascriptSuscritos = true;
            Console.WriteLine("Eventos de inactividad js suscritos.");
        }

        private async Task DesactivarEventosJavascriptInactividad()
        {
            if (_eventosJavascriptSuscritos)
            {
                await InteropInactividad.Finalizar();
                _eventosJavascriptSuscritos = false;
                Console.WriteLine("Eventos de inactividad js desuscritos.");
            }
        }

        /// <summary>
        /// ✅ Handler: Muestra el modal de advertencia de inactividad
        /// </summary>
        private void MostrarModalAdvertenciaInactividad(object? sender, EventArgs e)
        {
            MostrarAvisoInactividad = true;
            InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// ✅ Handler: Actualiza la cuenta atrás del modal
        /// </summary>
        private void ActualizarCuentaAtras(object? sender, int tiempoMs)
        {
            TiempoRestante = tiempoMs;
            InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// ✅ Handler: Oculta el modal de advertencia
        /// </summary>
        private void OcultarModalAdvertenciaInactividad(object? sender, EventArgs e)
        {
            MostrarAvisoInactividad = false;
            InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// ✅ Handler: Cierra la pantalla actual por inactividad
        /// </summary>
        private void CerrarPorInactividad(object? sender, EventArgs e)
        {
            MostrarAvisoInactividad = false;
            ControlCambiosNavegacion.LimpiarCambiosPendientes();
            InvokeAsync(StateHasChanged);
            InvokeAsync(() => Navigation.NavigateTo("/home", forceLoad: false));
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
        }

        public async ValueTask DisposeAsync()
        {
            Navigation.LocationChanged -= OnLocationChangedAsync;
            DialogoErrores.OnError -= MostrarPopupError;

            ControlInactividad.OnInactividadIniciada -= MostrarModalAdvertenciaInactividad;
            ControlInactividad.OnCuentaRegresiva -= ActualizarCuentaAtras;
            ControlInactividad.OnInactividadFinalizada -= CerrarPorInactividad;
            ControlInactividad.OnAdvertenciaCancelada -= OcultarModalAdvertenciaInactividad;
            await DesactivarEventosJavascriptInactividad();
        }

        #endregion
    }
}