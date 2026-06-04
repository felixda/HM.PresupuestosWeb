using HM.Presupuestos.Infrastructure.Servicios;
using HM.Core.Comun.v6.Seguridad.Interfaces;
using HM.Core.Servidor.v6.DAL.Interfaces;
using HM.Presupuestos.Domain.Puertos;

namespace HM.Presupuestos.Infrastructure.Persistencia
{
    public class AdminRepository(
        IJwt jwt, 
        IDataAccessHelperSecure dah) : BasePresupuestosRepository(dah,  jwt), IAdminRepository
    {
        protected readonly IJwt jwt = jwt;
        protected new readonly IDataAccessHelperSecure dah = dah; 

        public async Task<List<int>> ObtenerMesesBloqueados(int anio)
        {
            var resultado = new List<int>();

            var query = @"
                SELECT  MES
                FROM PPT_MESES_CERRADOS
                WHERE ANIO = :anio
                ORDER BY MES";

            dah.GetSqlStringComando(query);

            dah.AddParameter("anio", anio);
            await AñadirParametroMulticompania(dah);

            await Task.Run(() =>
            {
                dah.ProcesarDatos(dr =>
                {
                    while (dr.Read())
                    {

                        int item = dr.GetInt32("MES");
                        resultado.Add(item);
                    }
                });
            });

            return resultado;
        }

        public async Task InsertarMesBloqueado(int anio, int mes)
        {
            var query = @"
                INSERT INTO PPT_MESES_CERRADOS(
                    ANIO,
                    MES
                )
                VALUES (
                    :Anio,
                    :Mes
                )";
               

            dah.GetSqlStringComando(query);

            dah.AddParameter("Anio", anio);
            dah.AddParameter("Mes", mes);

            await Task.Run(() => dah.ExecuteNonQuery());

        }


        public async Task EliminarMesesBloqueados(int anio)
        {
            var query = @"
                DELETE FROM PPT_MESES_CERRADOS
                WHERE ANIO = :Anio";

            dah.GetSqlStringComando(query);

            dah.AddParameter("Anio", anio);

            await Task.Run(() => dah.ExecuteNonQuery());
          }

          public ITransaccion ObtenerTransaccion() => new TransaccionWrapper(base.ObtenerTransaccion());
      }
  }

