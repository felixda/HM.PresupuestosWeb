## 1. Corregir bug en Context.cs

- [x] 1.1 Eliminar la segunda llamada duplicada a `RegistroAplicacion.RegistrarExcepcion(ex)` en `EjecutarAsync<TResult>` en `Context.cs`
- [x] 1.2 Unificar el catch de `EjecutarAsync<TResult>` con `ManejarExcepcion` para que use el sistema de logging con respaldo igual que el overload `EjecutarAsync(Func<Task>)`

## 2. Migrar try/catch manuales en Versiones

- [x] 2.1 Sustituir los bloques try/catch con `LayerOverlayService.Start/Stop` y `RegistrarExcepcion` directos en `Versiones.razor.cs` por llamadas a `EjecutarAsync`
- [x] 2.2 Verificar que el comportamiento de overlay y mensajes de error es equivalente al anterior

## 3. Migrar ImportacionCondiciones al patrón ContextProtegido

- [x] 3.1 Eliminar el `OnAfterRenderAsync` propio de `ImportacionCondiciones.razor.cs` y reemplazarlo por `InicializarPaginaAsync()`
- [x] 3.2 Eliminar la propiedad `PageTitle` (inglés) y sustituir todas sus referencias por `TituloPagina` (heredado de `Context`)
- [x] 3.3 Sustituir los bloques try/catch manuales restantes en `ImportacionCondiciones.razor.cs` por `EjecutarAsync`

## 4. Limpiar código comentado obsoleto

- [x] 4.1 Revisar y eliminar bloques de código comentado (`//`) con lógica obsoleta en `Sobreprimas.razor.cs`
- [x] 4.2 Revisar y eliminar bloques de código comentado (`//`) con lógica obsoleta en `ImportacionCondiciones.razor.cs`
- [x] 4.3 Compilar y ejecutar los 16 tests unitarios; verificar 0 errores
