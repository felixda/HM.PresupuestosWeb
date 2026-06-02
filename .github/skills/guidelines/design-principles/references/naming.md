# Estándares de Nomenclatura

## Reglas

- **Pronunciable y descriptivo en español**. Los nombres de clases, métodos y variables deben describir con precisión el concepto. Abreviaciones solo permitidas en lambdas de ámbito reducido (`x =>`, `r =>`)
- **Sin información de tipo en el nombre**. El IDE ya muestra el tipo; el nombre no necesita repetirlo
- **Sin sinónimos para el mismo concepto**. Un concepto, un nombre. Si algo es una `Vigencia`, siempre se llama `Vigencia`, nunca `Periodo` o `Rango`
- **Distinguir sustantivos de verbos**. Las clases son sustantivos (`Indicador`, `VersionesService`). Los métodos son verbos (`ObtenerVersiones`, `EliminarVigencia`)
- **Evitar nombres genéricos sin contexto** (`data`, `info`, `manager`). Usar nombres específicos del dominio
- **Evitar comentarios si un nombre autoexplicativo es posible**

## Convenciones por Tipo

| Tipo | Convenio | Ejemplo |
|---|---|---|
| Clase | PascalCase | `IndicadoresService`, `CondicionFiltro` |
| Interfaz | PascalCase con prefijo `I` | `IIndicadoresService`, `ICondicionesRepository` |
| Método público | PascalCase verbo | `ObtenerVersiones`, `EliminarVigencia` |
| Método privado | PascalCase verbo | `TratarCondicion`, `ActualizarIndicadorEnListaLocal` |
| Propiedad | PascalCase | `CodigoVigencia`, `Descripcion`, `Porcentaje` |
| Campo privado | `_camelCase` con prefijo `_` | `_listVersion`, `_versionesRepository`, `_logger` |
| Variable local | camelCase | `anioActual`, `itemVersion`, `resultado` |
| Parámetro | camelCase | `codigoVigencia`, `filtro`, `indicador` |
| Constante de dominio | ALL_CAPS | `NUMERO_ESTADOS`, `RULE_ADMIN`, `USER_SSO` |
| Enum (nombre) | PascalCase | `EstadoEntidad`, `CampoErrorValidacion` |
| Enum (valor) | PascalCase | `SinCambios`, `Modificado`, `Nuevo` |
| Método async | Sufijo `Async` | `InicializarPaginaAsync`, `TratarExcepcionAsync` |

## Sufijos de Arquitectura Permitidos

| Sufijo | Uso |
|---|---|
| `Service` | Caso de uso / lógica de aplicación: `VersionesService` |
| `Repository` | Acceso a datos: `CondicionesRepository` |
| `Dto` | Objeto de transferencia sin lógica UI: `CondicionDto`, `ExcepcionDto` |
| `ViewModel` | DTO extendido con propiedades de UI (capa Web): `CondicionViewModel` |
| `Filtro` / `Param` | Agrupación de parámetros de entrada: `CondicionFiltro`, `ParamImportacionSobreprimas` |
| `Tests` | Clase de tests unitarios: `IndicadoresServiceTests` |

## Ejemplos

### Clases y Campos Privados

```csharp
// ✅ CORRECTO — nombre descriptivo del dominio + campo privado con _
public class VersionesService(
    ILogger<VersionesService> logger,
    IVersionesRepository versionesRepository) : IVersionesService
{
    private readonly ILogger<VersionesService> _logger = logger;
    private readonly IVersionesRepository _versionesRepository = versionesRepository;
}

// ❌ INCORRECTO — nombre genérico, campo sin _
public class VersionManager(ILogger logger, IVersionesRepository repo)
{
    private readonly ILogger logger;         // sin _, igual que parámetro
    private readonly IVersionesRepository r; // abreviado sin contexto
}
```

### Interfaces

