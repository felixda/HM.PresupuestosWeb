using HM.Presupuestos.Contratos.Comun;
using HM.Presupuestos.Contratos.Entidades;

namespace HM.Presupuestos.Application.Servicios
{
    /// <summary>
    /// Interfaz del servicio de gestión de condiciones comerciales y excepciones
    /// </summary>
    public interface ICondicionesService
    {
        Task<List<Vigencia>> ObtenerVigencias(CondicionFiltro filtro);

        Task InsertarVigencia(Vigencia item);

        Task ActualizarVigencia(Vigencia vigencia);

        /// <summary>
        /// Valida que una vigencia no se solape con otras vigencias existentes
        /// </summary>
        /// <param name="vigencia">Vigencia a validar con sus fechas desde/hasta</param>
        /// <returns>True si no hay solapamiento, false si existe al menos un solapamiento</returns>
        /// <remarks>
        /// Este método realiza las siguientes validaciones:
        /// 1. Busca vigencias con los mismos criterios (versión, grupo cliente, network, acuerdo)
        /// 2. Excluye la propia vigencia si ya existe (comparación por código)
        /// 3. Verifica solapamiento de rangos: [MesDesde..MesHasta]
        /// La lógica de solapamiento: vigencia.MesDesde &lt;= existente.MesHasta AND existente.MesDesde &lt;= vigencia.MesHasta
        /// </remarks>
        Task<bool> ValidarSolapesVigencia(Vigencia vigencia);

        /// <summary>
        /// Elimina una vigencia ejecutando un procedimiento almacenado que borra condiciones, excepciones y conceptos asociados
        /// </summary>
        /// <param name="vigencia">Vigencia a eliminar</param>
        /// <remarks>
        /// El PL de base de datos se encarga de eliminar en cascada:
        /// - Condiciones de la vigencia
        /// - Excepciones asociadas
        /// - Conceptos NMD de las excepciones
        /// Se registra la acción de auditoría después de la eliminación exitosa
        /// </remarks>
        Task EliminarVigencia(Vigencia vigencia);

        Task<bool> ExistenCondicionesVigencias(int codigoVigencia);

        /// <summary>
        /// Obtiene las condiciones por vigencia o devuelve una colección vacía basada en medios del network
        /// </summary>
        /// <param name="codigoVigencia">Código de la vigencia</param>
        /// <param name="codigoNetwork">Código del network para obtener medios si no hay condiciones</param>
        /// <returns>Lista de condiciones existentes o estructura vacía con medios del network</returns>
        /// <remarks>
        /// Lógica del método:
        /// 1. Si existen condiciones para la vigencia → las devuelve
        /// 2. Si NO existen condiciones → devuelve una colección con CondicionDto vacíos para cada medio del network
        /// Esto permite que la UI muestre todos los medios disponibles aunque no tengan condiciones configuradas
        /// </remarks>
        Task<List<CondicionDto>> ObtenerCondicionesPorVigencia(int codigoVigencia, int codigoNetwork);

        Task<List<ExcepcionDto>> ObtenerExcepcionesCondiciones(int codigoVigencia);

        /// <summary>
        /// Guarda condiciones y excepciones comerciales con sus conceptos asociados en una única transacción
        /// </summary>
        /// <param name="condicionesNoGuardadas">Diccionario de condiciones modificadas con sus campos cambiados</param>
        /// <param name="excepcionesNoGuardadas">Diccionario de excepciones modificadas con sus campos cambiados</param>
        /// <param name="codigoVigencia">Código de la vigencia a la que pertenecen</param>
        /// <remarks>
        /// Este método realiza un proceso complejo en una transacción:
        /// 
        /// CONDICIONES:
        /// - Para cada condición procesa 3 conceptos: SAG, Manpower, Devolución
        /// - Inserta si es nueva y tiene porcentaje
        /// - Actualiza si cambió el porcentaje o indicador de devolución
        /// - Elimina si el porcentaje pasa a null (también elimina excepciones relacionadas)
        /// 
        /// EXCEPCIONES:
        /// - Pre-tratamiento: Si cambió jerarquía, pone valores negativos temporales para evitar duplicados UK
        /// - Para cada excepción procesa 3 conceptos: SAG, Manpower, Devolución
        /// - Para cada excepción gestiona 7 conceptos NMD: Alcance, Objetivo, Disciplina, Diversified, TipoCompra, TipoDisciplina, DisciplinaGrupo
        /// - Solo actualiza campos que cambiaron (optimización mediante HashSet CamposCambiados)
        /// 
        /// Si cualquier operación falla, hace rollback de toda la transacción
        /// </remarks>
        Task GrabarCondicionesExcepciones(
            Dictionary<CondicionDto, CondicionesService.DatosCondicionCambiados> condicionesNoGuardadas,
            Dictionary<ExcepcionDto, CondicionesService.DatosExcepcionesCondicionCambiados> excepcionesNoGuardadas,
            int codigoVigencia);

        /// <summary>
        /// Elimina una excepción de condición y ajusta las jerarquías de excepciones posteriores
        /// </summary>
        /// <param name="codigosConceptosCondiciones">Lista de códigos de conceptos a eliminar</param>
        /// <param name="jerarquia">Jerarquía de la excepción eliminada</param>
        /// <param name="codigoVigencia">Código de vigencia para buscar excepciones afectadas</param>
        /// <param name="codigoMedio">Código de medio para filtrar excepciones del mismo medio</param>
        /// <param name="codigoUsuario">Código de usuario que realiza la operación</param>
        /// <remarks>
        /// Este método realiza las siguientes operaciones en una transacción:
        /// 1. Elimina los 7 conceptos NMD de cada condición (Alcance, Objetivo, Disciplina, etc.)
        /// 2. Elimina las excepciones de condición
        /// 3. Busca excepciones posteriores del mismo medio (jerarquía mayor)
        /// 4. Decrementa en 1 la jerarquía de todas las excepciones posteriores
        /// 5. Actualiza las jerarquías en base de datos
        /// 
        /// Esto mantiene la integridad de la jerarquía secuencial de excepciones por medio
        /// </remarks>
        Task EliminarExcepcionCondicion(
            List<CodigosConceptoCondicion> codigosConceptosCondiciones,
            int jerarquia,
            int codigoVigencia,
            int codigoMedio,
            int codigoUsuario);

        /// <summary>
        /// Importa condiciones desde MMS (Media Management System) registrando inicio y fin en auditoría
        /// </summary>
        /// <param name="param">Parámetros de filtro para la importación desde MMS</param>
        /// <param name="nombreMetodoLlamador">Nombre del método llamador (se obtiene automáticamente con CallerMemberName)</param>
        /// <remarks>
        /// Flujo del proceso:
        /// 1. Registra en log de auditoría el inicio de la importación
        /// 2. Ejecuta la importación en una transacción
        /// 3. Si tiene éxito: registra en auditoría la finalización
        /// 4. Si falla: hace rollback y propaga la excepción
        /// 
        /// Los logs de auditoría quedan registrados incluso si falla la importación,
        /// lo que permite trazabilidad de intentos fallidos
        /// </remarks>
        Task ImportarCondicionesMMS(CondicionImportarFiltro param, string nombreMetodoLlamador = "");
    }
}