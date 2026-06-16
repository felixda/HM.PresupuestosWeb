# Guía de Markup Razor — Páginas Blazor Server

Patrones para el fichero `.razor`: estructura, directivas, guard de permisos, layout y componentes DevExpress.

Ver la guía del code-behind en: [blazor-codebehind.md](blazor-codebehind.md)

---

## 1. Estructura de Ficheros

Cada página se compone de tres ficheros colocados juntos:

```
Pages/
  MiSeccion/
    MiPagina.razor        ← markup, directivas, estructura HTML  ← este fichero
    MiPagina.razor.cs     ← lógica C# (partial class)           → blazor-codebehind.md
    MiPagina.razor.css    ← estilos CSS scoped (puede estar vacío)
```

---

## 2. Plantilla Completa — `MiPagina.razor`

```razor
@page "/seccion/mi-pagina"

@* Solo los @using que no están en _Imports.razor *@
@using HM.Presupuestos.Domain.Entidades
@using HM.Presupuestos.Web.Componentes.Base

@inherits ContextProtegido
@attribute [Authorize]
@attribute [StreamRendering(true)]

@* === GUARD DE PERMISOS === *@
@if (!UsuarioCargado || ValidandoPermisos || TienePermiso == null)
{
    @* Silencio mientras carga — no mostrar spinner aquí *@
}
else if (TienePermiso == false)
{
    <AccesoDenegado Titulo="@TituloPagina" />
}
else
{
    <PageHeader CodigoMenu="@CodigosMenu.MiPagina"
                Titulo="@TituloPagina"
                TextoToolTipAyuda="@ObtenerTexto(TextosApp.Pages.MiPagina.ToolTip)" />

    <div class="layoutContainer">

        @* === FILTROS === *@
        <DxFormLayout>
            <DxFormLayoutGroup CssClass="filterContainer"
                               Caption="@ObtenerTexto(TextosApp.Common.Filtros)"
                               ExpandButtonDisplayMode="GroupExpandButtonDisplayMode.Start"
                               AnimationType="LayoutAnimationType.Slide">
                @* Combos y filtros aquí *@
            </DxFormLayoutGroup>
        </DxFormLayout>

        @* === GRID PRINCIPAL === *@
        <DxFormLayout>
            <DxFormLayoutGroup Caption="@ObtenerTexto(TextosApp.Pages.MiPagina.Titulo)"
                               ColSpanSm="12" ColSpanMd="12" ColSpanLg="12" ColSpanXl="12" ColSpanXxl="12">
                <DxFormLayoutItem ColSpanSm="12" ColSpanMd="12" ColSpanLg="12" ColSpanXl="12" ColSpanXxl="12">
                    <DxGrid @ref="Grid"
                            Data="_datos"
                            KeyFieldName="Codigo"
                            ShowFilterRow="true"
                            ValidationEnabled="false"
                            ColumnResizeMode="GridColumnResizeMode.NextColumn"
                            TextWrapEnabled="false"
                            HighlightRowOnHover="true"
                            PageSizeSelectorVisible="true"
                            PageSizeSelectorItems="@(new int[] { 10, 25, 50 })"
                            PageSize="25">
                        <ToolbarTemplate>
                            <DxButton CssClass="toolbarButtonCustom"
                                      RenderStyle="ButtonRenderStyle.None"
                                      Text="@ObtenerTexto(TextosApp.Botones.Añadir)"
                                      Click="NuevoAsync"
                                      IconCssClass="fa-solid fa-plus" />
                        </ToolbarTemplate>
                        <Columns>
                            <DxGridDataColumn FieldName="@nameof(MiEntidad.Codigo)"
                                              Caption="@ObtenerTexto(TextosApp.Common.Codigo)"
                                              Width="8%"
                                              ReadOnly="true"
                                              CaptionAlignment="GridTextAlignment.Right"
                                              TextAlignment="GridTextAlignment.Right">
                                <CellDisplayTemplate>
                                    <span class="dxgrid-readonly-cell">@context.DisplayText</span>
                                </CellDisplayTemplate>
                            </DxGridDataColumn>
                            <DxGridDataColumn FieldName="@nameof(MiEntidad.Descripcion)"
                                              Caption="@ObtenerTexto(TextosApp.Common.Descripcion)"
                                              CaptionAlignment="GridTextAlignment.Left" />
                            <DxGridCommandColumn Width="5%" NewButtonVisible="false">
                                <CellDisplayTemplate>
                                    <div class="grid-cell-align-center">
                                        <DxButton IconCssClass="fas fa-trash"
                                                  RenderStyle="ButtonRenderStyle.Link"
                                                  aria-label="Delete"
                                                  title="@ObtenerTexto(TextosApp.Common.EliminarRegistro)"
                                                  Click="@(() => EliminarAsync(context.DataItem))" />
                                    </div>
                                </CellDisplayTemplate>
                            </DxGridCommandColumn>
                        </Columns>
                    </DxGrid>
                </DxFormLayoutItem>
            </DxFormLayoutGroup>
        </DxFormLayout>

    </div>

    @* === POPUP EDICIÓN (fuera del layoutContainer) === *@
    <DxPopup @bind-Visible="EsPopupVisible"
             HeaderText="@TituloPopup"
             Width="600px"
             CloseOnOutsideClick="false">
        <BodyContentTemplate>
            <DxFormLayout>
                <DxFormLayoutItem Caption="@ObtenerTexto(TextosApp.Common.Descripcion)"
                                  CaptionPosition="CaptionPosition.Vertical"
                                  ColSpanSm="12">
                    <DxTextBox @bind-Text="@EntidadEnEdicion.Descripcion" />
                </DxFormLayoutItem>
            </DxFormLayout>
        </BodyContentTemplate>
        <FooterContentTemplate>
            <DxButton Text="@ObtenerTexto(TextosApp.Botones.Grabar)"
                      RenderStyle="ButtonRenderStyle.None"
                      CssClass="toolbarButtonCustom"
                      IconCssClass="fa-solid fa-save"
                      Click="GuardarAsync" />
            <DxButton Text="@ObtenerTexto(TextosApp.Botones.Cancelar)"
                      RenderStyle="ButtonRenderStyle.None"
                      CssClass="toolbarButtonCustom"
                      IconCssClass="fa-solid fa-cancel"
                      Click="() => EsPopupVisible = false" />
        </FooterContentTemplate>
    </DxPopup>
}
```

