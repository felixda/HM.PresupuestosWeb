# Estilos en Blazor Server — CSS Isolation y Guías

## Sistema de Estilos

El proyecto usa **tres capas de estilos** que se complementan:

| Capa | Ficheros | Alcance |
|---|---|---|
| **Global** | `wwwroot/css/comun.css`, `site.css` | Toda la aplicación |
| **Temas** | `switcher-resources/css/themes/*/` | Tema activo seleccionado por usuario |
| **Aislado** | `ComponentName.razor.css` | Solo el componente que lo declara |

El bundling de ficheros `.razor.css` se produce automáticamente en `HM.Presupuestos.Web.styles.css`.

---

## CSS Isolation — `.razor.css`

Cada componente o página que necesita estilos propios tiene un fichero `.razor.css` junto al `.razor`. Blazor aplica un atributo de scope automático en compilación, por lo que los estilos **no afectan a otros componentes**:

```
Layout/
  MainLayout.razor
  MainLayout.razor.css       ← estilos SOLO para MainLayout
  NavMenu.razor
  NavMenu.razor.css

Pages/Shared/
  PageHeader.razor
  PageHeader.razor.css

Pages/GestionSobreprimas/
  Sobreprimas.razor
  Sobreprimas.razor.css
```

### Cuándo crear un `.razor.css`

- El componente tiene layout o espaciado específico que no existe en `comun.css`
- Los estilos no deben afectar a otros componentes

### Cuándo usar clases de `comun.css`

- Estilos de grilla (`.grid-modified-cell`, `.dxgrid-readonly-cell`, `.grid-new_item`)
- Estilos de formulario (`.formLayoutGroupCaption`)
- Utilidades corporativas reutilizables

---

## Variables CSS — Paleta Corporativa

Todas las variables están definidas en `comun.css` con el prefijo `--havas-`:

```css
:root {
    /* Colores corporativos */
    --havas-color-red: #e60000;
    --havas-color-red-rgb: 230, 0, 0;
    --havas-color-black: #000000;
    --havas-color-blue-3: #008596;
    --havas-color-green-1: #00dcb9;
    --havas-color-yellow: #ffdc32;

    /* DevExpress overrides */
    --background-color_devexpress-ui-control: rgba(var(--havas-color-black-rgb), 80%);
    --dxbl-list-box-item-selected-bg: var(--havas-color-red-3);

    /* Bootstrap overrides */
    --bs-primary: rgba(var(--havas-color-red-rgb), 70%);
    --bs-danger: rgba(var(--havas-color-red-rgb), 95%);
}
```

**Reglas:**
- Nunca hardcodear colores corporativos (`#e60000`) directamente en componentes; usar `var(--havas-color-red)`
- Para transparencias, usar `rgba(var(--havas-color-red-rgb), 70%)` (el `*-rgb` companion)
- Las variables de Bootstrap (`--bs-primary`, `--bs-danger`) y DevExpress ya están redefinidas; no sobreescribir por componente

---

## Clases de DevExpress — Convenciones

Las clases personalizadas para controlar el comportamiento visual del DxGrid están en `comun.css` y se aplican desde el code-behind:

```csharp
// Aplicar clase en CustomizeElement del grid
private void GridCondiciones_CustomizeElement(GridCustomizeElementEventArgs e)
{
    if (e.ElementType == GridElementType.DataRow)
    {
        var condicion = (CondicionViewModel)e.Grid.GetDataItem(e.VisibleIndex);
        if (!condicion.MedioAccesible)
            e.CssClass = "alt-item";           // fila deshabilitada (opacidad 50%)
    }

    if (e.ElementType == GridElementType.DataCell)
    {
        if (esCeldaReadonly)
            e.CssClass = "dxgrid-readonly-cell";   // fondo amarillo 25%
        if (esCeldaModificada)
            e.CssClass = "grid-modified-cell";     // fondo rojo claro
    }
}
```

| Clase | Efecto visual |
|---|---|
| `grid-modified-cell` | Celda con cambio pendiente — fondo rojo claro |
| `grid-new_item` | Fila nueva — fondo verde claro |
| `grid-updated_item` | Fila actualizada |
| `alt-item` | Fila deshabilitada — opacidad 50% |
| `dxgrid-readonly-cell` | Celda de solo lectura — fondo amarillo |

---

## Bootstrap v5 — Uso en Markup

Bootstrap se usa para layout y utilidades, no para sobrescribir el sistema de temas:

