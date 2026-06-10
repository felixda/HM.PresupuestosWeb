# Estructura de Módulos

## Árbol Completo de la Solución

```
HM.Presupuestos.sln
├── HM.Presupuestos.Domain/           # Núcleo: entidades, puertos, enumerados
│   ├── Compartido/
│   │   ├── Constantes.cs
│   │   ├── Enumerados.cs
│   │   └── ValidacionException.cs
│   ├── Entidades/
│   │   ├── CodigoDescripcion.cs      # Clase base para maestros
│   │   ├── Condiciones/
│   │   │   ├── Condicion.cs          # Entidad de persistencia
│   │   │   ├── Vigencia.cs           # Entidad (hereda CodigoDescripcion)
│   │   │   ├── CondicionDto.cs       # DTO de presentación
│   │   │   ├── ExcepcionDto.cs       # DTO de presentación
│   │   │   └── CondicionFiltro.cs    # Parámetros de consulta
│   │   ├── Indicadores/
│   │   │   ├── Indicador.cs
│   │   │   └── IdiomaIndicador.cs
│   │   ├── Sobreprimas/
│   │   │   ├── Sobreprima.cs
│   │   │   ├── SobreprimaGridModel.cs
│   │   │   └── SobreprimaFiltro.cs
│   │   ├── Versiones/
│   │   │   ├── Version.cs
│   │   │   ├── VersionResumen.cs
│   │   │   └── VersionLinea.cs
│   │   ├── LogAcciones/
│   │   │   └── LogAccion.cs
│   │   └── Utilidades/
│   │       └── IConIcono.cs          # Interfaces IConIcono, IConCodigo
│   ├── Extensiones/
│   │   └── EnumExtensions.cs
│   └── Puertos/
│       ├── Repositorios/
│       │   ├── IAdminRepository.cs
│       │   ├── ICondicionesRepository.cs
│       │   ├── IConfiguracionRepository.cs
│       │   ├── IIndicadoresRepository.cs
│       │   ├── ILogAccionesRepository.cs
│       │   ├── IPresupuestosRepository.cs
│       │   ├── ISobreprimasRepository.cs
│       │   └── IVersionesRepository.cs
│       └── Servicios/
│           ├── ITransaccion.cs
│           └── ICoreLoggerService.cs
│
├── HM.Presupuestos.Application/      # Casos de uso / servicios de orquestación
│   └── CasosDeUso/
│       ├── Admin/
│       │   ├── IAdminService.cs
│       │   └── AdminService.cs
│       ├── Compartido/
│       │   ├── IMaestrosService.cs
│       │   └── MaestrosService.cs
│       ├── Condiciones/
│       │   ├── ICondicionesService.cs
│       │   └── CondicionesService.cs
│       ├── Configuracion/
│       │   ├── IConfiguracionService.cs
│       │   └── ConfiguracionService.cs
│       ├── LogAcciones/
│       │   ├── ILogAccionesService.cs
│       │   └── LogAccionesService.cs
│       ├── Sobreprimas/
│       │   ├── ISobreprimasService.cs
│       │   └── SobreprimasService.cs
│       └── Versiones/
│           ├── IVersionesService.cs
│           ├── VersionesService.cs
│           ├── IIndicadoresService.cs
│           └── IndicadoresService.cs
│
├── HM.Presupuestos.Infrastructure/   # Adaptadores: BD, APIs externas
│   ├── Persistencia/
│   │   ├── BasePresupuestosRepository.cs   # Base con DAL + JWT
│   │   ├── PresupuestosRepository.cs       # Maestros
│   │   ├── Admin/
│   │   ├── Condiciones/
│   │   │   └── CondicionesRepository.cs
│   │   ├── Configuracion/
│   │   ├── LogAcciones/
│   │   ├── Sobreprimas/
│   │   └── Versiones/
│   └── Servicios/
│       ├── IClienteApiCore.cs
│       ├── ClienteApiCore.cs           # Cliente HTTP a HM.Core
│       ├── RegistroErroresCore.cs
│       └── TransaccionWrapper.cs       # Implementa ITransaccion
│
├── HM.Presupuestos.Web/              # Presentación: Blazor Server
│   ├── Program.cs                    # Registro de DI, middleware, auth
│   ├── App.razor
│   ├── Routes.razor
│   ├── _Imports.razor
│   ├── GlobalUsings.cs
│   ├── Adaptadores/                  # Adaptadores de infraestructura Web
│   │   ├── Sesion/
│   │   │   ├── ContextoUsuario.cs
│   │   │   ├── SesionUsuario.cs
│   │   │   └── AlmacenSesionUsuario.cs
│   │   ├── Auditoria/
│   │   ├── Idioma/
│   │   ├── Inactividad/
│   │   ├── Navegacion/
│   │   └── Ui/
│   ├── Componentes/
│   │   ├── Base/
│   │   │   ├── Context.cs            # Componente base (sin permisos)
│   │   │   ├── ContextProtegido.cs   # Componente base con permisos
│   │   │   └── TextosApp.cs       # Claves de recursos/traducciones
│   │   ├── Helpers/
│   │   └── Ui/
│   │       └── DrawerUtils.cs
│   ├── Layout/
│   ├── Pages/
│   │   ├── Index.razor(.cs/.css)
│   │   ├── Admin/
│   │   │   ├── Avisos.razor(.cs/.css)
│   │   │   ├── MesesBloqueados.razor(.cs/.css)
│   │   │   └── Impersonation.razor
│   │   ├── Condiciones/
│   │   │   ├── PlanificacionCondiciones.razor(.cs/.css)
│   │   │   ├── ImportacionCondiciones.razor(.cs/.css)
│   │   │   ├── CondicionViewModel.cs          # ViewModel: CondicionDto + estado UI
│   │   │   └── ExcepcionCondicionViewModel.cs # ViewModel: ExcepcionDto + estado UI
│   │   ├── GestionSobreprimas/
│   │   │   └── Sobreprimas.razor(.cs/.css)
│   │   ├── Mantenimientos/
│   │   │   ├── Versiones.razor(.cs/.css)
│   │   │   └── Indicadores.razor(.cs/.css)
│   │   └── Configuracion/
│   └── wwwroot/
│
├── HM.Presupuestos.UnitTest/         # Tests unitarios de servicios
│   └── Mantenimientos/
│       └── IndicadoresServiceTests.cs
│
└── HM.Presupuestos.E2ETest/          # Tests end-to-end con Playwright
    ├── Base/
    │   └── E2ETestBase.cs
    ├── Configuracion/
    │   └── E2ETestSettings.cs
    └── Tests/
        └── InicioTests.cs
```

