## Why

Las páginas que usan datos maestros (Networks, Medios, Agrupaciones Comerciales, Editoriales) lanzan las mismas queries Oracle en cada carga de página, aunque estos datos apenas cambian durante una sesión de usuario. El refactor de carga en dos fases de Sobreprimas (`perf(sobreprimas)`) demostró que las queries de maestros son el cuello de botella real. Añadir una caché de sesión eliminaría ese coste en navegaciones sucesivas.

## What Changes

- Nuevo servicio `IMaestrosCacheService` / `MaestrosCacheService` en la capa Application que envuelve `IMaestrosService` y cachea sus resultados en memoria con un TTL configurable.
- La caché tiene alcance de sesión de usuario (scoped), no global, para evitar cruce de datos entre usuarios en Blazor Server.
- Las páginas que actualmente llaman directamente a `IMaestrosService` para datos maestros (Sobreprimas, ImportacionCondiciones, y otras) pueden opcionalmente resolver los datos a través del nuevo servicio en caché.
- Invalidación explícita disponible para los casos en que un administrador actualice los maestros.

## Capabilities

### New Capabilities

- `maestros-cache`: Caché de sesión de datos maestros (Networks, Medios, Agrupaciones Comerciales, Editoriales) con TTL configurable e invalidación explícita.

### Modified Capabilities

<!-- No hay cambios en requisitos de specs existentes -->

## Impact

- **Application**: nuevo puerto primario `IMaestrosCacheService` con su implementación.
- **Web**: registro del servicio en `Program.cs` como `AddScoped`; las páginas afectadas sustituyen `IMaestrosService` por `IMaestrosCacheService` en las llamadas a datos maestros.
- **Infrastructure**: sin cambios (los repositorios no se tocan).
- **Domain**: sin cambios.
- Sin nuevas claves de traducción (TextosApp).
- Sin nuevas acciones de auditoría (AccionesLog).
- Sin nuevos permisos (CodigosMenu).

## No incluido / Fuera de alcance

- Caché de versiones o condiciones (datos que cambian con frecuencia durante el uso).
- Caché distribuida (Redis, SQL Server): se usa `IMemoryCache` en proceso, suficiente para Blazor Server.
- Prefetch automático al iniciar sesión.
- UI de gestión de caché.
