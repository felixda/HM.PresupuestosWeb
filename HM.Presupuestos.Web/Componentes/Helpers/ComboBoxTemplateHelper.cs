
using Microsoft.AspNetCore.Components.Rendering;

namespace HM.Presupuestos.Web.Componentes.Helpers
{
    public static class ListasTemplateHelper
    {
        public static RenderFragment GetComboBoxItemTemplate<T>(ComboBoxItemDisplayTemplateContext<T> context)
        where T : IConIcono
        {
            return builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "cmb-template-container");

                // Icono
                var iconFragment = GetTemplateIcon(context.DataItem);
                iconFragment(builder);

                // Texto
                builder.AddContent(2, context.HighlightedDisplayText);

                builder.CloseElement();
            };
        }

        public static RenderFragment GetComboBoxEditBoxTemplate<T>(T item) where T : IConIcono
        {
            return builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "cmb-template-container");

                // Render icono
                var iconFragment = GetTemplateIcon(item);
                iconFragment(builder);

                // Render DxInputBox (usa componente desde C#)
                builder.OpenComponent<DevExpress.Blazor.DxInputBox>(2);
                builder.CloseComponent();

                builder.CloseElement();
            };
        }

        public static RenderFragment GetListBoxItemTemplate<T>(T dataItem) where T : IConIcono
        {
            return builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "lbx-template-container");

                // Icono
                var iconFragment = GetTemplateIcon(dataItem);
                iconFragment(builder);

                // Texto (usa ToString o una propiedad como Descripcion)
                builder.AddContent(2, dataItem?.DisplayText);

                builder.CloseElement();
            };
        }

        public static RenderFragment GetListBoxItemTemplate<T>(T dataItem, string displayText, string searchText)
            where T : IConIcono
        {
            return builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "lbx-template-container");

                // Icono
                var iconFragment = GetTemplateIcon(dataItem);
                iconFragment(builder);

                // Texto con highlight manual
                RenderHighlightedText(builder, displayText, searchText);

                builder.CloseElement();
            };
        }
        private static void RenderHighlightedText(RenderTreeBuilder builder, string text, string searchText)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (!string.IsNullOrEmpty(searchText) &&
                text.Contains(searchText, StringComparison.OrdinalIgnoreCase))
            {
                var idx = text.IndexOf(searchText, StringComparison.OrdinalIgnoreCase);
                var before = text.Substring(0, idx);
                var match = text.Substring(idx, searchText.Length);
                var after = text.Substring(idx + searchText.Length);

                builder.AddContent(5, before);
                builder.OpenElement(6, "span");
                builder.AddAttribute(7, "style", "background-color:yellow");
                builder.AddContent(8, match);
                builder.CloseElement();
                builder.AddContent(9, after);
            }
            else
            {
                builder.AddContent(10, text);
            }
        }

        public static RenderFragment GetTemplateIcon<T>(T dataItem) where T : IConIcono
        {
            return builder =>
            {
                if (!string.IsNullOrEmpty(dataItem?.IconoCssClass))
                {
                    builder.OpenElement(0, "span");
                    builder.AddAttribute(1, "class", $"cmb-template-icon {dataItem.IconoCssClass}");
                    builder.CloseElement();
                }
            };
        }

        

    }
}



