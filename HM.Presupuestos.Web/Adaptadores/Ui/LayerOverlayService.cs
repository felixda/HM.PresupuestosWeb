
namespace HM.Presupuestos.Web.Adaptadores.Ui
{
    /// <summary>
    /// Servicio para controlar una capa de overlay en la UI (p.ej. indicador de carga o de operación en curso) con conteo de referencias y parada automática opcional.
    /// Los consumidores llaman a <see cref="Start"/> para mostrar el overlay y a <see cref="Stop"/> para ocultarlo; múltiples operaciones solapadas se gestionan de forma segura.
    /// </summary>
    public interface ILayerOverlayService
    {
        /// <summary>
        /// Tiempo máximo por defecto (en segundos) tras el que se detiene el overlay automáticamente cuando no se especifica explícitamente.
        /// </summary>
        public const int _DEFAULT_AUTOSTOP_SECONDS = 20;

        /// <summary>
        /// Retardo (en segundos) antes de disparar el evento <see cref="OnChange"/> final con estado false, para permitir animaciones de cierre en la UI.
        /// </summary>
        public const decimal _DELAY_STOP_SECONDS = 0.5M;

        /// <summary>
        /// Activa (o mantiene activo) el overlay. Incrementa un contador interno.
        /// El overlay se hace visible inmediatamente y permanece así hasta:
        ///  - Que el contador vuelva a cero mediante las llamadas correspondientes a <see cref="Stop"/>, o
        ///  - Que expire el timeout de parada automática (solo si esta invocación transiciona el contador de 0 a 1 y el timeout es &gt; 0).
        /// </summary>
        /// <param name="message">
        /// Mensaje opcional a mostrar. Si es null, se conserva el mensaje previo (si lo hay); si no es null, reemplaza el mensaje actual.
        /// </param>
        /// <param name="secondsAutoStopAfter">
        /// Timeout en segundos tras el que se detiene el overlay automáticamente. Usar 0 o negativo para desactivar la parada automática en esta activación.
        /// </param>
        void Start(string? message = null, int secondsAutoStopAfter = _DEFAULT_AUTOSTOP_SECONDS);

        /// <summary>
        /// Decrementa el contador interno. Cuando llega a cero:
        ///  - Cancela cualquier timer de parada automática pendiente.
        ///  - Limpia el mensaje.
        ///  - Tras un breve retardo (<see cref="_DELAY_STOP_SECONDS"/>) dispara <see cref="OnChange"/> con (false, null).
        /// Si el contador sigue por encima de cero, el overlay permanece activo y se dispara <see cref="OnChange"/> actualizado (true, mensaje).
        /// </summary>
        void Stop();

        /// <summary>
        /// Crea un ámbito que llama a <see cref="Start"/> al construirse y a <see cref="Stop"/> al liberarse.
        /// Pensado para usarse con <c>using</c> y garantizar la liberación tanto en el camino éxito como en error.
        /// </summary>
        /// <param name="message">Mensaje opcional; ver <see cref="Start(string?, int)"/>.</param>
        /// <param name="secondsAutoStopAfter">Timeout de parada automática; ver <see cref="Start(string?, int)"/>.</param>
        /// <returns>Un <see cref="IDisposable"/> que detendrá el overlay al liberarse.</returns>
        IDisposable Scope(string? message = null, int secondsAutoStopAfter = _DEFAULT_AUTOSTOP_SECONDS);

        /// <summary>
        /// Se dispara cada vez que cambia la visibilidad o el mensaje del overlay.
        /// Primer parámetro: true = visible, false = oculto.
        /// Segundo parámetro: mensaje activo (null cuando está oculto).
        /// </summary>
        event Action<bool, string?>? OnChange;
    }

    public class LayerOverlayService : ILayerOverlayService
    {
        private int _counter;
        private string? _mensaje;
        private CancellationTokenSource? _cts;
        private Task? _autoStopTask;

        public event Action<bool, string?>? OnChange;

