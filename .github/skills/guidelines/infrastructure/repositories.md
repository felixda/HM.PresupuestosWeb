# Repositorios de Infrastructure — Patrón de Acceso a Datos

Los repositorios de `HM.Presupuestos.Infrastructure` usan `IDataAccessHelperSecure` (`dah`) del framework HM.Core para ejecutar queries Oracle. Este documento describe el patrón estándar que deben seguir todos los métodos de persistencia.

> **Referencia**: `VersionesRepository.cs` es el repositorio de referencia para los patrones de este documento.

---

## Estructura general de un repositorio

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
- Los campos del primary constructor se almacenan con prefijo `_` (o sin él si son `protected` heredados)
- `ITransaccion ObtenerTransaccion()` se sobreescribe con `TransaccionWrapper` cuando el repositorio participa en transacciones

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
                    Codigo = dr.GetInt32("COD_ESTADO_VERSION"),
                    Descripcion = dr.GetNullableString("DES_ESTADO_VERSION"),
                    BitAnd = dr.GetInt32("BITAND"),
                    Indice = indice++,
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
                    Codigo = dr.GetInt32("COD_VERSION"),
                    Descripcion = dr.GetString("DES_VERSION"),
                    Anio = anio,
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
               {(anio.HasValue ? "AND ANIO = :Anio" : "")}
               {(estadoIncluido.HasValue ? "AND BITAND(IND_ESTADO_VERSION, :IndEstado) = :IndEstado" : "")}
               {(estadoExcluido.HasValue ? "AND BITAND(IND_ESTADO_VERSION, :IndEstadoQuitar) != :IndEstadoQuitar" : "")}
         ORDER BY DES_VERSION";

    dah.GetSqlStringComando(query);

    if (anio.HasValue)
        dah.AddParameter("Anio", anio.Value);

    if (estadoIncluido.HasValue)
        dah.AddParameter("IndEstado", estadoIncluido.Value);

    if (estadoExcluido.HasValue)
        dah.AddParameter("IndEstadoQuitar", estadoExcluido.Value);

    await AñadirParametroMulticompania(dah);

    await Task.Run(() =>
    {
        dah.ProcesarDatos(dr =>
        {
            while (dr.Read())
            {
                resultado.Add(new VersionResumen
                {
                    Codigo = dr.GetInt32("COD_VERSION"),
                    Descripcion = dr.GetString("DES_VERSION"),
                });
            }
        });
    });

    return resultado;
}
```

### Antipatrones a evitar con `if`

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
    dah.AddParameter("Anio", version.Anio);
    dah.AddParameter("Mes", version.Mes);
    dah.AddParameter("Estado", version.IndEstado);
    dah.AddParameter("Orden", version.Orden);
    dah.AddParameter("Tipo", version.CodigoTipo);

    // Parámetro de salida — siempre al final
    dah.AddParameter("CodigoVersion", version.Codigo, DbType.Int32, ParameterDirection.Output, 10);

    await Task.Run(() => dah.ExecuteNonQuery());

    // Leer el valor devuelto y asignarlo a la entidad
    version.Codigo = Convert.ToInt32(dah.Comando.Parameters["CodigoVersion"].Value);
    return version.Codigo;
}
```

---

## Patrón 4: Stored Procedure

```csharp
// ✅ CORRECTO — GrabarCopiasVersiones en VersionesRepository.cs
public async Task GrabarCopiasVersiones(DatosCargarVersionDestinoJSON json)
{
    string jsonString = JsonSerializer.Serialize(json);

    dah.GetStoredProcComando("PKG_CARGA_DATOS_VERSIONES.SET_COPIA");
    dah.Comando.CommandType = CommandType.StoredProcedure;

    // IMPORTANTE: los parámetros deben ir en el mismo orden que en el procedimiento almacenado
    dah.AddParameter("pCod_Version_Destino", json.CodigoVersion);
    dah.AddParameter("p_jSonConf", jsonString);
    dah.AddParameter("p_Cod_Usuario", json.CodigoUsuario);

    IDbDataParameter resultadoInt = dah.AddParameter("pRESULTADO", null, DbType.Int32, ParameterDirection.Output, 0);
    IDbDataParameter resultadoStr = dah.AddParameter("pRESULTADO_STR", null, DbType.String, ParameterDirection.Output, 4000);

    await Task.Run(() => dah.ExecuteNonQuery(dah.Comando));

    int codigoResultado = Convert.ToInt32(resultadoInt.Value);
    if (codigoResultado < 0)
    {
        string mensajeResultado = resultadoStr.Value != null
            ? $"Error -> {Convert.ToString(resultadoStr.Value)}"
            : "Error de BD no especificado al ejecutar 'PKG_CARGA_DATOS_VERSIONES.SET_COPIA'";

        throw new ExcepcionBaseDatos(codigoResultado, mensajeResultado);
    }
}
```

- Código de resultado negativo = error controlado de BD (`RAISE_APPLICATION_ERROR` en PL/SQL)
- Lanzar siempre `ExcepcionBaseDatos(codigoResultado, mensajeResultado)` — nunca ignorar el resultado

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
