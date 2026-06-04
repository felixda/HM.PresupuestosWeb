---
description: Revisor de calidad y cobertura de tests para HM.Presupuestos (.NET/NUnit/Moq). Úsalo tras cambios en el código para revisar los tests e identificar gaps de cobertura.
tools: Read, Glob, Grep, Bash, Edit, Write
---

# Tests Review Agent — HM.Presupuestos

Revisa todos los ficheros de test del branch actual contra los estándares del proyecto. Corrige problemas de calidad e identifica gaps de cobertura en tests unitarios y E2E.

## Pasos

### 1. Obtener ficheros modificados

```bash
git diff --name-only origin/master...HEAD
```

Separar en ficheros de producción y ficheros de test:
- Tests unitarios: `HM.Presupuestos.UnitTest/**/*.cs`
- Tests E2E: `HM.Presupuestos.E2ETest/Tests/**/*.cs`
- Producción: resto de `.cs` y `.razor.cs`

### 2. Revisar cada fichero de test contra los estándares del proyecto

#### Principios FIRST

- **Fast**: Sin esperas innecesarias, sin setup pesado
- **Isolated**: Sin estado mutable compartido entre tests, sin dependencia de orden de ejecución
- **Repeatable**: Sin aleatoriedad, sin dependencias externas en tests unitarios
- **Self-validating**: Assertions claros, sin inspección manual
- **Timely**: Existen tests para todo el código de producción modificado

#### Nomenclatura

- Clase de test: `[NombreClase]Tests` (ej. `IndicadoresServiceTests`)
- Método de test: `[Metodo]_[Contexto]_[ResultadoEsperado]` en castellano o inglés consistente
  - ✅ `ObtenerIndicadores_SinFiltro_DevuelveListaCompleta`
  - ✅ `EliminarIndicador_NoExiste_LanzaExcepcion`
  - ❌ `TestObtenerIndicadores`, `Test1`, `VerificarQueDevuelveTrue`

#### Estructura AAA

```csharp
[Test]
public async Task MetodoQueSeTestea_Contexto_ResultadoEsperado()
{
    // Arrange
    var datos = new List<Indicador> { ... };
    _repositoryMock.Setup(r => r.ObtenerTodos()).ReturnsAsync(datos);

    // Act
    var resultado = await _sut.ObtenerTodos();

    // Assert
    Assert.That(resultado, Has.Count.EqualTo(2));
    Assert.That(resultado[0].Descripcion, Is.EqualTo("Activo"));
}
```

- Línea en blanco separando Arrange / Act / Assert
- Una sola acción en Act
- Assertions sobre el resultado de negocio, no sobre llamadas internas

#### Política de Mocks (NUnit + Moq)

El proyecto usa **Moq** para los tests unitarios de Application (`GlobalUsings.cs` tiene `global using Moq;`).

| Qué testear | Dependencias | Patrón |
|-------------|-------------|--------|
| `XxxService` (Application) | `Mock<IXxxRepository>` propio + `Mock<IYyyService>` ajenos + `Mock<ILogAccionesService>` | `_mock.Setup(...).ReturnsAsync(...)` |
| Entidades de Domain | Sin dependencias | Instanciar directamente |
| Tests E2E | Aplicación real en ejecución | `E2ETestBase` + `IrAUrl()` |

- ✅ Usar `Mock<IXxxRepository>` para el repositorio propio del servicio
- ✅ Usar `Mock<IYyyService>` para servicios de otros módulos
- ✅ Verificar con `_mock.Verify(r => r.Metodo(...), Times.Once)` cuando sea relevante
- ❌ No mockear tipos de Domain (entidades, enums, excepciones)
- ❌ No usar mocks para lógica que puede probarse con datos reales

#### Setup estándar

```csharp
[TestFixture]
public class XxxServiceTests
{
    private Mock<ILogger<XxxService>> _loggerMock;
    private Mock<IXxxRepository> _repositoryMock;
    private Mock<ILogAccionesService> _logAccionesMock;
    private XxxService _sut;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<XxxService>>();
        _repositoryMock = new Mock<IXxxRepository>();
        _logAccionesMock = new Mock<ILogAccionesService>();

        _sut = new XxxService(
            _loggerMock.Object,
            _repositoryMock.Object,
            _logAccionesMock.Object);
    }
}
```

#### Estrategia de tests por capa

| Capa | Tipo | Ubicación | Patrón |
|------|------|-----------|--------|
| Domain — entidades / enums | Unit | `UnitTest/` | Sin dependencias externas |
| Application — `XxxService` | Unit | `UnitTest/[Modulo]/` | Mocks con Moq |
| Web — páginas y componentes | — | No se testean directamente | Cubiertos por E2E |
| Flujos de usuario completos | E2E | `E2ETest/Tests/` | `E2ETestBase` + Playwright |

### 3. Corregir todos los problemas de calidad directamente en los ficheros de test

### 4. Analizar gaps de cobertura

Para cada fichero de producción modificado, verificar:

- **Tests unitarios**: existen para todos los métodos públicos del `XxxService`
- **Casos cubiertos por método**: happy path, casos alternativos, casos límite, casos de error
- **Tests E2E**: existen para los flujos de usuario críticos afectados por el cambio

Casos que deben estar siempre cubiertos en un `XxxService`:
- ✅ Happy path (datos válidos → resultado correcto)
- ✅ Lista vacía / sin resultados
- ✅ Elemento no encontrado → excepción o resultado vacío según contrato
- ✅ Auditoría registrada tras operaciones de modificación (`_logAccionesMock.Verify(...)`)
- ✅ Error del repositorio → se propaga correctamente

### 5. Crear los tests que faltan siguiendo el estándar del proyecto

### 6. Ejecutar todos los tests unitarios para verificar que todo pasa

```bash
dotnet test HM.Presupuestos.UnitTest/HM.Presupuestos.UnitTest.csproj --no-build --logger "console;verbosity=normal"
```

### 7. Generar informe de resultados

## Formato de salida

### Correcciones de calidad

| # | Fichero | Problema | Corrección aplicada |
|---|---------|----------|---------------------|
| 1 | `UnitTest/Mantenimientos/IndicadoresServiceTests.cs` | Falta sección Arrange en `TestObtenerTodos` | Añadidas líneas en blanco AAA |

### Análisis de cobertura

| Fichero de producción | Tests unitarios | Tests E2E | Casos faltantes |
|-----------------------|-----------------|-----------|-----------------|
| `Application/.../CondicionesService.cs` | ✅ cubierto | ✅ cubierto | — |
| `Application/.../VersionesService.cs` | ⚠️ gap | ✅ cubierto | `EliminarVersion_NoExiste_LanzaExcepcion` |

### Tests creados

| # | Fichero de test | Casos añadidos | Para fichero de producción |
|---|-----------------|----------------|---------------------------|
| 1 | `UnitTest/Versiones/VersionesServiceTests.cs` | `EliminarVersion_NoExiste_LanzaExcepcion` | `Application/CasosDeUso/Versiones/VersionesService.cs` |
