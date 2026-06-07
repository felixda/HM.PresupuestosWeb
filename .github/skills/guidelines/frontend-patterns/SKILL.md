---
name: frontend-patterns
description: Este skill debe usarse al trabajar con componentes Blazor Server, páginas con código-separado (*.razor + *.razor.cs), DevExpress UI (DxGrid, DxPopup, DxFormLayout), o cuando el usuario pregunta sobre ciclo de vida de componentes, inyección de dependencias en UI, mensajes al usuario, o localización de textos.
---

# Patrones Frontend — Blazor Server

Estándares para componentes Blazor Server con DevExpress UI: estructura de páginas, ciclo de vida, inyección de dependencias, estado, mensajes y localización. Aplica a `HM.Presupuestos.Web`.

## Estructura de Páginas

Cada página usa el patrón **código-separado**: un archivo `.razor` (markup) y un archivo `.razor.cs` (lógica).

- El `.razor` declara `@inherits ContextProtegido` (o `@inherits Context`) y el markup
- El `.razor.cs` declara `public partial class NombrePagina` con campos, propiedades e inyecciones
- Las páginas que requieren autenticación llevan `@attribute [Authorize]`

```razor
@* Versiones.razor *@
@page "/mantenimiento/versiones"
@inherits ContextProtegido
@attribute [Authorize]

@if (!UsuarioCargado || ValidandoPermisos || TienePermiso == null)
{
    <LayerOverlay />   @* estado de carga *@
}
else if (TienePermiso == false)
{
    <div class="alert alert-danger">Acceso Denegado</div>
}
else
{
    <PageHeader CodigoMenu="@CodigosMenu.Versiones" Titulo="@TituloPagina" />
    <!-- Grid y UI específica -->
}
```

```csharp
// Versiones.razor.cs
public partial class Versiones   // partial — se une con el .razor en compilación
{
    #region Inyección de Dependencias
    [Inject] protected IVersionesService VersionesService { get; set; } = default!;
    #endregion

    #region Private Properties
    private List<Version> _listVersion = [];
    private IGrid? GridVersiones { get; set; }
    #endregion
}
```

## Herencia: `Context` vs `ContextProtegido`

| Clase base | Cuándo usarla |
|---|---|
| `Context` | Componentes sin control de acceso: `PageHeader`, `NavMenu`, componentes compartidos |
| `ContextProtegido` | Páginas con permisos: `Indicadores`, `Versiones`, `Condiciones`, cualquier página de negocio |

`Context` proporciona: `MensajesHelper`, `NavigationManager`, `ObtenerTexto()`, `EjecutarAsync()`, `TituloPagina`, `ContextoUsuario`, `Usuario`.

`ContextProtegido` añade: `TienePermiso`, `ValidandoPermisos`, y el hook `InicializarPaginaAsync()`.

## Ciclo de Vida de Componentes

**Nunca usar `OnInitializedAsync()` para lógica que depende del usuario.** El usuario se carga de forma asíncrona a través de `ISesionUsuario`; usar los hooks del framework:

```
OnAfterRenderAsync (firstRender)
  └─ OnUsuarioDisponibleAsync()          ← usuario cargado; disponible en TODOS los componentes
       └─ [solo ContextProtegido]
            └─ OnPermisoValidadoAsync()  ← permisos OK; llama automáticamente a:
                 └─ InicializarPaginaAsync()  ← AQUÍ va la carga inicial de datos
            └─ OnPermisoDenegadoAsync()  ← si no tiene permiso
```

```csharp
// ✅ CORRECTO — carga de datos en InicializarPaginaAsync
protected override async Task InicializarPaginaAsync()
{
    DatosIndicadores = await IndicadoresService.ObtenerIndicadoresConIdiomas(null);

    if (DatosIndicadores.Count == 0)
        await MensajesHelper.MostrarMensajeInfo(TituloPagina,
            ObtenerTexto(AppResources.Mensajes.RegistrosNoEncontrados));
}

protected override Task OnPermisoDenegadoAsync()
{
    return Task.CompletedTask;
}

// ❌ INCORRECTO — carga en OnInitializedAsync (usuario aún no disponible)
protected override async Task OnInitializedAsync()
{
    DatosIndicadores = await IndicadoresService.ObtenerIndicadoresConIdiomas(null);
}
```

## Inyección de Dependencias con `[Inject]`

Siempre `protected`, siempre `= default!`. Agrupar en `#region Inyección de Dependencias`:

```csharp
#region Inyección de Dependencias

[Inject] protected IVersionesService VersionesService { get; set; } = default!;
[Inject] protected IIndicadoresService IndicadoresService { get; set; } = default!;
[Inject] protected TraduccionesHelper TraduccionesHelper { get; set; } = default!;

#endregion
```

Los servicios heredados de `Context` (como `MensajesHelper`, `NavigationManager`, `LocalizadorRecursos`) **no se redeclaran** en las páginas; ya están disponibles.

## `EjecutarAsync` — Wrapper de Operaciones Async

Todas las operaciones async en páginas se envuelven en `EjecutarAsync(...)`. Gestiona automáticamente: overlay de carga, manejo de errores y mensaje al usuario.

