namespace HM.Presupuestos.Domain.Entidades
{
    public class DetalleError
    {
        public string UserName { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string StackTrace { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
