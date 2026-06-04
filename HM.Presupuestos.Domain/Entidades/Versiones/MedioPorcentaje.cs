ï»¿using System.Text.Json.Serialization;

namespace HM.Presupuestos.Domain.Entidades
{
    [Serializable]
    public class MedioPorcentaje
    {
        [JsonPropertyName("cod_medio")]
        public int CodigoMedio { get; set; }

        [JsonIgnore]
        public string DescripcionMedio { get; set; } = string.Empty;


        [JsonIgnore]
        private decimal _porcentaje;

        [JsonPropertyName("porcentaje")]
        public decimal Porcentaje
        {
            get => _porcentaje;
            set => _porcentaje = Math.Round(value, 6);
        }
    }
}