```csharp
// Patrón estándar — acción sin retorno
await EjecutarAsync(async () =>
{
    _listVersion = await VersionesService.ObtenerVersiones(anioSeleccionado);
    _listOriginVersion = DatosHelper.ClonarObjeto(_listVersion);
});

// Con retorno
var versiones = await EjecutarAsync(
    async () => await VersionesService.ObtenerVersiones(anio),
    defaultValue: []
) ?? [];

// Sin overlay (operaciones rápidas de UI)
await EjecutarAsync(() =>
{
    FiltroActivo = true;
}, showOverlay: false);

// Con mensaje de error personalizado
await EjecutarAsync(async () =>
{
    await IndicadoresService.Eliminar(indicador);
    DatosIndicadores.Remove(indicador);
    GridIndicadores.Reload();
}, ObtenerTexto(AppResources.Mensajes.ErrorDelete));
```

## Estado de la Página — Campos Privados

El estado de UI se almacena en campos `_privados` (prefijo `_`). Organizar con `#region`:

```csharp
#region Private Properties

#region Filter
private CodigoDescripcion? ItemYearSelected { get; set; }
#endregion

#region Grid Versiones
private IGrid? GridVersiones { get; set; }
private List<Version> _listVersion = [];
private List<Version> _listOriginVersion = [];
private List<Indicador> _listMasterIndicador = [];
#endregion

#region Popup Edición
private bool EsPopupEdicionVisible { get; set; }
private Version? _versionEnEdicion;
private ModoEdicion _modoEdicion = ModoEdicion.Alta;
#endregion

#endregion

// Propiedades computadas de estado
private bool HayCambios =>
    JsonSerializer.Serialize(_listVersion) != JsonSerializer.Serialize(_listOriginVersion);
private bool HayCambiosPendientes =>
    _condicionesNoGuardados.Count > 0;
```

## DevExpress — DxGrid

```csharp
// Referencia al componente (nullable si se usa IGrid)
private IGrid? GridVersiones { get; set; }
// O tipo concreto si se necesitan más operaciones
private DxGrid GridIndicadores { get; set; } = new DxGrid();
```

```razor
<DxGrid @ref="GridIndicadores"
        Data="DatosIndicadores"
        KeyFieldName="Codigo"
        RowDoubleClick="GridIndicadores_DoubleClick"
        EditMode="GridEditMode.EditCell"
        EditModelSaving="GridIndicadores_EditModelSaving">
    <Columns>
        <DxGridDataColumn FieldName="Descripcion" Caption="@ObtenerTexto(AppResources.Common.Descripcion)" />
    </Columns>
</DxGrid>
```

```csharp
// Operaciones frecuentes
GridIndicadores.Reload();
GridIndicadores.ClearFilter();
var indicador = (Indicador?)GridIndicadores.GetDataItem(e.VisibleIndex);
await GridIdiomasIndicador.StartEditNewRowAsync();
GridIdiomasIndicador.CancelEditAsync();
```

Los event handlers de DevExpress que devuelven `void` son la **única excepción** a la regla del sufijo `Async`:
```csharp
private async void GridVersiones_EditModelSaving(GridEditModelSavingEventArgs e) { ... }
private async void GridVersiones_CustomizeElement(GridCustomizeElementEventArgs ea) { ... }
```

## DevExpress — DxPopup

El popup se controla con una propiedad booleana ligada a `@bind-Visible`:

```csharp
private bool EsPopupEdicionVisible { get; set; }

private async Task EditarIndicadorAsync(Indicador indicador)
{
    _indicadorEnEdicion = indicador;
    EsPopupEdicionVisible = true;   // abrir
}

private async Task CerrarPopupAsync()
{
    EsPopupEdicionVisible = false;  // cerrar
}
```

```razor
<DxPopup @bind-Visible="EsPopupEdicionVisible"
         HeaderText="@TituloPopup"
         Width="800px">
    <BodyContentTemplate>
        <DxFormLayout>
            <!-- Campos del formulario -->
        </DxFormLayout>
    </BodyContentTemplate>
    <FooterContentTemplate>
        <DxButton Text="@ObtenerTexto(AppResources.Botones.Guardar)" Click="GuardarAsync" />
        <DxButton Text="@ObtenerTexto(AppResources.Botones.Cancelar)" Click="CerrarPopupAsync" />
    </FooterContentTemplate>
</DxPopup>
```

## Mensajes al Usuario — `MensajesHelper`

Siempre usar `MensajesHelper`, nunca alertas nativas de JavaScript ni mensajes inline en markup:

```csharp
// Información (sin resultados, aviso neutral)
await MensajesHelper.MostrarMensajeInfo(TituloPagina,
    ObtenerTexto(AppResources.Mensajes.RegistrosNoEncontrados));

// Éxito (operación completada)
await MensajesHelper.MostrarMensajeExito(TituloPagina,
    ObtenerTexto(AppResources.Common.DatosGrabados));

// Advertencia (validación, estado inconsistente)
await MensajesHelper.MostrarMensajeAviso(TituloPagina, mensajeValidacion);

// Error (técnico o inesperado)
await MensajesHelper.MostrarMensajeError(TituloPagina,
    ObtenerTexto(AppResources.Mensajes.ErrorGeneral));

// Confirmación (acción destructiva)
bool confirmado = await MensajesHelper.MostrarMensajeParaConfirmacion(
    TituloPagina,
    ObtenerTexto(AppResources.Mensajes.ConfirmacionEliminar));
if (!confirmado) return;
```

