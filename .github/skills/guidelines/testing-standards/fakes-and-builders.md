# Patrón: Fakes In-Memory y Builders para tests

Objetivo
- Priorizar fakes in-memory (repositorios y servicios) cuando las pruebas verifican cambios de estado, transacciones o persistencia simulada.
- Usar mocks (Moq) únicamente para verificar interacciones o cuando inyectar un fake sería complejo.
- Usar builders para crear entidades y objetos de pruebas en lugar de instanciadores con inicializadores (`new X { ... }`).

Por qué
- Los fakes in-memory permiten pruebas deterministas del estado y de la lógica transaccional sin depender de infraestructuras externas.
- Los builders reducen duplicación y hacen los tests más legibles y fáciles de mantener.

Dónde
- Colocar fakes en `HM.Presupuestos.UnitTest/Fakes/`.
- Colocar builders en `HM.Presupuestos.UnitTest/Builders/`.

Reglas
- Si la prueba valida estado, persistencia, o commit/rollback, crear y usar un fake in-memory.
- Si la prueba solo verifica que se llamó a un método o evento, usar mock(s).
- Evitar mezclar mocks y fakes en la misma prueba salvo que el contexto lo exija.
- No usar inicializadores `new X { ... }` en tests; crear o reusar un builder.
- Los builders deben ser `internal` y ofrecer `WithX()` para parámetros opcionales y `Build()`.
- Los fakes deben exponer helpers `SeedX()` y `ObtenerTransaccion()` cuando soporten transacciones.

Ejemplo breve
- Fake:
  - `HM.Presupuestos.UnitTest/Fakes/InMemoryVersionesRepository.cs` con `SeedVersion()` y `ObtenerTransaccion()`.
- Builder:
  - `HM.Presupuestos.UnitTest/Builders/VersionBuilder.cs` con `WithId()`, `WithNombre()`, `Build()`.

Checklist al escribir un test
- [ ] ¿Verifico estado o transacciones? -> usar Fake In-Memory.
- [ ] ¿Solo verifico interacción? -> usar Mock.
- [ ] ¿Creo objetos con `new X { ... }`? -> refactorizar a Builder.
- [ ] ¿El builder/fake ya existe? -> reusar antes de crear uno nuevo.

Mantenimiento
- Centralizar y consolidar fakes existentes en `HM.Presupuestos.UnitTest/Fakes/`.
- Revisar PRs para asegurar que los tests nuevos usan builders y fakes según este patrón.

Referencias
- `HM.Presupuestos.UnitTest` (carpeta `Fakes/` y `Builders/`)
- Resumen de la sesión: patrón aplicado en múltiples tests y fakes.
