using Microsoft.AspNetCore.Components;

namespace HM.Presupuestos.Web.Adaptadores.Navegacion
{
    public interface IRutasNavegacion
    {
        string ObtenerRutaActual();
        string NormalizarRuta(string url);
        string ObtenerUrlBase();
        string ObtenerRutaActualNormalizada();
    }

    public class RutasNavegacion : IRutasNavegacion
    {
        private readonly NavigationManager _navigationManager;

        public RutasNavegacion(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        /// <summary>
        /// Obtiene la URL actual sin query string ni dominio
        /// </summary>
        public string ObtenerRutaActual()
        {
            var requestUri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
            return requestUri.AbsolutePath.ToLower();
        }


        public string ObtenerRutaActualNormalizada()
        {
            var requestUri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
            return NormalizarRuta(requestUri.AbsolutePath.ToLower());
        }
        /// <summary>
        /// Normalizar URL eliminando parámetros numéricos y convirtiendo a minúsculas
        /// </summary>
        public string NormalizarRuta(string url)
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
                Console.WriteLine($"[NavigationService] ?? Error normalizando URL '{url}': {ex.Message}");
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


