# Estándares de Diseño de Funciones / Métodos

## Reglas

1. **Tamaño contenido y responsabilidad única (SRP)**. Cada método debe "hacer exactamente lo que indica su nombre". Métodos de 10-15 líneas son una métrica para detectar si tienen más de una responsabilidad; si solo tienen una responsabilidad, el tamaño no importa. Los métodos complejos de dominio (transacciones, reglas de negocio) pueden ser más largos si encapsulan una única operación indivisible

2. **Nomenclatura impecable**. Los nombres de método son **verbos en español** que describen con precisión la acción. Los métodos async siempre llevan el sufijo `Async`

3. **Signatura mínima necesaria**. Reducir la aridad (0-3 parámetros es ideal). Si se supera, agrupar en una clase de filtro/parámetro con nombre propio (`CondicionFiltro`, `SobreprimaFiltro`, `ParamImportacionSobreprimas`)

4. **Evitar parámetros booleanos de configuración**. Ningún flag `bool` que bifurque el comportamiento. Preferir parámetros opcionales con `int?` o `string?` para variantes; o funciones específicas cuando el comportamiento diverge completamente

5. **Parámetros opcionales con moderación**. Máximo uno o dos y solo con `= null` (tipo nullable); si es evitable, mejor. No usar `= false` ni `= true` como valor por defecto

6. **Flujo de control simple**. Usar guard clauses para casos límite y salir temprano; reducir la indentación y la complejidad ciclomática. Usar `ArgumentNullException.ThrowIfNull()` para validar inyecciones

7. **Condiciones legibles**. Abstraer expresiones booleanas combinadas en variables o métodos explicativos. Priorizar condiciones afirmativas. Evitar `else` innecesarios

8. **Separar flujo de control y lógica de negocio**. Extraer iteraciones y ramas complejas; delegar cálculos/mutaciones a funciones privadas dedicadas (`TratarCondicion`, `TratarExcepcion`)

9. **Estilo declarativo cuando mejora la legibilidad**. Usar LINQ (`.Where()`, `.Select()`, `.Any()`, `.FirstOrDefault()`, `.Find()`, `OrderByDescending()`) con criterio; no es un dogma. Preferir expresiones de colección `[.. lista.Where(...)]` para materializaciones simples

10. **Aplicar CQS (Command-Query Separation)**. Los comandos mutan estado y retornan `void` / `Task`. Las consultas retornan un valor y no mutan estado. Excepción justificada: insertar y retornar el ID generado

11. **Constantes cerca del uso**. Las constantes de uso único deben definirse como `const` local o `private const` de clase cerca del método que las usa. Excepción válida: constantes con **significado de negocio compartidas entre múltiples capas** (p.ej. `Constantes.CodigosIndicadores`, `Constantes.BitAndVersion`) se centralizan en `HM.Presupuestos.Domain/Compartido/Constantes.cs` — su centralización es precisamente lo que garantiza consistencia entre capas

12. **No comentarios en métodos** (salvo en aquellos metodos que por su complejidad lo requieras y `/// <summary>` en métodos públicos de interfaz). Los `#region` se usan para agrupar bloques lógicos dentro de páginas y servicios

13. **Sin null en retornos de listas**. Devolver siempre `List<T>` vacía, nunca `null`. Para valores opcionales, usar `T?`

14. **Preferir funciones puras cuando sea posible**. Evitar efectos secundarios en métodos de consulta

## Verbos de Nomenclatura

| Verbo | Uso |
|---|---|
| `Obtener*` | Consulta / recuperación de datos |
| `Insertar*` | Alta de un único registro |
| `Actualizar*` | Modificación de un único registro |
| `Eliminar*` | Baja de un registro |
| `Grabar*` | Persistencia múltiple o compleja (insertar + actualizar) |
| `Validar*` | Validación sin persistencia, retorna `bool` |
| `Existen*` / `Existe*` | Comprobación de existencia, retorna `bool` |
| `Calcular*` | Cálculo puro, no persiste |
| `Inicializar*` | Configuración inicial de estado |
| `Tratar*` | Operación interna privada (procesamiento auxiliar) |

## Ejemplos

### Nomenclatura y sufijo Async

```csharp
// ✅ CORRECTO
public async Task<List<Vigencia>> ObtenerVigencias(CondicionFiltro filtro) { ... }
public async Task InsertarVigencia(Vigencia vigencia) { ... }
public async Task ActualizarVigencia(Vigencia vigencia) { ... }
public async Task EliminarVigencia(Vigencia vigencia) { ... }
public async Task GrabarCondicionesExcepciones(...) { ... }
public async Task<bool> ValidarSolapesVigencia(Vigencia vigencia) { ... }
public async Task<bool> ExistenCondicionesVigencias(int codigoVigencia) { ... }

// ❌ INCORRECTO
public async Task GetVigencias(CondicionFiltro filtro) { ... }  // inglés
public async Task Save(Vigencia vigencia) { ... }               // sin sufijo Async, verbo genérico
public async Task ProcessCondicion(Condicion c) { ... }         // verbo ambiguo
```

---

### Guard Clauses y Early Return

