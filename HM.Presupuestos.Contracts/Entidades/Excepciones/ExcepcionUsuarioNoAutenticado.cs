namespace HM.Presupuestos.Contratos.Entidades
{
    public class ExcepcionUsuarioNoAutenticado : InvalidOperationException
    {
        public ExcepcionUsuarioNoAutenticado()
            : base("No hay un usuario autenticado en el contexto actual. " +
                   "Por favor, inicie sesión nuevamente.")
        {
        }

        public ExcepcionUsuarioNoAutenticado(string message)
            : base(message)
        {
        }

        public ExcepcionUsuarioNoAutenticado(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
