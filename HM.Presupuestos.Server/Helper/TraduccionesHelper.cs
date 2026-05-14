
using System.Globalization;

namespace HM.Presupuestos.Server.Helper
{
    public class TraduccionesHelper(ITraductorRecursos resourceService, IGestorIdioma idiomaService)
    {

        private readonly ITraductorRecursos _resourceService = resourceService;
        private readonly IGestorIdioma _idiomaService = idiomaService;

        public async Task<string> GetResourceValue(string elementExpression)
        {
            var idioma = _idiomaService.IdiomaActual;
            var result = _resourceService.ObtenerTexto(elementExpression, idioma);

            return result;
        }

        /// <summary>
        /// Devuelve una lista con los meses del año en el idioma actual de la aplicacion
        /// </summary>
        /// <returns></returns>
        public async Task<List<CodigoDescripcion>> ObtenerMeses()
        {
            var idioma = _idiomaService.IdiomaActual; 
            List<CodigoDescripcion> meses = [];
            var culture = new CultureInfo(idioma);
            var dateTimeFormat = culture.DateTimeFormat;
            string[] nombresMeses = dateTimeFormat.MonthNames;
            int numeroMes = 1;

            foreach (var nombreMes in nombresMeses)
            {
                if (!string.IsNullOrEmpty(nombreMes))
                {
                    CodigoDescripcion mes = new()
                    {
                        Codigo = numeroMes,
                        Descripcion = nombreMes
                    };
                    meses.Add(mes);
                    numeroMes++;
                }
            }
            return meses;
        }

        /// <summary>
        /// Funcion para pasar los meses de los objetos de una coleccion de vigencias de numero a texto
        /// </summary>
        /// <param name="vigencias"></param>
        /// <returns></returns>
        public async Task TraducirMesesVigencias( List<Vigencia> vigencias)
        {
            List<Vigencia> resultado = [];
            var idioma = _idiomaService.IdiomaActual;
            var culture = new CultureInfo(idioma);
            var dateTimeFormat = culture.DateTimeFormat;
            string[] nombresMeses = dateTimeFormat.MonthNames;

            foreach (Vigencia vigencia in vigencias)
            {
                string mesDesde = nombresMeses[vigencia.MesDesde - 1];
                string mesHasta = nombresMeses[vigencia.MesHasta - 1];
                vigencia.Descripcion = $" {mesDesde} - {mesHasta}";
            }
        }

    }
}
