# Inside-Out TDD

## Qué es Inside-Out

Inside-Out (también llamado TDD Clásico o escuela Chicago/Detroit) construye el software **desde las capas internas hacia las externas**. Se empieza por las unidades más pequeñas e independientes — entidades de dominio — y se avanza hacia afuera — servicios, repositorios, UI — usando implementaciones reales en cada capa en lugar de mocks cuando es posible.

```
Domain (entidades, reglas)
    ↓  se testea primero — sin dependencias
Application (servicios, casos de uso)
    ↓  se testea con Moq en los repositorios
Infrastructure (repositorios Oracle)
    ↓  se testea con Oracle real (integración)
Web (Blazor)
    ↓  se testea con Playwright (E2E)
```

---

## Orden de Implementación

### 1. Entidades de Dominio (sin dependencias)

Empezar por la entidad más central del módulo. Las entidades no tienen dependencias externas, por lo que los tests son puramente unitarios:

```csharp
// Test primero
[Test]
public void Version_ConAnioInvalido_LanzaValidacionException()
{
    // Arrange & Act & Assert
    Assert.Throws<ValidacionException>(() =>
        new Version { Anio = 1900 });
}

// Implementación después
public class Version
{
    private int _anio;
    public int Anio
    {
        get => _anio;
        set
        {
            if (value < 2000)
                throw new ValidacionException(nameof(Anio), value);
            _anio = value;
        }
    }
}
```

### 2. Servicios de Application (mock de repositorios)

Con la entidad definida, se escribe el test del servicio. El repositorio aún no existe — se mockea con Moq:

```csharp
[TestFixture]
public class VersionesServiceTests
{
    private Mock<IVersionesRepository> _repositoryMock;
    private Mock<ITransaccion>         _transaccionMock;
    private VersionesService           _sut;

    [SetUp]
    public void SetUp()
    {
        _repositoryMock = new Mock<IVersionesRepository>();
        _transaccionMock = new Mock<ITransaccion>();

        _repositoryMock
            .Setup(r => r.ObtenerTransaccion())
            .Returns(_transaccionMock.Object);
        _transaccionMock.Setup(t => t.CommitAsync()).Returns(Task.CompletedTask);
        _transaccionMock.Setup(t => t.RollbackAsync()).Returns(Task.CompletedTask);

        _sut = new VersionesService(_repositoryMock.Object, ...);
    }

    [Test]
    public async Task InsertarVersion_DatosValidos_InsertaYHaceCommit()
    {
        // Arrange
        var version = new Version { Anio = 2025, Descripcion = "Presupuesto 2025" };
        _repositoryMock.Setup(r => r.InsertarVersion(version)).ReturnsAsync(42);

        // Act
        await _sut.InsertarVersion(version);

        // Assert
        _repositoryMock.Verify(r => r.InsertarVersion(version), Times.Once);
        _transaccionMock.Verify(t => t.CommitAsync(), Times.Once);
        Assert.That(version.Codigo, Is.EqualTo(42));
    }
}
```

En este punto **la interfaz `IVersionesRepository` existe pero `VersionesRepository` todavía no**. El servicio compila y los tests pasan gracias al mock.

### 3. Interfaz del Repositorio (puerto de dominio)

El mock del paso anterior fuerza a definir la interfaz exacta que necesita el servicio:

```csharp
// HM.Presupuestos.Domain/Puertos/Repositorios/IVersionesRepository.cs
public interface IVersionesRepository
{
    Task<List<Version>> ObtenerVersiones(int anio, int? estadoIncluido = null);
    Task<int> InsertarVersion(Version version);
    Task ActualizarVersion(Version version);
    Task EliminarVersion(int codigoVersion);
    Task<bool> ExistenPrevisionesEnVersion(int codigoVersion);
    ITransaccion ObtenerTransaccion();
}
```

La interfaz emerge del test del servicio, no del diseño previo de la BD.

### 4. Implementación del Repositorio (Oracle)

Con la interfaz definida, se implementa el repositorio real contra Oracle. Esta capa ya no es unitaria — requiere conexión a BD:

```csharp
// HM.Presupuestos.Infrastructure/Persistencia/Versiones/VersionesRepository.cs
public class VersionesRepository(
    IDataAccessHelperSecure dah,
    IJwt jwt) : BasePresupuestosRepository(dah, jwt), IVersionesRepository
{
    public async Task<List<Version>> ObtenerVersiones(int anio, int? estadoIncluido = null)
    {
        var resultado = new List<Version>();
        const string query = @"
            SELECT COD_VERSION, DESC_VERSION, ANIO
              FROM PPT_VERSION
             WHERE ANIO = :Anio";

        dah.GetSqlStringComando(query);
        dah.AddParameter("Anio", anio);
        await AñadirParametroMulticompania(dah);

        await Task.Run(() =>
        {
            dah.ProcesarDatos(dr =>
            {
                while (dr.Read())
                    resultado.Add(new Version
                    {
                        Codigo      = dr.GetInt32("COD_VERSION"),
                        Descripcion = dr.GetString("DESC_VERSION"),
                        Anio        = dr.GetInt32("ANIO")
                    });
            });
        });

        return resultado;
    }
}
```

