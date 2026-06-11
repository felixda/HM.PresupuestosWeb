## Context

Las páginas de Sobreprimas, ImportacionCondiciones y otras cargan en cada inicialización los mismos catálogos Oracle (Networks, Medios, Agrupaciones Comerciales, Editoriales). Estos datos son cuasi-estáticos: no cambian durante el uso normal de una sesión. El refactor `perf(sobreprimas)` mejoró la carga con paralelismo y query fusionada, pero el coste de red/BD sigue siendo el mismo en cada visita.

En Blazor Server cada usuario tiene su propio circuito. Los servicios registrados como `AddScoped` tienen una instancia por circuito, lo que hace trivial mantener estado en memoria sin riesgo de cruce entre usuarios.

## Goals / Non-Goals

**Goals:**
- Eliminar las queries Oracle repetidas para datos maestros dentro de la misma sesión de usuario.
- Primer hit transparente: si el dato no está en caché, se obtiene del repositorio y se almacena.
- Invalidación explícita para que las páginas puedan forzar la recarga tras una operación administrativa.
- TTL configurable (default: 30 min) para que la caché no viva indefinidamente si la sesión es larga.

**Non-Goals:**
- Caché distribuida entre instancias de servidor.
- Caché de versiones, condiciones u otros datos que mutan durante el uso normal.
- Precarga al inicio de sesión.
- UI de gestión o monitorización de caché.

## Decisions

### D1 — Patrón Decorator sobre `IMaestrosService`, no nuevo repositorio

`MaestrosCacheService` implementará `IMaestrosService` y envolverá al `MaestrosService` existente. La caché es un detalle de implementación de la capa Application, no un cambio de contratos ni de repositorios.

```
[Página] → [IMaestrosService]
                 ↓
         [MaestrosCacheService]  ← nuevo (decorator)
                 ↓ (cache miss)
         [MaestrosService]       ← existente
                 ↓
         [IPresupuestosRepository / IVersionesRepository]
```

*Alternativa descartada: añadir caché dentro de `MaestrosService`.* Mezclaría la responsabilidad de acceso a datos con la de gestión de caché, y haría los tests más complejos.

### D2 — `IMemoryCache` (in-process) con scope por instancia de servicio

`IMemoryCache` es el mecanismo estándar de .NET. Al ser el servicio `AddScoped`, cada usuario dispone de su propio `MaestrosCacheService`, pero todos comparten la misma instancia de `IMemoryCache` inyectada (`AddMemoryCache` es singleton). Las claves de caché deben incluir el identificador de usuario para aislar datos entre usuarios.

Formato de clave: `"maestros:{usuario}:{recurso}"` — por ejemplo `"maestros:fsmith:networks"`.

*Alternativa descartada: `Dictionary<string, object>` como campo privado del servicio.* Más simple, pero no tiene TTL ni métricas. `IMemoryCache` ya está en el contenedor y ofrece expiración nativa.

### D3 — TTL absoluto de 30 minutos, configurable en `appsettings.json`

```json
"MaestrosCache": {
  "TtlMinutos": 30
}
```

Si no se configura, se usa el valor por defecto. No se usa TTL deslizante para evitar que entradas muy usadas nunca expiren.

### D4 — Método `Invalidar()` e `InvalidarTodo()` en la interfaz

La interfaz expondrá métodos de invalidación explícita para cubrir el caso de páginas administrativas que modifican catálogos:

```csharp
void Invalidar(string recurso);
void InvalidarTodo();
```

### D5 — Qué métodos se cachean

Solo los catálogos cuasi-estáticos:
| Método | Clave |
|---|---|
| `ObtenerNetworks()` | `networks` |
| `ObtenerMedios()` | `medios` |
| `ObtenerMediosPorNetWork(codigos)` | `medios-por-network:{codigos}` |
| `ObtenerAgrupacionesComerciales(medios)` | `agrupaciones:{medios}` |
| `ObtenerEditoriales(filtro)` | `editoriales:{hash-filtro}` |
| `ObtenerAgrupacionesYEditoriales(medios)` | `agrupaciones-editoriales:{medios}` |

Métodos que **no** se cachean: `ObtenerGruposClientes`, `ObtenerGruposClientesConNetwork`, `ObtenerGruposClientePorNetworks` (dependen del contexto de la solicitud actual).

## Risks / Trade-offs

- **[Riesgo] Datos desactualizados en sesiones largas** → Mitigación: TTL de 30 min + método `InvalidarTodo()` disponible para páginas admin. Documentar en el código.
- **[Riesgo] `IMemoryCache` compartido aumenta presión de memoria con muchos usuarios simultáneos** → Mitigación: los datos son listas pequeñas (decenas de registros). El impacto es mínimo. Se puede añadir `SizeLimit` al `IMemoryCache` si se detecta problema.
- **[Trade-off] Clave de caché basada en parámetros string** → Las claves para métodos con parámetros (p. ej. `ObtenerMediosPorNetWork("1,2,3")`) dependen del formato del string de entrada. El llamador debe garantizar el formato consistente.
