namespace HM.Presupuestos.Server.Componentes.Helpers
{
    public static class IntHelper
    {
        /// <summary>
        /// Devuelve null si el valor es cero
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int? NullSiCero(this int? value) => value == 0 ? null : value;
    }
}


