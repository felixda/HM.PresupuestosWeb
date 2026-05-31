using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Domain.Entidades;

namespace HM.Presupuestos.Application.CasosDeUso.Compartido
{
    /// <summary>
    /// Interfaz del servicio de datos maestros que gestiona la obtenci�n de datos maestros y cat�logos
    /// </summary>
    public interface IMaestrosService
    {
        /// <summary>
        /// Obtiene la lista completa de tipolog�as disponibles
        /// </summary>
        /// <returns>Lista de tipolog�as con c�digo y descripci�n</returns>
        Task<List<CodigoDescripcion>> ObtenerTipologias();

        /// <summary>
        /// Obtiene la lista completa de alcances disponibles
        /// </summary>
        /// <returns>Lista de alcances con c�digo y descripci�n</returns>
        Task<List<CodigoDescripcion>> ObtenerAlcances();

        /// <summary>
        /// Obtiene la lista de Diversifieds NCB (Non-Core Business)
        /// </summary>
        /// <returns>Lista de Diversifieds NCB con c�digo y descripci�n</returns>
        Task<List<CodigoDescripcion>> ObtenerDiversifiedsNCB();

        /// <summary>
        /// Obtiene la lista completa de disciplinas disponibles
        /// </summary>
        /// <returns>Lista de disciplinas con c�digo y descripci�n</returns>
        Task<List<CodigoDescripcion>> ObtenerDisciplinas();

        /// <summary>
        /// Obtiene la lista completa de tipos de compra disponibles
        /// </summary>
        /// <returns>Lista de tipos de compra con c�digo y descripci�n</returns>
        Task<List<CodigoDescripcion>> ObtenerTiposCompra();

        /// <summary>
        /// Obtiene la lista completa de objetivos disponibles
        /// </summary>
        /// <returns>Lista de objetivos con c�digo y descripci�n</returns>
        Task<List<CodigoDescripcion>> ObtenerObjetivos();

        /// <summary>
        /// Obtiene la lista completa de medios de comunicaci�n
        /// </summary>
        /// <returns>Lista de medios con c�digo y descripci�n</returns>
        Task<List<CodigoDescripcion>> ObtenerMedios();

        /// <summary>
        /// Obtiene la lista de medios filtrados por networks espec�ficos
        /// </summary>
        /// <param name="codigosNetwork">C�digos de networks separados por coma (ej: "1,2,3")</param>
        /// <returns>Lista de medios asociados a los networks especificados</returns>
        Task<List<CodigoDescripcion>> ObtenerMediosPorNetWork(string codigosNetwork);

        /// <summary>
        /// Obtiene la lista completa de networks (redes de medios)
        /// </summary>
        /// <returns>Lista de networks con c�digo y descripci�n</returns>
        Task<List<CodigoDescripcion>> ObtenerNetworks();

        /// <summary>
        /// Obtiene la lista completa de grupos de clientes
        /// </summary>
        /// <returns>Lista de grupos de clientes con c�digo y descripci�n</returns>
        Task<List<CodigoDescripcion>> ObtenerGruposClientes();

        /// <summary>
        /// Obtiene los grupos de clientes asociados a un network espec�fico
        /// </summary>
        /// <param name="codigoNetwork">C�digo del network</param>
        /// <returns>Lista de grupos de clientes del network especificado</returns>
        Task<List<CodigoDescripcion>> ObtenerGruposClientePorNetwork(int codigoNetwork);

        /// <summary>
        /// Obtiene los grupos de clientes asociados a m�ltiples networks
        /// </summary>
        /// <param name="codigosNetworks">C�digos de networks separados por coma (ej: "1,2,3")</param>
        /// <returns>Lista de grupos de clientes de los networks especificados</returns>
        Task<List<CodigoDescripcion>> ObtenerGruposClientePorNetworks(string codigosNetworks);

        /// <summary>
        /// Obtiene la lista de grupos de clientes con su network asociado
        /// </summary>
        /// <returns>Lista de grupos de clientes incluyendo informaci�n del network</returns>
        Task<List<GrupoClientesConNetwork>> ObtenerGruposClientesConNetwork();

        /// <summary>
        /// Obtiene la lista de editoriales con filtros opcionales
        /// </summary>
        /// <param name="filtro">Filtro opcional para editoriales (medio, agrupaci�n, etc.)</param>
        /// <returns>Lista de editoriales con c�digo y descripci�n</returns>
        Task<List<CodigoDescripcion>> ObtenerEditoriales(FiltroEditoriales? filtro = null);

        /// <summary>
        /// Obtiene la lista de agrupaciones comerciales con filtro opcional por medios
        /// </summary>
        /// <param name="codigosMedios">C�digos de medios separados por coma para filtrar (opcional)</param>
        /// <returns>Lista de agrupaciones comerciales con c�digo y descripci�n</returns>
        Task<List<CodigoDescripcion>> ObtenerAgrupacionesComerciales(string? codigosMedios = null);

