using HM.Presupuestos.Web.Adaptadores.Sesion;
namespace HM.Presupuestos.Web.Layout
{
    public partial class MainLayout
    {
        #region Servicios Inyectados

        [Inject] protected ILocalizadorRecursos LocalizadorRecursos { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;
        [Inject] private ControlInactividad ControlInactividad { get; set; } = default!;
        [Inject] private JsInteropHelper InteropInactividad { get; set; } = default!;
        [Inject] private IConfiguration Configuracion { get; set; } = default!;
        [Inject] private IControlCambiosNavegacion ControlCambiosNavegacion { get; set; } = default!;
        [Inject] private ISesionUsuario SesionUsuario { get; set; } = default!;
        [Inject] private DialogoErrores DialogoErrores { get; set; } = default!;
        [Inject] private IRegistroAplicacion RegistroAplicacion { get; set; } = default!;
        [Inject] private IGestorIdioma GestorIdioma { get; set; } = default!;
        [Inject] private IRutasNavegacion RutasNavegacion { get; set; } = default!;
        [Inject] private IRecursosApp RecursosApp { get; set; } = default!;
        [Inject] private IRegistroSesionesActivas RegistroSesionesActivas { get; set; } = default!;
        [Inject] private IHistorialNavegacion HistorialNavegacion { get; set; } = default!;

        #endregion

        #region Propiedades para Binding (PascalCase)

        /// <summary>
        /// Popup de error - Se utiliza en el razor
        /// </summary>
        private DxPopup? ErrorPopup { get; set; }

        private bool MenuAbierto { get; set; } = true;
        private DrawerMode ModoMenu { get; set; } = DrawerMode.Shrink;
        private DrawerPosition PosicionMenu { get; set; } = DrawerPosition.Left;

        /// <summary>
        /// Referencia al componente modal de control de cambios
        /// </summary>
        private ModalControlCambios? ModalConfirmacionCambios { get; set; }

        #endregion

        #region Propiedades Internas (camelCase con _)

        private bool EsHome => Navigation.Uri.ToLower().EndsWith("/home");
        private bool _eventosJavascriptSuscritos = false;
        private string _ultimaUrlRegistrada = "";
        private string _ultimaRutaVisitada = "";

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Se ejecuta ANTES de renderizar - solo inicializacion basica
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            try
            {
                Navigation.LocationChanged -= OnLocationChangedAsync;
                Navigation.LocationChanged += OnLocationChangedAsync;

                DialogoErrores.OnError += MostrarPopupError;

                ControlCambiosNavegacion.RegistrarConfirmacionSalida(MostrarConfirmacionCambiosPendientes);

                GestorIdioma.IdiomaCambiado += async () =>
                {
                    await InvokeAsync(StateHasChanged);
                };

                Console.WriteLine("[MainLayout] OnInitializedAsync completado");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MainLayout] Error en OnInitializedAsync: {ex.Message}");
                await RegistroAplicacion.RegistrarExcepcion(nameof(MainLayout), ex);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Console.WriteLine("[MainLayout] OnAfterRenderAsync (firstRender: true)");

                try
                {
                    await SesionUsuario.InicializarUsuarioAsync();

                    if (SesionUsuario.UsuarioApp is null)
                    {
                        Navigation.NavigateTo("/", forceLoad: true);
                        return;
                    }

                    TituloAvisoInactividad = LocalizadorRecursos.ObtenerTexto(TextosApp.Pages.ModalInactividad.Titulo);
                    TextoCuentaAtras = LocalizadorRecursos.ObtenerTexto(TextosApp.Pages.ModalInactividad.CuentaAtras);

                    await ActualizarSubscripcionesInactividad();
                }
                catch (Exception ex)
                {
                    await RegistroAplicacion.RegistrarExcepcion(this.GetType().Name, ex);
                }
            }
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

