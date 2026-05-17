using HM.Presupuestos.Domain.Compartido;

namespace HM.Presupuestos.Domain.Entidades
{
    public class Indicador
    {
        public int? Codigo { get; set; }

        /// <summary>
        /// Orden en el que aparecen en las versiones. Lo llamamos asi porque es el valor que utilizaremos para acceder a las propiedades de las
        /// versiones que se denominan Indicador + indice
        /// </summary>
        public int Indice { get; set; }

        public int Orden { get; set; }

        public int CodigoIdioma { get; set; }

        public string NombreCampo
        {
            get
            {
                return $"IndicadorEstado{Indice}";
            }
        }
        public string Descripcion { get; set; } = string.Empty;

        
        public string DescripcionAbreviada
        {
            get
            {
                string resultado;
                IdiomaIndicador? idioma = Idiomas.Find(c => c.CodigoIdioma == this.CodigoIdioma);

                if (idioma == null)
                {
                    if (Descripcion.Length <= 4)
                    {
                        resultado = $"{Descripcion}";
                    }
                    else
                    {
                        resultado = $"{Descripcion[..4]}.";
                    }
                }
                else
                {
                   resultado = idioma.DescripcionAbreviada;
                }
                return resultado;
            }
        }


        public string DescripcionTraducida
        {
            get
            {
                string resultado;
                IdiomaIndicador? idioma = Idiomas.Find(c => c.CodigoIdioma == this.CodigoIdioma);

                if (idioma == null)
                {
                    resultado = Descripcion;
                }
                else
                {
                    resultado = idioma.Descripcion;
                }
                return resultado;
            }
        }



        public string Leyenda
        {
            get
            {
                IdiomaIndicador? idioma = Idiomas.Find(c => c.CodigoIdioma == this.CodigoIdioma);

                if (idioma == null || string.IsNullOrEmpty(idioma.Leyenda))
                {
                    return "Sin leyenda";
                }
                else
                {
                    return idioma.Leyenda;
                }
            }
        }

        public int BitAnd { get; set; }

        public bool IndMostrar { get; set;}

        /// <summary>
        /// Si es 1, no puede haber mas versiones con ese estado para el mismo año. Y si es 0, si
        /// </summary>
        public bool IndVersionUnica { get; set;}

        public List<IdiomaIndicador> Idiomas { get; set; } = [];

        public EstadoEntidad Estado { get; set; } = EstadoEntidad.Nuevo;

 
        public bool Equals(Indicador other)
        {
            if (other == null)
                return false;

            return
            (
                object.ReferenceEquals(this.Descripcion, other.Descripcion) ||
                this.Descripcion != null &&
                this.Descripcion.Equals(other.Descripcion)

            ) &&
            (
                this.Orden.Equals(other.Orden)
                )
             &&
            (
              
                this.BitAnd.Equals(other.BitAnd)
                ) &&
            (
                this.IndMostrar.Equals(other.IndMostrar)
                ) &&
            (
                this.IndVersionUnica.Equals(other.IndVersionUnica)
                )

            && Idiomas.SequenceEqual(other.Idiomas); // compara listas;

        }

      
    }
}

