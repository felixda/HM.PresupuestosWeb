//using DevExpress.Blazor;
//using HM.Presupuestos.Server.Pages.Shared;
//using Microsoft.AspNetCore.Components.Routing;

namespace HM.Presupuestos.Server.Layout
{
    public partial class MainLayout
    {
        #region Propiedades para Binding (PascalCase)

        /// <summary>
        /// Popup de error - Se utiliza en el razor
        /// </summary>
        private DxPopup? ErrorPopup { get; set; }
        
        private bool ErrorVisible { get; set; }
        private string MensajeError { get; set; } = string.Empty;
        private string TituloVentanaError { get; set; } = string.Empty;

        private bool IsOpen { get; set; } = true;
        private DrawerMode Mode { get; set; } = DrawerMode.Shrink;
        private DrawerPosition Position { get; set; } = DrawerPosition.Left;

        /// <summary>
        /// Controla la visibilidad del modal de advertencia de inactividad
        /// </summary>
        private bool MostrarAdvertenciaInactividad { get; set; } = false;
        
        /// <summary>
        /// Tiempo restante en milisegundos para la cuenta atrás
        /// </summary>
        private int TiempoRestante { get; set; }

        /// <summary>
        /// Título del modal de advertencia
        /// </summary>
        private string TituloModalAdvertencia { get; set; } = string.Empty;
        
        /// <summary>
        /// Texto para la cuenta atrás
        /// </summary>
        private string TextoCuentaAtras { get; set; } = string.Empty;

        /// <summary>
        /// Referencia al componente modal de control de cambios
        /// </summary>
        private ModalControlCambios? ConfirmModalControlCambios { get; set; }

        #endregion

        #region Propiedades Internas (camelCase con _)

        private bool _esHome => Navigation.Uri.ToLower().EndsWith("/home");
        private bool _suscripcionEventosJsActiva = false;
        private string _ultimaUrlLogeada = "";
        private bool _layoutInicializado = false;
        private string _ultimaUrlNavegada = "";

        #endregion

        #region Popup para mostrar error en la inicialización de las páginas

        /// <summary>
        /// Evento para mostrar un popup si se ha producido un error en la carga de la pantalla
        /// </summary>
        private async Task MostrarPopupError(string nombrePantalla, Exception ex)
        {
            TituloVentanaError = $"{nombrePantalla}";
            MensajeError = await _TraduccionesHelper.GetResourceValue("mensajes:ErrorAbriendoPantalla:label");
            ErrorVisible = true;
            await InvokeAsync(StateHasChanged);
            await _LogService.InsertException(this.GetType().Name, ex);
        }

        private void CerrarPopupError()
        {
            ErrorVisible = false;
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

                _ErrorService.OnError += MostrarPopupError;

                // Para indicar la función que se mostrará al cambiar de páginas en la navegación y se ha indicado que hay cambios
                _ControlCambiosService.RegistrarConfirmador(ShowModalConfirmacion);

                _IdiomaService.IdiomaCambiado += async () =>
                {
                    await InvokeAsync(StateHasChanged);
                };

                Console.WriteLine("[MainLayout] ✅ OnInitializedAsync completado");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MainLayout] ❌ Error en OnInitializedAsync: {ex.Message}");
                await _LogService.InsertException(nameof(MainLayout), ex);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Console.WriteLine("[MainLayout] 🔄 OnAfterRenderAsync (firstRender: true)");
                
