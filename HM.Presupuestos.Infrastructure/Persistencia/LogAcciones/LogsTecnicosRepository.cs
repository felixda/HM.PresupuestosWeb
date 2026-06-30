using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Entidades.LogAcciones;
using HM.Presupuestos.Domain.Puertos;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace HM.Presupuestos.Infrastructure.Persistencia
{
    public class LogsTecnicosRepository : ILogsTecnicosRepository
    {
        private static readonly string[] NivelesSoportados = ["Trace", "Debug", "Info", "Warn", "Error", "Fatal"];
        private static readonly Regex CampoRegex = new("\"(?<key>[^\"]+)\"\\s*:\\s*(?<value>\"(?:\\\\.|[^\"])*\"|[^,{}]+)", RegexOptions.Compiled);

        public async Task<List<LogTecnico>> ObtenerLogs(FiltroLogsTecnicos filtro)
        {
            var resultado = new List<LogTecnico>();

            foreach (var archivo in ObtenerArchivosPorRango(filtro.FechaDesde, filtro.FechaHasta))
            {
                using var stream = new FileStream(archivo, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(stream);

                while (!reader.EndOfStream)
                {
                    var linea = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(linea))
                        continue;

                    if (!TryParseLog(linea, out var log) || log == null)
                        continue;

                    if (!CumpleFiltro(log, filtro))
                        continue;

                    resultado.Add(log);
                }
            }

            return resultado.OrderByDescending(x => x.Fecha).ToList();
        }

        public Task<List<CodigoDescripcion>> ObtenerNivelesDisponibles()
        {
            var niveles = NivelesSoportados
                .Select((nivel, indice) => new CodigoDescripcion
                {
                    Codigo = indice + 1,
                    Descripcion = nivel
                })
                .ToList();

            return Task.FromResult(niveles);
        }

        private static IEnumerable<string> ObtenerArchivosPorRango(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var directorio = Path.Combine(AppContext.BaseDirectory, "logs");
            if (!Directory.Exists(directorio))
                return [];

            var desde = (fechaDesde ?? DateTime.Today).Date;
            var hasta = (fechaHasta ?? fechaDesde ?? DateTime.Today).Date;
            if (hasta < desde)
                return [];

            var archivos = new List<string>();
            for (var fecha = desde; fecha <= hasta; fecha = fecha.AddDays(1))
            {
                var ruta = Path.Combine(directorio, $"Presupuestos_{fecha:yyyy-MM-dd}.jsonl");
                if (File.Exists(ruta))
                    archivos.Add(ruta);
            }

            return archivos;
        }

        private static bool TryParseLog(string linea, out LogTecnico? log)
        {
            log = null;

            try
            {
                using var document = JsonDocument.Parse(linea);
                var root = document.RootElement;

                log = new LogTecnico
                {
                    Fecha = ObtenerFecha(root),
                    Nivel = ObtenerString(root, "level"),
                    Categoria = ObtenerString(root, "category"),
                    Usuario = NormalizarUsuario(ObtenerString(root, "login")),
                    Mensaje = ObtenerString(root, "message"),
                    Logger = ObtenerString(root, "logger"),
                    Excepcion = ObtenerString(root, "exception"),
                    StackTrace = ObtenerString(root, "stackTrace"),
                    Comentarios = ObtenerString(root, "comments")
                };

                return true;
            }
            catch
            {
                return TryParsePseudoJson(linea, out log);
            }
        }

        private static bool TryParsePseudoJson(string linea, out LogTecnico? log)
        {
            log = null;

            var campos = CampoRegex.Matches(linea)
                .ToDictionary(
                    x => x.Groups["key"].Value,
                    x => LimpiarValor(x.Groups["value"].Value),
                    StringComparer.OrdinalIgnoreCase);

            if (campos.Count == 0)
                return false;

            log = new LogTecnico
            {
                Fecha = ObtenerFecha(campos),
                Nivel = ObtenerValor(campos, "level"),
                Categoria = ObtenerValor(campos, "category"),
                Usuario = NormalizarUsuario(ObtenerValor(campos, "login")),
                Mensaje = ObtenerValor(campos, "message"),
                Logger = ObtenerValor(campos, "logger"),
                Excepcion = ObtenerValor(campos, "exception"),
                StackTrace = ObtenerValor(campos, "stackTrace"),
                Comentarios = ObtenerValor(campos, "comments")
            };

            return true;
        }

        private static bool CumpleFiltro(LogTecnico log, FiltroLogsTecnicos filtro)
        {
            if (filtro.FechaDesde.HasValue && log.Fecha < filtro.FechaDesde.Value)
                return false;

            if (filtro.FechaHasta.HasValue && log.Fecha > filtro.FechaHasta.Value)
                return false;

            if (!string.IsNullOrWhiteSpace(filtro.Nivel) && !string.Equals(log.Nivel, filtro.Nivel, StringComparison.OrdinalIgnoreCase))
                return false;

            if (!string.IsNullOrWhiteSpace(filtro.Usuario))
            {
                if (string.Equals(filtro.Usuario, LogsTecnicosConstantes.UsuarioSinUsuario, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.Equals(log.Usuario, LogsTecnicosConstantes.UsuarioSinUsuario, StringComparison.OrdinalIgnoreCase))
                        return false;
                }
                else if (!string.Equals(log.Usuario, filtro.Usuario, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(filtro.Categoria) && !string.Equals(log.Categoria, filtro.Categoria, StringComparison.OrdinalIgnoreCase))
                return false;

            if (!string.IsNullOrWhiteSpace(filtro.Mensaje)
                && (string.IsNullOrWhiteSpace(log.Mensaje)
                    || log.Mensaje.IndexOf(filtro.Mensaje, StringComparison.OrdinalIgnoreCase) < 0))
            {
                return false;
            }

            return true;
        }

        private static string ObtenerString(JsonElement root, string propiedad)
        {
            if (!root.TryGetProperty(propiedad, out var valor))
                return string.Empty;

            return valor.ValueKind switch
            {
                JsonValueKind.String => valor.GetString() ?? string.Empty,
                JsonValueKind.Null => string.Empty,
                _ => valor.ToString()
            };
        }

        private static DateTime ObtenerFecha(JsonElement root)
        {
            var valor = ObtenerString(root, "timestamp");
            return DateTime.TryParse(valor, out var fecha) ? fecha : DateTime.MinValue;
        }

        private static DateTime ObtenerFecha(IDictionary<string, string> campos)
        {
            var valor = ObtenerValor(campos, "timestamp");

            if (DateTime.TryParse(valor, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var fechaConZona))
                return fechaConZona;

            return DateTime.TryParse(valor, out var fecha) ? fecha : DateTime.MinValue;
        }

        private static string ObtenerValor(IDictionary<string, string> campos, string key)
        {
            return campos.TryGetValue(key, out var valor) ? valor : string.Empty;
        }

        private static string LimpiarValor(string valor)
        {
            var texto = valor.Trim();
            if (texto.Length >= 2 && texto[0] == '"' && texto[^1] == '"')
            {
                texto = texto[1..^1]
                    .Replace("\\\"", "\"")
                    .Replace("\\\\", "\\");
            }

            return texto;
        }

        private static string NormalizarUsuario(string? usuario)
        {
            return string.IsNullOrWhiteSpace(usuario)
                ? LogsTecnicosConstantes.UsuarioSinUsuario
                : usuario.Trim();
        }
    }
}