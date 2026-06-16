# Guía de Code-Behind C# — Páginas Blazor Server

Patrones para el fichero `.razor.cs`: estructura de clase, ciclo de vida, inyección de dependencias, operaciones async, mensajes y ViewModels.

Ver la guía de markup en: [blazor-razor.md](blazor-razor.md)

---

## 0. Convenciones de nomenclatura

| Tipo de miembro | Convención | Ejemplo |
|---|---|---|
| Miembro **bindeado** en el razor (`@bind-Value`, `@bind-Date`, `Data=`, `Enabled=`, etc.) | `PascalCase` con `{ get; set; }` | `private int? TipoSeleccionado { get; set; }` |
| Lista/colección que actúa como fuente de datos (`Data=`) | `PascalCase` con `{ get; set; }` | `private List<CodigoDescripcion> TiposAuditoria { get; set; } = [];` |
| Campo **interno** no expuesto al razor | `_camelCase` | `private ModoEdicion _modoEdicion = ModoEdicion.Alta;` |
| Propiedad computada (solo lectura) | `PascalCase` | `private bool HayCambios => ...;` |
| Referencia a componente DevExpress (`@ref`) | `PascalCase` con `{ get; set; }` | `private DxGrid Grid { get; set; } = new DxGrid();` |
| Event handler DevExpress (`EventName=`) | `On` + Componente + Descripción (PascalCase, sin guión bajo) | `OnGridVersionesElementCustomized`, `OnGridVersionesEditModelSaving` |

> **Regla clave**: si el miembro aparece en el markup del `.razor` como valor de un atributo (aunque sea solo lectura desde el razor), debe ser una **propiedad PascalCase**. Esto garantiza que Blazor puede rastrear los cambios y el StateHasChanged funcione correctamente.

---

## 1. Plantilla Completa — `MiPagina.razor.cs`

