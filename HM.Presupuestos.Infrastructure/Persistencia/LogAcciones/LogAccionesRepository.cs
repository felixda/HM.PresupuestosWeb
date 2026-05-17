using HM.Core.Comun.v6.Seguridad.Interfaces;
using HM.Core.Servidor.v6.DAL;
using HM.Core.Servidor.v6.DAL.Interfaces;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Puertos;
using System.Text;

namespace HM.Presupuestos.Infrastructure.Persistencia
{
    public class LogAccionesRepository(
        IJwt jwt,
        IDataAccessHelperSecure dah) : BasePresupuestosRepository(dah, jwt), ILogAccionesRepository
    {
        public async Task Insertar(LogAccion logAccion)
        {
            StringBuilder query = new();
            query.Append("INSERT INTO PPT_ACCION_LOG (COD_USUARIO, ");
            query.Append("                   DES_PROCESO, ");
            query.Append("                   FECHA_INICIO, ");
            query.Append("                   PARAMETROS) ");
            query.Append("     VALUES ( :CodigoUsuario, ");
            query.Append("             :DesProceso, ");
            query.Append("             :Fecha, ");
            query.Append("             :Parametros ) ");

            dah.GetSqlStringComando(query.ToString());

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