```razor
@* Contenedor flex *@
<div class="d-flex align-items-center gap-2">
    <DxButton ... />
    <DxButton ... />
</div>

@* Modal con overlay (fondo semi-transparente) *@
<div class="modal d-block text-center" style="background-color: rgba(0,0,0,0.5);">
    <!-- contenido -->
</div>

@* Espaciado *@
<div class="mt-2 mb-3">...</div>
```

**Usar Bootstrap para**: layout (`d-flex`, `d-grid`, `gap-*`), espaciado (`mt-*`, `mb-*`, `p-*`), visibilidad (`d-none`, `d-block`).

**No usar Bootstrap para**: colores de componentes DevExpress (los gestiona el sistema de temas), tipografía de encabezados de página (los gestiona `comun.css`).

---

## Sistema de Temas

El tema activo se persiste en una cookie (`ActiveTheme`) y se carga dinámicamente en `App.razor`. El usuario puede cambiar el tema desde `ThemeSwitcher`:

```razor
@* App.razor — carga dinámica según tema activo *@
@if (!string.IsNullOrEmpty(bsTheme))
{
    <link rel="stylesheet" href="@AppendVersion(bsTheme)" bs-theme-link />
}
@if (!string.IsNullOrEmpty(dxTheme))
{
    <link rel="stylesheet" href="@AppendVersion(dxTheme)" dx-theme-link />
}
```

**Temas disponibles:**
- DevExpress BuiltIn: `blazing-berry`, `blazing-dark`, `office-white`, `purple`
- Bootstrap/Bootswatch: `default`, `default-dark`, `cerulean`, `cyborg`, `flatly`, `journal`, `lumen`, `lux`, `pulse`, `simplex`, `solar`, `superhero`, `united`, `yeti`

Los estilos propios del proyecto **deben funcionar en todos los temas**. Usar variables CSS en lugar de colores fijos para asegurar compatibilidad.

---

## Estilos Inline — Cuándo Son Aceptables

Evitar estilos inline excepto cuando el valor es **dinámico y calculado en tiempo de ejecución**:

```razor
@* ✅ ACEPTABLE — valor dinámico que no puede ser clase *@
<div class="modal d-block" style="background-color: rgba(0,0,0,0.5);">

@* ✅ ACEPTABLE — propiedad específica de DevExpress *@
<DxGrid ColumnResizeMode="GridColumnResizeMode.NextColumn" />

@* ❌ EVITAR — estilo estático que debería ser clase CSS *@
<span style="font-weight: 600; padding-left: 1rem;">Texto</span>
@* Mejor: *@
<span class="formLayoutGroupCaption">Texto</span>
```

---

## Iconos

Se usan dos librerías de iconos:

- **Font Awesome 6** (CDN): `fas fa-*`, `far fa-*`, `fab fa-*`
- **Open Iconic** (`wwwroot/css/open.iconic/`): `oi oi-*`

```razor
<i class="fas fa-edit"></i>
<span class="oi oi-trash"></span>
```

Los iconos dinámicos del dominio (según `BitAnd` de indicadores) se calculan en la entidad como `IconoCssClass` y se usan directamente:

```razor
<i class="@indicador.IconoCssClass"></i>
```

---

## Prohibiciones

- **No CSS inline para estilos estáticos**; usar clase en `.razor.css` o en `comun.css`
- **No hardcodear colores corporativos**; usar `var(--havas-color-*)`
- **No crear nuevas variables CSS fuera de `comun.css`** a menos que sean estrictamente locales al componente
- **No `!important`** para sobreescribir estilos de DevExpress; usar selectores más específicos o variables CSS
- **No redefinir `--bs-primary` ni `--bs-danger` por componente**; están configurados globalmente
- **No añadir Bootstrap al margen del sistema de temas**; el tema ya incluye Bootstrap

---

## Estructura de `wwwroot`

```
wwwroot/
├── css/
│   ├── bootstrap/          ← Bootstrap 5 (tema base)
│   ├── fonts/              ← Tipografías corporativas (Baikal, etc.)
│   ├── highlight/          ← Syntax highlighting
│   ├── open.iconic/        ← Open Iconic icons
│   ├── site.css            ← Reset y base mínima
│   └── comun.css           ← Variables y clases globales del proyecto
├── images/
│   ├── favicon/
│   ├── menu/               ← Iconos SVG de navegación
│   └── logo_havas_blanco.png
├── js/
│   ├── app.js
│   ├── inactividad.js
│   └── controlCambios.js
├── drawer-resources/css/   ← Estilos del drawer de navegación
└── switcher-resources/
    ├── css/
    │   ├── theme-switcher.css
    │   ├── themes.css
    │   └── themes/         ← Un directorio por tema (default, cerulean, cyborg…)
    └── js/
        └── theme-controller.js
```