```csharp
namespace HM.Presupuestos.Web.Pages.MiSeccion
{
    public partial class MiPagina : ContextProtegido   // partial — se une con el .razor
    {
        #region Inyección de Dependencias

        [Inject] protected IMiServicio MiServicio { get; set; } = default!;
        [Inject] protected DialogoErrores ErrorService { get; set; } = default!;

        #endregion

        #region Propiedades Privadas

        #region Página
        private string TextoToolTipAyuda { get; set; } = string.Empty;
        #endregion

        #region Filtro
        private List<CodigoDescripcion> Anios { get; set; } = [];          // bindeado → PascalCase
        private CodigoDescripcion? AnioSeleccionado { get; set; }          // bindeado → PascalCase
        #endregion

        #region Grid
        private DxGrid Grid { get; set; } = new DxGrid();                  // @ref → PascalCase
        private List<MiEntidad> Datos { get; set; } = [];                  // Data= → PascalCase
        private List<MiEntidad> _datosOriginal = [];                       // interno → _camelCase
        #endregion

        #region Popup Edición
        private bool EsPopupVisible { get; set; }                          // bindeado → PascalCase
        private string TituloPopup { get; set; } = string.Empty;
        private MiEntidad _entidadEnEdicion = new();                       // interno → _camelCase
        private ModoEdicion _modoEdicion = ModoEdicion.Alta;               // interno → _camelCase
        #endregion

        #endregion

        #region Propiedades Computadas

        private bool HayCambios =>
            JsonSerializer.Serialize(_datos) != JsonSerializer.Serialize(_datosOriginal);

        #endregion

        #region Ciclo de Vida

        protected override Task OnPermisoDenegadoAsync()
        {
            return Task.CompletedTask;
        }

        protected override async Task InicializarPaginaAsync()
        {
            _anios = await MiServicio.ObtenerAnios();
            _datos = await MiServicio.ObtenerDatos();
            _datosOriginal = DatosHelper.ClonarObjeto(_datos);

            if (_datos.Count == 0)
                await MensajesHelper.MostrarMensajeInfo(TituloPagina,
                    ObtenerTexto(TextosApp.Mensajes.RegistrosNoEncontrados));
        }

        protected override async Task OnUsuarioImpersonadoDesconectado()
        {
            await base.OnUsuarioImpersonadoDesconectado();
            await InvokeAsync(StateHasChanged);
        }

        #endregion

        #region Filtro — Eventos

        private async Task OnAnioChangedAsync(SelectedDataItemChangedEventArgs<CodigoDescripcion> e)
        {
            if (e.DataItem == null) return;

            await EjecutarAsync(async () =>
            {
                _datos = await MiServicio.ObtenerDatosPorAnio(e.DataItem.Codigo);
                _datosOriginal = DatosHelper.ClonarObjeto(_datos);
            });
        }

        #endregion

        #region Grid — CRUD

        private async Task NuevoAsync()
        {
            await EjecutarAsync(async () =>
            {
                _entidadEnEdicion = new MiEntidad();
                _modoEdicion = ModoEdicion.Alta;
                TituloPopup = ObtenerTexto(TextosApp.Pages.MiPagina.NuevoRegistro);
                EsPopupVisible = true;
            });
        }

        private async Task EditarAsync(MiEntidad entidad)
        {
            await EjecutarAsync(async () =>
            {
                _entidadEnEdicion = DatosHelper.ClonarObjeto(entidad);
                _modoEdicion = ModoEdicion.Edicion;
                TituloPopup = ObtenerTexto(TextosApp.Pages.MiPagina.EditarRegistro);
                EsPopupVisible = true;
            });
        }

        private async Task EliminarAsync(object dataItem)
        {
            if (dataItem is not MiEntidad entidad) return;

            bool confirmado = await MensajesHelper.MostrarMensajeParaConfirmacion(
                TituloPagina,
                ObtenerTexto(TextosApp.Mensajes.ConfirmacionEliminar));

            if (!confirmado) return;

            await EjecutarAsync(async () =>
            {
                await MiServicio.Eliminar(entidad.Codigo);
                _datos.Remove(entidad);
                Grid.Reload();
                await MensajesHelper.MostrarMensajeInfo(TituloPagina,
                    ObtenerTexto(TextosApp.Mensajes.RegistroEliminado));
            });
        }

        private async Task GuardarAsync()
        {
            await EjecutarAsync(async () =>
            {
                if (_modoEdicion == ModoEdicion.Alta)
                    await MiServicio.Insertar(_entidadEnEdicion);
                else
                    await MiServicio.Actualizar(_entidadEnEdicion);

                _datos = await MiServicio.ObtenerDatos();
                _datosOriginal = DatosHelper.ClonarObjeto(_datos);
                Grid.Reload();
                EsPopupVisible = false;

                await MensajesHelper.MostrarMensajeInfo(TituloPagina,
                    ObtenerTexto(TextosApp.Mensajes.RegistroGrabado));
            });
        }

        #endregion

        #region Event Handlers DevExpress

        // Aplica a TODOS los controles (Grid, ComboBox, RadioGroup, ListBox, etc.)
        // Convención: On + Componente + Descripción (PascalCase, sin guión bajo)
        // Los que devuelven void son la única excepción al sufijo Async
        private async void OnGridEditModelSaving(GridEditModelSavingEventArgs e) { ... }
        private async void OnGridElementCustomized(GridCustomizeElementEventArgs e) { ... }
        private async Task OnComboAniosSelectedDataItemChangedAsync(SelectedDataItemChangedEventArgs<CodigoDescripcion> e) { ... }
        private async Task OnRadioGroupAcuerdoValueChangedAsync(string newValue) { ... }

        #endregion
    }
}
```

---

## 2. Herencia: `Context` vs `ContextProtegido`

