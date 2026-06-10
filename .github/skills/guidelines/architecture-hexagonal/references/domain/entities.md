# Entidades de Dominio

Las entidades son objetos con **identidad y ciclo de vida**. En este proyecto, la identidad suele ser un `int Codigo` (clave de BD). Dos entidades son iguales si tienen el mismo `Codigo`, independientemente del resto de atributos.

## Características

- **Identidad**: Campo `Codigo` (o clave compuesta) que viene de la base de datos
- **Propiedades con lógica de presentación**: `IconoCssClass`, `DisplayText`, `NombreCampo` — calculadas, sin setter
- **Redondeo en setter**: `decimal` se redondea a 2 decimales en el propio setter de la entidad
- **Sin lógica de negocio compleja**: la validación y orquestación van en el servicio de Application
- **Tipos con DataAnnotations** cuando se usan en formularios Blazor (`[Required]`)

## Estructura

```
HM.Presupuestos.Domain/
├── Compartido/
│   ├── Constantes.cs          # Constantes estáticas (BitAnd, entornos, session keys...)
│   ├── Enumerados.cs          # Todos los enumerados del dominio
│   └── ValidacionException.cs # Excepción tipada para errores de validación
├── Entidades/
│   ├── CodigoDescripcion.cs   # Clase base para maestros con Codigo + Descripcion
│   ├── Condiciones/
│   │   ├── Condicion.cs       # Entidad de persistencia para condiciones
│   │   ├── Vigencia.cs        # Entidad: hereda CodigoDescripcion
│   │   ├── CondicionDto.cs    # DTO de presentación del grid de condiciones
│   │   ├── ExcepcionDto.cs    # DTO de presentación del grid de excepciones
│   │   └── CondicionFiltro.cs # Parámetros de consulta
│   ├── Indicadores/
│   │   ├── Indicador.cs       # Entidad con propiedades calculadas (idiomas)
│   │   └── IdiomaIndicador.cs # Entidad de traducción
│   ├── Sobreprimas/
│   │   ├── Sobreprima.cs      # Entidad con redondeo en setter
│   │   └── SobreprimaGridModel.cs  # DTO desnormalizado para grid
│   ├── Versiones/
│   │   ├── Version.cs         # Entidad con IconoCssClass + BitAnd
│   │   ├── VersionResumen.cs  # DTO enriquecido con propiedades de estado
│   │   └── VersionLinea.cs
│   └── Utilidades/
│       └── IConIcono.cs       # Interfaces IConIcono, IConCodigo
├── Extensiones/
│   └── EnumExtensions.cs      # .ObtenerDescripcion() para enumerados
└── Puertos/
    └── Repositorios/          # Interfaces de repositorio (puertos de salida)
```

---

## Clase Base: `CodigoDescripcion`

La mayoría de maestros heredan de `CodigoDescripcion`, que implementa `IConCodigo` e `IConIcono`:

```csharp
public class CodigoDescripcion : IConCodigo, IConIcono
{
    public int Codigo { get; set; }
    public string Descripcion { get; set; } = "";

    // Propiedades calculadas de presentación
    public string DescripcionConCodigo => $"{Descripcion} ({Codigo})";
    public string CodigoConDescripcion => $"{Descripcion} ({Codigo})";

    public string IconoCssClass => string.Empty;  // Override en subclases
    public string DisplayText => Descripcion;     // Override en subclases
}

// Herencia típica de maestro simple
public class Vigencia : CodigoDescripcion
{
    public int CodigoVersion { get; set; }
    public int CodigoNetWork { get; set; }
    public int CodigoGrupoCliente { get; set; }
    public int MesDesde { get; set; }
    public int MesHasta { get; set; }
    public int IndicadorAcuerdo { get; set; }
}
```

---

## Interfaces de Presentación

Definidas en `HM.Presupuestos.Domain/Entidades/Utilidades/IConIcono.cs`, se usan para enlazar entidades a comboboxes DevExpress:

```csharp
// Entidades que pueden mostrarse en un combo/lista desplegable implementan estas interfaces
public interface IConIcono
{
    string? IconoCssClass { get; }  // Icono CSS para cada item
    string DisplayText { get; }     // Texto visible en el combo
}

public interface IConCodigo
{
    int Codigo { get; }             // Permite obtener el código del item seleccionado
}
```

---

## Redondeo de Decimales en el Setter

Las propiedades `decimal` que representan porcentajes se redondean a 2 decimales **en el propio setter** de la entidad. Esto garantiza consistencia independientemente de dónde venga el valor:

