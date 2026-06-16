# Implementación de Repositorios (Infrastructure)

Las implementaciones concretas de los puertos de repositorio viven en `HM.Presupuestos.Infrastructure/Persistencia/`. Implementan las interfaces definidas en `HM.Presupuestos.Domain/Puertos/Repositorios/`.

> **Referencia de código**: `VersionesRepository.cs` es el repositorio de referencia para los patrones de este documento.

Ver las interfaces correspondientes en: [domain/repositories.md](../domain/repositories.md)

## Estructura

```
HM.Presupuestos.Infrastructure/
└── Persistencia/
    ├── BasePresupuestosRepository.cs      ← Clase base con DAL + JWT
    ├── PresupuestosRepository.cs          ← IPresupuestosRepository (maestros, ~52 métodos)
    ├── Admin/
    │   └── AdminRepository.cs
    ├── Condiciones/
    │   └── CondicionesRepository.cs
    ├── Configuracion/
    │   └── ConfiguracionRepository.cs
    ├── LogAcciones/
    │   └── LogAccionesRepository.cs
    ├── Sobreprimas/
    │   └── SobreprimasRepository.cs
    └── Versiones/
        ├── VersionesRepository.cs
        └── IndicadoresRepository.cs
```

---

## Clase Base — `BasePresupuestosRepository`

Todos los repositorios heredan de `BasePresupuestosRepository`, que encapsula el acceso a datos (`IDataAccessHelperSecure` de HM.Core) y el usuario autenticado (`IJwt`):

```csharp
// HM.Presupuestos.Infrastructure/Persistencia/BasePresupuestosRepository.cs
public abstract class BasePresupuestosRepository(
    IDataAccessHelperSecure dah,
    IJwt jwt) : BaseDAL(dah)
{
    protected readonly IJwt Jwt = jwt;

    protected int CodigoAplicacion =>
        Jwt.Usuario?.CodigoAplicacion
        ?? throw new InvalidOperationException("No hay usuario autenticado.");

    protected int CodigoUsuario =>
        Jwt.Usuario?.CodigoUsuario
        ?? throw new InvalidOperationException("No hay usuario autenticado.");

    protected int CodigoPais =>
        Jwt.Usuario?.CodigoPais
        ?? throw new InvalidOperationException("No hay usuario autenticado.");

    protected async Task AñadirParametroMulticompania(IDataAccessHelperBase dahHelper) { ... }
}
```

### Propiedades Calculadas vs Campos

`CodigoUsuario`, `CodigoPais` y `CodigoAplicacion` son **propiedades calculadas** (no campos `readonly` asignados en el constructor) porque en Blazor Server el usuario se autentica **después** de que el DI construye los servicios. Un campo asignado en el constructor siempre obtendría `null`:

```csharp
// ❌ INCORRECTO — se evalúa en el constructor (antes de autenticación en Blazor Server)
public class MiRepository(IDataAccessHelperSecure dah, IJwt jwt) : BaseDAL(dah)
{
    private readonly int _codigoUsuario = jwt.Usuario?.CodigoUsuario ?? 0;  // siempre 0
}

// ✅ CORRECTO — se evalúa al acceder (cuando el usuario ya está autenticado)
protected int CodigoUsuario =>
    Jwt.Usuario?.CodigoUsuario
    ?? throw new InvalidOperationException("No hay usuario autenticado.");
```

---

## Implementación de un Repositorio