### 5. Registro en DI y página Blazor

Último paso: registrar en `Program.cs` y conectar con la página Web. En este punto ya existe todo lo demás y los tests unitarios ya validan el comportamiento del servicio.

---

## Ciclo Red-Green-Refactor

```
1. RED    → Escribir el test que falla (el código aún no existe)
2. GREEN  → Escribir el mínimo código que hace pasar el test
3. REFACTOR → Limpiar sin romper los tests
```

**Mínimo código en GREEN**: no anticipar funcionalidad. Si el test solo comprueba que se llamó a `InsertarVersion`, el servicio solo necesita llamar a `InsertarVersion`. La lógica de negocio adicional entra en tests posteriores.

```csharp
// Test RED — fuerza a escribir el mínimo
[Test]
public async Task InsertarVersion_DescripcionVacia_LanzaValidacionException()
{
    var version = new Version { Anio = 2025, Descripcion = "" };

    Assert.ThrowsAsync<ValidacionException>(() =>
        _sut.InsertarVersion(version));
}

// GREEN — añadir solo la validación necesaria
public async Task InsertarVersion(Version version)
{
    if (string.IsNullOrWhiteSpace(version.Descripcion))
        throw new ValidacionException(nameof(version.Descripcion), version.Descripcion);

    // ... resto del método
}
```

---

## Qué Mockear y Qué No

| Elemento | Estrategia |
|---|---|
| `IXxxRepository` | **Moq** — la implementación Oracle viene después |
| `IXxxService` (otros servicios) | **Moq** — cada servicio se testa de forma aislada |
| `ITransaccion` | **Moq** — siempre configurar `CommitAsync` y `RollbackAsync` |
| `ILogger<T>` | **Moq** — sin configurar (comportamiento por defecto) |
| Entidades de dominio | **Nunca mockear** — usar instancias reales |
| DTOs y ViewModels | **Nunca mockear** — usar instancias reales |

---

## Ejemplo Completo — Nuevo Módulo de Negocio

Secuencia completa para añadir la funcionalidad "Duplicar versión":

```
Paso 1: Test de validación en la entidad Version
        → Version_DuplicadaConMismoAnio_LanzaValidacionException

Paso 2: Test del servicio — happy path
        → DuplicarVersion_DatosValidos_LlamaInsertarYHaceCommit

Paso 3: Test del servicio — caso borde
        → DuplicarVersion_VersionOrigen_NoExiste_LanzaValidacionException

Paso 4: Test del servicio — transacción fallida
        → DuplicarVersion_ErrorEnBD_HaceRollback

Paso 5: Definir IVersionesRepository.DuplicarVersion(...)
        (emerge de los tests anteriores)

Paso 6: Implementar VersionesRepository.DuplicarVersion(...)
        (Oracle SP o SQL directo)

Paso 7: Registrar en DI si es nuevo servicio

Paso 8: Implementar la página Blazor
```

---

## Diferencia con Outside-In (London School)

| | Inside-Out (este proyecto) | Outside-In |
|---|---|---|
| Inicio | Entidad de dominio | Test E2E o de integración |
| Mocks | Solo para fronteras externas (repositorios) | Para todas las colaboraciones |
| InMemory | No se usa — Moq en su lugar | Recomendado para repositorios |
| Diseño emerge de | Tests unitarios de dominio | Tests de aceptación |
| Riesgo | Puede no integrar correctamente hasta el final | Más trazabilidad desde requisito a código |

En este proyecto se usa **Inside-Out** porque:
- El dominio tiene reglas de negocio claras que conviene estabilizar primero
- Oracle no tiene equivalente en memoria — los tests de integración son costosos de configurar
- Moq permite avanzar en la lógica sin bloquear al equipo en la infraestructura

---

## Reglas del Ciclo

- Nunca escribir código de producción sin un test en rojo que lo justifique
- Nunca pasar al siguiente paso sin que todos los tests actuales estén en verde
- El refactor solo se hace con todos los tests en verde
- Un test en rojo a la vez — no añadir múltiples tests fallidos simultáneamente
- Los tests de pasos anteriores nunca se modifican para hacer pasar código nuevo
