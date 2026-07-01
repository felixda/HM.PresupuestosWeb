using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Puertos;

namespace HM.Presupuestos.UnitTest.Fakes;

internal sealed class InMemoryConfiguracionRepository : IConfiguracionRepository
{
    private CodigoDescripcion _anioDiario = new() { Codigo = 2026, Descripcion = "2026" };

    public Task<CodigoDescripcion> ObtenerAnioDiario() => Task.FromResult(_anioDiario);

    public Task ActualizarAnioDiario(int anio)
    {
        _anioDiario = new CodigoDescripcion { Codigo = anio, Descripcion = anio.ToString() };
        return Task.CompletedTask;
    }
}
