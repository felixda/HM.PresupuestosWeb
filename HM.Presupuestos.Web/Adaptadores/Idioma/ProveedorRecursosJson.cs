using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace HM.Presupuestos.Web.Adaptadores.Idioma
{
    /// <summary>
    /// Responsable de cargar, cachear y consultar los archivos JSON de recursos por idioma.
    /// </summary>
    public interface IProveedorRecursosJson
    {
        JsonDocument? ObtenerDocumento(string codigoIdioma);
        bool IntentarObtenerPropiedadIgnorandoMayusculas(JsonElement elemento, 
            string propiedad, out JsonElement valorPropiedad);
        //void LimpiarCacheValores();
    }

    public class ProveedorRecursosJson : IProveedorRecursosJson
    {
        private readonly IConfiguration _configuracion;
        private readonly IWebHostEnvironment _entornoWeb;

        // CachÃ© nivel 1: JsonDocument por idioma
        private readonly ConcurrentDictionary<string, JsonDocument> _recursosPorIdioma = new();

        public ProveedorRecursosJson(IConfiguration configuracion, IWebHostEnvironment entornoWeb)
        {
            _configuracion = configuracion;
            _entornoWeb = entornoWeb;
        }

        /// <summary>
        /// Devuelve el JsonDocument del idioma indicado, cargÃ¡ndolo si no estaba en memoria.
        /// </summary>
        public JsonDocument? ObtenerDocumento(string codigoIdioma)
        {
            var languageCode = codigoIdioma.ToLower();

            if (!_recursosPorIdioma.ContainsKey(languageCode))
                CargarRecursosDeIdioma(languageCode);

            return _recursosPorIdioma.TryGetValue(languageCode, out var doc) ? doc : null;
        }

        // Limpia la caché interna de documentos JSON (útil al cambiar idioma).
        //public void LimpiarCacheValores()
        //{
        //    // Los JsonDocuments son inmutables; no necesitan limpiarse.
        //    // Expuesto para que LocalizadorRecursos pueda limpiar su propia cachÃ© de valores.
        //}

        /// <summary>
        /// Busca una propiedad en un JsonElement sin distinciÃ³n de mayÃºsculas/minÃºsculas.
        /// </summary>
        public bool IntentarObtenerPropiedadIgnorandoMayusculas(JsonElement elemento, 
            string propiedad, out JsonElement valorPropiedad)
        {
            foreach (JsonProperty property in elemento.EnumerateObject())
            {
                if (string.Equals(property.Name, propiedad, StringComparison.OrdinalIgnoreCase))
                {
                    valorPropiedad = property.Value;
                    return true;
                }
            }

            valorPropiedad = default;
            return false;
        }

        /// <summary>
        /// Carga el archivo JSON del idioma indicado desde disco y lo almacena en memoria.
        /// </summary>
        private void CargarRecursosDeIdioma(string codigoIdioma)
        {
            try
            {
                if (_recursosPorIdioma.ContainsKey(codigoIdioma))
                    return;

                var languagesSectionConfig = _configuracion.GetSection("AppSettings:Languages").AsEnumerable().ToList();

                var languageConfigKey = languagesSectionConfig
                    .FirstOrDefault(x => x.Key != null && x.Key.EndsWith(":Code") && x.Value?.ToLower() == codigoIdioma);

                if (languageConfigKey.Key == null)
                {
                    Console.WriteLine($"[ProveedorRecursosJson] âš ï¸ Idioma no encontrado en configuraciÃ³n: {codigoIdioma}");
                    return;
                }

                var languageNodeName = languageConfigKey.Key.Split(":")[2];
                var pathJsonFile = languagesSectionConfig
                    .FirstOrDefault(x => x.Key != null && x.Key.EndsWith($"{languageNodeName}:ResourcesPath")).Value;

                if (string.IsNullOrEmpty(pathJsonFile))
                {
                    Console.WriteLine($"[ProveedorRecursosJson] âš ï¸ No se encontrÃ³ ruta del archivo de recursos para: {codigoIdioma}");
                    return;
                }

                var fullPath = Path.Combine(_entornoWeb.WebRootPath, pathJsonFile);

                if (!File.Exists(fullPath))
                {
                    Console.WriteLine($"[ProveedorRecursosJson] âš ï¸ Archivo de recursos no existe: {fullPath}");
                    return;
                }

                var content = File.ReadAllText(fullPath, Encoding.UTF8);
                var jsonDoc = JsonDocument.Parse(content);
                _recursosPorIdioma.TryAdd(codigoIdioma, jsonDoc);

                Console.WriteLine($"[ProveedorRecursosJson] âœ… Idioma cargado en memoria: {codigoIdioma}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProveedorRecursosJson] âŒ Error cargando idioma {codigoIdioma}: {ex.Message}");
            }
        }
    }
}



