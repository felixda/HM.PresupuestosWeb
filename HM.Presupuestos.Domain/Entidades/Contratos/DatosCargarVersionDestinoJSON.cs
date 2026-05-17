
using HM.Presupuestos.Domain.Compartido;
using System.Text.Json.Serialization;

namespace HM.Presupuestos.Domain.Entidades
{
    [Serializable]
    public class DatosCargarVersionDestinoJSON
    {
        [JsonPropertyName("cod_version")]
        public required int CodigoVersion { get; set; }

        [JsonPropertyName("cod_usuario")]
        public required int CodigoUsuario { get; set; }

        [JsonPropertyName("lineas")]
        public List<Linea> Lineas { get; set; } = new List<Linea>();
    }

    [Serializable]
    public class Linea
    {
        [JsonPropertyName("tipo_carga")]
        public string DescripcionTipoCarga 
        { 
            get 
            {
                return Origen.ToString();
            } 
        }

        [JsonPropertyName("cod_tipo_carga")]
        public required OrigenVersion Origen { get; set; }

        [JsonPropertyName("anio")]
        public int? Anio { get; set;  } //Solo se pasa si es origen Inges

        [JsonPropertyName("cod_version")]
        public int? CodigoVersionOrigen { get; set; } //Solo se pasa si es origen presupuestos

        [JsonPropertyName("mes_desde")]
        public int MesDesde { get;set; }

        [JsonPropertyName("mes_hasta")]
        public int MesHasta { get; set;}

        [JsonPropertyName("ind_interco")]
        public int IndInterco { get; set; }

        [JsonPropertyName("ind_ajuste")]
        public int IndCopiarAjustes { get; set; }


        [JsonPropertyName("cod_medio")]
        public List<MedioPorcentaje> Medios { get; set; } = [];

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

    }

    

}


