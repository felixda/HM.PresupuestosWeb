---
name: task-testing-review
description: Revisa la calidad y cobertura de los tests unitarios y E2E. Disparadores: "revisa los tests", "testing review", "comprueba la cobertura", "revisa los tests unitarios".
argument-hint: "[git range]"
context: fork
agent: tests-reviewer
allowed-tools: Read, Glob, Grep, Bash, Edit, Write
---

# Testing Review

Lanza un subagente para revisar la calidad de los tests contra `.github/skills/guidelines/testing-standards/SKILL.md`.

## Scope

Si se proporciona un rango git (p.ej. `abc1234...HEAD`), revisar solo los ficheros de ese rango:
```bash
git diff --name-only <range> -- '*.cs'
```

Si no se proporciona rango, revisar todos los ficheros modificados en el branch actual:
```bash
git diff --name-only origin/master...HEAD -- '*.cs'
```
Separar en ficheros de test (`*.UnitTest/**`, `*.E2ETest/**`) y ficheros de producción.

## What the Agent Does

1. Determinar la lista de ficheros según el scope
2. Revisar nomenclatura: `[Clase]Tests`, `[Metodo]_[Contexto]_[ResultadoEsperado]`
3. Revisar estructura AAA: Arrange / Act / Assert con línea en blanco separadora
4. Revisar principios FIRST: fast, isolated, repeatable, self-validating, timely
5. Revisar política de mocks: `Mock<IXxxRepository>` y `Mock<IYyyService>`, sin mockear tipos de Domain
6. Identificar métodos de producción modificados sin test correspondiente
7. Corregir todos los problemas de calidad encontrados
8. Proponer los tests que faltan para cubrir los gaps identificados

## Output

Tabla de problemas encontrados y correcciones aplicadas, más lista de gaps de cobertura con tests propuestos.
