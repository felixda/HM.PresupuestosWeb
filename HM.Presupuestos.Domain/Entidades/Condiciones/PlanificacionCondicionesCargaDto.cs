using HM.Presupuestos.Domain.Entidades;


namespace HM.Presupuestos.Domain.Pack.Entidades
{
    public class PlanificacionCondicionesCargaDto
    {
        public List<CodigoDescripcion> Alcances { get; set; } = [];
        public List<CodigoDescripcion> Disciplinas { get; set; } = [];
        public List<CodigoDescripcion> Diversifieds { get; set; } = [];
        public List<CodigoDescripcion> Objetivos { get; set; } = [];
        public List<CodigoDescripcion> TiposCompra { get; set; } = [];
        public List<CodigoDescripcion> DisciplinasGrupo { get; set; } = [];
        public List<CodigoDescripcion> TiposDisciplina { get; set; } = [];

    }
}


