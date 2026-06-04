ï»¿

namespace HM.Presupuestos.Domain.Entidades
{
    public class Sobreprima
    {
        public int Codigo { get; set; }
        public int CodigoVersion { get; set; }
        public int CodigoConcepto { get; set; }
        public int CodigoNetwork { get; set; }
        public int CodigoPais { get; set; }
        public int CodigoMedio { get; set; }
        public int CodigoEditorial { get; set; }
        public int? CodigoAgrupacionComercial { get; set; }

        private decimal _porcentaje;
        public decimal Porcentaje
        {
            get => _porcentaje;
            set => _porcentaje = Math.Round(value, 2);
        }


    }
}
