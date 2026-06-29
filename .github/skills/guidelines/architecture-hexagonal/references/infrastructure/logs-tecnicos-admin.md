# Guideline: Logs Tecnicos en Admin

> Guia de referencia para el visor de logs tecnicos (`/admin/logs`) y su pipeline de escritura/lectura.
> Aplica a `HM.Presupuestos.Web`, `HM.Presupuestos.Application`, `HM.Presupuestos.Infrastructure` y `HM.Presupuestos.Domain`.

---

## 1. Objetivo

Permitir que administradores consulten logs tecnicos desde la UI sin acceso al servidor, con filtros por fecha, nivel, usuario, categoria y mensaje.

---

## 2. Ubicacion por capas

### Domain

- `Entidades/LogAcciones/LogTecnico.cs`
- `Entidades/LogAcciones/FiltroLogsTecnicos.cs`
- `Entidades/LogAcciones/LogsTecnicosConstantes.cs`
- `Puertos/Repositorios/ILogsTecnicosRepository.cs`

### Application

- `CasosDeUso/LogAcciones/ILogsTecnicosService.cs`
- `CasosDeUso/LogAcciones/LogsTecnicosService.cs`

### Infrastructure

- `Persistencia/LogAcciones/LogsTecnicosRepository.cs`

### Web

- `Pages/Admin/Logs.razor`
- `Pages/Admin/Logs.razor.cs`
- `Pages/Admin/Logs.razor.css`
- `Adaptadores/Auditoria/RegistroAplicacion.cs`
- `nlog.config` y `nlog._DEV_/_PRU_/_PRE_/_PRO_.config`

---

## 3. Formato del fichero

El target `FileLog` escribe un fichero diario JSONL:

- ruta: `${basedir}\\logs\\Presupuestos_yyyy-MM-dd.jsonl`
- una entrada por linea
- rotacion diaria

Campos esperados:

- `timestamp`
- `level`
- `category`
- `login`
- `logger`
- `message`
- `exception`
- `stackTrace`
- `comments`

---

## 4. Reglas clave de NLog

1. `FileLog` debe mantener `JsonLayout` con atributos anteriores.
2. En `FileLog`, usar `encode="true"` para generar JSON valido.
3. Nivel minimo a fichero: `Info` o superior (segun entorno), no limitar a `Warn` si se desea trazabilidad funcional-tecnica.

---

## 5. Lectura y filtros

`LogsTecnicosRepository` debe:

1. Leer ficheros por rango de fechas.
2. Parsear linea a linea de forma tolerante.
3. Aplicar filtros:
   - fecha desde/hasta
   - nivel
   - usuario (incluyendo valor especial "Sin usuario")
   - categoria
   - mensaje (contains, case-insensitive)

Resultado final:

- ordenar por `Fecha` descendente

---

## 6. Compatibilidad con historico

Se detecto un caso real donde algunas lineas se escribieron en pseudo-JSON (valores sin comillas) por configuracion previa de `encode="false"`.

Por eso el repositorio debe mantener fallback tolerante para ese historico, aunque la configuracion actual ya emita JSON valido.

---

## 7. Integracion de pipelines de logging

Para no perder logs generados via `ILogger<>`:

- en `Web/Program.cs` se debe registrar NLog en `builder.Logging` (`AddNLog`) y limpiar providers por defecto si aplica.

Sin esto, pueden existir logs escritos por `RegistroAplicacion` pero no los emitidos por `ILogger<>`.

---

## 8. Uso en UI Admin

Pagina: `/admin/logs`

Flujo recomendado:

1. Seleccionar rango de fechas.
2. Ajustar filtros opcionales (nivel, usuario, categoria, mensaje).
3. Buscar.
4. Abrir detalle tecnico por fila para inspeccionar excepcion/stack/comentarios.

---

## 9. Troubleshooting rapido

### "El fichero tiene muchas entradas pero no aparecen en Admin"

Verificar:

1. Formato JSONL valido (`encode="true"`).
2. Parser tolerante activo en `LogsTecnicosRepository`.
3. Filtros UI no restrictivos (nivel/usuario/categoria/mensaje vacios).
4. Rango de fechas correcto.
5. Aplicacion reiniciada tras cambios de `nlog.config` o `Program.cs`.

### "No aparecen logs de cambio de idioma"

Verificar integracion `ILogger<>` -> NLog en `Program.cs` y que la entrada exista en JSONL con `logger` de `SelectorDeIdioma`.

---

## 10. Regla de evolucion

Cualquier cambio del schema JSON de NLog debe actualizar, en el mismo cambio:

1. escritura (`RegistroAplicacion` / `nlog*.config`)
2. lectura (`LogsTecnicosRepository`)
3. UI (`Logs.razor` / columnas / popup)
4. esta guideline
