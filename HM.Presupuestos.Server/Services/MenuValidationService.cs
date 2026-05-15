using HM.Presupuestos.Server.Extensiones;
using System.Reflection;

namespace HM.Presupuestos.Server.Services
{
    public interface IValidadorMenusUsuario
    {
        Task<List<ResultadoValidacionSubmenu>> ValidarSubmenusDe(UsuarioEntidad usuario);
        List<string> ObtenerRutasDisponibles();
    }

    public class ValidadorMenusUsuario : IValidadorMenusUsuario
    {
        private readonly ILogger<ValidadorMenusUsuario> _logger;
        private readonly ILocalizadorRecursos _traductorRecursos;
        private readonly Dictionary<string, string> _rutasBlazor;

        public ValidadorMenusUsuario(
            ILogger<ValidadorMenusUsuario> logger,
            ILocalizadorRecursos traductorRecursos    )
        {
            _logger = logger;
            _traductorRecursos = traductorRecursos;
            _rutasBlazor = [];
            DescubrirRutasBlazor();
        }

        /// <summary>
        /// Descubre todas las rutas Blazor del ensamblado actual
        /// </summary>
        private void DescubrirRutasBlazor()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var tipos = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && typeof(ComponentBase).IsAssignableFrom(t));

                foreach (var tipo in tipos)
                {
                    var routeAttributes = tipo.GetCustomAttributes<RouteAttribute>();

                    foreach (var routeAttr in routeAttributes)
                    {
                        var rutaNormalizada = NormalizarRuta(routeAttr.Template);
                        if (!_rutasBlazor.ContainsKey(rutaNormalizada))
                        {
                            _rutasBlazor.Add(rutaNormalizada, tipo.FullName ?? tipo.Name);
                            _logger.LogDebug($"[MenuValidation] Página encontrada: {rutaNormalizada} -> {tipo.Name}");
                        }
                    }
                }

