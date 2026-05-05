using HM.Core.Comun.v6.Seguridad.Interfaces;
using HM.Core.Servidor.v6.DAL.Interfaces;
using HM.Presupuestos.Domain.Comun;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Puertos;
using System.Data;
using System.Text;
using System.Text.Json;

namespace HM.Presupuestos.Infrastructure.Repositorios
{
    public class CondicionesRepository(
        IDataAccessHelperSecure dah,
        IJwt jwt) : BasePresupuestosRepository(dah, jwt), ICondicionesRepository
    {
        public async Task<List<ConceptoCondicion>> ObtenerConceptos()
        {
            var resultado = new List<ConceptoCondicion>();

            const string query = @"
                SELECT COD_CONCEPTO_CONDICION, DES_CONCEPTO_CONDICION, DES_ABRV_CONCEPTO_CONDICION, IND_SIGNO, IND_CALCULO
                  FROM PPT_CONCEPTOS_CONDICIONES
                 WHERE F_BAJA IS NULL
              ORDER BY COD_CONCEPTO_CONDICION";

            dah.GetSqlStringComando(query);
            dah.Comando.Parameters.Clear();

            await AñadirParametroMulticompania(dah);

            await Task.Run(() =>
            {
                dah.ProcesarDatos(dr =>
                {
                    while (dr.Read())
                    {
                        resultado.Add(new ConceptoCondicion
                        {
                            Codigo = dr.GetInt32("COD_CONCEPTO_CONDICION"),
                            Descripcion = dr.GetString("DES_CONCEPTO_CONDICION"),
                            Abreviatura = dr.GetString("DES_ABRV_CONCEPTO_CONDICION"),
                            IndicadorCalculo = dr.GetInt16("IND_CALCULO"),
                            IndicadorSigno = dr.GetInt16("IND_SIGNO")
                        });
                    }
                });
            });

            return resultado;
        }


        public async Task<List<Vigencia>> ObtenerVigencias(CondicionFiltro filtro)
        {
            var resultado = new List<Vigencia>();

            var query = @"
                SELECT COD_CONDICION_VIGENCIA, MES_DESDE, MES_HASTA
                  FROM PPT_CONDICION_VIGENCIA
                 WHERE COD_VERSION = :CodigoVersion
                   AND COD_NETWORK = :CodigoNetwork
                   AND COD_GRUPO = :CodigoGrupo
                   AND IND_ACUERDO = :IndAcuerdo
              ORDER BY COD_CONDICION_VIGENCIA";

            dah.GetSqlStringComando(query);
            dah.Comando.Parameters.Clear();

            dah.AddParameter("CodigoVersion", filtro.CodigoVersion);
            dah.AddParameter("CodigoNetwork", filtro.CodigoNetwork);
            dah.AddParameter("CodigoGrupo", filtro.CodigoGrupoCliente);
            dah.AddParameter("IndAcuerdo", filtro.IndicadorAcuerdo);

            await AñadirParametroMulticompania(dah);

            await Task.Run(() =>
            {
                dah.ProcesarDatos(dr =>
                {
                    while (dr.Read())
                    {
                        var item = new Vigencia
                        {
                            Codigo = dr.GetInt32("COD_CONDICION_VIGENCIA"),
                            MesDesde = dr.GetInt32("MES_DESDE"),
                            MesHasta = dr.GetInt32("MES_HASTA"),
                            CodigoGrupoCliente = filtro.CodigoGrupoCliente,
                            CodigoNetWork = filtro.CodigoNetwork,
                            CodigoVersion = filtro.CodigoVersion,
                            IndicadorAcuerdo = filtro.IndicadorAcuerdo
                        };

                        resultado.Add(item);
                    }
                });
            });

            return resultado;
        }



        public async Task InsertarVigencia(Vigencia item)
        {
            var query = @"
                INSERT INTO PPT_CONDICION_VIGENCIA(
                    COD_VERSION,
                    COD_NETWORK,
                    COD_PAIS,
                    COD_GRUPO,
                    MES_DESDE,
                    MES_HASTA,
                    IND_ACUERDO,
                    COD_USUARIO_MODIFICACION,
                    COD_USUARIO_ALTA
                )
                VALUES (
                    :CodigoVersion,
                    :CodigoNetwork,
                    :CodigoPais,
                    :CodigoGrupo,
                    :MesDesde,
                    :MesHasta,
                    :IndicadorAcuerdo,
                    :CodigoUsuarioModificacion,
                    :CodigoUsuarioAlta
                )
                RETURNING COD_CONDICION_VIGENCIA INTO :CodigoVigencia";

            dah.GetSqlStringComando(query);
            dah.Comando.Parameters.Clear();

            dah.AddParameter("CodigoVersion", item.CodigoVersion);
            dah.AddParameter("CodigoNetwork", item.CodigoNetWork);
            dah.AddParameter("CodigoPais", CodigoPais);
            dah.AddParameter("CodigoGrupo", item.CodigoGrupoCliente);
            dah.AddParameter("MesDesde", item.MesDesde);
            dah.AddParameter("MesHasta", item.MesHasta);
            dah.AddParameter("IndicadorAcuerdo", item.IndicadorAcuerdo);
            dah.AddParameter("CodigoUsuarioAlta", CodigoUsuario);
            dah.AddParameter("CodigoUsuarioModificacion", CodigoUsuario);

            // Parámetro de salida
            dah.AddParameter("CodigoVigencia", item.Codigo, DbType.Int32, ParameterDirection.Output, 10);

            await Task.Run(() => dah.ExecuteNonQuery());

            item.Codigo = Convert.ToInt32(dah.Comando.Parameters["CodigoVigencia"].Value);
        }


        public async Task ActualizarVigencia(Vigencia item)
        {
            try
            {
                StringBuilder query = new();
                query.Append("UPDATE PPT_CONDICION_VIGENCIA ");
                query.Append("   SET MES_DESDE = :MesDesde, ");
                query.Append("       MES_HASTA = :MesHasta, ");
                query.Append("       COD_USUARIO_MODIFICACION = :CodigoUsuario ");
                query.Append(" WHERE COD_CONDICION_VIGENCIA = :CodigoVigencia ");

                dah.GetSqlStringComando(query.ToString());
                dah.Comando.Parameters.Clear();

                dah.AddParameter("CodigoVigencia", item.Codigo);
                dah.AddParameter("MesDesde", item.MesDesde);
                dah.AddParameter("MesHasta", item.MesHasta);
                dah.AddParameter("CodigoUsuario", CodigoUsuario);

                await Task.Run(() => dah.ExecuteNonQuery());
            }
            catch (Exception ex)
            {
                throw new Exception("CondicionesClientesRepository.ActualizarVigencia", ex);
            }
        }


        /// <summary>
        /// Metodo para borrar una Vigencia. Llama a un PL que es el que se encarga de borrar las condiciones y excepciones (con sus conceptos) de dicha vigencia.
        /// </summary>
        /// <param name="codigoVigencia"></param>
        /// <returns>Objeto con el codigo devuelto por la operacion y un texto si fuera un error no controlado en el PL</returns>
        public async Task EliminarVigencia( int codigoVigencia)
        {
            dah.GetStoredProcComando("PKG_BORRAR_VIGENCIAS.SET_BORRAR_VIGENCIAS");
            dah.Comando.CommandType = CommandType.StoredProcedure;
            dah.Comando.Parameters.Clear();

            //IMPORTANTE: Los parametros deben ir en el mismo orden que en el procedimiento almacenado
            dah.AddParameter("pCOD_CONDICION_VIGENCIA", codigoVigencia);
            dah.AddParameter("pCOD_USUARIO", CodigoUsuario);

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
                    : "Error de BD no especificado al ejecutar 'PKG_BORRAR_VIGENCIAS.SET_BORRAR_VIGENCIAS'";

                throw new ExcepcionBaseDatos(codigoResultado, mensajeResultado);
            }
        }


