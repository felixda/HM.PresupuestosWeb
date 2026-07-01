using HM.Presupuestos.Domain.Entidades;

namespace HM.Presupuestos.UnitTest.Builders;

internal sealed class SobreprimaBuilder
{
    private readonly Sobreprima _s = new();

    public SobreprimaBuilder WithCodigo(int codigo) { _s.Codigo = codigo; return this; }
    public SobreprimaBuilder WithPorcentaje(decimal porcentaje) { _s.Porcentaje = porcentaje; return this; }

    public Sobreprima Build() => _s;
}
