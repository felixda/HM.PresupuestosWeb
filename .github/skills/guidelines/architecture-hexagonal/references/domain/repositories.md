# Repositorios de Dominio

Los repositorios son **puertos de salida** del dominio. Las interfaces se definen en `HM.Presupuestos.Domain/Puertos/Repositorios/` y las implementaciones reales en `HM.Presupuestos.Infrastructure/Persistencia/`.

## Estructura

```
HM.Presupuestos.Domain/
└── Puertos/
    ├── Repositorios/
    │   ├── IAdminRepository.cs
    │   ├── ICondicionesRepository.cs
    │   ├── IConfiguracionRepository.cs
    │   ├── IIndicadoresRepository.cs
    │   ├── ILogAccionesRepository.cs
    │   ├── IPresupuestosRepository.cs     # Maestros (52 métodos)
    │   ├── ISobreprimasRepository.cs
    │   └── IVersionesRepository.cs
    └── Servicios/
        ├── ITransaccion.cs                # Puerto de transacción
        └── ICoreLoggerService.cs

HM.Presupuestos.Infrastructure/
└── Persistencia/
    ├── BasePresupuestosRepository.cs      # Clase base con DAL + JWT
    ├── PresupuestosRepository.cs          # Implementa IPresupuestosRepository
    ├── Admin/
    ├── Condiciones/
    │   └── CondicionesRepository.cs
    ├── Configuracion/
    ├── LogAcciones/
    ├── Sobreprimas/
    └── Versiones/
```

---

## Interfaz de Repositorio

La interfaz vive en Domain y solo usa tipos del dominio (entidades, DTOs, filtros, enumerados). No revela ningún detalle de infraestructura.

```csharp
// HM.Presupuestos.Domain/Puertos/Repositorios/ISobreprimasRepository.cs
public interface ISobreprimasRepository
{
    Task<List<Sobreprima>> ObtenerSobreprimas(SobreprimaFiltro filterSobreprima);
    Task InsertSobreprima(Sobreprima item);
    Task EliminarSobreprima(int codigoSobreprima);
    Task ActualizarSobreprima(Sobreprima item);
    Task<bool> ExistenSobreprimas(SobreprimaFiltro filterSobreprima, string? codigosSobreprima = null);

    ITransaccion ObtenerTransaccion();  // Puerto de transacción
}
```

```csharp
// HM.Presupuestos.Domain/Puertos/Repositorios/ICondicionesRepository.cs
public interface ICondicionesRepository
{
    Task<List<Vigencia>> ObtenerVigencias(CondicionFiltro filtro);
    Task InsertarVigencia(Vigencia item);
    Task ActualizarVigencia(Vigencia item);
    Task EliminarVigencia(int codigoVigencia);
    Task<bool> ExistenCondicionesVigencias(int codigoVigencia);

    Task<List<CondicionDto>> ObtenerCondicionesPorVigencia(int codigoVigencia);
    Task GrabarCondicion(Condicion medioCondicion);
    Task EliminarCondicion(Condicion condicion);

    Task<List<ExcepcionDto>> ObtenerExcepcionesCondiciones(int codigoVigencia);
    Task GrabarConceptoNMD(int codigoCondicionMedio, ConceptosCondicionesNMD codigoConceptoNMD, int codigo);
    Task EliminarConceptoNMDExcepcionCondicion(int codigoCondicionMedio, ConceptosCondicionesNMD concepto);
    Task EliminarExcepcionCondicion(int codigoCondicionMedio);

    ITransaccion ObtenerTransaccion();
}
```

---

## Características de las Interfaces

- **Lenguaje de dominio**: verbos del negocio (`Obtener`, `Insertar`, `Actualizar`, `Eliminar`, `Grabar`, `Importar`)
- **Tipos de dominio**: entidades (`Sobreprima`, `Vigencia`), DTOs de Domain (`CondicionDto`), filtros (`SobreprimaFiltro`), enumerados (`ConceptosCondicionesNMD`)
- **Sin detalles de BD**: no hay SQL, nombres de tabla ni tipos de ADO.NET en la interfaz
- **Sin `null` en retornos de lista**: devuelven `List<T>` vacía, nunca `null`
- **`Task`** para operaciones de escritura, **`Task<T>`** para lecturas