        public async Task<bool> ExistenCondicionesVigencias(int codigoVigencia)
        {
            var query = @"
                SELECT COUNT(*)
                FROM PPT_CONDICION_MEDIO
                WHERE COD_CONDICION_VIGENCIA = :CodigoVigencia";

            dah.GetSqlStringComando(query);
            dah.Comando.Parameters.Clear();

            dah.AddParameter("CodigoVigencia", codigoVigencia);

            int cuantos = await Task.Run(() => dah.ExecuteScalar<int>());

            return cuantos > 0;
        }


        public async Task<List<CondicionDto>> ObtenerCondicionesPorVigencia(int codigoVigencia)
        {
            List<CondicionDto> resultado = new();

            var query = @"
                SELECT 
                    COD_CONDICION_VIGENCIA,
                    COD_PAIS,
                    COD_MEDIO,
                    DES_MEDIO,
                    PCT_SAG,
                    PCT_MNP,
                    PCT_DEV,
                    IND_CALCULO_DEV,
                    COUNT_EXCEPCIONES
                FROM V_PPT_CONDICION_MEDIO
                WHERE COD_CONDICION_VIGENCIA = :CodigoVigencia
                ORDER BY COD_MEDIO";

            dah.GetSqlStringComando(query);
            dah.Comando.Parameters.Clear();
            dah.AddParameter("CodigoVigencia", codigoVigencia);

            await AñadirParametroMulticompania(dah);

            await Task.Run(() =>
            {
                dah.ProcesarDatos((dr) =>
                {
                    while (dr.Read())
                    {
                        CondicionDto item = new()
                        {
                            CodigoMedio = dr.GetInt32("COD_MEDIO"),
                            DescripcionMedio = dr.GetString("DES_MEDIO"),
                            PctSAG = dr.GetNullableDecimal("PCT_SAG"),
                            PctManPower = dr.GetNullableDecimal("PCT_MNP"),
                            PctDevolucion = dr.GetNullableDecimal("PCT_DEV"),
                            IndicadorCalculoDevolucion = dr.GetNullableInt32("IND_CALCULO_DEV") ?? 0,
                            NumeroExcepciones = dr.GetInt16("COUNT_EXCEPCIONES")
                        };

                        resultado.Add(item);
                    }
                });
            });

            return resultado;
        }


