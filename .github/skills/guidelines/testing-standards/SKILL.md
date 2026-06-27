---
name: testing-standards
description: Este skill debe usarse al escribir tests, revisar código de test, o trabajar con estructura de tests, nomenclatura, mocks, o tests E2E. Aplica a los proyectos HM.Presupuestos.UnitTest y HM.Presupuestos.E2ETest.
---

# Estándares de Testing

Estándares para escribir tests claros y mantenibles en todas las capas.

## Principios FIRST

- **Fast**: Los tests unitarios no tocan BD ni red; usar InMemory o Moq garantiza respuesta inmediata
- **Isolated**: Cada test tiene su propio `[SetUp]`; no comparten estado entre tests
- **Repeatable**: Datos construidos inline sin dependencia de entorno externo
- **Self-validating**: `Assert.That` con constraints claros; sin inspección manual
- **Timely**: Escritos durante o antes del desarrollo de la funcionalidad

## Pirámide de Tests

- **Muchos tests unitarios**: Servicios de Application con repositorios InMemory (preferente) o Moq, lógica de dominio
- **Algunos tests de integración**: Repositorios reales contra Oracle
- **Pocos E2E**: Flujos críticos de usuario con Playwright

## Nomenclatura

- Nombres **en español**, representando reglas de negocio, no detalles de implementación
- Formato: `Método_Condición_ResultadoEsperado`
- Usar lenguaje de dominio: "Lanza", "Devuelve", "Inserta", "Actualiza"
- Evitar verbos técnicos acoplados a implementación: "Returns", "Calls", "Should"

```csharp
// ✅ CORRECTO — regla de negocio clara
ObtenerIndicadoresConIdiomas_SinFiltro_DevuelveListaCompleta
Grabar_DescripcionDuplicada_LanzaValidacionException

// ❌ EVITAR — acoplado a implementación
TestObtenerIndicadores_Returns2Items
ShouldThrowWhenDuplicated
```

## Estructura: AAA (Arrange-Act-Assert)

- **Arrange**: Preparar contexto y datos necesarios
- **Act**: Ejecutar la acción a probar
- **Assert**: Verificar el resultado esperado
- Separar visualmente las tres secciones con una línea en blanco

## Política de Mocks

En tests unitarios de Application, preferir repositorios **InMemory** cuando la lógica a validar vive en el caso de uso.
Usar **Moq** cuando InMemory no aporte claridad o para colaboraciones secundarias.

- `InMemoryXxxRepository` para repositorios en tests de comportamiento de casos de uso
- `Mock<IXxxRepository>` para casos de delegación puntual o cuando el fake InMemory no compense
- `Mock<IXxxService>` para servicios de Application
- `InMemoryTransaccion` o `Mock<ITransaccion>` para transacciones
- `Mock<ILogger<T>>` para loggers

### Regla de decisión InMemory vs Integración

- Si la lógica vive en el caso de uso (Application), test unitario con InMemory.
- Si la lógica vive en el adapter SQL/Oracle (joins, filtros complejos, ordenaciones dependientes de SQL), test de integración con Oracle real.
- No duplicar lógica SQL del adapter en repositorios InMemory.

## Estrategia de Tests por Capa

| Capa | Tipo | Herramienta | Proyecto |
|---|---|---|---|
| Entidades de dominio | Unitario | NUnit | `UnitTest` |
| Servicios de Application | Unitario | NUnit + InMemory/Moq | `UnitTest` |
| Repositorios (Oracle) | Integración | Oracle real | _(pendiente)_ |
| Flujos de usuario | E2E | Playwright + NUnit | `E2ETest` |

## Localización de Tests

```
HM.Presupuestos.UnitTest/
└── [NombreModulo]/
    └── [NombreServicio]Tests.cs

HM.Presupuestos.E2ETest/
└── Tests/
    └── [NombreFlujo]Tests.cs
```

Para ejemplos completos, setup de tests, patrones de mocks y tests E2E, ver [`references/testing-detail.md`](references/testing-detail.md).

## Reglas No Negociables

- Nunca eliminar un test existente; corregir la implementación si falla
- Nunca modificar un test para hacer pasar la implementación
- Siempre usar estructura AAA con líneas en blanco entre secciones
- Siempre nombrar los tests como reglas de negocio, no aserciones técnicas
- Siempre usar `[SetUp]` para inicializar mocks y construir el SUT
- Siempre aislar dependencias externas (BD/red) usando InMemory o Moq
- Nunca usar `Assert.AreEqual` ni `Assert.IsTrue`; siempre `Assert.That(..., constraint)`
- Nunca compartir estado entre tests (no campos inicializados fuera de `[SetUp]`)
