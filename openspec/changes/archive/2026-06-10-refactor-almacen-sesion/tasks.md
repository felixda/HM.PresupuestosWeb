## 1. Interfaz IAlmacenSesionUsuario

- [x] 1.1 Cambiar el tipo de retorno de `ObtenerUsuarioSSO` de `Task<UsuarioEntidad>` a `Task<UsuarioEntidad?>` en la interfaz `IAlmacenSesionUsuario`

## 2. Implementación AlmacenSesionUsuario

- [x] 2.1 Eliminar el método privado `SetItemAsync` de `AlmacenSesionUsuario`
- [x] 2.2 Añadir el método privado `DeleteItemAsync(string key)` en `AlmacenSesionUsuario`
- [x] 2.3 Actualizar `GuardarUsuarioSSO` para llamar directamente a `_sessionStorage.SetAsync` (sin `SetItemAsync`)
- [x] 2.4 Actualizar `GuardarUsuarioImpersonado` para llamar directamente a `_sessionStorage.SetAsync` (sin `SetItemAsync`)
- [x] 2.5 Actualizar `EliminarUsuarioImpersonado` para usar el nuevo helper `DeleteItemAsync`
- [x] 2.6 Eliminar el `try/catch` de `ObtenerUsuarioSSO` y cambiar el tipo de retorno a `Task<UsuarioEntidad?>`
- [x] 2.7 Eliminar el `try/catch` de `ObtenerUsuarioImpersonado`

## 3. Caller SesionUsuario

- [x] 3.1 En `InicializarUsuarioAsync`, añadir null-check sobre el resultado de `ObtenerUsuarioSSO`: si es `null`, retornar sin inicializar el contexto
- [x] 3.2 Verificar que `AsignarUsuarioAutenticado` solo se llama cuando `usuarioSSO` no es `null`

## 4. Validación

- [x] 4.1 Compilar la solución sin errores (`dotnet build`)
- [x] 4.2 Ejecutar todos los tests unitarios y verificar que siguen en verde (`dotnet test`)
