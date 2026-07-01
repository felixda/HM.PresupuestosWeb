using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Puertos;

namespace HM.Presupuestos.UnitTest.Fakes;

internal sealed class InMemorySobreprimasRepository : ISobreprimasRepository
{
    private readonly List<Sobreprima> _data = new();
    private readonly InMemoryTransaccion _transaccion = new();

    public Task<List<Sobreprima>> ObtenerSobreprimas(SobreprimaFiltro filterSobreprima)
    {
        // Simple filter: ignore filter fields for tests; return clone
        return Task.FromResult(_data.Select(s => new Sobreprima { Codigo = s.Codigo, Porcentaje = s.Porcentaje }).ToList());
    }

    public Task InsertSobreprima(Sobreprima item)
    {
        if (item.Codigo > 0)
        {
            // preserve provided code for test seeding; replace if exists
            var existing = _data.FirstOrDefault(x => x.Codigo == item.Codigo);
            if (existing != null)
            {
                existing.Porcentaje = item.Porcentaje;
            }
            else
            {
                _data.Add(new Sobreprima { Codigo = item.Codigo, Porcentaje = item.Porcentaje });
            }
        }
        else
        {
            var next = (_data.Count == 0) ? 1 : _data.Max(x => x.Codigo) + 1;
            var copy = new Sobreprima { Codigo = next, Porcentaje = item.Porcentaje };
            _data.Add(copy);
        }
        return Task.CompletedTask;
    }

    public Task EliminarSobreprima(int codigoSobreprima)
    {
        var ex = _data.FirstOrDefault(x => x.Codigo == codigoSobreprima);
        if (ex != null) _data.Remove(ex);
        return Task.CompletedTask;
    }

    public Task ActualizarSobreprima(Sobreprima item)
    {
        var ex = _data.FirstOrDefault(x => x.Codigo == item.Codigo);
        if (ex != null)
        {
            ex.Porcentaje = item.Porcentaje;
        }
        return Task.CompletedTask;
    }

    public ITransaccion ObtenerTransaccion() => _transaccion;

    public Task<bool> ExistenSobreprimas(SobreprimaFiltro filterSobreprima, string? codigosSobreprima = null)
    {
        return Task.FromResult(_data.Any());
    }

    // Helpers for assertions in tests
    public IReadOnlyList<Sobreprima> All() => _data.AsReadOnly();
    public InMemoryTransaccion Transaccion => _transaccion;
}
