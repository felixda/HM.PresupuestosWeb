using HM.Presupuestos.Domain.Entidades;

namespace HM.Presupuestos.UnitTest.Condiciones;

internal sealed class VigenciaBuilder
{
    private Vigencia _v = new Vigencia();

    public VigenciaBuilder WithCodigo(int codigo) { _v.Codigo = codigo; return this; }
    public VigenciaBuilder WithVersion(int version) { _v.CodigoVersion = version; return this; }
    public VigenciaBuilder WithGrupoCliente(int grupo) { _v.CodigoGrupoCliente = grupo; return this; }
    public VigenciaBuilder WithNetwork(int network) { _v.CodigoNetWork = network; return this; }
    public VigenciaBuilder WithMesDesde(int mesDesde) { _v.MesDesde = mesDesde; return this; }
    public VigenciaBuilder WithMesHasta(int mesHasta) { _v.MesHasta = mesHasta; return this; }
    public VigenciaBuilder WithIndicadorAcuerdo(int indicador) { _v.IndicadorAcuerdo = indicador; return this; }

    public Vigencia Build() => _v;
}
