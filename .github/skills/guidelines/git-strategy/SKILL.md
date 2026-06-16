---
name: git-strategy
description: Esta skill se debe usar al trabajar con git, crear commits, gestionar ramas o el control de versiones. Cubre feature branching, conventional commits y disciplina de commits en TDD.
Triggers: "git", "commit", "branch", "feature branch", "conventional commits".
---

# Estrategia Git

Flujo de trabajo con ramas por funcionalidad, conventional commits y disciplina de commits guiada por TDD. Cada test en verde produce un commit.

> ⚠️ **REGLA CRÍTICA: Nunca se hacen commits directamente sobre `master`.** Toda modificación de código, guideline, configuración o documentación debe realizarse en una rama. `master` solo recibe cambios a través de Pull Requests o merges desde ramas. Esta regla no tiene excepciones.

## Modelo de Ramas

### Ramas por Funcionalidad (Feature Branching)

Todo el trabajo ocurre en ramas creadas desde `master`.

**Excepción — User Stories de Azure DevOps:** cuando el punto de partida es un US en Azure DevOps, la rama se crea desde `master` con el convenio `feat/us-<ID>-<descripcion>`. Ver el flujo completo en [guidelines/azure-devops/SKILL.md](../azure-devops/SKILL.md).

```
develop
 └── feat/add-user-registration
 └── fix/login-validation-error
 └── refactor/extract-payment-module

master
 └── feat/us-1234-auditoria-estadisticas   ← US de DevOps
 └── feat/us-5678-filtro-condiciones       ← US de DevOps
```

### Nomenclatura de Ramas

Formato: `<tipo>/<descripcion-corta>`

| Tipo | Uso |
|------|-------|
| `feat` | Nueva funcionalidad |
| `fix` | Correción de bug |
| `refactor` | Reestructuración de código sin cambio de comportamiento |
| `chore` | Herramientas, configuración, dependencias |
| `docs` | Solo documentación |
| `test` | Añadir o corregir tests |

Reglas:
- Minúsculas, solo guiones (sin guiones bajos, sin camelCase)
- Máximo 50 caracteres en la parte descriptiva
- Descriptivo pero conciso: `feat/user-login`, no `feat/add-the-new-login-feature-for-users`

### Ciclo de Vida de una Rama

1. Crear rama desde `master`
2. Desarrollar con commits TDD (ver más abajo)
3. Push y abrir PR
4. Merge a `master` (squash o merge commit según preferencia del equipo)
5. Eliminar la rama tras el merge

## Conventional Commits

Cada mensaje de commit sigue la especificación [Conventional Commits](https://www.conventionalcommits.org/).

### Formato

```
<tipo>(<ámbito>): <descripción>
```

- **tipo**: Obligatorio. Ver tabla más abajo.
- **ámbito**: Opcional. Módulo de negocio o componente afectado.
- **descripción**: Obligatoria. Imperativo, minúsculas, sin punto final. Máximo 50 caracteres.

### Tipos

| Tipo | Cuándo usarlo |
|------|---------------|
| `feat` | Nueva funcionalidad visible para el usuario |
| `fix` | Corrección de bug |
| `refactor` | Cambio de código que no corrige un bug ni añade funcionalidad |
| `test` | Añadir o corregir tests |
| `chore` | Cambios de build, configuración o herramientas |
| `docs` | Cambios en documentación |
| `style` | Formato, espacios en blanco (sin cambio lógico) |

### Reglas de Concisión

Los mensajes de commit deben ser extremadamente concisos:
- Máximo 50 caracteres en la descripción
- Sin artículos innecesarios
- Sin palabras de relleno
- Empezar con verbo en imperativo: `add`, `fix`, `remove`, `extract`, `rename`
- Describir el qué, no el cómo

### Ejemplos correctos

```
feat(user): add registration endpoint
fix(auth): validate token expiry
refactor(payment): extract discount calculator
test(order): add empty cart case
chore: update typescript to 5.4
```

### Ejemplos incorrectos (evitar)

```
feat: Added the new user registration feature    # tiempo pasado, demasiado largo
fix: fixing a bug in the login                   # gerundio, vago
update code                                      # sin tipo, vago
refactor(payment): refactor payment module       # redundante con el tipo
```

## Disciplina de Commits en TDD

Hacer commit en cada test en verde. Esto crea un historial granular y reversible.

### Flujo de Commits durante TDD

```
RED      → escribir test fallido      → sin commit
GREEN    → hacer pasar el test        → COMMIT
REFACTOR → mejorar el código          → COMMIT (si hay cambios)
```

### Tipos de Commit por Fase TDD

| Fase TDD | Tipo de commit | Ejemplo |
|----------|----------------|---------|
| GREEN (primer test) | `feat` o `fix` | `feat(user): add name validation` |
| GREEN (caso adicional) | `test` | `test(user): add empty name case` |
| REFACTOR | `refactor` | `refactor(user): extract validator` |

### Guía de decisión para el tipo

- Primer test en verde que introduce comportamiento nuevo → `feat`
- Casos de test adicionales para el mismo comportamiento → `test`
- Test en verde que corrige un bug → `fix`
- Refactorización tras verde → `refactor`
- Añadir infraestructura de tests o helpers → `test`

### Ejemplo de flujo

```
1. test(order): add create order case           # primer verde
2. refactor(order): extract price calculator    # refactor
3. test(order): add discount case               # segundo verde
4. test(order): add zero quantity case          # tercer verde
5. refactor(order): simplify discount logic     # refactor
6. feat(order): add order repository port       # nuevo comportamiento
```

## Reglas No Negociables

- Nunca hacer commit en rojo (tests fallando)
- Nunca omitir un commit tras verde — cada test que pasa recibe un commit
- Nunca escribir mensajes de commit largos — máximo 50 caracteres en la descripción
- Nunca usar tiempo pasado o gerundio en las descripciones de commit
- Nunca hacer commit directamente en `develop` o `main` — usar siempre ramas
- Usar siempre el formato de conventional commits
- Crear siempre las ramas desde `develop`
- Usar siempre imperativo en las descripciones de commit
- Incluir siempre el tipo; incluir el ámbito cuando el cambio está acotado a un módulo