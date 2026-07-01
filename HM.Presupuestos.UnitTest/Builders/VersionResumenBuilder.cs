using HM.Presupuestos.Domain.Entidades;

namespace HM.Presupuestos.UnitTest.Builders;

internal sealed class VersionResumenBuilder
{
    private readonly VersionResumen _v = new();

    public VersionResumenBuilder WithCodigo(int codigo) { _v.Codigo = codigo; return this; }
    public VersionResumenBuilder WithDescripcion(string descripcion) { _v.Descripcion = descripcion; return this; }

    public VersionResumen Build() => _v;
}