        public async Task ActualizarCondicion( Condicion medioCondicion)
        {
            var query = @"
                UPDATE PPT_CONDICION_MEDIO
                   SET PCT_CONDICION_MEDIO = :Porcentaje,
                       IND_CALCULO        = :IndCalculo,
                       NUM_JERARQUIA      = :Jerarquia,
                       COD_USUARIO_MODIFICACION = :CodigoUsuario
                 WHERE COD_CONDICION_MEDIO = :CodigoCondicionMedio";

            dah.GetSqlStringComando(query);
            dah.Comando.Parameters.Clear();

            dah.AddParameter("Porcentaje", medioCondicion.Porcentaje);
            dah.AddParameter("IndCalculo", medioCondicion.IndicadorCalculo);
            dah.AddParameter("Jerarquia", medioCondicion.Jerarquia);
            dah.AddParameter("CodigoCondicionMedio", medioCondicion.CodigoCondicion);
            dah.AddParameter("CodigoUsuario", CodigoUsuario);

            await Task.Run(() => dah.ExecuteNonQuery());
        }


        public async Task InsertarCondicion(Condicion condicion)
        {
            var query = @"
                INSERT INTO PPT_CONDICION_MEDIO(
                    COD_PAIS,
                    COD_MEDIO,
                    COD_CONDICION_VIGENCIA,
                    COD_CONCEPTO_CONDICION,
                    IND_CALCULO,
                    NUM_JERARQUIA,
                    PCT_CONDICION_MEDIO,
                    COD_USUARIO,
                    COD_USUARIO_MODIFICACION
                )
                VALUES (
                    :CodigoPais,
                    :CodigoMedio,
                    :CodigoVigencia,
                    :CodigoConcepto,
                    :IndCalculo,
                    :Jerarquia,
                    :Porcentaje,
                    :CodigoUsuario,
                    :CodigoUsuario
                )
                RETURNING COD_CONDICION_MEDIO INTO :CodigoCondicionMedio";

            dah.GetSqlStringComando(query);
            dah.Comando.Parameters.Clear();

            dah.AddParameter("CodigoPais", CodigoPais);
            dah.AddParameter("CodigoMedio", condicion.CodigoMedio);
            dah.AddParameter("CodigoVigencia", condicion.CodigoVigencia);
            dah.AddParameter("CodigoConcepto", (int)condicion.CodigoConcepto);
            dah.AddParameter("IndCalculo", condicion.IndicadorCalculo);
            dah.AddParameter("Jerarquia", condicion.Jerarquia);
            dah.AddParameter("Porcentaje", condicion.Porcentaje);
            dah.AddParameter("CodigoUsuario", CodigoUsuario);

            // Parámetro de salida
            dah.AddParameter("CodigoCondicionMedio", condicion.CodigoCondicion, DbType.Int32, ParameterDirection.Output, 10);

            await Task.Run(() => dah.ExecuteNonQuery());

            condicion.CodigoCondicion = Convert.ToInt32(dah.Comando.Parameters["CodigoCondicionMedio"].Value);
        }


