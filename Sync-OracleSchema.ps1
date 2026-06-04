#Requires -Version 7.0
<#
.SYNOPSIS
    Sincroniza la documentación del esquema Oracle con el estado actual de la base de datos.

.DESCRIPTION
    Conecta a Oracle usando variables de entorno, extrae el esquema completo (tablas, vistas,
    columnas, PKs, FKs, índices, comentarios), calcula un hash SHA-256 y actualiza los archivos
    de documentación en .github/skills/guidelines/database/ únicamente cuando se detectan cambios.
    Es idempotente: si el hash no cambia, solo actualiza el timestamp de la última comprobación.

.PARAMETER SchemaPath
    Ruta a la carpeta de destino para los archivos de documentación.
    Por defecto: .specs/skills relativo al directorio del script.

.PARAMETER Force
    Fuerza la regeneración completa aunque el hash no haya cambiado.

.ENVIRONMENT VARIABLES
    ORACLE_CONNECTION_STRING   Cadena de conexión completa (preferida).
    ORACLE_DATA_SOURCE         TNS alias o host:port/service (alternativa).
    ORACLE_USER_ID             Usuario Oracle (alternativa).
    ORACLE_PASSWORD            Contraseña Oracle (alternativa).
    ORACLE_SCHEMA              Schema a documentar (por defecto: usuario ORACLE_USER_ID).
    ORACLE_DLL_PATH            Ruta absoluta al Oracle.ManagedDataAccess.dll (opcional).

.EXAMPLE
    # Usando cadena de conexión completa
    $env:ORACLE_CONNECTION_STRING = "Data Source=host:1521/ORCL;User Id=myuser;Password=mypass"
    .\Sync-OracleSchema.ps1

    # Usando variables individuales
    $env:ORACLE_DATA_SOURCE = "host:1521/ORCL"
    $env:ORACLE_USER_ID     = "myuser"
    $env:ORACLE_PASSWORD    = "mypass"
    .\Sync-OracleSchema.ps1

    # Forzar regeneración
    .\Sync-OracleSchema.ps1 -Force
#>

