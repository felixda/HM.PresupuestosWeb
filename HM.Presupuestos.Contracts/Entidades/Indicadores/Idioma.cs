

namespace HM.Presupuestos.Contratos.Entidades
{
    public class Idioma
    {
        public int CodigoIdioma { get; set; }

        public string Descripcion { get; set; } = string.Empty;

        public string Iso { get; set; } = string.Empty; //es -> Español, en -> Ingles, pt ->Portugal
    }
}