Cada repositorio usa primary constructor (C# 12), pasa `dah` y `jwt` a la base, e implementa la interfaz de dominio correspondiente:

```csharp
// ✅ CORRECTO — primary constructor + herencia de BasePresupuestosRepository
public class VersionesRepository(
    IJwt jwt,
    IDataAccessHelperBase dahBase,
    IDataAccessHelperSecure dah) : BasePresupuestosRepository(dah, jwt), IVersionesRepository
{
    protected readonly IJwt jwt = jwt;
    protected new readonly IDataAccessHelperSecure dah = dah;
    protected readonly IDataAccessHelperBase dahBase = dahBase;

    public new ITransaccion ObtenerTransaccion() => new TransaccionWrapper(base.ObtenerTransaccion());

    // métodos...
}
```

- Hereda siempre de `BasePresupuestosRepository(dah, jwt)`
- Implementa la interfaz `IXxxRepository` definida en `Domain/Puertos/Repositorios/`
- `ITransaccion ObtenerTransaccion()` se sobreescribe con `TransaccionWrapper` cuando el repositorio participa en transacciones

**Patrones que se repiten en todos los repositorios:**
- `dah.GetSqlStringComando(query)` — prepara el comando SQL directo
- `dah.AddParameter(nombre, valor)` — parámetros con nombre (sintaxis Oracle `:Nombre`)
- `await AñadirParametroMulticompania(dah)` — añade filtro de compañía (en métodos que lo requieren)
- `await Task.Run(() => dah.ProcesarDatos(...))` — ejecuta en thread pool (DAL síncrona envuelta en async)
- `dr.GetInt32(columna)` / `dr.GetString(columna)` — lectura tipada de `IDataReader`

---

## Patrón 1: Query estática (SELECT sin partes dinámicas)

Cuando la query no cambia en tiempo de ejecución: `const string` con verbatim string `@"..."` alineado con la estructura SQL.

```csharp
// ✅ CORRECTO — ObtenerEstadosVersiones en VersionesRepository.cs
public async Task<List<Indicador>> ObtenerEstadosVersiones()
{
    List<Indicador> resultado = [];

    const string query = @"
        SELECT COD_ESTADO_VERSION, BITAND, DES_ESTADO_VERSION, IND_MOSTRAR, IND_VERSION_UNICA, ORDEN
          FROM PPT_ESTADOS_VERSIONES
         ORDER BY ORDEN, COD_ESTADO_VERSION";

    dah.GetSqlStringComando(query);

    await AñadirParametroMulticompania(dah);

    await Task.Run(() =>
    {
        dah.ProcesarDatos(dr =>
        {
            int indice = 1;
            while (dr.Read())
            {
                resultado.Add(new Indicador
                {
                    Codigo      = dr.GetInt32("COD_ESTADO_VERSION"),
                    Descripcion = dr.GetNullableString("DES_ESTADO_VERSION"),
                    BitAnd      = dr.GetInt32("BITAND"),
                    Indice      = indice++,
                });
            }
        });
    });

    return resultado;
}

// ❌ INCORRECTO — StringBuilder innecesario para una query que no cambia
public async Task<List<Indicador>> ObtenerEstadosVersiones()
{
    StringBuilder query = new StringBuilder();   // ← innecesario
    query.AppendLine("SELECT COD_ESTADO_VERSION ...");
    ...
}
```

---

## Patrón 2: Query dinámica con `if` (partes opcionales)

Cuando hay cláusulas que se añaden o no según los parámetros recibidos, siempre se usa **interpolación** `$@"..."` con expresiones ternarias, independientemente del número de condiciones.

```csharp
// ✅ CORRECTO — ObtenerVersiones en VersionesRepository.cs
public async Task<List<Version>> ObtenerVersiones(int anio, int? estadoIncluido = null)
{
    List<Version> resultado = [];

    string query = $@"
        SELECT COD_VERSION, DES_VERSION, MES_VERSION, IND_ESTADO_VERSION, ORDEN, COD_TIPO_VERSION
          FROM PPT_VERSIONES
         WHERE ANIO = :Anio
               {(estadoIncluido.HasValue ? "AND BITAND(IND_ESTADO_VERSION, :indEstado) = :indEstado" : "")}
         ORDER BY ORDEN DESC, DES_VERSION";

    dah.GetSqlStringComando(query);
    dah.AddParameter("Anio", anio);

    if (estadoIncluido.HasValue)
        dah.AddParameter("indEstado", estadoIncluido.Value);

    await AñadirParametroMulticompania(dah);

    await Task.Run(() =>
    {
        dah.ProcesarDatos(dr =>
        {
            while (dr.Read())
            {
                resultado.Add(new Version
                {
                    Codigo      = dr.GetInt32("COD_VERSION"),
                    Descripcion = dr.GetString("DES_VERSION"),
                    Anio        = anio,
                });
            }
        });
    });

    return resultado;
}
```

El mismo patrón con **múltiples condiciones** opcionales:

```csharp
// ✅ CORRECTO — varias condiciones opcionales, igualmente con interpolación
public async Task<List<VersionResumen>> ObtenerVersionesResumen(
    int? anio = null, int? estadoIncluido = null, int? estadoExcluido = null)
{
    List<VersionResumen> resultado = [];

    string query = $@"
        SELECT COD_VERSION, DES_VERSION, IND_ESTADO_VERSION, COD_TIPO_VERSION
          FROM PPT_VERSIONES
         WHERE 1=1
               {(anio.HasValue           ? "AND ANIO = :Anio"                                                        : "")}
               {(estadoIncluido.HasValue ? "AND BITAND(IND_ESTADO_VERSION, :IndEstado) = :IndEstado"                 : "")}
               {(estadoExcluido.HasValue ? "AND BITAND(IND_ESTADO_VERSION, :IndEstadoQuitar) != :IndEstadoQuitar"    : "")}
         ORDER BY DES_VERSION";

    dah.GetSqlStringComando(query);

    if (anio.HasValue)           dah.AddParameter("Anio",            anio.Value);
    if (estadoIncluido.HasValue) dah.AddParameter("IndEstado",        estadoIncluido.Value);
    if (estadoExcluido.HasValue) dah.AddParameter("IndEstadoQuitar",  estadoExcluido.Value);

    await AñadirParametroMulticompania(dah);

    await Task.Run(() =>
    {
        dah.ProcesarDatos(dr =>
        {
            while (dr.Read())
            {
                resultado.Add(new VersionResumen
                {
                    Codigo      = dr.GetInt32("COD_VERSION"),
                    Descripcion = dr.GetString("DES_VERSION"),
                });
            }
        });
    });

    return resultado;
}
```

### Antipatrones a evitar

```csharp
// ❌ INCORRECTO 1 — concatenación con += sobre string
string query = @"SELECT ... FROM PPT_VERSIONES WHERE 1=1";
if (anio.HasValue)
    query += " AND ANIO = :Anio";       // concatenación frágil

// ❌ INCORRECTO 2 — StringBuilder para construir query con condiciones
StringBuilder query = new();
query.AppendLine("SELECT ...");
if (anio.HasValue)
    query.AppendLine("AND ANIO = :Anio");   // usar interpolación en su lugar

// ❌ INCORRECTO 3 — parámetro fuera del if que controla su cláusula
string query = $@"... {(anio.HasValue ? "AND ANIO = :Anio" : "")} ...";
dah.GetSqlStringComando(query);
dah.AddParameter("Anio", anio ?? 0);   // ← siempre se añade, aunque la cláusula no esté
```

---

## Patrón 3: INSERT con RETURNING

Para obtener el código generado por Oracle (secuencia / trigger).

```csharp
// ✅ CORRECTO — InsertarVersion en VersionesRepository.cs
public async Task<int> InsertarVersion(int codigoPais, Version version)
{
    const string query = @"
        INSERT INTO PPT_VERSIONES (COD_PAIS, DES_VERSION, ANIO, MES_VERSION, IND_ESTADO_VERSION, ORDEN, COD_TIPO_VERSION)
        VALUES (:CodigoPais, :DesVersion, :Anio, :Mes, :Estado, :Orden, :Tipo)
        RETURNING COD_VERSION INTO :CodigoVersion";

    dah.GetSqlStringComando(query);

    dah.AddParameter("CodigoPais", codigoPais);
    dah.AddParameter("DesVersion", version.Descripcion);
    dah.AddParameter("Anio",       version.Anio);
    dah.AddParameter("Mes",        version.Mes);
    dah.AddParameter("Estado",     version.IndEstado);
    dah.AddParameter("Orden",      version.Orden);
    dah.AddParameter("Tipo",       version.CodigoTipo);

    // Parámetro de salida — siempre al final
    dah.AddParameter("CodigoVersion", version.Codigo, DbType.Int32, ParameterDirection.Output, 10);

    await Task.Run(() => dah.ExecuteNonQuery());

    version.Codigo = Convert.ToInt32(dah.Comando.Parameters["CodigoVersion"].Value);
    return version.Codigo;
}
```

---

## Patrón 4: Stored Procedure

Cuando la operación requiere lógica en BD se usa `GetStoredProcComando()` en lugar de `GetSqlStringComando()`. El SP devuelve un código de resultado por parámetro `OUTPUT`.

### SP que solo ejecuta (INSERT / UPDATE / DELETE)

```csharp
// ✅ CORRECTO — GrabarCopiasVersiones en VersionesRepository.cs
public async Task GrabarCopiasVersiones(DatosCargarVersionDestinoJSON json)
{
    string jsonString = JsonSerializer.Serialize(json);

    dah.GetStoredProcComando("PKG_CARGA_DATOS_VERSIONES.SET_COPIA");
    dah.Comando.CommandType = CommandType.StoredProcedure;

    // IMPORTANTE: los parámetros deben ir en el mismo orden que en el SP
    dah.AddParameter("pCod_Version_Destino", json.CodigoVersion);
    dah.AddParameter("p_jSonConf",           jsonString);
    dah.AddParameter("p_Cod_Usuario",        CodigoUsuario);

    IDbDataParameter pResultado    = dah.AddParameter("pRESULTADO",     null, DbType.Int32,  ParameterDirection.Output, 0);
    IDbDataParameter pResultadoStr = dah.AddParameter("pRESULTADO_STR", null, DbType.String, ParameterDirection.Output, 4000);

    await Task.Run(() => dah.ExecuteNonQuery(dah.Comando));

    int codigoResultado = Convert.ToInt32(pResultado.Value);
    if (codigoResultado < 0)
    {
        string mensaje = pResultadoStr.Value != null
            ? $"Error -> {Convert.ToString(pResultadoStr.Value)}"
            : "Error de BD no especificado al ejecutar 'PKG_CARGA_DATOS_VERSIONES.SET_COPIA'";

        throw new ExcepcionBaseDatos(codigoResultado, mensaje);
    }
}
```

- Código de resultado negativo = error controlado de BD (`RAISE_APPLICATION_ERROR` en PL/SQL)
- Lanzar siempre `ExcepcionBaseDatos(codigoResultado, mensaje)` — nunca ignorar el resultado

### SP que devuelve datos (cursor Oracle)

Cuando el SP devuelve filas se añade un parámetro `REF CURSOR` y se recogen los datos con `ExecuteDataSet()`:

```csharp
public async Task<List<MedioIncremento>> ObtenerImportes(ImportesParam param)
{
    var resultado  = new List<MedioIncremento>();
    var jsonString = JsonSerializer.Serialize(param);

    dah.GetStoredProcComando("PKG_CARGA_DATOS_VERSIONES.GET_IMPORTES");
    dah.Comando.CommandType = CommandType.StoredProcedure;

    dah.AddParameter("p_jSonConf",   jsonString);
    dah.AddParameter("pCOD_USUARIO", CodigoUsuario);

    // Cursor de salida (filas devueltas por el SP)
    IDbDataParameter pCursor = dah.AddParameter("p_CURSOR", null);
    ((OracleParameter)pCursor).AsignarParametroRefCursor();
    pCursor.Direction = ParameterDirection.InputOutput;
    pCursor.Size = 3000;

    IDbDataParameter pResultado    = dah.AddParameter("pRESULTADO",     null, DbType.Int32,  ParameterDirection.Output, 0);
    IDbDataParameter pResultadoStr = dah.AddParameter("pRESULTADO_STR", null, DbType.String, ParameterDirection.Output, 4000);

    await Task.Run(() =>
    {
        DataSet ds = dah.ExecuteDataSet();

        if (ds.Tables.Count > 0)
        {
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                resultado.Add(new MedioIncremento
                {
                    CodigoMedio     = Convert.ToInt32(row["COD_MEDIO"]),
                    NetoVentaOrigen = Convert.ToDecimal(row["IMP_NETO_VENTA"])
                });
            }
        }
    });

    int codigoResultado = Convert.ToInt32(pResultado.Value);
    if (codigoResultado < 0)
        throw new ExcepcionBaseDatos(codigoResultado,
            Convert.ToString(pResultadoStr.Value) ?? "Error en GET_IMPORTES");

    return resultado;
}
```

### Resumen de métodos de ejecución

| Método | Cuándo usarlo |
|---|---|
| `dah.ProcesarDatos(dr => ...)` | SQL directo que devuelve filas (usa `IDataReader`) |
| `dah.ExecuteDataSet()` | SP con cursor Oracle que devuelve filas |
| `dah.ExecuteNonQuery()` | SQL directo sin retorno de datos |
| `dah.ExecuteNonQuery(dah.Comando)` | SP sin retorno de datos |

---

## Patrón 5: COUNT / scalar

```csharp
// ✅ CORRECTO — ExistenPrevisionesEnVersion en VersionesRepository.cs
public async Task<bool> ExistenPrevisionesEnVersion(int codigoVersion)
{
    const string query = @"
        SELECT COUNT(*)
          FROM PPT_PREVISIONES
         WHERE COD_VERSION = :CodigoVersion";

    dah.GetSqlStringComando(query);
    dah.AddParameter("CodigoVersion", codigoVersion);

    await AñadirParametroMulticompania(dah);

    int cuantos = await Task.Run(() => dah.ExecuteScalar<int>());
    return cuantos > 0;
}
```

---

## Árbol de decisión

```
¿La query tiene cláusulas condicionales?
  ├── No  → const string query = @"..."                      (Patrón 1)
  └── Sí  → string query = $@"...{(cond ? "..." : "")}..."   (Patrón 2)

¿Necesitas el ID generado por Oracle?
  └── Sí → RETURNING ... INTO :Param + Output               (Patrón 3)

¿Es un procedimiento almacenado?
  └── Sí → GetStoredProcComando + verificar resultado        (Patrón 4)

¿Solo necesitas un valor escalar?
  └── Sí → ExecuteScalar<T>                                  (Patrón 5)
```

---

## Reglas de nomenclatura y estilo

| Elemento | Convención | Ejemplo |
|---|---|---|
| Nombre de parámetro Oracle | PascalCase, sin `:` | `dah.AddParameter("CodigoVersion", ...)` |
| Nombre en la query | con `:` | `WHERE COD_VERSION = :CodigoVersion` |
| Query estática | `const string query = @"..."` | Patrón 1 |
| Query dinámica (con `if`) | `string query = $@"...{(cond ? "..." : "")}..."` | Patrón 2 |
| Multicompañía | `await AñadirParametroMulticompania(dah)` | siempre en SELECT con datos del esquema |
| Propiedades de contexto | `CodigoPais`, `CodigoUsuario` | heredadas de `BasePresupuestosRepository` |

---

## Registro en el Contenedor de DI

Los repositorios se registran en `Program.cs` como **Scoped**, asociando la interfaz del Domain con la implementación de Infrastructure:

```csharp
// HM.Presupuestos.Web/Program.cs
builder.Services.AddScoped<ICondicionesRepository, CondicionesRepository>();
builder.Services.AddScoped<ISobreprimasRepository, SobreprimasRepository>();
builder.Services.AddScoped<IVersionesRepository, VersionesRepository>();
builder.Services.AddScoped<IIndicadoresRepository, IndicadoresRepository>();
builder.Services.AddScoped<IPresupuestosRepository, PresupuestosRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IConfiguracionRepository, ConfiguracionRepository>();
builder.Services.AddScoped<ILogAccionesRepository, LogAccionesRepository>();
```

La capa Web **solo conoce las interfaces** (Domain); nunca referencia directamente las clases de Infrastructure.

**Por qué Scoped y no Singleton:**
Los repositorios acceden a `IJwt` que contiene el usuario autenticado del request actual. Un Singleton compartiría estado entre usuarios. Scoped garantiza una instancia por petición HTTP.

## Estructura

```
HM.Presupuestos.Infrastructure/
└── Persistencia/
    ├── BasePresupuestosRepository.cs      ← Clase base con DAL + JWT
    ├── PresupuestosRepository.cs          ← IPresupuestosRepository (maestros, ~52 métodos)
    ├── Admin/
    │   └── AdminRepository.cs
    ├── Condiciones/
    │   └── CondicionesRepository.cs
    ├── Configuracion/
    │   └── ConfiguracionRepository.cs
    ├── LogAcciones/
    │   └── LogAccionesRepository.cs
    ├── Sobreprimas/
    │   └── SobreprimasRepository.cs
    └── Versiones/
        ├── VersionesRepository.cs
        └── IndicadoresRepository.cs
```

---

## Clase Base — `BasePresupuestosRepository`

Todos los repositorios heredan de `BasePresupuestosRepository`, que encapsula el acceso a datos (`IDataAccessHelperSecure` de HM.Core) y el usuario autenticado (`IJwt`):

```csharp
// HM.Presupuestos.Infrastructure/Persistencia/BasePresupuestosRepository.cs
public abstract class BasePresupuestosRepository(
    IDataAccessHelperSecure dah,
    IJwt jwt) : BaseDAL(dah)
{
    protected readonly IJwt Jwt = jwt;

    protected int CodigoAplicacion =>
        Jwt.Usuario?.CodigoAplicacion
        ?? throw new InvalidOperationException("No hay usuario autenticado.");

    protected int CodigoUsuario =>
        Jwt.Usuario?.CodigoUsuario
        ?? throw new InvalidOperationException("No hay usuario autenticado.");

    protected int CodigoPais =>
        Jwt.Usuario?.CodigoPais
        ?? throw new InvalidOperationException("No hay usuario autenticado.");

    protected async Task AñadirParametroMulticompania(IDataAccessHelperBase dahHelper) { ... }
}
```

### Propiedades Calculadas vs Campos

`CodigoUsuario`, `CodigoPais` y `CodigoAplicacion` son **propiedades calculadas** (no campos `readonly` asignados en el constructor) porque en Blazor Server el usuario se autentica **después** de que el DI construye los servicios. Un campo asignado en el constructor siempre obtendría `null`:

```csharp
// ❌ INCORRECTO — se evalúa en el constructor (antes de autenticación en Blazor Server)
public class MiRepository(IDataAccessHelperSecure dah, IJwt jwt) : BaseDAL(dah)
{
    private readonly int _codigoUsuario = jwt.Usuario?.CodigoUsuario ?? 0;  // siempre 0
}

// ✅ CORRECTO — se evalúa al acceder (cuando el usuario ya está autenticado)
protected int CodigoUsuario =>
    Jwt.Usuario?.CodigoUsuario
    ?? throw new InvalidOperationException("No hay usuario autenticado.");
```

---

## Implementación de un Repositorio

Cada repositorio usa primary constructor (C# 12), pasa `dah` y `jwt` a la base, e implementa la interfaz de dominio correspondiente:

```csharp
// HM.Presupuestos.Infrastructure/Persistencia/Condiciones/CondicionesRepository.cs
public class CondicionesRepository(
    IDataAccessHelperSecure dah,
    IJwt jwt) : BasePresupuestosRepository(dah, jwt), ICondicionesRepository
{
    public async Task<List<Vigencia>> ObtenerVigencias(CondicionFiltro filtro)
    {
        var resultado = new List<Vigencia>();

        const string query = @"
            SELECT COD_CONDICION_VIGENCIA, MES_DESDE, MES_HASTA
              FROM PPT_CONDICION_VIGENCIA
             WHERE COD_VERSION = :CodigoVersion
               AND COD_NETWORK = :CodigoNetwork
          ORDER BY COD_CONDICION_VIGENCIA";

        dah.GetSqlStringComando(query);
        dah.Comando.Parameters.Clear();

        dah.AddParameter("CodigoVersion", filtro.CodigoVersion);
        dah.AddParameter("CodigoNetwork", filtro.CodigoNetwork);

        await AñadirParametroMulticompania(dah);

        await Task.Run(() =>
        {
            dah.ProcesarDatos(dr =>
            {
                while (dr.Read())
                {
                    resultado.Add(new Vigencia
                    {
                        Codigo   = dr.GetInt32("COD_CONDICION_VIGENCIA"),
                        MesDesde = dr.GetInt32("MES_DESDE"),
                        MesHasta = dr.GetInt32("MES_HASTA")
                    });
                }
            });
        });

        return resultado;
    }
}
```

**Patrones que se repiten en todos los repositorios:**
- `dah.GetSqlStringComando(query)` — prepara el comando SQL directo
- `dah.AddParameter(nombre, valor)` — parámetros con nombre (sintaxis Oracle `:Nombre`)
- `await AñadirParametroMulticompania(dah)` — añade filtro de compañía (en métodos que lo requieren)
- `await Task.Run(() => dah.ProcesarDatos(...))` — ejecuta en thread pool (DAL síncrona envuelta en async)
- `dr.GetInt32(columna)` / `dr.GetString(columna)` — lectura tipada de `IDataReader`

---

## Llamada a Procedimiento Almacenado (Stored Procedure)

Cuando la operación requiere lógica en BD se usa `GetStoredProcComando()` en lugar de `GetSqlStringComando()`. El SP devuelve un código de resultado por parámetro `OUTPUT` que indica éxito o error.

### SP que solo ejecuta (INSERT / UPDATE / DELETE)

```csharp
public async Task CopiarVersion(CopiaVersionParam param)
{
    var jsonString = JsonSerializer.Serialize(param);

    dah.GetStoredProcComando("PKG_CARGA_DATOS_VERSIONES.SET_COPIA");
    dah.Comando.CommandType = CommandType.StoredProcedure;

    // IMPORTANTE: los parámetros deben ir en el mismo orden que en el SP
    dah.AddParameter("pCod_Version_Destino", param.CodigoVersion);
    dah.AddParameter("p_jSonConf",           jsonString);
    dah.AddParameter("p_Cod_Usuario",        CodigoUsuario);

    // Parámetros de salida para resultado y mensaje de error
    IDbDataParameter pResultado    = dah.AddParameter("pRESULTADO",     null, DbType.Int32,  ParameterDirection.Output, 0);
    IDbDataParameter pResultadoStr = dah.AddParameter("pRESULTADO_STR", null, DbType.String, ParameterDirection.Output, 4000);

    await Task.Run(() => dah.ExecuteNonQuery(dah.Comando));

    int codigoResultado = Convert.ToInt32(pResultado.Value);
    if (codigoResultado < 0)
    {
        string mensaje = pResultadoStr.Value != null
            ? $"Error -> {Convert.ToString(pResultadoStr.Value)}"
            : "Error de BD no especificado al ejecutar 'PKG_CARGA_DATOS_VERSIONES.SET_COPIA'";

        throw new ExcepcionBaseDatos(codigoResultado, mensaje);
    }
}
```

### SP que devuelve datos (cursor Oracle)

Cuando el SP devuelve filas se añade un parámetro `REF CURSOR` y se recogen los datos con `ExecuteDataSet()`:

```csharp
public async Task<List<MedioIncremento>> ObtenerImportes(ImportesParam param)
{
    var resultado   = new List<MedioIncremento>();
    var jsonString  = JsonSerializer.Serialize(param);

    dah.GetStoredProcComando("PKG_CARGA_DATOS_VERSIONES.GET_IMPORTES");
    dah.Comando.CommandType = CommandType.StoredProcedure;

    dah.AddParameter("p_jSonConf",   jsonString);
    dah.AddParameter("pCOD_USUARIO", CodigoUsuario);

    // Cursor de salida (filas devueltas por el SP)
    IDbDataParameter pCursor = dah.AddParameter("p_CURSOR", null);
    ((OracleParameter)pCursor).AsignarParametroRefCursor();
    pCursor.Direction = ParameterDirection.InputOutput;
    pCursor.Size = 3000;

    IDbDataParameter pResultado    = dah.AddParameter("pRESULTADO",     null, DbType.Int32,  ParameterDirection.Output, 0);
    IDbDataParameter pResultadoStr = dah.AddParameter("pRESULTADO_STR", null, DbType.String, ParameterDirection.Output, 4000);

    await Task.Run(() =>
    {
        DataSet ds = dah.ExecuteDataSet();

        if (ds.Tables.Count > 0)
        {
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                resultado.Add(new MedioIncremento
                {
                    CodigoMedio      = Convert.ToInt32(row["COD_MEDIO"]),
                    NetoVentaOrigen  = Convert.ToDecimal(row["IMP_NETO_VENTA"])
                });
            }
        }
    });

    int codigoResultado = Convert.ToInt32(pResultado.Value);
    if (codigoResultado < 0)
        throw new ExcepcionBaseDatos(codigoResultado,
            Convert.ToString(pResultadoStr.Value) ?? "Error en GET_IMPORTES");

    return resultado;
}
```

### Resumen de métodos de ejecución

| Método | Cuándo usarlo |
|---|---|
| `dah.ProcesarDatos(dr => ...)` | SQL directo que devuelve filas (usa `IDataReader`) |
| `dah.ExecuteDataSet()` | SP con cursor Oracle que devuelve filas |
| `dah.ExecuteNonQuery()` | SQL directo sin retorno de datos |
| `dah.ExecuteNonQuery(dah.Comando)` | SP sin retorno de datos |

---

## Registro en el Contenedor de DI

Los repositorios se registran en `Program.cs` como **Scoped**, asociando la interfaz del Domain con la implementación de Infrastructure:

```csharp
// HM.Presupuestos.Web/Program.cs
builder.Services.AddScoped<ICondicionesRepository, CondicionesRepository>();
builder.Services.AddScoped<ISobreprimasRepository, SobreprimasRepository>();
builder.Services.AddScoped<IVersionesRepository, VersionesRepository>();
builder.Services.AddScoped<IIndicadoresRepository, IndicadoresRepository>();
builder.Services.AddScoped<IPresupuestosRepository, PresupuestosRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IConfiguracionRepository, ConfiguracionRepository>();
builder.Services.AddScoped<ILogAccionesRepository, LogAccionesRepository>();
```

La capa Web **solo conoce las interfaces** (Domain); nunca referencia directamente las clases de Infrastructure.

**Por qué Scoped y no Singleton:**
Los repositorios acceden a `IJwt` que contiene el usuario autenticado del request actual. Un Singleton compartiría estado entre usuarios. Scoped garantiza una instancia por petición HTTP.
