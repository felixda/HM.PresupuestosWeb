using HM.Presupuestos.Domain.Puertos;

namespace HM.Presupuestos.UnitTest.Fakes;

internal sealed class InMemoryAdminRepository : IAdminRepository
{
    private readonly List<int> _mesesBloqueados = new();
    private readonly InMemoryTransaccion _trans = new();

    public Task<List<int>> ObtenerMesesBloqueados(int anio)
    {
        return Task.FromResult(_mesesBloqueados.ToList());
    }

    public Task InsertarMesBloqueado(int anio, int mes)
    {
        _mesesBloqueados.Add(mes);
        return Task.CompletedTask;
    }

    public Task EliminarMesesBloqueados(int anio)
    {
        _mesesBloqueados.Clear();
        return Task.CompletedTask;
    }

    public ITransaccion ObtenerTransaccion() => _trans;

    // Helpers para tests
    public List<int> All() => _mesesBloqueados.ToList();
    public InMemoryTransaccion Transaccion => _trans;
}
