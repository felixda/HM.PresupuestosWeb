# Estándares de Clases y Módulos

## Reglas

1. **Ámbito mínimo para máxima cohesión**. Si un campo o método solo lo usa una clase y nadie más, hazlo `private`. Si solo lo usa un método concreto, decláralo dentro de ese método.

2. **Sin `using` innecesarios**. Eliminar todos los `using` que el compilador o el IDE marquen como no utilizados. Solo deben aparecer los namespaces realmente referenciados en el fichero.

3. **Constructores simples**. Los constructores solo asignan dependencias. La validación de datos se hace en el método que los recibe, no en el constructor. No crear factory methods innecesarios cuando no hay validación compleja.

4. **Organización de la clase**: primero `[Inject]` y propiedades públicas, luego propiedades privadas con `#region`, después ciclo de vida del componente (`InicializarPaginaAsync`, `OnPermisoDenegadoAsync`), finalmente métodos privados. Usar `#region` para agrupar secciones relacionadas en clases grandes.

5. **Encapsulación por defecto**. Usar `private` para campos y métodos que no necesita el exterior. Los campos de estado de la página son `private`. Las dependencias inyectadas son `protected` (accesibles desde la clase base). No exponer lo que no se necesita.

6. **Tell, Don't Ask**. En lugar de preguntar el estado al objeto y decidir fuera, decirle al objeto lo que tiene que hacer.

7. **Sin modelos anémicos en el dominio**. Las entidades de dominio encapsulan comportamiento (propiedades calculadas, redondeo en setter). Los DTOs y ViewModels sí son planos — son objetos de transferencia, no de dominio.

8. **Objetos completos al construir**. Inicializar siempre con valores por defecto: `= []` para listas, `= string.Empty` para strings, `= new()` para objetos. Nunca dejar campos sin inicializar que puedan lanzar `NullReferenceException`.

9. **Getters/setters solo cuando tienen lógica**. No usar `get => _campo; set => _campo = value;` triviales — usar autopropiedades `{ get; set; }` directamente. Los getters/setters con lógica (redondeo, notificación de cambio) sí están justificados.

10. **Sin prefijo underscore en campos privados** cuando el campo es el parámetro del primary constructor asignado directamente. Sí usar `_` cuando se almacena el parámetro del primary constructor en un campo explícito (patrón del proyecto).

11. **Sin singletons**. Todos los servicios se registran como `AddScoped` en `Program.cs`. El estado por-usuario vive en `SesionUsuario` (scoped), no en singletons.

12. **Composición sobre herencia** — salvo en las jerarquías establecidas del proyecto:
    - Páginas heredan de `ContextProtegido` o `Context`
    - ViewModels heredan del DTO correspondiente (`CondicionViewModel : CondicionDto`)
    - Repositorios heredan de `BasePresupuestosRepository`

13. **Tipos específicos del dominio**. Usar enumerados del dominio como tipos de propiedad en lugar de `int` cuando representen conceptos de negocio (`ConceptosCondiciones CodigoConcepto` en lugar de `int CodigoConcepto`).

---

## Ejemplos

### Organización de una Página Blazor

```csharp
// ✅ CORRECTO — organización con #region por secciones
public partial class Indicadores : ContextProtegido
{
    #region Inyección de Dependencias

    [Inject] protected IIndicadoresService IndicadoresService { get; set; } = default!;

    #endregion

    #region Propiedades para Data Binding

    private DxGrid GridIndicadores { get; set; } = new DxGrid();
    private List<Indicador> DatosIndicadores { get; set; } = [];
    private Indicador IndicadorEnEdicion { get; set; } = new();
    private bool EsPopupEdicionVisible { get; set; }

    #endregion

    #region Ciclo de Vida del Componente

    protected override async Task InicializarPaginaAsync()
    {
        DatosIndicadores = await IndicadoresService.ObtenerIndicadoresConIdiomas(null);
    }

    protected override Task OnPermisoDenegadoAsync()
    {
        return Task.CompletedTask;
    }

    #endregion

    #region Métodos Privados

    private async Task GuardarIndicador() { ... }

    #endregion
}

// ❌ INCORRECTO — todo mezclado sin organización
public partial class Indicadores : ContextProtegido
{
    [Inject] protected IIndicadoresService IndicadoresService { get; set; } = default!;
    private List<Indicador> DatosIndicadores { get; set; } = [];
    private async Task GuardarIndicador() { ... }
    protected override async Task InicializarPaginaAsync() { ... }
    private DxGrid GridIndicadores { get; set; } = new DxGrid();
}
```

### Constructores: Primary Constructor + Campos Explícitos

```csharp
// ✅ CORRECTO — primary constructor + campos con _ para almacenamiento
public class SobreprimasService(
    ILogger<SobreprimasService> logger,
    ISobreprimasRepository sobreprimasRepository,
    ILogAccionesService logAccionesService) : ISobreprimasService
{
    private readonly ILogger<SobreprimasService> _logger = logger;
    private readonly ISobreprimasRepository _sobreprimasRepository = sobreprimasRepository;
    private readonly ILogAccionesService _logAccionesService = logAccionesService;

    // Los parámetros se almacenan en campos privados con prefijo _
}

// ❌ INCORRECTO — validación en el constructor
public class SobreprimasService(ISobreprimasRepository repo) : ISobreprimasService
{
    private readonly ISobreprimasRepository _repo =
        repo ?? throw new ArgumentNullException();  // validación en constructor
}
```

