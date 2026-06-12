# Guidelines — Índice

Guías de referencia del proyecto organizadas por área. Cada guía indica qué agente la aplica automáticamente.

## Áreas

| Área | Guía principal | Agente |
|---|---|---|
| Arquitectura hexagonal | [architecture-hexagonal/SKILL.md](architecture-hexagonal/SKILL.md) | `architecture-reviewer` |
| Principios de diseño | [design-principles/SKILL.md](design-principles/SKILL.md) | `code-reviewer` |
| Patrones de UI (Blazor/DevExpress) | [frontend-patterns/SKILL.md](frontend-patterns/SKILL.md) | `frontend-reviewer` |
| Tests (NUnit/Moq) | [testing-standards/SKILL.md](testing-standards/SKILL.md) | `tests-reviewer` |
| TDD / XP | [xp-tdd-practices/SKILL.md](xp-tdd-practices/SKILL.md) | — |
| Estrategia Git | [git-strategy/SKILL.md](git-strategy/SKILL.md) | — |
| Repositorios de Infrastructure (Oracle) | [infrastructure/repositories.md](infrastructure/repositories.md) | `repositories-reviewer` |
| Esquema de base de datos | [database/README.md](database/README.md) | — |
| Gestión de tareas (Azure DevOps) | [azure-devops/SKILL.md](azure-devops/SKILL.md) | — |

## Referencias detalladas por área

### Arquitectura hexagonal
- [Module structure](architecture-hexagonal/references/module-structure.md) — árbol completo de la solución: qué fichero vive en qué carpeta y por qué, con la correspondencia entre proyectos y capas
- [Domain — Entidades](architecture-hexagonal/references/domain/entities.md) — características de las entidades (identidad por `Codigo`, redondeo en setter, propiedades calculadas sin setter, DataAnnotations para formularios Blazor)
- [Domain — Repositorios](architecture-hexagonal/references/domain/repositories.md) — puertos de salida del dominio: dónde se define la interfaz `IXxxRepository`, qué tipos puede usar y cómo se relaciona con su implementación en Infrastructure
- [Application — Casos de uso](architecture-hexagonal/references/application/usecases.md) — estructura de `XxxService` + `IXxxService` por módulo, reglas de inyección (solo su propio repositorio) y registro de auditoría tras operaciones exitosas
- [Application — DTOs](architecture-hexagonal/references/application/dtos.md) — dónde viven los DTOs (en Domain, junto a la entidad), cuándo usar DTO vs entidad pura y cómo mapear en el servicio
- [Infrastructure — Repositorios](architecture-hexagonal/references/infrastructure/repositories.md) — implementaciones concretas de los puertos: herencia de `BasePresupuestosRepository`, acceso a `IDataAccessHelperSecure` y reglas de acceso al esquema Oracle
- [Infrastructure — Cliente API Core](architecture-hexagonal/references/infrastructure/clienteApiCore.md) — único punto de acceso a la API REST de HM.CORE: patrón Puerto/Adaptador, registro como Typed HttpClient, autenticación JWT y manejo de errores HTTP

### Principios de diseño
- [Nomenclatura](design-principles/references/naming.md) — convenciones PascalCase/camelCase por tipo de símbolo, sufijos permitidos (`Service`, `Repository`, `Dto`, `ViewModel`, `Filtro`), regla de un concepto = un nombre en todo el codebase
- [Funciones](design-principles/references/functions.md) — SRP, guard clauses, CQS (comandos `void`/`Task`, queries devuelven valor), máximo 3 parámetros, sin `bool` de configuración, constantes cerca del uso, devolver lista vacía nunca `null`
- [Clases y módulos](design-principles/references/classes-modules.md) — ámbito mínimo (`private` por defecto), constructores solo asignan dependencias, organización por `#region`, composición sobre herencia (salvo jerarquías establecidas: páginas heredan `ContextProtegido`, repositorios heredan `BasePresupuestosRepository`)
- [Gestión de errores](design-principles/references/error-handling.md) — tres tipos de error: `ValidacionException` (dominio, unicidad), `Exception` (técnico), `InvalidOperationException` (estado inválido); captura y presentación en la capa Web via `EjecutarAsync`

### Frontend
- [CSS / estilos](frontend-patterns/references/css-modules.md) — tres capas de estilos (global `comun.css`, temas por usuario, CSS isolation `.razor.css`); cuándo crear un fichero `.razor.css` propio vs reutilizar clases globales

### Tests
- [Detalle de estándares](testing-standards/references/testing-detail.md) — pirámide de tests (muchos unitarios con NUnit+Moq, algunos de integración con Oracle real, pocos E2E con Playwright); estructura de proyectos `UnitTest` y `E2ETest`; convenciones de nomenclatura y organización de clases de test

### TDD / XP *(Test-Driven Development / Extreme Programming)*
- [Ciclo TDD](xp-tdd-practices/references/tdd-cycle.md) — ciclo Red-Green-Refactor y tabla TPP (Transformation Priority Premise) para elegir la transformación mínima en fase GREEN
- [Inside-out](xp-tdd-practices/references/inside-out.md) — estrategia de construcción desde Domain hacia Web, usando mocks solo donde es necesario (Moq en Application, Oracle real en Infrastructure, Playwright en E2E)
- [Workflow detallado](xp-tdd-practices/references/workflow-detail.md) — cuándo y cómo consultar al Tech Lead: decisiones de arquitectura, requisitos ambiguos, trade-offs, nuevas dependencias y cambios en el esquema Oracle o contratos de API

### Base de datos
- [Esquema completo](database/database-schema.md)
- [ERD completo](database/database-erd.md)
- [ERD tablas PPT](database/database-erd-ppt.md)
- [Dominios de negocio](database/business-domain.md)
- [Changelog del esquema](database/schema-changelog.md)