| Clase base | Cuándo usarla | Ejemplo real |
|---|---|---|
| `ContextProtegido` | **Toda página de negocio** con permisos por menú | `Indicadores`, `Versiones`, `Sobreprimas`, `AnioDiario` |
| `Context` | Componentes sin control de acceso (layout, admin sin permiso) | `PageHeader`, `NavMenu`, `Avisos` |

`ContextProtegido` añade sobre `Context`:
- `TienePermiso` / `ValidandoPermisos` → alimentan el guard `@if` del razor
- `InicializarPaginaAsync()` → hook llamado automáticamente cuando los permisos son válidos
- `OnPermisoDenegadoAsync()` → hook cuando el usuario no tiene acceso

`Context` proporciona siempre (no redeclarar en páginas hijas):

| Servicio | Uso |
|---|---|
| `MensajesHelper` | Diálogos de info, éxito, error, confirmación |
| `NavigationManager` | Navegación programática |
| `ObtenerTexto(clave)` | Localización de textos |
| `EjecutarAsync(...)` | Wrapper async con overlay y error handling |
| `TituloPagina` | Título calculado desde el menú |
| `Usuario` | `UsuarioEntidad` activo (impersonado o SSO) |
| `LocalizadorRecursos` | Acceso directo al servicio de recursos |
| `LayerOverlayService` | Control manual del overlay |

---

## 3. Ciclo de Vida Correcto

```
OnAfterRenderAsync(firstRender)
  └─ OnUsuarioDisponibleAsync()           ← usuario disponible (Context y ContextProtegido)
       └─ [solo ContextProtegido]
            ├─ OnPermisoValidadoAsync()   ← permisos OK; llama automáticamente a:
            │    └─ InicializarPaginaAsync()   ← AQUÍ va toda la carga inicial de datos
            └─ OnPermisoDenegadoAsync()   ← si no tiene permiso
```

**Regla fundamental**: Nunca cargar datos en `OnInitializedAsync()`. El usuario no está disponible todavía.

```csharp
// ✅ CORRECTO — carga en InicializarPaginaAsync
protected override async Task InicializarPaginaAsync()
{
    _datos = await MiServicio.ObtenerDatos();
}

// ❌ INCORRECTO — usuario todavía NULL aquí
protected override async Task OnInitializedAsync()
{
    _datos = await MiServicio.ObtenerDatos();
}
```

`InicializarPaginaAsync()` ya está envuelto por el framework con overlay y manejo de errores. No añadir `EjecutarAsync` dentro de él.

---

## 4. `EjecutarAsync` — Wrapper Obligatorio

Toda operación async iniciada desde la UI (clic, cambio de filtro, guardar, eliminar) va dentro de `EjecutarAsync`. Gestiona automáticamente: overlay, manejo de errores y re-render.

```csharp
// Operación estándar con overlay
await EjecutarAsync(async () =>
{
    _datos = await MiServicio.ObtenerDatosPorAnio(anio);
    Grid.Reload();
});

// Sin overlay — operaciones rápidas de UI como abrir un popup
await EjecutarAsync(() =>
{
    EsPopupVisible = true;
}, showOverlay: false);

// Con mensaje de error personalizado
await EjecutarAsync(async () =>
{
    await MiServicio.Eliminar(entidad.Codigo);
    _datos.Remove(entidad);
}, ObtenerTexto(TextosApp.Mensajes.ErrorGeneral));
```

**No usar** `EjecutarAsync` dentro de `InicializarPaginaAsync()` — ya está envuelto por el framework.

---

## 5. Inyección de Dependencias

```csharp
#region Inyección de Dependencias

// Solo los servicios específicos de esta página
[Inject] protected IMiServicio MiServicio { get; set; } = default!;
[Inject] protected IServicioAdicional ServicioAdicional { get; set; } = default!;

// NO redeclarar los ya disponibles por herencia de Context:
// MensajesHelper, NavigationManager, LayerOverlayService,
// LocalizadorRecursos, GestorIdioma, RegistroAplicacion, etc.

#endregion
```

