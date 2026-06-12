## ADDED Requirements

### Requirement: Panel de estadísticas en la página de Auditorías
El sistema SHALL mostrar un panel colapsable con métricas agregadas del período y tipo seleccionados, cargado simultáneamente al grid de detalle al pulsar Buscar.

#### Scenario: Panel visible tras búsqueda
- **WHEN** el administrador pulsa Buscar con tipo y fechas informados
- **THEN** el panel de resumen aparece expandido con las métricas calculadas para ese mismo filtro

#### Scenario: Panel no visible antes de la primera búsqueda
- **WHEN** el administrador accede a la página sin haber pulsado Buscar
- **THEN** el panel de resumen no muestra datos

#### Scenario: Panel colapsable
- **WHEN** el administrador pulsa el botón de colapsar del panel
- **THEN** el panel se contrae ocultando las métricas sin afectar al grid

---

### Requirement: Métricas generales del período
El sistema SHALL mostrar en el panel: total de acciones, número de usuarios únicos, usuario más activo (login + total) y página más visitada (solo cuando el tipo es AccesoAPagina).

#### Scenario: Métricas con tipo AccesoAPagina
- **WHEN** el tipo seleccionado es AccesoAPagina y se realiza la búsqueda
- **THEN** el panel muestra total de acciones, usuarios únicos, usuario más activo y página más visitada

#### Scenario: Métricas con tipo distinto de AccesoAPagina
- **WHEN** el tipo seleccionado es diferente de AccesoAPagina y se realiza la búsqueda
- **THEN** el panel muestra total de acciones, usuarios únicos y usuario más activo, pero NO muestra la métrica de página más visitada

#### Scenario: Sin resultados en el período
- **WHEN** no hay registros para el filtro seleccionado
- **THEN** el panel muestra ceros en todas las métricas sin errores

---

### Requirement: Top 5 usuarios más activos
El sistema SHALL mostrar una lista de los 5 usuarios con más acciones del período filtrado, con una barra de progreso proporcional al máximo.

#### Scenario: Lista top usuarios visible
- **WHEN** hay al menos un resultado en el período filtrado
- **THEN** el panel muestra hasta 5 filas con login de usuario, total de acciones y barra de progreso `DxProgressBar` proporcional al usuario con más acciones

#### Scenario: Menos de 5 usuarios en el período
- **WHEN** el número de usuarios únicos en el período es menor de 5
- **THEN** el panel muestra solo los usuarios disponibles sin filas vacías

---

### Requirement: Gráfica de actividad diaria (sparkline)
El sistema SHALL mostrar una gráfica de barras compacta (`DxSparkline`) con la distribución de acciones por día cuando el rango de fechas no supera los 90 días.

#### Scenario: Sparkline visible con rango ≤ 90 días
- **WHEN** la diferencia entre FechaFin y FechaInicio es menor o igual a 90 días
- **THEN** el panel muestra un `DxSparkline` de tipo Bar con un punto por día del rango

#### Scenario: Sparkline oculta con rango > 90 días
- **WHEN** la diferencia entre FechaFin y FechaInicio supera los 90 días
- **THEN** el panel no muestra la sparkline y muestra un texto informativo indicando que el rango es demasiado amplio para la vista diaria

---

### Requirement: Fechas obligatorias con valor por defecto hoy
El sistema SHALL inicializar los filtros FechaInicio y FechaFin con la fecha actual y SHALL requerir ambas fechas para poder ejecutar la búsqueda.

#### Scenario: Fechas inicializadas al cargar la página
- **WHEN** el administrador accede a la página de Auditorías
- **THEN** los campos FechaInicio y FechaFin muestran la fecha de hoy

#### Scenario: Buscar sin fechas no es posible
- **WHEN** el administrador elimina una de las fechas e intenta pulsar Buscar
- **THEN** el sistema muestra un aviso indicando que las fechas son obligatorias y no ejecuta la consulta