```csharp
// ✅ CORRECTO — redondeo en setter de la entidad
public class Sobreprima
{
    public int Codigo { get; set; }
    public int CodigoVersion { get; set; }
    public int CodigoConcepto { get; set; }
    public int CodigoMedio { get; set; }

    private decimal _porcentaje;
    public decimal Porcentaje
    {
        get => _porcentaje;
        set => _porcentaje = Math.Round(value, 2);
    }
}

public class Condicion
{
    public int CodigoVigencia { get; set; }
    public int CodigoMedio { get; set; }
    public ConceptosCondiciones CodigoConcepto { get; set; }  // enum como tipo de propiedad

    private decimal? _porcentaje;
    public decimal? Porcentaje
    {
        get => _porcentaje;
        set => _porcentaje = value.HasValue ? Math.Round(value.Value, 2) : null;
    }
}
```

---

## Propiedades Calculadas de Presentación

Las entidades pueden tener propiedades calculadas (`get` sin `set`) para lógica de presentación. Esto es aceptable siempre que no sean reglas de negocio que deban testearse independientemente:

```csharp
public class Version : IConIcono, IConCodigo
{
    public int Codigo { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public int IndEstado { get; set; }          // Valor BitAnd de estados
    public bool IsDataLinked { get; set; }      // Calculado por el servicio, no en BD

    // Propiedad calculada: icono según estado BitAnd
    public string IconoCssClass
    {
        get
        {
            var cerrada = (IndEstado & Constantes.BitAndVersion.CERRADA) != 0;
            return cerrada ? "fas fa-lock" : "fas fa-pen";
        }
    }

    public string DisplayText => Descripcion;
}

// VersionResumen hereda de CodigoDescripcion y extiende con lógica de estado
public class VersionResumen : CodigoDescripcion, IConIcono
{
    public int IndEstado { get; set; }
    public bool EsUsuarioAdmin { get; set; }

    public bool Desbloqueada => (IndEstado & Constantes.BitAndVersion.DESBLOQUEADA) != 0;

    public new string IconoCssClass
    {
        get
        {
            var cerrada = (IndEstado & Constantes.BitAndVersion.CERRADA) != 0;
            var clase = cerrada ? "fas fa-lock" : "fas fa-pen";
            return EsUsuarioAdmin && Desbloqueada ? clase + " text-success" : clase;
        }
    }
}
```

---

## Entidades con Idiomas (Internacionalización)

La entidad `Indicador` tiene propiedades que devuelven texto según el idioma activo del usuario, buscando en una lista de `IdiomaIndicador`:

```csharp
public class Indicador
{
    public int? Codigo { get; set; }
    public int Indice { get; set; }     // Posición en el BitAnd de estados de versión
    public int Orden { get; set; }
    public int CodigoIdioma { get; set; }   // Idioma activo (para resolver las propiedades traducidas)
    public string Descripcion { get; set; } = string.Empty;
    public List<IdiomaIndicador> Idiomas { get; set; } = [];

    // Nombre del campo en la entidad Version ("IndicadorEstado1", "IndicadorEstado2"...)
    public string NombreCampo => $"IndicadorEstado{Indice}";

    // Devuelve la descripción en el idioma activo, con truncado automático si no hay traducción
    public string DescripcionAbreviada
    {
        get
        {
            IdiomaIndicador? idioma = Idiomas.Find(c => c.CodigoIdioma == CodigoIdioma);
            if (idioma == null)
                return Descripcion.Length <= 4 ? Descripcion : $"{Descripcion[..4]}.";
            return idioma.DescripcionAbreviada;
        }
    }

    public string DescripcionTraducida
    {
        get
        {
            IdiomaIndicador? idioma = Idiomas.Find(c => c.CodigoIdioma == CodigoIdioma);
            return idioma?.Descripcion ?? Descripcion;
        }
    }
}
```

---

## Enumerados (`Enumerados.cs`)

Todos los enumerados del dominio se centralizan en `HM.Presupuestos.Domain/Compartido/Enumerados.cs`:

```csharp
// Control de flujo en la UI
public enum EstadoEntidad { SinCambios = 0, Modificado = 1, Nuevo = 2, Eliminado = 3 }
public enum ModoOperacion { Ninguna = 0, Consultar = 1, Insertar = 2, Modificar = 3, Eliminar = 4 }

// Conceptos de negocio
public enum ConceptosSobreprimas { Sobreprima = 1, SLA = 2, HVP = 3 }
public enum ConceptosCondiciones { Sag = 1, Manpower = 2, Devolucion = 3, BaseCalculoDevolucion = 4 }

// Menú y permisos
public enum CodigosMenu
{
    Versiones = 1,
    PlanificacionCondiciones = 5,
    Sobreprimas = 14,
    // ...
}
```

