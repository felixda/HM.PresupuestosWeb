# Detalle del Flujo de Trabajo XP

## Cuándo Consultar al Tech Lead

Consultar (con análisis previo) cuando:
- **Decisiones de arquitectura**: "Se han considerado las opciones A y B, ¿cuál se prefiere dado que...?"
- **Requisitos ambiguos**: "Esto podría significar X o Y según el negocio, ¿cuál es la intención?"
- **Trade-offs importantes**: "Optimizar X implica perder Y, ¿cuál es la prioridad?"
- **Nuevas dependencias o librerías**: "¿Es aceptable este paquete NuGet o se prefiere otra alternativa?"
- **Validación de diseño**: "Se ha llegado a este diseño, ¿parece correcto?"
- **Cambios en el esquema de BD Oracle**: "La nueva funcionalidad requiere una tabla o SP nuevo, ¿lo coordinamos?"
- **Cambios en contratos de API con HM.Core**: "El servicio necesita un nuevo endpoint de Core, ¿lo gestionamos?"

## Formato de Consulta

Cuando se consulte, incluir siempre:
1. **Contexto**: qué se está intentando implementar
2. **Análisis**: opciones que se han considerado
3. **Pregunta concreta**: qué hay que decidir
4. **Recomendación** (si existe): qué parece mejor y por qué

Ejemplo:
```
"Se está implementando la lógica de validación de sobreprimas.
La condición tiene 3 reglas de negocio distintas (por medio, por
porcentaje, por vigencia).
Opciones consideradas:
- Opción A: métodos de validación en el servicio CondicionesService
  (todo en un sitio, más fácil de seguir)
- Opción B: clase separada SobreprimaValidator en Application
  (más testeable, más cohesión)

Recomendación: A por ahora (3 reglas únicamente). ¿Se prevén muchas
más validaciones para este módulo en el futuro?"
```

---

## Ciclo de Trabajo en Pareja

### Roles: Conductor y Navegador

| Rol | Responsabilidad |
|---|---|
| **Conductor** | Escribe el código. Un test o un método cada vez. |
| **Navegador** | Piensa en el diseño, detecta errores, propone el siguiente paso. No dicta línea a línea. |

Rotar roles después de cada ciclo RED-GREEN-REFACTOR completo.

### Flujo en cada ciclo

```
Navegador: propone el siguiente caso de test más simple
    ↓
Conductor: escribe el test (RED)
    ↓
Navegador: verifica que el test falla por la razón correcta
    ↓
Conductor: escribe el mínimo código que hace pasar el test (GREEN)
    ↓
Navegador: propone refactors según design-principles
    ↓
Conductor: aplica el refactor (REFACTOR)
    ↓
Ambos: confirman que todos los tests pasan antes de continuar
```

---

## Cómo Decidir el Siguiente Test

Criterio de prioridad para elegir el siguiente caso:

1. **Caso más simple primero**: lista vacía antes que lista con elementos
2. **Un solo caso borde a la vez**: no añadir múltiples tests fallidos en paralelo
3. **Validaciones de dominio** después del happy path
4. **Casos con transacción** (commit/rollback) después de los casos sin efectos secundarios

```csharp
// Orden recomendado para un nuevo servicio de Application:
// 1. Obtener → lista vacía devuelve lista vacía
// 2. Obtener → con datos devuelve los datos correctos
// 3. Insertar → datos válidos → llama al repo y hace commit
// 4. Insertar → validación de negocio falla → lanza ValidacionException
// 5. Insertar → error en BD → hace rollback
// 6. Actualizar → misma secuencia
// 7. Eliminar → misma secuencia
```

---

## Reglas de Comunicación

### Comunicación en el ciclo

- El Navegador no dicta código. Indica el objetivo: "el siguiente test debe cubrir el caso de porcentaje negativo"
- El Conductor pide aclaración si el objetivo no está claro antes de escribir
- Las discusiones de diseño se tienen **fuera del teclado**, no mientras el Conductor escribe
- Si hay desacuerdo, escribir ambas opciones como tests y dejar que el código revele cuál es más limpio

### Tono

- Directo y sin rodeos
- Constructivo, siempre con alternativas
- Sin ego: el código pertenece a los dos, no al que lo escribió

---

## Trabajo en Solitario (sin pareja)

Cuando se trabaja sin navegador, simular el rol:

1. Antes de escribir código: anotar la lista de casos en un comentario o en el propio `#region`
2. Seguir el ciclo RED-GREEN-REFACTOR de todos modos
3. Hacer commits frecuentes tras cada GREEN para tener puntos de rollback claros
4. Si aparece una decisión de diseño importante, abrir una discusión antes de continuar

```csharp
// Ejemplo de lista de casos anotada antes de empezar
[TestFixture]
public class VersionesServiceTests
{
    // Casos a cubrir:
    // ObtenerVersiones: sin resultados, con resultados, con filtro de año
    // InsertarVersion: válida, descripción duplicada, commit, rollback
    // EliminarVersion: con previsiones existentes (lanza), sin previsiones (elimina)

    ...
}
```

---

## Gestión de Deuda Técnica

Cuando durante el ciclo se detecta algo mejorable que **no pertenece al test actual**:

1. **No interrumpir el ciclo** — completar RED-GREEN-REFACTOR del test actual
2. **Anotar la deuda** como `// TODO:` en el código o en el backlog
3. **No acumular más de 3 TODOs sin resolverlos** — dedicar un ciclo específico a limpiar

```csharp
// Ejemplo de TODO acotado
public async Task<List<Sobreprima>> ObtenerSobreprimas(SobreprimaFiltro filtro)
{
    // TODO: añadir caché de 5 min para datos de referencia de medios
    return await _sobreprimasRepository.ObtenerSobreprimas(filtro);
}
```

---

## Límites del Ciclo TDD en Este Proyecto

Hay casos donde TDD directo no aplica o aplica de forma diferente:

| Situación | Estrategia |
|---|---|
| Nuevo stored procedure Oracle | Primero el SP en BD; luego test del repositorio con Oracle real |
| Componente Blazor nuevo | Implementar y validar con E2E Playwright; no test unitario de markup |
| Cambio en `Program.cs` (DI) | Verificar compilación y test E2E; no test unitario de registro |
| Migración de datos | Script SQL validado en PRE antes de PRO; no tests automatizados |
| Lógica puramente de cálculo | TDD estricto — el caso ideal |
| Validación de negocio en servicio | TDD estricto — el caso ideal |
