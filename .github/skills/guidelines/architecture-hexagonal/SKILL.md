---
name: architecture-hexagonal
description: Usar cuando se crean módulos, se organiza código por capas, se trabaja con arquitectura hexagonal, puertos y adaptadores, entidades de dominio, repositorios, casos de uso, DTOs, o cuando se decide dónde pertenece un fichero en Domain, Application, Infrastructure o Web.
---

# Arquitectura Hexagonal — HM.Presupuestos

El código se organiza en cuatro proyectos .csproj que representan las capas del hexágono. Dentro de cada capa, el código se agrupa por módulo de negocio (Condiciones, Versiones, Sobreprimas, Indicadores...). La estructura "grita" el dominio, no la técnica.

## Estructura de Proyectos

```
HM.Presupuestos.Domain/
├── Compartido/             # Constantes, enums, excepciones de dominio
├── Entidades/
│   ├── Condiciones/        # Entidades del módulo Condiciones
│   ├── Versiones/
│   ├── Sobreprimas/
│   ├── Indicadores/
│   ├── LogAcciones/
│   ├── Usuario/
│   └── Utilidades/
├── Extensiones/
└── Puertos/
    ├── Repositorios/       # IXxxRepository (contratos de persistencia)
    └── Servicios/          # (interfaces de servicios externos si procede)

HM.Presupuestos.Application/
└── CasosDeUso/
    ├── Condiciones/        # ICondicionesService + CondicionesService
    ├── Versiones/
    ├── Sobreprimas/
    ├── Admin/
    ├── Configuracion/
    ├── LogAcciones/
    └── Compartido/         # Servicios compartidos (MaestrosService...)

HM.Presupuestos.Infrastructure/
└── Persistencia/
    ├── Condiciones/        # CondicionesRepository (implementa ICondicionesRepository)
    ├── Versiones/
    ├── Sobreprimas/
    ├── Admin/
    ├── Configuracion/
    ├── LogAcciones/
    ├── BasePresupuestosRepository.cs
    └── PresupuestosRepository.cs

HM.Presupuestos.Web/
├── Pages/
│   ├── Condiciones/        # Páginas Blazor del módulo
│   ├── Versiones/
│   ├── Mantenimientos/
│   ├── Admin/
│   └── ...
├── Componentes/            # Componentes Blazor reutilizables
├── Layout/
└── Adaptadores/            # Adaptadores de servicios para la UI
```

## Capas y Responsabilidades

### 1. Domain (núcleo del hexágono)

- **Entidades**: clases con identidad y ciclo de vida (`Condicion`, `Vigencia`, `Indicador`...)
- **Enums y Constantes**: en `Compartido/Enumerados.cs` y `Compartido/Constantes.cs`
- **Excepciones de dominio**: `ValidacionException`, `ExcepcionBaseDatos`
- **IXxxRepository**: contratos de persistencia en `Puertos/Repositorios/`
- Sin dependencias externas: no referencia ningún otro proyecto de la solución

### 2. Application (casos de uso)

- **IXxxService**: interfaz del caso de uso (vive junto a la implementación en `CasosDeUso/`)
- **XxxService**: implementación, orquesta la lógica de negocio llamando al repositorio propio
- Inyecta solo `IXxxRepository` propio y otros `IXxxService` (nunca repositorios ajenos)
- Registra auditoría (`ILogAccionesService`) después de operaciones de modificación
- Depende solo de Domain

### 3. Infrastructure (adaptadores secundarios)

- **XxxRepository**: implementa `IXxxRepository` de Domain
## Ports and Adapters

Ver `references/domain/repositories.md` for details.

- Hereda de `BasePresupuestosRepository` para acceso a la API HM.CORE
- Depende de Domain. No conoce Application ni Web

### 4. Web (adaptadores primarios — Blazor Server)

- **Pages**: componentes Blazor que heredan de `ContextProtegido` o `Context`
- Inyecta `IXxxService` (nunca repositorios)
- Nunca contiene lógica de negocio ni auditoría
- Usa `EjecutarAsync(...)` para todas las operaciones async

## Regla de Dependencia

```
Web → Application → Domain ← Infrastructure
```

- Las dependencias apuntan siempre hacia el centro (Domain)
- Domain no importa de ningún otro proyecto
- Application no importa de Infrastructure ni Web
- Web no importa de Infrastructure

## Puertos y Adaptadores

**Puertos secundarios — IXxxRepository** (en `Domain/Puertos/Repositorios/`):
Definen el contrato de persistencia. El dominio los necesita; Infrastructure los implementa.

