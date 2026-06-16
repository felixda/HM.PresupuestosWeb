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
                const string query = @"
                    SELECT COD_TIPOLOGIA, DES_TIPOLOGIA
                      FROM GRU_TIPOLOGIA
                     WHERE F_BAJA IS NULL
                     ORDER BY DES_TIPOLOGIA";

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
                const string query = @"
                    SELECT COD_ALCANCE, DES_ALCANCE
                      FROM ALCANCE
                     WHERE F_BAJA IS NULL
                     ORDER BY DECODE(COD_ALCANCE, 1,'A',DES_ALCANCE)";

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
                const string query = @"
                    SELECT COD_DIVERSIFIED, DES_DIVERSIFIED
                      FROM DIVERSIFIED
                     WHERE F_BAJA IS NULL
                     ORDER BY DES_DIVERSIFIED";

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
                const string query = @"
                    SELECT COD_DISCIPLINA, DES_DISCIPLINA
                      FROM DISCIPLINA
                     WHERE F_BAJA IS NULL
                     ORDER BY DES_DISCIPLINA";

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
                const string query = @"
                    SELECT COD_TIPO_COMPRA, DES_TIPO_COMPRA
                      FROM TIPO_COMPRA
                     WHERE F_BAJA IS NULL
                     ORDER BY DES_TIPO_COMPRA";

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
                const string query = @"
                    SELECT COD_OBJETIVO, DES_OBJETIVO
                      FROM OBJETIVO
                     WHERE F_BAJA IS NULL
                     ORDER BY DES_OBJETIVO";

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
                const string query = @"
                    SELECT DISTINCT M.COD_MEDIO, M.DES_MEDIO
                      FROM V_MEDIO_PPTO M, NETWORK_MEDIO N
                     WHERE IND_APLICACION = 1
                       AND M.COD_MEDIO = N.COD_MEDIO
                     ORDER BY COD_MEDIO ASC";

                dah.GetSqlStringComando(query);

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
                string query = $@"
                    SELECT DISTINCT COD_MEDIO, DES_MEDIO
                      FROM V_MEDIO_PPTO
                     WHERE IND_APLICACION = 1
                       AND COD_MEDIO IN (
                               SELECT COD_MEDIO FROM NETWORK_MEDIO
                               WHERE COD_NETWORK IN ({codigosNetwork})
                           )
                     ORDER BY COD_MEDIO ASC";

                dah.GetSqlStringComando(query);

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
                const string query = @"
                    SELECT COD_NETWORK, DES_NETWORK
                      FROM NETWORK
                     WHERE F_BAJA IS NULL
                     ORDER BY DES_NETWORK";

                dah.GetSqlStringComando(query);

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
                const string query = @"
                    SELECT COD_GRUPO, DES_GRUPO
                      FROM V_ENTORNO_COMPANIA_GRUPO
                     GROUP BY COD_GRUPO, DES_GRUPO
                     ORDER BY DES_GRUPO";

                dah.GetSqlStringComando(query);

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
                string query = $@"
                    SELECT DISTINCT COD_GRUPO, DES_GRUPO
                      FROM V_ENTORNO_COMPANIA_GRUPO
                     WHERE COD_NETWORK IN ({codigosNetworks})
                     GROUP BY COD_GRUPO, DES_GRUPO
                     ORDER BY DES_GRUPO";

                dah.GetSqlStringComando(query);

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
                const string query = @"
                    SELECT COD_GRUPO, DES_GRUPO, COD_NETWORK
                      FROM V_ENTORNO_COMPANIA_GRUPO
                     GROUP BY COD_GRUPO, DES_GRUPO, COD_NETWORK
                     ORDER BY DES_GRUPO";

                dah.GetSqlStringComando(query);

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
                string query = $@"
                SELECT DISTINCT COD_EDITORIAL_COMERCIAL, DES_EDITORIAL_COMERCIAL 
                FROM V_SOPORTE 
                WHERE 1=1
                {(filtro != null && !string.IsNullOrEmpty(filtro.CodigosMedios)                  ? $"AND COD_MEDIO IN ({filtro.CodigosMedios})"                                  : "")}
                {(filtro != null && !string.IsNullOrEmpty(filtro.CodigosAgrupacionesComerciales) ? $"AND COD_AGRUPACION_COMERCIAL IN ({filtro.CodigosAgrupacionesComerciales})" : "")}
                ORDER BY DES_EDITORIAL_COMERCIAL";

                dah.GetSqlStringComando(query);

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

            string query = $@"
                SELECT DISTINCT COD_AGRUPACION_COMERCIAL, DES_AGRUPACION_COMERCIAL
                FROM V_SOPORTE
                WHERE COD_AGRUPACION_COMERCIAL IS NOT NULL
                {(!string.IsNullOrEmpty(codigosMedios) ? $"AND COD_MEDIO IN ({codigosMedios})" : "")}
                ORDER BY DES_AGRUPACION_COMERCIAL";

            dah.GetSqlStringComando(query);

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

        public async Task<(List<CodigoDescripcion> Agrupaciones, List<CodigoDescripcion> Editoriales)> ObtenerAgrupacionesYEditoriales(string codigosMedios)
        {
            var agrupacionesSet = new Dictionary<int, CodigoDescripcion>();
            var editorialesSet = new Dictionary<int, CodigoDescripcion>();

            string query = $@"
                SELECT DISTINCT
                    COD_AGRUPACION_COMERCIAL, DES_AGRUPACION_COMERCIAL,
                    COD_EDITORIAL_COMERCIAL, DES_EDITORIAL_COMERCIAL
                FROM V_SOPORTE
                WHERE COD_AGRUPACION_COMERCIAL IS NOT NULL
                {(!string.IsNullOrEmpty(codigosMedios) ? $"AND COD_MEDIO IN ({codigosMedios})" : "")}
                ORDER BY DES_AGRUPACION_COMERCIAL, DES_EDITORIAL_COMERCIAL";

            dah.GetSqlStringComando(query);

            await AñadirParametroMulticompania(dah);

            await Task.Run(() =>
            {
                dah.ProcesarDatos((dr) =>
                {
                    while (dr.Read())
                    {
                        int codAgrup = dr.GetInt32("COD_AGRUPACION_COMERCIAL");
                        if (!agrupacionesSet.ContainsKey(codAgrup))
                        {
                            agrupacionesSet[codAgrup] = new CodigoDescripcion
                            {
                                Codigo = codAgrup,
                                Descripcion = dr.GetString("DES_AGRUPACION_COMERCIAL")
                            };
                        }

                        int codEdit = dr.GetInt32("COD_EDITORIAL_COMERCIAL");
                        if (!editorialesSet.ContainsKey(codEdit))
                        {
                            editorialesSet[codEdit] = new CodigoDescripcion
                            {
                                Codigo = codEdit,
                                Descripcion = dr.GetString("DES_EDITORIAL_COMERCIAL")
                            };
                        }
                    }
                });
            });

            return (
                [.. agrupacionesSet.Values.OrderBy(x => x.Descripcion)],
                [.. editorialesSet.Values.OrderBy(x => x.Descripcion)]
            );
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
                const string query = @"
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
                const string query = @"
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
                const string query = @"
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
                const string query = @"
                    SELECT COD_AGRUPACION_COMERCIAL, DES_AGRUPACION_COMERCIAL
                      FROM V_SOPORTE
                     WHERE COD_MEDIO = :CodigoMedio
                     GROUP BY COD_AGRUPACION_COMERCIAL, DES_AGRUPACION_COMERCIAL
                     ORDER BY DES_AGRUPACION_COMERCIAL";

                dah.GetSqlStringComando(query);
                dah.AddParameter("CodigoMedio", codeMedio);

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
                const string query = @"
                    SELECT COD_EDITORIAL_COMERCIAL, DES_EDITORIAL_COMERCIAL
                      FROM V_SOPORTE
                     WHERE COD_MEDIO = :CodigoMedio
                     GROUP BY COD_EDITORIAL_COMERCIAL, DES_EDITORIAL_COMERCIAL
                     ORDER BY DES_EDITORIAL_COMERCIAL";

                dah.GetSqlStringComando(query);
                dah.AddParameter("CodigoMedio", codeMedio);

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
                const string query = @"
                    SELECT COD_EDITORIAL_COMERCIAL, DES_EDITORIAL_COMERCIAL
                      FROM V_SOPORTE
                     WHERE COD_AGRUPACION_COMERCIAL = :CodigoAgrupacionEditorial
                     GROUP BY COD_EDITORIAL_COMERCIAL, DES_EDITORIAL_COMERCIAL
                     ORDER BY DES_EDITORIAL_COMERCIAL";

                dah.GetSqlStringComando(query);
                dah.AddParameter("CodigoAgrupacionEditorial", codeAgrupacionEditorial);

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

            const string query = @"
                SELECT M.COD_MEDIO, M.DES_MEDIO 
                FROM V_MEDIO_PPTO M
                WHERE IND_APLICACION = 1 
                    AND M.COD_MEDIO = :CodigoMedio ";

            dah.GetSqlStringComando(query);

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

            const string query = @"
                SELECT COD_AGRUPACION_COMERCIAL, DES_AGRUPACION_COMERCIAL
                FROM V_SOPORTE
                WHERE COD_AGRUPACION_COMERCIAL = :CodigoAgrupacionComercial";

            dah.GetSqlStringComando(query);
            dah.AddParameter("CodigoAgrupacionComercial", codigoAgrupacionComercial);

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
                const string query = @"
                    SELECT COD_EDITORIAL_COMERCIAL, DES_EDITORIAL_COMERCIAL 
                    FROM V_SOPORTE 
                    WHERE COD_EDITORIAL_COMERCIAL = :CodigoEditorial ";
              

                dah.GetSqlStringComando(query);
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

            string query = $@"
                SELECT DISTINCT {campos}
                FROM V_NMD
                WHERE COD_MEDIO = :CodigoMedio
                {(concepto != ConceptosCondicionesNMD.Disciplina      ? "AND COD_DISCIPLINA = NVL(:CodigoDisciplina, COD_DISCIPLINA)"             : "")}
                {(concepto != ConceptosCondicionesNMD.Objetivo         ? "AND COD_OBJETIVO = NVL(:CodigoObjetivo, COD_OBJETIVO)"                 : "")}
                {(concepto != ConceptosCondicionesNMD.TipoCompra       ? "AND COD_TIPO_COMPRA = NVL(:CodigoTipoCompra, COD_TIPO_COMPRA)"         : "")}
                {(concepto != ConceptosCondicionesNMD.TipoDisciplina   ? "AND COD_TIPO_DISCIPLINA = NVL(:CodigoTipoDisciplina, COD_TIPO_DISCIPLINA)" : "")}
                {(concepto != ConceptosCondicionesNMD.DisciplinaGrupo  ? "AND COD_DISCIPLINA_GRUPO = NVL(:CodigoDisciplinaGrupo, COD_DISCIPLINA_GRUPO)" : "")}";

            dah.GetSqlStringComando(query);

            dah.AddParameter("CodigoMedio", codigoMedio);

            if (concepto != ConceptosCondicionesNMD.Disciplina)
                dah.AddParameter("CodigoDisciplina", valores.CodigoDisciplina);
            if (concepto != ConceptosCondicionesNMD.Objetivo)
                dah.AddParameter("CodigoObjetivo", valores.CodigoObjetivo);
            if (concepto != ConceptosCondicionesNMD.TipoCompra)
                dah.AddParameter("CodigoTipoCompra", valores.CodigoTipoCompra);
            if (concepto != ConceptosCondicionesNMD.TipoDisciplina)
                dah.AddParameter("CodigoTipoDisciplina", valores.CodigoTipoDisciplina);
            if (concepto != ConceptosCondicionesNMD.DisciplinaGrupo)
                dah.AddParameter("CodigoDisciplinaGrupo", valores.CodigoDisciplinaGrupo);

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