---

## 3. Directivas — Reglas

| Directiva | Obligatoria | Notas |
|---|---|---|
| `@page "/ruta"` | Sí | Kebab-case: `/mantenimiento/mis-datos` |
| `@inherits ContextProtegido` | Sí (negocio) | O `@inherits Context` para componentes sin permisos |
| `@attribute [Authorize]` | Sí | Siempre en páginas de negocio |
| `@attribute [StreamRendering(true)]` | Recomendado | Mejora el tiempo de primera carga |
| `@using` | Solo si no está en `_Imports.razor` | No duplicar los ya globales |

---

## 4. Guard de Permisos

El bloque `@if` al inicio del markup es obligatorio en páginas que heredan de `ContextProtegido`. La secuencia de estados es:

```
1. !UsuarioCargado || ValidandoPermisos || TienePermiso == null  → silencio (vacío)
2. TienePermiso == false                                          → `<AccesoDenegado Titulo="@TituloPagina" />`
3. else                                                           → contenido real de la página
```

El contenido real siempre empieza con `<PageHeader>` y `<div class="layoutContainer">`.

---

## 5. Grid DevExpress — Markup

### Propiedades base del DxGrid

```razor
<DxGrid @ref="Grid"
        Data="_datos"
        KeyFieldName="Codigo"
        ShowFilterRow="true"
        ValidationEnabled="false"
        ColumnResizeMode="GridColumnResizeMode.NextColumn"
        TextWrapEnabled="false"
        HighlightRowOnHover="true"
        PageSizeSelectorVisible="true"
        PageSizeSelectorItems="@(new int[] { 10, 25, 50 })"
        PageSize="25">
```

