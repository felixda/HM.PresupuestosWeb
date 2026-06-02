# Estándares de Testing — Detalle

## Pirámide de Tests

```
        /\
       /  \        E2E (pocos)
      /----\       - Playwright + NUnit
     /      \      - Flujos críticos de usuario
    /--------\
   /          \    Integración (algunos)
  /            \   - Adaptadores de repositorio con Oracle real
 /              \
/----------------\ Unitarios (muchos)
                   - Servicios de Application con Moq
                   - Lógica de dominio
```

- **Muchos tests unitarios**: rápidos, aislados, cubren casos borde
- **Algunos tests de integración**: repositorios reales contra Oracle
- **Pocos E2E**: flujos de usuario críticos con Playwright

---

## Proyectos de Test

| Proyecto | Tipo | Framework |
|---|---|---|
| `HM.Presupuestos.UnitTest` | Unitarios | NUnit 3 + Moq 4 |
| `HM.Presupuestos.E2ETest` | E2E | Playwright + NUnit |

---

## Tests Unitarios — `HM.Presupuestos.UnitTest`

### Dependencias

```xml
<PackageReference Include="NUnit"               Version="3.14.0" />
<PackageReference Include="NUnit3TestAdapter"   Version="4.5.0" />
<PackageReference Include="NUnit.Analyzers"     Version="3.9.0" />
<PackageReference Include="Moq"                 Version="4.20.72" />
<PackageReference Include="coverlet.collector"  Version="6.0.0" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
```

```csharp
// GlobalUsings.cs
global using NUnit.Framework;
global using Moq;
```

### Estructura de carpetas

```
HM.Presupuestos.UnitTest/
├── GlobalUsings.cs
├── Mantenimientos/
│   └── IndicadoresServiceTests.cs
└── [NombreModulo]/
    └── [NombreServicio]Tests.cs
```

Los tests se organizan por módulo de negocio, igual que en `Application/CasosDeUso/`.

---

## Nomenclatura

Formato: `Método_Condición_ResultadoEsperado`

```csharp
// ✅ CORRECTO — describe la regla de negocio
ObtenerIndicadoresConIdiomas_SinFiltro_DevuelveListaCompleta
Grabar_DescripcionDuplicada_LanzaValidacionException
Grabar_IndicadorNuevoSinDuplicados_InsertaYHaceCommit

// ❌ EVITAR — acoplado a implementación
TestObtenerIndicadores_Returns2Items
ShouldThrowWhenDuplicated
```

---

## Estructura AAA (Arrange-Act-Assert)

Separar visualmente las tres secciones con una línea en blanco:

```csharp
[Test]
public async Task ObtenerIndicadoresConIdiomas_SinFiltro_DevuelveListaCompleta()
{
    // Arrange
    var indicadoresEsperados = new List<Indicador>
    {
        new() { Codigo = 1, Descripcion = "Activo",  BitAnd = 1, Orden = 10 },
        new() { Codigo = 2, Descripcion = "Cerrado", BitAnd = 2, Orden = 20 },
    };

    _repositoryMock
        .Setup(r => r.ObtenerIndicadoresConIdiomas(null))
        .ReturnsAsync(indicadoresEsperados);

    // Act
    var resultado = await _sut.ObtenerIndicadoresConIdiomas();

    // Assert
    Assert.That(resultado, Has.Count.EqualTo(2));
    Assert.That(resultado[0].Descripcion, Is.EqualTo("Activo"));
}
```

---

## Setup de la Clase de Test

Usar `[SetUp]` para inicializar los mocks y construir el SUT. El repositorio siempre expone `ObtenerTransaccion()` que devuelve el mock de transacción:

```csharp
[TestFixture]
public class IndicadoresServiceTests
{
    private Mock<ILogger<IndicadoresService>>  _loggerMock;
    private Mock<IIndicadoresRepository>       _repositoryMock;
    private Mock<IVersionesService>            _versionesServiceMock;
    private Mock<ILogAccionesService>          _logAccionesMock;
    private Mock<ITransaccion>                 _transaccionMock;
    private IndicadoresService                 _sut;

    [SetUp]
    public void SetUp()
    {
        _loggerMock           = new Mock<ILogger<IndicadoresService>>();
        _repositoryMock       = new Mock<IIndicadoresRepository>();
        _versionesServiceMock = new Mock<IVersionesService>();
        _logAccionesMock      = new Mock<ILogAccionesService>();
        _transaccionMock      = new Mock<ITransaccion>();

        // El repositorio siempre devuelve la transacción mock
        _repositoryMock
            .Setup(r => r.ObtenerTransaccion())
            .Returns(_transaccionMock.Object);

        _transaccionMock.Setup(t => t.CommitAsync()).Returns(Task.CompletedTask);
        _transaccionMock.Setup(t => t.RollbackAsync()).Returns(Task.CompletedTask);

        _sut = new IndicadoresService(
            _loggerMock.Object,
            _repositoryMock.Object,
            _versionesServiceMock.Object,
            _logAccionesMock.Object);
    }
}
```

---

## Política de Mocks

Los repositorios se mockean con **Moq**. No se usan implementaciones InMemory.

```csharp
// Devolver lista de entidades
_repositoryMock
    .Setup(r => r.ObtenerVersiones(2025, null))
    .ReturnsAsync(new List<Version> { ... });

// Lista vacía
_repositoryMock
    .Setup(r => r.ObtenerVersiones(2025, null))
    .ReturnsAsync([]);

// Valor escalar
_repositoryMock.Setup(r => r.ObtenerUltimoBitAnd()).ReturnsAsync(16);

// Flag de validación
_repositoryMock.Setup(r => r.ExisteIndicador(indicador)).ReturnsAsync(false);

// Operación sin retorno
_repositoryMock.Setup(r => r.ActualizarIndicador(indicador)).Returns(Task.CompletedTask);
```

