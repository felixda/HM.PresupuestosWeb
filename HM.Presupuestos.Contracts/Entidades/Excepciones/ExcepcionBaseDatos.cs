


namespace HM.Presupuestos.Contratos.Entidades
{
    public class ExcepcionBaseDatos : Exception
    {
        public int Codigo { get;}

        public ExcepcionBaseDatos (int codigo, string mensaje) : base (mensaje)
        {
            Codigo = codigo;
        }

        public override string ToString ()
        {
            return $"Error {Codigo} de Base de datos: {Message}";
        }

    }
}
