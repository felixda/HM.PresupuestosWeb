using HM.Core.Comun.v6.Seguridad.Interfaces;
using HM.Core.Servidor.v6.DAL.Interfaces;
using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Entidades.LogAcciones;
using HM.Presupuestos.Domain.Puertos;
using System.Text.Json;

namespace HM.Presupuestos.Infrastructure.Persistencia
{
    public class LogAccionesRepository(
        IJwt jwt,
        IDataAccessHelperSecure dah) : BasePresupuestosRepository(dah, jwt), ILogAccionesRepository
    {
        public async Task Insertar(LogAccion logAccion)
        {
            const string query = @"
                INSERT INTO PPT_ACCION_LOG (COD_USUARIO, DES_PROCESO, FECHA_INICIO, PARAMETROS)
                VALUES (:CodigoUsuario, :DesProceso, :Fecha, :Parametros)";

            dah.GetSqlStringComando(query);

            dah.AddParameter("CodigoUsuario", CodigoUsuario);
            dah.AddParameter("DesProceso", logAccion.Accion);
            dah.AddParameter("Fecha", DateTime.Now);
            if (string.IsNullOrEmpty(logAccion.Parametros))
            {
                dah.AddParameter("Parametros", "-");
            }
            else
            {
                dah.AddParameter("Parametros", logAccion.Parametros);
            }

            await Task.Run(() => dah.ExecuteNonQuery());
        }

        public async Task<List<Auditoria>> ObtenerAuditorias(AccionesLog tipo, DateTime? fechaInicio, DateTime? fechaFin, int? codigoPagina = null)
        {
            List<Auditoria> resultado = [];

            string query = $@"
                SELECT DES_PROCESO, FECHA_INICIO, PARAMETROS
                  FROM PPT_ACCION_LOG
                 WHERE DES_PROCESO LIKE :Patron
                       {(fechaInicio.HasValue ? "AND FECHA_INICIO >= :FechaInicio" : "")}
                       {(fechaFin.HasValue ? "AND FECHA_INICIO <= :FechaFin" : "")}
                       {(codigoPagina.HasValue ? "AND DES_PROCESO LIKE :PatronPagina" : "")}
                 ORDER BY FECHA_INICIO DESC";

            dah.GetSqlStringComando(query);

            dah.AddParameter("Patron", $"[{(int)tipo}]%");

            if (fechaInicio.HasValue)
                dah.AddParameter("FechaInicio", fechaInicio.Value);

            if (fechaFin.HasValue)
                dah.AddParameter("FechaFin", fechaFin.Value);

            if (codigoPagina.HasValue)
                dah.AddParameter("PatronPagina", $"%[{codigoPagina.Value}]");

            await Task.Run(() =>
            {
                dah.ProcesarDatos(dr =>
                {
                    while (dr.Read())
                    {
                        resultado.Add(new Auditoria
                        {
                            Descripcion = QuitarPrefijoAccion(dr.GetString("DES_PROCESO")),
                            FechaInicio = dr.GetDateTime("FECHA_INICIO"),
                            Usuario = ExtraerUsuario(dr.GetNullableString("PARAMETROS") ?? string.Empty)
                        });
                    }
                });
            });

            return resultado;
        }

        private static string QuitarPrefijoAccion(string descripcion)
        {
            int separador = descripcion.IndexOf("-> ", StringComparison.Ordinal);
            if (separador >= 0)
                return descripcion[(separador + 3)..].TrimStart();

            int corchete = descripcion.IndexOf(']');
            return corchete >= 0 ? descripcion[(corchete + 1)..].TrimStart() : descripcion;
        }

        private static string ExtraerUsuario(string parametrosJson)
        {
            if (string.IsNullOrWhiteSpace(parametrosJson) || parametrosJson == "-")
                return "Sin Usuario especificado";

            try
            {
                var json = JsonDocument.Parse(parametrosJson);
                string nombre = json.RootElement.TryGetProperty("Nombre", out var n) ? n.GetString() ?? "" : "";
                string apellido1 = json.RootElement.TryGetProperty("Apellido1", out var a) ? a.GetString() ?? "" : "";
                string nombreCompleto = $"{nombre} {apellido1}".Trim();
                return string.IsNullOrEmpty(nombreCompleto) ? "Sin Usuario especificado" : nombreCompleto;
            }
            catch
            {
                return "Sin Usuario especificado";
            }
        }
    }
}
