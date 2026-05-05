

using HM.Presupuestos.Domain.Comun;

namespace HM.Presupuestos.Domain.Entidades
{
    public class VersionLinea
    {
        public required string MonthFrom { get; set; }
        public required int CodeMonthFrom { get; set; }
        public required string MonthTo { get; set; } 
        public required int CodeMonthTo { get; set; }

        public required IEnumerable<MedioPorcentaje> ListMedia { get; set; }
        public IEnumerable<CodigoDescripcion> ListNetwork { get; set; } = [];
        public IEnumerable<GrupoClientesConNetwork>? ListGroupCustomer { get; set; }
        public  IEnumerable<CodigoDescripcion>? ListScope { get; set; }
        public IEnumerable<CodigoDescripcion>? ListTypology { get; set; }
        public IEnumerable<CodigoDescripcion>? ListDiversifiedNBC { get; set; }
        public IEnumerable<CodigoDescripcion>? ListDiscipline { get; set; }
        public IEnumerable<CodigoDescripcion>? ListPurchaseType { get; set; }
        public IEnumerable<CodigoDescripcion>? ListTarget { get; set; }
        
        public required OrigenVersion CodeSource { get; set; }
        public string Source
        {
            get
            {
                return CodeSource.ToString();
            }
        }

        public int? VersionYear { get; set; }
        public int? VersionCodeSource { get; set; }
        public string? VersionSource { get; set; }
        public int? IngesYear { get; set; }
       // public bool Closing { get; set; } = false;
        public bool Interco { get; set; } = false;

        public bool CopiarAjustes { get; set; } = false;
                
        
        //Network description list ","
        public string NetworkNameStringList
        {
            get
            {
                var itemNameList = "";
                if (ListNetwork != null && ListNetwork.Count() > 0)
                {
                    itemNameList = string.Join(", ", ListNetwork.Select(x => x.Descripcion.Trim()).Distinct());
                }
                return itemNameList;
            }
        }

        //Network description list ","
        public string MediaNameStringList
        {
            get
            {
                var itemNameList = "";
                if (ListMedia != null && ListMedia.Count() > 0)
                {
                    itemNameList = string.Join(", ", ListMedia.Select(x => x.DescripcionMedio.Trim()).Distinct());
                }
                return itemNameList;
            }
        }
    }

}
