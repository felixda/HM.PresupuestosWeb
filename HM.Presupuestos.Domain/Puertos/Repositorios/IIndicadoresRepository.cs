using HM.Presupuestos.Domain.Entidades;

namespace HM.Presupuestos.Domain.Puertos
{
    public interface IIndicadoresRepository 
    {
        /// <summary>
        /// Obtener los indicadores de las versiones con una lista de sus descripciones, abreviaturas y leyendas en los diferentes idiomas
        /// </summary>
        /// <param name="descripcion">Filtro opcinal para la bÃºsqueda</param>
        /// <returns></returns>
        Task<List<Indicador>> ObtenerIndicadoresConIdiomas(string? descripcion = null);

        Task EliminarIndicador(int codigoIndicador);

        Task EliminarIdiomasIndicador(int codigoIndicador);

        Task<bool> ExisteIndicador(Indicador indicador);

        Task<bool> ExisteOrden(Indicador indicador);

        Task<bool> ExisteBitAnd(Indicador indicador);

        Task<int> InsertarIndicador(Indicador indicador);

        Task ActualizarIndicador(Indicador indicador);

        Task<int> ObtenerUltimoBitAnd();

        Task<int> ObtenerUltimoOrden();

        Task InsertarIdiomaIndicador(IdiomaIndicador idiomaIndicador);

        Task ActualizarIdiomaIndicador(IdiomaIndicador idiomaIndicador);

        ITransaccion ObtenerTransaccion();

        Task EliminarIdiomaIndicador(int codigo);

        Task<int> ObtenerBitAndIndicador(int codigoIndicador);

        /// <summary>
        /// Actualiza el bitAnd de una version. Se mete en este repositorio para poder meterlo en una transaccion
        /// </summary>
        /// <param name="codigoVersion"></param>
        /// <param name="bitAnd"></param>
        /// <returns></returns>
        Task Actualizar1BitAndVersion(int codigoVersion, int bitAnd);

    }
}
