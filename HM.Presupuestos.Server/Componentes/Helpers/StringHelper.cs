namespace HM.Presupuestos.Server.Componentes.Helpers
{
    public static class StringHelper
    {
        /// <summary>
        /// Funcion para poner la primera leta de una cadena en mayúsculas y las demas en minúsculas
        /// </summary>
        /// <param name="input">Cadena a modificar</param>
        /// <returns></returns>
        public static string Capitalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            input = input.Trim().ToLower();
            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }
}


