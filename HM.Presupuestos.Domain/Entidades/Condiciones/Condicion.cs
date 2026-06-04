
using HM.Presupuestos.Domain.Compartido;

namespace HM.Presupuestos.Domain.Entidades
{
    public class Condicion
    {
        public int CodigoVigencia { get; set; }

        public int CodigoCondicion { get; set; }

        public int CodigoMedio { get; set; }

        public ConceptosCondiciones CodigoConcepto { get; set; }

        private decimal? _porcentaje;
        public decimal? Porcentaje
        {
            get => _porcentaje;
            set => _porcentaje = value.HasValue ? Math.Round(value.Value, 2) : null;
        }

        public int Jerarquia { get; set; }

        /// <summary>
        ///Indica sobre qué se hace el cálculo. (1=Neto Venta, 2 = Sobreprima)'
        /// </summary>
        public int IndicadorCalculo { get;set; }  

    }
}

