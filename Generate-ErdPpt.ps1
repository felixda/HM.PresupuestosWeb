#Requires -Version 7.0
param([string]$ConnStr = "user id=presupuestos;password=sotseupuserp18c;data source=IBEDESA19c;Pooling=True;Min Pool Size=0;")

$dllPath = "C:\Users\felix.davilla-ext\.nuget\packages\oracle.manageddataaccess.core\2.19.200\lib\netstandard2.0\Oracle.ManagedDataAccess.dll"
if (-not ([System.Management.Automation.PSTypeName]'Oracle.ManagedDataAccess.Client.OracleConnection').Type) {
    Add-Type -Path $dllPath
}

function Exec-Query($conn, $sql) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = $sql
    $reader = $cmd.ExecuteReader()
    $rows = [System.Collections.Generic.List[System.Collections.Hashtable]]::new()
    while ($reader.Read()) {
        $row = [System.Collections.Hashtable]::new()
        for ($i = 0; $i -lt $reader.FieldCount; $i++) {
            $row[$reader.GetName($i)] = if ($reader.IsDBNull($i)) { '' } else { $reader.GetValue($i).ToString() }
        }
        $rows.Add($row)
    }
    $reader.Close(); $cmd.Dispose()
    return ,$rows  # coma fuerza devolver como array
}

$conn = [Oracle.ManagedDataAccess.Client.OracleConnection]::new($ConnStr)
$conn.Open()
Write-Host "Conectado OK"

$owner = 'PRESUPUESTOS'

Write-Host "Consultando tablas PPT_..."
$tables = Exec-Query $conn "SELECT t.TABLE_NAME, NVL(c.COMMENTS,'') AS COMMENTS FROM ALL_TABLES t LEFT JOIN ALL_TAB_COMMENTS c ON t.TABLE_NAME=c.TABLE_NAME AND t.OWNER=c.OWNER WHERE t.OWNER='$owner' AND t.TABLE_NAME LIKE 'PPT%' ORDER BY t.TABLE_NAME"
Write-Host "  $($tables.Count) tablas"

Write-Host "Consultando columnas PPT_..."
$cols = Exec-Query $conn "SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE, DATA_LENGTH, DATA_PRECISION, DATA_SCALE, NULLABLE, COLUMN_ID FROM ALL_TAB_COLUMNS WHERE OWNER='$owner' AND TABLE_NAME LIKE 'PPT%' ORDER BY TABLE_NAME, COLUMN_ID"
Write-Host "  $($cols.Count) columnas"

Write-Host "Consultando PKs PPT_..."
$pks = Exec-Query $conn "SELECT cc.TABLE_NAME, cc.COLUMN_NAME FROM ALL_CONSTRAINTS c JOIN ALL_CONS_COLUMNS cc ON c.CONSTRAINT_NAME=cc.CONSTRAINT_NAME AND c.OWNER=cc.OWNER WHERE c.OWNER='$owner' AND c.CONSTRAINT_TYPE='P' AND cc.TABLE_NAME LIKE 'PPT%' ORDER BY cc.TABLE_NAME, cc.POSITION"
Write-Host "  $($pks.Count) PKs"

Write-Host "Consultando FKs PPT_..."
$fks = Exec-Query $conn "SELECT c.CONSTRAINT_NAME, c.TABLE_NAME, cc.COLUMN_NAME, cc.POSITION, rc.TABLE_NAME AS REF_TABLE FROM ALL_CONSTRAINTS c JOIN ALL_CONS_COLUMNS cc ON c.CONSTRAINT_NAME=cc.CONSTRAINT_NAME AND c.OWNER=cc.OWNER JOIN ALL_CONSTRAINTS rc ON c.R_CONSTRAINT_NAME=rc.CONSTRAINT_NAME WHERE c.OWNER='$owner' AND c.CONSTRAINT_TYPE='R' AND (c.TABLE_NAME LIKE 'PPT%' OR rc.TABLE_NAME LIKE 'PPT%') ORDER BY c.TABLE_NAME, c.CONSTRAINT_NAME, cc.POSITION"
Write-Host "  $($fks.Count) filas FK"

