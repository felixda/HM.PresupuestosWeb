
    using System.Collections.Generic;
    using DevExpress.Blazor.Internal;

    namespace HM.Presupuestos.Server.Utils;
    public class DrawerUtils
    {
        public static Dictionary<string, object> GetToolbarButtonAttributes(bool isOpen, string? drawerName = null)
        {
            if (!string.IsNullOrEmpty(drawerName))
                drawerName += " ";
            return new Dictionary<string, object> {
            { A11yAriaAttributeUtils.AriaLabel, isOpen ? $"Close {drawerName}Drawer" : $"Open {drawerName}Drawer" },
            { A11yAriaAttributeUtils.AriaChecked, isOpen.ToString() }
        };
        }

    // A11yAriaAttributeUtils es una utilidad o conjunto de funciones para manejar atributos ARIA (Accessible Rich Internet Applications), 
    // que son usados en el desarrollo web para mejorar la accesibilidad de las interfaces de usuario. Uno de los atributos ARIA más comunes es aria-label,
    // que proporciona una descripción accesible para un elemento que es leído por los lectores de pantalla, pero no es visible para los usuarios.


}

