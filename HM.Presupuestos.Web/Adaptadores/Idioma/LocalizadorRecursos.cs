using System.Collections.Concurrent;
using System.Text.Json;
using HM.Presupuestos.Domain.Entidades;

//using IdiomaEntidad = HM.Presupuestos.Domain.Entidades.Idioma;

namespace HM.Presupuestos.Web.Adaptadores.Idioma
{
    public interface ILocalizadorRecursos
    {
        string ObtenerTexto(string claveRecurso, string? codigoIdioma = null);
        bool ExisteRecurso(string claveRecurso, string? codigoIdioma = null);
        List<string> ObtenerCodigosIdiomas();
        List<HM.Presupuestos.Domain.Entidades.Idioma> ObtenerIdiomas();
    }

    public class LocalizadorRecursos : ILocalizadorRecursos
    {
        #region Propiedades privadas

        private readonly IConfiguration _configuracion;
        private readonly IGestorIdioma _gestorIdiomas;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IProveedorRecursosJson _proveedorJson;

        // Caché de valores resueltos por clave compuesta (idioma:expresión)
        private readonly ConcurrentDictionary<string, string> _cacheValoresResueltos = new();

        private readonly string _idiomaPorDefecto;

        #endregion

        #region Constructor

        public LocalizadorRecursos(IConfiguration configuracion, IGestorIdioma idiomaService,
            IServiceScopeFactory scopeFactory, IProveedorRecursosJson proveedorJson)
        {
            _configuracion = configuracion;
            _gestorIdiomas = idiomaService;
            _scopeFactory = scopeFactory;
            _proveedorJson = proveedorJson;

            _idiomaPorDefecto = _configuracion.GetValue<string>("AppSettings:DefaultLanguage") ?? "es";

            _gestorIdiomas.IdiomaCambiado += () =>
            {
                Console.WriteLine("[LocalizadorRecursos] ?? IdiomaEntidad cambiado, limpiando caché de valores...");
                _cacheValoresResueltos.Clear();
                return Task.CompletedTask;
            };
        }

        #endregion

        #region Métodos privados

        private string NormalizarCodigoIdioma(string? codigoIdioma)
        {
            var code = codigoIdioma ?? _gestorIdiomas.IdiomaActual ?? _idiomaPorDefecto;
            return code.Trim('"', '\\', ' ').Split('-')[0].ToLower();
        }

        private string GenerarClaveCache(string idioma, string claveRecurso) =>
            $"{idioma.ToLower()}:{claveRecurso.ToLower().Replace(" ", "")}";

        private bool IntentarNavegar(string claveRecurso, string codigoIdioma, out JsonElement resultado)
        {
            resultado = default;

            var doc = _proveedorJson.ObtenerDocumento(codigoIdioma);
            if (doc == null) return false;

            var element = doc.RootElement;
            foreach (var parte in claveRecurso.Replace(" ", "").Split(":"))
            {
                if (element.ValueKind == JsonValueKind.Array) return false;
                if (!_proveedorJson.IntentarObtenerPropiedadIgnorandoMayusculas(element, parte, out element)) return false;
            }

            resultado = element;
            return true;
        }

        #endregion

        #region Métodos públicos

        public string ObtenerTexto(string claveRecurso, string? codigoIdioma = null)
        {
            try
            {
                var idioma = NormalizarCodigoIdioma(codigoIdioma);
                var cacheKey = GenerarClaveCache(idioma, claveRecurso);

                if (_cacheValoresResueltos.TryGetValue(cacheKey, out var valorCacheado))
                    return valorCacheado;

                if (!IntentarNavegar(claveRecurso, idioma, out var element))
                    return $"Error label -> {claveRecurso}";

                string resultado = element.ValueKind switch
                {
                    JsonValueKind.Array => JsonSerializer.Serialize(element),
                    JsonValueKind.String => element.GetString() ?? "",
                    _ => element.GetRawText()
                };

                _cacheValoresResueltos.TryAdd(cacheKey, resultado);
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LocalizadorRecursos] ? Error en ObtenerTexto('{claveRecurso}'): {ex.Message}");
                RegistrarExcepcion($"{GetType().Name}.ObtenerTexto", ex);
                return $"Error label -> {claveRecurso}";
            }
        }