---

## Convención de Nombrado

### Domain — Entidades y DTOs

```
Condicion.cs                  # Entidad de dominio (singular, PascalCase)
Vigencia.cs                   # Entidad (hereda CodigoDescripcion)
CondicionDto.cs               # DTO de presentación — sufijo Dto
ExcepcionDto.cs               # DTO de presentación — sufijo Dto
CondicionFiltro.cs            # Parámetros de consulta — sufijo Filtro
```

### Domain — Puertos

```
ICondicionesRepository.cs     # Interfaz de repositorio — prefijo I, sufijo Repository
IVersionesService.cs          # Puerto de servicio externo — prefijo I, sufijo Service
ITransaccion.cs               # Puerto de transacción — prefijo I
```

### Application — Servicios

```
ICondicionesService.cs        # Contrato del servicio — prefijo I
CondicionesService.cs         # Implementación del servicio
IMaestrosService.cs / MaestrosService.cs
IIndicadoresService.cs / IndicadoresService.cs
```

### Infrastructure — Repositorios

```
CondicionesRepository.cs      # Implementa ICondicionesRepository
PresupuestosRepository.cs     # Implementa IPresupuestosRepository (maestros)
BasePresupuestosRepository.cs # Clase base abstracta
TransaccionWrapper.cs         # Implementa ITransaccion
```

### Web — Páginas

```
PlanificacionCondiciones.razor        # Componente Blazor (markup)
PlanificacionCondiciones.razor.cs     # Code-behind (partial class)
PlanificacionCondiciones.razor.css    # CSS aislado (scoped)
CondicionViewModel.cs                 # ViewModel de la página — sufijo ViewModel
```

---

## Reglas de Comunicación Entre Capas

### Dependencias permitidas

```
Web          → Application (IXxxService)
Web          → Domain (entidades, DTOs, enumerados)
Application  → Domain (IXxxRepository, entidades, DTOs)
Infrastructure → Domain (implementa IXxxRepository)
```

### Dependencias prohibidas

```csharp
// ❌ Web no referencia Infrastructure directamente
// HM.Presupuestos.Web.csproj NO tiene referencia a HM.Presupuestos.Infrastructure

// ❌ Domain no referencia Application ni Infrastructure
// HM.Presupuestos.Domain.csproj no tiene referencias a otros proyectos del repo

// ❌ Application no referencia Infrastructure
// HM.Presupuestos.Application.csproj NO tiene referencia a HM.Presupuestos.Infrastructure

// ❌ Un servicio no inyecta repositorios de otro módulo
public class CondicionesService(
    ICondicionesRepository condicionesRepository,
    IPresupuestosRepository presupuestosRepository   // ❌ repositorio ajeno
) { }

// ✅ Un servicio llama al servicio del otro módulo
public class CondicionesService(
    ICondicionesRepository condicionesRepository,
    IMaestrosService maestrosService                 // ✅ servicio del módulo de maestros
) { }
```

