using HM.Presupuestos.Domain.Compartido;

namespace HM.Presupuestos.Domain.Entidades
{
    public class SobreprimaGridModel
    {
        public string KeyGrid => $"{CodigoVersion}_{CodigoNetwork}_{CodigoPais}_{CodigoMedio}_{CodigoAgrupacionComercial}_{CodigoEditorial}";
        public int Codigo { get; set; }
        public int CodigoVersion { get; set; }
        public int Anio { get; set; }
        public int CodigoNetwork { get; set; }
        public int CodigoPais { get; set; }
        public int? CodigoMedio { get; set; }
        public int? CodigoAgrupacionComercial { get; set; }
        public int? CodigoEditorial { get; set; }
        public string DescripcionNetwork { get; set; } = "";
        public string DescripcionMedio { get; set; } = "";
        public string DescripcionAgrupacionComercial { get; set; } = "";
        public string DescripcionEditorial { get; set; } = "";

        private decimal _porcentajeSobreprimaSLA;
        public decimal PorcentajeSobreprimaSLA
        {
            get => _porcentajeSobreprimaSLA;
            set => _porcentajeSobreprimaSLA = Math.Round(value, 2);
        }

        private decimal _porcentajeSobreprimaHVP;
        public decimal PorcentajeSobreprimaHVP
        {
            get => _porcentajeSobreprimaHVP;
            set => _porcentajeSobreprimaHVP = Math.Round(value, 2);
        }

        private decimal _porcentajeSobreprimaDefault;
        public decimal PorcentajeSobreprimaDefault
        {
            get => _porcentajeSobreprimaDefault;
            set => _porcentajeSobreprimaDefault = Math.Round(value, 2);
        }

        //public string CodigoDescripcionAgrupacionComercial => $"{DescripcionAgrupacionComercial} ({CodigoAgrupacionComercial})";
        public string CodigoDescripcionAgrupacionComercial =>
                                CodigoAgrupacionComercial == null
                                    ? string.Empty
                                    : $"{DescripcionAgrupacionComercial} ({CodigoAgrupacionComercial})";

        public string CodigoDescripcionEditorial => $"{DescripcionEditorial} ({CodigoEditorial})";


        public RegistroConcepto ConceptoDefaul { get; set; } = new RegistroConcepto { Codigo = 0, CodigoConcepto = (int) ConceptosSobreprimas.Sobreprima };
        public RegistroConcepto ConceptoSLA { get; set; } = new RegistroConcepto { Codigo = 0, CodigoConcepto = (int)ConceptosSobreprimas.SLA };
        public RegistroConcepto ConceptoHVP { get; set; } = new RegistroConcepto { Codigo = 0, CodigoConcepto = (int)ConceptosSobreprimas.HVP };

        public ModoOperacion ModoOperacion =  ModoOperacion.Ninguna;

        public bool MedioAccesible = true;

    }

    public class RegistroConcepto
    {
        public int Codigo { get; set; }
        public int CodigoConcepto { get; set; }

        private decimal _porcentaje;
        public decimal Porcentaje
        {
            get => _porcentaje;
            set => _porcentaje = Math.Round(value, 2);
        }
    }
}

