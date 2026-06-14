using HM.Presupuestos.Domain.Entidades;

namespace HM.Presupuestos.Web.Pages.Condiciones
{
    /// <summary>
    /// ViewModel de la capa Web que extiende ExcepcionDto con estado de UI:
    /// flag de accesibilidad del medio y catÃ¡logos para los combos dinÃ¡micos.
    /// Solo vive en la capa Web; el servicio y el repositorio siguen usando ExcepcionDto.
    /// </summary>
    public class ExcepcionCondicionViewModel : ExcepcionDto
    {
        /// <summary>
        /// Calculado en pÃ¡gina Web comparando el medio contra los medios accesibles del network.
        /// No procede de BD.
        /// </summary>
        public bool MedioAccesible { get; set; }

        /// <summary>CatÃ¡logo de tipos de disciplina disponibles para el combo de esta excepciÃ³n.</summary>
        public List<CodigoDescripcion>? TiposDisciplinaDisponibles { get; set; }

        /// <summary>CatÃ¡logo de grupos de disciplina disponibles para el combo de esta excepciÃ³n.</summary>
        public List<CodigoDescripcion>? DisciplinasGrupoDisponibles { get; set; }

        /// <summary>CatÃ¡logo de disciplinas disponibles para el combo de esta excepciÃ³n.</summary>
        public List<CodigoDescripcion>? DisciplinasDisponibles { get; set; }

        /// <summary>CatÃ¡logo de objetivos disponibles para el combo de esta excepciÃ³n.</summary>
        public List<CodigoDescripcion>? ObjetivosDisponibles { get; set; }

        /// <summary>CatÃ¡logo de tipos de compra disponibles para el combo de esta excepciÃ³n.</summary>
        public List<CodigoDescripcion>? TiposCompraDisponibles { get; set; }
    }
}