---

## Páginas y su Code-Behind

Cada página Blazor se divide en tres ficheros (partial class):

```csharp
// PlanificacionCondiciones.razor — markup + componentes DevExpress
@page "/condiciones/planificacion"
@inherits ContextProtegido

<DxGrid Data="_condiciones" ...>
    <Columns>
        <DxGridDataColumn FieldName="@nameof(CondicionViewModel.DescripcionMedio)" />
    </Columns>
</DxGrid>

// PlanificacionCondiciones.razor.cs — lógica C#
public partial class PlanificacionCondiciones : ContextProtegido
{
    [Inject] private ICondicionesService CondicionesService { get; set; } = default!;

    private List<CondicionViewModel> _condiciones = [];

    protected override async Task InicializarPaginaAsync()
    {
        await EjecutarAsync(async () =>
        {
            var dtos = await CondicionesService.ObtenerCondicionesPorVigencia(...);
            _condiciones = dtos.Select(d => new CondicionViewModel { ... }).ToList();
        });
    }
}

// PlanificacionCondiciones.razor.css — CSS scoped a este componente
.grid-condiciones { ... }
```

---

## ViewModels: Cuándo y Dónde

Los ViewModels viven junto a la página que los usa, en la misma carpeta de `Pages/`:

```
Pages/
└── Condiciones/
    ├── PlanificacionCondiciones.razor
    ├── PlanificacionCondiciones.razor.cs
    ├── PlanificacionCondiciones.razor.css
    ├── CondicionViewModel.cs          ← ViewModel de esta página
    └── ExcepcionCondicionViewModel.cs ← ViewModel de esta página
```

Solo se crean cuando la página necesita estado de UI que no existe en el DTO (flags de accesibilidad, listas de opciones para combos, etc.). Ver [dtos.md](../references/application/dtos.md) para el patrón completo.

---

## Tests: Estructura y Convención

### Tests unitarios (`HM.Presupuestos.UnitTest`)

Testean servicios de Application aislados, con dependencias mockeadas:

```
HM.Presupuestos.UnitTest/
└── Mantenimientos/
    └── IndicadoresServiceTests.cs    # Tests de IndicadoresService
```

Nomenclatura de métodos: `Metodo_Escenario_ResultadoEsperado`

```csharp
[Test]
public async Task Grabar_DescripcionDuplicada_LanzaValidacionException() { ... }

[Test]
public async Task Eliminar_IndicadorConVersiones_ActualizaBitAndEnCascada() { ... }
```

### Tests E2E (`HM.Presupuestos.E2ETest`)

Tests de navegador con Playwright + NUnit. Heredan de `E2ETestBase`:

```csharp
// Tests/InicioTests.cs
public class InicioTests : E2ETestBase
{
    [Test]
    public async Task PaginaInicio_CargaCorrectamente()
    {
        await IrAUrl("/");
        await Expect(Page.GetByRole(AriaRole.Heading)).ToBeVisibleAsync();
    }
}
```

La sesión SSO se precarga desde `sesion_auth.json` (generado con `GuardarSesion.ps1`, excluido del repo).

---

## Registro de Dependencias (`Program.cs`)

Toda la configuración de DI está centralizada en `HM.Presupuestos.Web/Program.cs`:

```csharp
// Servicios de Application
builder.Services.AddScoped<IMaestrosService, MaestrosService>();
builder.Services.AddScoped<ICondicionesService, CondicionesService>();
builder.Services.AddScoped<ISobreprimasService, SobreprimasService>();
builder.Services.AddScoped<IVersionesService, VersionesService>();
builder.Services.AddScoped<IIndicadoresService, IndicadoresService>();
builder.Services.AddScoped<ILogAccionesService, LogAccionesService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IConfiguracionService, ConfiguracionService>();

// Repositorios de Infrastructure
builder.Services.AddScoped<ICondicionesRepository, CondicionesRepository>();
builder.Services.AddScoped<ISobreprimasRepository, SobreprimasRepository>();
builder.Services.AddScoped<IVersionesRepository, VersionesRepository>();
builder.Services.AddScoped<IIndicadoresRepository, IndicadoresRepository>();
builder.Services.AddScoped<IPresupuestosRepository, PresupuestosRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IConfiguracionRepository, ConfiguracionRepository>();
builder.Services.AddScoped<ILogAccionesRepository, LogAccionesRepository>();
```

Siempre `AddScoped` — un ciclo de vida por request HTTP, compatible con Blazor Server.
