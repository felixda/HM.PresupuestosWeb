
namespace HM.Presupuestos.Domain.Entidades
{
    public class Vigencia :CodigoDescripcion
    {
        public int CodigoVersion { get; set; }
        public int CodigoNetWork { get; set; }
        public int CodigoGrupoCliente { get; set; }
        public int MesDesde { get; set; }
        public int MesHasta { get; set; }
        public int IndicadorAcuerdo { get; set; }

    }
}
