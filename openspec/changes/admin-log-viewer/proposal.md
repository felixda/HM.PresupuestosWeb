## Why

Los administradores funcionales necesitan consultar errores tecnicos y avisos generados por la aplicacion sin depender del acceso directo al servidor ni de soporte tecnico. Actualmente los logs de fichero no tienen una interfaz de consulta en la web y el formato de salida no facilita filtros por fecha, usuario, nivel o categoria.

## What Changes

- Nueva pantalla de administracion para consultar logs tecnicos y avisos con filtros por fecha, usuario, nivel y categoria, siendo la categoria el nombre del metodo origen del log.
- Adaptacion del logging a un formato estructurado JSON por linea en fichero diario para facilitar la lectura desde la aplicacion.
- Nuevo flujo de lectura de logs desde la capa Web a traves de puertos y servicios de aplicacion, sin acceso directo a Infrastructure desde la UI.
- Nuevo permiso de menu para exponer la pantalla dentro del menu padre Administracion.
- Nuevos textos multiidioma para la pantalla, filtros, columnas y mensajes de estado.

## Capabilities

### New Capabilities
- `admin-log-viewer`: Consulta de logs tecnicos y avisos desde Admin con filtros por rango de fechas, usuario, nivel y categoria, incluyendo todos los niveles actuales, opcion explicita de `Sin usuario` y visualizacion de detalle de cada entrada.

### Modified Capabilities
*(ninguna)*

## Impact

- **Domain**: nuevas entidades/modelos de consulta de log y puertos para leer entradas de log estructuradas; nuevo valor en `CodigosMenu`.
- **Application**: nuevo caso de uso para consultar logs filtrados y mapear resultados a la UI.
- **Infrastructure**: adaptador para leer ficheros diarios JSON desde el directorio de logs y parsear entradas linea a linea; ajustes de configuracion NLog.
- **Web**: nueva pagina Admin/Logs con filtros, grid y popup/panel de detalle; nuevos recursos de traduccion.
- **Dependencias/Sistemas**: se reutiliza NLog ya presente; no se introducen sistemas externos ni almacenamiento adicional.
- **Sin breaking changes**: se añade una nueva capacidad administrativa sin alterar los flujos actuales de auditoria.

## No incluido / Fuera de alcance

- Borrado individual de entradas de log desde la interfaz.
- Centralizacion de logs entre multiples servidores o entornos.
- Exportacion a Excel, CSV o integracion con herramientas externas de observabilidad.
- Unificacion con la pantalla existente de Auditorias o cambios en `AccionesLog`.
- Alta definitiva del nuevo menu en HM.CORE si aun no existe; si hace falta, se resolvera temporalmente en la web como en otras paginas Admin.