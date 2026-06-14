---
name: azure-devops
description: Convenciones para la gestión de work items en Azure DevOps. Cubre creación de tareas, numeración, títulos y vinculación a User Stories.
Triggers: "crea tasks", "crea tareas", "define las tareas", "azure devops", "work items".
---

# Azure DevOps — Gestión de Tareas

Convenciones para crear y organizar Tasks dentro de un User Story.

## Numeración de tareas

Las tareas se numeran **en orden correlativo de creación**, comenzando por 1.

El número se incluye al inicio del título, entre corchetes, antes del prefijo de capa:

```
[1][TEST] Escribir test unitario: ...
[2][DOMINIO] Añadir propiedad ...
[3][PUERTO] Añadir método ...
[4][APLICACIÓN] Actualizar servicio ...
[5][INFRAESTRUCTURA] Implementar consulta Oracle ...
[6][WEB] Mostrar aviso en página ...
```

El número refleja el **orden en que se crean** en Azure DevOps, no una prioridad ni un orden de implementación (aunque habitualmente coincide con el flujo inside-out de la arquitectura hexagonal).

## Prefijos de capa

Cada tarea lleva un prefijo que identifica la capa afectada:

| Prefijo | Capa |
|---|---|
| `[TEST]` | Tests unitarios (TDD red) |
| `[DOMINIO]` | `HM.Presupuestos.Domain` |
| `[PUERTO]` | Puertos de dominio (`Domain/Puertos`) |
| `[APLICACIÓN]` | `HM.Presupuestos.Application` |
| `[INFRAESTRUCTURA]` | `HM.Presupuestos.Infrastructure` |
| `[WEB]` | `HM.Presupuestos.Web` (UI Blazor) |

## Formato del título

```
[N][PREFIJO] Verbo en infinitivo + qué + contexto
```

Ejemplos correctos:
- `[1][TEST] Escribir test unitario: CondicionesService marca medios sin condiciones MMS`
- `[2][DOMINIO] Añadir propiedad SinCondicionesMMS a CondicionDto`
- `[3][PUERTO] Añadir método ObtenerMediosSinCondicionesMMS en ICondicionesRepository`

## Vinculación al User Story

Todas las tareas deben crearse con `parentId` igual al ID del User Story. Esto las enlaza automáticamente como hijas del US en Azure DevOps.

## Campo obligatorio

El campo `Custom.CompetencyCenter` debe establecerse siempre a `"Develop"` al crear cualquier Task.
