

using System.Text.Json.Serialization;

namespace HM.Presupuestos.Domain.Entidades
{
    [Serializable]
    public class DatosGuardarSobreprimasJSON
    {
        [JsonPropertyName("anio")]
        public int Anio { get; set; }

        [JsonPropertyName("cod_version")]
        public int CodigoVersion { get; set; }

        [JsonPropertyName("cod_network")]
        public int[] CodigoNetwork { get; set; } = [];

        [JsonPropertyName("cod_pais")]
        public int CodigoPais { get; set; }

        [JsonPropertyName("cod_medio")]
        public int[] CodigoMedio { get; set; } = [];

        [JsonPropertyName("cod_editorial_comercial")]
        public int[] CodigoEditorial { get; set; } = [];
    }
}

