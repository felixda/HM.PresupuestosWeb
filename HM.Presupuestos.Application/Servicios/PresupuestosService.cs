using HM.Presupuestos.Application.Repositorios;
using HM.Presupuestos.Contratos.Entidades;
using HM.Core.Comun.v6.Loggers.Interfaces;
using HM.Presupuestos.Contratos.Comun;
using System.Runtime.CompilerServices;
using HM.Core.Comun.v6.Entidades.Seguridad;

namespace HM.Presupuestos.Application.Servicios
{

    public class PresupuestosService(
        ILogger logger, 
        IPresupuestosRepository presupRepository, 
        ILogAccionesRepository logAccionesRepository) :IPresupuestosService
    {
        private readonly ILogger _logger = logger;
        private readonly IPresupuestosRepository _presupuestosRepository = presupRepository;
        private readonly ILogAccionesRepository _logAccionesRepository = logAccionesRepository;

        public async Task<List<CodigoDescripcion>> ObtenerTipologias()
        {
            List<CodigoDescripcion> resultado;

            _logger.Trace($"Llamando método ObtenerTipologías");
            resultado = await _presupuestosRepository.ObtenerTipologias();

            return resultado;
        }

        public async Task<List<CodigoDescripcion>> ObtenerAlcances()
        {
            List<CodigoDescripcion> resultado;

            _logger.Trace($"Llamando método ObtenerAlcances");
            resultado = await _presupuestosRepository.ObtenerAlcances();

            return resultado;
        }


        public async Task<List<CodigoDescripcion>> ObtenerDiversifiedsNCB()
        {
            List<CodigoDescripcion> resultado;

            _logger.Trace($"Llamando método ObtenerDiversifiedsNCB");
            resultado = await _presupuestosRepository.ObtenerDiversifiedsNCB();

            return resultado;
        }

        public async Task<List<CodigoDescripcion>> ObtenerDisciplinas()
        {
            List<CodigoDescripcion> resultado;

            _logger.Trace($"Llamando método ObtenerDisciplinas");
            resultado = await _presupuestosRepository.ObtenerDisciplinas();

            return resultado;
        }


        public async Task<List<CodigoDescripcion>> ObtenerTiposCompra()
        {
            List<CodigoDescripcion> resultado;

            _logger.Trace($"Llamando método ObtenerTiposCompra");
            resultado = await _presupuestosRepository.ObtenerTiposCompra();

            return resultado;
        }

        public async Task<List<CodigoDescripcion>> ObtenerObjetivos()
        {
            List<CodigoDescripcion> resultado;

            _logger.Trace($"Llamando método ObtenerObjetivos");
            resultado = await _presupuestosRepository.ObtenerObjetivos();

            return resultado;
        }


        public async Task<List<CodigoDescripcion>> ObtenerMedios()
        {
            List<CodigoDescripcion> resultado;

            _logger.Trace($"Llamando método ObtenerMedios");

            resultado = await _presupuestosRepository.ObtenerMedios();

            return resultado;
        }

        public async Task<List<CodigoDescripcion>> ObtenerMediosPorNetWork(string codigosNetwork)
        {
            _logger.Trace($"Llamando método ObtenerMediosPorNetWork");
            List<CodigoDescripcion> resultado = await _presupuestosRepository.ObtenerMediosPorNetWork(codigosNetwork);
            return resultado;
        }

        public async Task<List<CodigoDescripcion>> ObtenerNetworks()
        {
            List<CodigoDescripcion> resultado;

            _logger.Trace($"Llamando método ObtenerNetworks");

            resultado = await _presupuestosRepository.ObtenerNetworks();

            return resultado;
        }

        public async Task<List<CodigoDescripcion>> ObtenerGruposClientes()
        {
            List<CodigoDescripcion> resultado;

            _logger.Trace($"Llamando método ObtenerGruposClientes");

            resultado = await _presupuestosRepository.ObtenerGruposClientes();

            return resultado;
        }

        public async Task<List<CodigoDescripcion>> ObtenerGruposClientePorNetwork(int codigoNetwork)
        {
            _logger.Trace($"Llamando método ObtenerGruposClientePorNetwork");
            return await _presupuestosRepository.ObtenerGruposClientePorNetworks(codigoNetwork.ToString());
        }

        public async Task<List<CodigoDescripcion>> ObtenerGruposClientePorNetworks(string codigosNetworks)
        {
            _logger.Trace($"Llamando método ObtenerGruposClientePorNetwork");
            return await _presupuestosRepository.ObtenerGruposClientePorNetworks(codigosNetworks);
        }


        public async Task<List<GrupoClientesConNetwork>> ObtenerGruposClientesConNetwork()
        {
            List<GrupoClientesConNetwork> resultado;

            _logger.Trace($"Llamando método ObtenerGruposClientesConNetwork");

            resultado = await _presupuestosRepository.ObtenerGruposClientesConNetwork();

            return resultado;
        }

        public async Task<List<CodigoDescripcion>> ObtenerEditoriales(FiltroEditoriales? filtro = null)
        {
            List<CodigoDescripcion> resultado;

            _logger.Trace($"PresupuestosService.ObtenerEditoriales");

            resultado = await _presupuestosRepository.ObtenerEditoriales(filtro);

            return resultado;
        }



        public async Task<List<CodigoDescripcion>> ObtenerAgrupacionesComerciales(string? codigosMedios = null)
        {
            List<CodigoDescripcion> resultado;

            _logger.Trace($"Llamando método ObtenerAgrupacionesComerciales");

            resultado = await _presupuestosRepository.ObtenerAgrupacionesComerciales(codigosMedios);

            return resultado;
        }

        public async Task<List<AgrupacionComercialConMedio>> ObtenerAgrupacionesComercialesConMedio()
        {
            List<AgrupacionComercialConMedio> resultado;

            _logger.Trace($"Llamando método ObtenerAgrupacionesComercialesConMedio");

            resultado = await _presupuestosRepository.ObtenerAgrupacionesComercialesConMedio();

            return resultado;
        }


        public async Task<List<CodigoDescripcion>> ObtenerEditorialesPorAgrupacionComercial(int codigoAgrupacionComercial)
        {
            List<CodigoDescripcion> resultado;

            _logger.Trace($"AjustesService.ObtenerEditorialesPorAgrupacionComercial");

            resultado = await _presupuestosRepository.ObtenerEditorialesPorAgrupacionComercial(codigoAgrupacionComercial);

            return resultado;
        }

        //public async Task<List<EditorialConAgrupacionComercial>> ObtenerEditorialesConAgrupacionComercial()
        //{
        //    List<EditorialConAgrupacionComercial> resultado;

        //    _logger.Trace($"AjustesService.ObtenerEditorialesConAgrupacionComercial");

        //    resultado = await _presupuestosRepository.ObtenerEditorialesConAgrupacionComercial();

        //    return resultado;
        //}

        public async Task<List<EditorialConAgrupacionComercialAndMedio>> ObtenerEditorialesConAgrupacionComercialAndMedio()
        {
            List<EditorialConAgrupacionComercialAndMedio> resultado;
            _logger.Trace($"AjustesService.ObtenerEditorialesConAgrupacionComercialAndMedio");

            resultado = await _presupuestosRepository.ObtenerEditorialesConAgrupacionComercialAndMedio();
            return resultado;
        }



       
        public async Task<List<CodigoDescripcion>> ObtenerTiposDisciplinas()
        {
            List<CodigoDescripcion> resultado;

            _logger.Trace($"Llamando método ObtenerDisciplinas");
            resultado = await _presupuestosRepository.ObtenerTiposDisciplina();

            return resultado;
        }
        public async Task<List<CodigoDescripcion>> ObtenerDisciplinasGrupos()
        {
            List<CodigoDescripcion> resultado;

            _logger.Trace($"Llamando método ObtenerDisciplinasGrupos");
            resultado = await _presupuestosRepository.ObtenerDisciplinasGrupos();

            return resultado;
        }

        /// <summary>
        /// Obtiene lista de meses cerrados
        /// </summary>
        /// <param name="year">Año de filtro</param>
        /// <returns>Lista de int</returns>
        public async Task<List<int>> ObtenerMesCerradoList(int year )
        {
            List<int> resultado;

            _logger.Trace($"PresupuestosService.ObtenerMesCerradoList");
            resultado = await _presupuestosRepository.ObtenerMesCerradoList(year);

            return resultado;
        }


        /// <summary>
        /// Obtiene lista agrupaciones editoriales de un medio
        /// </summary>
        /// <param name="codeMedio">Código de medio</param>
        /// <returns>Lista de objeto CodigoDescripcion</returns>
        public async Task<List<CodigoDescripcion>> GetAgrupacionEditorialListByMedio(int codeMedio)
        {
            List<CodigoDescripcion> resultado;

            _logger.Trace($"PresupuestosService.GetAgrupacionEditorialListByMedio");
            resultado = await _presupuestosRepository.GetAgrupacionEditorialListByMedio(codeMedio);

            return resultado;
        }


        /// <summary>
        /// Obtiene lista editoriales de un medio
        /// </summary>
        /// <param name="codeMedio">Código de medio</param>
        /// <returns>Lista de objeto CodigoDescripcion</returns>
        public async Task<List<CodigoDescripcion>> GetEditorialListByMedio(int codeMedio)
        {
            List<CodigoDescripcion> resultado;

            _logger.Trace($"PresupuestosService.GetEditorialListByMedio");
            resultado = await _presupuestosRepository.GetEditorialListByMedio(codeMedio);

            return resultado;
        }

        /// <summary>
        /// Obtiene lista editoriales de una agrupacion
        /// </summary>
        /// <param name="codeAgrupacionEditorial">Código de Agrupación Editorial</param>
        /// <returns>Lista de objeto CodigoDescripcion</returns>
        public async Task<List<CodigoDescripcion>> GetEditorialListByAgrupacionEditorial(int codeAgrupacionEditorial)
        {
            List<CodigoDescripcion> resultado;

            _logger.Trace($"PresupuestosService.GetEditorialListByAgrupacionEditorial");
            resultado = await _presupuestosRepository.GetEditorialListByAgrupacionEditorial(codeAgrupacionEditorial);

            return resultado;
        }


        public async Task<CodigoDescripcion?> ObtenerMedio(int codigoMedio)
        {
            CodigoDescripcion? resultado;

            _logger.Trace($"PresupuestosService.ObtenerMedio");
            resultado = await _presupuestosRepository.ObtenerMedio(codigoMedio);

            return resultado;
        }

        public async Task<CodigoDescripcion?> ObtenerAgrupacionComercial(int codigoAgrupacionComercial)
        {
            CodigoDescripcion? resultado;

            _logger.Trace($"PresupuestosService.ObtenerAgrupacionComercial");
            resultado = await _presupuestosRepository.ObtenerAgrupacionComercial(codigoAgrupacionComercial);

            return resultado;
        }

        public async Task<CodigoDescripcion?> ObtenerEditorial(int codigoEditorial)
        {
            CodigoDescripcion? resultado;

            _logger.Trace($"PresupuestosService.ObtenerEditorial");
            resultado = await _presupuestosRepository.ObtenerEditorial(codigoEditorial);

            return resultado;
        }

        public async Task<List<CodigoDescripcion>> ObtenerConceptosNMD(int codigoMedio, ConceptosCondicionesNMD concepto, ValoresConceptosNMD valores)
        {
            List<CodigoDescripcion> resultado;

            _logger.Trace($"PresupuestosService.GetEditorialListByAgrupacionEditorial");
            resultado = await _presupuestosRepository.ObtenerConceptosNMD(codigoMedio, concepto,valores);

            return resultado;
        }
    }
}
