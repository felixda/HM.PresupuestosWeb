# Ciclo TDD con Ejemplo TPP

## El Ciclo Red-Green-Refactor

```
RED    → Escribir el test mínimo que falla (el código aún no existe)
  ↓
GREEN  → Escribir el mínimo código que hace pasar el test
  ↓
REFACTOR → Limpiar sin romper los tests
  ↓
RE-EVALUAR → Elegir el siguiente caso más simple
  ↑____________________________________________|
```

---

## Transformations Priority Premise (TPP)

Cuando se está en la fase GREEN, elegir la transformación más simple del código que haga pasar el test:

| Prioridad | Transformación | Ejemplo |
|---|---|---|
| 1 | `{}` → constante | `return 0;` |
| 2 | constante → escalar | `return precio;` |
| 3 | escalar → parámetro | `return valor;` |
| 4 | sentencia → recursión | usar `foreach` |
| 5 | if → while | iterar en lugar de bifurcar |

---

## Ejemplo Completo — Cálculo de Sobreprima

Ejemplo real adaptado al dominio: calcular el porcentaje de sobreprima total a partir de una lista de tramos.

### Lista de casos (Navigator primero)

```
1. Lista vacía → devuelve 0
2. Lista con un tramo → devuelve su porcentaje
3. Lista con varios tramos → suma los porcentajes
4. Tramo con porcentaje negativo → lanza ValidacionException
```

---

### Test 1: Lista vacía

```csharp
// RED — el método no existe aún
[Test]
public void CalcularPorcentajeTotal_ListaVacia_DevuelveCero()
{
    var tramos = new List<TramoSobreprima>();

    var total = SobreprimaCalculator.CalcularPorcentajeTotal(tramos);

    Assert.That(total, Is.EqualTo(0m));
}

// El test no compila → crear el mínimo para compilar
public static class SobreprimaCalculator
{
    public static decimal CalcularPorcentajeTotal(List<TramoSobreprima> tramos) { }
}

// Test falla (null/undefined != 0)

// GREEN — TPP: {} → constante
public static decimal CalcularPorcentajeTotal(List<TramoSobreprima> tramos)
{
    return 0m;
}

// Test pasa ✅

// REFACTOR — no hay nada que limpiar aún
```

---

### Test 2: Un solo tramo

```csharp
// RED
[Test]
public void CalcularPorcentajeTotal_UnTramo_DevuelveSuPorcentaje()
{
    var tramos = new List<TramoSobreprima>
    {
        new() { PorcentajeAplicado = 15m }
    };

    var total = SobreprimaCalculator.CalcularPorcentajeTotal(tramos);

    Assert.That(total, Is.EqualTo(15m));
}

// Test falla (0 != 15)

// GREEN — TPP: constante → escalar (usar el parámetro)
public static decimal CalcularPorcentajeTotal(List<TramoSobreprima> tramos)
{
    if (tramos.Count == 0) return 0m;
    return tramos[0].PorcentajeAplicado;
}

// Ambos tests pasan ✅

// REFACTOR — guard clause está bien según design-principles.
// Nombres claros. Sin cambios.
```

---

### Test 3: Varios tramos

```csharp
// RED
[Test]
public void CalcularPorcentajeTotal_VariosTramos_SumaLosPorcentajes()
{
    var tramos = new List<TramoSobreprima>
    {
        new() { PorcentajeAplicado = 10m },
        new() { PorcentajeAplicado = 5m  },
        new() { PorcentajeAplicado = 2m  }
    };

    var total = SobreprimaCalculator.CalcularPorcentajeTotal(tramos);

    Assert.That(total, Is.EqualTo(17m));
}

// Test falla (10 != 17)

// GREEN — TPP: sentencia → iteración. LINQ Sum es más expresivo que foreach.
// Según design-principles: "preferir estilo declarativo cuando mejora la legibilidad"
public static decimal CalcularPorcentajeTotal(List<TramoSobreprima> tramos)
{
    return tramos.Sum(t => t.PorcentajeAplicado);
}

// Los tres tests pasan ✅

// REFACTOR — el guard clause ya no es necesario: Sum de lista vacía es 0.
// La implementación final es una línea. Sin cambios adicionales.
```

