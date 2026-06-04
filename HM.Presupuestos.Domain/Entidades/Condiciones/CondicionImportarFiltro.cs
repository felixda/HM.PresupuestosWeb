
using System.Text.Json.Serialization;

namespace HM.Presupuestos.Domain.Entidades
{
    [Serializable]
    public class CondicionImportarFiltro
    {
        [JsonPropertyName("cod_network")]
        public required int[] CodigosNetwork { get; set; } = [];

        [JsonPropertyName("cod_grupo")]
        public required int[] CodigosGrupoCliente { get; set; } = [];
        
        [JsonPropertyName("anio")]
        public required int Anio { get; set; }

        [JsonPropertyName("cod_version_destino")]
        public required int CodigoVersion { get; set; }

    }

}