        public async Task GrabarCondicion(Condicion condicion)
        {
            var query = @"
                MERGE INTO PPT_CONDICION_MEDIO T
                USING DUAL
                   ON (T.COD_PAIS = :CodigoPais
                   AND T.COD_MEDIO = :CodigoMedio
                   AND T.COD_CONDICION_VIGENCIA = :CodigoVigencia
                   AND T.COD_CONCEPTO_CONDICION = :CodigoConcepto
                   AND T.NUM_JERARQUIA = :Jerarquia)
                WHEN MATCHED THEN
                    UPDATE SET
                        PCT_CONDICION_MEDIO = :Porcentaje,
                        IND_CALCULO = :IndCalculo,
                        COD_USUARIO_MODIFICACION = :CodigoUsuario
                WHEN NOT MATCHED THEN
                    INSERT (
                        COD_PAIS,
                        COD_MEDIO,
                        COD_CONDICION_VIGENCIA,
                        COD_CONCEPTO_CONDICION,
                        IND_CALCULO,
                        NUM_JERARQUIA,
                        PCT_CONDICION_MEDIO,
                        COD_USUARIO,
                        COD_USUARIO_MODIFICACION
                    )
                    VALUES (
                        :CodigoPais,
                        :CodigoMedio,
                        :CodigoVigencia,
                        :CodigoConcepto,
                        :IndCalculo,
                        :Jerarquia,
                        :Porcentaje,
                        :CodigoUsuario,
                        :CodigoUsuario
                    )";

            dah.GetSqlStringComando(query);
            dah.Comando.Parameters.Clear();

            dah.AddParameter("CodigoPais", CodigoPais);
            dah.AddParameter("CodigoMedio", condicion.CodigoMedio);
            dah.AddParameter("CodigoVigencia", condicion.CodigoVigencia);
            dah.AddParameter("CodigoConcepto", (int)condicion.CodigoConcepto);
            dah.AddParameter("Jerarquia", condicion.Jerarquia);
            dah.AddParameter("Porcentaje", condicion.Porcentaje);
            dah.AddParameter("IndCalculo", condicion.IndicadorCalculo);
            dah.AddParameter("CodigoUsuario", CodigoUsuario);

            await Task.Run(() => dah.ExecuteNonQuery());
        }


        public async Task EliminarCondicion(Condicion condicion)
        {
            var query = @"
                DELETE FROM PPT_CONDICION_MEDIO
                WHERE COD_PAIS = :CodigoPais
                  AND COD_MEDIO = :CodigoMedio
                  AND COD_CONDICION_VIGENCIA = :CodigoVigencia
                  AND COD_CONCEPTO_CONDICION = :CodigoConcepto
                  AND NUM_JERARQUIA = :Jerarquia";

            dah.GetSqlStringComando(query);
            dah.Comando.Parameters.Clear();

            dah.AddParameter("CodigoPais", CodigoPais);
            dah.AddParameter("CodigoMedio", condicion.CodigoMedio);
            dah.AddParameter("CodigoVigencia", condicion.CodigoVigencia);
            dah.AddParameter("CodigoConcepto", (int)condicion.CodigoConcepto);
            dah.AddParameter("Jerarquia", condicion.Jerarquia);

            await Task.Run(() => dah.ExecuteNonQuery());
        }


