---
name: task-qa
description: Prueba funcionalmente la aplicación en ejecución contra una spec usando el navegador. Disparadores: "prueba la funcionalidad", "QA", "verifica los flujos", "testa contra la spec".
argument-hint: "[ruta a la spec]"
context: fork
agent: qa-tester
allowed-tools: Read, Glob, Bash, mcp__playwright__browser_navigate, mcp__playwright__browser_snapshot, mcp__playwright__browser_take_screenshot, mcp__playwright__browser_click, mcp__playwright__browser_type, mcp__playwright__browser_wait_for
---

# QA Testing

Lanza un subagente para realizar testing funcional de la aplicación en ejecución contra los criterios de aceptación de una spec.

## Scope

Si se proporciona una ruta a la spec, leer ese fichero:
```
<ruta a la spec>
```

Si no se proporciona, buscar la spec activa en `openspec/changes/`.

> **Prerrequisitos**: aplicación arrancada en `https://localhost:7001` y `sesion_auth.json` generado con `GuardarSesion.ps1`.

## What the Agent Does

1. Leer la spec y extraer requisitos y criterios de aceptación
2. Construir el plan de pruebas (un caso de test por criterio de aceptación)
3. Verificar que la aplicación responde antes de continuar
4. Ejecutar cada caso de test: navegar, interactuar, verificar resultado
5. Probar casos límite: estado vacío, validaciones, errores, transiciones de estado
6. Verificar permisos: acciones restringidas no visibles sin permiso
7. Revisar errores de consola y peticiones de red fallidas
8. Auditar los tests E2E existentes e identificar gaps de cobertura

## Output

Tabla de casos de test con resultado (pass/fail), lista de gaps de cobertura E2E detectados y propuesta de nuevos tests.
