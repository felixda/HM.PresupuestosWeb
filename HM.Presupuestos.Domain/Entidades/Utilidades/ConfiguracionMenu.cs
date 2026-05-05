

namespace HM.Presupuestos.Domain.Entidades
{
    public class ConfiguracionMenu
    {
        public int CodigoMenu { get; set; }
        public string DescripcionMenu { get; set; } = "";
        public int? CodigoMenuPadre { get; set; }
        public int IndiceOrdenacion { get; set; }
        public bool Favorito { get; set; }
        public string Icono { get; set; } = "";
        public string Url { get; set; } = "";
        public List<ConfiguracionMenu> Submenus = new();
    }
}
