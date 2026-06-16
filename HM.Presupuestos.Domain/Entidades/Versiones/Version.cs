using HM.Presupuestos.Domain.Compartido;
using System.ComponentModel.DataAnnotations;

namespace HM.Presupuestos.Domain.Entidades
{
    public class Version : IConIcono, IConCodigo
    {
        public int Codigo { get; set; }
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public int Anio { get; set; }
        [Required]
        public int Mes { get; set; }

        [Required]
        public int Orden { get; set; }
        [Required]
        public int CodigoTipo { get; set; }

        ///// <summary>
        ///// entero en formato Binario para indicar todos los estado posibles de una (BitAnd)
        ///// </summary>
        [Required]
        public int IndEstado { get; set; }

        public List<VersionIndicador> IndicadorList { get; set; } = new List<VersionIndicador>();

        public class VersionIndicador
        {
            public int Codigo { get; set; }
            public bool Estado { get; set; }
        }


        public string IconoCssClass
        {
            get
            {
                var cerrada = (IndEstado & Constantes.BitAndVersion.CERRADA) != 0;
                return cerrada ? "fas fa-lock" : "fas fa-pen";
            }
        }

        /// <summary>
        /// Propiedad que se muestra cuando se asocia esta clase a una lista o combo
        /// </summary>
        public string DisplayText
        {
            get
            {
                return Descripcion;
            }
        }

        /// <summary>
        /// Indica si la version tiene  datos relaciones con otras entidades (Previsiones, condiciones...)
        /// </summary>
        public bool TieneDatosVinculados { get; set; }

    }
}

