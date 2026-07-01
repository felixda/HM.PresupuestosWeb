using HM.Presupuestos.Domain.Puertos;

namespace HM.Presupuestos.UnitTest.Fakes;

internal sealed class InMemoryTransaccion : ITransaccion
{
    public bool CommitInvocado { get; private set; }

    public bool RollbackInvocado { get; private set; }

    public bool DisposeInvocado { get; private set; }

    public Task CommitAsync()
    {
        CommitInvocado = true;
        return Task.CompletedTask;
    }

    public Task RollbackAsync()
    {
        RollbackInvocado = true;
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        DisposeInvocado = true;
    }
}
