# Casos de Uso (Application Services)

## Estructura

Los casos de uso viven en `HM.Presupuestos.Application/CasosDeUso/`, organizados por módulo de negocio. Cada módulo tiene una interfaz (`IXxxService`) y su implementación (`XxxService`).

```
HM.Presupuestos.Application/
└── CasosDeUso/
    ├── Admin/
    │   ├── IAdminService.cs
    │   └── AdminService.cs
    ├── Compartido/
    │   ├── IMaestrosService.cs
    │   └── MaestrosService.cs
    ├── Condiciones/
    │   ├── ICondicionesService.cs
    │   └── CondicionesService.cs
    ├── Configuracion/
    │   ├── IConfiguracionService.cs
    │   └── ConfiguracionService.cs
    ├── LogAcciones/
    │   ├── ILogAccionesService.cs
    │   └── LogAccionesService.cs
    ├── Sobreprimas/
    │   ├── ISobreprimasService.cs
    │   └── SobreprimasService.cs
    └── Versiones/
        ├── IVersionesService.cs
        ├── VersionesService.cs
        ├── IIndicadoresService.cs
        └── IndicadoresService.cs
```

Ejemplo de un servicio completo:

```csharp
// ICondicionesService.cs — contrato público
public interface ICondicionesService
{
    Task<List<Vigencia>> ObtenerVigencias(CondicionFiltro filtro);
    Task<bool> ValidarSolapesVigencia(Vigencia vigencia);
    Task<List<CondicionDto>> ObtenerCondicionesPorVigencia(int codigoVigencia, int codigoNetwork);
    Task GrabarCondicionesExcepciones(
        Dictionary<CondicionDto, DatosCondicionCambiados> condicionesNoGuardadas,
        Dictionary<ExcepcionDto, DatosExcepcionesCondicionCambiados> excepcionesNoGuardadas,
        int codigoVigencia);
}

// CondicionesService.cs — implementación con primary constructor
public class CondicionesService(
    ILogger<CondicionesService> logger,
    ICondicionesRepository condicionesRepository,
    IMaestrosService maestrosService,
    ILogAccionesService logAccionesService) : ICondicionesService
{
    public async Task<List<CondicionDto>> ObtenerCondicionesPorVigencia(int codigoVigencia, int codigoNetwork)
    {
        List<CondicionDto> condiciones = await condicionesRepository.ObtenerCondiciones(codigoVigencia);

        if (condiciones.Count == 0)
        {
            // Si no hay condiciones en BD, se construyen desde los medios del network
            List<CodigoDescripcion> medios = await maestrosService.ObtenerMediosPorNetWork(codigoNetwork.ToString());
            condiciones = [.. medios.Select(m => new CondicionDto
            {
                CodigoMedio = m.Codigo,
                DescripcionMedio = m.Descripcion
            })];
        }

        return condiciones;
    }
}
```

---

## Sintaxis del Constructor (Primary Constructor)

Todos los servicios usan **primary constructor syntax** de C# 12. Las dependencias se declaran como parámetros del constructor directamente en la firma de la clase:

```csharp
// ✅ CORRECTO — primary constructor
public class SobreprimasService(
    ILogger<SobreprimasService> logger,
    ISobreprimasRepository sobreprimasRepository,
    ILogAccionesService logAccionesService) : ISobreprimasService
{
    // Los parámetros están disponibles como fields implícitos en todo el cuerpo de la clase
}

// ❌ EVITAR — constructor explícito (más verboso, no es el patrón del proyecto)
public class SobreprimasService : ISobreprimasService
{
    private readonly ISobreprimasRepository _sobreprimasRepository;

    public SobreprimasService(ISobreprimasRepository sobreprimasRepository)
    {
        _sobreprimasRepository = sobreprimasRepository;
    }
}
```

---

## Parámetros: Primitivos vs Filtros

Usar **parámetros primitivos** cuando son pocos (hasta 3). Usar una clase **Filtro** cuando son muchos o forman una unidad conceptual.

