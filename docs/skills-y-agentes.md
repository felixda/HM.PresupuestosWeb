# Skills y agentes definidos en la solución

Este documento resume, en forma de esquema, las skills y los agentes disponibles en el repositorio para HM.Presupuestos. El objetivo es tener una referencia rápida sobre para qué sirve cada uno y cuándo usarlo.

## Skills

- **action-refactor**: refactoriza código, tests o renombra artefactos siguiendo los principios de diseño del proyecto. Se usa cuando hay que limpiar, mejorar o renombrar algo ya existente.
- **action-tdd**: corrige el flujo de trabajo cuando se está saltando el ciclo TDD. Sirve para volver a un ciclo red-green-refactor o aplicar TPP.
- **openspec-apply-change**: implementa tareas de un cambio OpenSpec ya definido. Se usa cuando el cambio ya tiene proposal, design y tasks y toca ejecutar.
- **openspec-archive-change**: archiva un cambio OpenSpec completado. Sirve para cerrar formalmente el trabajo una vez terminado.
- **openspec-explore**: modo exploración para pensar, investigar y aclarar requisitos antes de implementar. Se usa para analizar opciones y contexto sin escribir código.
- **openspec-propose**: crea un nuevo cambio OpenSpec generando propuesta, diseño y tareas en una sola pasada. Sirve para arrancar un trabajo nuevo de forma ordenada.
- **task-architecture-review**: revisa si los cambios respetan la arquitectura hexagonal. Se usa para validar fronteras de capa y dependencias.
- **task-code-review**: revisa la calidad del código C# y Blazor frente a los principios del proyecto. Sirve para detectar problemas de diseño, legibilidad o mantenibilidad.
- **task-frontend-review**: revisa páginas, componentes y CSS de Blazor contra los patrones visuales y técnicos del proyecto. Se usa cuando hay cambios en la interfaz.
- **task-qa**: prueba la funcionalidad en ejecución con navegador contra una spec. Sirve para validar flujos reales de usuario.
- **task-testing-review**: revisa la calidad y cobertura de los tests unitarios y E2E. Se usa para detectar huecos de cobertura o tests poco útiles.
- **task-ux-review**: hace revisión visual de UX sobre la aplicación en ejecución. Sirve para evaluar claridad, consistencia y experiencia de uso.
- **task-validate**: compila la solución y ejecuta todos los tests unitarios. Se usa como validación global del estado del proyecto.

## Agentes

- **architecture-reviewer**: revisa el cumplimiento de arquitectura hexagonal en los cambios actuales.
- **code-reviewer**: revisa el código .NET y Blazor contra los principios de diseño del proyecto.
- **env-switcher**: cambia los user-secrets al entorno indicado, como DEV, PRU, PRE o PRO.
- **frontend-reviewer**: revisa patrones de UI en páginas, componentes o CSS de Blazor y DevExpress.
- **project-validator**: compila la solución y ejecuta todos los tests para validar el proyecto completo.
- **qa-tester**: prueba funcionalmente la aplicación con Playwright y NUnit contra las specs.
- **repositories-reviewer**: revisa queries SQL de Infrastructure para comprobar que siguen los patrones del proyecto.
- **tests-reviewer**: revisa calidad y cobertura de tests unitarios y detecta huecos de validación.
- **ux-reviewer**: revisa visualmente la UX de la aplicación contra las buenas prácticas del proyecto.

## Guidelines

- **architecture-hexagonal**: explica la estructura por capas, reglas de dependencia y organización modular de la solución.
- **azure-devops**: recoge convenciones para work items, pipelines y gestión de ramas en Azure DevOps.
- **database**: define normas para Oracle, migraciones y nomenclatura de objetos de base de datos.
- **design-principles**: concentra las reglas de nomenclatura, diseño de clases y criterios generales de calidad del código.
- **frontend-patterns**: documenta los patrones de Blazor, code-behind, ciclo de vida y convenciones de UI.
- **git-strategy**: describe la estrategia de ramas, commits y flujo de trabajo con Git.
- **testing-standards**: establece las reglas para escribir tests, su estructura y su nomenclatura.
- **xp-tdd-practices**: resume el ciclo TDD, TPP e implementación inside-out en el proyecto.

## Resumen rápido de uso

- Para implementar o corregir código: `action-refactor`, `action-tdd` o `openspec-apply-change`.
- Para iniciar o cerrar cambios OpenSpec: `openspec-propose` y `openspec-archive-change`.
- Para revisión técnica: `task-architecture-review`, `task-code-review`, `task-testing-review` y `repositories-reviewer`.
- Para revisión de producto: `task-frontend-review`, `task-ux-review` y `task-qa`.
- Para validación global: `task-validate` o `project-validator`.
- Para consultar reglas de trabajo y desarrollo: revisa las guidelines según el área afectada.

## Nota

Las skills son flujos o comandos reutilizables, mientras que los agentes están pensados para ejecutar revisiones o tareas especializadas con más contexto y autonomía.