Reglas:
- Siempre `protected` — para que el markup `.razor` los pueda usar
- Siempre `= default!` — suprimir la advertencia de nullable (el DI los resuelve)
- Siempre agrupados en `#region Inyección de Dependencias`

---

## 6. Organización con `#region`

El code-behind se organiza siempre con regiones. Orden canónico:

```csharp
#region Inyección de Dependencias
#region Propiedades Privadas
    #region Página
    #region Filtro
    #region Grid
    #region Popup Edición
#region Propiedades Computadas       // bool HayCambios, etc.
#region Ciclo de Vida                // InicializarPaginaAsync, OnPermisoDenegadoAsync
#region Filtro — Eventos             // SelectedDataItemChanged, etc.
#region Grid — CRUD                  // NuevoAsync, EditarAsync, EliminarAsync, GuardarAsync
#region Grid — Event Handlers        // handlers DevExpress con void
#region Helpers Privados             // métodos auxiliares sin estado UI
```

---

## 7. Localización de Textos

```csharp
// ✅ CORRECTO — clave tipada desde TextosApp
ObtenerTexto(TextosApp.Common.Descripcion)
ObtenerTexto(TextosApp.Botones.Grabar)
ObtenerTexto(TextosApp.Mensajes.ConfirmacionEliminar)
ObtenerTexto(TextosApp.Pages.MiPagina.ToolTip)

// ❌ INCORRECTO — string literal (funciona pero no es tipado ni seguro)
ObtenerTexto("Common:Descripcion:label")

// ❌ INCORRECTO — texto hardcodeado sin localizar
"Descripción"
```

Añadir una nueva clave → seguir el proceso de `.github/prompts/anadir-traduccion.prompt.md`.

---

## 8. Mensajes al Usuario — `MensajesHelper`

Nunca usar alertas JavaScript nativas. Siempre `MensajesHelper`:

```csharp
// Información — resultado vacío, estado neutro
await MensajesHelper.MostrarMensajeInfo(TituloPagina,
    ObtenerTexto(TextosApp.Mensajes.RegistrosNoEncontrados));

// Éxito — operación completada correctamente
await MensajesHelper.MostrarMensajeExito(TituloPagina,
    ObtenerTexto(TextosApp.Mensajes.RegistroGrabado));

// Advertencia — validación, estado inconsistente
await MensajesHelper.MostrarMensajeAviso(TituloPagina, mensajeValidacion);

// Error — técnico o inesperado
await MensajesHelper.MostrarMensajeError(TituloPagina,
    ObtenerTexto(TextosApp.Mensajes.ErrorGeneral));

// Confirmación — antes de acción destructiva (eliminar, sobrescribir)
bool confirmado = await MensajesHelper.MostrarMensajeParaConfirmacion(
    TituloPagina,
    ObtenerTexto(TextosApp.Mensajes.ConfirmacionEliminar));

if (!confirmado) return;
```

---

## 9. Referencia al Grid — `IGrid` vs `DxGrid`

```csharp
// Grid editable en celda (EditMode="EditCell") → IGrid nullable
private IGrid? GridVersiones { get; set; }

// Grid con operaciones directas frecuentes (Reload, ClearFilter, GetDataItem) → DxGrid concreto
private DxGrid GridIndicadores { get; set; } = new DxGrid();
```

Operaciones más frecuentes:

```csharp
Grid.Reload();                                            // refrescar datos
Grid.ClearFilter();                                       // limpiar filtro de búsqueda
var entidad = (MiEntidad?)Grid.GetDataItem(rowIndex);     // obtener item por índice
await Grid.StartEditNewRowAsync();                        // iniciar edición de nueva fila
Grid.CancelEditAsync();                                   // cancelar edición en curso
```

---

## 10. Estado Popup — Patrón Estándar

