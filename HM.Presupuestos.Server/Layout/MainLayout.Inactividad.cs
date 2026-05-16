namespace HM.Presupuestos.Server.Layout
{
    public partial class MainLayout
    {
        #region Inactividad — Propiedades de binding

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

        #endregion

        #region Inactividad — Lógica

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

        /// <summary>Handler: Muestra el modal de advertencia de inactividad</summary>
        private void MostrarModalAdvertenciaInactividad(object? sender, EventArgs e)
        {
            MostrarAvisoInactividad = true;
            InvokeAsync(StateHasChanged);
        }

        /// <summary>Handler: Actualiza la cuenta atrás del modal</summary>
        private void ActualizarCuentaAtras(object? sender, int tiempoMs)
        {
            TiempoRestante = tiempoMs;
            InvokeAsync(StateHasChanged);
        }

        /// <summary>Handler: Oculta el modal de advertencia</summary>
        private void OcultarModalAdvertenciaInactividad(object? sender, EventArgs e)
        {
            MostrarAvisoInactividad = false;
            InvokeAsync(StateHasChanged);
        }

        /// <summary>Handler: Cierra la pantalla actual por inactividad</summary>
        private void CerrarPorInactividad(object? sender, EventArgs e)
        {
            MostrarAvisoInactividad = false;
            ControlCambiosNavegacion.LimpiarCambiosPendientes();
            InvokeAsync(StateHasChanged);
            InvokeAsync(() => Navigation.NavigateTo("/home", forceLoad: false));
        }

        #endregion
    }
}
