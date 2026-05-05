using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using HM.Presupuestos.Domain.Entidades;

namespace HM.Presupuestos.Server.Services
{
    public interface IResourceService
    {
        string T(string elementExpression, string? codigoIdioma = null);
        bool ExisteValue(string elementExpression, string? codigoIdioma = null);

        string? BuscarUrlMenuPorCodeMenuEnJson(int codigoBuscado);

        int ObtenerCodigoMenu(string url);

        List<string> ObtenerCodigosIdiomas();

        List<Idioma> ObtenerIdiomas();
    }

    public class ResourceService: IResourceService
    {

        #region Propiedades privadas
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IIdiomaService _idiomaService;
        //private Dictionary<string, JsonDocument> _dictionaryLanguage;
        private readonly IServiceScopeFactory _scopeFactory;

        // ? Caché nivel 1: JsonDocument por idioma
        private readonly ConcurrentDictionary<string, JsonDocument> _dictionaryLanguage = new();

        // ? Caché nivel 2: Valores resueltos por clave compuesta (idioma:expresión)
        private readonly ConcurrentDictionary<string, string> _cacheValoresResueltos = new();

        // ? Cache de idioma por defecto
        private string _idiomaPorDefecto;

        #endregion

        #region Constructor
        public ResourceService(IConfiguration config, IWebHostEnvironment webHostEnvironment, 
            IIdiomaService idiomaService, IServiceScopeFactory scopeFactory)
        {
            _config = config;
            _webHostEnvironment = webHostEnvironment;
            _idiomaService = idiomaService;
            _scopeFactory = scopeFactory;

            _idiomaPorDefecto = _config.GetValue<string>("AppSettings:DefaultLanguage") ?? "es";

            // Suscribirse al cambio de idioma para limpiar caché de valores resueltos
            _idiomaService.OnIdiomaCambiado += () =>
            {
                Console.WriteLine("[ResourceService] ?? Idioma cambiado, limpiando caché de valores...");
                // Solo limpiar caché de valores, mantener JsonDocuments cargados
                _cacheValoresResueltos.Clear();
                return Task.CompletedTask;
            };
        }

       
        #endregion



        #region Métodos


        /// <summary>
        /// Cargar un idioma específico en memoria (versión síncrona para constructor)
        /// </summary>
        private void CargarRecursosIdiomaEnMemoriaSync(string codigoIdioma)
        {
            try
            {
                var languageCode = codigoIdioma.ToLower();

                // ? Si ya está cargado, no hacer nada
                if (_dictionaryLanguage.ContainsKey(languageCode))
                {
                    Console.WriteLine($"[ResourceService] ?? Idioma ya en memoria: {languageCode}");
                    return;
                }

                var obLanguagesSectionConfig = _config.GetSection("AppSettings:Languages").AsEnumerable().ToList();

                // Buscar configuración del idioma
                var languageConfigKey = obLanguagesSectionConfig
                    .FirstOrDefault(x => x.Key != null && x.Key.EndsWith(":Code") && x.Value?.ToLower() == languageCode);

                if (languageConfigKey.Key == null)
                {
                    Console.WriteLine($"[ResourceService] ?? Idioma no encontrado en configuración: {languageCode}");
                    return;
                }

                var languageNodeName = languageConfigKey.Key.Split(":")[2];
                var pathJsonFile = obLanguagesSectionConfig
                    .FirstOrDefault(x => x.Key != null && x.Key.EndsWith($"{languageNodeName}:ResourcesPath")).Value;

                if (string.IsNullOrEmpty(pathJsonFile))
                {
                    Console.WriteLine($"[ResourceService] ?? No se encontró ruta del archivo de recursos para: {languageCode}");
                    return;
                }

                string fullPathJsonFile = Path.Combine(_webHostEnvironment.WebRootPath, pathJsonFile);

                if (!File.Exists(fullPathJsonFile))
                {
                    Console.WriteLine($"[ResourceService] ?? Archivo de recursos no existe: {fullPathJsonFile}");
                    return;
                }

                string fileJsonContent = File.ReadAllText(fullPathJsonFile, Encoding.UTF8);
                var jsonDoc = JsonDocument.Parse(fileJsonContent);

                _dictionaryLanguage.TryAdd(languageCode, jsonDoc);

                Console.WriteLine($"[ResourceService] ? Idioma cargado en memoria: {languageCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ResourceService] ? Error cargando idioma {codigoIdioma}: {ex.Message}");
                InsertException($"{GetType().Name}.CargarIdiomaEnMemoriaSync", ex);
            }
        }

       