> **Regla**: al añadir un nuevo valor a `CodigosMenu`, hay que añadir también la entrada `Menu_N` correspondiente en los tres ficheros de recursos JSON de la aplicación:
> - `HM.Presupuestos.Web/wwwroot/data/app.es.json`
> - `HM.Presupuestos.Web/wwwroot/data/app.en.json`
> - `HM.Presupuestos.Web/wwwroot/data/app.pt.json`
>
> Estructura de la entrada:
> ```json
> "Menu_26": {
>   "label": "Auditorías",
>   "url": "/admin/auditorias",
>   "icono": "fa-solid fa-magnifying-glass-chart",
>   "visible": true,
>   "code": 26
> }
> ```

```csharp

// Errores de validación tipados (usados con ValidacionException)
public enum CampoErrorValidacion { Descripcion, Orden, BitAnd }
```

Para obtener la descripción de un enumerado, usar la extensión `ObtenerDescripcion()`:

```csharp
// Extensión en HM.Presupuestos.Domain/Extensiones/EnumExtensions.cs
public static class EnumExtensions
{
    public static string ObtenerDescripcion(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttributes(typeof(DescriptionAttribute), false)
                              .FirstOrDefault() as DescriptionAttribute;
        return attribute?.Description ?? value.ToString();
    }
}

// Uso:
[Description("Planificación de Condiciones")]
PlanificacionCondiciones = 5,

var texto = CodigosMenu.PlanificacionCondiciones.ObtenerDescripcion();
// → "Planificación de Condiciones"
```

---

## Excepción de Validación Tipada

En lugar de lanzar `Exception` genéricas, el dominio usa `ValidacionException` cuando un valor ya existe en BD:

```csharp
public class ValidacionException : Exception
{
    public CampoErrorValidacion CampoValidado { get; }
    public string Valor { get; }

    public ValidacionException(CampoErrorValidacion campoValidado, string valor)
        : base("Error de validación")
    {
        CampoValidado = campoValidado;
        Valor = valor;
    }
}

// Uso en el servicio de Application
if (await ExisteDescripcion(indicador.Descripcion, indicador.Codigo))
    throw new ValidacionException(CampoErrorValidacion.Descripcion, indicador.Descripcion);

// Captura en la página Web para mostrar mensaje localizado
catch (ValidacionException ex)
{
    MostrarError(ObtenerTexto(TextosApp.Indicadores.ErrorDuplicado, ex.Valor));
}
```

---

## Constantes (`Constantes.cs`)

Las constantes se agrupan en clases estáticas anidadas por área:

```csharp
public static class Constantes
{
    public const int NUMERO_ESTADOS = 25;

    // Valores BitAnd para el campo IndEstado de Version
    public static class BitAndVersion
    {
        public const int CERRADA      = 1;
        public const int DESBLOQUEADA = 2;
        public const int PUBLICADA    = 8;
        public const int ABIERTA      = 64;
        public const int REAL         = 32768;
    }

    // Entornos de despliegue
    public static class Environment
    {
        public const string DEV = "DEV";
        public const string PRU = "PRU";
        public const string PRE = "PRE";
        public const string PRO = "PRO";
    }

    // Claves de sesión
    public static class Session
    {
        public const string USER_SSO   = "UsuarioSSO";
        public const string USER_LOGIN = "UsuarioLogin";
    }
}

// Uso del BitAnd para comprobar estado de una versión
var estaCerrada  = (version.IndEstado & Constantes.BitAndVersion.CERRADA) != 0;
var estaAbierta  = (version.IndEstado & Constantes.BitAndVersion.ABIERTA) != 0;
```

---

## Qué va en Domain vs en Application

| Domain | Application |
|---|---|
| Entidades con identidad (`Version`, `Indicador`, `Sobreprima`...) | Servicios que orquestan entidades |
| DTOs que retornan los repositorios del Domain | Filtros y objetos de entrada de use-cases |
| Interfaces de repositorio (`ICondicionesRepository`) | Implementación de los servicios |
| Enumerados, constantes, `ValidacionException` | Mapeos de entidades a DTOs |
| Interfaces de presentación (`IConIcono`, `IConCodigo`) | |

**Regla clave:** Domain no referencia a Application ni a Infrastructure. Es el núcleo sin dependencias externas.
