using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Domain.Entidades;

namespace HM.Presupuestos.Application.CasosDeUso
{
    /// <summary>
    /// Interfaz del servicio de presupuestos que gestiona la obtención de datos maestros y catálogos
    /// </summary>
    public interface IPresupuestosService
    {
        /// <summary>
        /// Obtiene la lista completa de tipologías disponibles
        /// </summary>
        /// <returns>Lista de tipologías con código y descripción</returns>
        Task<List<CodigoDescripcion>> ObtenerTipologias();

        /// <summary>
        /// Obtiene la lista completa de alcances disponibles
        /// </summary>
        /// <returns>Lista de alcances con código y descripción</returns>
        Task<List<CodigoDescripcion>> ObtenerAlcances();

        /// <summary>
        /// Obtiene la lista de Diversifieds NCB (Non-Core Business)
        /// </summary>
        /// <returns>Lista de Diversifieds NCB con código y descripción</returns>
        Task<List<CodigoDescripcion>> ObtenerDiversifiedsNCB();

        /// <summary>
        /// Obtiene la lista completa de disciplinas disponibles
        /// </summary>
        /// <returns>Lista de disciplinas con código y descripción</returns>
        Task<List<CodigoDescripcion>> ObtenerDisciplinas();

        /// <summary>
        /// Obtiene la lista completa de tipos de compra disponibles
        /// </summary>
        /// <returns>Lista de tipos de compra con código y descripción</returns>
        Task<List<CodigoDescripcion>> ObtenerTiposCompra();

        /// <summary>
        /// Obtiene la lista completa de objetivos disponibles
        /// </summary>
        /// <returns>Lista de objetivos con código y descripción</returns>
        Task<List<CodigoDescripcion>> ObtenerObjetivos();

        /// <summary>
        /// Obtiene la lista completa de medios de comunicación
        /// </summary>
        /// <returns>Lista de medios con código y descripción</returns>
        Task<List<CodigoDescripcion>> ObtenerMedios();

        /// <summary>
        /// Obtiene la lista de medios filtrados por networks específicos
        /// </summary>
        /// <param name="codigosNetwork">Códigos de networks separados por coma (ej: "1,2,3")</param>
        /// <returns>Lista de medios asociados a los networks especificados</returns>
        Task<List<CodigoDescripcion>> ObtenerMediosPorNetWork(string codigosNetwork);

        /// <summary>
        /// Obtiene la lista completa de networks (redes de medios)
        /// </summary>
        /// <returns>Lista de networks con código y descripción</returns>
        Task<List<CodigoDescripcion>> ObtenerNetworks();

        /// <summary>
        /// Obtiene la lista completa de grupos de clientes
        /// </summary>
        /// <returns>Lista de grupos de clientes con código y descripción</returns>
        Task<List<CodigoDescripcion>> ObtenerGruposClientes();

        /// <summary>
        /// Obtiene los grupos de clientes asociados a un network específico
        /// </summary>
        /// <param name="codigoNetwork">Código del network</param>
        /// <returns>Lista de grupos de clientes del network especificado</returns>
        Task<List<CodigoDescripcion>> ObtenerGruposClientePorNetwork(int codigoNetwork);

        /// <summary>
        /// Obtiene los grupos de clientes asociados a múltiples networks
        /// </summary>
        /// <param name="codigosNetworks">Códigos de networks separados por coma (ej: "1,2,3")</param>
        /// <returns>Lista de grupos de clientes de los networks especificados</returns>
        Task<List<CodigoDescripcion>> ObtenerGruposClientePorNetworks(string codigosNetworks);

        /// <summary>
        /// Obtiene la lista de grupos de clientes con su network asociado
        /// </summary>
        /// <returns>Lista de grupos de clientes incluyendo información del network</returns>
        Task<List<GrupoClientesConNetwork>> ObtenerGruposClientesConNetwork();

        /// <summary>
        /// Obtiene la lista de editoriales con filtros opcionales
        /// </summary>
        /// <param name="filtro">Filtro opcional para editoriales (medio, agrupación, etc.)</param>
        /// <returns>Lista de editoriales con código y descripción</returns>
        Task<List<CodigoDescripcion>> ObtenerEditoriales(FiltroEditoriales? filtro = null);

        /// <summary>
        /// Obtiene la lista de agrupaciones comerciales con filtro opcional por medios
        /// </summary>
        /// <param name="codigosMedios">Códigos de medios separados por coma para filtrar (opcional)</param>
        /// <returns>Lista de agrupaciones comerciales con código y descripción</returns>
        Task<List<CodigoDescripcion>> ObtenerAgrupacionesComerciales(string? codigosMedios = null);