        /// <summary>
        /// Generar clave única para caché de valores resueltos
        /// </summary>
        private string GenerarClaveCacheValor(string idioma, string elementExpression)
        {
            return $"{idioma.ToLower()}:{elementExpression.ToLower().Replace(" ", "")}";
        }


        public string T(string elementExpression, string? codigoIdioma = null)
        {
            try
            {
                // ? 1. Obtener idioma
                var languageCode = codigoIdioma ?? _idiomaService.Idioma ?? _idiomaPorDefecto;
                var cleanLanguageCode = languageCode.Trim('"', '\\', ' ');
                var resourceLanguageCode = cleanLanguageCode.Split('-')[0].ToLower();

                // ? 2. Generar clave de caché
                var cacheKey = GenerarClaveCacheValor(resourceLanguageCode, elementExpression);

                // ? 3. INTENTAR OBTENER DEL CACHÉ PRIMERO
                if (_cacheValoresResueltos.TryGetValue(cacheKey, out var valorCacheado))
                {
                    // ? Cache hit - devolver inmediatamente
                    return valorCacheado;
                }


                if (!_dictionaryLanguage.ContainsKey(resourceLanguageCode))
                {
                    CargarRecursosIdiomaEnMemoriaSync(resourceLanguageCode);
                }

                // ? 4. No está en caché, cargar idioma si es necesario
                if (!_dictionaryLanguage.TryGetValue(resourceLanguageCode, out var obJsonDocument))
                {
                    Console.WriteLine($"[ResourceService] ? No se pudo cargar idioma: {resourceLanguageCode}");
                    return $"Error label -> {elementExpression} (idioma no disponible)";
                }

                var obJsonElement = obJsonDocument.RootElement;

                // Navegar por la expresión
                var obElementExpressionList = elementExpression.Replace(" ", "").Split(":");
                foreach (var element in obElementExpressionList)
                {
                    if (obJsonElement.ValueKind == JsonValueKind.Array)
                        break;

                    if (!TryGetPropertyCaseInsensitive(obJsonElement, element, out obJsonElement))
                    {
                        return $"Error label -> {elementExpression}";
                    }
                }

                // Devolver valor según tipo
                string resultado = obJsonElement.ValueKind switch
                {
                    JsonValueKind.Array => JsonSerializer.Serialize(obJsonElement),
                    JsonValueKind.String => obJsonElement.GetString() ?? "",
                    _ => obJsonElement.GetRawText()
                };

                //GUARDAR EN CACHÉ para futuros usos
                _cacheValoresResueltos.TryAdd(cacheKey, resultado);


                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ResourceService] ? Error en GetValue('{elementExpression}'): {ex.Message}");
                InsertException($"{GetType().Name}.GetValue", ex);
                return $"Error label -> {elementExpression}";
            }
        }


        public bool ExisteValue(string elementExpression,  string? codigoIdioma = null)
        {

            try
            {
                var languageCode = codigoIdioma ?? _idiomaService.Idioma ?? _idiomaPorDefecto;
                var cleanLanguageCode = languageCode.Trim('"', '\\', ' ');
                var resourceLanguageCode = cleanLanguageCode.Split('-')[0].ToLower();


                // ? Verificar en caché primero
                var cacheKey = GenerarClaveCacheValor(resourceLanguageCode, elementExpression);
                if (_cacheValoresResueltos.ContainsKey(cacheKey))
                {
                    return true; // Si está en caché, existe
                }

                // ? Cargar idioma si no está en memoria
                if (!_dictionaryLanguage.ContainsKey(resourceLanguageCode))
                {
                    CargarRecursosIdiomaEnMemoriaSync(resourceLanguageCode);
                }

                return TryGetJsonElement(elementExpression, resourceLanguageCode, out _);
            }
            catch (Exception ex)
            {
                InsertException($"{GetType().Name}.ExisteValue", ex);
                return false;
            }
        }

        /// <summary>
        /// Limpiar toda la caché de valores resueltos (útil para testing o cambios de idioma)
        /// </summary>
        public void LimpiarCache()
        {
            _cacheValoresResueltos.Clear();
            Console.WriteLine("[ResourceService] ??? Caché de valores resueltos limpiada");
        }

        /// <summary>
        /// Find property name in json element for case insensitive
        /// </summary>
        /// <param name="element">JsonElement element reference</param>
        /// <param name="propertyName">Property name to find</param>
        /// <param name="propertyValue">Property value found</param>
        /// <returns>True when property found, otherwise false</returns>
        private bool TryGetPropertyCaseInsensitive(JsonElement element, string propertyName, out JsonElement propertyValue)
        {
            foreach (JsonProperty property in element.EnumerateObject())
            {
                if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    propertyValue = property.Value;
                    return true;
                }
            }

