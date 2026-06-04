using System.ComponentModel.DataAnnotations;

namespace HM.Presupuestos.Domain.Entidades
{
    public class IdiomaIndicador
    {
        public int? Codigo { get; set; }

        public int? CodigoIndicador { get; set; }

        [Required]
        public int? CodigoIdioma { get; set; }

        [Required]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public string DescripcionAbreviada { get; set; } = string.Empty;

        [Required]
        public string Leyenda { get; set; } = string.Empty;

        /// <summary>
        /// Devuelve cadena en formato Descripcion (Codigo)
        /// </summary>
        public string DescripcionCodigoIdioma
        {
            get
            {
                return $"{Descripcion} ({CodigoIdioma})";
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is not IdiomaIndicador other)
                return false;

            return Codigo == other.Codigo
                && CodigoIdioma == other.CodigoIdioma
                && Descripcion == other.Descripcion
                && DescripcionAbreviada == other.DescripcionAbreviada
                && Leyenda == other.Leyenda;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Codigo, Descripcion, DescripcionAbreviada, Leyenda);
        }

    }
}