        public async Task<List<ExcepcionDto>> ObtenerExcepcionesCondiciones(int codigoVigencia)
        {
            var resultado = new List<ExcepcionDto>();

            var query = @"
                SELECT 
                    COD_CONDICION_VIGENCIA, COD_PAIS, COD_MEDIO, DES_MEDIO, NUM_JERARQUIA,
                    PORC_SAG, PORC_MNP, PORC_DEV, IND_CALCULO_DEV, COD_ALCANCE,
                    COD_DISCIPLINA, COD_DIVERSIFIED, COD_OBJETIVO, COD_TIPO_COMPRA,
                    COD_TIPO_DISCIPLINA, COD_DISCIPLINA_GRUPO, COD_CONDICION_MEDIO
                FROM V_PPT_CONDICION_MEDIO_EXCEP
                WHERE COD_CONDICION_VIGENCIA = :CodigoVigencia
                ORDER BY COD_MEDIO, NUM_JERARQUIA";

            dah.GetSqlStringComando(query);
            dah.Comando.Parameters.Clear();
            dah.AddParameter("CodigoVigencia", codigoVigencia);

            await AñadirParametroMulticompania(dah);

            await Task.Run(() =>
            {
                dah.ProcesarDatos(dr =>
                {
                    while (dr.Read())
                    {
                        var item = new ExcepcionDto
                        {
                            CodigoCondicionMedio = dr.GetString("COD_CONDICION_MEDIO"),
                            Jerarquia = dr.GetInt32("NUM_JERARQUIA"),
                            CodigoMedio = dr.GetInt32("COD_MEDIO"),
                            DescripcionMedio = dr.GetString("DES_MEDIO"),
                            PctSAG = dr.GetNullableDecimal("PORC_SAG"),
                            PctManPower = dr.GetNullableDecimal("PORC_MNP"),
                            PctDevolucion = dr.GetNullableDecimal("PORC_DEV"),
                            CodigoAlcance = dr.GetNullableInt32("COD_ALCANCE") ?? 0,
                            CodigoDisciplina = dr.GetNullableInt32("COD_DISCIPLINA") ?? 0,
                            CodigoDiversified = dr.GetNullableInt32("COD_DIVERSIFIED") ?? 0,
                            CodigoObjetivo = dr.GetNullableInt32("COD_OBJETIVO") ?? 0,
                            CodigoTipoCompra = dr.GetNullableInt32("COD_TIPO_COMPRA") ?? 0,
                            CodigoTipoDisciplina = dr.GetNullableInt32("COD_TIPO_DISCIPLINA") ?? 0,
                            CodigoDisciplinaGrupo = dr.GetNullableInt32("COD_DISCIPLINA_GRUPO") ?? 0
                        };

                        resultado.Add(item);
                    }
                });
            });

            return resultado;
        }



        public async Task EliminarExcepcionCondicion(int codigoCondicionMedio)
        {
            var query = @"
                DELETE FROM PPT_CONDICION_MEDIO
                WHERE COD_CONDICION_MEDIO = :CodigoCondicionMedio";

            dah.GetSqlStringComando(query);
            dah.Comando.Parameters.Clear();
            dah.AddParameter("CodigoCondicionMedio", codigoCondicionMedio);

            await Task.Run(() => dah.ExecuteNonQuery());
        }


        private static string ObtenerTablaConceptosNMD (ConceptosCondicionesNMD concepto)
        {
            string? result;
          
            const string prefijo = "PPT_COND_EXC";
            result = concepto switch
            {
                ConceptosCondicionesNMD.Alcance => $"{prefijo}_ALCANCE",
                ConceptosCondicionesNMD.Disciplina => $"{prefijo}_DISCIPLINA",
                ConceptosCondicionesNMD.Diversified => $"{prefijo}_DIVERSIFIED",
                ConceptosCondicionesNMD.Objetivo => $"{prefijo}_OBJETIVO",
                ConceptosCondicionesNMD.TipoCompra => $"{prefijo}_TIPO_COMPRA",
                ConceptosCondicionesNMD.TipoDisciplina => $"{prefijo}_TIPO_DISCIPLINA",
                ConceptosCondicionesNMD.DisciplinaGrupo => $"{prefijo}_DISCIPLINA_GRUPO",
                _ => string.Empty
            };
          
            return result;
        }

