namespace HM.Presupuestos.Domain.Puertos
{
    public interface ITransaccion : IDisposable
    {
        Task CommitAsync();
        Task RollbackAsync();
    }
}
