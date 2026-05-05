using System.ComponentModel;

namespace HM.Presupuestos.Domain.Helper
{
    public static class EnumExtensions
    {
        public static string ObtenerDescripcion(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttributes(typeof(DescriptionAttribute), false)
                                  .FirstOrDefault() as DescriptionAttribute;
            return attribute?.Description ?? value.ToString();
        }
    }
}