**Puertos primarios — IXxxService** (en `Application/CasosDeUso/[Modulo]/`):
Definen el caso de uso. Web los consume; Application los implementa.
Se colocan en Application (no en Domain) siguiendo el patrón Clean Architecture.

### ¿Domain o Application? Criterio para ubicar una interfaz de servicio

La capa donde vive una interfaz depende de **quién la necesita** para funcionar:

| Criterio | Capa | Ejemplo |
|----------|------|---------|
| La lógica de negocio del dominio la necesita directamente | **Domain** | `IRegistroErroresCore` — el dominio necesita registrar errores sin saber nada de HTTP |
| Es un caso de uso que orquesta servicios externos o infraestructura | **Application** | `IMenuFavoritosService` — orquesta llamadas a la API externa para gestionar preferencias de UI |

```
Domain
  └── Puertos/IRegistroErroresCore     ← el dominio NECESITA esto para funcionar

Application
  └── CasosDeUso/IMenuFavoritosService ← la app ORQUESTA esto como caso de uso
```

En ambos casos, **Infrastructure implementa la interfaz** y **Web solo ve Application y Domain**, nunca Infrastructure directamente.

**Adaptadores secundarios — XxxRepository** (en `Infrastructure/Persistencia/[Modulo]/`):
Implementaciones concretas de `IXxxRepository`. Llaman a la API HM.CORE.

**Adaptadores primarios — Páginas Blazor** (en `Web/Pages/[Modulo]/`):
Consumen `IXxxService` vía DI. Punto de entrada del usuario al sistema.

## Comunicación entre Módulos

### Un servicio puede:
- Inyectar su propio `IXxxRepository`
- Inyectar `IXxxService` de otro módulo para obtener datos de ese dominio
- Inyectar `ILogAccionesService` para auditoría

### Un servicio no puede:
- Inyectar `IXxxRepository` de otro módulo
- Llamar directamente a Infrastructure
- Contener lógica de presentación

## Convenciones de Nomenclatura

| Capa | Sufijos permitidos |
|------|-------------------|
| Domain — entidades | Sin sufijo (`Condicion`, `Vigencia`, `Indicador`) |
| Domain — puertos | `IXxxRepository` |
| Application — puertos primarios | `IXxxService` |
| Application — implementaciones | `XxxService` |
| Infrastructure | `XxxRepository` |
| Web — páginas | `NombrePagina.razor` + `NombrePagina.razor.cs` |
| Web — componentes | `NombreComponente.razor` + `NombreComponente.razor.cs` |

- Un fichero por clase/interfaz
- Nombre de fichero = nombre de clase (PascalCase)
- Subcarpeta por módulo de negocio en todas las capas

## Registro de Dependencias (DI)

El único lugar que conoce todas las capas es `Web/Program.cs`. Es el único sitio donde se registran las implementaciones:

```csharp
// ✅ CORRECTO — en Program.cs
builder.Services.AddScoped<ICondicionesRepository, CondicionesRepository>();
builder.Services.AddScoped<ICondicionesService, CondicionesService>();
```

## Reglas No Negociables

- ❌ Nunca referenciar `Infrastructure` desde `Web`
- ❌ Nunca referenciar `Infrastructure` ni `Web` desde `Application`
- ❌ Nunca referenciar ningún proyecto desde `Domain`
- ❌ Nunca inyectar `IXxxRepository` de otro módulo en un servicio
- ❌ Nunca registrar auditoría desde una página Web
- ❌ Nunca poner lógica de negocio en Infrastructure
- ❌ Nunca usar `@inject` en ficheros `.razor` (usar `[Inject]` en `.razor.cs`)
- ✅ Siempre crear la interfaz antes que la implementación
- ✅ Siempre colocar `IXxxRepository` en `Domain/Puertos/Repositorios/`
- ✅ Siempre colocar `IXxxService` junto a `XxxService` en `Application/CasosDeUso/[Modulo]/`
- ✅ Siempre registrar las dependencias en `Web/Program.cs`
- ✅ Siempre agrupar por módulo de negocio dentro de cada capa

## Patrones Detallados por Capa

- **Domain**: [`references/domain/entities.md`](references/domain/entities.md), [`references/domain/repositories.md`](references/domain/repositories.md)
- **Infrastructure**: [`references/infrastructure/repositories.md`](references/infrastructure/repositories.md)
- **Application**: [`references/application/usecases.md`](references/application/usecases.md), [`references/application/dtos.md`](references/application/dtos.md)
- **Estructura completa**: [`module-structure.md`](refereces/module-structure.md)