        public bool ExisteRecurso(string claveRecurso, string? codigoIdioma = null)
        {
            try
            {
                var idioma = NormalizarCodigoIdioma(codigoIdioma);
                var cacheKey = GenerarClaveCache(idioma, claveRecurso);

                if (_cacheValoresResueltos.ContainsKey(cacheKey)) return true;

                return IntentarNavegar(claveRecurso, idioma, out _);
            }
            catch (Exception ex)
            {
                RegistrarExcepcion($"{GetType().Name}.ExisteRecurso", ex);
                return false;
            }
        }

        public List<string> ObtenerCodigosIdiomas()
        {
            List<string> result = [];
            try
            {
                var idioma = NormalizarCodigoIdioma(null);
                var doc = _proveedorJson.ObtenerDocumento(idioma);
                if (doc == null) return result;

                var root = doc.RootElement;
                if (!root.TryGetProperty("Language", out var languageSection)) return result;

                foreach (var languageItem in languageSection.EnumerateObject())
                {
                    if (languageItem.Value.TryGetProperty("code", out var codeProp))
                    {
                        var code = codeProp.GetString();
                        if (!string.IsNullOrEmpty(code))
                            result.Add(code.ToLower());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LocalizadorRecursos] ? Error en ObtenerCodigosIdiomas: {ex.Message}");
                RegistrarExcepcion($"{GetType().Name}.ObtenerCodigosIdiomas", ex);
            }

            return result;
        }

        public List<HM.Presupuestos.Domain.Entidades.Idioma> ObtenerIdiomas()
        {
            List<HM.Presupuestos.Domain.Entidades.Idioma> result = [];
            try
            {
                var languageSection = _configuracion.GetSection("AppSettings:Languages");
                var languageCodeList = ObtenerCodigosIdiomas();

                foreach (var languageCode in languageCodeList)
                {
                    try
                    {
                        var languageSettings = languageSection.GetChildren()
                            .FirstOrDefault(lang => lang.GetSection("Code").Value?.ToLower() == languageCode);

                        if (languageSettings == null) continue;

                        var languageItem = new HM.Presupuestos.Domain.Entidades.Idioma { Iso = languageCode };

                        if (int.TryParse(languageSettings.GetSection("Id").Value, out int languageId))
                            languageItem.CodigoIdioma = languageId;

                        languageItem.Descripcion = ObtenerDescripcionIdioma(languageCode);
                        result.Add(languageItem);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[LocalizadorRecursos] ? Error procesando IdiomaEntidad {languageCode}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LocalizadorRecursos] ? Error en ObtenerIdiomas: {ex.Message}");
                RegistrarExcepcion($"{GetType().Name}.ObtenerIdiomas", ex);
            }

            return result;
        }

        private string ObtenerDescripcionIdioma(string codigoIdioma)
        {
            try
            {
                var languageKey = codigoIdioma.ToUpper();
                var resourceKey = $"Language:{languageKey}:label";
                var descripcion = ObtenerTexto(resourceKey, codigoIdioma);

                if (string.IsNullOrEmpty(descripcion) || descripcion == resourceKey)
                    return languageKey;

                return descripcion;
            }
            catch
            {
                return codigoIdioma.ToUpper();
            }
        }

        private void RegistrarExcepcion(string methodName, Exception exception)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var logService = scope.ServiceProvider.GetRequiredService<IRegistroAplicacion>();
                logService?.RegistrarEvento(exception.TargetSite?.Name ?? methodName, exception.Message, exception.StackTrace, "");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LocalizadorRecursos] ? Error al insertar excepción: {ex.Message}");
            }
        }

        #endregion
    }
}