**Otros servicios** (logger, servicios de aplicación) también se mockean con Moq.

---

## Verificaciones con Moq

```csharp
// Verificar que se llamó exactamente una vez
_repositoryMock.Verify(r => r.InsertarIndicador(indicador), Times.Once);
_transaccionMock.Verify(t => t.CommitAsync(), Times.Once);

// Verificar que NO se llamó
_repositoryMock.Verify(r => r.InsertarIndicador(It.IsAny<Indicador>()), Times.Never);

// Verificar con cualquier argumento
_repositoryMock.Verify(r => r.EliminarVersion(It.IsAny<int>()), Times.Once);
```

---

## Assertions — NUnit

Usar siempre `Assert.That(actual, constraint)`:

```csharp
// Colecciones
Assert.That(resultado, Has.Count.EqualTo(2));
Assert.That(resultado, Is.Empty);
Assert.That(resultado, Has.Exactly(1).Matches<Indicador>(i => i.Codigo == 5));

// Valores
Assert.That(resultado.Descripcion, Is.EqualTo("Activo"));
Assert.That(indicador.Codigo,      Is.EqualTo(99));   // ID asignado tras inserción

// Excepciones de dominio
Assert.ThrowsAsync<ValidacionException>(() => _sut.Grabar(indicador, [], [], []));

// Null / not null
Assert.That(resultado, Is.Not.Null);
```

---

## Tests de Validación de Negocio

Cuando el servicio debe lanzar `ValidacionException` ante datos incorrectos:

```csharp
[Test]
public void Grabar_DescripcionDuplicada_LanzaValidacionException()
{
    // Arrange
    var indicador = new Indicador
    {
        Descripcion = "Duplicado",
        BitAnd = 4,
        Orden = 30,
        Estado = EstadoEntidad.Nuevo
    };

    _repositoryMock.Setup(r => r.ExisteIndicador(indicador)).ReturnsAsync(true);

    // Act & Assert
    Assert.ThrowsAsync<ValidacionException>(() => _sut.Grabar(indicador, [], [], []));
}
```

---

## Tests Transaccionales

Cuando el servicio usa `ITransaccion`, verificar que se hace `CommitAsync` en éxito y que `RollbackAsync` se llama en caso de error:

```csharp
[Test]
public async Task Grabar_IndicadorNuevoSinDuplicados_InsertaYHaceCommit()
{
    // Arrange
    var indicador = new Indicador
    {
        Descripcion = "Nuevo",
        BitAnd = 4,
        Orden = 30,
        Estado = EstadoEntidad.Nuevo
    };

    _repositoryMock.Setup(r => r.ExisteIndicador(indicador)).ReturnsAsync(false);
    _repositoryMock.Setup(r => r.ExisteOrden(indicador)).ReturnsAsync(false);
    _repositoryMock.Setup(r => r.ExisteBitAnd(indicador)).ReturnsAsync(false);
    _repositoryMock.Setup(r => r.InsertarIndicador(indicador)).ReturnsAsync(99);

    // Act
    await _sut.Grabar(indicador, [], [], []);

    // Assert
    _repositoryMock.Verify(r => r.InsertarIndicador(indicador), Times.Once);
    _transaccionMock.Verify(t => t.CommitAsync(),   Times.Once);
    _transaccionMock.Verify(t => t.RollbackAsync(), Times.Never);
    Assert.That(indicador.Codigo, Is.EqualTo(99));
}
```

---

## Organización con `#region`

Agrupar los tests por método del servicio que prueban:

```csharp
[TestFixture]
public class VersionesServiceTests
{
    // ... setup ...

    #region ObtenerVersiones

    [Test]
    public async Task ObtenerVersiones_ConAnio_DevuelveVersionesDelAnio() { ... }

    [Test]
    public async Task ObtenerVersiones_SinResultados_DevuelveListaVacia() { ... }

    #endregion

    #region InsertarVersion

    [Test]
    public async Task InsertarVersion_DatosValidos_InsertaYHaceCommit() { ... }

    [Test]
    public void InsertarVersion_VersionDuplicada_LanzaValidacionException() { ... }

    #endregion
}
```

---

## Tests E2E — `HM.Presupuestos.E2ETest`

Ver [../../e2e-standards.md](../../e2e-standards.md) para el detalle completo. Resumen:

- Framework: **Playwright + NUnit**
- Heredan de `E2ETestBase` y navegan con `IrAUrl("ruta-relativa")`
- La sesión SSO se guarda en `sesion_auth.json` (generado con `GuardarSesion.ps1`)
- URL base configurada en `appsettings.json` → `E2ETest:BaseUrl` (default: `https://localhost:7001`)
- Cubren flujos completos de negocio, no detalles de implementación

---

## Principios FIRST

- **Fast**: los tests unitarios no tocan BD ni red; Moq garantiza respuesta inmediata
- **Isolated**: cada test tiene su propio `[SetUp]`; no comparten estado
- **Repeatable**: datos construidos inline sin dependencia de entorno
- **Self-validating**: `Assert.That` con constraints claros; no inspección manual
- **Timely**: escribir el test antes o durante el desarrollo de la funcionalidad

---

## Qué NO testear en tests unitarios

- Lógica interna de los repositorios (acceso a Oracle) → tests de integración
- Rendering de componentes Blazor → E2E con Playwright
- Llamadas a HM.Core API → mockear `IDataAccessHelperSecure`
- Configuración del DI container (`Program.cs`) → no necesita test
