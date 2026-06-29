namespace HM.Presupuestos.Domain.Entidades.LogAcciones
{
    public class FiltroLogsTecnicos
    {
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string? Nivel { get; set; }
        public string? Usuario { get; set; }
        public string? Categoria { get; set; }
        public string? Mensaje { get; set; }
    }
}