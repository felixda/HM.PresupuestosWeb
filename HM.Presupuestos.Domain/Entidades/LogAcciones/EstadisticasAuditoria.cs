namespace HM.Presupuestos.Domain.Entidades.LogAcciones
{
    public class EstadisticasAuditoria
    {
        public int TotalAcciones { get; set; }
        public int UsuariosUnicos { get; set; }
        public string UsuarioMasActivo { get; set; } = string.Empty;
        public int UsuarioMasActivoTotal { get; set; }
        public string PaginaMasVisitada { get; set; } = string.Empty;
        public int PaginaMasVisitadaTotal { get; set; }
        public List<PuntoTemporal> ActividadPorDia { get; set; } = [];
        public List<UsuarioContador> TopUsuarios { get; set; } = [];
    }
}
