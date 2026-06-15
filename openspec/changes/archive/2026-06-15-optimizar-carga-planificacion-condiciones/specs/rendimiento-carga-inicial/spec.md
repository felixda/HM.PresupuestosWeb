## ADDED Requirements

### Requirement: Carga inicial de maestros en paralelo
El sistema SHALL cargar todos los datos maestros de catálogo necesarios para la inicialización de `PlanificacionCondiciones` de forma paralela, sin esperar el resultado de cada llamada antes de iniciar la siguiente.

#### Scenario: Primera visita (caché fría)
- **WHEN** el usuario navega a la página de planificación de condiciones por primera vez en su sesión
- **THEN** la página MUST obtener los datos maestros (networks, alcances, disciplinas, diversifieds, objetivos, tipos de compra, disciplinas grupo, tipos de disciplina, años con versiones) en paralelo antes de renderizar el contenido

#### Scenario: Visita posterior (caché caliente)
- **WHEN** el usuario navega a la página de planificación de condiciones por segunda vez o más en la misma sesión
- **THEN** los datos maestros cuasi-estáticos SHALL resolverse desde la caché de sesión sin consultar la base de datos

#### Scenario: Comportamiento funcional inalterado
- **WHEN** la página carga con o sin caché
- **THEN** el contenido mostrado y el comportamiento funcional SHALL ser idénticos a los anteriores al cambio
