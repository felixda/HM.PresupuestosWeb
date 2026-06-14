using HM.Presupuestos.Infrastructure.Servicios;

using HM.Core.Comun.v6.Seguridad.Interfaces;
using HM.Core.Servidor.v6.DAL.Interfaces;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Puertos;
using System.Data;

namespace HM.Presupuestos.Infrastructure.Persistencia
{

    public class SobreprimasRepository(
        IJwt jwt, 
        IDataAccessHelperSecure dah) : BasePresupuestosRepository(dah, jwt), ISobreprimasRepository
    {
        protected readonly IJwt jwt = jwt;
        protected new readonly IDataAccessHelperSecure dah = dah;

        public async Task<List<Sobreprima>> ObtenerSobreprimas( SobreprimaFiltro filterSobreprima)
        {
            List<Sobreprima> list = [];
            try
            {
                string query = $@"
                    SELECT PSM.COD_SOBREPRIMA_MEDIO, PSM.COD_VERSION, PSM.COD_CONCEPTO_SOBREPRIMA, PSM.COD_NETWORK,
                           PSM.COD_PAIS, PSM.COD_MEDIO, PSM.COD_EDITORIAL_COMERCIAL,
                           VAC.COD_AGRUPACION_COMERCIAL, PSM.PCT_SOBREPRIMA
                    FROM PPT_SOBREPRIMAS_MEDIO PSM
                    LEFT JOIN V_AGRUPACION_COMERCIAL VAC ON PSM.COD_EDITORIAL_COMERCIAL = VAC.COD_EDITORIAL_COMERCIAL AND PSM.COD_PAIS = VAC.COD_PAIS
                    WHERE PSM.COD_VERSION = :CodigoVersion
                    {(!string.IsNullOrEmpty(filterSobreprima.CodigoNetworkList)           ? $"AND PSM.COD_NETWORK IN ({filterSobreprima.CodigoNetworkList})"                        : "")}
                    {(!string.IsNullOrEmpty(filterSobreprima.CodigoMedioList)             ? $"AND PSM.COD_MEDIO IN ({filterSobreprima.CodigoMedioList})"                          : "")}
                    {(!string.IsNullOrEmpty(filterSobreprima.CodigoAgrupacionComercialList) ? $"AND VAC.COD_AGRUPACION_COMERCIAL IN ({filterSobreprima.CodigoAgrupacionComercialList})" : "")}
                    {(!string.IsNullOrEmpty(filterSobreprima.CodigoEditorialList)         ? $"AND PSM.COD_EDITORIAL_COMERCIAL IN ({filterSobreprima.CodigoEditorialList})"         : "")}";

                dah.GetSqlStringComando(query);

                dah.AddParameter("CodigoVersion", filterSobreprima.CodigoVersion);

                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        while (dr.Read())
                        {
                            Sobreprima item = new();

                            item.Codigo = dr.GetInt32("COD_SOBREPRIMA_MEDIO"); ;
                            item.CodigoVersion = dr.GetInt32("COD_VERSION");
                            item.CodigoConcepto = dr.GetInt32("COD_CONCEPTO_SOBREPRIMA");
                            item.CodigoNetwork = dr.GetInt32("COD_NETWORK");
                            item.CodigoMedio = dr.GetInt32("COD_MEDIO");
                            item.CodigoAgrupacionComercial = dr.GetNullableInt32("COD_AGRUPACION_COMERCIAL");
                            item.CodigoEditorial = dr.GetInt32("COD_EDITORIAL_COMERCIAL");
                            item.Porcentaje = dr.GetDecimal("PCT_SOBREPRIMA");
                            list.Add(item);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("SobreprimasRepository.GetSobreprimaList", ex);
            }

            return list;
        }

        /// <summary>
        /// Insert Sobreprima data
        /// </summary>
        /// <param name="codigoUsuario">User code</param> 
        /// <param name="item">Sobreprima object</param> 
        public async Task InsertSobreprima( Sobreprima item)
        {
            try
            {
                const string query = @"
                    INSERT INTO PPT_SOBREPRIMAS_MEDIO (
                        COD_VERSION, COD_CONCEPTO_SOBREPRIMA, COD_NETWORK, COD_PAIS, COD_MEDIO,
                        COD_EDITORIAL_COMERCIAL, PCT_SOBREPRIMA, F_ALTA, F_MODIFICACION,
                        COD_USUARIO_ALTA, COD_USUARIO_MODIFICACION)
                    VALUES (
                        :CodigoVersion, :CodigoConcepto, :CodigoNetwork, :CodigoPais, :CodigoMedio,
                        :CodigoEditorial, :Porcentaje, :Fecha, :Fecha,
                        :CodigoUsuario, :CodigoUsuario)
                    RETURNING COD_SOBREPRIMA_MEDIO INTO :Codigo";

                dah.GetSqlStringComando(query);

                dah.AddParameter("CodigoVersion", item.CodigoVersion);
                dah.AddParameter("CodigoConcepto", item.CodigoConcepto);
                dah.AddParameter("CodigoNetwork", item.CodigoNetwork);
                dah.AddParameter("CodigoPais", item.CodigoPais);
                dah.AddParameter("CodigoMedio", item.CodigoMedio);
                dah.AddParameter("CodigoEditorial", item.CodigoEditorial);
                dah.AddParameter("Porcentaje", item.Porcentaje);
                dah.AddParameter("Fecha", DateTime.Now);
                dah.AddParameter("CodigoUsuario", CodigoUsuario);

                dah.AddParameter("Codigo", item.Codigo, DbType.Int32, ParameterDirection.Output, 10);

                await Task.Run(() => dah.ExecuteNonQuery());

                item.Codigo = Convert.ToInt32(dah.Comando.Parameters["Codigo"].Value);
            }
            catch (Exception ex)
            {
                throw new Exception("SobreprimasRepository.InsertSobreprima", ex);
            }
        }


        public async Task EliminarSobreprima(int codigoSobreprima)
        {
            try
            {
                const string query = @"DELETE FROM PPT_SOBREPRIMAS_MEDIO
                             WHERE COD_SOBREPRIMA_MEDIO = :Codigo";

                dah.GetSqlStringComando(query);
                dah.AddParameter("Codigo", codigoSobreprima);

                await Task.Run(() => dah.ExecuteNonQuery());
            }
            catch (Exception ex)
            {
                throw new Exception("SobreprimasRepository.EliminarSobreprima", ex);
            }
        }

        public async Task ActualizarSobreprima(Sobreprima item)
        {
            try
            {
                const string query = @"
                UPDATE PPT_SOBREPRIMAS_MEDIO 
                SET COD_VERSION = :CodigoVersion,
                    COD_CONCEPTO_SOBREPRIMA = :CodigoConcepto, 
                    COD_NETWORK = :CodigoNetwork, 
                    COD_PAIS = :CodigoPais, 
                    COD_MEDIO = :CodigoMedio,  
                    COD_EDITORIAL_COMERCIAL = :CodigoEditorial,  
                    PCT_SOBREPRIMA = :Porcentaje,  
                    F_MODIFICACION = :Fecha,  
                    COD_USUARIO_MODIFICACION = :CodigoUsuario 
                WHERE COD_SOBREPRIMA_MEDIO = :Codigo";

                dah.GetSqlStringComando(query);

                dah.AddParameter("CodigoVersion", item.CodigoVersion);
                dah.AddParameter("CodigoConcepto", item.CodigoConcepto);
                dah.AddParameter("CodigoNetwork", item.CodigoNetwork);
                dah.AddParameter("CodigoPais", item.CodigoPais);
                dah.AddParameter("CodigoMedio", item.CodigoMedio);
                dah.AddParameter("CodigoEditorial", item.CodigoEditorial);
                dah.AddParameter("Porcentaje", item.Porcentaje);
                dah.AddParameter("Fecha", DateTime.Now);
                dah.AddParameter("CodigoUsuario", CodigoUsuario);
                dah.AddParameter("Codigo", item.Codigo);

                int filasActualizadas = await Task.Run(() => dah.ExecuteNonQuery());
            }
            catch (Exception ex)
            {
                throw new Exception("SobreprimasRepository.ActualizarSobreprima", ex);
            }
        }


        public async Task<bool> ExistenSobreprimas(SobreprimaFiltro filterSobreprima, string? codigosSobreprima = null)
        {
            bool result= false;
            try
            {
                string query = $@"
                    SELECT COUNT(*)
                    FROM PPT_SOBREPRIMAS_MEDIO
                    WHERE COD_VERSION = :CodigoVersion
                    {(!string.IsNullOrEmpty(codigosSobreprima)                              ? $"AND COD_SOBREPRIMA_MEDIO NOT IN ({codigosSobreprima})"                         : "")}
                    {(!string.IsNullOrEmpty(filterSobreprima.CodigoNetworkList)            ? $"AND COD_NETWORK IN ({filterSobreprima.CodigoNetworkList})"                     : "")}
                    {(!string.IsNullOrEmpty(filterSobreprima.CodigoMedioList)              ? $"AND COD_MEDIO IN ({filterSobreprima.CodigoMedioList})"                         : "")}
                    {(!string.IsNullOrEmpty(filterSobreprima.CodigoEditorialList)          ? $"AND COD_EDITORIAL_COMERCIAL IN ({filterSobreprima.CodigoEditorialList})"       : "")}";

                dah.GetSqlStringComando(query);

                dah.AddParameter("CodigoVersion", filterSobreprima.CodigoVersion);

                await AñadirParametroMulticompania(dah);

                int cuantos = await Task.Run(() => dah.ExecuteScalar<int>());
                result = (cuantos > 0);
            }
            catch (Exception ex)
            {
                throw new Exception("SobreprimasRepository.ExistenSobreprimas", ex);
            }
            return result;
        }

            public ITransaccion ObtenerTransaccion() => new TransaccionWrapper(base.ObtenerTransaccion());

        }
    }

