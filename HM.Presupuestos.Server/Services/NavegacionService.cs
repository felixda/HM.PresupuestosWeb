namespace HM.Presupuestos.Server.Services
{
    public class NavegacionService
    {
        public object? Datos { get; private set; }

        public void SetDatos(object datos)
        {
            Datos = datos;
        }

        public T? GetDatos<T>()
        {
            return (T?)Datos;
        }

        public void Limpiar() => Datos = null;
    }
}


