namespace HM.Presupuestos.Application.CasosDeUso
{
    /// <summary>
    /// Servicio de gestiÃ³n de menÃºs favoritos del usuario en la API de HM.CORE.
    /// </summary>
    public interface IMenuFavoritosService
    {
        /// <summary>
        /// Obtiene los cÃ³digos de menÃºs favoritos del usuario autenticado.
        /// </summary>
        /// <returns>HashSet con los cÃ³digos de menÃºs favoritos.</returns>
        Task<HashSet<string>> ObtenerFavoritos();

        /// <summary>
        /// Persiste los cÃ³digos de menÃºs favoritos del usuario autenticado.
        /// </summary>
        /// <param name="favoritos">HashSet con los cÃ³digos de menÃºs favoritos a guardar.</param>
        Task GuardarFavoritos(HashSet<string> favoritos);
    }
}
