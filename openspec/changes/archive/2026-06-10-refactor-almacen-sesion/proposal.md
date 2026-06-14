## Why

`AlmacenSesionUsuario` mezcla tres políticas de error distintas (silenciar, propagar, ignorar), devuelve un `UsuarioEntidad` vacío en lugar de `null` cuando la sesión SSO falla —ocultando errores de auth— y contiene un wrapper `SetItemAsync` sin valor añadido. El resultado es un adaptador imprevisible donde el caller (SesionUsuario) no puede confiar en los contratos de retorno.

## What Changes

- Eliminar `SetItemAsync` (wrapper vacío de una línea, cero valor añadido)
- Añadir `DeleteItemAsync` helper privado (simetría con `GetItemAsync`)
- Eliminar `try/catch` de `ObtenerUsuarioSSO` y `ObtenerUsuarioImpersonado`: el adaptador propaga, el caller decide
- Cambiar `ObtenerUsuarioSSO` de `Task<UsuarioEntidad>` a `Task<UsuarioEntidad?>` (contrato honesto)
- Actualizar `SesionUsuario` para manejar el `null` de `ObtenerUsuarioSSO` igual que ya hace con `ObtenerUsuarioImpersonado`

## Capabilities

### New Capabilities

_(ninguna — es un refactor interno sin nueva funcionalidad)_

### Modified Capabilities

_(sin cambios en requisitos de negocio; el comportamiento observable para el usuario no cambia)_

## Impact

- **Web/Adaptadores/Sesion/AlmacenSesionUsuario.cs** — clase e interfaz modificadas
- **Web/Adaptadores/Sesion/SesionUsuario.cs** — ajuste del null-check en `InicializarUsuarioAsync`
- No requiere nuevas claves de traducción
- No requiere nuevas acciones de auditoría
- No requiere nuevos permisos
- Capas afectadas: únicamente **Web** (adaptadores de sesión)

## No incluido / Fuera de alcance

- Cambio de mecanismo de almacenamiento (sigue siendo `ProtectedSessionStorage`)
- Gestión de expiración de sesión
- Cifrado adicional más allá del que provee Blazor
- Tests unitarios de `AlmacenSesionUsuario` (la clase depende de `ProtectedSessionStorage` que requiere contexto de navegador)
