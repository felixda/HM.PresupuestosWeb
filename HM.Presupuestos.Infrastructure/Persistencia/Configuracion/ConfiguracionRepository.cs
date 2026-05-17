using HM.Core.Comun.v6.Seguridad.Interfaces;
using HM.Core.Servidor.v6.DAL.Interfaces;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Puertos;

namespace HM.Presupuestos.Infrastructure.Persistencia
{
    public class ConfiguracionRepository(
        IJwt jwt, 
        IDataAccessHelperSecure dah) : BasePresupuestosRepository(dah, jwt), IConfiguracionRepository
    {
        protected readonly IJwt jwt = jwt;
        protected new readonly IDataAccessHelperSecure dah = dah;

        public async Task<CodigoDescripcion> ObtenerAnioDiario()
        {
            var resultado = new CodigoDescripcion();

            var query = @"
                SELECT  VALOR
                FROM PPT_CONFIGURACION
                WHERE COD_CONFIGURACION = 1";

            dah.GetSqlStringComando(query);

            await AñadirParametroMulticompania(dah);

            await Task.Run(() =>
            {
                dah.ProcesarDatos(dr =>
                {
                    while (dr.Read())
                    {
                        var item = dr.GetString("VALOR");
                        resultado.Codigo = int.Parse(item);
                        resultado.Descripcion = resultado.Codigo.ToString();
                    }
                });
            });

            return resultado;
        }


        public async Task ActualizarAnioDiario(int anio)
        {
            var query = @"
                UPDATE PPT_CONFIGURACION
                    SET VALOR = :Valor
                WHERE COD_CONFIGURACION = 1";

            dah.GetSqlStringComando(query);

            dah.AddParameter("Valor", anio);

            await Task.Run(() => dah.ExecuteNonQuery());
        }
    }
}
