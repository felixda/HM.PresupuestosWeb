ï»¿
namespace HM.Presupuestos.Domain.Entidades
{
    public  class ConceptoCondicion : CodigoDescripcion
    {
        
        public string Abreviatura { get; set; } = "";

        /// <summary>
        /// Indica el signo del concepto (1 = Positivo, -1 = Negativo)
        /// </summary>
        public int IndicadorSigno { get; set; } 

        /// <summary>
        ///  Indica para el concepto, sobre quÃ© se hace el cÃ¡lculo. (1=Neto Venta, 2 = Sobreprima, 0 = Elige el usuario).
        ///  Estos valores se arrastran a PPT_CONDICION_MEDIO y si es 0, el usuario tiene que rellenar si serÃ¡ 1 Ã³ 2 cuando se inserte en esta Ãºltima tabla
        /// </summary>
        public int IndicadorCalculo { get; set; }

        private decimal _porcentaje;
        public decimal Porcentaje
        {
            get => _porcentaje;
            set => _porcentaje = Math.Round(value, 2);
        }
    }
}