### Toolbar con botones de acción

```razor
<ToolbarTemplate>
    <DxButton CssClass="toolbarButtonCustom"
              RenderStyle="ButtonRenderStyle.None"
              Text="@ObtenerTexto(TextosApp.Botones.Añadir)"
              Click="NuevoAsync"
              IconCssClass="fa-solid fa-plus" />
    <DxButton CssClass="toolbarButtonCustom"
              RenderStyle="ButtonRenderStyle.None"
              Text="@ObtenerTexto(TextosApp.Botones.Cancelar)"
              Click="CancelarAsync"
              IconCssClass="fa-solid fa-cancel"
              Enabled="@HayCambios" />
</ToolbarTemplate>
```

### Columna numérica de solo lectura

```razor
<DxGridDataColumn FieldName="@nameof(Entidad.Codigo)"
                  Caption="@ObtenerTexto(TextosApp.Common.Codigo)"
                  Width="8%"
                  ReadOnly="true"
                  CaptionAlignment="GridTextAlignment.Right"
                  TextAlignment="GridTextAlignment.Right">
    <FilterRowCellTemplate></FilterRowCellTemplate>
    <CellDisplayTemplate>
        <span class="dxgrid-readonly-cell">@context.DisplayText</span>
    </CellDisplayTemplate>
</DxGridDataColumn>
```

### Columna con ComboBox en edición

```razor
<DxGridDataColumn FieldName="@nameof(Entidad.CodigoTipo)"
                  Caption="@ObtenerTexto(TextosApp.Common.Tipo)"
                  Width="12%">
    <FilterRowCellTemplate></FilterRowCellTemplate>
    <CellDisplayTemplate>
        @_listaTipos.FirstOrDefault(o => o.Codigo == (int?)context.Value)?.Descripcion
    </CellDisplayTemplate>
    <CellEditTemplate>
        @{ var entidad = (Entidad)context.EditModel; }
        <DxComboBox Data="@_listaTipos"
                    TextFieldName="@nameof(CodigoDescripcion.Descripcion)"
                    ValueFieldName="@nameof(CodigoDescripcion.Codigo)"
                    @bind-Value="@entidad.CodigoTipo" />
    </CellEditTemplate>
</DxGridDataColumn>
```

### Columna de acciones (eliminar)

```razor
<DxGridCommandColumn Width="5%" NewButtonVisible="false">
    <FilterRowCellTemplate></FilterRowCellTemplate>
    <CellDisplayTemplate>
        @{ var entidad = (Entidad)context.DataItem; }
        <div class="grid-cell-align-center">
            <DxButton IconCssClass="fas fa-trash"
                      RenderStyle="ButtonRenderStyle.Link"
                      Disabled="@entidad.TieneDatosRelacionados"
                      title="@(entidad.TieneDatosRelacionados
                                    ? ObtenerTexto(TextosApp.Pages.MiPagina.NoSePuedeBorrar)
                                    : ObtenerTexto(TextosApp.Common.EliminarRegistro))"
                      Click="@(() => EliminarAsync(context.DataItem))" />
        </div>
    </CellDisplayTemplate>
</DxGridCommandColumn>
```

### Band column (columnas agrupadas)

```razor
<DxGridBandColumn Caption="@ObtenerTexto(TextosApp.Pages.MiPagina.GrupoColumnas)"
                  Width="40%"
                  CaptionAlignment="GridTextAlignment.Center">
    <Columns>
        <DxGridDataColumn FieldName="Campo1" Caption="..." Width="20%" />
        <DxGridDataColumn FieldName="Campo2" Caption="..." Width="20%" />
    </Columns>
</DxGridBandColumn>
```

---

## 6. ComboBox DevExpress — Markup

### Estándar con búsqueda y template

