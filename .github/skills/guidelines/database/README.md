# Documentación del Esquema Oracle — PRESUPUESTOS

Archivos generados automáticamente a partir del esquema Oracle del schema `PRESUPUESTOS`.

## Archivos disponibles

| Archivo | Descripción |
|---|---|
| [database-schema.md](database-schema.md) | Esquema completo: todas las tablas con columnas, tipos, PKs, FKs e índices |
| [business-domain.md](business-domain.md) | Tablas agrupadas por dominio funcional (prefijo de nombre) |
| [database-erd.md](database-erd.md) | Diagrama ERD completo en formato Mermaid |
| [database-erd-ppt.md](database-erd-ppt.md) | Diagrama ERD reducido — solo tablas `PPT` (visualizable en VS Code) |
| [schema-changelog.md](schema-changelog.md) | Historial de cambios detectados entre ejecuciones |
| [schema-version.json](schema-version.json) | Hash del esquema actual + timestamp de última comprobación |

> Para ver los diagramas Mermaid en VS Code: abre el archivo `.md` y pulsa `Ctrl+Shift+V`.

---

## Scripts PowerShell

### `Sync-OracleSchema.ps1` — Sincronización completa

Extrae el esquema Oracle completo y regenera todos los archivos de documentación si detecta cambios.

```powershell
# Configurar la cadena de conexión
$env:ORACLE_CONNECTION_STRING = "user id=presupuestos;password=...;data source=IBEDESA19c;Pooling=True;Min Pool Size=0;"

# Ejecución normal (solo regenera si hay cambios)
.\Sync-OracleSchema.ps1

# Forzar regeneración aunque no haya cambios en el esquema
.\Sync-OracleSchema.ps1 -Force

# Carpeta de salida personalizada (por defecto: .github/skills/guidelines/database/)
.\Sync-OracleSchema.ps1 -SchemaPath "ruta/personalizada"

#ejemplo de ejecucion normal
$env:ORACLE_CONNECTION_STRING = "user id=MMS_DESA_IBERIA;password=...;data source=IBEDESA19c;Pooling=True;Min Pool Size=0;"
.\Sync-OracleSchema.ps1
```

**Comportamiento:**
- Si el hash del esquema **no ha cambiado** → actualiza solo el timestamp en `schema-version.json` y termina sin tocar los `.md`
- Si el hash **ha cambiado** → regenera `database-schema.md`, `business-domain.md`, `database-erd.md` y añade una entrada a `schema-changelog.md`

**Variables de entorno admitidas:**

| Variable | Descripción |
|---|---|
| `ORACLE_CONNECTION_STRING` | Cadena de conexión completa (preferida) |
| `ORACLE_DATA_SOURCE` | Host:puerto/servicio (alternativa) |
| `ORACLE_USER_ID` | Usuario Oracle (alternativa) |
| `ORACLE_PASSWORD` | Contraseña Oracle (alternativa) |
| `ORACLE_SCHEMA` | Schema a documentar (por defecto: el usuario conectado) |
| `ORACLE_DLL_PATH` | Ruta al `Oracle.ManagedDataAccess.dll` (opcional) |

---

### `Generate-ErdPpt.ps1` — ERD reducido de tablas PPT

Genera el archivo `database-erd-ppt.md` con el diagrama ERD limitado a las tablas con prefijo `PPT`. Más ligero y visualizable directamente en VS Code.

```powershell
# Con cadena de conexión por defecto (presupuestos@IBEDESA19c)
.\Generate-ErdPpt.ps1

# Con cadena de conexión personalizada
.\Generate-ErdPpt.ps1 -ConnStr "user id=presupuestos;password=...;data source=IBEDESA19c;Pooling=True;Min Pool Size=0;"
```

---

## Automatización (tarea programada / CI)

### Windows Task Scheduler

```powershell
$action  = New-ScheduledTaskAction -Execute "pwsh.exe" `
    -Argument "-NonInteractive -File C:\GitHub\ESFNN-PresupuestosWeb\Sync-OracleSchema.ps1" `
    -WorkingDirectory "C:\GitHub\ESFNN-PresupuestosWeb"

$trigger = New-ScheduledTaskTrigger -Daily -At "07:00"

$env_var = [System.Environment]::SetEnvironmentVariable(
    "ORACLE_CONNECTION_STRING",
    "user id=presupuestos;password=...;data source=IBEDESA19c;Pooling=True;Min Pool Size=0;",
    "Machine"
)

Register-ScheduledTask -TaskName "SyncOracleSchema" -Action $action -Trigger $trigger -RunLevel Highest
```

### Pipeline CI/CD (Azure DevOps / GitHub Actions)

```yaml
# Azure DevOps
- task: PowerShell@2
  displayName: Sincronizar esquema Oracle
  inputs:
    filePath: Sync-OracleSchema.ps1
    pwsh: true
  env:
    ORACLE_CONNECTION_STRING: $(ORACLE_CONNECTION_STRING)  # variable secreta del pipeline
```

```yaml
# GitHub Actions
- name: Sincronizar esquema Oracle
  shell: pwsh
  run: .\Sync-OracleSchema.ps1
  env:
    ORACLE_CONNECTION_STRING: ${{ secrets.ORACLE_CONNECTION_STRING }}
```
