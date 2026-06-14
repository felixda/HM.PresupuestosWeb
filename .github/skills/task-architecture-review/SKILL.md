---
name: task-architecture-review
description: Revisa el cumplimiento de la arquitectura hexagonal en los cambios actuales. Disparadores: "revisa la arquitectura", "verifica las capas", "comprueba dependencias", "architecture review".
argument-hint: "[git range]"
context: fork
agent: architecture-reviewer
allowed-tools: Read, Glob, Grep, Bash, Edit, Write
---

# Architecture Review

Lanza un subagente para revisar el código contra `.github/skills/guidelines/architecture-hexagonal/SKILL.md`.

## Scope

Si se proporciona un rango git (p.ej. `abc1234...HEAD`), revisar solo los ficheros de ese rango:
```bash
git diff --name-only <range> -- '*.cs' '*.razor' '*.razor.cs'
```

Si no se proporciona rango, revisar todos los ficheros modificados en el branch actual:
```bash
git diff --name-only origin/master...HEAD -- '*.cs' '*.razor' '*.razor.cs'
```

## What the Agent Does

1. Determinar la lista de ficheros según el scope
2. Verificar la regla de dependencia: `Web → Application → Domain ← Infrastructure`
3. Verificar responsabilidades por capa (Domain, Application, Infrastructure, Web)
4. Verificar que `IXxxRepository` vive en `Domain/Puertos/Repositorios/`
5. Verificar que las páginas Blazor heredan de `ContextProtegido` o `Context`
6. Corregir todas las violaciones encontradas
7. Ejecutar tests para verificar que las correcciones no rompen nada

## Output

Tabla de violaciones encontradas y correcciones aplicadas, con un resumen del estado de cada capa.
