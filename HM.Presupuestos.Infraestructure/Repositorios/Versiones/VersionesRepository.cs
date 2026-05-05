using HM.Core.Comun.v6.Seguridad.Interfaces;
using HM.Core.Servidor.v6.DAL;
using HM.Core.Servidor.v6.DAL.Interfaces;
using HM.Core.Servidor.v6.Repositories;
using HM.Presupuestos.Domain.Comun;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Puertos;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Text;
using System.Text.Json;
using Version = HM.Presupuestos.Domain.Entidades.Version;

namespace HM.Presupuestos.Infraestructure.Repositorios
{
    public class VersionesRepository(
        IJwt jwt, 
        IDataAccessHelperBase dahBase,
        IDataAccessHelperSecure dah) : BasePresupuestosRepository(dah, jwt), IVersionesRepository
    {
        protected readonly IJwt jwt = jwt;
        protected new readonly IDataAccessHelperSecure dah = dah;
        protected readonly IDataAccessHelperBase dahBase = dahBase;

        public async Task<List<Indicador>> ObtenerEstadosVersiones()
        {
            List<Indicador> resultado = [];

            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT COD_ESTADO_VERSION, BITAND, DES_ESTADO_VERSION, IND_MOSTRAR, IND_VERSION_UNICA, ORDEN ");
                query.AppendLine("FROM PPT_ESTADOS_VERSIONES ");
                query.AppendLine(" ORDER BY ORDEN, COD_ESTADO_VERSION");

                dah.GetSqlStringComando(query.ToString());

                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        int indice = 1;
                        while (dr.Read())
                        {
                            resultado.Add(new Indicador()
                            {
                                Codigo = dr.GetInt32("COD_ESTADO_VERSION"),
                                Descripcion = dr.GetNullableString("DES_ESTADO_VERSION"),
                                IndMostrar = dr.GetInt16("IND_MOSTRAR") == 1,
                                IndVersionUnica = dr.GetInt16("IND_VERSION_UNICA") == 1,
                                BitAnd = dr.GetInt32("BITAND"),
                                Estado = EstadoEntidad.SinCambios,
                                Indice = indice,
                            });
                            indice++;
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("VersionesRepository.ObtenerEstadosVersiones", ex);
            }

            return resultado;
        }

        /// <summary>
        /// Devuelve una lista de versiones resumen filtrada
        /// </summary>
        /// <param name="anio">Filtro opcional para el año (null = todos los años)</param>
        /// <param name="estadoIncluido">Filtro para buscar por el BitAnd (Indicador de estado). Para mas de un indicador hay que sumarlos</param>
        /// <param name="estadoExcluido">Filtro para buscar excluyendo por el BitAnd (Indicador de estado). Para mas de un indicador hay que sumarlos</param>
        /// <returns>Lista de versiones</returns>
        public async Task<List<VersionResumen>> ObtenerVersionesResumen(int? anio = null, int? estadoIncluido = null, int? estadoExcluido = null)
        {
            List<VersionResumen> resultado = [];

            try
            {
                string query = @"
                    SELECT COD_VERSION, DES_VERSION, IND_ESTADO_VERSION, COD_TIPO_VERSION 
                    FROM PPT_VERSIONES 
                    WHERE 1=1";

                if (anio.HasValue)
                {
                    query += " AND ANIO = :Anio";
                }

                if (estadoIncluido.HasValue)
                {
                    query += " AND BITAND(IND_ESTADO_VERSION, :indEstado) = :indEstado";
                }

                if (estadoExcluido.HasValue)
                {
                    query += " AND BITAND(IND_ESTADO_VERSION, :indEstadoQuitar) != :indEstadoQuitar";
                }

                query += " ORDER BY DES_VERSION";

                dah.GetSqlStringComando(query);

                if (anio.HasValue)
                {
                    dah.AddParameter("Anio", anio.Value);
                }

                if (estadoIncluido.HasValue)
                {
                    dah.AddParameter("indEstado", estadoIncluido.Value);
                }

                if (estadoExcluido.HasValue)
                {
                    dah.AddParameter("indEstadoQuitar", estadoExcluido.Value);
                }

                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        while (dr.Read())
                        {
                            VersionResumen version = new()
                            {
                                Codigo = dr.GetInt32("COD_VERSION"),
                                Descripcion = dr.GetString("DES_VERSION"),
                                IndEstado = dr.GetInt32("IND_ESTADO_VERSION"),
                                CodigoTipo = dr.GetInt16("COD_TIPO_VERSION"),
                            };
                            resultado.Add(version);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("VersionesRepository.ObtenerVersionesResumen", ex);
            }
            return resultado;
        }



        /// <summary>
        /// Devuelve una lista de versiones filtrada
        /// </summary>
        /// <param name="anio">Filtro para el año</param>
        /// <param name="estadoIncluido">Filtro para buscar por el BitAnd (Indicador de estado). Para mas de un indicador hay que sumarlos en binario</param>
        /// <returns>Lista de versiones</returns>
        public async Task<List<Version>> ObtenerVersiones( int anio, int? estadoIncluido = null)
        {
            List<Version> resultado = [];

            try
            {
                StringBuilder query = new();
                query.AppendLine(" SELECT COD_VERSION,DES_VERSION, MES_VERSION, IND_ESTADO_VERSION, ORDEN, COD_TIPO_VERSION ");
                query.AppendLine(" FROM PPT_VERSIONES ");
                query.AppendLine(" WHERE ANIO = :Anio");

                if (estadoIncluido.HasValue)
                {
                    query.AppendLine(" AND BITAND(IND_ESTADO_VERSION, :indEstado ) = :indEstado ");
                }

                query.AppendLine(" ORDER BY ORDEN DESC, DES_VERSION");

                dah.GetSqlStringComando(query.ToString());

                dah.AddParameter("Anio", anio);

                if (estadoIncluido.HasValue)
                {
                    dah.AddParameter("indEstado", estadoIncluido.Value);
                }

                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        while (dr.Read())
                        {
                            Version version = new()
                            {
                                Codigo = dr.GetInt32("COD_VERSION"),
                                Descripcion = dr.GetString("DES_VERSION"),
                                IndEstado = dr.GetInt32("IND_ESTADO_VERSION"),
                                Anio = anio,
                                Mes = dr.GetInt16("MES_VERSION"),
                                Orden = dr.GetInt16("ORDEN"),
                                CodigoTipo = dr.GetInt16("COD_TIPO_VERSION"),
                            };
                            resultado.Add(version);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("VersionesRepository.ObtenerVersiones", ex);
            }
            return resultado;
        }

        public async Task<List<CodigoDescripcion>> ObtenerAniosConVersiones()
        {
            List<CodigoDescripcion> resultado = [];

            try
            {
                StringBuilder query = new();
                query.AppendLine(" SELECT DISTINCT(ANIO) ");
                query.AppendLine(" FROM PPT_VERSIONES ");
                query.AppendLine(" ORDER BY ANIO DESC");

                dah.GetSqlStringComando(query.ToString());

                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        while (dr.Read())
                        {
                            CodigoDescripcion anio = new();
                            anio.Codigo = dr.GetInt16("ANIO");
                            anio.Descripcion = dr.GetString("ANIO");

                            resultado.Add(anio);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("VersionesRepository.ObtenerAniosConVersiones", ex);
            }
            return resultado;
        }

      


        public async Task<int> InsertarVersion(int codigoPais, Version version)
        {
            int result = 0;
            try
            {
                StringBuilder query = new();
                query.Append("INSERT INTO PPT_VERSIONES (COD_PAIS, ");
                query.Append("                   DES_VERSION, ");
                query.Append("                   ANIO, ");
                query.Append("                   MES_VERSION, ");
                query.Append("                   IND_ESTADO_VERSION, ");
                query.Append("                   ORDEN, ");
                query.Append("                   COD_TIPO_VERSION) ");
                query.Append("     VALUES ( :CodigoPais, ");
                query.Append("             :DesVersion, ");
                query.Append("             :Anio, ");
                query.Append("             :Mes, ");
                query.Append("             :Estado, ");
                query.Append("             :Orden, ");
                query.Append("             :Tipo) ");
                query.Append("    RETURNING COD_VERSION ");
                query.Append("       INTO :CodigoVersion ");

                dah.GetSqlStringComando(query.ToString());

                dah.AddParameter("CodigoPais", codigoPais);
                dah.AddParameter("DesVersion", version.Descripcion);
                dah.AddParameter("Anio", version.Anio);
                dah.AddParameter("Mes", version.Mes);
                dah.AddParameter("Estado", version.IndEstado);
                dah.AddParameter("Orden", version.Orden);
                dah.AddParameter("Tipo", version.CodigoTipo);

                dah.AddParameter("CodigoVersion", version.Codigo, DbType.Int32, ParameterDirection.Output, 10);

                await Task.Run(() => dah.ExecuteNonQuery());

                // Obtener el valor del parámetro de salida
                version.Codigo = Convert.ToInt32(dah.Comando.Parameters["CodigoVersion"].Value);
                result = version.Codigo;
            }
            catch (Exception ex)
            {
                throw new Exception("VersionesRepository.InsertarVersion", ex);
            }

            return result;
        }

        public async Task ActualizarVersion(Version version)
        {
            try
            {
                StringBuilder query = new();
                query.Append("UPDATE PPT_VERSIONES ");
                query.Append("   SET DES_VERSION = :DesVersion, ");
                query.Append("       MES_VERSION = :Mes, ");
                query.Append("       IND_ESTADO_VERSION = :Estado, ");
                query.Append("       ORDEN = :Orden, ");
                query.Append("       COD_TIPO_VERSION = :Tipo ");
                query.Append(" WHERE COD_VERSION = :CodigoVersion ");

                dah.GetSqlStringComando(query.ToString());

                dah.AddParameter("DesVersion", version.Descripcion);
                dah.AddParameter("Mes", version.Mes);
                dah.AddParameter("Estado", version.IndEstado);
                dah.AddParameter("Orden", version.Orden);
                dah.AddParameter("Tipo", version.CodigoTipo);
                dah.AddParameter("CodigoVersion", version.Codigo);

                await Task.Run(() => dah.ExecuteNonQuery());
            }
            catch (Exception ex)
            {
                throw new Exception("VersionesRepository.ActualizarVersion", ex);
            }
        }

        public async Task EliminarVersion(int codigoVersion)
        {
            try
            {
                StringBuilder query = new();
                query.Append("DELETE FROM PPT_VERSIONES ");
                query.Append("WHERE COD_VERSION = :CodigoVersion");

                dah.GetSqlStringComando(query.ToString());
                dah.AddParameter("CodigoVersion", codigoVersion);

                await Task.Run(() => dah.ExecuteNonQuery());
            }
            catch (Exception ex)
            {
                throw new Exception("VersionesRepository.EliminarVersion", ex);
            }
        }

        //not referenced
        //deprecated
        public async Task<bool> ExistenPrevisionesEnVersion(int codigoVersion)
        {
            bool result = false;
            try
            {
                StringBuilder query = new();
                query.Append("SELECT COUNT(*)  ");
                query.Append(" FROM PPT_PREVISIONES ");
                query.Append(" WHERE COD_VERSION = :CodigoVersion ");

                dah.GetSqlStringComando(query.ToString());
                dah.AddParameter("CodigoVersion", codigoVersion);

                await AñadirParametroMulticompania(dah);

                int cuantos = await Task.Run(() => dah.ExecuteScalar<int>());
                result = (cuantos > 0);
            }
            catch (Exception ex)
            {
                throw new Exception("VersionesRepository.ExistenPrevisionesEnVersion", ex);
            }

            return result;
        }

        //not referenced
        //deprecated
        public async Task<bool> ExistenCondicionesEnVersion(int codigoVersion)
        {
            bool result = false;
            try
            {
                StringBuilder query = new();
                query.Append("SELECT COUNT(*)  ");
                query.Append(" FROM PPT_CONDICION_VIGENCIA ");
                query.Append(" WHERE COD_VERSION = :CodigoVersion ");

                dah.GetSqlStringComando(query.ToString());
                dah.AddParameter("CodigoVersion", codigoVersion);

                await AñadirParametroMulticompania(dah);

                int cuantos = await Task.Run(() => dah.ExecuteScalar<int>());
                result = (cuantos > 0);
            }
            catch (Exception ex)
            {
                throw new Exception("VersionesRepository.ExistenCondicionesEnVersion", ex);
            }

            return result;
        }

        //not referenced
        //deprecated
        public async Task<bool> ExistenSobreprimasEnVersion(int codigoVersion)
        {
            bool result = false;
            try
            {
                StringBuilder query = new();
                query.Append("SELECT COUNT(*)  ");
                query.Append(" FROM PPT_SOBREPRIMAS_MEDIO ");
                query.Append(" WHERE COD_VERSION = :CodigoVersion ");

                dah.GetSqlStringComando(query.ToString());
                dah.AddParameter("CodigoVersion", codigoVersion);

                await AñadirParametroMulticompania(dah);

                int cuantos = await Task.Run(() => dah.ExecuteScalar<int>());
                result = (cuantos > 0);
            }
            catch (Exception ex)
            {
                throw new Exception("VersionesRepository.ExistenSobreprimasEnVersion", ex);
            }

            return result;
        }


        /// <summary>
        /// Obtener importes de los medios
        /// </summary>
        /// <param name="json">Filtro con todos los datos necesarios para buscar estos importes (origen, medios y otros)</param>
        /// <returns></returns>
        public async Task<List<MedioIncremento>> ObtenerImportesMedios(FiltroComprobarNetoVentaOrigenJSON json)
        {
            List<MedioIncremento> resultado = [];
          
            string jsonString = JsonSerializer.Serialize(json);

            dah.GetStoredProcComando("PKG_CARGA_DATOS_VERSIONES.GET_IMPORTES");
            dah.Comando.CommandType = CommandType.StoredProcedure;

            //IMPORTANTE: Los parametros deben ir en el mismo orden que en el procedimiento almacenado
            dah.AddParameter("p_jSonConf", jsonString);
            dah.AddParameter("pCOD_USUARIO", json.CodigoUsuario);
            IDbDataParameter pCursor = dah.AddParameter("p_CURSOR", null);
            ((OracleParameter)pCursor).AsignarParametroRefCursor();
            pCursor.Direction = ParameterDirection.InputOutput;
            pCursor.Size = 3000;

            IDbDataParameter resultadoInt = dah.AddParameter("pRESULTADO", null, DbType.Int32, ParameterDirection.Output, 0);
            IDbDataParameter resultadoStr = dah.AddParameter("pRESULTADO_STR", null, DbType.String, ParameterDirection.Output, 4000);

            await Task.Run(() =>
            {
                DataSet ds = dah.ExecuteDataSet();

                if (ds.Tables.Count > 0)
                {
                    DataTable table = ds.Tables[0];

                    List<MedioIncremento> lista = [];

                    foreach (DataRow row in table.Rows)
                    {
                        MedioIncremento item = new()
                        {
                            CodigoMedio = Convert.ToInt32(row["COD_MEDIO"]),
                            NetoVentaOrigen = Convert.ToDecimal(row["IMP_NETO_VENTA"])
                        };

                        lista.Add(item);
                    }
                    resultado = lista;
                }
            });

            int codigoResultado = Convert.ToInt32(resultadoInt.Value);
            if (codigoResultado < 0)
            {
                string mensajeResultado = resultadoStr.Value != null
                    ? $"Error -> {Convert.ToString(resultadoStr.Value)}"
                    : "Error de BD no especificado al ejecutar 'PKG_CARGA_DATOS_VERSIONES.GET_IMPORTES'";

                throw new ExcepcionBaseDatos(codigoResultado, mensajeResultado);
            }
            return resultado;
        }

      
        public async Task GrabarCopiasVersiones( DatosCargarVersionDestinoJSON json)
        {
            string jsonString = JsonSerializer.Serialize(json);

            dah.GetStoredProcComando("PKG_CARGA_DATOS_VERSIONES.SET_COPIA");
            dah.Comando.CommandType = CommandType.StoredProcedure;

            //IMPORTANTE: Los parametros deben ir en el mismo orden que en el procedimiento almacenado
            dah.AddParameter("pCod_Version_Destino", json.CodigoVersion);
            dah.AddParameter("p_jSonConf", jsonString);
            dah.AddParameter("p_Cod_Usuario", json.CodigoUsuario);

            IDbDataParameter resultadoInt = dah.AddParameter("pRESULTADO", null, DbType.Int32, ParameterDirection.Output, 0);
            IDbDataParameter resultadoStr = dah.AddParameter("pRESULTADO_STR", null, DbType.String, ParameterDirection.Output, 4000);

            await Task.Run(() =>
            {
                dah.ExecuteNonQuery(dah.Comando);
            });

            int codigoResultado = Convert.ToInt32(resultadoInt.Value);

            if (codigoResultado < 0)
            {
                string mensajeResultado = resultadoStr.Value != null 
                    ? $"Error -> {Convert.ToString(resultadoStr.Value)}" 
                    : "Error de BD no especificado al ejecutar 'PKG_CARGA_DATOS_VERSIONES.SET_COPIA'";

                throw new ExcepcionBaseDatos (codigoResultado, mensajeResultado );
            }
        }


        /// <summary>
        /// Checking there is data linked to version
        /// </summary>
        /// <param name="codigoVersion">Version code</param>
        /// <returns>True when is data linked</returns>
        public async Task<bool> IsDataLinked(int codigoVersion)
        {
            bool result=false;

            try
            {
                dah.GetStoredProcComando(@"
                SELECT PKG_PPT_FUNCION.GET_HAY_DATOS_RELACION_VERSION(:pCOD_VERSION) FROM DUAL");

                dah.Comando.CommandType = CommandType.Text;

                dah.AddParameter("pCOD_VERSION", codigoVersion);

                await Task.Run(() =>
                {
                    var output = dah.ExecuteScalar<int>(dah.Comando);
                    result=(output==1);
                });
            }
            catch (Exception ex)
            {
                throw new Exception("VersionesRepository.IsDataLinked", ex);
            }
            return result;
        }

    }
}


