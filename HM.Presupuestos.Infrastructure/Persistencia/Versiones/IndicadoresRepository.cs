using HM.Presupuestos.Infrastructure.Servicios;
using HM.Core.Comun.v6.Seguridad.Interfaces;
using HM.Core.Servidor.v6.DAL.Interfaces;
using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Puertos;
using System.Data;

namespace HM.Presupuestos.Infrastructure.Persistencia
{
    public class IndicadoresRepository(
        IJwt jwt,
        IDataAccessHelperSecure dah) : BasePresupuestosRepository(dah, jwt), IIndicadoresRepository
    {
        //protected new readonly IDataAccessHelperSecure dah = dah;

        public async Task<List<Indicador>> ObtenerIndicadoresConIdiomas(string? descripcion = null)
        {
            List<Indicador> resultado = [];

            try
            {
                string query = $@"
                    SELECT E.COD_ESTADO_VERSION, E.BITAND, E.DES_ESTADO_VERSION, E.IND_MOSTRAR, E.IND_VERSION_UNICA, E.ORDEN,
                           EI.COD_ESTADO_VERSION_IDIOMA, EI.COD_IDIOMA, EI.DES_ESTADO_VERSION_IDIOMA, EI.DES_ESTADO_VERSION_ABRV_IDIOMA, EI.LEYENDA
                    FROM PPT_ESTADOS_VERSIONES E, PPT_ESTADOS_VERSIONES_IDIOMAS EI
                    WHERE E.COD_ESTADO_VERSION = EI.COD_ESTADO_VERSION (+)
                    {(!string.IsNullOrEmpty(descripcion) ? "AND UPPER(E.DES_ESTADO_VERSION) LIKE UPPER('%' || :Descripcion || '%')" : "")}
                    ORDER BY E.ORDEN, E.COD_ESTADO_VERSION";

                dah.GetSqlStringComando(query);

                if (!string.IsNullOrEmpty(descripcion))
                {
                    dah.AddParameter("Descripcion", descripcion);
                }

                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        int indice = 1;
                        int codigo;
                        while (dr.Read())
                        {
                            codigo = dr.GetInt32("COD_ESTADO_VERSION");

                            Indicador? indicador = resultado.Find(c => c.Codigo == codigo);
                            if (indicador == null)
                            {
                                indicador = new();

                                indicador.Codigo = codigo;
                                indicador.Descripcion = dr.GetNullableString("DES_ESTADO_VERSION");
                                indicador.IndMostrar = dr.GetInt16("IND_MOSTRAR") == 1;
                                indicador.IndVersionUnica = dr.GetInt16("IND_VERSION_UNICA") == 1;
                                indicador.BitAnd = dr.GetInt32("BITAND");
                                indicador.Orden = dr.GetInt32("ORDEN");
                                indicador.Indice = indice;
                                indicador.Estado = EstadoEntidad.SinCambios;
                                resultado.Add(indicador);
                                indice++;
                            }

                            int? codigoIdioma = dr.GetNullableInt16("COD_IDIOMA");

                            if (codigoIdioma != null)
                            {
                                IdiomaIndicador idioma = new();
                                idioma.Codigo = dr.GetInt16("COD_ESTADO_VERSION_IDIOMA");
                                idioma.CodigoIndicador = indicador.Codigo;
                                idioma.CodigoIdioma = codigoIdioma.Value;
                                idioma.Descripcion = dr.GetNullableString("DES_ESTADO_VERSION_IDIOMA");
                                idioma.DescripcionAbreviada = dr.GetNullableString("DES_ESTADO_VERSION_ABRV_IDIOMA");
                                idioma.Leyenda = dr.GetNullableString("LEYENDA");
                                indicador.Idiomas.Add(idioma);
                            }
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("IndicadoresRepository.ObtenerIndicadoresConIdiomas", ex);
            }
            return resultado;
        }


        public async Task EliminarIndicador(int codigoIndicador)
        {
            try
            {
                const string query = @"
                    DELETE FROM PPT_ESTADOS_VERSIONES
                    WHERE COD_ESTADO_VERSION = :CodigoEstadoVersion";

                dah.GetSqlStringComando(query);

                dah.AddParameter("codigoEstadoVersion", codigoIndicador);

                await Task.Run(() => dah.ExecuteNonQuery());
            }
            catch (Exception ex)
            {
                throw new Exception("IndicadoresRepository.EliminarIndicador", ex);
            }
        }

        public async Task EliminarIdiomasIndicador(int codigoIndicador)
        {
            try 
            { 
                const string query = @"
                    DELETE FROM PPT_ESTADOS_VERSIONES_IDIOMAS
                    WHERE COD_ESTADO_VERSION = :CodigoEstadoVersion";

                dah.GetSqlStringComando(query);

                dah.AddParameter("codigoEstadoVersion", codigoIndicador);

                await Task.Run(() => dah.ExecuteNonQuery());
            }
            catch (Exception ex)
            {
                throw new Exception("IndicadoresRepository.EliminarIdiomasIndicador", ex);
            }
        }


        public async Task<bool> ExisteIndicador(Indicador indicador)
        {
            bool result = false;
            try
            {
                const string query = @"
                    SELECT COUNT(*)
                    FROM PPT_ESTADOS_VERSIONES
                    WHERE UPPER(DES_ESTADO_VERSION) = :Descripcion
                      AND COD_ESTADO_VERSION != :Codigo";

                dah.GetSqlStringComando(query);

                dah.AddParameter("Descripcion", indicador.Descripcion.ToUpper());
                dah.AddParameter("Codigo", indicador.Codigo ?? -1);

                await AñadirParametroMulticompania(dah);

                int cuantos = await Task.Run(() => dah.ExecuteScalar<int>());
                result = (cuantos > 0);
            }
            catch (Exception ex)
            {
                throw new Exception("IndicadoresRepository.ExisteIndicador", ex);
            }
            return result;
        }


        public async Task<bool> ExisteOrden(Indicador indicador)
        {
            bool result = false;
            try
            {
                const string query = @"
                    SELECT COUNT(*)
                    FROM PPT_ESTADOS_VERSIONES
                    WHERE ORDEN = :Orden
                      AND COD_ESTADO_VERSION != :Codigo";

                dah.GetSqlStringComando(query);

                dah.AddParameter("Orden", indicador.Orden);
                dah.AddParameter("Codigo", indicador.Codigo ?? -1);

                await AñadirParametroMulticompania(dah);

                int cuantos = await Task.Run(() => dah.ExecuteScalar<int>());
                result = (cuantos > 0);
            }
            catch (Exception ex)
            {
                throw new Exception("IndicadoresRepository.ExisteOrden", ex);
            }
            return result;
        }

        public async Task<bool> ExisteBitAnd(Indicador indicador)
        {
            bool result = false;
            try
            {
                const string query = @"
                    SELECT COUNT(*)
                    FROM PPT_ESTADOS_VERSIONES
                    WHERE BITAND = :BitAnd
                      AND COD_ESTADO_VERSION != :Codigo";

                dah.GetSqlStringComando(query);

                dah.AddParameter("BitAnd", indicador.BitAnd);
                dah.AddParameter("Codigo", indicador.Codigo ?? -1);

                await AñadirParametroMulticompania(dah);

                int cuantos = await Task.Run(() => dah.ExecuteScalar<int>());
                result = (cuantos > 0);
            }
            catch (Exception ex)
            {
                throw new Exception("IndicadoresRepository.ExisteBitAnd", ex);
            }
            return result;
        }

        public async Task<int> InsertarIndicador(Indicador indicador)
        {
            int result = 0;

            try
            {
                const string query = @"
                    INSERT INTO PPT_ESTADOS_VERSIONES (DES_ESTADO_VERSION, BITAND, ORDEN, IND_MOSTRAR, IND_VERSION_UNICA)
                    VALUES (:Descripcion, :Bitand, :Orden, :IndMostrar, :IndVersionUnica)
                    RETURNING COD_ESTADO_VERSION INTO :Codigo";

                dah.GetSqlStringComando(query);

                dah.AddParameter("Descripcion", indicador.Descripcion);
                dah.AddParameter("Bitand", indicador.BitAnd);
                dah.AddParameter("Orden", indicador.Orden);
                dah.AddParameter("IndMostrar", indicador.IndMostrar);
                dah.AddParameter("IndVersionUnica", indicador.IndVersionUnica);

                dah.AddParameter("Codigo", indicador.Codigo, DbType.Int32, ParameterDirection.Output, 10);

                await Task.Run(() => dah.ExecuteNonQuery());

                // Obtener el valor del parámetro de salida
                indicador.Codigo = Convert.ToInt32(dah.Comando.Parameters["Codigo"].Value);
                result = indicador.Codigo.Value;
            }
            catch (Exception ex)
            {
                throw new Exception("IndicadoresRepository.InsertarIndicador", ex);
            }
            return result;
        }

        public async Task ActualizarIndicador(Indicador indicador)
        {
            try
            {
                const string query = @"
                    UPDATE PPT_ESTADOS_VERSIONES
                    SET DES_ESTADO_VERSION = :Descripcion,
                        BITAND = :Bitand,
                        ORDEN = :Orden,
                        IND_MOSTRAR = :IndMostrar,
                        IND_VERSION_UNICA = :IndVersionUnica
                    WHERE COD_ESTADO_VERSION = :Codigo";

                dah.GetSqlStringComando(query);

                dah.AddParameter("Descripcion", indicador.Descripcion);
                dah.AddParameter("Bitand", indicador.BitAnd);
                dah.AddParameter("Orden", indicador.Orden);
                dah.AddParameter("IndMostrar", indicador.IndMostrar);
                dah.AddParameter("IndVersionUnica", indicador.IndVersionUnica);
                dah.AddParameter("Codigo", indicador.Codigo);

                await Task.Run(() => dah.ExecuteNonQuery());
            }
            catch (Exception ex)
            {
                throw new Exception("IndicadoresRepository.ActualizarIndicador", ex);
            }
        }

        public async Task InsertarIdiomaIndicador(IdiomaIndicador idiomaIndicador)
        {
            try
            {
                const string query = @"
                    INSERT INTO PPT_ESTADOS_VERSIONES_IDIOMAS (
                        COD_ESTADO_VERSION, COD_IDIOMA, DES_ESTADO_VERSION_IDIOMA, DES_ESTADO_VERSION_ABRV_IDIOMA, LEYENDA)
                    VALUES (:CodigoEstadoVersion, :CodigoIdioma, :Descripcion, :DescripcionAbreviada, :Leyenda)
                    RETURNING COD_ESTADO_VERSION_IDIOMA INTO :Codigo";

                dah.GetSqlStringComando(query);


                dah.AddParameter("CodigoEstadoVersion", idiomaIndicador.CodigoIndicador);
                dah.AddParameter("CodigoIdioma", idiomaIndicador.CodigoIdioma);
                dah.AddParameter("Descripcion", idiomaIndicador.Descripcion);
                dah.AddParameter("DescripcionAbreviada", idiomaIndicador.DescripcionAbreviada);
                dah.AddParameter("Leyenda", idiomaIndicador.Leyenda);

                dah.AddParameter("Codigo", idiomaIndicador.Codigo, DbType.Int32, ParameterDirection.Output, 10);

                await Task.Run(() => dah.ExecuteNonQuery());

                // Obtener el valor del parámetro de salida
                idiomaIndicador.Codigo = Convert.ToInt32(dah.Comando.Parameters["Codigo"].Value);
            }
            catch (Exception ex)
            {
                throw new Exception("IndicadoresRepository.InsertarIdiomaIndicador", ex);
            }
        }


        public async Task ActualizarIdiomaIndicador(IdiomaIndicador idiomaIndicador)
        {
            try
            {
                const string query = @"
                    UPDATE PPT_ESTADOS_VERSIONES_IDIOMAS
                    SET COD_IDIOMA = :CodigoIdioma,
                        DES_ESTADO_VERSION_IDIOMA = :Descripcion,
                        DES_ESTADO_VERSION_ABRV_IDIOMA = :DescripcionAbreviada,
                        LEYENDA = :Leyenda
                    WHERE COD_ESTADO_VERSION_IDIOMA = :Codigo";

                dah.GetSqlStringComando(query);

                dah.AddParameter("CodigoIdioma", idiomaIndicador.CodigoIdioma);
                dah.AddParameter("Descripcion", idiomaIndicador.Descripcion);
                dah.AddParameter("DescripcionAbreviada", idiomaIndicador.DescripcionAbreviada);
                dah.AddParameter("Leyenda", idiomaIndicador.Leyenda);
                dah.AddParameter("Codigo", idiomaIndicador.Codigo);

                await Task.Run(() => dah.ExecuteNonQuery());
            }
            catch (Exception ex)
            {
                throw new Exception("IndicadoresRepository.ActualizarIdiomaIndicador", ex);
            }
        }


        public async Task EliminarIdiomaIndicador(int codigo)
        {
            try
            {
                const string query = @"
                    DELETE FROM PPT_ESTADOS_VERSIONES_IDIOMAS
                    WHERE COD_ESTADO_VERSION_IDIOMA = :Codigo";

                dah.GetSqlStringComando(query);

                dah.AddParameter("Codigo", codigo);

                await Task.Run(() => dah.ExecuteNonQuery());
            }
            catch (Exception ex)
            {
                throw new Exception("IndicadoresRepository.EliminarIdiomaIndicador", ex);
            }
        }


        public async Task<int> ObtenerUltimoBitAnd()
        {
            int resultado = -1;
            try
            {
                const string query = @"
                    SELECT MAX(BITAND) BITAND
                    FROM PPT_ESTADOS_VERSIONES";

                dah.GetSqlStringComando(query);

                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        while (dr.Read())
                        {
                            resultado = dr.GetInt32("BITAND");
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("IndicadoresRepository.ObtenerUltimoBitAnd", ex);
            }
            return resultado;
        }

        public async Task<int> ObtenerUltimoOrden()
        {
            int resultado = -1;
            try
            {
                const string query = @"
                    SELECT MAX(ORDEN) ORDEN
                    FROM PPT_ESTADOS_VERSIONES";

                dah.GetSqlStringComando(query);

                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        while (dr.Read())
                        {
                            resultado = dr.GetInt32("ORDEN");
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("IndicadoresRepository.ObtenerUltimoOrden", ex);
            }
            return resultado;
        }

        public async Task<int> ObtenerBitAndIndicador(int codigoIndicador)
        {
            int resultado = -1;

            try
            {
                const string query = @"
                    SELECT BITAND
                    FROM PPT_ESTADOS_VERSIONES
                    WHERE COD_ESTADO_VERSION = :CodigoIndicador";

                dah.GetSqlStringComando(query);

                dah.AddParameter("CodigoIndicador", codigoIndicador);
                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        if (dr.Read())
                        {
                            resultado = dr.GetInt32("BITAND");
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("IndicadoresRepository.ObtenerBitAndIndicador", ex);
            }
            return resultado;
        }


        /// <summary>
        /// Actualiza el bitAnd de una version. Se mete en este repositorio para poder meterlo en una transaccion
        /// </summary>
        /// <param name="codigoVersion"></param>
        /// <param name="bitAnd"></param>
        /// <returns></returns>
        public async Task Actualizar1BitAndVersion(int codigoVersion, int bitAnd)
        {
            try
            {
                const string query = @"
                    UPDATE PPT_VERSIONES
                    SET IND_ESTADO_VERSION = :Estado
                    WHERE COD_VERSION = :CodigoVersion";

                dah.GetSqlStringComando(query);


                dah.AddParameter("Estado", bitAnd);

                dah.AddParameter("CodigoVersion", codigoVersion);

                await Task.Run(() => dah.ExecuteNonQuery());
            }
            catch (Exception ex)
            {
                throw new Exception("IndicadoresRepository.Actualizar1BitAndVersion", ex);
            }
        }

        public new ITransaccion ObtenerTransaccion() => new TransaccionWrapper(base.ObtenerTransaccion());

    }
}




