
using System.Text.Json.Serialization;

namespace HM.Presupuestos.Domain.Entidades
{
    [Serializable]
    public class SobreprimaImportarFiltro
    {

        [JsonPropertyName("cod_usuario")]
        public required int CodigoUsuario { get; set; }

        [JsonPropertyName("cod_pais")]
        public required int CodigoPais { get; set; }

        [JsonPropertyName("anio")]
        public required int Anio { get; set; }

        [JsonPropertyName("cod_network")]
        public required int[] CodigosNetwork { get; set; } = [];

        [JsonPropertyName("cod_medio")]
        public required int[] CodigosMedio { get; set; } = [];


        [JsonPropertyName("cod_version")]
        public required int CodigoVersion { get; set; }

        [JsonPropertyName("cod_editorial_comercial")]
        public required int[] CodigosEditoriales { get; set; } = [];

    }

}
