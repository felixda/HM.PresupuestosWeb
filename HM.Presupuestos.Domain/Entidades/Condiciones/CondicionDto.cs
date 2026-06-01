
namespace HM.Presupuestos.Domain.Entidades
{
    public class CondicionDto
    {
        public string Key => $"{CodigoMedio}";

      
        public int CodigoMedio { get; set; }

        public string DescripcionMedio { get; set; } = string.Empty;
                
        private decimal? _pctSAG;
        public decimal? PctSAG
        {
            get => _pctSAG;
            set => _pctSAG = value.HasValue ? Math.Round(value.Value, 2) : null;
        }

        private decimal? _pctManPower;
        public decimal? PctManPower
        {
            get => _pctManPower;
            set => _pctManPower = value.HasValue ? Math.Round(value.Value, 2) : null;
        }

        private decimal? _pctDevolucion;
        public decimal? PctDevolucion
        {
            get => _pctDevolucion;
            set => _pctDevolucion = value.HasValue ? Math.Round(value.Value, 2) : null;
        }

        public int  IndicadorCalculoDevolucion { get; set; }

        public int NumeroExcepciones { get; set; }

    }
}
