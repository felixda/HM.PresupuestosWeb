## Context

La página `/admin/auditorias` muestra un grid con tres columnas (Descripción, Fecha, Usuario). La tabla `PPT_ACCION_LOG` contiene el campo `PARAMETROS` (CLOB) con el JSON de la operación auditada, con tamaños que van de unos pocos bytes hasta ~17 KB. La query del repositorio ya selecciona `PARAMETROS` pero solo lo usa internamente para extraer el usuario — no lo expone en la entidad `Auditoria`.

Estado actual de los ficheros clave:
- `Auditoria.cs`: 3 propiedades (`Descripcion`, `FechaInicio`, `Usuario`). Sin `Parametros`.
- `LogAccionesRepository.cs`: query ya incluye `SELECT ... PARAMETROS ...` pero no asigna al modelo.
- `Auditorias.razor`: grid con 3 columnas `DxGridDataColumn`. Sin columna de parámetros.

## Goals / Non-Goals

**Goals:**
- Exponer `Parametros` en la entidad `Auditoria` (Domain)
- Asignar el valor en el repositorio al mapear la fila (Infrastructure)
- Mostrar una columna con icono en el grid que abra un `DxPopup` con el JSON formateado (Web)
- Añadir las claves de traducción necesarias

**Non-Goals:**
- Coloreado de sintaxis JSON
- Árbol interactivo / JSON viewer
- Filtrado por contenido de parámetros
- Exponer `RESULTADO_STR`

## Decisions

### 1. `DxPopup` en lugar de tooltip

**Decisión**: usar `DxPopup` de DevExpress para mostrar el contenido.

**Alternativas consideradas**:
- *Tooltip nativo*: descartado, el contenido llega a 17 KB de JSON — inutilizable sin scroll.
- *Panel lateral (DxFlyout)*: válido, pero no hay precedente en el proyecto. `DxPopup` ya se usa en Indicadores, Sobreprimas y Condiciones.

**Rationale**: consistencia con patrones existentes + permite scroll + tamaño configurable.

### 2. La propiedad `Parametros` vive en `Auditoria` (Domain)

El campo ya se lee desde BD. Solo falta exponerlo. No requiere nuevo puerto ni nuevo método de repositorio — es un cambio en el mapeo existente.

### 3. Columna con `DxGridCommandColumn` / template personalizado

Se añade una columna con `CellDisplayTemplate` que renderiza un botón icono (`fa-solid fa-file-code`). Al pulsar, asigna la fila seleccionada a `_auditoriaSeleccionada` y activa `_popupParametrosVisible = true`.

### 4. Traducciones necesarias

| Clave | Uso |
|---|---|
| `TextosApp.Pages.Auditorias.Parametros` | Caption columna del grid |
| `TextosApp.Pages.Auditorias.TituloPopupParametros` | Título del `DxPopup` |

Seguir el proceso de `.github/prompts/anadir-traduccion.prompt.md`.

## Risks / Trade-offs

- **JSON muy largo (~17 KB para logins)**: el popup con `<pre>` y scroll es legible pero no es un viewer interactivo. Asumido como suficiente para una pantalla de administración. → Mitigación: el popup tiene `overflow: auto` con altura máxima.
- **Datos sensibles en pantalla** (el JSON de login incluye token, roles, etc.): la página ya está protegida por permisos de administrador (`CodigosMenu.Auditorias`). No hay riesgo adicional.
