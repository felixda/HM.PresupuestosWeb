---
name: task-code-review
description: Revisa la calidad del código C# y Blazor contra los principios de diseño del proyecto. Disparadores: "revisa el código", "code review", "revisa la calidad", "comprueba el diseño".
argument-hint: "[git range]"
context: fork
agent: code-reviewer
allowed-tools: Read, Glob, Grep, Bash, Edit, Write
---

# Code Review

Lanza un subagente para revisar el código contra `.github/skills/guidelines/design-principles/SKILL.md`.

## Scope

Si se proporciona un rango git (p.ej. `abc1234...HEAD`), revisar solo los ficheros de ese rango:
```bash
git diff --name-only <range> -- '*.cs' '*.razor' '*.razor.cs'
```

Si no se proporciona rango, revisar todos los ficheros modificados en el branch actual:
```bash
git diff --name-only origin/master...HEAD -- '*.cs' '*.razor' '*.razor.cs'
```
Excluir tests (`*.UnitTest/**`, `*.E2ETest/**`).

## What the Agent Does

1. Determinar la lista de ficheros según el scope
2. Revisar nomenclatura: PascalCase/camelCase, prefijos `I` y `_`, sufijos correctos
3. Revisar funciones: responsabilidad única, guard clauses, CQS, sin flags booleanos
4. Revisar clases: primary constructor, ámbito mínimo, objetos completos al construir
5. Revisar componentes Blazor: ciclo de vida, separación `.razor`/`.razor.cs`, `EjecutarAsync`
6. Corregir todos los problemas encontrados
7. Ejecutar tests para verificar que las correcciones no rompen nada

## Output

Tabla de problemas encontrados y correcciones aplicadas, agrupados por fichero.