[CmdletBinding()]
param(
    [string]$SchemaPath = (Join-Path $PSScriptRoot ".github/skills/guidelines/database"),
    [switch]$Force
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# ─────────────────────────────────────────────────────────────────────────────
# REGIÓN: Carga del driver Oracle
# ─────────────────────────────────────────────────────────────────────────────

function Find-OracleDll {
    # 1. Variable de entorno explícita
    if ($env:ORACLE_DLL_PATH -and (Test-Path $env:ORACLE_DLL_PATH)) {
        return $env:ORACLE_DLL_PATH
    }

    # 2. Bin del proyecto Infrastructure (si está compilado)
    $infraBins = @(
        (Join-Path $PSScriptRoot "HM.Presupuestos.Infrastructure/bin/Debug/net10.0/Oracle.ManagedDataAccess.dll"),
        (Join-Path $PSScriptRoot "HM.Presupuestos.Infrastructure/bin/Release/net10.0/Oracle.ManagedDataAccess.dll")
    )
    foreach ($path in $infraBins) {
        if (Test-Path $path) { return $path }
    }

    # 3. Caché NuGet del usuario
    $nugetVersions = @("2.19.200", "2.19.100", "2.18.3", "23.4.0")
    $nugetBase = Join-Path $env:USERPROFILE ".nuget/packages/oracle.manageddataaccess.core"
    foreach ($ver in $nugetVersions) {
        $candidate = Join-Path $nugetBase "$ver/lib/netstandard2.0/Oracle.ManagedDataAccess.dll"
        if (Test-Path $candidate) { return $candidate }
    }

    throw @"
No se encontró Oracle.ManagedDataAccess.dll.
Opciones:
  1. Compilar el proyecto: dotnet build HM.Presupuestos.Infrastructure
  2. Definir la variable de entorno ORACLE_DLL_PATH con la ruta al DLL
  3. Instalar el paquete NuGet: dotnet add package Oracle.ManagedDataAccess.Core
"@
}

# ─────────────────────────────────────────────────────────────────────────────
# REGIÓN: Construcción de la cadena de conexión
# ─────────────────────────────────────────────────────────────────────────────

function Get-ConnectionString {
    if ($env:ORACLE_CONNECTION_STRING) {
        return $env:ORACLE_CONNECTION_STRING
    }

    $dataSource = $env:ORACLE_DATA_SOURCE
    $userId     = $env:ORACLE_USER_ID
    $password   = $env:ORACLE_PASSWORD

    if (-not $dataSource -or -not $userId -or -not $password) {
        throw @"
Variables de entorno de conexión no encontradas.
Defina ORACLE_CONNECTION_STRING  o  ORACLE_DATA_SOURCE + ORACLE_USER_ID + ORACLE_PASSWORD.
"@
    }

    return "Data Source=$dataSource;User Id=$userId;Password=$password"
}

# ─────────────────────────────────────────────────────────────────────────────
# REGIÓN: Consultas Oracle
# ─────────────────────────────────────────────────────────────────────────────

function Invoke-OracleQuery {
    param(
        [Oracle.ManagedDataAccess.Client.OracleConnection]$Connection,
        [string]$Sql,
        [hashtable]$Parameters = @{}
    )

    $cmd = $Connection.CreateCommand()
    $cmd.CommandText = $Sql
    foreach ($key in $Parameters.Keys) {
        $null = $cmd.Parameters.Add(
            [Oracle.ManagedDataAccess.Client.OracleParameter]::new($key, $Parameters[$key])
        )
    }

    $reader = $cmd.ExecuteReader()
    $results = [System.Collections.Generic.List[hashtable]]::new()

    while ($reader.Read()) {
        $row = @{}
        for ($i = 0; $i -lt $reader.FieldCount; $i++) {
            $row[$reader.GetName($i)] = if ($reader.IsDBNull($i)) { $null } else { $reader.GetValue($i) }
        }
        $results.Add($row)
    }

    $reader.Close()
    $cmd.Dispose()
    return $results
}

function Get-CurrentSchema {
    param([Oracle.ManagedDataAccess.Client.OracleConnection]$Connection)

    if ($env:ORACLE_SCHEMA) { return $env:ORACLE_SCHEMA.ToUpper() }

    # @() fuerza el envoltorio como array: evita que PowerShell "desenrolle"
    # una List<hashtable> de una sola fila y la entregue como hashtable directo,
    # lo que haría que $result[0] devolviera $null al indexar por entero.
    $rows = @(Invoke-OracleQuery -Connection $Connection `
        -Sql "SELECT SYS_CONTEXT('USERENV','SESSION_USER') AS CURRENT_SCHEMA FROM DUAL")
    return $rows[0]['CURRENT_SCHEMA']
}

function Get-OracleSchema {
    param(
        [Oracle.ManagedDataAccess.Client.OracleConnection]$Connection,
        [string]$Owner
    )

    Write-Host "  Consultando tablas..." -ForegroundColor Gray
    $tables = Invoke-OracleQuery -Connection $Connection -Sql @"
SELECT t.TABLE_NAME, NVL(c.COMMENTS, '') AS COMMENTS
FROM   ALL_TABLES t
LEFT JOIN ALL_TAB_COMMENTS c ON t.TABLE_NAME = c.TABLE_NAME AND t.OWNER = c.OWNER
WHERE  t.OWNER = :owner
ORDER  BY t.TABLE_NAME
"@ -Parameters @{ owner = $Owner }

    Write-Host "  Consultando vistas..." -ForegroundColor Gray
    $views = Invoke-OracleQuery -Connection $Connection -Sql @"
SELECT VIEW_NAME
FROM   ALL_VIEWS
WHERE  OWNER = :owner
ORDER  BY VIEW_NAME
"@ -Parameters @{ owner = $Owner }

    Write-Host "  Consultando columnas..." -ForegroundColor Gray
    $columns = Invoke-OracleQuery -Connection $Connection -Sql @"
SELECT c.TABLE_NAME, c.COLUMN_NAME, c.DATA_TYPE, c.DATA_LENGTH, c.DATA_PRECISION,
       c.DATA_SCALE, c.NULLABLE, c.COLUMN_ID, NVL(cc.COMMENTS, '') AS COMMENTS
FROM   ALL_TAB_COLUMNS c
LEFT JOIN ALL_COL_COMMENTS cc
       ON c.TABLE_NAME = cc.TABLE_NAME AND c.COLUMN_NAME = cc.COLUMN_NAME AND c.OWNER = cc.OWNER
WHERE  c.OWNER = :owner
ORDER  BY c.TABLE_NAME, c.COLUMN_ID
"@ -Parameters @{ owner = $Owner }

    Write-Host "  Consultando claves primarias..." -ForegroundColor Gray
    $primaryKeys = Invoke-OracleQuery -Connection $Connection -Sql @"
SELECT cc.TABLE_NAME, cc.COLUMN_NAME, cc.POSITION, c.CONSTRAINT_NAME
FROM   ALL_CONSTRAINTS c
JOIN   ALL_CONS_COLUMNS cc ON c.CONSTRAINT_NAME = cc.CONSTRAINT_NAME AND c.OWNER = cc.OWNER
WHERE  c.OWNER = :owner AND c.CONSTRAINT_TYPE = 'P'
ORDER  BY cc.TABLE_NAME, cc.POSITION
"@ -Parameters @{ owner = $Owner }

    Write-Host "  Consultando claves foráneas..." -ForegroundColor Gray
    # 3 JOINs (sin ALL_CONS_COLUMNS rcc): evita el producto cartesiano que bloquea esquemas grandes.
    # REF_COLUMN no se obtiene aquí; se puede añadir en una consulta separada si se necesita.
    $foreignKeys = Invoke-OracleQuery -Connection $Connection -Sql @"
SELECT c.CONSTRAINT_NAME, c.TABLE_NAME, cc.COLUMN_NAME, cc.POSITION,
       rc.TABLE_NAME AS REF_TABLE, c.DELETE_RULE
FROM   ALL_CONSTRAINTS c
JOIN   ALL_CONS_COLUMNS cc ON c.CONSTRAINT_NAME  = cc.CONSTRAINT_NAME AND c.OWNER = cc.OWNER
JOIN   ALL_CONSTRAINTS  rc ON c.R_CONSTRAINT_NAME = rc.CONSTRAINT_NAME
WHERE  c.OWNER = :owner AND c.CONSTRAINT_TYPE = 'R'
ORDER  BY c.TABLE_NAME, c.CONSTRAINT_NAME, cc.POSITION
"@ -Parameters @{ owner = $Owner }

    Write-Host "  Consultando restricciones CHECK..." -ForegroundColor Gray
    $checks = Invoke-OracleQuery -Connection $Connection -Sql @"
SELECT CONSTRAINT_NAME, TABLE_NAME, SEARCH_CONDITION
FROM   ALL_CONSTRAINTS
WHERE  OWNER = :owner AND CONSTRAINT_TYPE = 'C' AND GENERATED = 'USER NAME'
ORDER  BY TABLE_NAME, CONSTRAINT_NAME
"@ -Parameters @{ owner = $Owner }

    Write-Host "  Consultando índices..." -ForegroundColor Gray
    $indexes = Invoke-OracleQuery -Connection $Connection -Sql @"
SELECT i.INDEX_NAME, i.TABLE_NAME, i.INDEX_TYPE, i.UNIQUENESS,
       ic.COLUMN_NAME, ic.COLUMN_POSITION, ic.DESCEND
FROM   ALL_INDEXES i
JOIN   ALL_IND_COLUMNS ic ON i.INDEX_NAME = ic.INDEX_NAME AND i.OWNER = ic.INDEX_OWNER
WHERE  i.OWNER = :owner
ORDER  BY i.TABLE_NAME, i.INDEX_NAME, ic.COLUMN_POSITION
"@ -Parameters @{ owner = $Owner }

    return @{
        Owner      = $Owner
        Tables     = $tables
        Views      = $views
        Columns    = $columns
        PrimaryKeys = $primaryKeys
        ForeignKeys = $foreignKeys
        Checks     = $checks
        Indexes    = $indexes
    }
}

# ─────────────────────────────────────────────────────────────────────────────
# REGIÓN: Hash del esquema
# ─────────────────────────────────────────────────────────────────────────────

function Format-DataType {
    param([hashtable]$Col)
    $type = $Col['DATA_TYPE']
    switch -Regex ($type) {
        '^(VARCHAR2|NVARCHAR2|CHAR|NCHAR)$' { return "$type($($Col['DATA_LENGTH']))" }
        '^NUMBER$' {
            if ($null -ne $Col['DATA_PRECISION'] -and $null -ne $Col['DATA_SCALE']) {
                return "NUMBER($($Col['DATA_PRECISION']),$($Col['DATA_SCALE']))"
            }
            if ($null -ne $Col['DATA_PRECISION']) { return "NUMBER($($Col['DATA_PRECISION']))" }
            return "NUMBER"
        }
        '^FLOAT$'  { return "FLOAT($($Col['DATA_PRECISION']))" }
        default    { return $type }
    }
}

function Compute-SchemaHash {
    param([hashtable]$Schema)

    $parts = [System.Text.StringBuilder]::new()

    # Pre-índice columnas por tabla (O(n) en lugar de O(n*m) por búsqueda anidada)
    $colsByTable = @{}
    foreach ($c in $Schema.Columns) {
        $tn = $c['TABLE_NAME']
        if (-not $colsByTable.ContainsKey($tn)) {
            $colsByTable[$tn] = [System.Collections.Generic.List[hashtable]]::new()
        }
        $colsByTable[$tn].Add($c)
    }

    # Tablas y columnas ordenadas
    foreach ($t in $Schema.Tables | Sort-Object { $_['TABLE_NAME'] }) {
        $tn = $t['TABLE_NAME']
        $null = $parts.AppendLine("TABLE|$tn")
        if ($colsByTable.ContainsKey($tn)) {
            $tableCols = $colsByTable[$tn] | Sort-Object { [int]$_['COLUMN_ID'] }
            foreach ($c in $tableCols) {
                $dt = Format-DataType $c
                $null = $parts.AppendLine("COL|$tn|$($c['COLUMN_NAME'])|$dt|$($c['NULLABLE'])")
            }
        }
    }

    # PKs
    foreach ($pk in $Schema.PrimaryKeys | Sort-Object { "$($_['TABLE_NAME'])|$($_['POSITION'])" }) {
        $null = $parts.AppendLine("PK|$($pk['TABLE_NAME'])|$($pk['COLUMN_NAME'])")
    }

    # FKs
    foreach ($fk in $Schema.ForeignKeys | Sort-Object { "$($_['TABLE_NAME'])|$($_['CONSTRAINT_NAME'])|$($_['POSITION'])" }) {
        $null = $parts.AppendLine("FK|$($fk['TABLE_NAME'])|$($fk['COLUMN_NAME'])|$($fk['REF_TABLE'])|$($fk['REF_COLUMN'])")
    }

    $bytes = [System.Text.Encoding]::UTF8.GetBytes($parts.ToString())
    $sha   = [System.Security.Cryptography.SHA256]::Create()
    $hash  = $sha.ComputeHash($bytes)
    return [System.BitConverter]::ToString($hash).Replace("-", "").ToLower()
}

# ─────────────────────────────────────────────────────────────────────────────
# REGIÓN: Detección de cambios
# ─────────────────────────────────────────────────────────────────────────────

function Compare-Schemas {
    param(
        [hashtable]$Old,
        [hashtable]$New
    )

    $changes = @{
        NewTables     = [System.Collections.Generic.List[string]]::new()
        DroppedTables = [System.Collections.Generic.List[string]]::new()
        NewColumns    = [System.Collections.Generic.List[string]]::new()
        DroppedColumns= [System.Collections.Generic.List[string]]::new()
        TypeChanges   = [System.Collections.Generic.List[string]]::new()
        NullChanges   = [System.Collections.Generic.List[string]]::new()
        NewFKs        = [System.Collections.Generic.List[string]]::new()
        DroppedFKs    = [System.Collections.Generic.List[string]]::new()
        CommentChanges= [System.Collections.Generic.List[string]]::new()
    }

    if ($null -eq $Old) { return $changes }  # primer uso, no hay diferencia que reportar

    $oldTableNames = $Old.Tables | ForEach-Object { $_['TABLE_NAME'] }
    $newTableNames = $New.Tables | ForEach-Object { $_['TABLE_NAME'] }

    foreach ($t in $newTableNames) { if ($t -notin $oldTableNames) { $changes.NewTables.Add($t) } }
    foreach ($t in $oldTableNames) { if ($t -notin $newTableNames) { $changes.DroppedTables.Add($t) } }

    # Columnas
    $oldColIndex = @{}
    foreach ($c in $Old.Columns) { $oldColIndex["$($c['TABLE_NAME']).$($c['COLUMN_NAME'])"] = $c }

    foreach ($c in $New.Columns) {
        $key = "$($c['TABLE_NAME']).$($c['COLUMN_NAME'])"
        if (-not $oldColIndex.ContainsKey($key)) {
            $changes.NewColumns.Add("$($c['TABLE_NAME']).$($c['COLUMN_NAME'])")
        } else {
            $old = $oldColIndex[$key]
            $newType = Format-DataType $c
            $oldType = Format-DataType $old
            if ($newType -ne $oldType) {
                $changes.TypeChanges.Add("$($c['TABLE_NAME']).$($c['COLUMN_NAME']): $oldType -> $newType")
            }
            if ($old['NULLABLE'] -ne $c['NULLABLE']) {
                $changes.NullChanges.Add("$($c['TABLE_NAME']).$($c['COLUMN_NAME']): NULLABLE=$($old['NULLABLE']) -> $($c['NULLABLE'])")
            }
            if ($old['COMMENTS'] -ne $c['COMMENTS']) {
                $changes.CommentChanges.Add("$($c['TABLE_NAME']).$($c['COLUMN_NAME'])")
            }
        }
    }

    $newColKeys = $New.Columns | ForEach-Object { "$($_['TABLE_NAME']).$($_['COLUMN_NAME'])" }
    foreach ($key in $oldColIndex.Keys) {
        if ($key -notin $newColKeys) { $changes.DroppedColumns.Add($key) }
    }

    # FKs (identificadas por tabla_origen.columna->tabla_destino)
    $oldFkKeys = $Old.ForeignKeys | ForEach-Object { "$($_['TABLE_NAME']).$($_['COLUMN_NAME'])->$($_['REF_TABLE'])" } | Sort-Object -Unique
    $newFkKeys = $New.ForeignKeys | ForEach-Object { "$($_['TABLE_NAME']).$($_['COLUMN_NAME'])->$($_['REF_TABLE'])" } | Sort-Object -Unique

    foreach ($fk in $newFkKeys) { if ($fk -notin $oldFkKeys) { $changes.NewFKs.Add($fk) } }
    foreach ($fk in $oldFkKeys) { if ($fk -notin $newFkKeys) { $changes.DroppedFKs.Add($fk) } }

    return $changes
}

function Has-Changes {
    param([hashtable]$Changes)
    return ($Changes.NewTables.Count + $Changes.DroppedTables.Count +
            $Changes.NewColumns.Count + $Changes.DroppedColumns.Count +
            $Changes.TypeChanges.Count + $Changes.NullChanges.Count +
            $Changes.NewFKs.Count + $Changes.DroppedFKs.Count +
            $Changes.CommentChanges.Count) -gt 0
}

# ─────────────────────────────────────────────────────────────────────────────
# REGIÓN: Generadores de documentación
# ─────────────────────────────────────────────────────────────────────────────

function Generate-DatabaseSchemaMd {
    param([hashtable]$Schema)

    $sb = [System.Text.StringBuilder]::new()
    $now = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

    $null = $sb.AppendLine("# Esquema de Base de Datos — $($Schema.Owner)")
    $null = $sb.AppendLine("")
    $null = $sb.AppendLine("> Generado automáticamente por Sync-OracleSchema.ps1  ")
    $null = $sb.AppendLine("> Última actualización: $now")
    $null = $sb.AppendLine("")

    # ── Resumen ──────────────────────────────────────────────────────────────
    $tableCount  = $Schema.Tables.Count
    $viewCount   = $Schema.Views.Count
    $columnCount = $Schema.Columns.Count
    $fkCount     = ($Schema.ForeignKeys | Select-Object -ExpandProperty 'CONSTRAINT_NAME' -Unique | Measure-Object).Count

    $null = $sb.AppendLine("## Resumen")
    $null = $sb.AppendLine("")
    $null = $sb.AppendLine("| Elemento | Cantidad |")
    $null = $sb.AppendLine("|---|---|")
    $null = $sb.AppendLine("| Tablas | $tableCount |")
    $null = $sb.AppendLine("| Vistas | $viewCount |")
    $null = $sb.AppendLine("| Columnas totales | $columnCount |")
    $null = $sb.AppendLine("| Claves foráneas | $fkCount |")
    $null = $sb.AppendLine("")

    # ── Tablas ───────────────────────────────────────────────────────────────
    $null = $sb.AppendLine("## Tablas")
    $null = $sb.AppendLine("")

    # Índice de PKs y FKs por tabla (para rendimiento)
    $pkIndex = @{}
    foreach ($pk in $Schema.PrimaryKeys) {
        $tn = $pk['TABLE_NAME']
        if (-not $pkIndex.ContainsKey($tn)) { $pkIndex[$tn] = [System.Collections.Generic.List[string]]::new() }
        $pkIndex[$tn].Add($pk['COLUMN_NAME'])
    }

    $fkIndex = @{}
    foreach ($fk in $Schema.ForeignKeys) {
        $tn = $fk['TABLE_NAME']
        $cn = $fk['COLUMN_NAME']
        $fkIndex["$tn.$cn"] = $fk['REF_TABLE']
    }

    $indexByTable = @{}
    foreach ($idx in $Schema.Indexes) {
        $tn = $idx['TABLE_NAME']
        $inm = $idx['INDEX_NAME']
        if (-not $indexByTable.ContainsKey($tn)) { $indexByTable[$tn] = @{} }
        if (-not $indexByTable[$tn].ContainsKey($inm)) {
            $indexByTable[$tn][$inm] = @{
                Type      = $idx['INDEX_TYPE']
                Unique    = $idx['UNIQUENESS']
                Columns   = [System.Collections.Generic.List[string]]::new()
            }
        }
        $indexByTable[$tn][$inm].Columns.Add($idx['COLUMN_NAME'])
    }

    # Pre-índice columnas por tabla
    $colsByTableSchema = @{}
    foreach ($c in $Schema.Columns) {
        $tn = $c['TABLE_NAME']
        if (-not $colsByTableSchema.ContainsKey($tn)) {
            $colsByTableSchema[$tn] = [System.Collections.Generic.List[hashtable]]::new()
        }
        $colsByTableSchema[$tn].Add($c)
    }

    foreach ($t in $Schema.Tables | Sort-Object { $_['TABLE_NAME'] }) {
        $tn = $t['TABLE_NAME']
        $comment = $t['COMMENTS']

        $null = $sb.AppendLine("### $tn")
        $null = $sb.AppendLine("")
        if ($comment) { $null = $sb.AppendLine("$comment"); $null = $sb.AppendLine("") }

        $tableCols = if ($colsByTableSchema.ContainsKey($tn)) {
            $colsByTableSchema[$tn] | Sort-Object { [int]$_['COLUMN_ID'] }
        } else { @() }

        $null = $sb.AppendLine("| Columna | Tipo | Nulable | PK | FK | Descripción |")
        $null = $sb.AppendLine("|---|---|:---:|:---:|:---:|---|")

        foreach ($c in $tableCols) {
            $cn       = $c['COLUMN_NAME']
            $dt       = Format-DataType $c
            $nullable = if ($c['NULLABLE'] -eq 'Y') { "✓" } else { "" }
            $isPk     = if ($pkIndex.ContainsKey($tn) -and $cn -in $pkIndex[$tn]) { "PK" } else { "" }
            $fkRef    = if ($fkIndex.ContainsKey("$tn.$cn")) { "→ $($fkIndex["$tn.$cn"])" } else { "" }
            $colComment = $c['COMMENTS']

            $null = $sb.AppendLine("| $cn | $dt | $nullable | $isPk | $fkRef | $colComment |")
        }
        $null = $sb.AppendLine("")

        # Índices de la tabla
        if ($indexByTable.ContainsKey($tn) -and $indexByTable[$tn].Count -gt 0) {
            $null = $sb.AppendLine("**Índices:**")
            $null = $sb.AppendLine("")
            foreach ($inm in $indexByTable[$tn].Keys | Sort-Object) {
                $idx = $indexByTable[$tn][$inm]
                $uniqueLabel = if ($idx.Unique -eq 'UNIQUE') { " (UNIQUE)" } else { "" }
                $colsList = $idx.Columns -join ", "
                $null = $sb.AppendLine("- ``$inm``$uniqueLabel — $colsList")
            }
            $null = $sb.AppendLine("")
        }
    }

    # ── Vistas ───────────────────────────────────────────────────────────────
    if ($Schema.Views.Count -gt 0) {
        $null = $sb.AppendLine("## Vistas")
        $null = $sb.AppendLine("")
        foreach ($v in $Schema.Views | Sort-Object { $_['VIEW_NAME'] }) {
            $null = $sb.AppendLine("- ``$($v['VIEW_NAME'])``")
        }
        $null = $sb.AppendLine("")
    }

    return $sb.ToString()
}

function Generate-BusinessDomainMd {
    param([hashtable]$Schema)

    $sb = [System.Text.StringBuilder]::new()
    $now = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

    $null = $sb.AppendLine("# Dominio de Negocio — $($Schema.Owner)")
    $null = $sb.AppendLine("")
    $null = $sb.AppendLine("> Generado automáticamente por Sync-OracleSchema.ps1  ")
    $null = $sb.AppendLine("> Última actualización: $now")
    $null = $sb.AppendLine("")

    # Agrupar tablas por prefijo (antes del primer '_')
    $groups = @{}
    foreach ($t in $Schema.Tables) {
        $tn = $t['TABLE_NAME']
        $prefix = if ($tn -match '^([A-Z0-9]+)_') { $Matches[1] } else { "GENERAL" }
        if (-not $groups.ContainsKey($prefix)) {
            $groups[$prefix] = [System.Collections.Generic.List[hashtable]]::new()
        }
        $groups[$prefix].Add($t)
    }

    $null = $sb.AppendLine("## Agrupación por dominio funcional")
    $null = $sb.AppendLine("")
    $null = $sb.AppendLine("Los dominios se infieren del prefijo de nombre de tabla (prefijo antes del primer `_`).")
    $null = $sb.AppendLine("")

    foreach ($prefix in $groups.Keys | Sort-Object) {
        $tables = $groups[$prefix]
        $null = $sb.AppendLine("### Dominio: $prefix ($($tables.Count) tablas)")
        $null = $sb.AppendLine("")
        $null = $sb.AppendLine("| Tabla | Descripción |")
        $null = $sb.AppendLine("|---|---|")

        foreach ($t in $tables | Sort-Object { $_['TABLE_NAME'] }) {
            $null = $sb.AppendLine("| $($t['TABLE_NAME']) | $($t['COMMENTS']) |")
        }
        $null = $sb.AppendLine("")
    }

    # Relaciones entre dominios (FKs cross-domain)
    $crossDomainFks = [System.Collections.Generic.List[string]]::new()
    $fkGroups = $Schema.ForeignKeys | Group-Object { $_['CONSTRAINT_NAME'] }
    foreach ($fkGroup in $fkGroups) {
        $fk = $fkGroup.Group[0]
        $fromPrefix = if ($fk['TABLE_NAME']  -match '^([A-Z0-9]+)_') { $Matches[1] } else { "GENERAL" }
        $toPrefix   = if ($fk['REF_TABLE']   -match '^([A-Z0-9]+)_') { $Matches[1] } else { "GENERAL" }
        if ($fromPrefix -ne $toPrefix) {
            $crossDomainFks.Add("$($fk['TABLE_NAME']).$($fk['COLUMN_NAME']) → $($fk['REF_TABLE']).$($fk['REF_COLUMN']) [$fromPrefix → $toPrefix]")
        }
    }

    if ($crossDomainFks.Count -gt 0) {
        $null = $sb.AppendLine("## Relaciones entre dominios")
        $null = $sb.AppendLine("")
        foreach ($rel in $crossDomainFks | Sort-Object) {
            $null = $sb.AppendLine("- $rel")
        }
        $null = $sb.AppendLine("")
    }

    return $sb.ToString()
}

function Generate-DatabaseErdMd {
    param([hashtable]$Schema)

    $sb = [System.Text.StringBuilder]::new()
    $now = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

    $null = $sb.AppendLine("# Diagrama ERD — $($Schema.Owner)")
    $null = $sb.AppendLine("")
    $null = $sb.AppendLine("> Generado automáticamente por Sync-OracleSchema.ps1  ")
    $null = $sb.AppendLine("> Última actualización: $now")
    $null = $sb.AppendLine("")

    # PKs por tabla para anotación
    $pkIndex = @{}
    foreach ($pk in $Schema.PrimaryKeys) {
        $tn = $pk['TABLE_NAME']
        if (-not $pkIndex.ContainsKey($tn)) { $pkIndex[$tn] = [System.Collections.Generic.List[string]]::new() }
        $pkIndex[$tn].Add($pk['COLUMN_NAME'])
    }

    $null = $sb.AppendLine('```mermaid')
    $null = $sb.AppendLine("erDiagram")

    # Entidades (solo tablas con relaciones FK para mantener el diagrama manejable)
    $tablesInFk = [System.Collections.Generic.HashSet[string]]::new()
    foreach ($fk in $Schema.ForeignKeys) {
        $null = $tablesInFk.Add($fk['TABLE_NAME'])
        $null = $tablesInFk.Add($fk['REF_TABLE'])
    }

    # Si no hay FKs, incluir todas las tablas
    $tablesToRender = if ($tablesInFk.Count -gt 0) {
        $Schema.Tables | Where-Object { $_['TABLE_NAME'] -in $tablesInFk }
    } else {
        $Schema.Tables
    }

    # Pre-índice columnas por tabla
    $colsByTableErd = @{}
    foreach ($c in $Schema.Columns) {
        $tn = $c['TABLE_NAME']
        if (-not $colsByTableErd.ContainsKey($tn)) {
            $colsByTableErd[$tn] = [System.Collections.Generic.List[hashtable]]::new()
        }
        $colsByTableErd[$tn].Add($c)
    }

    foreach ($t in $tablesToRender | Sort-Object { $_['TABLE_NAME'] }) {
        $tn = $t['TABLE_NAME']
        $tableCols = if ($colsByTableErd.ContainsKey($tn)) {
            $colsByTableErd[$tn] | Sort-Object { [int]$_['COLUMN_ID'] } | Select-Object -First 10
        } else { @() }  # Limitar columnas para legibilidad del ERD

        $null = $sb.AppendLine("    $tn {")
        foreach ($c in $tableCols) {
            $cn = $c['COLUMN_NAME']
            $dt = (Format-DataType $c) -replace '[(),]', '_' -replace '\s', '_'
            $pk = if ($pkIndex.ContainsKey($tn) -and $cn -in $pkIndex[$tn]) { " PK" } else { "" }
            $null = $sb.AppendLine("        $dt $cn$pk")
        }
        $null = $sb.AppendLine("    }")
    }

    # Relaciones (FKs)
    $renderedFks = [System.Collections.Generic.HashSet[string]]::new()
    foreach ($fkGroup in ($Schema.ForeignKeys | Group-Object { $_['CONSTRAINT_NAME'] })) {
        $fk    = $fkGroup.Group[0]
        $from  = $fk['TABLE_NAME']
        $to    = $fk['REF_TABLE']
        $label = $fk['CONSTRAINT_NAME']
        $relKey = "$from|$to"
        if ($renderedFks.Contains($relKey)) { continue }
        $null = $renderedFks.Add($relKey)
        $null = $sb.AppendLine("    $from }o--|| $to : `"$label`"")
    }

    $null = $sb.AppendLine('```')

    return $sb.ToString()
}

function Generate-ChangelogEntry {
    param(
        [hashtable]$Changes,
        [string]$Hash
    )

    $now = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $sb  = [System.Text.StringBuilder]::new()

    $null = $sb.AppendLine("## Ejecución: $now")
    $null = $sb.AppendLine("")
    $null = $sb.AppendLine("**Hash de esquema:** ``$Hash``")
    $null = $sb.AppendLine("")

    if ($Changes.NewTables.Count -gt 0) {
        $null = $sb.AppendLine("### Nuevas tablas")
        $null = $sb.AppendLine("")
        foreach ($t in $Changes.NewTables | Sort-Object) { $null = $sb.AppendLine("- $t") }
        $null = $sb.AppendLine("")
    }
    if ($Changes.DroppedTables.Count -gt 0) {
        $null = $sb.AppendLine("### Tablas eliminadas")
        $null = $sb.AppendLine("")
        foreach ($t in $Changes.DroppedTables | Sort-Object) { $null = $sb.AppendLine("- $t") }
        $null = $sb.AppendLine("")
    }
    if ($Changes.NewColumns.Count -gt 0) {
        $null = $sb.AppendLine("### Nuevas columnas")
        $null = $sb.AppendLine("")
        foreach ($c in $Changes.NewColumns | Sort-Object) { $null = $sb.AppendLine("- $c") }
        $null = $sb.AppendLine("")
    }
    if ($Changes.DroppedColumns.Count -gt 0) {
        $null = $sb.AppendLine("### Columnas eliminadas")
        $null = $sb.AppendLine("")
        foreach ($c in $Changes.DroppedColumns | Sort-Object) { $null = $sb.AppendLine("- $c") }
        $null = $sb.AppendLine("")
    }
    if ($Changes.TypeChanges.Count -gt 0) {
        $null = $sb.AppendLine("### Cambios de tipo")
        $null = $sb.AppendLine("")
        foreach ($c in $Changes.TypeChanges | Sort-Object) { $null = $sb.AppendLine("- $c") }
        $null = $sb.AppendLine("")
    }
    if ($Changes.NullChanges.Count -gt 0) {
        $null = $sb.AppendLine("### Cambios de nulabilidad")
        $null = $sb.AppendLine("")
        foreach ($c in $Changes.NullChanges | Sort-Object) { $null = $sb.AppendLine("- $c") }
        $null = $sb.AppendLine("")
    }
    if ($Changes.NewFKs.Count -gt 0) {
        $null = $sb.AppendLine("### Relaciones añadidas")
        $null = $sb.AppendLine("")
        foreach ($fk in $Changes.NewFKs | Sort-Object) { $null = $sb.AppendLine("- $fk") }
        $null = $sb.AppendLine("")
    }
    if ($Changes.DroppedFKs.Count -gt 0) {
        $null = $sb.AppendLine("### Relaciones eliminadas")
        $null = $sb.AppendLine("")
        foreach ($fk in $Changes.DroppedFKs | Sort-Object) { $null = $sb.AppendLine("- $fk") }
        $null = $sb.AppendLine("")
    }
    if ($Changes.CommentChanges.Count -gt 0) {
        $null = $sb.AppendLine("### Cambios en comentarios")
        $null = $sb.AppendLine("")
        foreach ($c in $Changes.CommentChanges | Sort-Object) { $null = $sb.AppendLine("- $c") }
        $null = $sb.AppendLine("")
    }

    return $sb.ToString()
}

# ─────────────────────────────────────────────────────────────────────────────
# REGIÓN: Serialización de esquema para persistencia (comparación futura)
# ─────────────────────────────────────────────────────────────────────────────

function Export-SchemaSnapshot {
    param([hashtable]$Schema, [string]$Path)
    $snapshot = @{
        Owner      = $Schema.Owner
        Tables     = $Schema.Tables
        Views      = $Schema.Views
        Columns    = $Schema.Columns
        PrimaryKeys = $Schema.PrimaryKeys
        ForeignKeys = $Schema.ForeignKeys
    }
    $snapshot | ConvertTo-Json -Depth 5 | Set-Content -Path $Path -Encoding UTF8
}

function Import-SchemaSnapshot {
    param([string]$Path)
    if (-not (Test-Path $Path)) { return $null }
    $raw = Get-Content -Path $Path -Raw -Encoding UTF8 | ConvertFrom-Json -Depth 5 -AsHashtable
    return $raw
}

# ─────────────────────────────────────────────────────────────────────────────
# REGIÓN: Ejecución principal
# ─────────────────────────────────────────────────────────────────────────────

Write-Host ""
Write-Host "═══════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "  Sync-OracleSchema — Sincronización de esquema Oracle " -ForegroundColor Cyan
Write-Host "═══════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""

# 1. Cargar DLL Oracle
Write-Host "▶ Cargando driver Oracle..." -ForegroundColor Yellow
$dllPath = Find-OracleDll
Add-Type -Path $dllPath
Write-Host "  OK — $dllPath" -ForegroundColor Green

# 2. Cadena de conexión
Write-Host "▶ Construyendo cadena de conexión..." -ForegroundColor Yellow
$connectionString = Get-ConnectionString
Write-Host "  OK" -ForegroundColor Green

# 3. Preparar carpeta de salida
if (-not (Test-Path $SchemaPath)) {
    New-Item -ItemType Directory -Path $SchemaPath -Force | Out-Null
    Write-Host "  Carpeta creada: $SchemaPath" -ForegroundColor Gray
}

$versionFile   = Join-Path $SchemaPath "schema-version.json"
$snapshotFile  = Join-Path $SchemaPath "schema-snapshot.json"
$schemaFile    = Join-Path $SchemaPath "database-schema.md"
$domainFile    = Join-Path $SchemaPath "business-domain.md"
$erdFile       = Join-Path $SchemaPath "database-erd.md"
$changelogFile = Join-Path $SchemaPath "schema-changelog.md"

# 4. Cargar versión existente
$existingVersion = $null
if (Test-Path $versionFile) {
    $existingVersion = Get-Content -Path $versionFile -Raw -Encoding UTF8 | ConvertFrom-Json
    Write-Host "▶ Versión anterior: hash=$($existingVersion.schemaHash) | $($existingVersion.lastUpdate)" -ForegroundColor Gray
}

# 5. Conectar y obtener esquema
Write-Host "▶ Conectando a Oracle..." -ForegroundColor Yellow
$conn = [Oracle.ManagedDataAccess.Client.OracleConnection]::new($connectionString)
$conn.Open()
Write-Host "  OK — Conectado" -ForegroundColor Green

try {
    $owner = Get-CurrentSchema -Connection $conn
    Write-Host "▶ Schema: $owner" -ForegroundColor Yellow

    Write-Host "▶ Extrayendo esquema..." -ForegroundColor Yellow
    $schema = Get-OracleSchema -Connection $conn -Owner $owner
    Write-Host "  OK — $($schema.Tables.Count) tablas, $($schema.Views.Count) vistas, $($schema.Columns.Count) columnas" -ForegroundColor Green
}
finally {
    $conn.Close()
    $conn.Dispose()
}

# 6. Calcular hash
Write-Host "▶ Calculando hash..." -ForegroundColor Yellow
$currentHash = Compute-SchemaHash -Schema $schema
Write-Host "  Hash: $currentHash" -ForegroundColor Gray

# 7. Comparar hash
if (-not $Force -and $null -ne $existingVersion -and $existingVersion.schemaHash -eq $currentHash) {
    Write-Host ""
    Write-Host "✔ Sin cambios en el esquema." -ForegroundColor Green
    Write-Host "  Actualizando timestamp de última comprobación..." -ForegroundColor Gray

    $existingVersion.lastChecked = (Get-Date -Format "yyyy-MM-dd HH:mm:ss")
    $existingVersion | ConvertTo-Json | Set-Content -Path $versionFile -Encoding UTF8

    Write-Host "  Finalizado sin regeneración." -ForegroundColor Green
    Write-Host ""
    exit 0
}

# 8. Hay cambios (o primer uso): cargar snapshot anterior para diff
Write-Host "▶ Detectando cambios respecto al estado anterior..." -ForegroundColor Yellow
$oldSchema = Import-SchemaSnapshot -Path $snapshotFile
$changes   = Compare-Schemas -Old $oldSchema -New $schema

if (Has-Changes -Changes $changes) {
    Write-Host "  ⚠ Se detectaron cambios en el esquema:" -ForegroundColor Yellow
    if ($changes.NewTables.Count     -gt 0) { Write-Host "    + $($changes.NewTables.Count) tablas nuevas" -ForegroundColor Green }
    if ($changes.DroppedTables.Count -gt 0) { Write-Host "    - $($changes.DroppedTables.Count) tablas eliminadas" -ForegroundColor Red }
    if ($changes.NewColumns.Count    -gt 0) { Write-Host "    + $($changes.NewColumns.Count) columnas nuevas" -ForegroundColor Green }
    if ($changes.DroppedColumns.Count-gt 0) { Write-Host "    - $($changes.DroppedColumns.Count) columnas eliminadas" -ForegroundColor Red }
    if ($changes.TypeChanges.Count   -gt 0) { Write-Host "    ~ $($changes.TypeChanges.Count) cambios de tipo" -ForegroundColor DarkYellow }
    if ($changes.NewFKs.Count        -gt 0) { Write-Host "    + $($changes.NewFKs.Count) relaciones nuevas" -ForegroundColor Green }
    if ($changes.DroppedFKs.Count    -gt 0) { Write-Host "    - $($changes.DroppedFKs.Count) relaciones eliminadas" -ForegroundColor Red }
} else {
    Write-Host "  Primera ejecución o regeneración forzada." -ForegroundColor Gray
}

# 9. Generar documentación
Write-Host "▶ Generando database-schema.md..." -ForegroundColor Yellow
Generate-DatabaseSchemaMd -Schema $schema | Set-Content -Path $schemaFile -Encoding UTF8
Write-Host "  OK" -ForegroundColor Green

Write-Host "▶ Generando business-domain.md..." -ForegroundColor Yellow
Generate-BusinessDomainMd -Schema $schema | Set-Content -Path $domainFile -Encoding UTF8
Write-Host "  OK" -ForegroundColor Green

Write-Host "▶ Generando database-erd.md..." -ForegroundColor Yellow
Generate-DatabaseErdMd -Schema $schema | Set-Content -Path $erdFile -Encoding UTF8
Write-Host "  OK" -ForegroundColor Green

# 10. Actualizar changelog
Write-Host "▶ Actualizando schema-changelog.md..." -ForegroundColor Yellow
$changelogEntry = Generate-ChangelogEntry -Changes $changes -Hash $currentHash
$changelogHeader = @"
# Historial de cambios del esquema — $($schema.Owner)

> Generado automáticamente por Sync-OracleSchema.ps1

## Resumen histórico

| Fecha | Hash | Tablas | Columnas |
|---|---|---|---|

"@

if (Test-Path $changelogFile) {
    $existingChangelog = Get-Content -Path $changelogFile -Raw -Encoding UTF8
    # Insertar nueva entrada después del encabezado (antes de la primera ejecución anterior)
    $insertMarker = "## Ejecución:"
    if ($existingChangelog -match $insertMarker) {
        $newChangelog = $existingChangelog -replace "($insertMarker)", "$changelogEntry`n`$1"
    } else {
        $newChangelog = $existingChangelog + "`n" + $changelogEntry
    }
    $newChangelog | Set-Content -Path $changelogFile -Encoding UTF8
} else {
    ($changelogHeader + $changelogEntry) | Set-Content -Path $changelogFile -Encoding UTF8
}
Write-Host "  OK" -ForegroundColor Green

# 11. Guardar snapshot del esquema actual
Export-SchemaSnapshot -Schema $schema -Path $snapshotFile

# 12. Actualizar schema-version.json
$now = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$version = @{
    schemaHash  = $currentHash
    lastUpdate  = $now
    lastChecked = $now
    tables      = $schema.Tables.Count
    columns     = $schema.Columns.Count
    views       = $schema.Views.Count
}
$version | ConvertTo-Json | Set-Content -Path $versionFile -Encoding UTF8
Write-Host "▶ schema-version.json actualizado." -ForegroundColor Green

# 13. Resumen final
Write-Host ""
Write-Host "═══════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "  Documentación generada en: $SchemaPath" -ForegroundColor Cyan
Write-Host ""
Write-Host "  database-schema.md    ✔" -ForegroundColor Green
Write-Host "  business-domain.md    ✔" -ForegroundColor Green
Write-Host "  database-erd.md       ✔" -ForegroundColor Green
Write-Host "  schema-changelog.md   ✔" -ForegroundColor Green
Write-Host "  schema-version.json   ✔" -ForegroundColor Green
Write-Host "═══════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""
