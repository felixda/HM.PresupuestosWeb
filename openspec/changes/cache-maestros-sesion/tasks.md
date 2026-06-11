## 1. Contrato del servicio de caché (Application)

- [x] 1.1 Crear `IMaestrosCacheService` en `Application/CasosDeUso/Compartido/` que extienda `IMaestrosService` añadiendo `Invalidar(string recurso)` e `InvalidarTodo()`
- [x] 1.2 Crear `MaestrosCacheService` en `Application/CasosDeUso/Compartido/` implementando `IMaestrosCacheService` con patrón decorator sobre `IMaestrosService` e `IMemoryCache`
- [x] 1.3 Leer TTL desde `IOptions<MaestrosCacheOptions>` (clase de opciones con `TtlMinutos = 30` por defecto)
- [x] 1.4 Cachear los 6 métodos definidos en design.md D5 con claves `"maestros:{usuario}:{recurso}:{params}"` (resto de métodos delegan directamente al inner service)

## 2. Registro en contenedor de DI (Web)

- [x] 2.1 Añadir `builder.Services.AddMemoryCache()` en `Program.cs` si no está ya registrado
- [x] 2.2 Registrar `MaestrosCacheOptions` con `builder.Services.Configure<MaestrosCacheOptions>(...)` leyendo la sección `"MaestrosCache"` de `appsettings.json`
- [x] 2.3 Registrar `IMaestrosCacheService` → `MaestrosCacheService` como `AddScoped`
- [x] 2.4 Añadir la sección `"MaestrosCache": { "TtlMinutos": 30 }` en `appsettings.json`

## 3. Uso en páginas

- [x] 3.1 En `Sobreprimas.razor.cs`: inyectar `IMaestrosCacheService` en lugar de `IMaestrosService` y actualizar las llamadas a maestros en `InicializarPaginaAsync` y `CargarMaestrosDependientesAsync`
- [x] 3.2 En `ImportacionCondiciones.razor.cs`: inyectar `IMaestrosCacheService` en lugar de `IMaestrosService` y actualizar las llamadas a maestros en `InicializarPaginaAsync`

## 4. Tests unitarios

- [x] 4.1 Crear `MaestrosCacheServiceTests` en `HM.Presupuestos.UnitTest/`: verificar que en el segundo llamado a un método cacheado no se invoca el inner service (mock de `IMaestrosService`)
- [x] 4.2 Verificar que `Invalidar(recurso)` fuerza recarga desde el inner service en la siguiente llamada
- [x] 4.3 Verificar que `InvalidarTodo()` limpia todas las entradas del usuario actual

## 5. Validación final

- [x] 5.1 Compilar la solución y verificar 0 errores
- [x] 5.2 Ejecutar los tests unitarios y verificar que todos pasan
- [x] 5.3 Commit y push con mensaje `feat(cache): añadir caché de sesión para datos maestros`
