using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Extensiones;
using System.Text.Json;

namespace HM.Presupuestos.Application.CasosDeUso
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

