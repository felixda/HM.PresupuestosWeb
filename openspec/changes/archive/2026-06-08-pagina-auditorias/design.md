## Context

La aplicación dispone de una tabla Oracle `PPT_ACCION_LOG` que registra todas las acciones de los usuarios. Cada registro incluye el tipo de acción codificado en `DES_PROCESO` con el formato `[N] (método) -> descripción`, donde `N` es el valor numérico del enum `AccionesLog`. El campo `PARAMETROS` almacena un JSON con los datos del usuario que realizó la acción.

No existe ninguna página que permita consultar estos registros. La nueva página se añade a la sección Admin, que ya tiene páginas similares de gestión (Avisos, Impersonación, Meses Bloqueados).

## Goals / Non-Goals

**Goals:**
- Página `/admin/auditorias` protegida por permiso `CodigosMenu.Auditorias = 26`
- Filtro con tipo de auditoría (obligatorio), fecha inicio y fecha fin (opcionales)
- Validación en cliente: si se pulsa Buscar sin tipo seleccionado → aviso con `MensajesHelper`
- Grid siempre visible, vacío hasta primera búsqueda
- Botón Limpiar que resetea filtro y grid
- Seguir `guidelines/infrastructure/repositories.md` Patrón 2 (interpolación `$@"..."`) para la query Oracle

**Non-Goals:**
- Alta del menú 26 en HM.CORE (se inyecta temporalmente en `SesionUsuario.cs`)
- Paginación del grid
- Exportación de resultados
- Nuevas acciones de auditoría (`AccionesLog`)

## Decisions

### D1 — Entidad `Auditoria` en Domain, no DTO en Web
La consulta devuelve una proyección de `PPT_ACCION_LOG` adaptada a la UI. Se crea `Auditoria` como entidad de dominio (no DTO) porque encapsula lógica de presentación específica del negocio (extracción del usuario desde JSON). Alternativa descartada: devolver `LogAccion` directamente — acoplaría la UI a la entidad de persistencia.

### D2 — Extracción de usuario en Infrastructure, no en Web
El campo `PARAMETROS` contiene un JSON serializado de `UsuarioEntidad`. La extracción del nombre (`Nombre + Apellido1`) se hace en el repositorio al mapear filas, no en la página. Razón: la página no debe conocer la estructura interna del JSON de parámetros. Si el JSON está vacío o no tiene los campos, se devuelve `"Sin Usuario especificado"`.

### D3 — Patrón 2 de queries (interpolación) para condiciones opcionales
La query tiene cláusulas `AND FECHA_INICIO >= :FechaInicio` y `AND FECHA_INICIO <= :FechaFin` que se añaden o no según los parámetros. Se usa interpolación `$@"...{(cond ? "AND ..." : "")}..."` siguiendo el Patrón 2 de `guidelines/infrastructure/repositories.md`. Se descarta `StringBuilder` (antipatrón documentado en la guideline).

### D4 — `CodigosMenu.Auditorias = 26` + inyección temporal en SesionUsuario
El menú 26 no existe aún en HM.CORE. Se añade `CodigosMenu.Auditorias = 26` al enum y se inyecta el objeto `Menu` temporalmente en `SesionUsuario.CrearUsuarioDesdeRespuestaServicioExterno` con comentario `TODO-TEMPORAL`. Las entradas `Menu_26` se añaden a los JSON de recursos.

### D5 — Módulo LogAcciones como punto de extensión
El nuevo método `ObtenerAuditorias` se añade al módulo existente `LogAcciones` (repositorio, servicio e interfaz) en lugar de crear un módulo nuevo. Razón: la tabla y el dominio son los mismos; crear un módulo `Auditorias` paralelo violaría el principio de cohesión.

## Capas y artefactos afectados

```
Domain
  Compartido/Enumerados.cs              → CodigosMenu.Auditorias = 26
  Entidades/LogAcciones/Auditoria.cs    → nueva entidad (Descripcion, FechaInicio, Usuario)
  Puertos/Repositorios/
    ILogAccionesRepository.cs           → nuevo método ObtenerAuditorias(...)

Application
  CasosDeUso/LogAcciones/
    ILogAccionesService.cs              → nuevo método ObtenerAuditorias(...)
    LogAccionesService.cs               → implementación delegando al repositorio

Infrastructure
  Persistencia/LogAcciones/
    LogAccionesRepository.cs            → implementación con Patrón 2 + ExtraerUsuario

Web
  Pages/Admin/
    Auditorias.razor                    → markup: filtro + grid
    Auditorias.razor.cs                 → code-behind: lógica de búsqueda y limpieza
  Adaptadores/Sesion/SesionUsuario.cs   → inyección temporal Menu_26 (TODO-TEMPORAL)
  wwwroot/data/app.es.json             → Menu_26: "Auditorías"
  wwwroot/data/app.en.json             → Menu_26: "Audits"
  wwwroot/data/app.pt.json             → Menu_26: "Auditorias"
```

## Diseño UI

```
┌─────────────────────────────────────────────────────────────┐
│  Auditorías                                                 │
├─────────────────────────────────────────────────────────────┤
│  FILTRO (DxFormLayout)                                      │
│  ┌──────────────────────┬─────────────┬───────────────────┐│
│  │ * Tipo auditoria     │ Fecha inicio│ Fecha fin         ││
│  │ [DxComboBox        ▼]│ [DxDateEdit]│ [DxDateEdit]      ││
│  └──────────────────────┴─────────────┴───────────────────┘│
│                                      [Limpiar]  [Buscar]   │
├─────────────────────────────────────────────────────────────┤
│  GRID (DxGrid, siempre visible)                             │
│  Descripción              │ Fecha Inicio        │ Usuario   │
│  ─────────────────────────┼─────────────────────┼──────────│
│  [resultado búsqueda...]  │ dd/MM/yyyy HH:mm:ss │ Nom Ape   │
└─────────────────────────────────────────────────────────────┘
```

## Risks / Trade-offs

- **[Riesgo] JSON de PARAMETROS con estructura diferente** → El repositorio usa `try/catch` al parsear; si falla devuelve `"Sin Usuario especificado"` sin propagar el error.
- **[Riesgo] Menú temporal en SesionUsuario** → Requiere eliminación manual cuando HM.CORE provea el menú 26. El comentario `TODO-TEMPORAL` y `// FIN TODO-TEMPORAL` marcan claramente el bloque.
- **[Trade-off] Sin paginación** → Búsquedas con rango de fechas amplio pueden devolver muchos registros. Aceptado para esta versión; se puede añadir en una iteración posterior.

## Open Questions

*(ninguna — todo resuelto en la fase de exploración)*