                try
                {
                    await _UsuarioService.CargarUsuarioAsync();

                    TituloModalAdvertencia = TraductorRecursos.ObtenerTexto("Pages:ModalInactividad:Titulo:label");
                    TextoCuentaAtras = TraductorRecursos.ObtenerTexto("Pages:ModalInactividad:CuentaAtras:label");

                    await ManejarSubscripciones();
                }
                catch (Exception ex)
                {
                    await _LogService.InsertException(this.GetType().Name, ex);
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
                var urlNormalizada = _NavigationService.NormalizarUrl(e.Location);
                if (urlNormalizada == _ultimaUrlNavegada)
                {
                    Console.WriteLine($"[MainLayout] ⏭️ URL duplicada en navegación, saltando procesamiento: {urlNormalizada}");
                    return;
                }

                _ultimaUrlNavegada = urlNormalizada;

                // ✅ NO registrar log si estamos en Index/Home
                if (!EsPaginaIndex(urlNormalizada))
                {
                    await RegistrarLogAcceso(urlNormalizada);
                }
                else
                {
                    Console.WriteLine($"[MainLayout] ⏭️ Página Index/Home detectada, no se registra log de acceso");
                }

                await ManejarSubscripciones();
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MainLayout] ❌ Error en OnLocationChangedAsync: {ex.Message}");
                await _LogService.InsertException(nameof(MainLayout), ex);
            }
        }

        /// <summary>
        /// Verifica si la URL corresponde a la página Index/Home
        /// </summary>
        private bool EsPaginaIndex(string urlNormalizada)
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

        private async Task OnBeforeInternalNavigation(LocationChangingContext context)
        {
            bool permitir = await _ControlCambiosService.ConfirmarNavegacionAsync(context.TargetLocation);

            if (!permitir)
            {
                context.PreventNavigation();
            }
            else
            {
                _ControlCambiosService.LimpiarCambios();
            }
        }

        private async Task<bool> ShowModalConfirmacion(string destino)
        {
            if (ConfirmModalControlCambios is not null)
            {
                string mensaje = await _TraduccionesHelper.GetResourceValue("Mensajes:AvisoCambiosPendientes:label");
                return await ConfirmModalControlCambios.Show(mensaje);
            }

            return true;
        }

        private async Task RegistrarLogAcceso(string urlNormalizada)
        {
            try
            {
                // Evitar registrar duplicados consecutivos
                if (urlNormalizada == _ultimaUrlLogeada)
                {
                    Console.WriteLine($"[MainLayout] ⏭️ URL duplicada, no se registra: {urlNormalizada}");
                    return;
                }

                // Obtener código de menú para la URL
                int menuCode = TraductorRecursos.ObtenerCodigoMenuPorUrl(urlNormalizada);

                string tituloParaLog;
                if (menuCode > 0)
                {
                    tituloParaLog = TraductorRecursos.ObtenerTexto($"Menu:Menu_{menuCode}:label");
                }
                else
                {
                    tituloParaLog = urlNormalizada;
                }

                await _LogService.GrabarAccesoAPagina(tituloParaLog);

                // ✅ Actualizar última URL DESPUÉS de registrar exitosamente
                _ultimaUrlLogeada = urlNormalizada;

                Console.WriteLine($"[MainLayout] ✅ Log registrado: {tituloParaLog} ({urlNormalizada})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MainLayout] ⚠️ Error al registrar log: {ex.Message}");
                await _LogService.InsertException(nameof(MainLayout), ex);
            }
        }

        #endregion

        #region Mensaje Inactividad

        private async Task ManejarSubscripciones()
        {
            if (_esHome)
            {
                _InactividadService.OnInactividadIniciada -= MostrarModalAdvertenciaInactividad;
                _InactividadService.OnCuentaRegresiva -= ActualizarCuentaAtras;
                _InactividadService.OnInactividadFinalizada -= CerrarPorInactividad;
                _InactividadService.OnAdvertenciaCancelada -= OcultarModalAdvertenciaInactividad;
                await DesuscribirEventosJsInactividad();
            }
            else
            {
                if (!_suscripcionEventosJsActiva)
                {
                    _InactividadService.OnInactividadIniciada -= MostrarModalAdvertenciaInactividad;
                    _InactividadService.OnInactividadIniciada += MostrarModalAdvertenciaInactividad;
                    _InactividadService.OnCuentaRegresiva -= ActualizarCuentaAtras;
                    _InactividadService.OnCuentaRegresiva += ActualizarCuentaAtras;
                    _InactividadService.OnInactividadFinalizada -= CerrarPorInactividad;
                    _InactividadService.OnInactividadFinalizada += CerrarPorInactividad;
                    _InactividadService.OnAdvertenciaCancelada -= OcultarModalAdvertenciaInactividad;
                    _InactividadService.OnAdvertenciaCancelada += OcultarModalAdvertenciaInactividad;
                    await SuscribirEventosJsInactividad();
                }
            }
        }

        private async Task SuscribirEventosJsInactividad()
        {
            var inactividadTimeMinutos = _Configuration.GetValue<int>("AppSettings:Session:InactividadMinutos", 10);
            var tiempoMostrarAvisoSegundos = _Configuration.GetValue<int>("AppSettings:Session:TiempoVisualizacionAvisoInactividadSegundos", 30);

            var tiempoInactividad = inactividadTimeMinutos * 60 * 1000;
            TiempoRestante = tiempoMostrarAvisoSegundos * 1000;

            await _InactividadInterop.Iniciar(tiempoInactividad, TiempoRestante);
            _suscripcionEventosJsActiva = true;
            Console.WriteLine("Eventos de inactividad js suscritos.");
        }

        private async Task DesuscribirEventosJsInactividad()
        {
            if (_suscripcionEventosJsActiva)
            {
                await _InactividadInterop.Finalizar();
                _suscripcionEventosJsActiva = false;
                Console.WriteLine("Eventos de inactividad js desuscritos.");
            }
        }

        /// <summary>
        /// ✅ Handler: Muestra el modal de advertencia de inactividad
        /// </summary>
        private void MostrarModalAdvertenciaInactividad(object? sender, EventArgs e)
        {
            MostrarAdvertenciaInactividad = true;
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
            MostrarAdvertenciaInactividad = false;
            InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// ✅ Handler: Cierra la pantalla actual por inactividad
        /// </summary>
        private void CerrarPorInactividad(object? sender, EventArgs e)
        {
            MostrarAdvertenciaInactividad = false;
            _ControlCambiosService.LimpiarCambios();
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
            _ErrorService.OnError -= MostrarPopupError;

            _InactividadService.OnInactividadIniciada -= MostrarModalAdvertenciaInactividad;
            _InactividadService.OnCuentaRegresiva -= ActualizarCuentaAtras;
            _InactividadService.OnInactividadFinalizada -= CerrarPorInactividad;
            _InactividadService.OnAdvertenciaCancelada -= OcultarModalAdvertenciaInactividad;
            await DesuscribirEventosJsInactividad();
        }

        #endregion
    }
}