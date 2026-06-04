

namespace HM.Presupuestos.Domain.Entidades
{
    public class SobreprimaFiltro
    {
        public int Anio { get; set; }
        public int? CodigoVersion { get; set; }
        public string CodigoNetworkList { get; set; } = string.Empty;
        public string CodigoMedioList { get; set; } = string.Empty;
        public string CodigoAgrupacionComercialList { get; set; } = string.Empty;
        public string CodigoEditorialList { get; set; } = string.Empty;
    }
}
