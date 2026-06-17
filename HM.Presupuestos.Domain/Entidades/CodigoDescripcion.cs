
namespace HM.Presupuestos.Domain.Entidades
{
    public class CodigoDescripcion:  IConCodigo, IConIcono
    {
        public int Codigo {  get; set; }
        public string Descripcion { get; set; } = "";

        /// <summary>
        /// Devuelve cadena en formato Descripcion (Codigo)
        /// </summary>
        public string DescripcionConCodigo
        {
            get
            {
                return $"{Descripcion} ({Codigo})";
            }
        }

        /// <summary>
        /// Devuelve cadena en formato (Codigo) Descripcion
        /// </summary>
        public string CodigoConDescripcion
        {
            get
            {
                return $"{Descripcion} ({Codigo})";
            }
        }

        public string IconoCssClass
        {
            get
            {

                string icono = string.Empty;
                return icono;
            }
        }

        /// <summary>
        /// Propiedad que se muestra cuando se asocia esta clase a una lista o combo
        /// </summary>
        public string DisplayText
        {
            get
            {
                return Descripcion;
            }
        }

    }
}