```csharp
// ✅ CORRECTO — lenguaje de dominio, tipos de dominio
public interface IVersionesRepository
{
    Task<List<Version>> ObtenerVersiones(int anio, int? estadoIncluido = null);
    Task<int> InsertarVersion(int codigoPais, Version version);   // devuelve el código generado
    Task ActualizarVersion(Version version);
    Task EliminarVersion(int codigoVersion);
    Task<bool> ExistenPrevisionesEnVersion(int codigoVersion);
    ITransaccion ObtenerTransaccion();
}

// ❌ INCORRECTO — expone detalles de infraestructura
public interface IVersionesRepository
{
    Task<DataTable> GetVersiones(string sqlFilter);         // tipo de BD
    Task<int> ExecuteNonQuery(string proc, object[] args);  // detalle de ADO.NET
}
```

---

## Puerto de Transacción (`ITransaccion`)

La interfaz `ITransaccion` permite que los servicios de Application controlen transacciones sin conocer la implementación:

```csharp
// HM.Presupuestos.Domain/Puertos/Servicios/ITransaccion.cs
public interface ITransaccion : IDisposable
{
    Task CommitAsync();
    Task RollbackAsync();
}
```

Se obtiene desde el repositorio y se pasa como parámetro a las operaciones que deben incluirse en la misma transacción:

```csharp
// Uso en un servicio de Application
public async Task GrabarCondicionesExcepciones(...)
{
    using var transaction = condicionesRepository.ObtenerTransaccion();
    try
    {
        await condicionesRepository.GrabarCondicion(condicion, transaction);
        await condicionesRepository.GrabarConceptoNMD(codigoCondicion, concepto, transaction);
        await transaction.CommitAsync();
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}
```

---

## Implementación en Infrastructure

Ver [infrastructure/repositories.md](../../infrastructure/repositories.md) para: clase base `BasePresupuestosRepository`, patrón de implementación con `IDataAccessHelperSecure`, propiedades calculadas del usuario y registro en el contenedor de DI.

---

## Repositorio de Maestros (`IPresupuestosRepository`)

El repositorio de maestros agrupa ~52 métodos de consulta de datos de referencia (medios, networks, países, tipologías, etc.). Todos son de solo lectura:

```csharp
public interface IPresupuestosRepository
{
    // Medios y networks
    Task<List<CodigoDescripcion>> ObtenerMedios();
    Task<List<CodigoDescripcion>> ObtenerMediosPorNetWork(string codigoNetwork);
    Task<List<CodigoDescripcion>> ObtenerNetworks();

    // Países y estructuras
    Task<List<CodigoDescripcion>> ObtenerPaises();
    Task<List<CodigoDescripcion>> ObtenerTipologias();
    Task<List<CodigoDescripcion>> ObtenerAlcances();

    // Conceptos NMD y condiciones
    Task<List<CodigoDescripcion>> ObtenerConceptosNMD(int codigoVigencia, ConceptosTipoNMD tipo, ConceptosTipoNMDFiltro filtro);

    // Editoriales y agrupaciones
    Task<List<EditorialConAgrupacionComercial>> ObtenerEditoriales(FiltroEditoriales filtro);

    // ... ~52 métodos en total
}
```

---

## Cuándo Devolver Entidad vs DTO desde el Repositorio

| Devolver **Entidad** cuando... | Devolver **DTO** cuando... |
|---|---|
| El dato se va a modificar y guardar | El dato es solo para mostrar en grid/formulario |
| El servicio necesita aplicar lógica de negocio | Se devuelven datos desnormalizados (con descripciones) |
| Se persiste directamente lo que devuelve el repo | Se necesitan propiedades calculadas o claves compuestas |

```csharp
// Devuelve entidad — se va a persistir
Task<List<Version>> ObtenerVersiones(int anio, int? estadoIncluido = null);
Task<List<Vigencia>> ObtenerVigencias(CondicionFiltro filtro);

// Devuelve DTO — es para presentación en el grid
Task<List<CondicionDto>> ObtenerCondicionesPorVigencia(int codigoVigencia);
Task<List<ExcepcionDto>> ObtenerExcepcionesCondiciones(int codigoVigencia);
Task<List<VersionResumen>> ObtenerVersionesResumen(int? anio = null, int? estadoIncluido = null, int? estadoExcluido = null);
```


