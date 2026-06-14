# DTOs y Fronteras de Datos

## Dónde viven los DTOs

Los DTOs viven en la capa **Domain**, dentro de la carpeta del módulo al que pertenecen, junto a las entidades de dominio. Los servicios de Application los usan como tipo de retorno cuando los datos de presentación difieren de la entidad pura.

```
HM.Presupuestos.Domain/
└── Entidades/
    ├── Condiciones/
    │   ├── Condicion.cs              # Entidad de dominio
    │   ├── CondicionDto.cs           # DTO para presentación/transferencia
    │   ├── ExcepcionDto.cs           # DTO de respuesta
    │   └── Vigencia.cs               # Entidad de dominio
    ├── Sobreprimas/
    │   ├── Sobreprima.cs             # Entidad de dominio
    │   └── SobreprimaGridModel.cs    # DTO para el grid de UI
    └── ...

HM.Presupuestos.Application/
└── CasosDeUso/
    └── Condiciones/
        ├── ICondicionesService.cs    # Define qué devuelve: entidad o DTO
        └── CondicionesService.cs     # Hace el mapeo
```

## DTOs de Respuesta

```csharp
// DTO con lógica de presentación: claves compuestas, redondeo, propiedades calculadas
public class CondicionDto
{
    public string Key => $"{CodigoMedio}";   // Clave para el grid
    public int CodigoMedio { get; set; }
    public string DescripcionMedio { get; set; } = string.Empty;

    private decimal? _pctSAG;
    public decimal? PctSAG
    {
        get => _pctSAG;
        set => _pctSAG = value.HasValue ? Math.Round(value.Value, 2) : null;
    }

    public int NumeroExcepciones { get; set; }  // Dato real de BD (COUNT_EXCEPCIONES)
    // ❌ MedioAccesible NO va aquí — es estado de UI calculado en la página Web
}

// DTO de grid con datos denormalizados (descripciones incluidas para evitar joins en UI)
public class SobreprimaGridModel
{
    public string KeyGrid => $"{CodigoVersion}_{CodigoNetwork}_{CodigoPais}_{CodigoMedio}";
    public int Codigo { get; set; }
    public decimal PorcentajeSobreprimaSLA { get; set; }
    public string DescripcionNetwork { get; set; } = "";
    public string DescripcionMedio { get; set; } = "";
}
```

## Cuándo usar DTO vs Entidad de Dominio

| Usar **Entidad** cuando... | Usar **DTO** cuando... |
|---|---|
| El servicio devuelve datos para persistir | Los datos son para mostrar en grid/formulario |
| Los datos representan el estado del negocio | Se necesitan campos calculados, claves compuestas |
| No hay enriquecimiento de presentación | Se incluyen descripciones de maestros ajenos |
| El llamador va a modificar y guardar | El llamador solo lee / muestra |

```csharp
// Entidad de dominio — para persistencia y lógica de negocio
public class Sobreprima
{
    public int Codigo { get; set; }
    public int CodigoVersion { get; set; }
    public decimal Porcentaje { get; set; }
}

// DTO — para presentación en UI, incluye datos desnormalizados
public class SobreprimaGridModel
{
    public int Codigo { get; set; }
    public decimal PorcentajeSobreprimaSLA { get; set; }
    public string DescripcionNetwork { get; set; } = "";  // enriquecido
    public string DescripcionMedio { get; set; } = "";    // enriquecido
}
```

## Mapeo en el Servicio (Application)

El proyecto no usa AutoMapper. Los mapeos son siempre **manuales e inline** dentro del servicio.

```csharp
// ✅ CORRECTO — mapeo inline con LINQ en el servicio
public async Task<List<CondicionDto>> ObtenerCondicionesPorVigencia(int codigoVigencia, int codigoNetwork)
{
    List<CodigoDescripcion> medios = await _maestrosService.ObtenerMediosPorNetWork(codigoNetwork.ToString());

    return [.. medios.Select(m => new CondicionDto
    {
        CodigoMedio = m.Codigo,
        DescripcionMedio = m.Descripcion
    })];
}
```

Si el DTO necesita datos de múltiples fuentes:

```csharp
// ✅ CORRECTO — mapeo enriquecido con datos de otros servicios
public async Task<List<SobreprimaGridModel>> ObtenerSobreprimasGrid(SobreprimaFiltro filtro)
{
    var sobreprimas = await _sobreprimasRepository.ObtenerSobreprimas(filtro);
    var redes = await _maestrosService.ObtenerRedes();

    return [.. sobreprimas.Select(s => new SobreprimaGridModel
    {
        Codigo = s.Codigo,
        PorcentajeSobreprimaSLA = s.Porcentaje,
        DescripcionNetwork = redes.FirstOrDefault(r => r.Codigo == s.CodigoNetwork)?.Descripcion ?? ""
    })];
}
```

## ViewModels en la capa Web

Cuando una página necesita **estado de UI** que no procede de la base de datos (flags de accesibilidad, listas de opciones para combos, estados de selección, etc.), ese estado **no va en el DTO**. Se crea un ViewModel en la capa Web que hereda del DTO.

