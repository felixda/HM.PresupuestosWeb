namespace HM.Presupuestos.Domain.Entidades.LogAcciones
{
    public class LogTecnico
    {
        public DateTime Fecha { get; set; }
        public string Nivel { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string Logger { get; set; } = string.Empty;
        public string Excepcion { get; set; } = string.Empty;
        public string StackTrace { get; set; } = string.Empty;
        public string Comentarios { get; set; } = string.Empty;
    }
}