        /// <summary>
        /// Activa el overlay y opcionalmente establece un mensaje.
        /// Cada llamada incrementa un contador interno; el overlay permanece activo hasta que se realicen todas las llamadas
        /// correspondientes a <see cref="Stop"/> o expire el timeout de parada automática.
        /// </summary>
        /// <param name="message">
        /// Mensaje opcional a mostrar. Si es null, se conserva cualquier mensaje previo.
        /// Pasar un valor no nulo reemplaza el mensaje actual.
        /// </param>
        /// <param name="secondsAutoStopAfter">
        /// Timeout en segundos tras el que el overlay se detiene automáticamente, aplicado solo cuando esta llamada
        /// transiciona el contador interno de 0 a 1. Usar valor 0 para desactivar la parada automática.
        /// </param>
        /// <remarks>
        /// Thread-safe: usa <see cref="Interlocked.Increment(ref int)"/> para el contador activo.
        /// Solo la primera activación (contador == 1) programa una tarea de parada automática; las llamadas posteriores
        /// prolongan la visibilidad pero no reprograman el timer.
        /// Si se produce una nueva primera activación antes de que se dispare una parada automática previa, el timer anterior se cancela.
        /// El evento <see cref="OnChange"/> se dispara inmediatamente tras la activación con (true, mensajeActual).
        /// En la parada automática exitosa o en el <see cref="Stop"/> final, dispara (false, null) tras un breve retardo
        /// definido por <see cref="ILayerOverlayService._DELAY_STOP_SECONDS"/>.
        /// </remarks>
        public void Start(string? message = null, int secondsAutoStopAfter = ILayerOverlayService._DEFAULT_AUTOSTOP_SECONDS)
        {
            var newValue = Interlocked.Increment(ref _counter);

            if (message is not null)
                _mensaje = message;

            OnChange?.Invoke(true, _mensaje);

            if (newValue == 1 && secondsAutoStopAfter > 0)
            {
                _cts?.Cancel();
                _cts?.Dispose();
                _cts = new CancellationTokenSource();
                var token = _cts.Token;

                _autoStopTask = Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(secondsAutoStopAfter), token).ConfigureAwait(false);
                        if (!token.IsCancellationRequested)
                            Stop();
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception)
                    {
                        Stop();
                    }
                });
            }
        }

        /// <summary>
        /// Decrementa el contador de operaciones activas y potencialmente oculta el overlay.
        /// Comportamiento:
        ///  - Si el contador tras el decremento es &gt; 0, el overlay permanece activo y notifica con el mensaje actual.
        ///  - Si el contador llega a 0 o menos:
        ///      * El contador se resetea a 0 (defensa).
        ///      * El timer de parada automática pendiente (si lo hay) se cancela y libera.
        ///      * El mensaje se limpia.
        ///      * Se espera un breve retardo (<see cref="ILayerOverlayService._DELAY_STOP_SECONDS"/>) antes de disparar (false, null).
        /// Thread-safe mediante <see cref="Interlocked.Decrement(ref int)"/>.
        /// </summary>
        public void Stop()
        {
            var newValue = Interlocked.Decrement(ref _counter);

            if (newValue <= 0)
            {
                _counter = 0;

                if (_cts != null)
                {
                    _cts.Cancel();
                    _cts.Dispose();
                    _cts = null;
                }
                _autoStopTask = null;

                _mensaje = null;
                _autoStopTask = Task.Run(async () =>
                {
                    try
                    {
                        if (ILayerOverlayService._DELAY_STOP_SECONDS > 0)
                            await Task.Delay((int)ILayerOverlayService._DELAY_STOP_SECONDS * 1000).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    finally
                    {
                        OnChange?.Invoke(false, null);
                    }
                });
            }
            else
            {
                OnChange?.Invoke(true, _mensaje);
            }
        }

        /// <summary>
        /// Devuelve un ámbito <see cref="IDisposable"/> que llama a <see cref="Start"/> inmediatamente y a <see cref="Stop"/> al liberarse.
        /// Uso típico:
        /// <code>
        /// using (overlay.Scope("Cargando datos"))
        /// {
        ///     await CargarAsync();
        /// }
        /// </code>
        /// Garantiza la liberación del overlay incluso si se produce una excepción.
        /// </summary>
        /// <param name="message">Mensaje opcional a mostrar durante el ámbito.</param>
        /// <param name="secondsAutoStopAfter">Timeout de parada automática; ver <see cref="Start(string?, int)"/>.</param>
        /// <returns>Ámbito desechable que gestiona el ciclo de vida del overlay.</returns>
        public IDisposable Scope(string? message = null, int secondsAutoStopAfter = ILayerOverlayService._DEFAULT_AUTOSTOP_SECONDS)
            => new LoadingScope(this, message, secondsAutoStopAfter);

        private sealed class LoadingScope : IDisposable
        {
            private readonly LayerOverlayService _svc;
            private bool _disposed;

            public LoadingScope(LayerOverlayService svc, string? message, int seconds)
            {
                _svc = svc;
                _svc.Start(message, seconds);
            }

            public void Dispose()
            {
                if (_disposed) return;
                _disposed = true;
                _svc.Stop();
            }
        }
    }
}



