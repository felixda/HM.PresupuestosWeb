using HM.Presupuestos.Domain.Compartido;
using System.Text.Json.Serialization;

namespace HM.Presupuestos.Domain.Entidades
{
    [Serializable]
    public class FiltroComprobarNetoVentaOrigenJSON
    {
        [JsonPropertyName("des_tipo_carga")]
        public string DescripcionTipoCarga {
            get
            {
                return  OrigenTipoCarga.ToString();
            }
        }

        [JsonPropertyName("cod_pais")]
        public int CodigoPais { get; set; }

        [JsonPropertyName("cod_tipo_carga")]
        public OrigenVersion OrigenTipoCarga { get; set; }

        [JsonPropertyName("anio")]
        public int? Anio { get; set; }

        [JsonPropertyName("cod_version")]
        public int? CodigoVersion { get; set; }

        [JsonPropertyName("mes_desde")]
        public int MesDesde { get; set; }

        [JsonPropertyName("mes_hasta")]
        public int MesHasta { get; set; }

        [JsonPropertyName("ind_interco")]
        public int IndInterco { get; set; } = 0;


        [JsonPropertyName("ind_ajuste")]
        public int IndCopiarAjustes { get; set; } = 0;

        [JsonPropertyName("cod_network")]
        public int[] Networks { get; set; } = [];

        [JsonPropertyName("cod_grupo")]
        public int[] GruposCliente { get; set; } = [];

        [JsonPropertyName("cod_alcance")]
        public int[] Alcances { get; set; } = [];

        [JsonPropertyName("cod_tipologia")]
        public int[] Tipologias { get; set; } = [];

        [JsonPropertyName("cod_diversified")]
        public int[] DiversifiedNBCs { get; set; } = [];

        [JsonPropertyName("cod_disciplina")]
        public int[] Disciplinas { get; set; } = [];

        [JsonPropertyName("cod_tipo_compra")]
        public int[] TiposCompra { get; set; } = [];

        [JsonPropertyName("cod_objetivo")]
        public int[] Objetivos { get; set; } = [];

        [JsonPropertyName("cod_medio")]
        public int[] CodigosMedios { get; set; } = [];

        [JsonPropertyName("cod_usuario")]
        public required int CodigoUsuario { get; set; }

    }
}