        private static string ObtenerCampoConceptosNMD(ConceptosCondicionesNMD concepto)
        {
            string? result;
            const string prefijo = "COD";
            result = concepto switch
            {
                ConceptosCondicionesNMD.Alcance => $"{prefijo}_ALCANCE",
                ConceptosCondicionesNMD.Disciplina => $"{prefijo}_DISCIPLINA",
                ConceptosCondicionesNMD.Diversified => $"{prefijo}_DIVERSIFIED",
                ConceptosCondicionesNMD.Objetivo => $"{prefijo}_OBJETIVO",
                ConceptosCondicionesNMD.TipoCompra => $"{prefijo}_TIPO_COMPRA",
                ConceptosCondicionesNMD.TipoDisciplina => $"{prefijo}_TIPO_DISCIPLINA",
                ConceptosCondicionesNMD.DisciplinaGrupo => $"{prefijo}_DISCIPLINA_GRUPO",
                _ => string.Empty
            };
           
            return result;
        }

        public async Task EliminarConceptoNMDExcepcionCondicion(int codigoCondicionMedio, ConceptosCondicionesNMD concepto)
        {
            string tabla = ObtenerTablaConceptosNMD(concepto);

            var query = $@"
                DELETE FROM {tabla}
                WHERE COD_CONDICION_MEDIO = :CodigoCondicionMedio";

            dah.GetSqlStringComando(query);
            dah.Comando.Parameters.Clear();
            dah.AddParameter("CodigoCondicionMedio", codigoCondicionMedio);

            await Task.Run(() => dah.ExecuteNonQuery());
        }


        public async Task ActualizarJerarquiaExcepcion(int codigoCondicionMedio, int jerarquia)
        {
            var query = @"
                UPDATE PPT_CONDICION_MEDIO
                SET NUM_JERARQUIA = :Jerarquia,
                    COD_USUARIO_MODIFICACION = :CodigoUsuario
                WHERE COD_CONDICION_MEDIO = :CodigoCondicionMedio";

            dah.GetSqlStringComando(query);
            dah.Comando.Parameters.Clear();

            dah.AddParameter("CodigoCondicionMedio", codigoCondicionMedio);
            dah.AddParameter("Jerarquia", jerarquia);
            dah.AddParameter("CodigoUsuario", CodigoUsuario);

            await Task.Run(() => dah.ExecuteNonQuery());
        }


        public async Task GrabarConceptoNMD( int codigoCondicionMedio, ConceptosCondicionesNMD codigoConceptoNMD, int codigo)
        {
            string tabla = ObtenerTablaConceptosNMD(codigoConceptoNMD);
            string campo = ObtenerCampoConceptosNMD(codigoConceptoNMD);

            var query = $@"
                MERGE INTO {tabla} T
                USING DUAL
                ON (T.COD_CONDICION_MEDIO = :CodigoCondicionMedio)
                WHEN MATCHED THEN
                    UPDATE SET
                        {campo} = :Codigo,
                        COD_USUARIO_MODIFICACION = :CodigoUsuario
                WHEN NOT MATCHED THEN
                    INSERT (COD_CONDICION_MEDIO, {campo}, COD_USUARIO, COD_USUARIO_MODIFICACION)
                    VALUES (:CodigoCondicionMedio, :Codigo, :CodigoUsuario, :CodigoUsuarioModificacion)";

            dah.GetSqlStringComando(query);
            dah.Comando.Parameters.Clear();

            dah.AddParameter("CodigoCondicionMedio", codigoCondicionMedio);
            dah.AddParameter("Codigo", codigo);
            dah.AddParameter("CodigoUsuario", CodigoUsuario);
            dah.AddParameter("CodigoUsuarioModificacion", CodigoUsuario);

            await Task.Run(() => dah.ExecuteNonQuery());
        }





        public async Task<List<int>> ObtenerCodigosExcepcionesCondiciones(Condicion condicion)
        {
            List<int> resultado = new();

            var query = @"
                SELECT COD_CONDICION_MEDIO 
                FROM PPT_CONDICION_MEDIO
                WHERE COD_PAIS = :CodigoPais
                  AND COD_MEDIO = :CodigoMedio
                  AND COD_CONDICION_VIGENCIA = :CodigoVigencia
                  AND COD_CONCEPTO_CONDICION = :CodigoConcepto
                  AND NUM_JERARQUIA > 0";

            dah.GetSqlStringComando(query);
            dah.Comando.Parameters.Clear();

            dah.AddParameter("CodigoPais", CodigoPais);
            dah.AddParameter("CodigoMedio", condicion.CodigoMedio);
            dah.AddParameter("CodigoVigencia", condicion.CodigoVigencia);
            dah.AddParameter("CodigoConcepto", (int)condicion.CodigoConcepto);

            await AñadirParametroMulticompania(dah);

            await Task.Run(() =>
            {
                dah.ProcesarDatos((dr) =>
                {
                    while (dr.Read())
                    {
                        resultado.Add(dr.GetInt32("COD_CONDICION_MEDIO"));
                    }
                });
            });

            return resultado;
        }


