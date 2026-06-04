ï»¿

namespace HM.Presupuestos.Domain.Entidades
{
    public class Idioma
    {
        public int CodigoIdioma { get; set; }

        public string Descripcion { get; set; } = string.Empty;

        public string Iso { get; set; } = string.Empty; //es -> EspaÃ±ol, en -> Ingles, pt ->Portugal
    }
}
