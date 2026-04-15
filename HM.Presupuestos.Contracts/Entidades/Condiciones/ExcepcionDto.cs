
namespace HM.Presupuestos.Contratos.Entidades
{
    /// <summary>
    /// Clase para los objetos con los que se pinta el grid de excepciones y luego se mandan al Servicio para grabar en BD
    /// </summary>
    public  class ExcepcionDto
    {
        public string Key => $"{CodigoCondicionMedio}";

        public int CodigoMedio { get; set; }
        public int Jerarquia { get; set; }

        public bool MedioAccesible { get; set; }


        /// <summary>
        /// Combinacion de Cod_Concepto_Condicion - Cod_Condicion_Medio separados por comas
        /// </summary>
        public string CodigoCondicionMedio { get; set; } = "";

        /// <summary>
        /// Lista de objetos con dos propiedades: 1.- El codigo del concepto (SAG, ManPower o Devolucion) y 3.- El codigo Condicion Medio
        /// </summary>
        public List<CodigosConceptoCondicion> CodigosConceptosCondiciones {
            get 
            {
                List<CodigosConceptoCondicion> resultado = new();
                var datos = CodigoCondicionMedio.Split(',');
                foreach (var dato in datos)
                {
                    var codigos = dato.Split("-");
                    CodigosConceptoCondicion ccc = new();
                    //Solo para los distintos a esto, que es por los codigos para las nuevas excepciones que son del tipo -1, -2, etc...
                    if (!string.IsNullOrEmpty(codigos[0]))
                    {
                        ccc.CodigoConcepto = int.Parse(codigos[0]);
                        ccc.CodigoCondicion = int.Parse(codigos[1]);
                        resultado.Add(ccc);
                    }
                }
                return resultado;
            }
        }

        public string DescripcionMedio { get; set; } = string.Empty;


        private decimal? _pctSAG;
        public decimal? PctSAG
        {
            get => _pctSAG;
            set => _pctSAG = value.HasValue ? Math.Round(value.Value, 2) : null;
        }

        private decimal? _pctManPower;
        public decimal? PctManPower
        {
            get => _pctManPower;
            set => _pctManPower = value.HasValue ? Math.Round(value.Value, 2) : null;
        }

        private decimal? _pctDevolucion;
        public decimal? PctDevolucion
        {
            get => _pctDevolucion;
            set => _pctDevolucion = value.HasValue ? Math.Round(value.Value, 2) : null;
        }

        public int  IndicadorCalculoDevolucion { get; set; }

        public int? CodigoAlcance { get; set; }
        public int? CodigoDisciplina { get; set; }
        public int? CodigoDiversified { get; set; }
        public int? CodigoObjetivo { get; set; }
        public int? CodigoTipoCompra { get; set; }



        public int? CodigoTipoDisciplina { get; set; }
        public int? CodigoDisciplinaGrupo { get; set; }

        /// <summary>
        /// Naturaleza
        /// </summary>
        public List<CodigoDescripcion>? TiposDisciplinaDisponibles { get; set; }
        public List<CodigoDescripcion>? DisciplinasGrupoDisponibles { get; set; }
        public List<CodigoDescripcion>? DisciplinasDisponibles { get; set; }
        public List<CodigoDescripcion>? ObjetivosDisponibles { get; set; }
        public List<CodigoDescripcion>? TiposCompraDisponibles { get; set; }
       


    }

    public class CodigosConceptoCondicion
    {
        public int CodigoConcepto { get; set; }
        public int CodigoCondicion { get; set; }
    }
}