            propertyValue = default;
            return false;
        }

        private bool TryGetJsonElement(string elementExpression, string languageCode, out JsonElement obJsonElement)
        {
            obJsonElement = default;

            if (string.IsNullOrEmpty(languageCode))
                return false;

            var resourceLanguageCode = languageCode.Split('-')[0];

            if (!_dictionaryLanguage.TryGetValue(resourceLanguageCode, out var jsonDoc))
                return false;

            obJsonElement = jsonDoc.RootElement;

            var obElementExpressionList = elementExpression.Replace(" ", "").Split(":");

            foreach (var element in obElementExpressionList)
            {
                if (obJsonElement.ValueKind == JsonValueKind.Array)
                    return false;

                if (!TryGetPropertyCaseInsensitive(obJsonElement, element, out obJsonElement))
                    return false;
            }

            return true;
        }




        public string? BuscarUrlMenuPorCodeMenuEnJson(int codigoBuscado)
        {
            try
            {
                // ? Obtener idioma actual
                var idioma = _idiomaService.Idioma.ToLower() ?? _idiomaPorDefecto;

                // ? Cargar si no está en memoria
                if (!_dictionaryLanguage.ContainsKey(idioma))
                {
                    CargarRecursosIdiomaEnMemoriaSync(idioma);
                }

                if (!_dictionaryLanguage.TryGetValue(idioma, out var obJsonDocument))
                {
                    Console.WriteLine($"[ResourceService] ?? Idioma no disponible para buscar menú: {idioma}");
                    return null;
                }

                var root = obJsonDocument.RootElement;

                if (root.TryGetProperty("Menu", out var menuSection))
                {
                    foreach (var item in menuSection.EnumerateObject())
                    {
                        var menuItem = item.Value;

                        if (menuItem.TryGetProperty("code", out var codeProp) &&
                            codeProp.GetInt32() == codigoBuscado)
                        {
                            if (menuItem.TryGetProperty("url", out var urlProp))
                            {
                                return urlProp.GetString();
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ResourceService] ? Error buscando URL menú {codigoBuscado}: {ex.Message}");
                InsertException($"{GetType().Name}.BuscarUrlMenuPorCodeMenuEnJson", ex);
                return null;
            }
        }



        public int ObtenerCodigoMenu(string url)
        {
            var result = -1;

            try
            {
                var jsonMenu = T("menu");

                if (string.IsNullOrEmpty(jsonMenu))
                {
                    Console.WriteLine("[ResourceService] ?? No se pudo obtener JSON del menú");
                    return result;
                }

                JsonNode? jsonNode = JsonNode.Parse(jsonMenu);
                if (jsonNode is not JsonObject jsonObject)
                {
                    return result;
                }

                foreach (var property in jsonObject.AsObject())
                {
                    if (property.Value is not JsonObject element)
                        continue;

                    var urlElement = element["url"]?.ToString();
                    if (string.IsNullOrEmpty(urlElement))
                        continue;

                    if (urlElement.Equals(url, StringComparison.OrdinalIgnoreCase))
                    {
                        bool.TryParse(element["visible"]?.ToString(), out bool visibleMenu);
                        result = visibleMenu ? 0 : -1;

                        if (visibleMenu &&
                            element.TryGetPropertyValue("code", out JsonNode? codeNode) &&
                            codeNode is JsonValue codeValue &&
                            codeValue.TryGetValue<int>(out int code))
                        {
                            result = code;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ResourceService] ? Error en ObtenerCodigoMenu: {ex.Message}");
                InsertException($"{GetType().Name}.ObtenerCodigoMenu", ex);
            }

            return result;
        }


        /// <summary>
        /// Obtiene la lista de códigos de idiomas desde los archivos JSON de recursos
        /// </summary>
        /// <returns>Lista de códigos ISO de idiomas (es, en, pt)</returns>
        public List<string> ObtenerCodigosIdiomas()
        {
            List<string> result = [];

            try
            {
                // Obtener el idioma actual
                var idioma = _idiomaService.Idioma.ToLower() ?? _idiomaPorDefecto;

                // Cargar si no está en memoria
                if (!_dictionaryLanguage.ContainsKey(idioma))
                {
                    CargarRecursosIdiomaEnMemoriaSync(idioma);
                }

                if (!_dictionaryLanguage.TryGetValue(idioma, out var jsonDoc))
                {
                    Console.WriteLine($"[ResourceService] ?? No se pudo cargar idioma para obtener códigos: {idioma}");
                    return result;
                }

                var root = jsonDoc.RootElement;

                // Navegar a Language section
                if (root.TryGetProperty("Language", out var languageSection))
                {
                    foreach (var languageItem in languageSection.EnumerateObject())
                    {
                        if (languageItem.Value.TryGetProperty("code", out var codeProp))
                        {
                            var code = codeProp.GetString();
                            if (!string.IsNullOrEmpty(code))
                            {
                                result.Add(code.ToLower());
                            }
                        }
                    }
                }

                Console.WriteLine($"[ResourceService] ? Códigos de idiomas obtenidos: {string.Join(", ", result)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ResourceService] ? Error obteniendo códigos de idiomas: {ex.Message}");
                InsertException($"{GetType().Name}.ObtenerCodigosIdiomas", ex);
            }

            return result;
        }

        /// <summary>
        /// Obtiene la lista completa de idiomas disponibles con ID, código ISO y descripción
        /// Lee la información desde appsettings.json y los archivos JSON de recursos
        /// </summary>
        /// <returns>Lista de objetos Idioma con todos sus datos</returns>
        public List<Idioma> ObtenerIdiomas()
        {
            List<Idioma> result = [];

            try
            {
                var obLanguageSection = _config.GetSection("AppSettings:Languages");
                var languageCodeList = ObtenerCodigosIdiomas();

                foreach (var languageCode in languageCodeList)
                {
                    try
                    {
                        // Obtener settings del idioma desde appsettings.json
                        var languageSettings = obLanguageSection.GetChildren()
                            .FirstOrDefault(lang => lang.GetSection("Code").Value?.ToLower() == languageCode.ToLower());

                        if (languageSettings == null)
                        {
                            Console.WriteLine($"[ResourceService] ?? No se encontró configuración para idioma: {languageCode}");
                            continue;
                        }

                        Idioma languageItem = new();

                        // Obtener ID del appsettings.json
                        if (int.TryParse(languageSettings.GetSection("Id").Value, out int languageId))
                        {
                            languageItem.CodigoIdioma = languageId;
                        }

                        // Obtener código ISO
                        languageItem.Iso = languageCode;

                        // Obtener descripción del archivo JSON de recursos
                        languageItem.Descripcion = ObtenerDescripcionIdioma(languageCode);

                        result.Add(languageItem);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ResourceService] ? Error procesando idioma {languageCode}: {ex.Message}");
                    }
                }

                Console.WriteLine($"[ResourceService] ? Lista de idiomas obtenida: {result.Count} idiomas");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ResourceService] ? Error obteniendo idiomas: {ex.Message}");
                InsertException($"{GetType().Name}.ObtenerIdiomas", ex);
            }

            return result;
        }

        /// <summary>
        /// Obtiene la descripción del idioma desde el archivo JSON de recursos
        /// </summary>
        /// <param name="codigoIdioma">Código ISO del idioma (es, en, pt)</param>
        /// <returns>Descripción del idioma en su propio idioma</returns>
        private string ObtenerDescripcionIdioma(string codigoIdioma)
        {
            try
            {
                var languageKey = codigoIdioma.ToUpper();
                var resourceKey = $"Language:{languageKey}:label";

                // Usar el método T() existente para obtener la traducción
                var descripcion = T(resourceKey, codigoIdioma);

                // Si no se encuentra o devuelve la misma clave, usar el código como fallback
                if (string.IsNullOrEmpty(descripcion) || descripcion == resourceKey)
                {
                    Console.WriteLine($"[ResourceService] ?? No se encontró descripción para idioma {codigoIdioma}, usando fallback");
                    return languageKey;
                }

                return descripcion;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ResourceService] ? Error obteniendo descripción para idioma {codigoIdioma}: {ex.Message}");
                return codigoIdioma.ToUpper();
            }
        }


        /// <summary>
        /// Insert exception en log database and file
        /// </summary>
        /// <param name="exception">Exception object</param>
        /// <param name="methodName">Method name from exception</param>
        private void InsertException(string methodName, Exception exception)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var serviceLogService = scope.ServiceProvider.GetRequiredService<ILogService>();
                if (serviceLogService != null)
                {
                    var category = exception.TargetSite?.Name ?? methodName;
                    serviceLogService.InsertLog(category, exception.Message, exception.StackTrace,"");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ResourceService] ? Error al insertar excepción: {ex.Message}");
            }
        }
        #endregion
    }
}