```csharp
#region Popup Edición

private bool EsPopupVisible { get; set; }
private string TituloPopup { get; set; } = string.Empty;
private MiEntidad _entidadEnEdicion = new();
private ModoEdicion _modoEdicion = ModoEdicion.Alta;

#endregion

// Abrir para alta
private async Task NuevoAsync()
{
    await EjecutarAsync(async () =>
    {
        _entidadEnEdicion = new MiEntidad();
        _modoEdicion = ModoEdicion.Alta;
        TituloPopup = ObtenerTexto(TextosApp.Pages.MiPagina.NuevoRegistro);
        EsPopupVisible = true;
    });
}

// Abrir para edición — clonar para no mutar el original
private async Task EditarAsync(MiEntidad entidad)
{
    await EjecutarAsync(async () =>
    {
        _entidadEnEdicion = DatosHelper.ClonarObjeto(entidad);
        _modoEdicion = ModoEdicion.Edicion;
        TituloPopup = ObtenerTexto(TextosApp.Pages.MiPagina.EditarRegistro);
        EsPopupVisible = true;
    });
}
```

---

## 11. ViewModels — Cuándo y Dónde

Los ViewModels viven **únicamente** en `HM.Presupuestos.Web`. Se crean cuando la UI necesita propiedades de estado que no pertenecen al dominio ni a Application:

```csharp
// CondicionViewModel.cs — en Web/Pages/Condiciones/ (junto al razor)
public class CondicionViewModel : CondicionDto
{
    public bool MedioAccesible { get; set; }   // solo tiene sentido en la UI
}

// Uso en la página
private List<CondicionViewModel> _condiciones = [];
private Dictionary<CondicionViewModel, DatosCambiados> _noGuardados { get; } = [];
```

Los servicios de Application **nunca** reciben ViewModels. Se mapean a DTOs o entidades antes de la llamada al servicio.

---

## 12. Detección de Cambios

Patrón para habilitar/deshabilitar botones de guardar o cancelar:

```csharp
// Para listas — comparación serializada (simple, suficiente para datos planos)
private bool HayCambios =>
    JsonSerializer.Serialize(_datos) != JsonSerializer.Serialize(_datosOriginal);

// Para diccionarios de edición — contar elementos modificados
private bool HayCambiosPendientes =>
    _registrosNoGuardados.Count > 0;

// Para un objeto único
private bool HayCambiosPendientes =>
    AnioSeleccionado is not null &&
    AnioSeleccionado.Codigo != AnioOriginal?.Codigo;
```

---

## 13. Checklist — Fichero `.razor.cs`

- [ ] `public partial class NombrePagina : ContextProtegido` (o `Context`)
- [ ] Inyecciones solo con servicios específicos de la página (`[Inject] protected ... = default!`)
- [ ] Propiedades agrupadas con `#region` por responsabilidad
- [ ] `InicializarPaginaAsync()` sobrescrito para carga de datos (no `OnInitializedAsync`)
- [ ] `OnPermisoDenegadoAsync()` sobrescrito (aunque devuelva `Task.CompletedTask`)
- [ ] `OnUsuarioImpersonadoDesconectado()` sobrescrito si hay estado que refrescar
- [ ] Toda operación async de UI envuelta en `EjecutarAsync`
- [ ] Confirmación antes de cualquier acción destructiva (`MostrarMensajeParaConfirmacion`)
- [ ] Todos los textos via `ObtenerTexto(TextosApp.*)`
- [ ] Claves de traducción nuevas añadidas a los 3 JSON + `TextosApp.cs`
- [ ] Los handlers de eventos DevExpress usan el patrón `On` + Componente + Descripción (PascalCase, sin guión bajo) para todos los controles — p.ej. `OnComboAniosSelectedDataItemChangedAsync`, `OnGridVersionesElementCustomized`. Los que retornan `void` (p.ej. `CustomizeElement`) son la única excepción al sufijo `Async`
- [ ] No redeclarar servicios ya heredados de `Context`
