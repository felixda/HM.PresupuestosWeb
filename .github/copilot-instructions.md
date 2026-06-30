# Copilot Instructions — HM.Presupuestos

## Fuente de verdad

Toda convención, patrón de código, regla de arquitectura y proceso de trabajo está definido en:

- **`openspec/config.yaml`** — contexto del proyecto, convenciones, reglas de arquitectura y proceso OpenSpec
- **`.github/skills/guidelines/`** — guías detalladas por área:
  - `architecture-hexagonal/` — capas, estructura de módulos, puertos y adaptadores
  - `design-principles/` — nomenclatura, funciones, clases, manejo de errores
  - `testing-standards/` — nomenclatura de tests, estructura AAA, principios FIRST
  - `xp-tdd-practices/` — ciclo TDD, TPP, inside-out
  - `frontend-patterns/` — componentes Blazor, ciclo de vida, CSS isolation, DevExpress
  - `architecture-hexagonal/references/infrastructure/repositories.md` — patrones de implementación de repositorios Oracle
  - `architecture-hexagonal/references/infrastructure/swagger-local.md` — uso de Swagger local con JWT de desarrollo (`RunSwaggerWithToken.ps1`) y diagnóstico de 401
  - `architecture-hexagonal/references/infrastructure/logs-tecnicos-admin.md` — lineamientos del visor Admin/Logs, formato JSONL de NLog y filtros de búsqueda
  - `git-strategy/` — feature branching, conventional commits
  - `azure-devops/` — convenciones de work items, pipelines y gestión de ramas en Azure DevOps
  - `database/` — convenciones de base de datos Oracle, migraciones y nomenclatura de objetos

Antes de implementar cualquier artefacto, leer la guideline correspondiente a la capa afectada.

## Especificaciones técnicas

Ficheros en `.github/specs/`:

- **`technical-specs.md`** — Diagramas de arquitectura, ejemplos de código por capa y modelo de datos.
- **`devops/mcp-azure-devops.md`** — Configuración y uso del MCP de Azure DevOps (servidores, herramientas disponibles, ejemplos de uso).

## Agentes especializados

Los agentes de `.github/agents/` encapsulan flujos de trabajo autónomos. Invocarlos con `@<nombre-agente>` en el chat:

- **`architecture-reviewer`** — Verifica el cumplimiento de arquitectura hexagonal (fronteras de capa, dependencias). Úsalo tras cambios en el código.
- **`code-reviewer`** — Revisa calidad de código C#/Blazor contra los principios de diseño del proyecto. Úsalo tras cambios en el código.
- **`env-switcher`** — Cambia los user-secrets al entorno indicado (DEV, PRU, PRE, PRO). Úsalo para arrancar la aplicación apuntando a un entorno concreto.
- **`frontend-reviewer`** — Revisa patrones de UI en páginas, componentes o CSS Blazor/DevExpress. Úsalo tras cambios en la capa Web.
- **`project-validator`** — Compila la solución y ejecuta todos los tests. Úsalo para validar el estado del proyecto tras cualquier cambio.
- **`qa-tester`** — Prueba flujos funcionales contra las specs usando Playwright/NUnit. Úsalo tras una implementación para auditar cobertura E2E.
- **`repositories-reviewer`** — Verifica que las queries SQL de Infrastructure siguen los patrones de `repositories.md`. Úsalo tras cambios en repositorios.
- **`tests-reviewer`** — Revisa calidad y cobertura de tests unitarios (.NET/NUnit/Moq). Úsalo para identificar gaps de cobertura.
- **`ux-reviewer`** — Evalúa la UI visualmente contra las buenas prácticas de UX del proyecto. Úsalo tras una implementación para revisar la interfaz.

## Prompts reutilizables

Los prompts de `.github/prompts/` se invocan con `/nombre-prompt` en el chat:

- **`opsx-apply`** — Implementa las tareas de un change OpenSpec en curso.
- **`opsx-archive`** — Archiva un change completado en el flujo OpenSpec.
- **`opsx-explore`** — Modo exploración: pensar ideas, investigar problemas y clarificar requisitos antes de implementar.
- **`opsx-propose`** — Propone un nuevo change generando todos sus artefactos (design, spec, tasks) en un solo paso.

### Traducciones y recursos

Prompts en `.github/prompts/traducciones-recursos/`:

- **`anadir-traduccion`** — Añade una nueva entrada de traducción en los tres ficheros de recursos JSON (`app.es.json`, `app.en.json`, `app.pt.json`).
- **`ConvetirJson`** — Regenera la clase `Helper/TextosApp.cs` con constantes fuertemente tipadas a partir de `wwwroot/data/app.es.json`.