```
HM.Presupuestos.Web/
└── Pages/
    └── Condiciones/
        ├── PlanificacionCondiciones.razor
        ├── PlanificacionCondiciones.razor.cs
        ├── CondicionViewModel.cs          # ← ViewModel: CondicionDto + estado UI
        └── ExcepcionCondicionViewModel.cs # ← ViewModel: ExcepcionDto + estado UI
```

```csharp
// CondicionDto vive en Domain — solo datos de persistencia/transferencia
public class CondicionDto
{
    public string Key => $"{CodigoMedio}";
    public int CodigoMedio { get; set; }
    public string DescripcionMedio { get; set; } = string.Empty;
    public int NumeroExcepciones { get; set; }  // viene de COUNT_EXCEPCIONES en BD
}

// CondicionViewModel vive en Web — extiende el DTO con estado de UI
public class CondicionViewModel : CondicionDto
{
    // Calculado en la página comparando el medio contra los medios accesibles del network.
    // No procede de BD.
    public bool MedioAccesible { get; set; }
}
```

```csharp
// ExcepcionDto vive en Domain — solo datos de persistencia/transferencia
public class ExcepcionDto
{
    public string Key => $"{CodigoMedio}_{CodigoCondicionMedio}";
    public int CodigoMedio { get; set; }
    public string CodigoAlcance { get; set; } = string.Empty;
    // ... resto de campos de BD
}

// ExcepcionCondicionViewModel vive en Web — extiende el DTO con listas para combos
public class ExcepcionCondicionViewModel : ExcepcionDto
{
    public bool MedioAccesible { get; set; }
    public List<CodigoDescripcion>? DisciplinasDisponibles { get; set; }
    public List<CodigoDescripcion>? ObjetivosDisponibles { get; set; }
    public List<CodigoDescripcion>? TiposCompraDisponibles { get; set; }
    public List<CodigoDescripcion>? TiposDisciplinaDisponibles { get; set; }
    public List<CodigoDescripcion>? DisciplinasGrupoDisponibles { get; set; }
}
```

### Por qué herencia y no composición

Los servicios de Application reciben `CondicionDto` y `ExcepcionDto` en sus parámetros. Si usáramos composición, habría que mapear manualmente antes de cada llamada. Con herencia, un `CondicionViewModel` **es** un `CondicionDto` y se puede pasar directamente — con una excepción:

### Problema de covarianza en Dictionary

`Dictionary<CondicionViewModel, V>` **no** es asignable a `Dictionary<CondicionDto, V>` en C#. Se resuelve con una conversión explícita antes de la llamada al servicio:

```csharp
// ❌ No compila — Dictionary no es covariante
await _condicionesService.GrabarCondicionesExcepciones(
    _condicionesNoGuardadas,   // Dictionary<CondicionViewModel, ...>
    ...);

// ✅ Correcto — conversión explícita de clave
await _condicionesService.GrabarCondicionesExcepciones(
    _condicionesNoGuardadas.ToDictionary(kvp => (CondicionDto)kvp.Key, kvp => kvp.Value),
    ...);
```

### Cuándo crear un ViewModel

| Crear **ViewModel** cuando... | El DTO es suficiente cuando... |
|---|---|
| La página necesita flags de estado UI (`MedioAccesible`, `Seleccionado`...) | La página solo muestra los datos del DTO |
| Hay listas de opciones para combos/selects que no vienen de BD | No hay estado mutable en la UI por fila |
| El grid necesita propiedades que no existen en el dominio | El componente solo lee y no necesita estado extra |

---

## Características de los DTOs

- **Solo propiedades**: no contienen lógica de negocio
- **Tipos primitivos o DTOs anidados**: `string`, `int`, `decimal`, `bool`, `List<T>`, otros DTOs
- **Pueden tener propiedades calculadas de presentación**: claves compuestas para grid (`Key`, `KeyGrid`), redondeo de decimales en setter
- **No contienen entidades de dominio** como tipo de propiedad
- **Inicializadores por defecto**: `string.Empty`, `[]`, `new()` para evitar nulls

```csharp
// ❌ MAL — contiene tipo de dominio como propiedad
public class CondicionDto
{
    public Vigencia Vigencia { get; set; }   // tipo de dominio
}

// ✅ CORRECTO — solo primitivos y DTOs
public class CondicionDto
{
    public int CodigoVigencia { get; set; }
    public string DescripcionVigencia { get; set; } = string.Empty;
}
```

## Definición en la interfaz del servicio

El contrato `IXxxService` declara explícitamente si un método devuelve entidad o DTO:

```csharp
public interface ICondicionesService
{
    // Devuelve entidad — para operaciones de dominio
    Task<List<Vigencia>> ObtenerVigencias(CondicionFiltro filtro);

    // Devuelve DTO — datos enriquecidos para presentación
    Task<List<CondicionDto>> ObtenerCondicionesPorVigencia(int codigoVigencia, int codigoNetwork);
    Task<List<ExcepcionDto>> ObtenerExcepcionesCondiciones(int codigoVigencia);

    // Recibe DTO como entrada — datos de la UI hacia el dominio
    Task GrabarCondicionesExcepciones(
        Dictionary<CondicionDto, DatosCondicionCambiados> condicionesNoGuardadas,
        Dictionary<ExcepcionDto, DatosExcepcionesCondicionCambiados> excepcionesNoGuardadas,
        int codigoVigencia);
}
```