        /// <summary>
        /// Obtiene la lista de agrupaciones comerciales con su medio asociado
        /// </summary>
        /// <returns>Lista de agrupaciones comerciales incluyendo informaci�n del medio</returns>
        Task<List<AgrupacionComercialConMedio>> ObtenerAgrupacionesComercialesConMedio();

        /// <summary>
        /// Obtiene las editoriales asociadas a una agrupaci�n comercial espec�fica
        /// </summary>
        /// <param name="codigoAgrupacionComercial">C�digo de la agrupaci�n comercial</param>
        /// <returns>Lista de editoriales de la agrupaci�n comercial especificada</returns>
        Task<List<CodigoDescripcion>> ObtenerEditorialesPorAgrupacionComercial(int codigoAgrupacionComercial);

        /// <summary>
        /// Obtiene la lista de editoriales con informaci�n de agrupaci�n comercial y medio
        /// </summary>
        /// <returns>Lista de editoriales con su agrupaci�n comercial y medio asociados</returns>
        Task<List<EditorialConAgrupacionComercialAndMedio>> ObtenerEditorialesConAgrupacionComercialAndMedio();

        /// <summary>
        /// Obtiene la lista de tipos de disciplinas disponibles
        /// </summary>
        /// <returns>Lista de tipos de disciplinas con c�digo y descripci�n</returns>
        Task<List<CodigoDescripcion>> ObtenerTiposDisciplinas();

        /// <summary>
        /// Obtiene la lista de grupos de disciplinas disponibles
        /// </summary>
        /// <returns>Lista de grupos de disciplinas con c�digo y descripci�n</returns>
        Task<List<CodigoDescripcion>> ObtenerDisciplinasGrupos();

        /// <summary>
        /// Obtiene la lista de meses cerrados para un a�o espec�fico
        /// </summary>
        /// <param name="year">A�o para filtrar los meses cerrados</param>
        /// <returns>Lista de n�meros de mes (1-12) que est�n cerrados</returns>
        Task<List<int>> ObtenerMesCerradoList(int year);

        /// <summary>
        /// Obtiene las agrupaciones editoriales asociadas a un medio espec�fico
        /// </summary>
        /// <param name="codeMedio">C�digo del medio</param>
        /// <returns>Lista de agrupaciones editoriales del medio especificado</returns>
        Task<List<CodigoDescripcion>> GetAgrupacionEditorialListByMedio(int codeMedio);

        /// <summary>
        /// Obtiene las editoriales asociadas a un medio espec�fico
        /// </summary>
        /// <param name="codeMedio">C�digo del medio</param>
        /// <returns>Lista de editoriales del medio especificado</returns>
        Task<List<CodigoDescripcion>> GetEditorialListByMedio(int codeMedio);

        /// <summary>
        /// Obtiene las editoriales asociadas a una agrupaci�n editorial espec�fica
        /// </summary>
        /// <param name="codeAgrupacionEditorial">C�digo de la agrupaci�n editorial</param>
        /// <returns>Lista de editoriales de la agrupaci�n especificada</returns>
        Task<List<CodigoDescripcion>> GetEditorialListByAgrupacionEditorial(int codeAgrupacionEditorial);

        /// <summary>
        /// Obtiene un medio espec�fico por su c�digo
        /// </summary>
        /// <param name="codigoMedio">C�digo del medio a buscar</param>
        /// <returns>Medio encontrado o null si no existe</returns>
        Task<CodigoDescripcion?> ObtenerMedio(int codigoMedio);

        /// <summary>
        /// Obtiene una agrupaci�n comercial espec�fica por su c�digo
        /// </summary>
        /// <param name="codigoAgrupacionComercial">C�digo de la agrupaci�n comercial a buscar</param>
        /// <returns>Agrupaci�n comercial encontrada o null si no existe</returns>
        Task<CodigoDescripcion?> ObtenerAgrupacionComercial(int codigoAgrupacionComercial);

        /// <summary>
        /// Obtiene una editorial espec�fica por su c�digo
        /// </summary>
        /// <param name="codigoEditorial">C�digo de la editorial a buscar</param>
        /// <returns>Editorial encontrada o null si no existe</returns>
        Task<CodigoDescripcion?> ObtenerEditorial(int codigoEditorial);

        /// <summary>
        /// Obtiene conceptos NMD (Network/Medio/Disciplina) filtrados por medio, concepto y valores
        /// </summary>
        /// <param name="codigoMedio">C�digo del medio para filtrar</param>
        /// <param name="concepto">Tipo de concepto NMD a obtener</param>
        /// <param name="valores">Valores espec�ficos del concepto a filtrar</param>
        /// <returns>Lista de conceptos NMD que cumplen los criterios especificados</returns>
        Task<List<CodigoDescripcion>> ObtenerConceptosNMD(int codigoMedio, ConceptosCondicionesNMD concepto, ValoresConceptosNMD valores);
    }
}