---

### Test 4: Porcentaje negativo → validación de dominio

```csharp
// RED
[Test]
public void CalcularPorcentajeTotal_TramoConPorcentajeNegativo_LanzaValidacionException()
{
    var tramos = new List<TramoSobreprima>
    {
        new() { PorcentajeAplicado = -5m }
    };

    Assert.Throws<ValidacionException>(() =>
        SobreprimaCalculator.CalcularPorcentajeTotal(tramos));
}

// Test falla (no lanza excepción)

// GREEN — mínimo: validar antes de sumar
public static decimal CalcularPorcentajeTotal(List<TramoSobreprima> tramos)
{
    foreach (var tramo in tramos)
    {
        if (tramo.PorcentajeAplicado < 0)
            throw new ValidacionException(
                nameof(tramo.PorcentajeAplicado),
                tramo.PorcentajeAplicado);
    }

    return tramos.Sum(t => t.PorcentajeAplicado);
}

// Los cuatro tests pasan ✅

// REFACTOR — el foreach con guard puede convertirse en una
// validación declarativa con Any():
public static decimal CalcularPorcentajeTotal(List<TramoSobreprima> tramos)
{
    if (tramos.Any(t => t.PorcentajeAplicado < 0))
        throw new ValidacionException(
            nameof(TramoSobreprima.PorcentajeAplicado),
            "negativo");

    return tramos.Sum(t => t.PorcentajeAplicado);
}

// Los cuatro tests siguen pasando ✅
```

---

## Ciclo con Servicio de Application (InMemory preferente)

Cuando el caso de uso involucra un repositorio, el ciclo es el mismo y en Arrange
se usa InMemory para lógica de negocio del caso de uso (Moq para casos puntuales):

```csharp
// RED — el servicio aún no llama al repositorio
[Test]
public async Task ObtenerSobreprimas_ConFiltro_DevuelveElementosFiltrados()
{
    // Arrange
    var filtro = new SobreprimaFiltro { CodigoVersion = 10 };
    _repositoryInMemory.Sembrar(
        new SobreprimaBuilder().WithCodigoVersion(10).Build(),
        new SobreprimaBuilder().WithCodigoVersion(20).Build());

    // Act
    var resultado = await _sut.ObtenerSobreprimas(filtro);

    // Assert
    Assert.That(resultado, Has.Count.EqualTo(1));
    Assert.That(resultado.All(x => x.CodigoVersion == 10), Is.True);
}

// GREEN — implementar el servicio para que delegue en el repositorio
public async Task<List<Sobreprima>> ObtenerSobreprimas(SobreprimaFiltro filtro)
{
    return await _sobreprimasRepository.ObtenerSobreprimas(filtro);
}

// REFACTOR — sin cambios, implementación mínima y correcta
```

---

## Refactoring Continuo

Durante la fase REFACTOR, identificar y corregir activamente:

- **Duplicación**: si dos tests fuerzan el mismo patrón, extraer método privado
- **Nombres poco expresivos**: renombrar variables, métodos y clases según `design-principles`
- **Complejidad ciclomática alta**: extraer condiciones a métodos con nombre semántico
- **Listas que podrían ser null**: garantizar que el método devuelve lista vacía, nunca `null`

```csharp
// ANTES del refactor (Green pero sucio)
if (tramos != null && tramos.Count > 0 && tramos.Any(t => t.PorcentajeAplicado > 0))
    return tramos.Where(t => t.PorcentajeAplicado > 0).Sum(t => t.PorcentajeAplicado);
return 0m;

// DESPUÉS del refactor (mismo comportamiento, más legible)
var tramosActivos = tramos.Where(t => t.PorcentajeAplicado > 0).ToList();
return tramosActivos.Sum(t => t.PorcentajeAplicado);
```

Los tests en verde validan que el refactor no cambia el comportamiento.
