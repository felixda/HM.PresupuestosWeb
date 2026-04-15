using System.Text;

namespace HM.Presupuestos.Server.Helper
{
    public static class ExcepcionesHelper
    {
        
            public static string ObtenerMensajeCompletoExcepcion (Exception ex)
            {
                if (ex == null) return string.Empty;

                var sb = new StringBuilder();
                sb.AppendLine(ex.Message);

                var inner = ex.InnerException;
                while (inner != null)
                {
                    sb.AppendLine("➡ " + inner.Message);
                    inner = inner.InnerException;
                }

                return sb.ToString();
            }
        

    }
}
