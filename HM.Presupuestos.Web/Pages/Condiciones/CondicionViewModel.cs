using HM.Presupuestos.Domain.Entidades;

namespace HM.Presupuestos.Web.Pages.Condiciones
{
    /// <summary>
    /// ViewModel de la capa Web que extiende CondicionDto con estado de UI:
    /// flag de accesibilidad del medio para controlar ediciÃ³n en el grid.
    /// Solo vive en la capa Web; el servicio y el repositorio usan CondicionDto.
    /// </summary>
    public class CondicionViewModel : CondicionDto
    {
        /// <summary>
        /// Calculado en pÃ¡gina Web comparando el medio contra los medios accesibles del network.
        /// No procede de BD.
        /// </summary>
        public bool MedioAccesible { get; set; }
    }
}
