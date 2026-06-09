using System.Text.Json.Nodes;

namespace HM.Presupuestos.Web.Adaptadores.Navegacion
{
    /// <summary>
    /// Clase para mapear cÃ³digos de menÃº a URLs, etiquetas, iconos y visibilidad.
    /// </summary>
    public interface IMapaMenu
    {
        string? ObtenerUrlMenu(int codigoMenu);
        string? ObtenerUrlMenu( CodigosMenu codigoMenu) => ObtenerUrlMenu((int)codigoMenu);
        int ObtenerCodigoMenuPorUrl(string url);
        string ObtenerEtiquetaMenuPorUrl(string url);
        string ObtenerEtiquetaMenu(CodigosMenu codigoMenu);
        string ObtenerIconoMenu(CodigosMenu codigoMenu);
        string ObtenerVisibilidadMenu(CodigosMenu codigoMenu);
        List<CodigoDescripcion> ObtenerPaginasNavegables();
        List<CodigoDescripcion> ObtenerAccionesLog();
    }

    public class MapaMenu : IMapaMenu
    {
        private readonly IProveedorRecursosJson _proveedorJson;
        private readonly IGestorIdioma _gestorIdiomas;
        private readonly ILocalizadorRecursos _localizadorRecursos;
        private readonly string _idiomaPorDefecto;

        public MapaMenu(IProveedorRecursosJson proveedorJson, IGestorIdioma gestorIdiomas,
            ILocalizadorRecursos localizadorRecursos, IConfiguration configuracion)
        {
            _proveedorJson = proveedorJson;
            _gestorIdiomas = gestorIdiomas;
            _localizadorRecursos = localizadorRecursos;
            _idiomaPorDefecto = configuracion.GetValue<string>("AppSettings:DefaultLanguage") ?? "es";
        }

        private string IdiomaActual =>
            (_gestorIdiomas.IdiomaActual ?? _idiomaPorDefecto)
            .Trim('"', '\\', ' ').Split('-')[0].ToLower();

        public string? ObtenerUrlMenu(int codigoMenu)
        {
            try
            {
                var doc = _proveedorJson.ObtenerDocumento(IdiomaActual);
                if (doc == null) return null;

                var root = doc.RootElement;
                if (!root.TryGetProperty("Menu", out var menuSection)) return null;

                foreach (var item in menuSection.EnumerateObject())
                {
                    var menuItem = item.Value;
                    if (menuItem.TryGetProperty("code", out var codeProp) &&
                        codeProp.GetInt32() == codigoMenu &&
                        menuItem.TryGetProperty("url", out var urlProp))
                    {
                        return urlProp.GetString();
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MapaMenu] âŒ Error en ObtenerUrlMenu({codigoMenu}): {ex.Message}");
                return null;
            }
        }

        public int ObtenerCodigoMenuPorUrl(string url)
        {
            var result = -1;
            try
            {
                var jsonMenu = _localizadorRecursos.ObtenerTexto("menu");
                if (string.IsNullOrEmpty(jsonMenu)) return result;

                JsonNode? jsonNode = JsonNode.Parse(jsonMenu);
                if (jsonNode is not JsonObject jsonObject) return result;

                foreach (var property in jsonObject.AsObject())
                {
                    if (property.Value is not JsonObject element) continue;

                    var urlElement = element["url"]?.ToString();
                    if (string.IsNullOrEmpty(urlElement)) continue;

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
                Console.WriteLine($"[MapaMenu] âŒ Error en ObtenerCodigoMenuPorUrl: {ex.Message}");
            }

            return result;
        }

        public string ObtenerEtiquetaMenuPorUrl(string url)
        {
            int codigoMenu = ObtenerCodigoMenuPorUrl(url);
            if (codigoMenu > 0)
                return _localizadorRecursos.ObtenerTexto($"Menu:Menu_{codigoMenu}:label");

            return url;
        }

        public string ObtenerEtiquetaMenu(CodigosMenu codigoMenu)
        {
            return _localizadorRecursos.ObtenerTexto($"Menu:Menu_{(int)codigoMenu}:label");
        }

        public string ObtenerIconoMenu(CodigosMenu codigoMenu)
        {
            return _localizadorRecursos.ObtenerTexto($"Menu:Menu_{(int)codigoMenu}:icono");
        }

        public string ObtenerVisibilidadMenu(CodigosMenu codigoMenu)
        {
            return _localizadorRecursos.ObtenerTexto($"Menu:Menu_{(int)codigoMenu}:visible");
        }

        public List<CodigoDescripcion> ObtenerPaginasNavegables()
        {
            var resultado = new List<CodigoDescripcion>();
            try
            {
                var doc = _proveedorJson.ObtenerDocumento(IdiomaActual);
                if (doc == null) return resultado;

                var root = doc.RootElement;
                if (!root.TryGetProperty("Menu", out var menuSection)) return resultado;

                foreach (var item in menuSection.EnumerateObject())
                {
                    var menuItem = item.Value;
                    if (!menuItem.TryGetProperty("url", out var urlProp)) continue;
                    var url = urlProp.GetString();
                    if (string.IsNullOrEmpty(url)) continue;
                    if (!menuItem.TryGetProperty("code", out var codeProp)) continue;
                    if (!menuItem.TryGetProperty("label", out var labelProp)) continue;

                    resultado.Add(new CodigoDescripcion
                    {
                        Codigo = codeProp.GetInt32(),
                        Descripcion = labelProp.GetString() ?? string.Empty
                    });
                }

                resultado = [.. resultado.OrderBy(x => x.Descripcion)];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MapaMenu] Error en ObtenerPaginasNavegables: {ex.Message}");
            }
            return resultado;
        }

        public List<CodigoDescripcion> ObtenerAccionesLog()
        {
            var resultado = new List<CodigoDescripcion>();
            try
            {
                var doc = _proveedorJson.ObtenerDocumento(IdiomaActual);
                if (doc == null) return resultado;

                var root = doc.RootElement;
                if (!root.TryGetProperty("AccionesLog", out var accionesSection)) return resultado;

                foreach (var item in accionesSection.EnumerateObject())
                {
                    var entry = item.Value;
                    if (!entry.TryGetProperty("visible", out var visibleProp) || !visibleProp.GetBoolean()) continue;
                    if (!entry.TryGetProperty("label", out var labelProp)) continue;
                    if (!Enum.TryParse<AccionesLog>(item.Name, out var accion)) continue;

                    resultado.Add(new CodigoDescripcion
                    {
                        Codigo = (int)accion,
                        Descripcion = labelProp.GetString() ?? string.Empty
                    });
                }

                resultado = [.. resultado.OrderBy(x => x.Descripcion)];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MapaMenu] Error en ObtenerAccionesLog: {ex.Message}");
            }
            return resultado;
        }
    }
}