### Getters con Lógica Justificada vs Autopropiedades

```csharp
// ✅ CORRECTO — setter con lógica real (redondeo)
public class Sobreprima
{
    private decimal _porcentaje;
    public decimal Porcentaje
    {
        get => _porcentaje;
        set => _porcentaje = Math.Round(value, 2);   // lógica justificada
    }
}

// ✅ CORRECTO — getter calculado con BitAnd (lógica real)
public class VersionResumen : CodigoDescripcion
{
    public int IndEstado { get; set; }

    public bool Desbloqueada => (IndEstado & Constantes.BitAndVersion.DESBLOQUEADA) != 0;

    public string IconoCssClass
    {
        get
        {
            var cerrada = (IndEstado & Constantes.BitAndVersion.CERRADA) != 0;
            return cerrada ? "fas fa-lock" : "fas fa-pen";
        }
    }
}

// ✅ CORRECTO — setter con lógica (notificación + efecto secundario)
private object? NetworksSeleccionados
{
    get => _networksSeleccionados;
    set
    {
        if (_networksSeleccionados != value)
        {
            _networksSeleccionados = value;
            _ = ActualizarMediosCuandoModificamosNetworks(...);  // efecto secundario
        }
    }
}

// ❌ INCORRECTO — getter/setter trivial, usar autopropied
public class Filtro
{
    private int _anio;
    public int Anio
    {
        get => _anio;         // sin lógica → usar { get; set; }
        set => _anio = value;
    }
}
```

### Tell, Don't Ask

```csharp
// ❌ PEOR — preguntar estado y decidir fuera
if (sobreprima.ConceptoDefaul.Codigo > 0)
    codigosAExcluir.Add(sobreprima.ConceptoDefaul.Codigo.ToString());
if (sobreprima.ConceptoSLA.Codigo > 0)
    codigosAExcluir.Add(sobreprima.ConceptoSLA.Codigo.ToString());
if (sobreprima.ConceptoHVP.Codigo > 0)
    codigosAExcluir.Add(sobreprima.ConceptoHVP.Codigo.ToString());

// ✅ MEJOR — LINQ encapsula la decisión
var codigosExcluir = new[]
{
    sobreprima.ConceptoDefaul.Codigo,
    sobreprima.ConceptoSLA.Codigo,
    sobreprima.ConceptoHVP.Codigo
}
.Where(codigo => codigo > 0)
.Select(codigo => codigo.ToString());
```

### Objetos Completos al Construir

```csharp
// ✅ CORRECTO — inicializadores por defecto en todas las propiedades
public class Sobreprimas : ContextProtegido
{
    private SobreprimaFiltro _filtroSobreprima = new();           // new() en lugar de null
    private List<CodigoDescripcion> AñosMaestros { get; set; } = [];   // [] en lugar de null
    private VersionResumen? VersionSeleccionada { get; set; }     // ? cuando null es válido
    private string CaptionIzquierda { get; set; } = string.Empty; // string.Empty, nunca null
}

// ❌ INCORRECTO — campos sin inicializar
public class Sobreprimas : ContextProtegido
{
    private SobreprimaFiltro _filtroSobreprima;       // NullReferenceException en tiempo de ejecución
    private List<CodigoDescripcion> AñosMaestros;     // NullReferenceException al iterar
    private string CaptionIzquierda;                  // null en lugar de ""
}
```

### Tipos del Dominio como Tipos de Propiedad

```csharp
// ✅ CORRECTO — tipo enumerado para propiedades con semántica de negocio
public class Condicion
{
    public ConceptosCondiciones CodigoConcepto { get; set; }  // enum, no int
}

// También en llamadas al repositorio
await condicionesRepository.EliminarConceptoNMDExcepcionCondicion(
    codigoCondicionMedio,
    ConceptosCondicionesNMD.Alcance);   // enum, legible y seguro

// ❌ INCORRECTO — int para algo que tiene semántica de dominio
public class Condicion
{
    public int CodigoConcepto { get; set; }  // ¿qué valores son válidos? no queda claro
}
```

### Validación: Lanzar, No Silenciar

```csharp
// ✅ CORRECTO — lanzar excepción tipada del dominio para errores de validación
if (await _indicadoresRepository.ExisteIndicador(indicador))
    throw new ValidacionException(CampoErrorValidacion.Descripcion, indicador.Descripcion);

// ✅ CORRECTO — lanzar en propiedades de infraestructura cuando el estado es inválido
protected int CodigoUsuario =>
    Jwt.Usuario?.CodigoUsuario
    ?? throw new InvalidOperationException("No hay usuario autenticado.");

// ❌ INCORRECTO — silenciar errores devolviendo valores por defecto
protected int CodigoUsuario =>
    Jwt.Usuario?.CodigoUsuario ?? 0;  // oculta el problema, produce errores silenciosos
```