```csharp
// ✅ CORRECTO — primitivos para pocos parámetros
Task<List<int>> ObtenerMesesBloqueados(int anio);
Task<bool> ValidarSolapesVigencia(Vigencia vigencia);

// ✅ CORRECTO — objeto Filtro cuando son varios parámetros relacionados
Task<List<VersionResumen>> ObtenerVersionesResumen(VersionFiltro filtro);
// donde VersionFiltro encapsula: anio?, estadoIncluido?, estadoExcluido?

Task<List<SobreprimaGridModel>> ObtenerSobreprimas(SobreprimaFiltro filtro);
// donde SobreprimaFiltro encapsula: codigoVersion, codigoNetwork, codigoPais...

// ❌ EVITAR — muchos parámetros primitivos sueltos
Task<List<VersionResumen>> ObtenerVersionesResumen(int? anio, EstadoVersion? estadoIncluido, EstadoVersion? estadoExcluido);
```

---

## Responsabilidad Única

Cada servicio tiene responsabilidad sobre **un módulo de negocio**. Cuando un módulo tiene entidades claramente diferenciadas, se crean servicios separados:

```csharp
// ✅ CORRECTO — dos servicios para dos entidades distintas del módulo Versiones
public interface IVersionesService
{
    Task<List<VersionResumen>> ObtenerVersionesResumen(VersionFiltro filtro);
    Task GrabarVersiones(List<Version> nuevas, List<Version> modificadas, int codigoPais);
    Task EliminarVersion(Version version);
}

public interface IIndicadoresService
{
    Task<List<IndicadorConIdiomas>> ObtenerIndicadoresConIdiomas(string? descripcion = null);
    Task Grabar(Indicador indicador, List<IndicadorIdioma> nuevos, List<IndicadorIdioma> actualizar, List<IndicadorIdioma> eliminar);
    Task Eliminar(Indicador indicador);
}

// ❌ EVITAR — un solo servicio que mezcla versiones e indicadores
public interface IVersionesIndicadoresService
{
    Task GrabarVersiones(...);
    Task GrabarIndicador(...);
    // → difícil de mantener, viola SRP
}
```

---

## Dependencias

Los servicios de Application dependen de:
- **Puertos de repositorio** (`IXxxRepository`) — solo para persistencia
- **Otros servicios de Application** — para orquestación, nunca repositorios de otro módulo directamente
- `ILogger<TService>` — siempre presente
- Servicios externos de infraestructura (`IJwt`, `IRegistroErroresCore`) — solo cuando sea imprescindible

```csharp
// CondicionesService orquesta maestros y auditoría sin acceder a sus repositorios
public class CondicionesService(
    ILogger<CondicionesService> logger,
    ICondicionesRepository condicionesRepository,   // Puerto propio del módulo
    IMaestrosService maestrosService,               // Servicio de Application (no IPresupuestosRepository)
    ILogAccionesService logAccionesService          // Servicio de Application (no ILogAccionesRepository)
) : ICondicionesService { ... }

// IndicadoresService depende de VersionesService para actualizar BitAnd en cascada
public class IndicadoresService(
    ILogger<IndicadoresService> logger,
    IIndicadoresRepository indicadoresRepository,
    IVersionesService versionesService,             // Composición de servicios
    ILogAccionesService logAccionesService
) : IIndicadoresService { ... }
```

**Regla:** un servicio llama a otro servicio; **nunca** inyecta el repositorio de otro módulo.

---

## Transacciones

Las operaciones de escritura complejas (múltiples entidades) se envuelven en una transacción explícita:

```csharp
public async Task GrabarCondicionesExcepciones(
    Dictionary<CondicionDto, DatosCondicionCambiados> condicionesNoGuardadas,
    Dictionary<ExcepcionDto, DatosExcepcionesCondicionCambiados> excepcionesNoGuardadas,
    int codigoVigencia)
{
    using var transaction = condicionesRepository.ObtenerTransaccion();
    try
    {
        // Graba los 3 conceptos de cada condición modificada
        foreach (var (condicion, datos) in condicionesNoGuardadas)
        {
            await condicionesRepository.GrabarCondicion(condicion, datos, codigoVigencia, transaction);
        }

        // Graba los 7 conceptos NMD de cada excepción modificada
        foreach (var (excepcion, datos) in excepcionesNoGuardadas)
        {
            await condicionesRepository.GrabarExcepcion(excepcion, datos, codigoVigencia, transaction);
        }

        await transaction.CommitAsync();
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }

    // Auditoría FUERA de la transacción (post-commit)
    try
    {
        await logAccionesService.Insertar(AccionesLog.GrabarCondiciones, new { codigoVigencia });
    }
    catch (Exception logEx)
    {
        logger.LogWarning(logEx, "Error registrando log de acción. La operación principal fue exitosa.");
        // No se propaga — la operación principal fue exitosa
    }
}
```