        /// <summary>
        /// Obtiene la lista de agrupaciones comerciales con su medio asociado
        /// </summary>
        /// <returns>Lista de agrupaciones comerciales incluyendo información del medio</returns>
        Task<List<AgrupacionComercialConMedio>> ObtenerAgrupacionesComercialesConMedio();

        /// <summary>
        /// Obtiene las editoriales asociadas a una agrupación comercial específica
        /// </summary>
        /// <param name="codigoAgrupacionComercial">Código de la agrupación comercial</param>
        /// <returns>Lista de editoriales de la agrupación comercial especificada</returns>
        Task<List<CodigoDescripcion>> ObtenerEditorialesPorAgrupacionComercial(int codigoAgrupacionComercial);

        /// <summary>
        /// Obtiene la lista de editoriales con información de agrupación comercial y medio
        /// </summary>
        /// <returns>Lista de editoriales con su agrupación comercial y medio asociados</returns>
        Task<List<EditorialConAgrupacionComercialAndMedio>> ObtenerEditorialesConAgrupacionComercialAndMedio();

        /// <summary>
        /// Obtiene la lista de tipos de disciplinas disponibles
        /// </summary>
        /// <returns>Lista de tipos de disciplinas con código y descripción</returns>
        Task<List<CodigoDescripcion>> ObtenerTiposDisciplinas();

        /// <summary>
        /// Obtiene la lista de grupos de disciplinas disponibles
        /// </summary>
        /// <returns>Lista de grupos de disciplinas con código y descripción</returns>
        Task<List<CodigoDescripcion>> ObtenerDisciplinasGrupos();

        /// <summary>
        /// Obtiene la lista de meses cerrados para un año específico
        /// </summary>
        /// <param name="year">Año para filtrar los meses cerrados</param>
        /// <returns>Lista de números de mes (1-12) que están cerrados</returns>
        Task<List<int>> ObtenerMesCerradoList(int year);

        /// <summary>
        /// Obtiene las agrupaciones editoriales asociadas a un medio específico
        /// </summary>
        /// <param name="codeMedio">Código del medio</param>
        /// <returns>Lista de agrupaciones editoriales del medio especificado</returns>
        Task<List<CodigoDescripcion>> GetAgrupacionEditorialListByMedio(int codeMedio);

        /// <summary>
        /// Obtiene las editoriales asociadas a un medio específico
        /// </summary>
        /// <param name="codeMedio">Código del medio</param>
        /// <returns>Lista de editoriales del medio especificado</returns>
        Task<List<CodigoDescripcion>> GetEditorialListByMedio(int codeMedio);

        /// <summary>
        /// Obtiene las editoriales asociadas a una agrupación editorial específica
        /// </summary>
        /// <param name="codeAgrupacionEditorial">Código de la agrupación editorial</param>
        /// <returns>Lista de editoriales de la agrupación especificada</returns>
        Task<List<CodigoDescripcion>> GetEditorialListByAgrupacionEditorial(int codeAgrupacionEditorial);

        /// <summary>
        /// Obtiene un medio específico por su código
        /// </summary>
        /// <param name="codigoMedio">Código del medio a buscar</param>
        /// <returns>Medio encontrado o null si no existe</returns>
        Task<CodigoDescripcion?> ObtenerMedio(int codigoMedio);

        /// <summary>
        /// Obtiene una agrupación comercial específica por su código
        /// </summary>
        /// <param name="codigoAgrupacionComercial">Código de la agrupación comercial a buscar</param>
        /// <returns>Agrupación comercial encontrada o null si no existe</returns>
        Task<CodigoDescripcion?> ObtenerAgrupacionComercial(int codigoAgrupacionComercial);

        /// <summary>
        /// Obtiene una editorial específica por su código
        /// </summary>
        /// <param name="codigoEditorial">Código de la editorial a buscar</param>
        /// <returns>Editorial encontrada o null si no existe</returns>
        Task<CodigoDescripcion?> ObtenerEditorial(int codigoEditorial);

        /// <summary>
        /// Obtiene conceptos NMD (Network/Medio/Disciplina) filtrados por medio, concepto y valores
        /// </summary>
        /// <param name="codigoMedio">Código del medio para filtrar</param>
        /// <param name="concepto">Tipo de concepto NMD a obtener</param>
        /// <param name="valores">Valores específicos del concepto a filtrar</param>
        /// <returns>Lista de conceptos NMD que cumplen los criterios especificados</returns>
        Task<List<CodigoDescripcion>> ObtenerConceptosNMD(int codigoMedio, ConceptosCondicionesNMD concepto, ValoresConceptosNMD valores);
    }
}