```csharp
// ✅ CORRECTO — guards al inicio, lógica principal al final
private async void CheckboxIndicadorCheckedChanged(bool? newValue, int? indicadorCodigo)
{
    if (newValue is null || indicadorCodigo is null) return;

    var itemVersion = _listVersion.Find(x => x.Codigo == versionCodigo);
    if (itemVersion == null) return;

    var itemIndicador = itemVersion.IndicadorList.Find(x => x.Codigo == indicadorCodigo);
    if (itemIndicador == null) return;

    // Lógica principal con profundidad 1
    itemIndicador.Estado = (bool)newValue;
}

// ✅ CORRECTO — colección vacía como guard para lista
public static IEnumerable<int> GetCodigosSeleccionados(object? value)
{
    var result = new List<int>();
    if (value == null) return result;   // Early return con vacío, nunca null

    // ... procesamiento
    return result;
}

// ❌ INCORRECTO — lógica principal anidada en else
private void ProcesarDatos(Data? data)
{
    if (data != null)
    {
        if (data.Items != null)
        {
            // lógica real aquí — demasiada indentación
        }
    }
}
```

---

### CQS — Separación Consulta / Comando

```csharp
// ✅ CORRECTO — interfaz con separación clara
public interface ISobreprimasService
{
    // QUERIES — solo leen, nunca mutan
    Task<List<Sobreprima>> ObtenerSobreprimas(SobreprimaFiltro filtro);
    Task<bool> ExistenSobreprimas(SobreprimaFiltro filtro, SobreprimaGridModel? sobreprima = null);

    // COMMANDS — mutan estado, retornan void/Task
    Task GrabarSobreprima(SobreprimaGridModel item);
    Task EliminarSobreprimas(SobreprimaGridModel sobreprima);
    Task ImportarSobreprimas(ParamImportacionSobreprimas param);
}

// ✅ CORRECTO — query puro sin efectos secundarios
public async Task<List<Sobreprima>> ObtenerSobreprimas(SobreprimaFiltro filtro)
{
    _logger.LogTrace("Llamando método ObtenerSobreprimas");
    return await _sobreprimasRepository.ObtenerSobreprimas(filtro);
}

// ❌ INCORRECTO — query que muta estado
public async Task<List<Sobreprima>> ObtenerSobreprimas(SobreprimaFiltro filtro)
{
    var resultado = await _sobreprimasRepository.ObtenerSobreprimas(filtro);
    _ultimaConsulta = filtro;    // efecto secundario — viola CQS
    return resultado;
}
```

---

### Parámetros Opcionales — `int?` en lugar de `bool`

```csharp
// ✅ CORRECTO — parámetros nullable en lugar de flags booleanos
public async Task<List<Version>> ObtenerVersiones(int anio, int? estadoIncluido = null)
{
    return await _versionesRepository.ObtenerVersiones(anio, estadoIncluido);
}

public async Task<List<VersionResumen>> ObtenerVersionesResumen(
    int? anio = null,
    int? estadoIncluido = null,
    int? estadoExcluido = null)
{
    return await _versionesRepository.ObtenerVersionesResumen(anio, estadoIncluido, estadoExcluido);
}

// ⚠️ EVITAR — bool que bifurca comportamiento
public async Task<List<CodigoDescripcion>> ObtenerAniosConVersiones(bool incluirAnios = false)
{
    // bool obliga a leer el cuerpo para entender qué hace
}
```

---

### Clase de Filtro para Reducir Aridad

```csharp
// ❌ INCORRECTO — 4+ parámetros individuales
Task<List<Vigencia>> ObtenerVigencias(int codigoVersion, int codigoNetwork, int codigoGrupo, int indicadorAcuerdo);

// ✅ CORRECTO — clase de filtro con nombre descriptivo
Task<List<Vigencia>> ObtenerVigencias(CondicionFiltro filtro);

// Clase de filtro en Domain:
public class CondicionFiltro
{
    public int CodigoVersion { get; set; }
    public int CodigoNetwork { get; set; }
    public int CodigoGrupoCliente { get; set; }
    public int IndicadorAcuerdo { get; set; }
}

// Uso en el llamador:
var filtro = new CondicionFiltro
{
    CodigoVersion = vigencia.CodigoVersion,
    CodigoGrupoCliente = vigencia.CodigoGrupoCliente,
    CodigoNetwork = vigencia.CodigoNetWork,
    IndicadorAcuerdo = vigencia.IndicadorAcuerdo
};
List<Vigencia> vigencias = await _condicionesService.ObtenerVigencias(filtro);
```

---

### LINQ — Estilo Declarativo

