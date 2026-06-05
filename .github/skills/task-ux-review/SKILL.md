---
name: task-ux-review
description: Revisión visual de UX de la aplicación en ejecución usando el navegador. Disparadores: "revisa la UX", "UX review", "evalúa la interfaz", "revisa el diseño visual".
argument-hint: "[ruta relativa de la página]"
context: fork
agent: ux-reviewer
allowed-tools: Read, Glob, Bash, mcp__playwright__browser_navigate, mcp__playwright__browser_snapshot, mcp__playwright__browser_take_screenshot, mcp__playwright__browser_click, mcp__playwright__browser_wait_for, mcp__playwright__browser_resize
---

# UX Review

Lanza un subagente para evaluar visualmente la UI navegando la aplicación en ejecución.

## Scope

Si se proporciona una ruta relativa (p.ej. `/condiciones`), revisar solo esa página.

Si no se proporciona, revisar todas las páginas modificadas en el branch actual:
```bash
git diff --name-only origin/master...HEAD -- '*.razor'
```

> **Prerrequisitos**: aplicación arrancada en `https://localhost:7001` y `sesion_auth.json` generado con `GuardarSesion.ps1`.

## What the Agent Does

1. Navegar a cada página y capturar screenshot en estado inicial
2. Evaluar consistencia visual: tipografía, colores, espaciado con el tema DevExpress
3. Evaluar feedback de estado: loading overlay, mensajes de error, confirmaciones, estado vacío
4. Evaluar formularios: etiquetas, validaciones inline, orden de tabulación
5. Evaluar grids DevExpress: columnas, paginación, filtros, acciones de fila
6. Evaluar popups: apertura, cierre, foco, scroll interno
7. Probar comportamiento responsive: desktop y tablet (1024px)
8. Revisar accesibilidad básica: contraste, navegación por teclado

## Output

Screenshots por pantalla y tabla de hallazgos UX con severidad (bloqueante / mejora / sugerencia).