```csharp
// ✅ CORRECTO — prefijo I siempre en interfaces
public interface IVersionesService { ... }
public interface ICondicionesRepository { ... }
public interface ITransaccion { ... }

// ❌ INCORRECTO — sin prefijo I
public interface VersionesService { ... }   // confunde con la clase concreta
public interface VersionesServiceInterface { ... }   // sufijo redundante
```

### Variables Locales y Parámetros

```csharp
// ✅ CORRECTO — camelCase descriptivo
int anioActual = DateTime.Now.Year;
int anioAnterior = anioActual - 1;
var itemVersion = _listVersion.Find(x => x.Codigo == versionCodigo);
var itemIndicador = itemVersion.IndicadorList.Find(x => x.Codigo == indicadorCodigo);

// ✅ CORRECTO — lambdas con abreviaciones de ámbito reducido
var seSolapa = vigencias.Any(r => vigencia.MesDesde <= r.MesHasta);
return [.. resultado.OrderByDescending(x => x.Codigo)];

// ❌ INCORRECTO — nombres sin contexto
var d = _listVersion.Find(x => x.Codigo == versionCodigo);
var obj = itemVersion.IndicadorList.Find(x => x.Codigo == indicadorCodigo);
```

### Propiedades

```csharp
// ✅ CORRECTO — PascalCase, descriptivo, sin prefijo
public int Codigo { get; set; }
public string Descripcion { get; set; }
public int CodigoVigencia { get; set; }
public decimal? Porcentaje { get; set; }
public bool EsDatoAdjunto { get; set; }

// ✅ CORRECTO — propiedad computada descriptiva
private bool HayCambios =>
    JsonSerializer.Serialize(_listVersion) != JsonSerializer.Serialize(_listOriginVersion);

private bool HayCambiosPendientes =>
    _condicionesNoGuardados.Count > 0 || ExcepcionesNoGuardadas.Count > 0;
```

### Enumerados

```csharp
// ✅ CORRECTO — PascalCase en nombre y valores
public enum EstadoEntidad { SinCambios = 0, Modificado = 1, Nuevo = 2, Eliminado = 3 }
public enum CampoErrorValidacion { Descripcion, Orden, BitAnd }
public enum ModoOperacion { Ninguna = 0, Consultar = 1, Insertar = 2, Modificar = 3, Eliminar = 4 }
public enum ConceptosCondiciones { Sag = 1, Manpower = 2, Devolucion = 3 }
public enum OrigenVersion { Presupuestos = 1, Inges = 2, PoterA0 = 3 }

// ❌ INCORRECTO — UPPER_CASE en valores de enum, o nombres de tipo en el valor
public enum EstadoEntidad { SIN_CAMBIOS = 0, MODIFICADO = 1 }   // ALL_CAPS no aplica a valores enum
public enum EstadoEntidad { EstadoSinCambios, EstadoModificado } // prefijo redundante
```

### Constantes de Dominio

```csharp
// ✅ CORRECTO — ALL_CAPS, agrupadas en clase estática anidada en Constantes.cs (Domain)
public static class Constantes
{
    public const int NUMERO_ESTADOS = 25;

    public static class CodigosIndicadores
    {
        public const int CERRADA = 3;
        public const int REAL = 43;
    }

    public static class BitAndVersion
    {
        public const int PUBLICADA = 8;
        public const int CERRADA = 1;
        public const int REAL = 32768;
    }

    public static class Environment
    {
        public const string DEV = "DEV";
        public const string PRO = "PRO";
    }
}

// ❌ INCORRECTO — constante de dominio con camelCase
public const int numeroEstados = 25;    // no sigue convenio C# para constantes
public const int PublicadaBitAnd = 8;   // PascalCase es para propiedades, no constantes de dominio
```

### Sufijo `Async` en Métodos

