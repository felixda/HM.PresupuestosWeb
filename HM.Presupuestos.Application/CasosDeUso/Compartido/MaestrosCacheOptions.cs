namespace HM.Presupuestos.Application.CasosDeUso.Compartido
{
    /// <summary>
    /// Opciones de configuración para la caché de datos maestros.
    /// </summary>
    public class MaestrosCacheOptions
    {
        /// <summary>
        /// Tiempo de vida de las entradas en caché, en minutos. Por defecto: 30.
        /// </summary>
        public int TtlMinutos { get; set; } = 30;
    }
}
