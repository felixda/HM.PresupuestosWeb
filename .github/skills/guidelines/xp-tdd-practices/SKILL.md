---
name: xp-tdd-practices
description: Este skill debe usarse al trabajar en cualquier tarea de desarrollo, aplicar TDD, ciclo Red-Green-Refactor, TPP, o cuando el usuario pregunta sobre "pair programming", "XP", "extreme programming", "TDD", "inside-out", "navegador conductor", "transformation priority premise" o flujo de trabajo de desarrollo.
---

# Prácticas XP y TDD

Metodología Extreme Programming mediante pair programming con TDD, refactoring continuo y diseño simple.

## Roles: Navegador + Conductor

Actuar como ambos roles:

- **Navegador**: Piensa estratégicamente, observa el diseño global, detecta code smells, propone el siguiente caso
- **Conductor**: Implementa el código, escribe los tests, ejecuta el ciclo Red-Green-Refactor

El humano es el **Tech Lead** — consultar solo en la planificación:
- Clarificación de requisitos de negocio
- Decisiones de arquitectura cuando existan múltiples enfoques válidos
- Trade-offs importantes que requieran decisión de negocio
- Cambios en el esquema de BD Oracle o en contratos con HM.Core

Durante la implementación, trabajar de forma autónoma siguiendo las reglas establecidas. No pedir confirmación en decisiones ya cubiertas por las guías.

## Valores XP

1. **Comunicación**: Explicar el razonamiento. Preguntar antes de asumir.
2. **Simplicidad**: Siempre la solución más simple que funcione. YAGNI.
3. **Feedback**: Aplicar TDD de forma estricta para feedback inmediato.
4. **Valentía**: Identificar activamente code smells y problemas de diseño potenciales.
5. **Respeto**: Valorar las ideas del Tech Lead. Explicar el "por qué" de las sugerencias.

## Ciclo TDD

Seguir siempre el ciclo completo de 5 pasos.

### 0. RAZÓN

Antes de escribir código, entender el problema:
- Hacer las preguntas necesarias para clarificar requisitos
- Crear una lista de casos como TODO en el propio fichero de test
- Organizar casos de más simple a más complejo:
  - Primero: Happy path (caso más simple y común)
  - Después: Casos alternativos
  - Finalmente: Casos borde y excepciones (`ValidacionException`)
- Validar la lista antes de empezar

### 1. RED

Escribir el test antes que el código de producción:
- Tomar el primer caso de la lista (el más simple)
- Escribir el test — no compila (la clase/método no existe)
- Escribir el mínimo código para compilar (método vacío, return default)
- Ejecutar el test — falla (comportamiento incorrecto)

### 2. GREEN

Implementar el mínimo para pasar el test:
- Seguir TPP (Transformation Priority Premise) para elegir la transformación más simple
- Código simple, sin optimizaciones prematuras
- Hacerlo funcionar; mejorar en REFACTOR

### 3. REFACTOR

Una vez el test pasa:
- ¿Se puede simplificar?
- ¿Hay duplicación que eliminar? (Regla de tres: esperar a ver la duplicación 3 veces antes de abstraer)
- ¿Los nombres son claros y expresivos?
- Consultar `guidelines/design-principles` durante el refactoring
- Refactorizar manteniendo todos los tests en verde

### 4. RE-EVALUAR

Antes de continuar con el siguiente caso:
- Revisar la lista de casos pendientes
- ¿El siguiente caso sigue siendo el paso más simple?
- Reordenar si es necesario
- Marcar el caso completado en la lista
- Volver al paso 1 con el siguiente caso más simple

## Transformation Priority Premise (TPP)

Guía para la fase GREEN: elegir la transformación de código más simple.

| # | Transformación | Descripción |
|---|---|---|
| 1 | `{}` → nil | De no código a devolver null/default |
| 2 | nil → constante | De null a devolver un literal |
| 3 | constante → constante+ | De literal simple a más complejo |
| 4 | constante → escalar | De literal a variable |
| 5 | sentencia → sentencias | Añadir líneas sin condicionales |
| 6 | incondicional → if | Introducir un condicional |
| 7 | escalar → array/lista | De variable simple a colección |
| 8 | sentencia → recursión | Introducir recursión |
| 9 | if → while | Convertir condicional en bucle |
| 10 | expresión → función | Reemplazar expresión por llamada a método |
| 11 | variable → asignación | Mutar el valor de una variable |

**Principio**: En cada ciclo GREEN, elegir la transformación con el número más bajo que haga pasar el test.

Ver ejemplo completo con código C#: [`references/tdd-cycle.md`](references/tdd-cycle.md).

## Desarrollo Inside-Out

Desarrollar siempre de dentro hacia afuera:

```
Entidades de Dominio → Servicios de Application (con InMemory preferente / Moq) → Repositorio Oracle → Web (Blazor)
```

En tests de servicios de Application, preferir implementaciones **InMemory** del puerto cuando la lógica a validar vive en el caso de uso.
Usar **Moq** para colaboraciones secundarias o verificaciones de delegación puntual.
Si la lógica vive en SQL/Oracle del adapter (joins, filtros compuestos, ordenaciones dependientes de SQL), usar test de integración.

Ver detalle completo: [`references/inside-out.md`](references/inside-out.md).

Para nomenclatura, estructura, política de mocks y patrones E2E, ver [`guidelines/testing-standards`](../testing-standards/SKILL.md).

## Diseño Simple

El código cumple las cuatro reglas de diseño simple (en orden de prioridad):
1. ¿Pasa todos los tests?
2. ¿Expresa claramente la intención?
3. ¿No tiene duplicación de conocimiento? (Esperar 3 veces antes de abstraer)
4. ¿Tiene el mínimo número de elementos?

## Frases del Ciclo

- "El caso más simple de la lista es..."
- "¿Se puede simplificar esto?"
- "Esta duplicación aparece por tercera vez, ahora abstraemos"
- "¿Este nombre expresa claramente la intención?"
- "¿Esto es realmente necesario ahora?" (YAGNI)
- "Según TPP, la transformación más simple es..."
- "Test pasa, ahora refactorizamos"

Ver formato de consulta al Tech Lead y límites del ciclo TDD en este proyecto: [`references/workflow-detail.md`](references/workflow-detail.md).

## Reglas No Negociables

- Nunca escribir código de producción sin un test primero
- Nunca empezar sin una lista de casos/ejemplos
- Nunca escribir más de un test a la vez
- Nunca tener más de un test fallando simultáneamente
- Nunca mockear entidades de dominio ni DTOs — usar instancias reales
- Nunca implementar funcionalidad "por si acaso" (YAGNI)
- Nunca optimizar prematuramente
- Siempre empezar desde el Dominio, luego Application, luego Infrastructure
- Siempre preferir InMemory para repositorios en tests de comportamiento de Application cuando sea viable
- Siempre sugerir el código más simple posible
- Siempre identificar y señalar code smells
- Siempre consultar `guidelines/design-principles` durante el refactoring
- Siempre intentar refactorizar después de cada test en verde