```csharp
// ✅ CORRECTO — sufijo Async en todos los métodos async (Task o Task<T>)
protected override async Task InicializarPaginaAsync() { ... }
public async Task<List<Version>> ObtenerVersionesAsync(int anio) { ... }
private async Task TratarExcepcionAsync(Condicion excepcion, ...) { ... }
private async Task InsertErrorLogAsync(string methodName, ...) { ... }

// ✅ EXCEPCIÓN VÁLIDA — event handlers de DevExpress devuelven void, no Task
private async void CheckboxIndicadorCheckedChanged(bool? newValue, int? versionCodigo, int? indicadorCodigo)
private async void GridVersiones_CustomizeElement(GridCustomizeElementEventArgs ea)
private async void GridVersiones_EditModelSaving(GridEditModelSavingEventArgs e)
// Los event handlers de UI son la única excepción al sufijo Async

// ❌ INCORRECTO — método async Task sin sufijo Async
private async Task TratarExcepcion(Condicion excepcion, ...) { ... }  // falta Async
public async Task<List<Version>> ObtenerVersiones(int anio) { ... }   // falta Async
```

### Blazor: Pages y Code-Behind

```csharp
// ✅ CORRECTO — clase partial con el mismo nombre que el fichero .razor
// Fichero: Pages/Mantenimientos/Versiones.razor → Versiones.razor.cs
public partial class Versiones : ContextProtegido
{
    // Servicios inyectados: PascalCase, nombre = tipo sin prefijo I
    [Inject] protected IVersionesService VersionesService { get; set; } = default!;
    [Inject] protected IIndicadoresService IndicadoresService { get; set; } = default!;

    // Componentes DevExpress: PascalCase con tipo en nombre
    private IGrid? GridVersiones { get; set; }
    private IGrid? GridIdiomasIndicador { get; set; }

    // Datos de la página: _ + camelCase, tipado explícito
    private List<Version> _listVersion = [];
    private List<CodigoDescripcion> _listYear = [];
    private Version? _versionSeleccionada;
}
```

### Tests Unitarios

El patrón para nombres de métodos de test es `Método_Condición_ResultadoEsperado`:

```csharp
[TestFixture]
public class IndicadoresServiceTests  // clase: nombre de SUT + "Tests"
{
    private Mock<ILogger<IndicadoresService>> _loggerMock;
    private Mock<IIndicadoresRepository> _repositoryMock;
    private IndicadoresService _sut;   // "System Under Test" — nombre estándar

    [Test]
    public async Task ObtenerIndicadoresConIdiomas_SinFiltro_DevuelveListaCompleta() { ... }

    [Test]
    public async Task ObtenerIndicadoresConIdiomas_SinResultados_DevuelveListaVacia() { ... }

    [Test]
    public async Task Grabar_IndicadorNuevoSinDuplicados_InsertaYHaceCommit() { ... }

    [Test]
    public async Task Grabar_IndicadorModificado_ActualizaYHaceCommit() { ... }

    // ❌ INCORRECTO — nombre de test sin estructura clara
    [Test]
    public async Task TestObtenerIndicadores() { ... }    // no describe condición ni resultado
    [Test]
    public async Task Test1() { ... }                     // sin semántica
}
```

### Qué Evitar

```csharp
// ❌ Nombres sin contexto de dominio
var data = await VersionesService.ObtenerVersiones(anio);
var info = _listVersion.Find(x => x.Codigo == codigo);
var manager = new VersionesService(...);

// ✅ Nombres con contexto
var versiones = await VersionesService.ObtenerVersiones(anio);
var versionEncontrada = _listVersion.Find(x => x.Codigo == codigo);
var versionesService = new VersionesService(...);

// ❌ Abreviaciones fuera de lambdas
private readonly IVersionesRepository _vRepo;
public async Task<List<V>> ObtVers(int a) { ... }

// ✅ Nombres completos
private readonly IVersionesRepository _versionesRepository;
public async Task<List<Version>> ObtenerVersiones(int anio) { ... }
```
