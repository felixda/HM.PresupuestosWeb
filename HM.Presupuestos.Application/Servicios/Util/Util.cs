using HM.Presupuestos.Contratos.Comun;
using HM.Presupuestos.Contratos.Entidades;
using HM.Presupuestos.Contratos.Helper;
using System.Text.Json;

namespace HM.Presupuestos.Application.Servicios
{
    internal class Util
    {
        public static LogAccion CrearLogAccion(AccionesLog accion, string nombreMetodoLlamador, object? objetoConParametros = null)
        {
            string parametrosJson = objetoConParametros != null
                   ? JsonSerializer.Serialize(objetoConParametros, new JsonSerializerOptions { WriteIndented = true })
                   : string.Empty;

            LogAccion logAccion = new()
            {
                Accion = $"({nombreMetodoLlamador}) -> {accion.ObtenerDescripcion()} ",
                Parametros = parametrosJson
            };
           
            return logAccion;
        }

    }
}
