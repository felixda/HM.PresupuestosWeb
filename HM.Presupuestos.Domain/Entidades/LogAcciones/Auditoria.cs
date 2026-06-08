namespace HM.Presupuestos.Domain.Entidades.LogAcciones
{
    public class Auditoria
    {
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public string Usuario { get; set; } = string.Empty;
    }
}
