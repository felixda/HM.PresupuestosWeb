using HM.Core.Comun.v6.Seguridad.Interfaces;
using HM.Core.Servidor.v6.DAL;
using HM.Core.Servidor.v6.DAL.Interfaces;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Puertos;

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

    }
}