---

## Auditoría (LogAcciones)

La auditoría se registra **siempre después del commit**. Los errores de log nunca deben hacer fallar la operación principal:

```csharp
// ✅ CORRECTO — auditoría post-commit, error silenciado
await transaction.CommitAsync();
try
{
    await logAccionesService.Insertar(AccionesLog.EliminarSobreprima, new { sobreprima.Codigo });
}
catch (Exception logEx)
{
    logger.LogWarning(logEx, "Error al registrar log. La eliminación fue exitosa.");
}

// ❌ INCORRECTO — auditoría dentro de la transacción
using var transaction = repository.ObtenerTransaccion();
await repository.Eliminar(entidad, transaction);
await logAccionesService.Insertar(...);   // si falla, hace rollback de la operación principal
await transaction.CommitAsync();
```

El servicio `ILogAccionesService` admite tres formas de llamada:

```csharp
// Con enum de acción (recomendado — tipado)
await logAccionesService.Insertar(AccionesLog.GrabarVersion, new { codigoPais, anio });

// Con string libre (cuando no hay enum definido)
await logAccionesService.Insertar("Acción personalizada", new { parametro });

// Con objeto LogAccion completo (casos avanzados)
await logAccionesService.Insertar(new LogAccion { ... });
```

---

## Lógica de Negocio en el Servicio

El servicio es el lugar correcto para la lógica de negocio que cruza entidades o requiere datos externos. Los repositorios solo persisten/consultan.

```csharp
// ✅ CORRECTO — lógica condicional (INSERT/UPDATE/DELETE/IGNORAR) en el servicio
public async Task GrabarSobreprimas(List<Sobreprima> sobreprimas)
{
    foreach (var sobreprima in sobreprimas)
    {
        if (sobreprima.Codigo == 0 && sobreprima.Porcentaje > 0)
            await sobreprimasRepository.Insertar(sobreprima);
        else if (sobreprima.Codigo > 0 && sobreprima.Porcentaje == 0)
            await sobreprimasRepository.Eliminar(sobreprima);
        else if (sobreprima.Codigo > 0 && sobreprima.Porcentaje > 0)
            await sobreprimasRepository.Actualizar(sobreprima);
        // Codigo == 0 && Porcentaje == 0 → se ignora, sin operación
    }
}

// ✅ CORRECTO — validación de dominio en el servicio
public async Task<bool> ValidarSolapesVigencia(Vigencia vigencia)
{
    List<Vigencia> vigenciasExistentes = await condicionesRepository.ObtenerVigencias(vigencia.CodigoNetwork);

    return !vigenciasExistentes
        .Where(v => v.Codigo != vigencia.Codigo)                 // Excluye la propia vigencia
        .Any(v => v.MesDesde <= vigencia.MesHasta               // Rango [MesDesde..MesHasta]
                  && v.MesHasta >= vigencia.MesDesde);
}
```

---

## Registro en el Contenedor de DI

Todos los servicios se registran en `Program.cs` con `AddScoped`:

```csharp
// HM.Presupuestos.Web/Program.cs
builder.Services.AddScoped<IMaestrosService, MaestrosService>();
builder.Services.AddScoped<ISobreprimasService, SobreprimasService>();
builder.Services.AddScoped<ICondicionesService, CondicionesService>();
builder.Services.AddScoped<IVersionesService, VersionesService>();
builder.Services.AddScoped<IIndicadoresService, IndicadoresService>();
builder.Services.AddScoped<ILogAccionesService, LogAccionesService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IConfiguracionService, ConfiguracionService>();
```

Usar siempre `AddScoped` (ciclo de vida por request HTTP), nunca `AddSingleton` para servicios que accedan a datos.
