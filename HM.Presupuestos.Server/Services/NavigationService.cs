using Microsoft.AspNetCore.Components;

namespace HM.Presupuestos.Server.Services
{
    public interface INavigationService
    {
        string ObtenerUrlActual();
        string NormalizarUrl(string url);
        string ObtenerUrlBase();
        string ObtenerUrlActualNormalizada();
    }

    public class NavigationService : INavigationService
    {
        private readonly NavigationManager _navigationManager;

        public NavigationService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        /// <summary>
        /// Obtiene la URL actual sin query string ni dominio
        /// </summary>
        public string ObtenerUrlActual()
        {
            var requestUri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
            return requestUri.AbsolutePath.ToLower();
        }


        public string ObtenerUrlActualNormalizada()
        {
            var requestUri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
            return NormalizarUrl(requestUri.AbsolutePath.ToLower());
        }
        /// <summary>
        /// Normalizar URL eliminando parámetros numéricos y convirtiendo a minúsculas
        /// </summary>
        public string NormalizarUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return string.Empty;
            }

            try
            {
                var uri = _navigationManager.ToAbsoluteUri(url);
                var path = uri.AbsolutePath.ToLowerInvariant();

                var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var filteredSegments = segments
                    .Where(segment => !int.TryParse(segment, out _))
                    .ToArray();

                return filteredSegments.Length == 0
                    ? "/"
                    : "/" + string.Join("/", filteredSegments);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NavigationService] ⚠️ Error normalizando URL '{url}': {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Obtiene la URL base de la aplicación
        /// </summary>
        public string ObtenerUrlBase()
        {
            return _navigationManager.BaseUri;
        }
    }
}