## ADDED Requirements

### Requirement: AlmacenSesionUsuario es un adaptador puro sin política de errores
`AlmacenSesionUsuario` SHALL actuar como adaptador de traducción entre el puerto `IAlmacenSesionUsuario`
y `ProtectedSessionStorage`. No SHALL capturar ni suprimir excepciones. Toda excepción propagada por
`ProtectedSessionStorage` MUST ser propagada al caller sin modificación.

#### Scenario: Error al leer sesión propaga la excepción
- **WHEN** `ProtectedSessionStorage.GetAsync` lanza una excepción
- **THEN** `ObtenerUsuarioSSO` propaga esa excepción al caller sin capturarla

#### Scenario: Error al guardar sesión propaga la excepción
- **WHEN** `ProtectedSessionStorage.SetAsync` lanza una excepción
- **THEN** `GuardarUsuarioSSO` propaga esa excepción al caller sin capturarla

---

### Requirement: ObtenerUsuarioSSO devuelve null cuando no hay sesión
`ObtenerUsuarioSSO` SHALL devolver `null` cuando no existe la clave en sesión o cuando
`ProtectedBrowserStorageResult.Success` es `false`.

#### Scenario: Sesión vacía devuelve null
- **WHEN** no existe el valor de la clave `USER_SSO` en la sesión del navegador
- **THEN** `ObtenerUsuarioSSO` retorna `null`

#### Scenario: Sesión con valor válido devuelve la entidad
- **WHEN** existe la clave `USER_SSO` con un JSON válido de `UsuarioEntidad`
- **THEN** `ObtenerUsuarioSSO` retorna el `UsuarioEntidad` deserializado correctamente

---

### Requirement: SesionUsuario inicializa el contexto solo cuando hay usuario en sesión
`SesionUsuario.InicializarUsuarioAsync` SHALL comprobar explícitamente si `ObtenerUsuarioSSO`
devuelve `null`. En ese caso MUST retornar sin inicializar el `ContextoUsuario`.

#### Scenario: Sesión SSO nula no inicializa el contexto
- **WHEN** `ObtenerUsuarioSSO` retorna `null`
- **THEN** `InicializarUsuarioAsync` retorna sin asignar usuario autenticado ni emitir el evento `UsuarioCargado`

#### Scenario: Sesión SSO presente inicializa el contexto correctamente
- **WHEN** `ObtenerUsuarioSSO` retorna un `UsuarioEntidad` válido
- **THEN** `InicializarUsuarioAsync` asigna el usuario autenticado y emite el evento `UsuarioCargado`

---

### Requirement: Simetría de helpers privados en AlmacenSesionUsuario
`AlmacenSesionUsuario` SHALL tener exactamente dos helpers privados:
- `GetItemAsync(string key)` → `Task<string?>`: lee de sesión, devuelve `null` si no existe
- `DeleteItemAsync(string key)` → `Task`: elimina la clave de sesión

`SetItemAsync` SHALL ser eliminado. Los métodos `Guardar*` MUST llamar directamente a
`_sessionStorage.SetAsync`.

#### Scenario: EliminarUsuarioImpersonado usa DeleteItemAsync
- **WHEN** se llama a `EliminarUsuarioImpersonado`
- **THEN** la eliminación se realiza a través del helper `DeleteItemAsync` (no directamente con `_sessionStorage.DeleteAsync`)
