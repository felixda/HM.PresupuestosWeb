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

## Campos obligatorios al crear una Task

| Campo | Valor |
|---|---|
| `Custom.CompetencyCenter` | `"Develop"` |
| `Microsoft.VSTS.Scheduling.OriginalEstimate` | Horas estimadas para esa tarea concreta |

La estimación por tarea es libre, pero la suma de todas las tasks debe ser coherente con la estimación del US.

---

## Flujo de trabajo: US de DevOps → OpenSpec → Rama git

Cuando el punto de partida es un **User Story en Azure DevOps**, seguir este proceso de forma ordenada:

### 1. Crear la rama git desde `master`

Antes de generar ningún artefacto, crear una rama feature a partir de `master` (no de `develop`).

Convenio de nombre: `feat/us-<ID>-<descripcion-corta>`

```
feat/us-1234-auditoria-estadisticas
feat/us-5678-filtro-condiciones-por-fecha
```

- `<ID>` es el número del Work Item en Azure DevOps.
- `<descripcion-corta>` en kebab-case, máximo 40 caracteres.

```bash
git checkout master
git pull origin master
git checkout -b feat/us-<ID>-<descripcion-corta>
```

### 2. Generar la propuesta con OpenSpec

Con la rama activa, invocar `/opsx:propose` describiendo el US o adjuntando su contenido como contexto.

El resultado es el artefacto `proposal.md` en `openspec/changes/<nombre-change>/`.

### 3. Completar el ciclo OpenSpec (design → specs → tasks)

Continuar con `/opsx:apply` hasta generar `tasks.md`. El archivo `tasks.md` es la **fuente de verdad** de la descomposición técnica del US.

### 4. Crear las Tasks en Azure DevOps como hijas del US

Por cada ítem de `tasks.md`, crear una Task en Azure DevOps vinculada al US padre:

- Seguir la numeración y prefijos de capa descritos arriba (`[N][PREFIJO] ...`).
- Usar `parentId` = ID del US para que queden como hijas.
- Establecer `Custom.CompetencyCenter` = `"Develop"`.
- Establecer `Microsoft.VSTS.Scheduling.OriginalEstimate` con la estimación en horas de esa tarea.

El **orden de creación** en DevOps debe coincidir con el orden de las tareas en `tasks.md`.

### 5. Actualizar el US con estimación y fechas de inicio/fin previstas

Tras crear las tasks, rellenar en el US:

| Campo | Valor |
|---|---|
| `Microsoft.VSTS.Scheduling.OriginalEstimate` | Suma de estimaciones de tasks **+ 1 hora de margen** |
| `Microsoft.VSTS.Scheduling.StartDate` | Fecha de inicio (solo fecha, sin hora) |
| `Microsoft.VSTS.Scheduling.FinishDate` | Fecha objetivo de entrega (solo fecha, sin hora) |

### 6. Implementar y hacer commits por tarea

Al hacer commit de cada tarea, incluir en el mensaje:

```
#AB<ID_TASK> <descripción en inglés>
```

- `#AB<ID>` vincula automáticamente el commit a la Task en Azure DevOps.
- Si un commit cubre varias tasks del mismo commit: `#AB112691 #AB112692 #AB112693 ...`

### 7. Añadir comentario con el commit de GitHub en cada Task

Tras hacer push, añadir en el comentario de cada Task el enlace al commit exacto de GitHub:

```markdown
**Commit GitHub:** [<sha7> — <descripción>](https://github.com/<org>/<repo>/commit/<sha_completo>)
```

### 8. Archivar el cambio OpenSpec

Mover el directorio `openspec/changes/<nombre>` a `openspec/changes/archive/YYYY-MM-DD-<nombre>` y hacer un commit:

```
chore(openspec): archive <nombre> change
```

### 9. Cerrar las Tasks y el US

Una vez archivado y pusheado, cerrar en este orden:

1. **Cerrar cada Task** con:
   - `System.State` → `Closed`
   - `Microsoft.VSTS.Scheduling.CompletedWork` → horas reales invertidas en esa tarea

2. **Cerrar el US** con:
   - `System.State` → `Closed`
   - `Microsoft.VSTS.Scheduling.CompletedWork` → horas totales reales
   - `Microsoft.VSTS.Scheduling.StartDate` → fecha/hora real de inicio
   - `Microsoft.VSTS.Scheduling.FinishDate` → fecha/hora real de cierre

### 10. Crear el Pull Request

Crear un PR de la rama `feat/us-<ID>-<desc>` hacia `master`.

> **El merge lo realiza siempre un usuario — nunca se automatiza ni se hace desde el agente.**

### Resumen visual del flujo

```
US en Azure DevOps
       │
       ▼
git checkout -b feat/us-<ID>-<desc> (desde master)
       │
       ▼
/opsx:propose  →  proposal.md
       │
       ▼
/opsx:apply    →  design.md + specs/ + tasks.md
       │
       ▼
Crear Tasks en DevOps como hijas del US
(parentId + OriginalEstimate por tarea)
       │
       ▼
Actualizar US: OriginalEstimate (tasks + 1h margen) + StartDate + FinishDate
       │
       ▼
Implementar (TDD) → commit por tarea con #AB<ID> en el mensaje
       │
       ▼
Añadir comentario con link al commit de GitHub en cada Task
       │
       ▼
Archive OpenSpec → commit chore(openspec)
       │
       ▼
Cerrar Tasks (CompletedWork real) → Cerrar US (CompletedWork + fechas reales)
       │
       ▼
Crear PR (Pull Request hacia master)
       │
       ▼
⚠️  El merge lo realiza un usuario — NO se automatiza
```
