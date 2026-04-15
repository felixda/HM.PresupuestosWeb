using HM.Presupuestos.Contratos.Comun;
using HM.Presupuestos.Contratos.Helper;
using Microsoft.Graph;
using System.ComponentModel.DataAnnotations;
using StringHelper = HM.Presupuestos.Contratos.Helper.StringHelper;

namespace HM.Presupuestos.Contratos.Entidades
{
    public class Version : IConIcono, IConCodigo
    {
        public int Codigo { get; set; }

        private string _descripcion = "";
        public string Descripcion
        {
            get
            {
                return StringHelper.CapitalizeText(_descripcion, 3);
            }
            set { _descripcion = value; }
        }

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
        public bool IsDataLinked { get; set; }

    }
}