                _logger.LogInformation($"[MenuValidation] ✅ Se encontraron {_rutasBlazor.Count} páginas Blazor");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[MenuValidation] ❌ Error al cargar páginas Blazor");
            }
        }

        /// <summary>
        /// Normaliza una ruta eliminando parámetros y caracteres especiales
        /// </summary>
        private static string NormalizarRuta(string ruta)
        {
            if (string.IsNullOrWhiteSpace(ruta))
                return string.Empty;

            // Eliminar parámetros de ruta como {id}, {codigo}, etc.
            var rutaLimpia = System.Text.RegularExpressions.Regex.Replace(
                ruta,
                @"\{[^}]+\}",
                string.Empty
            );

            // Limpiar barras duplicadas
            rutaLimpia = System.Text.RegularExpressions.Regex.Replace(
                rutaLimpia,
                @"/+",
                "/"
            );

            // Convertir a minúsculas y eliminar barra final
            return rutaLimpia.ToLowerInvariant().TrimEnd('/');
        }

        /// <summary>
        /// Valida SOLO los menús hijos (submenús) con IdPadre != 0
        /// </summary>
        public async Task<List<ResultadoValidacionSubmenu>> ValidarSubmenusDe(UsuarioEntidad usuario)
        {
            var resultados = new List<ResultadoValidacionSubmenu>();

            if (usuario?.Menus == null || usuario.Menus.Count == 0)
            {
                _logger.LogWarning("[MenuValidation] ⚠️ Usuario sin menús para validar");
                return resultados;
            }

            // Filtrar SOLO menús hijos
            var menusHijos = usuario.Menus.Where(m => m.TienePadre()).ToList();

            if (menusHijos.Count == 0)
            {
                _logger.LogWarning($"[MenuValidation] ⚠️ Usuario {usuario.Login} no tiene menús hijos");
                return resultados;
            }

            _logger.LogInformation($"[MenuValidation] 🔍 Validando {menusHijos.Count} menús hijos del usuario {usuario.Login}");

            foreach (var menu in menusHijos)
            {
                var resultado = new ResultadoValidacionSubmenu
                {
                    CodigoMenu = menu.Id,
                    NombreMenu = menu.NombreMenu,
                    CodigoMenuPadre = menu.IdPadre
                };

                try
                {
                    var urlKey = AppResources.Menu.ObtenerUrl(menu.Id);
                    var url = _traductorRecursos.ObtenerTexto(urlKey);

                    resultado.UrlOriginal = url;

                    // También puedes obtener otros datos del menú
                    //var label = _resourceService.ObtenerTexto(AppResources.Menu.ObtenerEtiqueta(menu.Id));
                    //var icono = _resourceService.ObtenerTexto(AppResources.Menu.ObtenerIcono(menu.Id));

                    if (string.IsNullOrWhiteSpace(url))
                    {
                        resultado.Existe = false;
                        resultado.Mensaje = "⚠️ URL vacía o no encontrada en recursos";
                        _logger.LogWarning($"[MenuValidation] ⚠️ Menú hijo {menu.Id} ({menu.NombreMenu}) sin URL");
                    }
                    else
                    {
                        // Normalizar URL para comparación
                        var urlNormalizada = NormalizarRuta(url);
                        resultado.UrlNormalizada = urlNormalizada;

                        // Verificar si existe la página
                        if (_rutasBlazor.TryGetValue(urlNormalizada, out string? value))
                        {
                            resultado.Existe = true;
                            resultado.ComponenteBlazor = value;
                            resultado.Mensaje = "✅ Página Blazor encontrada";
                            _logger.LogDebug($"[MenuValidation] ✅ Menú hijo {menu.Id} ({menu.NombreMenu}) -> URL válida: {url}");
                        }
                        else
                        {
                            resultado.Existe = false;
                            resultado.Mensaje = $"❌ Página Blazor NO encontrada";
                            _logger.LogWarning($"[MenuValidation] ❌ Menú hijo {menu.Id} ({menu.NombreMenu}) -> URL inválida: {url}");

                            // Buscar URLs similares para ayudar a corregir
                            var urlsSimilares = BuscarUrlsSimilares(urlNormalizada);
                            if (urlsSimilares.Count != 0)
                            {
                                resultado.UrlsSimilares = urlsSimilares;
                                resultado.Mensaje += $" | Similares: {string.Join(", ", urlsSimilares.Take(2))}";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    resultado.Existe = false;
                    resultado.Mensaje = $"❌ Error al validar: {ex.Message}";
                    _logger.LogError(ex, $"[MenuValidation] ❌ Error validando menú hijo {menu.Id}");
                }

                resultados.Add(resultado);
            }

            // Resumen detallado
            var menusValidos = resultados.Count(r => r.Existe);
            var menusInvalidos = resultados.Count(r => !r.Existe);

            _logger.LogInformation($"[MenuValidation] 📊 Resumen: {menusValidos} válidos, {menusInvalidos} inválidos de {menusHijos.Count} menús hijos");

            // Log de menús inválidos
            if (menusInvalidos > 0)
            {
                _logger.LogWarning($"[MenuValidation] ⚠️ Menús hijos con URLs inválidas:");
                foreach (var resultado in resultados.Where(r => !r.Existe))
                {
                    _logger.LogWarning($"   - ID: {resultado.CodigoMenu}, Nombre: {resultado.NombreMenu}, URL: {resultado.UrlOriginal}");
                }
            }

            return resultados;
        }

        /// <summary>
        /// Busca URLs similares usando distancia de Levenshtein simplificada
        /// </summary>
        private List<string> BuscarUrlsSimilares(string url)
        {
            return [.. _rutasBlazor.Keys
                .Where(k => CalcularSimilitudEntreRutas(k, url) > 0.6)
                .OrderByDescending(k => CalcularSimilitudEntreRutas(k, url))
                .Take(3)];
        }

        /// <summary>
        /// Calcula similitud entre dos strings (0-1)
        /// </summary>
        private double CalcularSimilitudEntreRutas(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
                return 0;

            var longitudMaxima = Math.Max(s1.Length, s2.Length);
            var distancia = CalcularDistanciaLevenshtein(s1, s2);

            return 1.0 - ((double)distancia / longitudMaxima);
        }

        /// <summary>
        /// Calcula distancia de Levenshtein entre dos strings
        /// </summary>
        private int CalcularDistanciaLevenshtein(string s1, string s2)
        {
            var matriz = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 0; i <= s1.Length; i++)
                matriz[i, 0] = i;

            for (int j = 0; j <= s2.Length; j++)
                matriz[0, j] = j;

            for (int i = 1; i <= s1.Length; i++)
            {
                for (int j = 1; j <= s2.Length; j++)
                {
                    var costo = (s1[i - 1] == s2[j - 1]) ? 0 : 1;
                    matriz[i, j] = Math.Min(
                        Math.Min(matriz[i - 1, j] + 1, matriz[i, j - 1] + 1),
                        matriz[i - 1, j - 1] + costo
                    );
                }
            }

            return matriz[s1.Length, s2.Length];
        }

        /// <summary>
        /// Obtiene todas las páginas Blazor registradas
        /// </summary>
        public List<string> ObtenerRutasDisponibles()
        {
            return [.. _rutasBlazor.Keys.OrderBy(k => k)];
        }
    }

    /// <summary>
    /// Resultado de la validación de un menú hijo
    /// </summary>
    public class ResultadoValidacionSubmenu
    {
        public int CodigoMenu { get; set; }
        public int? CodigoMenuPadre { get; set; }
        public string NombreMenu { get; set; } = string.Empty;
        public string UrlOriginal { get; set; } = string.Empty;
        public string UrlNormalizada { get; set; } = string.Empty;
        public bool Existe { get; set; }
        public string ComponenteBlazor { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public List<string> UrlsSimilares { get; set; } = new();
    }
}