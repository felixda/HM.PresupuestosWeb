
namespace HM.Presupuestos.Server.Services
{
    /// <summary>
    /// Service to control a UI layer overlay (e.g., loading or busy indicator) with reference counting and optional auto-stop.
    /// Consumers call <see cref="Start"/> to show and <see cref="Stop"/> to hide; multiple overlapping operations are handled safely.
    /// </summary>
    public interface ILayerOverlayService
    {
        /// <summary>
        /// Default auto-stop timeout (in seconds) applied when not explicitly provided.
        /// </summary>
        public const int _DEFAULT_AUTOSTOP_SECONDS = 20;

        /// <summary>
        /// Delay (in seconds) before firing the final <see cref="OnChange"/> event with a false state to allow UI fade-out/transition.
        /// </summary>
        public const decimal _DELAY_STOP_SECONDS = 0.5M;

        /// <summary>
        /// Activates (or keeps active) the overlay. Increments an internal counter.
        /// The overlay becomes visible immediately and remains so until:
        ///  - The counter is decremented back to zero via matching <see cref="Stop"/> calls, or
        ///  - The optional auto-stop timeout elapses (only if this invocation transitions the counter 0 -> 1 and timeout &gt; 0).
        /// </summary>
        /// <param name="message">
        /// Optional message to display. If null, the existing message (if any) is preserved; if non-null, it replaces the current message.
        /// </param>
        /// <param name="secondsAutoStopAfter">
        /// Auto-stop timeout in seconds. Use 0 or negative to disable auto-stop for this activation.
        /// </param>
        void Start(string? message = null, int secondsAutoStopAfter = _DEFAULT_AUTOSTOP_SECONDS);

        /// <summary>
        /// Decrements the internal counter. When it reaches zero:
        ///  - Cancels any pending auto-stop timer.
        ///  - Clears the message.
        ///  - After a short delay (<see cref="_DELAY_STOP_SECONDS"/>) raises <see cref="OnChange"/> with (false, null).
        /// If the counter remains above zero, the overlay stays active and an updated <see cref="OnChange"/> (true, message) is fired.
        /// </summary>
        void Stop();

        /// <summary>
        /// Creates a scope that calls <see cref="Start"/> on construction and <see cref="Stop"/> on dispose.
        /// Intended for use with <c>using</c> to guarantee release in success/error paths.
        /// </summary>
        /// <param name="message">Optional message; see <see cref="Start(string?, int)"/>.</param>
        /// <param name="secondsAutoStopAfter">Auto-stop timeout; see <see cref="Start(string?, int)"/>.</param>
        /// <returns>An <see cref="IDisposable"/> that will stop the overlay when disposed.</returns>
        IDisposable Scope(string? message = null, int secondsAutoStopAfter = _DEFAULT_AUTOSTOP_SECONDS);

        /// <summary>
        /// Raised whenever the overlay visibility or message changes.
        /// First parameter: true = visible, false = hidden.
        /// Second parameter: active message (null when hidden).
        /// </summary>
        event Action<bool, string?>? OnChange;
    }

    public class LayerOverlayService : ILayerOverlayService
    {
        private int _counter;
        private string? _message;
        private CancellationTokenSource? _cts;
        private Task? _autoStopTask;

        public event Action<bool, string?>? OnChange;

        /// <summary>
        /// Activates the overlay and optionally sets a message.
        /// Each call increments an internal counter; the overlay stays active until all corresponding
        /// <see cref="Stop"/> calls are made or the optional auto-stop timeout elapses.
        /// </summary>
        /// <param name="message">
        /// Optional message to display. If null, any previously set message is retained.
        /// Passing a non-null value replaces the current message.
        /// </param>
        /// <param name="secondsAutoStopAfter">
        /// Timeout (in seconds) after which the overlay auto-stops, applied only when this call
        /// transitions the internal counter from 0 to 1. Use a value ≤ 0 to disable auto-stop.
        /// </param>
        /// <remarks>
        /// Thread-safe: uses <see cref="Interlocked.Increment(ref int)"/> for the active counter.
        /// Only the first activation (counter == 1) schedules an auto-stop task; subsequent calls
        /// extend visibility but do not reschedule the timer.
        /// If a new first activation occurs before a prior auto-stop fires, the previous timer is canceled.
        /// The <see cref="OnChange"/> event is raised immediately after activation with (true, currentMessage).
        /// On successful auto-stop or final <see cref="Stop"/> it raises (false, null) after a short delay
        /// defined by <see cref="ILayerOverlayService._DELAY_STOP_SECONDS"/>.
        /// </remarks>
        public void Start(string? message = null, int secondsAutoStopAfter = ILayerOverlayService._DEFAULT_AUTOSTOP_SECONDS)
        {
            var newValue = Interlocked.Increment(ref _counter);

            if (message is not null)
                _message = message;

            OnChange?.Invoke(true, _message);

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
        /// Decrements the active operations counter and potentially hides the overlay.
        /// Behavior:
        ///  - If the counter after decrement is &gt; 0, overlay remains active and notifies with current message.
        ///  - If the counter reaches 0 or below:
        ///      * Counter is reset to 0 (defensive).
        ///      * Pending auto-stop timer (if any) is canceled and disposed.
        ///      * Message cleared.
        ///      * A short delay (<see cref="ILayerOverlayService._DELAY_STOP_SECONDS"/>) is awaited (if configured) before raising (false, null).
        /// Thread-safe via <see cref="Interlocked.Decrement(ref int)"/>.
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

                _message = null;
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
                OnChange?.Invoke(true, _message);
            }
        }

        /// <summary>
        /// Returns an <see cref="IDisposable"/> scope that calls <see cref="Start"/> immediately and <see cref="Stop"/> upon disposal.
        /// Typical usage:
        /// <code>
        /// using (overlay.Scope("Loading data"))
        /// {
        ///     await LoadAsync();
        /// }
        /// </code>
        /// Guarantees overlay release even if an exception occurs.
        /// </summary>
        /// <param name="message">Optional message to display for the duration of the scope.</param>
        /// <param name="secondsAutoStopAfter">Auto-stop timeout; see <see cref="Start(string?, int)"/>.</param>
        /// <returns>Disposable scope managing overlay lifetime.</returns>
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