        public async Task<Condicion?> ObtenerExcepcionOrCondicion(Condicion item)
        {
            Condicion? resultado = null;

            var query = @"
                SELECT COD_CONDICION_MEDIO, PCT_CONDICION_MEDIO, IND_CALCULO
                FROM PPT_CONDICION_MEDIO
                WHERE COD_PAIS = :CodigoPais
                  AND COD_MEDIO = :CodigoMedio
                  AND COD_CONDICION_VIGENCIA = :CodigoVigencia
                  AND COD_CONCEPTO_CONDICION = :CodigoConcepto
                  AND NUM_JERARQUIA = :Jerarquia";

            dah.GetSqlStringComando(query);
            dah.Comando.Parameters.Clear();

            dah.AddParameter("CodigoPais", CodigoPais);
            dah.AddParameter("CodigoMedio", item.CodigoMedio);
            dah.AddParameter("CodigoVigencia", item.CodigoVigencia);
            dah.AddParameter("CodigoConcepto", (int)item.CodigoConcepto);
            dah.AddParameter("Jerarquia", item.Jerarquia);

            await AñadirParametroMulticompania(dah);

            await Task.Run(() =>
            {
                dah.ProcesarDatos((dr) =>
                {
                    if (dr.Read())
                    {
                        resultado = new Condicion
                        {
                            CodigoCondicion = dr.GetInt32("COD_CONDICION_MEDIO"),
                            Porcentaje = dr.GetNullableDecimal("PCT_CONDICION_MEDIO"),
                            IndicadorCalculo = dr.GetInt32("IND_CALCULO"),
                        };
                    }
                });
            });

            return resultado;
        }


        public async Task<Condicion?> ObtenerExcepcionOrCondicion(int codigoCondicion)
        {
            Condicion? resultado = null;

            var query = @"
                SELECT COD_CONDICION_MEDIO, PCT_CONDICION_MEDIO, NUM_JERARQUIA
                FROM PPT_CONDICION_MEDIO
                WHERE COD_CONDICION_MEDIO = :CodigoCondicionMedio";

            dah.GetSqlStringComando(query);
            dah.Comando.Parameters.Clear();
            dah.AddParameter("CodigoCondicionMedio", codigoCondicion);

            await AñadirParametroMulticompania(dah);

            await Task.Run(() =>
            {
                dah.ProcesarDatos((dr) =>
                {
                    if (dr.Read())
                    {
                        resultado = new Condicion
                        {
                            CodigoCondicion = dr.GetInt32("COD_CONDICION_MEDIO"),
                            Porcentaje = dr.GetNullableDecimal("PCT_CONDICION_MEDIO"),
                            Jerarquia = dr.GetInt32("NUM_JERARQUIA")
                        };
                    }
                });
            });

            return resultado;
        }


        public async Task ImportarCondicionesMMS(CondicionImportarFiltro param)
        {
            dah.GetStoredProcComando("PKG_CARGA_DATOS_CONDICIONES.SET_CARGA_CONDICIONES_MMS");
            dah.Comando.CommandType = CommandType.StoredProcedure;
            dah.Comando.Parameters.Clear();

            string jsonParametros = JsonSerializer.Serialize(param);

            //IMPORTANTE: Los parametros deben ir en el mismo orden que en el procedimiento almacenado
            dah.AddParameter("pCod_Version_Destino", param.CodigoVersion);
            dah.AddParameter("p_jSonConf", jsonParametros);
            dah.AddParameter("p_Cod_Usuario", CodigoUsuario);

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
                    : "Error de BD no especificado al ejecutar 'PKG_CARGA_DATOS_CONDICIONES.SET_CARGA_CONDICIONES_MMS'";

                throw new ExcepcionBaseDatos(codigoResultado, mensajeResultado);
            }

        }

        public ITransaccion ObtenerTransaccion() => new TransaccionWrapper(base.ObtenerTransaccion());

    }
}
