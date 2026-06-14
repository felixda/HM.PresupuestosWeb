namespace HM.Presupuestos.Application.CasosDeUso.Compartido
{
    /// <summary>
    /// Extiende IMaestrosService con capacidad de caché de sesión e invalidación explícita.
    /// </summary>
    public interface IMaestrosCacheService : IMaestrosService
    {
        /// <summary>
        /// Invalida una entrada específica de la caché por su clave de recurso.
        /// </summary>
        void Invalidar(string recurso);

        /// <summary>
        /// Invalida todas las entradas de la caché del usuario actual.
        /// </summary>
        void InvalidarTodo();
    }
}
