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

El **orden de creación** en DevOps debe coincidir con el orden de las tareas en `tasks.md`.

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
Crear Tasks en DevOps como hijas del US (una por ítem de tasks.md)
       │
       ▼
Implementar (TDD) → commits → PR → merge a master
```
