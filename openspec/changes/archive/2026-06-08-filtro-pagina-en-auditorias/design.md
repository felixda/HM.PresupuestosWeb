## Context

La página de Auditorías ya dispone de filtro por tipo de acción y rango de fechas. El tipo `AccesoAPagina` (29) es el más frecuente y necesita un filtro adicional por página concreta. El texto grabado en BD para este tipo tiene actualmente el prefijo método antes del prefijo numérico, lo que impide que el patrón `[29]%` de la query lo encuentre.

La lista de páginas navegables ya existe en los JSON de menú (`app.es.json`, `app.en.json`, `app.pt.json`), respetando el idioma activo. `IMapaMenu` ya lee esos JSON, por lo que es el punto natural donde añadir el nuevo método.

## Goals / Non-Goals

**Goals:**
- Corregir el formato de grabación de AccesoAPagina para que el filtrado por tipo funcione
- Exponer un combo "Página" en el filtro, condicionado al tipo 29
- Filtrar la query por `[codigoPagina]` al final del texto si se selecciona página
- Limpiar el nombre del método llamador de la columna Descripción para todos los tipos

**Non-Goals:**
- Cambiar el formato de grabación para otros tipos de acción
- Migrar registros históricos en BD
- Añadir filtros por página para otros tipos de acción

## Decisions

### D1 — Nuevo método en IMapaMenu en lugar de leer el JSON desde el page

**Decisión**: Añadir `List<CodigoDescripcion> ObtenerPaginasNavegables()` a `IMapaMenu` / `MapaMenu`.

**Alternativa descartada**: Inyectar `IProveedorRecursosJson` directamente en el code-behind de la página y parsear el JSON allí.

**Rationale**: El code-behind no debe conocer la estructura del JSON de menú. `MapaMenu` ya encapsula ese conocimiento y ya tiene acceso al idioma activo. Añadir el método ahí mantiene la cohesión y no viola capas (Web → Web.Adaptadores).

---

### D2 — `codigoPagina` como `int?` opcional en la firma de `ObtenerAuditorias`

**Decisión**: Añadir `int? codigoPagina = null` al final de la firma en puerto, servicio y repositorio.

**Alternativa descartada**: Crear una sobrecarga separada.

**Rationale**: La consulta es la misma con una cláusula opcional adicional. Añadir el parámetro con valor por defecto `null` no rompe los llamadores existentes y evita duplicación.

---

### D3 — Patrón de búsqueda `LIKE '%[codigoPagina]'`

**Decisión**: Usar `AND DES_PROCESO LIKE :PatronPagina` con valor `%[{codigoPagina}]`.

**Rationale**: En Oracle `[` y `]` son literales en `LIKE` (no wildcards), lo que garantiza coincidencia exacta del código de menú. El patrón `%[26]` no matchea `%[261]`.

---

### D4 — `QuitarPrefijoAccion` limpia `[N](Method) -> ` para todos los tipos

**Decisión**: Buscar `-> ` en el texto y devolver lo que hay después. Si no existe el separador, aplicar el comportamiento anterior (quitar hasta `]`).

**Rationale**: Todos los tipos grabados via `Insertar(AccionesLog)` usan el patrón `[N] (Method) -> descripción`. Limpiar solo hasta `]` deja visible el nombre del método en la columna Descripción. La búsqueda de `-> ` es más robusta y funciona para el nuevo formato del tipo 29 también.

## Risks / Trade-offs

- **Registros históricos**: Los logs del tipo 29 grabados antes de este cambio tienen el formato `(RegistrarAccesoAPagina) -> [29] ...`. No serán encontrados por el filtro de tipo 29 (el patrón `[29]%` no casa). Riesgo bajo: son datos de auditoría histórica, no datos de negocio. Sin mitigación activa.

- **`QuitarPrefijoAccion` más agresivo**: Si algún texto de descripción legítimo contiene `-> `, se truncará. Riesgo mínimo dado que el separador `->` es un artefacto del framework de logging, no parte del texto de negocio.

## Migration Plan

No requiere migración de datos ni cambios de esquema de BD. El cambio de formato es forward-only. Despliegue directo sin rollback especial.

## Open Questions

_(ninguna — todas resueltas en explore)_
