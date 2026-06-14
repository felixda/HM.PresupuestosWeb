---
name: task-frontend-review
description: Revisa cambios en páginas, componentes o CSS de Blazor contra los patrones del proyecto. Disparadores: "revisa la UI", "frontend review", "revisa los componentes", "comprueba el Blazor".
argument-hint: "[git range]"
context: fork
agent: frontend-reviewer
allowed-tools: Read, Glob, Grep, Bash, Edit, Write
---

# Frontend Review

Lanza un subagente para revisar la UI contra `.github/skills/guidelines/frontend-patterns/SKILL.md`.

## Scope

Si se proporciona un rango git (p.ej. `abc1234...HEAD`), revisar solo los ficheros de ese rango:
```bash
git diff --name-only <range> -- '*.razor' '*.razor.cs' '*.css'
```

Si no se proporciona rango, revisar todos los ficheros modificados en el branch actual:
```bash
git diff --name-only origin/master...HEAD -- '*.razor' '*.razor.cs' '*.css'
```
Filtrar solo ficheros bajo `HM.Presupuestos.Web/`.

## What the Agent Does

1. Determinar la lista de ficheros según el scope
2. Verificar herencia: `ContextProtegido` o `Context`, nunca `ComponentBase`
3. Verificar separación: markup en `.razor`, lógica en `.razor.cs`, sin bloques `@code`
4. Verificar ciclo de vida: sin `OnInitializedAsync`, uso correcto de hooks del framework
5. Verificar inyección: `[Inject]` en `.razor.cs`, solo `IXxxService`, nunca repositorios
6. Verificar `EjecutarAsync` en todas las operaciones async
7. Verificar uso correcto de componentes DevExpress (`DxGrid`, `DxPopup`, `DxFormLayout`...)
8. Corregir todos los problemas encontrados
9. Ejecutar tests para verificar que las correcciones no rompen nada

## Output

Tabla de problemas encontrados y correcciones aplicadas, agrupados por fichero.
