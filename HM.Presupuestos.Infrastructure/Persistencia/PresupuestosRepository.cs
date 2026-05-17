using System.Text;
using HM.Core.Comun.v6.Seguridad.Interfaces;
using HM.Core.Servidor.v6.DAL.Interfaces;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Domain.Puertos;

namespace HM.Presupuestos.Infrastructure.Persistencia
{
    /// <summary>
    /// Constructor del repositorio de presupuestos
    /// </summary>
    public class PresupuestosRepository(
        IJwt jwt,
        IDataAccessHelperSecure dah) : BasePresupuestosRepository(dah,  jwt), IPresupuestosRepository
    {
        protected readonly IJwt jwt = jwt ?? throw new ArgumentNullException(nameof(jwt));
        protected new readonly IDataAccessHelperSecure dah = dah ?? throw new ArgumentNullException(nameof(dah));

        public async Task<List<CodigoDescripcion>> ObtenerTipologias()
        {
            List<CodigoDescripcion> resultado = [];

            try
            {
                StringBuilder query = new();

                query.Append("SELECT COD_TIPOLOGIA, DES_TIPOLOGIA ");
                query.Append("  FROM GRU_TIPOLOGIA ");
                query.Append("  WHERE F_BAJA IS NULL ");
                query.Append(" ORDER BY DES_TIPOLOGIA ");

                dah.GetSqlStringComando(query.ToString());

                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        while (dr.Read())
                        {
                            CodigoDescripcion item = new()
                            {
                                Codigo = dr.GetInt32("COD_TIPOLOGIA"),
                                Descripcion = dr.GetString("DES_TIPOLOGIA")
                            };

                            resultado.Add(item);
                        }
                    });
                });

            }
            catch (Exception ex)
            {
                throw new Exception("PresupuestosRepository.ObtenerTipologias", ex);
            }
            return resultado;
        }

        public async Task<List<CodigoDescripcion>> ObtenerAlcances()
        {
            List<CodigoDescripcion> resultado = [];
            try
            {
                StringBuilder query = new();

                query.Append("SELECT COD_ALCANCE, DES_ALCANCE ");
                query.Append("FROM ALCANCE ");
                query.Append("WHERE F_BAJA IS NULL ");
                query.Append("ORDER BY DECODE (COD_ALCANCE, 1,'A',DES_ALCANCE) ");

                dah.GetSqlStringComando(query.ToString());

                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        while (dr.Read())
                        {
                            CodigoDescripcion item = new()
                            {
                                Codigo = dr.GetInt32("COD_ALCANCE"),
                                Descripcion = dr.GetString("DES_ALCANCE")
                            };

                            resultado.Add(item);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("PresupuestosRepository.ObtenerAlcances", ex);
            }
            return resultado;
        }

        public async Task<List<CodigoDescripcion>> ObtenerDiversifiedsNCB()
        {
            List<CodigoDescripcion> resultado = [];

            try
            {
                StringBuilder query = new();

                query.Append("SELECT COD_DIVERSIFIED, DES_DIVERSIFIED ");
                query.Append("  FROM DIVERSIFIED ");
                query.Append("  WHERE  F_BAJA IS NULL ");
                query.Append(" ORDER BY DES_DIVERSIFIED ");

                dah.GetSqlStringComando(query.ToString());

                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        while (dr.Read())
                        {
                            CodigoDescripcion item = new()
                            {
                                Codigo = dr.GetInt32("COD_DIVERSIFIED"),
                                Descripcion = dr.GetString("DES_DIVERSIFIED")
                            };

                            resultado.Add(item);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("PresupuestosRepository.ObtenerDiversifiedsNCB", ex);
            }
            return resultado;
        }

        public async Task<List<CodigoDescripcion>> ObtenerDisciplinas()
        {
            List<CodigoDescripcion> resultado = [];

            try
            {
                StringBuilder query = new StringBuilder();

                query.Append("SELECT COD_DISCIPLINA, DES_DISCIPLINA ");
                query.Append("  FROM DISCIPLINA ");
                query.Append("  WHERE  F_BAJA IS NULL ");
                query.Append(" ORDER BY DES_DISCIPLINA ");

                dah.GetSqlStringComando(query.ToString());

                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        while (dr.Read())
                        {
                            CodigoDescripcion item = new()
                            {
                                Codigo = dr.GetInt32("COD_DISCIPLINA"),
                                Descripcion = dr.GetString("DES_DISCIPLINA")
                            };

                            resultado.Add(item);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("PresupuestosRepository.ObtenerDisciplinas", ex);
            }
            return resultado;
        }

        public async Task<List<CodigoDescripcion>> ObtenerTiposCompra()
        {
            List<CodigoDescripcion> resultado = [];

            try
            {
                StringBuilder query = new();

                query.Append("SELECT COD_TIPO_COMPRA, DES_TIPO_COMPRA ");
                query.Append("  FROM TIPO_COMPRA ");
                query.Append("  WHERE F_BAJA IS NULL ");
                query.Append(" ORDER BY DES_TIPO_COMPRA ");

                dah.GetSqlStringComando(query.ToString());

                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        while (dr.Read())
                        {
                            CodigoDescripcion item = new()
                            {
                                Codigo = dr.GetInt32("COD_TIPO_COMPRA"),
                                Descripcion = dr.GetString("DES_TIPO_COMPRA")
                            };

                            resultado.Add(item);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("PresupuestosRepository.ObtenerTiposCompra", ex);
            }

            return resultado;
        }


        public async Task<List<CodigoDescripcion>> ObtenerObjetivos()
        {
            List<CodigoDescripcion> resultado = [];

            try
            {
                StringBuilder query = new();

                query.Append("SELECT COD_OBJETIVO, DES_OBJETIVO ");
                query.Append("  FROM OBJETIVO ");
                query.Append("  WHERE F_BAJA IS NULL ");
                query.Append(" ORDER BY DES_OBJETIVO ");

                dah.GetSqlStringComando(query.ToString());

                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        while (dr.Read())
                        {
                            CodigoDescripcion item = new()
                            {
                                Codigo = dr.GetInt32("COD_OBJETIVO"),
                                Descripcion = dr.GetString("DES_OBJETIVO")
                            };

                            resultado.Add(item);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("PresupuestosRepository.ObtenerObjetivos", ex);
            }
            return resultado;
        }

        public async Task<List<CodigoDescripcion>> ObtenerMedios()
        {
            List<CodigoDescripcion> medios = [];

            try
            {
                StringBuilder query = new();

                query.Append("SELECT DISTINCT M.COD_MEDIO, M.DES_MEDIO ");
                query.Append("  FROM V_MEDIO_PPTO M, NETWORK_MEDIO N ");
                query.Append("  WHERE IND_APLICACION = 1 ");
                query.Append("    AND M.COD_MEDIO = N.COD_MEDIO ");
                query.Append(" ORDER BY COD_MEDIO ASC ");

                dah.GetSqlStringComando(query.ToString());

                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        while (dr.Read())
                        {
                            CodigoDescripcion linea = new CodigoDescripcion();
                            linea.Codigo = dr.GetInt32("COD_MEDIO");
                            linea.Descripcion = dr.GetString("DES_MEDIO");

                            medios.Add(linea);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("PresupuestosRepository.ObtenerMedios", ex);
            }
            return medios;
        }

        public async Task<List<CodigoDescripcion>> ObtenerMediosPorNetWork(string codigosNetwork)
        {
            List<CodigoDescripcion> medios = [];

            try
            {
                StringBuilder query = new();

                //query.Append("SELECT DISTINCT M.COD_MEDIO, M.DES_MEDIO ");
                //query.Append("  FROM V_MEDIO_PPTO M, NETWORK_MEDIO N ");
                //query.Append("  WHERE IND_APLICACION = 1 ");
                //query.Append("    AND M.COD_MEDIO = N.COD_MEDIO ");
                //query.Append($"    AND N.COD_NETWORK IN ({codigosNetwork}) ");
                //query.Append(" ORDER BY COD_MEDIO ASC ");


                query.Append("SELECT DISTINCT COD_MEDIO, DES_MEDIO ");
                query.Append("  FROM V_MEDIO_PPTO ");
                query.Append("  WHERE IND_APLICACION = 1 ");
                query.Append("    AND COD_MEDIO IN ( ");
                query.Append("        SELECT COD_MEDIO FROM NETWORK_MEDIO ");
                query.Append($"        WHERE COD_NETWORK IN ({codigosNetwork}) ");
                query.Append("    ) ");
                query.Append(" ORDER BY COD_MEDIO ASC ");

                dah.GetSqlStringComando(query.ToString());

                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        while (dr.Read())
                        {
                            CodigoDescripcion linea = new CodigoDescripcion();
                            linea.Codigo = dr.GetInt32("COD_MEDIO");
                            linea.Descripcion = dr.GetString("DES_MEDIO");

                            medios.Add(linea);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("PresupuestosRepository.ObtenerMedios", ex);
            }
            return medios;
        }

        public async Task<List<CodigoDescripcion>> ObtenerNetworks()
        {
            List<CodigoDescripcion> networks = [];

            try
            {
                StringBuilder query = new();

                query.Append("SELECT COD_NETWORK, DES_NETWORK ");
                query.Append("  FROM NETWORK ");
                query.Append("  WHERE F_BAJA IS NULL ");
                query.Append(" ORDER BY DES_NETWORK ");

                dah.GetSqlStringComando(query.ToString());

                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        while (dr.Read())
                        {
                            CodigoDescripcion linea = new();
                            linea.Codigo = dr.GetInt32("COD_NETWORK");
                            linea.Descripcion = dr.GetString("DES_NETWORK");

                            networks.Add(linea);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("PresupuestosRepository.ObtenerNetworks", ex);
            }
            return networks;
        }

        public async Task<List<CodigoDescripcion>> ObtenerGruposClientes()
        {
            List<CodigoDescripcion> grupos = [];

            try
            {
                StringBuilder query = new();

                query.Append("SELECT COD_GRUPO, DES_GRUPO ");
                query.Append("  FROM V_ENTORNO_COMPANIA_GRUPO ");
                query.Append("GROUP BY COD_GRUPO,DES_GRUPO ");
                query.Append("ORDER BY DES_GRUPO");

                dah.GetSqlStringComando(query.ToString());

                await AñadirParametroMulticompania(dah);
  
                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        while (dr.Read())
                        {
                            CodigoDescripcion linea = new();
                            linea.Codigo = dr.GetInt32("COD_GRUPO");
                            linea.Descripcion = dr.GetString("DES_GRUPO");

                            grupos.Add(linea);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("PresupuestosRepository.ObtenerGruposClientes", ex);
            }
            return grupos;
        }


        public async Task<List<CodigoDescripcion>> ObtenerGruposClientePorNetworks(string codigosNetworks)
        {
            List<CodigoDescripcion> grupos = [];

            try
            {
                StringBuilder query = new();

                query.Append("SELECT DISTINCT COD_GRUPO, DES_GRUPO ");
                query.Append("FROM V_ENTORNO_COMPANIA_GRUPO ");
                query.Append($"WHERE  COD_NETWORK IN ({codigosNetworks}) ");
                query.Append("GROUP BY COD_GRUPO,DES_GRUPO ");
                query.Append("ORDER BY DES_GRUPO");

                dah.GetSqlStringComando(query.ToString());

                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        while (dr.Read())
                        {
                            CodigoDescripcion linea = new();
                            linea.Codigo = dr.GetInt32("COD_GRUPO");
                            linea.Descripcion = dr.GetString("DES_GRUPO");

                            grupos.Add(linea);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("PresupuestosRepository.ObtenerGruposClientePorNetwork", ex);
            }
            return grupos;
        }

        public async Task<List<GrupoClientesConNetwork>> ObtenerGruposClientesConNetwork()
        {
            List<GrupoClientesConNetwork> grupos = [];
            try
            {
                StringBuilder query = new();

                query.Append("SELECT COD_GRUPO, DES_GRUPO, COD_NETWORK ");
                query.Append("  FROM V_ENTORNO_COMPANIA_GRUPO ");
                query.Append("GROUP BY COD_GRUPO,DES_GRUPO, COD_NETWORK ");
                query.Append("ORDER BY DES_GRUPO");

                dah.GetSqlStringComando(query.ToString());

                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        while (dr.Read())
                        {
                            GrupoClientesConNetwork linea = new();
                            linea.Codigo = dr.GetInt32("COD_GRUPO");
                            linea.Descripcion = dr.GetString("DES_GRUPO");
                            linea.CodigoNetwork = dr.GetInt32("COD_NETWORK");

                            grupos.Add(linea);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("PresupuestosRepository.ObtenerGruposClientesConNetwork", ex);
            }
            return grupos;
        }

        public async Task<List<CodigoDescripcion>> ObtenerEditoriales(FiltroEditoriales? filtro = null)
        {
            List<CodigoDescripcion> lista = [];

            try
            {
                string query = @"
                SELECT DISTINCT COD_EDITORIAL_COMERCIAL, DES_EDITORIAL_COMERCIAL 
                FROM V_SOPORTE 
                WHERE 1=1";

                if (filtro != null)
                {
                    if (!string.IsNullOrEmpty(filtro.CodigosMedios))
                    {
                        query += $" AND COD_MEDIO IN ({filtro.CodigosMedios})";
                    }
                    if (!string.IsNullOrEmpty(filtro.CodigosAgrupacionesComerciales))
                    {
                        query += $" AND COD_AGRUPACION_COMERCIAL IN ({filtro.CodigosAgrupacionesComerciales})";
                    }
                }
                query += " ORDER BY DES_EDITORIAL_COMERCIAL";

                dah.GetSqlStringComando(query.ToString());

                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        while (dr.Read())
                        {
                            CodigoDescripcion linea = new();
                            linea.Codigo = dr.GetInt32("COD_EDITORIAL_COMERCIAL");
                            linea.Descripcion = dr.GetString("DES_EDITORIAL_COMERCIAL");

                            lista.Add(linea);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("PresupuestosRepository.ObtenerEditoriales", ex);
            }
            return lista;
        }
        public async Task<List<AgrupacionComercialConMedio>> ObtenerAgrupacionesComercialesConMedio()
        {
            List<AgrupacionComercialConMedio> lista = [];
            const string query = @"
            SELECT COD_AGRUPACION_COMERCIAL, DES_AGRUPACION_COMERCIAL , COD_MEDIO
            FROM V_SOPORTE 
            WHERE COD_AGRUPACION_COMERCIAL IS NOT NULL
            ORDER BY DES_AGRUPACION_COMERCIAL";

            dah.GetSqlStringComando(query);

            await AñadirParametroMulticompania(dah);

            await Task.Run(() =>
            {
                dah.ProcesarDatos((dr) =>
                {
                    while (dr.Read())
                    {
                        AgrupacionComercialConMedio linea = new();
                        linea.Codigo = dr.GetInt32("COD_AGRUPACION_COMERCIAL");
                        linea.Descripcion = dr.GetString("DES_AGRUPACION_COMERCIAL");
                        linea.CodigoMedio = dr.GetInt32("COD_MEDIO");
                        lista.Add(linea);
                    }
                });
            });

            return lista;
        }


        public async Task<List<CodigoDescripcion>> ObtenerAgrupacionesComerciales(string? codigosMedios = null)
        {
            List<CodigoDescripcion> lista = [];

            var query = new StringBuilder(@"
                SELECT DISTINCT COD_AGRUPACION_COMERCIAL, DES_AGRUPACION_COMERCIAL
                FROM V_SOPORTE
                WHERE COD_AGRUPACION_COMERCIAL IS NOT NULL
            ");

            if (!string.IsNullOrEmpty(codigosMedios))
            {
                query.Append($" AND COD_MEDIO IN ({codigosMedios})");
            }

            query.Append(" ORDER BY DES_AGRUPACION_COMERCIAL");

            dah.GetSqlStringComando(query.ToString());

            await AñadirParametroMulticompania(dah);

            await Task.Run(() =>
            {
                dah.ProcesarDatos((dr) =>
                {
                    while (dr.Read())
                    {
                        CodigoDescripcion linea = new();
                        linea.Codigo = dr.GetInt32("COD_AGRUPACION_COMERCIAL");
                        linea.Descripcion = dr.GetString("DES_AGRUPACION_COMERCIAL");

                        lista.Add(linea);
                    }
                });
            });

            return lista;
        }

        public async Task<List<CodigoDescripcion>> ObtenerEditorialesPorAgrupacionComercial(int codigoAgrupacionComercial)
        {
            List<CodigoDescripcion> resultado = [];

            const string query = @"
                SELECT COD_EDITORIAL_COMERCIAL, DES_EDITORIAL_COMERCIAL 
                FROM V_SOPORTE 
                WHERE COD_AGRUPACION_COMERCIAL = :CodigoAgrupacionEditorial 
                GROUP BY COD_EDITORIAL_COMERCIAL, DES_EDITORIAL_COMERCIAL 
                ORDER BY DES_EDITORIAL_COMERCIAL";

            dah.GetSqlStringComando(query);
            
            dah.AddParameter("CodigoAgrupacionEditorial", codigoAgrupacionComercial);
            await AñadirParametroMulticompania(dah);

            await Task.Run(() =>
            {
                dah.ProcesarDatos((dr) =>
                {
                    while (dr.Read())
                    {
                        CodigoDescripcion item = new();
                        item.Codigo = dr.IsDBNull(dr.GetOrdinal("COD_EDITORIAL_COMERCIAL"))
                                ? 0
                                : dr.GetInt32(dr.GetOrdinal("COD_EDITORIAL_COMERCIAL"));

                        item.Descripcion = dr.IsDBNull(dr.GetOrdinal("DES_EDITORIAL_COMERCIAL"))
                            ? string.Empty
                            : dr.GetString(dr.GetOrdinal("DES_EDITORIAL_COMERCIAL"));

                        if (item.Codigo > 0 && !String.IsNullOrEmpty(item.Descripcion))
                        {
                            resultado.Add(item);
                        }
                    }
                });
            });
            return resultado;
        }

        public async Task<List<CodigoDescripcion>> ObtenerEditorialesPorAgrupacionComercialAndMedio(int codigoAgrupacionComercial, int codigoMedio)
        {
            List<CodigoDescripcion> resultado = [];

            const string query = @"
                SELECT COD_EDITORIAL_COMERCIAL, DES_EDITORIAL_COMERCIAL 
                FROM V_SOPORTE 
                WHERE COD_AGRUPACION_COMERCIAL = :CodigoAgrupacionEditorial 
                    AND COD_MEDIO = :CodigoMedio
                GROUP BY COD_EDITORIAL_COMERCIAL, DES_EDITORIAL_COMERCIAL 
                ORDER BY DES_EDITORIAL_COMERCIAL";

            dah.GetSqlStringComando(query);
            dah.AddParameter("CodigoAgrupacionEditorial", codigoAgrupacionComercial);
            dah.AddParameter("CodigoMedio", codigoMedio);

            await AñadirParametroMulticompania(dah);

            await Task.Run(() =>
            {
                dah.ProcesarDatos((dr) =>
                {
                    while (dr.Read())
                    {
                        CodigoDescripcion item = new();
                        item.Codigo = dr.IsDBNull(dr.GetOrdinal("COD_EDITORIAL_COMERCIAL"))
                                ? 0
                                : dr.GetInt32(dr.GetOrdinal("COD_EDITORIAL_COMERCIAL"));

                        item.Descripcion = dr.IsDBNull(dr.GetOrdinal("DES_EDITORIAL_COMERCIAL"))
                            ? string.Empty
                            : dr.GetString(dr.GetOrdinal("DES_EDITORIAL_COMERCIAL"));

                        if (item.Codigo > 0 && !String.IsNullOrEmpty(item.Descripcion))
                        {
                            resultado.Add(item);
                        }
                    }
                });
            });
            return resultado;
        }

        

        public async Task<List<EditorialConAgrupacionComercialAndMedio>> ObtenerEditorialesConAgrupacionComercialAndMedio()
        {
            List<EditorialConAgrupacionComercialAndMedio> resultado = [];

            const string query = @"
            SELECT COD_EDITORIAL_COMERCIAL, DES_EDITORIAL_COMERCIAL, COD_MEDIO, COD_AGRUPACION_COMERCIAL
            FROM V_SOPORTE 
            ORDER BY DES_EDITORIAL_COMERCIAL";

            dah.GetSqlStringComando(query.ToString());

            await AñadirParametroMulticompania(dah);

            await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        while (dr.Read())
                        {
                            EditorialConAgrupacionComercialAndMedio linea = new();
                            linea.Codigo = dr.GetInt32("COD_EDITORIAL_COMERCIAL");
                            linea.Descripcion = dr.GetString("DES_EDITORIAL_COMERCIAL");
                            linea.CodigoAgrupacionComercial = dr.GetNullableInt32("COD_AGRUPACION_COMERCIAL");
                            linea.CodigoMedio = dr.GetInt32("COD_MEDIO");
                            resultado.Add(linea);
                        }
                    });
                });
            return resultado;
        }




       
        public async Task<List<CodigoDescripcion>> ObtenerTiposDisciplina()
        {
            List<CodigoDescripcion> resultado = [];

            try
            {
                string query = @"
                SELECT COD_TIPO_DISCIPLINA, DES_TIPO_DISCIPLINA
                FROM TIPO_DISCIPLINA
                ORDER BY DES_TIPO_DISCIPLINA";

                dah.GetSqlStringComando(query);

                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        while (dr.Read())
                        {
                            CodigoDescripcion item = new()
                            {
                                Codigo = dr.GetInt32("COD_TIPO_DISCIPLINA"),
                                Descripcion = dr.GetString("DES_TIPO_DISCIPLINA")
                            };

                            resultado.Add(item);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("PresupuestosRepository.ObtenerTiposDisciplina", ex);
            }
            return resultado;
        }




        public async Task<List<CodigoDescripcion>> ObtenerDisciplinasGrupos()
        {
            List<CodigoDescripcion> resultado = [];

            try
            {
                string query = @"
                    SELECT COD_DISCIPLINA_GRUPO, DES_DISCIPLINA_GRUPO
                    FROM DISCIPLINA_GRUPO
                    WHERE F_BAJA IS NULL
                    ORDER BY DES_DISCIPLINA_GRUPO";

                dah.GetSqlStringComando(query);

                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        while (dr.Read())
                        {
                            CodigoDescripcion item = new()
                            {
                                Codigo = dr.GetInt32("COD_DISCIPLINA_GRUPO"),
                                Descripcion = dr.GetString("DES_DISCIPLINA_GRUPO")
                            };

                            resultado.Add(item);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("PresupuestosRepository.ObtenerDisciplinasGrupos", ex);
            }
            return resultado;
        }


        /// <summary>
        /// Obtiene lista de meses cerrados
        /// </summary>
        /// <param name="year">Año de filtro</param>
        /// <returns>Lista de int</returns>
        public async Task<List<int>> ObtenerMesCerradoList(int year)
        {
            List<int> resultado = [];

            try
            {
                string query = @"
                    SELECT MES
                    FROM PPT_MESES_CERRADOS
                    WHERE ANIO= :Year";

                dah.GetSqlStringComando(query);

                dah.AddParameter("Year", year);

                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        while (dr.Read())
                        {
                            resultado.Add(dr.GetInt32("MES"));
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("PresupuestosRepository.ObtenerMesCerradoList", ex);
            }
            return resultado;
        }


        /// <summary>
        /// Obtiene lista agrupaciones editoriales de un medio
        /// </summary>
        /// <param name="codeMedio">Código de medio</param>
        /// <returns>Lista de objeto CodigoDescripcion</returns>
        public async Task<List<CodigoDescripcion>> GetAgrupacionEditorialListByMedio(int codeMedio)
        {
            List<CodigoDescripcion> result = [];
            try
            {
                StringBuilder query = new();

                query.Append("SELECT COD_AGRUPACION_COMERCIAL, DES_AGRUPACION_COMERCIAL ");
                query.Append("FROM V_SOPORTE ");
                query.Append("WHERE COD_MEDIO = :codeMedio ");
                query.Append("GROUP BY COD_AGRUPACION_COMERCIAL, DES_AGRUPACION_COMERCIAL ");
                query.Append("ORDER BY DES_AGRUPACION_COMERCIAL");

                dah.GetSqlStringComando(query.ToString());
                dah.AddParameter("codeMedio", codeMedio);

                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        while (dr.Read())
                        {
                            CodigoDescripcion item = new();

                            item.Codigo = dr.IsDBNull(dr.GetOrdinal("COD_AGRUPACION_COMERCIAL"))
                                    ? 0 
                                    : dr.GetInt32(dr.GetOrdinal("COD_AGRUPACION_COMERCIAL"));

                            item.Descripcion = dr.IsDBNull(dr.GetOrdinal("DES_AGRUPACION_COMERCIAL"))
                                ? string.Empty 
                                : dr.GetString(dr.GetOrdinal("DES_AGRUPACION_COMERCIAL"));

                            if (item.Codigo>0 && !String.IsNullOrEmpty(item.Descripcion))
                                result.Add(item);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("PresupuestosRepository.GetAgrupacionEditorialListByMedio", ex);
            }
            return result;
        }



        /// <summary>
        /// Obtiene lista editoriales de un medio
        /// </summary>
        /// <param name="codeMedio">Código de medio</param>
        /// <returns>Lista de objeto CodigoDescripcion</returns>
        public async Task<List<CodigoDescripcion>> GetEditorialListByMedio(int codeMedio)
        {
            List<CodigoDescripcion> result = [];
            try
            {
                StringBuilder query = new();

                query.Append("SELECT COD_EDITORIAL_COMERCIAL, DES_EDITORIAL_COMERCIAL ");
                query.Append("FROM V_SOPORTE ");
                query.Append("WHERE COD_MEDIO = :codeMedio ");
                query.Append("GROUP BY COD_EDITORIAL_COMERCIAL, DES_EDITORIAL_COMERCIAL ");
                query.Append("ORDER BY DES_EDITORIAL_COMERCIAL");

                dah.GetSqlStringComando(query.ToString());
                dah.AddParameter("codeMedio", codeMedio);

                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        while (dr.Read())
                        {
                            CodigoDescripcion item = new();
                            item.Codigo = dr.IsDBNull(dr.GetOrdinal("COD_EDITORIAL_COMERCIAL"))
                                    ? 0
                                    : dr.GetInt32(dr.GetOrdinal("COD_EDITORIAL_COMERCIAL"));

                            item.Descripcion = dr.IsDBNull(dr.GetOrdinal("DES_EDITORIAL_COMERCIAL"))
                                ? string.Empty
                                : dr.GetString(dr.GetOrdinal("DES_EDITORIAL_COMERCIAL"));

                            if (item.Codigo > 0 && !String.IsNullOrEmpty(item.Descripcion))
                                result.Add(item);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("PresupuestosRepository.GetEditorialListByMedio", ex);
            }
            return result;
        }



        /// <summary>
        /// Obtiene lista editoriales de un medio
        /// </summary>
        /// <param name="codeAgrupacionEditorial">Código de Agrupación Editorial</param>
        /// <returns>Lista de objeto CodigoDescripcion</returns>
        public async Task<List<CodigoDescripcion>> GetEditorialListByAgrupacionEditorial(int codeAgrupacionEditorial)
        {
            List<CodigoDescripcion> result = [];
            try
            {
                StringBuilder query = new();

                query.Append("SELECT COD_EDITORIAL_COMERCIAL, DES_EDITORIAL_COMERCIAL ");
                query.Append("FROM V_SOPORTE ");
                query.Append("WHERE COD_AGRUPACION_COMERCIAL = :codeAgrupacionEditorial ");
                query.Append("GROUP BY COD_EDITORIAL_COMERCIAL, DES_EDITORIAL_COMERCIAL ");
                query.Append("ORDER BY DES_EDITORIAL_COMERCIAL");

                dah.GetSqlStringComando(query.ToString());
                dah.AddParameter("codeAgrupacionEditorial", codeAgrupacionEditorial);

                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        while (dr.Read())
                        {
                            CodigoDescripcion item = new();
                            item.Codigo = dr.IsDBNull(dr.GetOrdinal("COD_EDITORIAL_COMERCIAL"))
                                    ? 0
                                    : dr.GetInt32(dr.GetOrdinal("COD_EDITORIAL_COMERCIAL"));

                            item.Descripcion = dr.IsDBNull(dr.GetOrdinal("DES_EDITORIAL_COMERCIAL"))
                                ? string.Empty
                                : dr.GetString(dr.GetOrdinal("DES_EDITORIAL_COMERCIAL"));

                            if (item.Codigo > 0 && !String.IsNullOrEmpty(item.Descripcion))
                                result.Add(item);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("PresupuestosRepository.GetEditorialListByMedio", ex);
            }
            return result;
        }


        public async Task<CodigoDescripcion?> ObtenerMedio(int codigoMedio)
        {
            CodigoDescripcion? resultado = null;

            string query = @"
                SELECT M.COD_MEDIO, M.DES_MEDIO 
                FROM V_MEDIO_PPTO M
                WHERE IND_APLICACION = 1 
                    AND M.COD_MEDIO = :CodigoMedio ";

            dah.GetSqlStringComando(query.ToString());

            dah.AddParameter("CodigoMedio", codigoMedio);

            await AñadirParametroMulticompania(dah);

            await Task.Run(() =>
            {
                dah.ProcesarDatos((dr) =>
                {
                    while (dr.Read())
                    {
                        resultado = new CodigoDescripcion();
                        resultado.Codigo = dr.GetInt32("COD_MEDIO");
                        resultado.Descripcion = dr.GetString("DES_MEDIO");

                    }
                });
            });
           
            return resultado;
        }

        public async Task<CodigoDescripcion?> ObtenerAgrupacionComercial(int codigoAgrupacionComercial)
        {
            CodigoDescripcion? resultado = null;

            string query = @"
                SELECT COD_AGRUPACION_COMERCIAL, DES_AGRUPACION_COMERCIAL
                FROM V_SOPORTE
                WHERE COD_AGRUPACION_COMERCIAL = :codigoAgrupacionComercial";

            dah.GetSqlStringComando(query);
            dah.AddParameter("codigoAgrupacionComercial", codigoAgrupacionComercial);

            await AñadirParametroMulticompania(dah);

            await Task.Run(() =>
            {
                dah.ProcesarDatos((dr) =>
                {
                    while (dr.Read())
                    {
                        resultado = new();
                        resultado.Codigo =  dr.GetInt32(dr.GetOrdinal("COD_AGRUPACION_COMERCIAL"));
                        resultado.Descripcion = dr.GetString(dr.GetOrdinal("DES_AGRUPACION_COMERCIAL"));
                    }
                });
            });
            return resultado;
        }

        public async Task<CodigoDescripcion?> ObtenerEditorial(int codigoEditorial)
        {
            CodigoDescripcion? resultado = null;
            try
            {
                string query = @"
                    SELECT COD_EDITORIAL_COMERCIAL, DES_EDITORIAL_COMERCIAL 
                    FROM V_SOPORTE 
                    WHERE COD_EDITORIAL_COMERCIAL = :CodigoEditorial ";
              

                dah.GetSqlStringComando(query.ToString());
                dah.AddParameter("CodigoEditorial", codigoEditorial);

                await AñadirParametroMulticompania(dah);

                await Task.Run(() =>
                {
                    dah.ProcesarDatos((dr) =>
                    {
                        while (dr.Read())
                        {
                            resultado = new();
                            resultado.Codigo = dr.GetInt32(dr.GetOrdinal("COD_EDITORIAL_COMERCIAL"));
                            resultado.Descripcion = dr.GetString(dr.GetOrdinal("DES_EDITORIAL_COMERCIAL"));
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                throw new Exception("PresupuestosRepository.GetEditorialListByMedio", ex);
            }
            return resultado;
        }

        public async Task<List<CodigoDescripcion>> ObtenerConceptosNMD(int codigoMedio, ConceptosCondicionesNMD concepto, ValoresConceptosNMD valores)
        {
            List<CodigoDescripcion> resultado = [];


            // Campos según el concepto solicitado
            string campos = concepto switch
            {
                ConceptosCondicionesNMD.Disciplina => "COD_DISCIPLINA CODIGO, DES_DISCIPLINA DESCRIPCION",
                ConceptosCondicionesNMD.Objetivo => "COD_OBJETIVO CODIGO, DES_OBJETIVO DESCRIPCION",
                ConceptosCondicionesNMD.TipoCompra => "COD_TIPO_COMPRA CODIGO, DES_TIPO_COMPRA DESCRIPCION",
                ConceptosCondicionesNMD.TipoDisciplina => "COD_TIPO_DISCIPLINA CODIGO, DES_TIPO_DISCIPLINA DESCRIPCION",
                ConceptosCondicionesNMD.DisciplinaGrupo => "COD_DISCIPLINA_GRUPO CODIGO, DES_DISCIPLINA_GRUPO DESCRIPCION",
                _ => throw new ArgumentOutOfRangeException(nameof(concepto))
            };

            // Construcción dinámica del WHERE
            var filtros = new List<string>
            {
                "COD_MEDIO = :codigoMedio"
            };

            if (concepto != ConceptosCondicionesNMD.Disciplina)
                filtros.Add("COD_DISCIPLINA = NVL(:codigoDisciplina, COD_DISCIPLINA)");
            if (concepto != ConceptosCondicionesNMD.Objetivo)
                filtros.Add("COD_OBJETIVO = NVL(:codigoObjetivo, COD_OBJETIVO)");
            if (concepto != ConceptosCondicionesNMD.TipoCompra)
                filtros.Add("COD_TIPO_COMPRA = NVL(:codigoTipoCompra, COD_TIPO_COMPRA)");
            if (concepto != ConceptosCondicionesNMD.TipoDisciplina)
                filtros.Add("COD_TIPO_DISCIPLINA = NVL(:codigoTipoDisciplina, COD_TIPO_DISCIPLINA)");
            if (concepto != ConceptosCondicionesNMD.DisciplinaGrupo)
                filtros.Add("COD_DISCIPLINA_GRUPO = NVL(:codigoDisciplinaGrupo, COD_DISCIPLINA_GRUPO)");

            string where = string.Join(" AND ", filtros);

            string query = $@"
                SELECT DISTINCT {campos}
                FROM V_NMD
                WHERE {where}";

            dah.GetSqlStringComando(query);

            dah.AddParameter("codigoMedio", codigoMedio);

            if (concepto != ConceptosCondicionesNMD.Disciplina)
                dah.AddParameter("codigoDisciplina", valores.CodigoDisciplina);
            if (concepto != ConceptosCondicionesNMD.Objetivo)
                dah.AddParameter("codigoObjetivo", valores.CodigoObjetivo); 
            if (concepto != ConceptosCondicionesNMD.TipoCompra)
                dah.AddParameter("codigoTipoCompra", valores.CodigoTipoCompra);
            if (concepto != ConceptosCondicionesNMD.TipoDisciplina)
                dah.AddParameter("codigoTipoDisciplina", valores.CodigoTipoDisciplina);
            if (concepto != ConceptosCondicionesNMD.DisciplinaGrupo)
                dah.AddParameter("codigoDisciplinaGrupo", valores.CodigoDisciplinaGrupo);

            await AñadirParametroMulticompania(dah);

            await Task.Run(() =>
            {
                dah.ProcesarDatos(dr =>
                {
                    while (dr.Read())
                    {
                        resultado.Add(new CodigoDescripcion
                        {
                            Codigo = dr.GetInt32("CODIGO"),
                            Descripcion = dr.GetString("DESCRIPCION")
                        });
                    }
                });
            });
           

            return resultado;
        }

       

}
}

