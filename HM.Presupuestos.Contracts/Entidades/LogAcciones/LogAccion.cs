
using HM.Presupuestos.Contratos.Comun;
using System.Runtime.CompilerServices;

namespace HM.Presupuestos.Contratos.Entidades
{
    public class LogAccion
    {
        public int CodigoUsuario { get; set; }
        public string Accion { get; set; } = "";
        public string? Parametros { get; set; }
    }


}
