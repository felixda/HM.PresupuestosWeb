## Context

`AlmacenSesionUsuario` es el adaptador que encapsula `ProtectedSessionStorage` (cifrado por Blazor) para persistir y recuperar `UsuarioEntidad` entre navegaciones. Su único consumidor directo es `SesionUsuario`, que es quien sí tiene try/catch y es el lugar correcto para la política de errores.

**Estado actual — problemas identificados:**

| # | Síntoma | Riesgo |
|---|---------|--------|
| 1 | `ObtenerUsuarioSSO` devuelve `new UsuarioEntidad()` cuando la deserialización falla | Fallo silencioso en auth: el caller recibe un objeto válido sin datos de usuario |
| 2 | `ObtenerUsuarioSSO` → `Task<UsuarioEntidad>`, `ObtenerUsuarioImpersonado` → `Task<UsuarioEntidad?>` | Asimetría de contratos sin justificación |
| 3 | `SetItemAsync` es un wrapper de una línea que delega directamente a `_sessionStorage.SetAsync` | Cero valor añadido; `GuardarUsuario*` puede llamar `SetAsync` directamente |
| 4 | `EliminarUsuarioImpersonado` llama `_sessionStorage.DeleteAsync` directamente (bypassa helper) | Inconsistencia con `GetItemAsync`; viola simetría del patrón |
| 5 | `Obtener*` tienen try/catch, `Guardar*` y `Eliminar*` no | Tres políticas de error distintas dentro de la misma clase |

## Goals / Non-Goals

**Goals:**
- Convertir `AlmacenSesionUsuario` en adaptador puro: sin try/catch, sin valores de rescate
- Contrato honesto: `ObtenerUsuarioSSO` → `Task<UsuarioEntidad?>`
- Simetría de helpers privados: `GetItemAsync` + `DeleteItemAsync`; eliminar `SetItemAsync`
- Ajustar `SesionUsuario.InicializarUsuarioAsync` para gestionar el `null` correctamente

**Non-Goals:**
- Cambiar el mecanismo de almacenamiento
- Añadir caché en memoria
- Gestionar expiración de sesión
- Cubrir con tests unitarios `AlmacenSesionUsuario` (requiere contexto de navegador real)

## Decisions

### D1 — Adaptador puro sin try/catch

**Decisión**: Eliminar todos los try/catch de `AlmacenSesionUsuario`. El adaptador propaga excepciones al caller.

**Alternativa descartada**: Mantener try/catch con logging. Descartada porque `SesionUsuario` ya tiene su propio try/catch en los métodos de autenticación, lo que produciría doble captura sin valor añadido.

**Rationale**: Siguiendo el principio de responsabilidad única, la política de errores (loggear, reintentar, mostrar UI) pertenece al caller, no al adaptador. El adaptador solo traduce entre la abstracción del puerto y la tecnología subyacente.

### D2 — ObtenerUsuarioSSO devuelve `Task<UsuarioEntidad?>`

**Decisión**: Cambiar el tipo de retorno de `Task<UsuarioEntidad>` a `Task<UsuarioEntidad?>`.

**Rationale**: Si la sesión no existe o falla la deserialización, lo correcto es devolver `null`. Devolver `new UsuarioEntidad()` es mentir al caller: le dice que hay usuario cuando no lo hay.

**Impacto en SesionUsuario**: `InicializarUsuarioAsync` deberá comprobar si `usuarioSSO` es `null` antes de llamar `AsignarUsuarioAutenticado`. Si es `null`, el contexto de usuario no se inicializa (comportamiento idéntico al actual cuando la sesión está vacía, pero ahora explícito en lugar de silencioso).

> Nota: `AsignarUsuarioAutenticado(UsuarioEntidad)` recibe un parámetro no-nullable. El null-check debe estar en `SesionUsuario` antes de llamarlo.

### D3 — Eliminar SetItemAsync, añadir DeleteItemAsync

**Decisión**: Eliminar el helper `SetItemAsync` (sin valor) y añadir `DeleteItemAsync` como helper privado equivalente a `GetItemAsync`.

**Rationale**: 
- `SetItemAsync` solo redirige a `_sessionStorage.SetAsync` sin transformación alguna. Los métodos `Guardar*` pueden llamar directamente a `_sessionStorage.SetAsync`.
- `DeleteItemAsync` centraliza la llamada a `_sessionStorage.DeleteAsync` por simetría con `GetItemAsync` y permite añadir logging en un único punto si se necesitara en el futuro.

## Diseño resultante

```
IAlmacenSesionUsuario
  Task<UsuarioEntidad?> ObtenerUsuarioSSO()          ← antes Task<UsuarioEntidad>
  Task GuardarUsuarioSSO(UsuarioEntidad usuario)
  Task GuardarUsuarioImpersonado(UsuarioEntidad usuario)
  Task<UsuarioEntidad?> ObtenerUsuarioImpersonado()
  Task EliminarUsuarioImpersonado()

AlmacenSesionUsuario (sin try/catch)
  Guardar*()             → _sessionStorage.SetAsync(clave, json)   ← sin wrapper
  Obtener*()             → GetItemAsync(clave)                      ← devuelve T?
  EliminarUsuarioImpersonado() → DeleteItemAsync(clave)             ← nuevo helper

SesionUsuario.InicializarUsuarioAsync()
  var usuarioSSO = await _almacen.ObtenerUsuarioSSO();
  if (usuarioSSO is null) return;    ← null-check explícito (comportamiento equivalente)
  usuario.AsignarUsuarioAutenticado(usuarioSSO);
```

## Risks / Trade-offs

**[Riesgo] Propagación de excepción en `InicializarUsuarioAsync`**
→ Si `ProtectedSessionStorage` lanza (p.ej. sesión expirada, cookie corrupta), la excepción llega a `SesionUsuario`. Mitigación: `SesionUsuario` ya tiene try/catch en `AutenticarUsuarioSSOAsync` y similares; revisar que `InicializarUsuarioAsync` también quede protegido en su caller (la página base `Context`).

**[Trade-off] Sin logging en el adaptador**
→ Los errores de `ProtectedSessionStorage` ya no se loggearán en `AlmacenSesionUsuario`. Se asume que el caller registra. Aceptable porque el logging en el adaptador era redundante con el del caller.

## Open Questions

_(ninguna — diseño acordado en fase de exploración previa)_
