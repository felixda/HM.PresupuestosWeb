using HM.Presupuestos.Domain.Entidades;

namespace HM.Presupuestos.Web.Pages.Condiciones
{
    /// <summary>
    /// ViewModel de la capa Web que extiende ExcepcionDto con estado de UI:
    /// flag de accesibilidad del medio y catálogos para los combos dinámicos.
    /// Solo vive en la capa Web; el servicio y el repositorio siguen usando ExcepcionDto.
    /// </summary>
    public class ExcepcionCondicionViewModel : ExcepcionDto
    {
        /// <summary>
        /// Calculado en página Web comparando el medio contra los medios accesibles del network.
        /// No procede de BD.
        /// </summary>
        public bool MedioAccesible { get; set; }

        /// <summary>Catálogo de tipos de disciplina disponibles para el combo de esta excepción.</summary>
        public List<CodigoDescripcion>? TiposDisciplinaDisponibles { get; set; }

        /// <summary>Catálogo de grupos de disciplina disponibles para el combo de esta excepción.</summary>
        public List<CodigoDescripcion>? DisciplinasGrupoDisponibles { get; set; }

        /// <summary>Catálogo de disciplinas disponibles para el combo de esta excepción.</summary>
        public List<CodigoDescripcion>? DisciplinasDisponibles { get; set; }

        /// <summary>Catálogo de objetivos disponibles para el combo de esta excepción.</summary>
        public List<CodigoDescripcion>? ObjetivosDisponibles { get; set; }

        /// <summary>Catálogo de tipos de compra disponibles para el combo de esta excepción.</summary>
        public List<CodigoDescripcion>? TiposCompraDisponibles { get; set; }
    }
}
