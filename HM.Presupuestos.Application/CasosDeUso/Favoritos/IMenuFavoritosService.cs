namespace HM.Presupuestos.Application.CasosDeUso
{
    /// <summary>
    /// Servicio de gestión de menús favoritos del usuario en la API de HM.CORE.
    /// </summary>
    public interface IMenuFavoritosService
    {
        /// <summary>
        /// Obtiene los códigos de menús favoritos del usuario autenticado.
        /// </summary>
        /// <returns>HashSet con los códigos de menús favoritos.</returns>
        Task<HashSet<string>> ObtenerFavoritos();

        /// <summary>
        /// Persiste los códigos de menús favoritos del usuario autenticado.
        /// </summary>
        /// <param name="favoritos">HashSet con los códigos de menús favoritos a guardar.</param>
        Task GuardarFavoritos(HashSet<string> favoritos);
    }
}
