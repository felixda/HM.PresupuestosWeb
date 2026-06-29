## Context

La aplicacion ya registra errores y avisos mediante NLog y mantiene una pantalla de Auditorias para eventos funcionales de usuario. Sin embargo, los logs tecnicos se almacenan hoy en ficheros diarios de texto plano y no existe una interfaz en Admin para que administradores funcionales puedan consultarlos con filtros sin acceso al servidor.

El despliegue previsto es de un solo servidor por entorno, lo que permite una solucion basada en lectura directa de ficheros diarios del propio nodo sin necesidad de agregar multiples fuentes. La nueva capacidad debe respetar la arquitectura hexagonal: la pagina Blazor no puede leer el sistema de archivos directamente, sino a traves de un servicio de aplicacion y un adaptador de infraestructura.

## Goals / Non-Goals

**Goals:**
- Exponer una pagina `/admin/logs` protegida por un permiso especifico bajo el menu Administracion.
- Consultar errores tecnicos y avisos registrados en fichero con filtros por fecha desde/hasta, nivel, usuario y categoria.
- Mostrar resultados en un grid con acceso a detalle ampliado por entrada.
- Persistir cada evento en un formato JSON por linea dentro de un fichero diario para permitir parseo incremental.
- Leer los ficheros del rango solicitado de forma secuencial, sin cargar todos los logs del servidor en memoria.
- Reutilizar patrones ya presentes en Admin, especialmente la estructura de filtros y grid de la pantalla de Auditorias.
- Considerar como categoria el nombre del metodo donde se genera el log.
- Mostrar todos los niveles de log existentes actualmente.
- Permitir visualizar y filtrar entradas sin usuario mediante una opcion explicita `Sin usuario`.

**Non-Goals:**
- Borrar entradas de log individuales desde la UI.
- Consolidar logs de varios servidores o de varios entornos.
- Sustituir la auditoria funcional existente o mezclarla con los logs tecnicos.
- Introducir una base de datos o un sistema externo de observabilidad en esta iteracion.

## Decisions

### D1 — Separar Logs tecnicos de Auditorias funcionales
Se crea una capacidad y una pagina Admin independientes de `Auditorias`. Razon: la auditoria funcional ya tiene semantica de negocio y fuente de datos propia, mientras que los logs tecnicos representan avisos y errores operativos. Alternativa descartada: extender la pagina `Auditorias` para mezclar ambos tipos de registro, lo que complicaria filtros, permisos y lenguaje de negocio.

### D2 — Fichero diario JSON por linea como formato de persistencia inicial
El target de fichero de NLog pasara de texto plano a JSON por linea, manteniendo un fichero por dia. Razon: permite escritura append-only, parseo streaming y compatibilidad con el despliegue actual de un solo servidor por entorno. Alternativa descartada: almacenar en una tabla de base de datos desde la primera version; ofrece mejor consulta, pero aumenta el coste y no es necesario con el escenario operativo actual.

### D3 — Nuevo puerto de lectura de logs en Domain/Application
La lectura de logs se modelara con un puerto secundario dedicado, por ejemplo `ILogTecnicoRepository`, y un servicio/caso de uso asociado en Application. Razon: la capa Web no debe conocer el sistema de ficheros ni el formato NLog. Alternativa descartada: leer ficheros directamente desde la pagina o crear un helper solo en Web, porque romperia la frontera de capas.

### D4 — Parseo incremental y filtros en memoria acotada por rango de fechas
La infraestructura leera solo los ficheros diarios incluidos en el rango de fechas pedido y procesara linea a linea, aplicando filtros sobre el modelo parseado. Razon: el nombre diario del fichero reduce el espacio de busqueda y evita lecturas innecesarias. Alternativa descartada: indexacion previa o precarga completa de todos los logs, porque seria compleja para una primera version.

### D5 — Modelo de entrada de log orientado a consulta, no acoplado a NLog internamente
Se definira una entidad/modelo de dominio para la consulta, con campos como fecha, nivel, categoria, usuario, mensaje, logger, detalle y stack trace. La categoria sera el nombre del metodo origen del log y se persistira como campo explicito en el JSON para no tener que inferirla despues. Razon: desacopla la UI del layout exacto de NLog y deja margen para evolucionar el schema JSON. Alternativa descartada: derivar la categoria desde `logger` o exponer directamente un diccionario dinamico o `JsonDocument` hasta la UI.

### D6 — El filtro de nivel expone todos los niveles actuales
La pantalla mostrara todos los niveles de log existentes en la aplicacion en el momento de la implementacion, junto con la opcion de consultar sin filtrar por nivel. Razon: el usuario ha definido el visor como una herramienta general de consulta de logs tecnicos, no solo de errores y avisos. Alternativa descartada: limitar la pantalla a `Warn` y `Error`, porque dejaria fuera niveles actualmente relevantes.

### D7 — Opcion explicita `Sin usuario` en la consulta
Las entradas sin usuario se mostraran en resultados y el filtro de usuario ofrecera una opcion explicita `Sin usuario` para aislarlas. Razon: algunos logs tecnicos se generan fuera de una sesion autenticada y siguen siendo utiles operativamente. Alternativa descartada: ocultarlas o mostrarlas solo mezcladas sin posibilidad de filtrado.

### D8 — Nuevo `CodigosMenu` y recursos de textos para Admin/Logs
La pagina requerira un nuevo codigo de menu y nuevas claves de traduccion para filtros, columnas, estados vacios y detalle. Razon: sigue el mismo patron que otras paginas administrativas y permite control de acceso consistente. Si HM.CORE aun no expone el menu, se aplicara la misma estrategia temporal que ya existe para otras paginas Admin.

### D9 — Sin nueva accion de auditoria funcional en esta iteracion
La consulta de logs no introduce una nueva `AccionesLog` obligatoria en esta propuesta. Razon: es una capacidad de lectura operativa de ficheros y no requiere, por alcance inicial, ampliar el catalogo de auditoria funcional. Alternativa posible para una iteracion futura: registrar accesos a la pantalla de logs o consultas ejecutadas.

## Risks / Trade-offs

- [Riesgo] Si el schema JSON de NLog cambia sin coordinar el lector, la pantalla dejara de parsear algunas entradas. → Mitigacion: definir un contrato estable de campos minimos, incluyendo categoria como nombre de metodo, y tolerar campos ausentes.
- [Riesgo] Rangos de fechas amplios pueden provocar lecturas costosas de multiples ficheros diarios. → Mitigacion: limitar el rango por defecto en UI y leer solo dias incluidos en el filtro.
- [Riesgo] Exponer demasiado detalle tecnico a administradores funcionales puede generar ruido o fuga de informacion interna. → Mitigacion: mostrar resumen en grid y reservar excepcion/stack trace al panel de detalle.
- [Trade-off] La solucion basada en ficheros funciona bien con un solo servidor, pero no escala de forma natural a multiples instancias. → Aceptado en esta iteracion por el contexto operativo declarado.
- [Trade-off] No permitir borrado individual simplifica la consistencia del fichero, pero reduce capacidades de gestion directa. → Aceptado porque el borrado no es requisito imprescindible.