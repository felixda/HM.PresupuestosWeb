# Implementación de Repositorios (Infrastructure)

Las implementaciones concretas de los puertos de repositorio viven en `HM.Presupuestos.Infrastructure/Persistencia/`. Implementan las interfaces definidas en `HM.Presupuestos.Domain/Puertos/Repositorios/`.

Ver las interfaces correspondientes en: [domain/repositories.md](../../domain/repositories.md)

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
