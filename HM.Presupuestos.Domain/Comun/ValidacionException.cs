
namespace HM.Presupuestos.Domain.Comun
{
    public class ValidacionException : Exception
    {
        public CampoErrorValidacion CampoValidado { get;}
        public string Valor { get; }

        /// <summary>
        /// Excepcion que se genera cuando el valor de un campo ya existe en la BD
        /// </summary>
        /// <param name="campoValidado">Tipo del campo que se esta validando</param>
        /// <param name="valor">Valor que se comprueba y que ya existe</param>
        public ValidacionException(CampoErrorValidacion campoValidado, string valor)
       : base("Error de validación") //Se pone un texto para que no salga vacio en la excepcion, pero la descripcion se pone luego con el texto en el idioma correspondiente
        {
            CampoValidado = campoValidado;
            Valor = valor;
        }

    }
}
