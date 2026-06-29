## 1. Domain

- [x] 1.1 Añadir un nuevo valor en `CodigosMenu` para la pagina de logs de administracion
- [x] 1.2 Crear la entidad o modelo de dominio para representar una entrada de log consultable con los campos necesarios para filtros, resumen y detalle
- [x] 1.3 Definir el puerto secundario para consultar logs tecnicos estructurados desde Application

## 2. Application

- [x] 2.1 Crear el caso de uso/interfaz de servicio para consultar logs tecnicos filtrados por rango de fechas, nivel, usuario y categoria
- [x] 2.2 Implementar el servicio de aplicacion delegando en el puerto de lectura de logs y devolviendo el modelo de consulta a la capa Web
- [x] 2.3 Exponer en Application el catalogo de niveles actuales y la opcion funcional `Sin usuario` para que la UI pueda construir los filtros sin acoplarse al formato de infraestructura

## 3. Tests unitarios

- [x] 3.1 Crear tests del servicio de aplicacion para validar el filtrado por fecha, nivel, usuario y categoria
- [x] 3.2 Crear tests para el comportamiento cuando no existan resultados y cuando falten campos opcionales en una entrada de log

## 4. Infrastructure

- [x] 4.1 Ajustar la configuracion de NLog para escribir el fichero diario en formato JSON por linea manteniendo la rotacion diaria existente
- [x] 4.2 Implementar el adaptador de infraestructura que lea los ficheros diarios del rango solicitado y procese las lineas de forma incremental
- [x] 4.3 Implementar el parseo tolerante del JSON de log, mapeando cada linea al modelo de dominio sin fallar por campos ausentes no criticos
- [x] 4.4 Persistir en cada linea JSON la categoria como nombre del metodo origen del log y contemplar entradas sin usuario

## 5. Web — Recursos y menu

- [x] 5.1 Añadir las entradas de recursos multiidioma para el nuevo menu y para todos los textos de la pantalla de logs
- [x] 5.2 Añadir la entrada del nuevo menu de logs bajo Administracion y resolver su exposicion temporal en Web si HM.CORE aun no lo proporciona

## 6. Web — Pagina Admin/Logs

- [x] 6.1 Crear la nueva pagina de administracion con ruta protegida, heredando de `ContextProtegido`
- [x] 6.2 Implementar el formulario de filtros con fecha desde, fecha hasta, nivel, usuario y categoria siguiendo los patrones DevExpress del proyecto
- [x] 6.3 Implementar la consulta con `EjecutarAsync`, mostrando grid de resultados, estado vacio y accion para ver detalle de una entrada
- [x] 6.4 Implementar el popup o panel de detalle mostrando la informacion tecnica ampliada disponible para la entrada seleccionada
- [x] 6.5 Añadir al filtro de usuario la opcion `Sin usuario` y al filtro de nivel todos los niveles actualmente soportados

## 7. Validacion

- [x] 7.1 Compilar la solucion completa sin errores
- [x] 7.2 Ejecutar los tests unitarios del alcance modificado
- [x] 7.3 Ejecutar la validacion del proyecto y revisiones de arquitectura, codigo y frontend aplicables al cambio