## Localización — `ObtenerTexto` y `AppResources`

Todos los textos visibles al usuario se obtienen mediante `ObtenerTexto(clave)`. Las claves están centralizadas en `AppResources.cs`:

```csharp
// ✅ CORRECTO — clave tipada desde AppResources
ObtenerTexto(AppResources.Common.Descripcion)
ObtenerTexto(AppResources.Botones.Guardar)
ObtenerTexto(AppResources.Mensajes.RegistrosNoEncontrados)
ObtenerTexto(AppResources.Pages.Versiones.TipoVersion.User)

// ❌ INCORRECTO — string literal hardcodeado
ObtenerTexto("Common:Descripcion:label")   // funciona pero no tipado
// o peor:
"Descripción"                               // no localizable
```

En markup `.razor`:
```razor
<DxButton Text="@ObtenerTexto(AppResources.Botones.Guardar)" />
<PageHeader Titulo="@TituloPagina"
            TextoToolTipAyuda="@ObtenerTexto(AppResources.Pages.Indicadores.ToolTip)" />
```

## ViewModels — Estado UI sobre DTOs

Los ViewModels viven exclusivamente en `HM.Presupuestos.Web`. Heredan de los DTOs de Application añadiendo propiedades de estado UI que no pertenecen al dominio:

```csharp
// CondicionViewModel.cs — en Web/Pages/Condiciones/
public class CondicionViewModel : CondicionDto
{
    // Estado de UI: si el medio es accesible para edición en el grid
    public bool MedioAccesible { get; set; }
}

// Uso en páginas
private List<CondicionViewModel> _condiciones = [];
private Dictionary<CondicionViewModel, DatosCondicionCambiados> _condicionesNoGuardados { get; } = [];
```

Los servicios de Application **nunca reciben ViewModels**; solo reciben DTOs o entidades de dominio.

## `StateHasChanged` — Cuándo Usarlo

`StateHasChanged()` es necesario cuando el cambio de estado viene de fuera del ciclo de renderizado de Blazor (eventos externos, callbacks async no iniciados por UI):

```csharp
// ✅ Necesario — callback de evento externo (cambio de idioma)
protected async Task ActualizarIdioma()
{
    await InvokeAsync(StateHasChanged);
}

// ✅ Necesario — usuario cargado desde evento asíncrono externo
await OnUsuarioDisponibleAsync();
await InvokeAsync(StateHasChanged);

// ✅ Necesario — EjecutarAsync no llama a StateHasChanged automáticamente
// si el campo modificado no está bound con @bind
await EjecutarAsync(async () =>
{
    _listVersion = await VersionesService.ObtenerVersiones(anio);
});
// Blazor re-renderiza al completar el await; StateHasChanged solo es explícito cuando viene de fuera
```

## CSS y Estilos

El proyecto usa **tres capas de estilos**: CSS isolation por componente (`*.razor.css`), estilos globales (`comun.css` con variables corporativas `--havas-color-*`) y temas Bootstrap+DevExpress dinámicos (sistema `ThemeSwitcher`).

Reglas clave:
- Los estilos específicos de un componente van en `ComponentName.razor.css` (colocado junto al `.razor`)
- Los colores corporativos se usan siempre como `var(--havas-color-*)`, nunca hardcodeados
- Bootstrap v5 se usa para layout y utilidades (`d-flex`, `gap-*`, `mt-*`)
- Las clases de estado de DxGrid (`.grid-modified-cell`, `.dxgrid-readonly-cell`, etc.) están en `comun.css`
- No usar `!important`; sobreescribir DevExpress mediante variables CSS
- Los estilos inline solo se aceptan para valores dinámicos calculados en C#

Ver detalle completo: [references/css-modules.md](references/css-modules.md)

## Creación de Nuevas Páginas

Para crear una nueva página desde cero, consultar las guías especializadas:

- Markup y componentes DevExpress: [references/blazor-razor.md](references/blazor-razor.md)
- Lógica C# y ciclo de vida: [references/blazor-codebehind.md](references/blazor-codebehind.md)

## Reglas No Negociables

- Nunca lógica de datos en `OnInitializedAsync`; siempre en `InicializarPaginaAsync`
- Nunca código sin `EjecutarAsync` para operaciones async en páginas (excepto `InicializarPaginaAsync`, que ya está dentro del ciclo de overlay)
- Nunca textos literales en la UI; siempre `ObtenerTexto(AppResources.*)`
- Nunca ViewModels en la capa Application o Domain
- Nunca alertas JavaScript nativas; siempre `MensajesHelper`
- Siempre `partial class` en el code-behind
- Siempre `= default!` en propiedades `[Inject]`
- Siempre `#region` para organizar campos de página por responsabilidad