```csharp
// ✅ CORRECTO — .Where() + .Select() en cadena fluida
var codigosExcluir = new[]
{
    sobreprima.ConceptoDefaul.Codigo,
    sobreprima.ConceptoHVP.Codigo,
    sobreprima.ConceptoSLA.Codigo
}
.Where(codigo => codigo > 0)
.Select(codigo => codigo.ToString());

if (!codigosExcluir.Any()) return false;

// ✅ CORRECTO — expresión de colección con LINQ
return [.. resultado.OrderByDescending(x => x.Codigo)];

// ✅ CORRECTO — .Any() con predicado de solapamiento
var seSolapa = vigencias.Any(r =>
    vigencia.MesDesde <= r.MesHasta &&
    r.MesDesde <= vigencia.MesHasta);

// ✅ CORRECTO — .FirstOrDefault() con null-coalescing
var codigoCondicion = excepcionDto.CodigosConceptosCondiciones
    .FirstOrDefault(c => c.CodigoConcepto == (int)concepto)?.CodigoCondicion ?? 0;

// ✅ CORRECTO — .Find() en List<T> (equivalente a FirstOrDefault pero más eficiente)
Indicador? indicador = resultado.Find(c => c.Codigo == codigo);

// ❌ INCORRECTO — bucle imperativo cuando LINQ es más claro
var resultado = new List<int>();
for (int i = 0; i < codigos.Count; i++)
{
    if (codigos[i] > 0) resultado.Add(codigos[i].ToString());
}
```

---

### Constantes Cerca del Uso

```csharp
// ✅ CORRECTO — const local en el método que la usa (SQL solo usado aquí)
public async Task<List<ConceptoCondicion>> ObtenerConceptos()
{
    const string query = @"
        SELECT COD_CONCEPTO_CONDICION, DES_CONCEPTO_CONDICION
          FROM PPT_CONCEPTOS_CONDICIONES
         WHERE F_BAJA IS NULL
      ORDER BY COD_CONCEPTO_CONDICION";

    dah.GetSqlStringComando(query);
    // ...
}

// ✅ CORRECTO — private const de clase cuando solo la usa esa clase
private const string IDIOMA_COOKIE_KEY = "app_idioma";
private const int IDIOMA_COOKIE_EXPIRE_DAYS = 365;
private const string IDIOMA_POR_DEFECTO = "es";

// ❌ INCORRECTO — constante de uso único sacada a archivo global sin razón
// AppConstants.cs
public static class AppConstants
{
    public const string IDIOMA_COOKIE_KEY = "app_idioma";  // solo la usa Init.razor.cs
}
```

**Excepción — Constantes de dominio compartidas entre capas:**

```csharp
// ✅ CORRECTO — Constantes.cs en Domain, usadas por Web + Application + Infrastructure
// HM.Presupuestos.Domain/Compartido/Constantes.cs
public static class Constantes
{
    public static class CodigosIndicadores
    {
        public const int CERRADA = 3;   // mismo valor que debe coincidir en BD
        public const int REAL = 43;     // usado en Web, Application e Infrastructure
    }

    public static class BitAndVersion
    {
        public const int PUBLICADA = 8;
        public const int CERRADA = 1;
        // ...
    }
}

// Uso correcto desde cualquier capa:
if (itemIndicador.Codigo == Constantes.CodigosIndicadores.REAL) { ... }
```

La regla se resume en: **una constante debe vivir en el nivel más bajo en el que tiene sentido compartirla**. Si solo la usa un método → local. Si solo la usa una clase → `private const`. Si la usan múltiples capas con significado de negocio → `Constantes.cs` en Domain.

---

### Funciones Privadas Auxiliares para SRP

Cuando un método público crece, extraer lógica interna a métodos privados con nombre descriptivo:

```csharp
// ✅ CORRECTO — método público orquesta; privados hacen el trabajo concreto
public async Task GrabarCondicionesExcepciones(
    Dictionary<CondicionDto, DatosCondicionCambiados> condicionesNoGuardadas,
    Dictionary<ExcepcionDto, DatosExcepcionesCondicionCambiados> excepcionesNoGuardadas,
    int codigoVigencia)
{
    List<ConceptoCondicion> conceptos = await _condicionesRepository.ObtenerConceptos();

    using var transaction = _condicionesRepository.ObtenerTransaccion();
    try
    {
        foreach (var (condicionDto, _) in condicionesNoGuardadas)
        {
            await TratarCondicion(condicionBase);  // Extrae complejidad a privado
        }

        foreach (var (excepcionDto, _) in excepcionesNoGuardadas)
        {
            await TratarExcepcion(excepcionBD, excepcionDto, ...);  // Extrae complejidad a privado
        }

        await transaction.CommitAsync();
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}

// Métodos privados con responsabilidad única
private async Task TratarCondicion(Condicion condicion) { ... }
private async Task TratarExcepcion(ExcepcionBD excepcionBD, ExcepcionDto excepcionDto, ...) { ... }
```

---

### Retorno de Listas — Nunca `null`

```csharp
// ✅ CORRECTO — lista vacía, nunca null
public async Task<List<Vigencia>> ObtenerVigencias(CondicionFiltro filtro)
{
    var resultado = new List<Vigencia>();
    // ... si no hay datos, resultado queda vacío
    return resultado;
}

// ✅ CORRECTO — valor escalar opcional con T?
Indicador? indicador = resultado.Find(c => c.Codigo == codigo);  // puede ser null, documentado con ?

// ❌ INCORRECTO — retornar null para lista
public async Task<List<Version>?> ObtenerVersiones(int anio)
{
    if (sinResultados) return null;   // fuerza null checks en todos los llamadores
}
```