```razor
<DxComboBox Data="@_listaDatos"
            TextFieldName="@nameof(CodigoDescripcion.Descripcion)"
            ValueFieldName="@nameof(CodigoDescripcion.Codigo)"
            @bind-Value="@_valorSeleccionado"
            SearchMode="@ListSearchMode.AutoSearch"
            SearchFilterCondition="@ListSearchFilterCondition.Contains"
            ClearButtonDisplayMode="DataEditorClearButtonDisplayMode.Auto"
            SelectedDataItemChanged="(SelectedDataItemChangedEventArgs<CodigoDescripcion> args) => OnComboSelectedDataItemChangedAsync(args)">
    <EditBoxDisplayTemplate Context="ctx">
        @ListasTemplateHelper.GetComboBoxEditBoxTemplate(ctx.DataItem)
    </EditBoxDisplayTemplate>
</DxComboBox>
```

### Campo obligatorio con asterisco

Cuando un campo del filtro o formulario es **obligatorio**, se marca visualmente usando `CaptionTemplate` con la clase CSS `mandatory`.

> **Regla crítica**: en cuanto se usa `CaptionTemplate`, el control hijo **ya no puede ir suelto** dentro del `DxFormLayoutItem` — debe ir obligatoriamente dentro de `<Template>`. De lo contrario Blazor lanza `RZ9996: Unrecognized child content`.

```razor
<DxFormLayoutItem CaptionPosition="CaptionPosition.Vertical">
    <CaptionTemplate>
        <span>@ObtenerTexto(TextosApp.Common.Anio)</span><span class="mandatory"></span>
    </CaptionTemplate>
    <Template>                                  ← obligatorio cuando hay CaptionTemplate
        <DxComboBox Data="@Anios"
                    TextFieldName="Descripcion"
                    ValueFieldName="Codigo"
                    @bind-Value="@AnioSeleccionado" />
    </Template>
</DxFormLayoutItem>
```

El mismo patrón aplica a cualquier control (`DxDateEdit`, `DxTextBox`, `DxSpinEdit`, etc.):

```razor
<DxFormLayoutItem CaptionPosition="CaptionPosition.Vertical">
    <CaptionTemplate>
        <span>Fecha inicio</span><span class="mandatory"></span>
    </CaptionTemplate>
    <Template>
        <DxDateEdit @bind-Date="@FechaInicio" NullText="dd/mm/yyyy" />
    </Template>
</DxFormLayoutItem>
```

Campos **opcionales** no necesitan `CaptionTemplate` y pueden usar el atributo `Caption` directamente:

```razor
<DxFormLayoutItem Caption="Fecha fin" CaptionPosition="CaptionPosition.Vertical">
    <DxDateEdit @bind-Date="@FechaFin" NullText="dd/mm/yyyy" />
</DxFormLayoutItem>
```


### DropDownBox con selección múltiple

```razor
<DxDropDownBox @bind-Value="@_seleccionados"
               QueryDisplayText="@(arg => GetDropDownBoxTextoSeleccionados<CodigoDescripcion, string>(
                                               arg,
                                               x => x.Descripcion,
                                               ObtenerTexto(TextosApp.Common.Todos)
                                           ))"
               DropDownWidthMode="DropDownWidthMode.EditorWidth"
               NullText="@ObtenerTexto(TextosApp.Common.Todos)">
    <DropDownBodyTemplate>
        <DxListBox Data="@_listaMaestra"
                   TData="CodigoDescripcion"
                   TValue="CodigoDescripcion"
                   Values="@(context.DropDownBox.Value as IEnumerable<CodigoDescripcion>
                               ?? Enumerable.Empty<CodigoDescripcion>())"
                   ValuesChanged="context.DropDownBox.BeginUpdate" />
    </DropDownBodyTemplate>
</DxDropDownBox>
```

---

## 7. Popup DevExpress — Markup

El popup se sitúa **fuera** del `<div class="layoutContainer">`, al final del bloque `else`:

```razor
<DxPopup @bind-Visible="EsPopupVisible"
         HeaderText="@TituloPopup"
         Width="700px"
         CloseOnOutsideClick="false">
    <BodyContentTemplate>
        <DxFormLayout>
            <DxFormLayoutItem Caption="@ObtenerTexto(TextosApp.Common.Descripcion)"
                              CaptionPosition="CaptionPosition.Vertical"
                              ColSpanSm="12" ColSpanMd="6">
                <DxTextBox @bind-Text="@_entidadEnEdicion.Descripcion" />
            </DxFormLayoutItem>
            <DxFormLayoutItem Caption="@ObtenerTexto(TextosApp.Common.Anio)"
                              CaptionPosition="CaptionPosition.Vertical"
                              ColSpanSm="12" ColSpanMd="3">
                <DxComboBox Data="@_anios"
                            TextFieldName="Descripcion"
                            ValueFieldName="Codigo"
                            @bind-Value="@_entidadEnEdicion.Anio" />
            </DxFormLayoutItem>
        </DxFormLayout>
    </BodyContentTemplate>
    <FooterContentTemplate>
        <DxButton Text="@ObtenerTexto(TextosApp.Botones.Grabar)"
                  CssClass="toolbarButtonCustom"
                  RenderStyle="ButtonRenderStyle.None"
                  IconCssClass="fa-solid fa-save"
                  Click="GuardarAsync" />
        <DxButton Text="@ObtenerTexto(TextosApp.Botones.Cancelar)"
                  CssClass="toolbarButtonCustom"
                  RenderStyle="ButtonRenderStyle.None"
                  IconCssClass="fa-solid fa-cancel"
                  Click="() => EsPopupVisible = false" />
    </FooterContentTemplate>
</DxPopup>
```

---

## 8. Localización en Markup

Todos los textos visibles se obtienen siempre con `ObtenerTexto(TextosApp.*)`:

```razor
@* ✅ CORRECTO *@
<DxButton Text="@ObtenerTexto(TextosApp.Botones.Grabar)" />
<DxFormLayoutGroup Caption="@ObtenerTexto(TextosApp.Common.Filtros)">
<PageHeader TextoToolTipAyuda="@ObtenerTexto(TextosApp.Pages.MiPagina.ToolTip)" />

@* ❌ INCORRECTO — texto literal no localizable *@
<DxButton Text="Guardar" />
```

Añadir una nueva clave → seguir el proceso de `.github/prompts/anadir-traduccion.prompt.md`.

---

## 9. CSS Scoped (`*.razor.css`)

```css
/* Estilos específicos del componente — no afectan al resto de la app */

/* Para penetrar el shadow DOM de DevExpress */
::deep .dxbl-grid {
    max-height: 30rem;
    overflow-y: auto;
}

/* Clases de estado de grid ya definidas en comun.css — no redefinir:
   .dxgrid-readonly-cell, .grid-modified-cell, .grid-cell-align-center */

/* Variables corporativas — siempre var(--havas-color-*), nunca valor hardcodeado */
.miClase { background-color: var(--havas-color-primary); }

/* Bootstrap para layout y utilidades */
/* d-flex, gap-*, mt-*, col-*, etc. — no crear clases equivalentes */
```

Ver detalle completo en: [css-modules.md](css-modules.md)

---

## 10. Checklist — Fichero `.razor`

- [ ] Ruta `@page` en kebab-case: `/seccion/nombre-kebab`
- [ ] `@inherits ContextProtegido` (o `@inherits Context`)
- [ ] `@attribute [Authorize]`
- [ ] `@attribute [StreamRendering(true)]`
- [ ] Bloque guard completo: `@if (!UsuarioCargado || ...) / else if (TienePermiso == false) / else`
- [ ] `<PageHeader>` con `CodigoMenu`, `Titulo` y `TextoToolTipAyuda`
- [ ] `<div class="layoutContainer">` como contenedor raíz
- [ ] Filtros en `DxFormLayoutGroup` con `CssClass="filterContainer"`
- [ ] Grid dentro de `DxFormLayout > DxFormLayoutGroup > DxFormLayoutItem`
- [ ] Popup **fuera** de `layoutContainer`, al final del bloque `else`
- [ ] Todos los textos via `ObtenerTexto(TextosApp.*)`
- [ ] Fichero `.razor.css` creado junto al razor (aunque quede vacío)
