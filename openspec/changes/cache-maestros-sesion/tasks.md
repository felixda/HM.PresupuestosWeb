## 1. Contrato del servicio de caché (Application)

- [ ] 1.1 Crear `IMaestrosCacheService` en `Application/CasosDeUso/Compartido/` que extienda `IMaestrosService` añadiendo `Invalidar(string recurso)` e `InvalidarTodo()`
- [ ] 1.2 Crear `MaestrosCacheService` en `Application/CasosDeUso/Compartido/` implementando `IMaestrosCacheService` con patrón decorator sobre `IMaestrosService` e `IMemoryCache`
- [ ] 1.3 Leer TTL desde `IOptions<MaestrosCacheOptions>` (clase de opciones con `TtlMinutos = 30` por defecto)
- [ ] 1.4 Cachear los 6 métodos definidos en design.md D5 con claves `"maestros:{usuario}:{recurso}:{params}"` (resto de métodos delegan directamente al inner service)

## 2. Registro en contenedor de DI (Web)

- [ ] 2.1 Añadir `builder.Services.AddMemoryCache()` en `Program.cs` si no está ya registrado
- [ ] 2.2 Registrar `MaestrosCacheOptions` con `builder.Services.Configure<MaestrosCacheOptions>(...)` leyendo la sección `"MaestrosCache"` de `appsettings.json`
- [ ] 2.3 Registrar `IMaestrosCacheService` → `MaestrosCacheService` como `AddScoped`
- [ ] 2.4 Añadir la sección `"MaestrosCache": { "TtlMinutos": 30 }` en `appsettings.json`

## 3. Uso en páginas

- [ ] 3.1 En `Sobreprimas.razor.cs`: inyectar `IMaestrosCacheService` en lugar de `IMaestrosService` y actualizar las llamadas a maestros en `InicializarPaginaAsync` y `CargarMaestrosDependientesAsync`
- [ ] 3.2 En `ImportacionCondiciones.razor.cs`: inyectar `IMaestrosCacheService` en lugar de `IMaestrosService` y actualizar las llamadas a maestros en `InicializarPaginaAsync`

## 4. Tests unitarios

- [ ] 4.1 Crear `MaestrosCacheServiceTests` en `HM.Presupuestos.UnitTest/`: verificar que en el segundo llamado a un método cacheado no se invoca el inner service (mock de `IMaestrosService`)
- [ ] 4.2 Verificar que `Invalidar(recurso)` fuerza recarga desde el inner service en la siguiente llamada
- [ ] 4.3 Verificar que `InvalidarTodo()` limpia todas las entradas del usuario actual

## 5. Validación final

- [ ] 5.1 Compilar la solución y verificar 0 errores
- [ ] 5.2 Ejecutar los tests unitarios y verificar que todos pasan
- [ ] 5.3 Commit y push con mensaje `feat(cache): añadir caché de sesión para datos maestros`
