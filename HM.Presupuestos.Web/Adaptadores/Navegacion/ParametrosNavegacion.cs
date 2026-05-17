namespace HM.Presupuestos.Web.Adaptadores.Navegacion
{
    public class ParametrosNavegacion
    {
        public object? Parametros { get; private set; }

        public void Guardar(object parametros)
        {
            Parametros = parametros;
        }

        public T? Obtener<T>()
        {
            return (T?)Parametros;
        }

        public void Limpiar() => Parametros = null;
    }
}





