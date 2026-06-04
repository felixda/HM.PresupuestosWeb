---
name: action-tdd
description: Aplica el ciclo TDD correcto cuando la implementación salta el flujo red-green-refactor. Disparadores: "aplica TDD", "ciclo TDD", "red green refactor".
argument-hint: "[tdd | tpp]"
allowed-tools: Read, Write, Edit, Glob, Grep, Bash
---

# Enforce TDD — HM.Presupuestos

Comando correctivo cuando se está saltando el ciclo TDD o se están haciendo saltos de implementación demasiado grandes.

## Uso

```
/action-tdd           → Aplica el ciclo TDD completo (por defecto)
/action-tdd tdd       → Aplica el ciclo TDD completo
/action-tdd tpp       → Aplica TPP (transformación más simple)
```

## Modo: TDD (por defecto)

El ciclo TDD está siendo saltado. Aplicar TDD estricto:

### El ciclo de 5 pasos

1. **RAZÓN** — Identificar el comportamiento concreto a implementar a continuación
   - Leer el requisito o criterio de aceptación pendiente
   - Formularlo como un caso de test con nombre `Metodo_Contexto_ResultadoEsperado`

2. **ROJO** — Escribir el test mínimo que falla
   ```csharp
   [Test]
   public async Task EliminarCondicion_NoExiste_LanzaValidacionException()
   {
       // Arrange
       _repositoryMock.Setup(r => r.ObtenerPorCodigo(99)).ReturnsAsync((Condicion?)null);

       // Act & Assert
       Assert.ThrowsAsync<ValidacionException>(
           () => _sut.EliminarCondicion(99));
   }
   ```
   - Ejecutar y verificar que **falla por la razón correcta** (no por error de compilación):
     ```bash
     dotnet test HM.Presupuestos.UnitTest/HM.Presupuestos.UnitTest.csproj --no-build
     ```

3. **VERDE** — Escribir el código mínimo para que el test pase
   - Solo el código necesario para pasar **este test concreto**
   - No anticipar casos futuros
   - No generalizar todavía
   - Ejecutar los tests y verificar que pasan

4. **REFACTOR** — Limpiar sin cambiar el comportamiento
   - Aplicar los principios del proyecto (nomenclatura, primary constructor, AAA en tests...)
   - Ejecutar los tests tras cada cambio para confirmar que siguen en verde
   - Ver `/action-refactor` para las reglas de refactorización del proyecto

5. **RE-EVALUAR** — Decidir el siguiente paso
   - ¿Hay más casos para este método? → volver al paso 1
   - ¿El método está completo? → pasar al siguiente requisito

### Enfoque inside-out

Orden de implementación por capa:

```
Domain (entidades, excepciones) 
  → Application (XxxService con Mocks) 
  → Infrastructure (XxxRepository — sin tests unitarios, cubierto por E2E)
  → Web (página Blazor — cubierta por E2E)
```

No implementar Infrastructure ni Web antes de tener verde el Application.

### Detener la implementación actual

Si se está implementando sin un test rojo previo:

1. Identificar el último comportamiento añadido sin test
2. Escribir el test que debería haber guiado esa implementación
3. Verificar que el test pasa (verde retrospectivo)
4. Continuar con el siguiente comportamiento siguiendo el ciclo correcto

## Modo: TPP

La implementación está haciendo saltos demasiado grandes. Aplicar TPP (Transformation Priority Premise):

### Tabla de transformaciones (de más simple a más compleja)

| Prioridad | Transformación | Ejemplo en C# |
|-----------|---------------|---------------|
| 1 | `{}` → `null` / excepción | Método vacío que lanza `NotImplementedException` |
| 2 | `null` → constante | `return new List<Condicion>();` |
| 3 | Constante → variable | `return resultado;` |
| 4 | Variable → array/lista | `return new List<Condicion> { item };` |
| 5 | Sentencia → condicional | `if (condicion == null) throw ...` |
| 6 | Condicional → bucle | `foreach (var item in lista)` |
| 7 | Recursión | (raro en C# de aplicación) |

### Aplicar TPP

1. Identificar la transformación que se está intentando aplicar
2. Verificar que no hay una transformación más simple disponible que también haría pasar el test
3. Elegir la **transformación más simple** que hace pasar el test (número más bajo)
4. Implementar solo esa transformación
5. Ejecutar los tests:
   ```bash
   dotnet test HM.Presupuestos.UnitTest/HM.Presupuestos.UnitTest.csproj --no-build
   ```
6. Si pasan → refactorizar → siguiente test
7. Si no pasan → buscar una transformación más simple
