using HM.Presupuestos.Domain.Entidades;
using DomainVersion = HM.Presupuestos.Domain.Entidades.Version;

namespace HM.Presupuestos.UnitTest.Versiones;

internal sealed class VersionBuilder
{
    private int _codigo = 1;
    private string _descripcion = "Versión";
    private int _anio = 2026;
    private int _mes = 1;
    private int _orden = 1;
    private int _codigoTipo = 1;
    private int _indEstado = 0;

    public VersionBuilder WithCodigo(int codigo) { _codigo = codigo; return this; }
    public VersionBuilder WithDescripcion(string descripcion) { _descripcion = descripcion; return this; }
    public VersionBuilder WithAnio(int anio) { _anio = anio; return this; }
    public VersionBuilder WithMes(int mes) { _mes = mes; return this; }
    public VersionBuilder WithOrden(int orden) { _orden = orden; return this; }
    public VersionBuilder WithCodigoTipo(int codigoTipo) { _codigoTipo = codigoTipo; return this; }
    public VersionBuilder WithIndEstado(int indEstado) { _indEstado = indEstado; return this; }

    public DomainVersion Build()
    {
        return new DomainVersion
        {
            Codigo = _codigo,
            Descripcion = _descripcion,
            Anio = _anio,
            Mes = _mes,
            Orden = _orden,
            CodigoTipo = _codigoTipo,
            IndEstado = _indEstado
        };
    }
}
