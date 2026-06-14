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
                            Usuario = ExtraerUsuario(dr.GetNullableString("PARAMETROS") ?? string.Empty),
                            Parametros = dr.GetNullableString("PARAMETROS") ?? string.Empty
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

        public async Task<EstadisticasAuditoria> ObtenerEstadisticas(AccionesLog tipo, DateTime fechaInicio, DateTime fechaFin, int? codigoPagina = null)
        {
            var estadisticas = new EstadisticasAuditoria();
            var fechaFinExclusiva = fechaFin.Date.AddDays(1);
            var patron = $"[{(int)tipo}]%";

            // Query 1: top usuarios + métricas generales
            const string queryTopUsuarios = @"
                SELECT COD_USUARIO,
                       JSON_VALUE(PARAMETROS, '$.Login') AS LOGIN,
                       COUNT(*) AS TOTAL
                  FROM PPT_ACCION_LOG
                 WHERE DES_PROCESO LIKE :Patron
                   AND FECHA_INICIO >= :FechaInicio
                   AND FECHA_INICIO < :FechaFin
                 GROUP BY COD_USUARIO, JSON_VALUE(PARAMETROS, '$.Login')
                 ORDER BY TOTAL DESC
                 FETCH FIRST 5 ROWS ONLY";

            dah.GetSqlStringComando(queryTopUsuarios);
            dah.AddParameter("Patron", patron);
            dah.AddParameter("FechaInicio", fechaInicio.Date);
            dah.AddParameter("FechaFin", fechaFinExclusiva);

            await Task.Run(() =>
            {
                dah.ProcesarDatos(dr =>
                {
                    while (dr.Read())
                    {
                        var loginRaw = dr.GetNullableString("LOGIN");
                        var codUsuario = dr.GetInt32("COD_USUARIO").ToString();
                        var total = dr.GetInt32("TOTAL");
                        var login = string.IsNullOrWhiteSpace(loginRaw) ? codUsuario : loginRaw;

                        estadisticas.TopUsuarios.Add(new UsuarioContador { Login = login, Total = total });
                        estadisticas.TotalAcciones += total;
                        estadisticas.UsuariosUnicos++;
                    }
                });
            });

            if (estadisticas.TopUsuarios.Count > 0)
            {
                estadisticas.UsuarioMasActivo = estadisticas.TopUsuarios[0].Login;
                estadisticas.UsuarioMasActivoTotal = estadisticas.TopUsuarios[0].Total;
            }

            // Query 2: actividad por día
            const string queryActividad = @"
                SELECT TRUNC(FECHA_INICIO) AS DIA, COUNT(*) AS TOTAL
                  FROM PPT_ACCION_LOG
                 WHERE DES_PROCESO LIKE :Patron
                   AND FECHA_INICIO >= :FechaInicio
                   AND FECHA_INICIO < :FechaFin
                 GROUP BY TRUNC(FECHA_INICIO)
                 ORDER BY DIA";

            dah.GetSqlStringComando(queryActividad);
            dah.AddParameter("Patron", patron);
            dah.AddParameter("FechaInicio", fechaInicio.Date);
            dah.AddParameter("FechaFin", fechaFinExclusiva);

            await Task.Run(() =>
            {
                dah.ProcesarDatos(dr =>
                {
                    while (dr.Read())
                    {
                        estadisticas.ActividadPorDia.Add(new PuntoTemporal
                        {
                            Fecha = dr.GetDateTime("DIA"),
                            Total = dr.GetInt32("TOTAL")
                        });
                    }
                });
            });

            // Query 3: página más visitada (solo si AccesoAPagina)
            if (tipo == AccionesLog.AccesoAPagina)
            {
                string queryPagina = $@"
                    SELECT DES_PROCESO, COUNT(*) AS TOTAL
                      FROM PPT_ACCION_LOG
                     WHERE DES_PROCESO LIKE :Patron
                       AND FECHA_INICIO >= :FechaInicio
                       AND FECHA_INICIO < :FechaFin
                       {(codigoPagina.HasValue ? "AND DES_PROCESO LIKE :PatronPagina" : "")}
                     GROUP BY DES_PROCESO
                     ORDER BY TOTAL DESC
                     FETCH FIRST 1 ROW ONLY";

                dah.GetSqlStringComando(queryPagina);
                dah.AddParameter("Patron", patron);
                dah.AddParameter("FechaInicio", fechaInicio.Date);
                dah.AddParameter("FechaFin", fechaFinExclusiva);
                if (codigoPagina.HasValue)
                    dah.AddParameter("PatronPagina", $"%[{codigoPagina.Value}]");

                await Task.Run(() =>
                {
                    dah.ProcesarDatos(dr =>
                    {
                        if (dr.Read())
                        {
                            estadisticas.PaginaMasVisitada = QuitarPrefijoAccion(dr.GetString("DES_PROCESO"));
                            estadisticas.PaginaMasVisitadaTotal = dr.GetInt32("TOTAL");
                        }
                    });
                });
            }

            return estadisticas;
        }
    }
}
