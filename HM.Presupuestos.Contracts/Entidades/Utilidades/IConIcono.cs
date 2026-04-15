

namespace HM.Presupuestos.Contratos.Entidades
{
    public interface IConIcono
    {
        string? IconoCssClass { get; } //Icono a mostrar en cada item de la lista desplegable
        string DisplayText { get; } //Texto que se muestra en cada item de la lista desplegable
    }
    
    /// <summary>
    /// Interfaz que deben tener las clases de los objetos que se enlazan a los combobox para poder 
    /// obtener el o los codigos de los objetos seleccionados
    /// </summary>
    public interface IConCodigo
    {
        int Codigo { get; }
    }
}