$conn.Close(); $conn.Dispose()

# Pre-índices
$pkIndex = @{}
foreach ($pk in $pks) {
    $tn = $pk['TABLE_NAME']
    if (-not $pkIndex.ContainsKey($tn)) { $pkIndex[$tn] = [System.Collections.Generic.List[string]]::new() }
    $pkIndex[$tn].Add($pk['COLUMN_NAME'])
}

$colsByTable = @{}
foreach ($c in $cols) {
    $tn = $c['TABLE_NAME']
    if (-not $colsByTable.ContainsKey($tn)) { $colsByTable[$tn] = [System.Collections.Generic.List[System.Collections.Hashtable]]::new() }
    $colsByTable[$tn].Add($c)
}

function Format-DT($c) {
    switch -Regex ($c['DATA_TYPE']) {
        '^(VARCHAR2|NVARCHAR2|CHAR|NCHAR)$' { return "$($c['DATA_TYPE'])_$($c['DATA_LENGTH'])_" }
        '^NUMBER$' {
            if ($c['DATA_PRECISION'] -ne '' -and $c['DATA_SCALE'] -ne '') { return "NUMBER_$($c['DATA_PRECISION'])_$($c['DATA_SCALE'])_" }
            if ($c['DATA_PRECISION'] -ne '') { return "NUMBER_$($c['DATA_PRECISION'])_" }
            return "NUMBER"
        }
        default { return ($c['DATA_TYPE'] -replace '[\s(),]','_') }
    }
}

$sb = [System.Text.StringBuilder]::new()
$now = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$null = $sb.AppendLine("# ERD Reducido — PPT (PRESUPUESTOS)")
$null = $sb.AppendLine("")
$null = $sb.AppendLine("> Generado automáticamente · $now  ")
$null = $sb.AppendLine("> Tablas con prefijo ``PPT`` — $($tables.Count) tablas, $($cols.Count) columnas")
$null = $sb.AppendLine("")
$null = $sb.AppendLine('```mermaid')
$null = $sb.AppendLine("erDiagram")

$tableNames = $tables | ForEach-Object { $_['TABLE_NAME'] }

foreach ($t in $tables | Sort-Object { $_['TABLE_NAME'] }) {
    $tn = $t['TABLE_NAME']
    $null = $sb.AppendLine("    $tn {")
    if ($colsByTable.ContainsKey($tn)) {
        $tableCols = $colsByTable[$tn] | Sort-Object { [int]$_['COLUMN_ID'] }
        foreach ($c in $tableCols) {
            $dt = Format-DT $c
            $pk = if ($pkIndex.ContainsKey($tn) -and $c['COLUMN_NAME'] -in $pkIndex[$tn]) { " PK" } else { "" }
            $null = $sb.AppendLine("        $dt $($c['COLUMN_NAME'])$pk")
        }
    }
    $null = $sb.AppendLine("    }")
}

# Relaciones solo entre tablas PPT_
$rendered = [System.Collections.Generic.HashSet[string]]::new()
foreach ($fkGroup in ($fks | Group-Object { $_['CONSTRAINT_NAME'] })) {
    $fk = $fkGroup.Group[0]
    $from = $fk['TABLE_NAME']; $to = $fk['REF_TABLE']
    if ($from -notin $tableNames -or $to -notin $tableNames) { continue }
    $relKey = "$from||$to"
    if ($rendered.Contains($relKey)) { continue }
    $null = $rendered.Add($relKey)
    $null = $sb.AppendLine("    $from }o--|| $to : `"`"")
}

$null = $sb.AppendLine('```')

$outPath = Join-Path $PSScriptRoot ".github\skills\guidelines\database\database-erd-ppt.md"
$sb.ToString() | Set-Content -Path $outPath -Encoding UTF8
$sizeKb = [math]::Round((Get-Item $outPath).Length / 1KB, 1)
Write-Host "✔ Generado: $outPath ($sizeKb KB)"
