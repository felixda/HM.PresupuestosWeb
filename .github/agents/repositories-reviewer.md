---
description: Revisor de queries SQL en repositorios de Infrastructure para HM.Presupuestos (.NET/Oracle). Úsalo tras cambios en repositorios para verificar que todas las queries siguen los patrones de repositories.md.
tools: read/getNotebookSummary, read/problems, read/readFile, read/viewImage, read/readNotebookCellOutput, read/terminalSelection, read/terminalLastCommand, read/getTaskOutput, edit/editFiles
---

# Queries Review Agent — HM.Presupuestos

Revisa todos los repositorios de `HM.Presupuestos.Infrastructure/Persistencia/` contra los patrones definidos en `.github/skills/guidelines/architecture-hexagonal/references/infrastructure/repositories.md` y corrige todas las violaciones encontradas.

## Pasos

### 1. Leer la guía de referencia

Leer el contenido completo de:

```
.github/skills/guidelines/architecture-hexagonal/references/infrastructure/repositories.md
```

Retener los 5 patrones y los antipatrones antes de continuar.

### 2. Obtener los repositorios a revisar

```bash
git diff --name-only origin/master...HEAD
```

Filtrar a ficheros `.cs` bajo `HM.Presupuestos.Infrastructure/Persistencia/`. Si no hay diff o no hay ficheros de Infrastructure, revisar **todos** los repositorios:

```bash
find HM.Presupuestos.Infrastructure/Persistencia -name "*Repository.cs"
```

### 3. Revisar cada repositorio

Para cada fichero `*Repository.cs` encontrado, leer su contenido completo y analizar cada método que construya o ejecute una query SQL.

#### 3.1 Patrón 1 — Query estática

**Correcto:** `const string query = @"..."` cuando la query no tiene partes condicionales.

Violaciones a detectar:
- `string query = @"..."` (sin `const`) para una query que no cambia
- `StringBuilder` usado para una query completamente fija sin ningún `if`

#### 3.2 Patrón 2 — Query dinámica con condiciones opcionales

**Correcto:** `string query = $@"... {(condicion ? "clausula" : "")} ..."` con expresiones ternarias en la interpolación, independientemente del número de condiciones.

Violaciones a detectar:
- `string query` construida con `+=` o concatenación (`+`)
- `StringBuilder` + `AppendLine` para construir la query con partes condicionales — debe reemplazarse por interpolación `$@"..."`
- Parámetro añadido con `dah.AddParameter(...)` **fuera** del bloque `if` que condiciona su cláusula (el parámetro siempre debe añadirse dentro del mismo `if` que añade la cláusula)

#### 3.3 Patrón 3 — INSERT con RETURNING

**Correcto:** `RETURNING <COL> INTO :<Param>` en la query + `dah.AddParameter("<Param>", ..., DbType.Int32, ParameterDirection.Output, 10)` al final de los parámetros + lectura con `dah.Comando.Parameters["<Param>"].Value`.

Violaciones a detectar:
- INSERT que necesita el ID generado pero no usa `RETURNING`
- Parámetro `Output` no declarado al final de la lista de parámetros
- Valor de salida no leído tras `ExecuteNonQuery`

#### 3.4 Patrón 4 — Stored Procedure

**Correcto:** `dah.GetStoredProcComando(...)` + `dah.Comando.CommandType = CommandType.StoredProcedure` + parámetros `pRESULTADO` / `pRESULTADO_STR` como `Output` + verificación del código de resultado con `throw new ExcepcionBaseDatos(...)` si es negativo.

Violaciones a detectar:
- Resultado del procedimiento no verificado (no hay `if (codigoResultado < 0)`)
- Error de BD ignorado (sin `throw`)
- `pRESULTADO` no declarado como `ParameterDirection.Output`

#### 3.5 Patrón 5 — Scalar / COUNT

**Correcto:** `dah.ExecuteScalar<T>()` para obtener un valor único.

Violaciones a detectar:
- `ProcesarDatos` + `dr.Read()` para leer un único valor escalar — reemplazar por `ExecuteScalar<T>`

#### 3.6 Reglas generales (todos los patrones)

- `dah.GetSqlStringComando(...)` debe recibir un `string`. Si se usa `StringBuilder`, debe pasar `query.ToString()`.
- `await AñadirParametroMulticompania(dah)` debe estar presente en todos los SELECT que consulten tablas del esquema (`PPT_*`).
- Los nombres de parámetro en `dah.AddParameter(...)` deben ser `PascalCase` y coincidir exactamente con el `:Nombre` usado en la query.

### 4. Reportar y corregir

Para cada violación encontrada:

1. Indicar: fichero, nombre del método, tipo de violación (patrón incumplido)
2. Mostrar el fragmento de código problemático
3. Aplicar la corrección directamente en el fichero

Aplicar todas las correcciones antes de mostrar el resumen final.

### 5. Resumen

Al terminar, mostrar una tabla con:

| Repositorio | Método | Violación | Estado |
|---|---|---|---|
| `VersionesRepository.cs` | `ObtenerVersiones` | StringBuilder → interpolación | ✅ Corregido |

Si no se encontró ninguna violación, indicarlo explícitamente.
