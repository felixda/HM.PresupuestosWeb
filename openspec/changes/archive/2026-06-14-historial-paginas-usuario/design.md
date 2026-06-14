## Context

La página `UsuariosConectados` ya registra la página actual de cada usuario mediante `IRegistroSesionesActivas`. Sin embargo, ese adaptador solo guarda el estado puntual (una única página por usuario). Para mostrar el historial de navegación del día es necesario un nuevo adaptador en memoria que acumule las visitas sin modificar el diseño existente.

El punto de captura natural ya existe: `MainLayout.OnLocationChangedAsync` invoca `ActualizarPagina` con la URL normalizada y ya filtra URLs duplicadas consecutivas (`_ultimaRutaVisitada`). Solo hay que añadir una llamada adicional al nuevo adaptador en ese mismo punto.

## Goals / Non-Goals

**Goals:**
- Nuevo singleton `IHistorialNavegacion` con implementación `HistorialNavegacion` en `Web/Adaptadores/Sesion/`.
- Límite máximo de entradas por usuario configurable desde `appsettings.json` (`HistorialNavegacion:MaxEntradas`, default 50).
- Historial acumulativo entre reconexiones del mismo login.
- Sin duplicados consecutivos (heredado del filtro de `MainLayout`).
- Interacción en `UsuariosConectados.razor`: seleccionar una fila del grid de usuarios muestra debajo un segundo `DxGrid` con el historial del usuario seleccionado.

**Non-Goals:**
- Persistencia en base de datos.
- Historial de días anteriores.
- Limpieza automática al final del día (fuera de alcance).

## Decisions

### D1 — Nuevo adaptador independiente (no extender SesionActivaInfo)

`SesionActivaInfo` es un `record` inmutable optimizado para la consulta de estado actual. Añadirle una lista mutable requeriría reconstruirlo con `with` en cada visita, lo que es ineficiente y complica la concurrencia.

Se crea un adaptador dedicado con `ConcurrentDictionary<string, Queue<EntradaHistorial>>`. Al superar `MaxEntradas`, se hace `Dequeue` de la entrada más antigua.

**Alternativa descartada**: añadir `List<EntradaHistorial>` a `SesionActivaInfo` — descartada por la inmutabilidad del record y la complejidad de sincronización.

### D2 — `record EntradaHistorial(string Pagina, DateTime Hora)` en el mismo archivo del adaptador

Sigue el patrón establecido en `RegistroSesionesActivas.cs` donde `SesionActivaInfo` se define junto a su interfaz e implementación.

### D3 — Configuración vía `IConfiguration` inyectada en el constructor

El valor `MaxEntradas` se lee de `IConfiguration["HistorialNavegacion:MaxEntradas"]` con fallback a 50. Permite cambiar el límite sin recompilar.

### D4 — Interacción con DxGrid mediante `SelectedDataItemChanged`

El grid de usuarios expone `SelectedDataItem`. Al cambiar la selección se llama a `OnUsuarioSeleccionadoAsync` que carga el historial del usuario seleccionado en `_historialUsuario`. El segundo grid se renderiza condicionalmente (`@if (_usuarioSeleccionado != null)`).

## Risks / Trade-offs

- **Memoria creciente** → Mitigación: límite de `MaxEntradas` por usuario; el diccionario se purga al cerrar el circuito (integración con `CircuitSesionTracker` podría añadirse en el futuro, pero no es requerido aquí).
- **Historial stale en el segundo grid** → Mitigación: el botón "Actualizar" existente recargará también el historial del usuario seleccionado.
- **Concurrencia en Queue** → `Queue<T>` no es thread-safe; se usa `lock` por entrada de diccionario en `RegistrarVisita`. Alternativa: `ConcurrentQueue<T>` pero no soporta `Dequeue` condicional limpiamente.
