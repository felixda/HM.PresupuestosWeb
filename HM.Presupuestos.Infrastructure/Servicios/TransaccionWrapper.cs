using HM.Presupuestos.Domain.Puertos;

namespace HM.Presupuestos.Infrastructure.Servicios
{
    /// <summary>
    /// Envuelve el objeto de transacciÃ³n de HM.Core en la abstracciÃ³n ITransaccion del dominio.
    /// Permite que Application use transacciones sin depender de HM.Core directamente.
    /// </summary>
    public class TransaccionWrapper : ITransaccion
    {
        private readonly dynamic _transaccion;
        private bool _disposed;

        public TransaccionWrapper(dynamic transaccion)
        {
            _transaccion = transaccion ?? throw new ArgumentNullException(nameof(transaccion));
        }

        public async Task CommitAsync()
        {
            await _transaccion.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            await _transaccion.RollbackAsync();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _transaccion?.Dispose();
                _disposed = true;
            }
        }
    }
}
