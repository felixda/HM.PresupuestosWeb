using HM.Presupuestos.Application.CasosDeUso.Compartido;

namespace HM.Presupuestos.UnitTest.Builders;

internal sealed class MaestrosCacheOptionsBuilder
{
    private int _ttl = 30;

    public MaestrosCacheOptionsBuilder WithTtlMinutos(int ttl) { _ttl = ttl; return this; }

    public MaestrosCacheOptions Build()
    {
        return new MaestrosCacheOptions { TtlMinutos = _ttl };
    }
}
