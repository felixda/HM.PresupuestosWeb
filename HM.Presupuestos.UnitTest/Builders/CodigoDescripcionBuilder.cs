using HM.Presupuestos.Domain.Entidades;

namespace HM.Presupuestos.UnitTest.Builders;

internal sealed class CodigoDescripcionBuilder
{
    private readonly CodigoDescripcion _c = new();

    public CodigoDescripcionBuilder WithCodigo(int codigo) { _c.Codigo = codigo; return this; }
    public CodigoDescripcionBuilder WithDescripcion(string descripcion) { _c.Descripcion = descripcion; return this; }

    public CodigoDescripcion Build() => _c;
}
