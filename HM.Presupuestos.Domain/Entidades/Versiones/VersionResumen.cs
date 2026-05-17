using HM.Presupuestos.Domain.Compartido;

namespace HM.Presupuestos.Domain.Entidades
{
    public class VersionResumen :CodigoDescripcion, IConIcono
    {

        public bool EsUsuarioAdmin { get; set; }

        /// <summary>
        /// Valor BitAnd para los distintos valores de los estados
        /// </summary>
        public int  IndEstado { get; set;}

        public int CodigoTipo { get; set; }

        public new string IconoCssClass
        {
            get
            {
                var cerrada = (IndEstado & Constantes.BitAndVersion.CERRADA) != 0;
                var clase = cerrada ? "fas fa-lock" : "fas fa-pen";
                return EsUsuarioAdmin && Desbloqueada ? clase + " text-success" : clase;
            }
        }

        /// <summary>
        /// Propiedad que se muestra cuando se asocia esta clase a una lista o combo
        /// </summary>
        public new string DisplayText
        {
            get
            {
                return Descripcion;
            }
        }

        public bool Desbloqueada
        {
            get
            {
                return (IndEstado & Constantes.BitAndVersion.DESBLOQUEADA) != 0;
            }
        }
    }
}

