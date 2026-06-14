# Esquema de Base de Datos — PRESUPUESTOS

> Generado automáticamente por Sync-OracleSchema.ps1  
> Última actualización: 2026-06-04 20:30:32

## Resumen

| Elemento | Cantidad |
|---|---|
| Tablas | 237 |
| Vistas | 97 |
| Columnas totales | 5403 |
| Claves foráneas | 188 |

## Tablas

### AN_CI_CIA_PCT_REPARTO_22062018

BACKUP cierre marzo antes nueva carga. Eliminar cuando sea posible

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER |  |  |  |  |
| ANIO | NUMBER |  |  |  |  |
| COD_CIERRE | NUMBER |  |  |  |  |
| COD_CDC_ORIGEN | VARCHAR2(50) |  |  |  |  |
| COD_CDC_DESTINO | VARCHAR2(50) |  |  |  |  |
| COD_COMPANIA_ORIGEN | NUMBER |  |  |  |  |
| COD_COMPANIA_DESTINO | NUMBER |  |  |  |  |
| COD_CONCEPTO_COMPENSACION | NUMBER |  |  |  |  |
| F_CIERRE | DATE |  |  |  |  |
| IMPORTE_ACUMULADO | NUMBER |  |  |  |  |
| PORCENTAJE_ACUMULADO | NUMBER |  |  |  |  |

### AN_CIERRE_CIA_PCT_REPARTO

Solo hasta el mes de cierre

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER |  |  |  |  |
| ANIO | NUMBER |  |  |  |  |
| COD_CIERRE | NUMBER |  |  |  | Cierre al que estará asociado |
| COD_TIPO_AGRUPACION_CDC | NUMBER |  |  |  |  |
| COD_CDC_ORIGEN | VARCHAR2(50) |  |  |  | -1 Sin centro de coste origen |
| COD_CDC_DESTINO | VARCHAR2(50) |  |  |  | Solo para Global y Services de momento -1 Sin centro de coste origen |
| COD_COMPANIA_ORIGEN | NUMBER |  |  |  |  |
| COD_COMPANIA_DESTINO | NUMBER |  |  |  |  |
| COD_CONCEPTO_COMPENSACION | NUMBER |  |  |  |  |
| F_CIERRE | DATE |  |  |  |  |
| IMPORTE_ACUMULADO | NUMBER |  |  |  |  |
| PORCENTAJE_ACUMULADO | NUMBER |  |  |  |  |
| COD_EMPLEADO | NUMBER | ✓ |  |  |  |

**Índices:**

- `UQ_AN_CIERRE_CIA_PCT_REPARTO` (UNIQUE) — COD_PAIS, ANIO, COD_CIERRE, COD_CDC_ORIGEN, COD_CDC_DESTINO, COD_COMPANIA_ORIGEN, COD_COMPANIA_DESTINO, COD_CONCEPTO_COMPENSACION, COD_TIPO_AGRUPACION_CDC, COD_EMPLEADO

### AN_CIERRE_CONSOLIDA_INVERSION

Tabla temporal donde se precalculan los datos por virtual, vertical y especialidad para su posterior tratamiento en los cálculos de los porcentajes

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER |  |  |  |  |
| ANIO | NUMBER |  |  |  |  |
| COD_CIERRE | NUMBER | ✓ |  |  |  |
| COD_VIRTUAL | NUMBER |  |  |  |  |
| COD_VERTICAL | NUMBER |  |  |  |  |
| COD_ESPECIALIDAD | NUMBER |  |  |  |  |
| COD_OFICINA | NUMBER |  |  |  |  |
| MES | NUMBER |  |  |  |  |
| IMPORTE | NUMBER |  |  |  |  |
| IMPORTE_ACUMULADO | NUMBER |  |  |  |  |

**Índices:**

- `IDX_CONSOLIDA_CI_INV` — COD_PAIS, ANIO, COD_CIERRE
- `IDX_CONSOLIDA_CI_IV_TOT` — COD_PAIS, ANIO, COD_CIERRE, COD_VIRTUAL, COD_VERTICAL, COD_ESPECIALIDAD, COD_OFICINA, MES

### AN_CIERRE_EMPLEADO_PCT_BACKUP

Tabla que guardará un backup cuando se vuelve a recargar un cierre.

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER | ✓ |  |  |  |
| ANIO | NUMBER |  |  |  |  |
| COD_CIERRE | NUMBER |  |  |  |  |
| COD_EMPLEADO | NUMBER |  |  |  |  |
| COD_VIRTUAL | NUMBER |  |  |  |  |
| COD_VERTICAL | NUMBER |  |  |  |  |
| COD_ESPECIALIDAD | NUMBER |  |  |  |  |
| COD_OFICINA | NUMBER |  |  |  |  |
| MES | NUMBER |  |  |  |  |
| ID_CDC_ORIGEN | NUMBER | ✓ |  |  |  |
| ID_CDC_DESTINO | NUMBER | ✓ |  |  |  |
| COD_TIPO_AGRUPACION_CDC_ORI | NUMBER | ✓ |  |  |  |
| COD_TIPO_AGRUPACION_CDC_DEST | NUMBER |  |  |  |  |
| IMPORTE | NUMBER |  |  |  |  |
| PORCENTAJE | NUMBER |  |  |  |  |
| IMPORTE_ACUMULADO | NUMBER |  |  |  |  |
| PORCENTAJE_ACUMULADO | NUMBER |  |  |  |  |
| COD_USUARIO | NUMBER |  |  |  |  |
| F_BACKUP | DATE |  |  |  |  |
| COM_0_MES | NUMBER | ✓ |  |  |  |
| COM_23_MES | NUMBER | ✓ |  |  |  |
| COM_1_MES | NUMBER | ✓ |  |  |  |
| COM_2_MES | NUMBER | ✓ |  |  |  |
| COM_3_MES | NUMBER | ✓ |  |  |  |
| COM_4_MES | NUMBER | ✓ |  |  |  |
| COM_5_MES | NUMBER | ✓ |  |  |  |
| COM_6_MES | NUMBER | ✓ |  |  |  |
| COM_7_MES | NUMBER | ✓ |  |  |  |
| COM_8_MES | NUMBER | ✓ |  |  |  |
| COM_9_MES | NUMBER | ✓ |  |  |  |
| COM_10_MES | NUMBER | ✓ |  |  |  |
| COM_11_MES | NUMBER | ✓ |  |  |  |
| COM_12_MES | NUMBER | ✓ |  |  |  |
| COM_13_MES | NUMBER | ✓ |  |  |  |
| COM_14_MES | NUMBER | ✓ |  |  |  |
| COM_15_MES | NUMBER | ✓ |  |  |  |
| COM_16_MES | NUMBER | ✓ |  |  |  |
| COM_17_MES | NUMBER | ✓ |  |  |  |
| COM_18_MES | NUMBER | ✓ |  |  |  |
| COM_19_MES | NUMBER | ✓ |  |  |  |
| COM_20_MES | NUMBER | ✓ |  |  |  |
| COM_21_MES | NUMBER | ✓ |  |  |  |
| COM_22_MES | NUMBER | ✓ |  |  |  |
| COM_0_ACU | NUMBER | ✓ |  |  |  |
| COM_23_ACU | NUMBER | ✓ |  |  |  |
| COM_1_ACU | NUMBER | ✓ |  |  |  |
| COM_2_ACU | NUMBER | ✓ |  |  |  |
| COM_3_ACU | NUMBER | ✓ |  |  |  |
| COM_4_ACU | NUMBER | ✓ |  |  |  |
| COM_5_ACU | NUMBER | ✓ |  |  |  |
| COM_6_ACU | NUMBER | ✓ |  |  |  |
| COM_7_ACU | NUMBER | ✓ |  |  |  |
| COM_8_ACU | NUMBER | ✓ |  |  |  |
| COM_9_ACU | NUMBER | ✓ |  |  |  |
| COM_10_ACU | NUMBER | ✓ |  |  |  |
| COM_11_ACU | NUMBER | ✓ |  |  |  |
| COM_12_ACU | NUMBER | ✓ |  |  |  |
| COM_13_ACU | NUMBER | ✓ |  |  |  |
| COM_14_ACU | NUMBER | ✓ |  |  |  |
| COM_15_ACU | NUMBER | ✓ |  |  |  |
| COM_16_ACU | NUMBER | ✓ |  |  |  |
| COM_17_ACU | NUMBER | ✓ |  |  |  |
| COM_18_ACU | NUMBER | ✓ |  |  |  |
| COM_19_ACU | NUMBER | ✓ |  |  |  |
| COM_20_ACU | NUMBER | ✓ |  |  |  |
| COM_21_ACU | NUMBER | ✓ |  |  |  |
| COM_22_ACU | NUMBER | ✓ |  |  |  |
| COD_PURE_PLAYER | NUMBER |  |  |  | Realmente es "equipo". Cuando se decidió cambiar el coste de cambiar el nombre de bbdd es alto. Primer cierre que lo utilizamos: SEP.2018 |
| COD_DEPARTAMENTO | NUMBER | ✓ |  |  |  |

### AN_CIERRE_EMPLEADO_PORCENTAJES

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER |  |  |  |  |
| ANIO | NUMBER |  |  |  |  |
| COD_CIERRE | NUMBER |  |  |  |  |
| COD_EMPLEADO | NUMBER |  |  |  |  |
| COD_VIRTUAL | NUMBER |  |  | → AN_VIRTUAL |  |
| COD_VERTICAL | NUMBER |  |  | → AN_VERTICAL |  |
| COD_ESPECIALIDAD | NUMBER |  |  | → AN_ESPECIALIDAD |  |
| COD_OFICINA | NUMBER |  |  | → AN_OFICINA |  |
| MES | NUMBER |  |  |  |  |
| ID_CDC_ORIGEN | NUMBER |  |  |  |  |
| ID_CDC_DESTINO | NUMBER |  |  |  |  |
| COD_TIPO_AGRUPACION_CDC_ORI | NUMBER |  |  | → AN_TIPO_AGRUPACION_CDC |  |
| COD_TIPO_AGRUPACION_CDC_DEST | NUMBER |  |  | → AN_TIPO_AGRUPACION_CDC |  |
| IMPORTE | NUMBER |  |  |  |  |
| PORCENTAJE | NUMBER |  |  |  |  |
| IMPORTE_ACUMULADO | NUMBER |  |  |  | Importe acumulado hasta el mes |
| PORCENTAJE_ACUMULADO | NUMBER |  |  |  | Porcentaje con respecto al importe acumulado hasta el mes |
| COD_USUARIO_CARGA | NUMBER |  |  |  |  |
| F_CARGA | DATE |  |  |  |  |
| COM_0_MES | NUMBER | ✓ |  |  |  |
| COM_23_MES | NUMBER | ✓ |  |  |  |
| COM_1_MES | NUMBER | ✓ |  |  |  |
| COM_2_MES | NUMBER | ✓ |  |  |  |
| COM_3_MES | NUMBER | ✓ |  |  |  |
| COM_4_MES | NUMBER | ✓ |  |  |  |
| COM_5_MES | NUMBER | ✓ |  |  |  |
| COM_6_MES | NUMBER | ✓ |  |  |  |
| COM_7_MES | NUMBER | ✓ |  |  |  |
| COM_8_MES | NUMBER | ✓ |  |  |  |
| COM_9_MES | NUMBER | ✓ |  |  |  |
| COM_10_MES | NUMBER | ✓ |  |  |  |
| COM_11_MES | NUMBER | ✓ |  |  |  |
| COM_12_MES | NUMBER | ✓ |  |  |  |
| COM_13_MES | NUMBER | ✓ |  |  |  |
| COM_14_MES | NUMBER | ✓ |  |  |  |
| COM_15_MES | NUMBER | ✓ |  |  |  |
| COM_16_MES | NUMBER | ✓ |  |  |  |
| COM_17_MES | NUMBER | ✓ |  |  |  |
| COM_18_MES | NUMBER | ✓ |  |  |  |
| COM_19_MES | NUMBER | ✓ |  |  |  |
| COM_20_MES | NUMBER | ✓ |  |  |  |
| COM_21_MES | NUMBER | ✓ |  |  |  |
| COM_22_MES | NUMBER | ✓ |  |  |  |
| COM_0_ACU | NUMBER | ✓ |  |  |  |
| COM_23_ACU | NUMBER | ✓ |  |  |  |
| COM_1_ACU | NUMBER | ✓ |  |  |  |
| COM_2_ACU | NUMBER | ✓ |  |  |  |
| COM_3_ACU | NUMBER | ✓ |  |  |  |
| COM_4_ACU | NUMBER | ✓ |  |  |  |
| COM_5_ACU | NUMBER | ✓ |  |  |  |
| COM_6_ACU | NUMBER | ✓ |  |  |  |
| COM_7_ACU | NUMBER | ✓ |  |  |  |
| COM_8_ACU | NUMBER | ✓ |  |  |  |
| COM_9_ACU | NUMBER | ✓ |  |  |  |
| COM_10_ACU | NUMBER | ✓ |  |  |  |
| COM_11_ACU | NUMBER | ✓ |  |  |  |
| COM_12_ACU | NUMBER | ✓ |  |  |  |
| COM_13_ACU | NUMBER | ✓ |  |  |  |
| COM_14_ACU | NUMBER | ✓ |  |  |  |
| COM_15_ACU | NUMBER | ✓ |  |  |  |
| COM_16_ACU | NUMBER | ✓ |  |  |  |
| COM_17_ACU | NUMBER | ✓ |  |  |  |
| COM_18_ACU | NUMBER | ✓ |  |  |  |
| COM_19_ACU | NUMBER | ✓ |  |  |  |
| COM_20_ACU | NUMBER | ✓ |  |  |  |
| COM_21_ACU | NUMBER | ✓ |  |  |  |
| COM_22_ACU | NUMBER | ✓ |  |  |  |
| COD_PURE_PLAYER | NUMBER |  |  | → PY_PURE_PLAYER | Realmente es "equipo". Cuando se decidió cambiar el coste de cambiar el nombre de bbdd es alto. Primer cierre que lo utilizamos: SEP.2018 |
| COD_DEPARTAMENTO | NUMBER | ✓ |  | → PY_DEPARTAMENTO |  |

**Índices:**

- `IDX_AN_CIE_EMP_PCT_AN_VER_EMP` — COD_CIERRE, COD_EMPLEADO
- `IDX_CIER_CON_ANIO_CIERR` — ANIO, COD_CIERRE
- `IDX_CIER_CON_ANIO_CIERR_MES` — ANIO, COD_CIERRE, MES
- `UQ_AN_CIERRE_EMPLEADO` (UNIQUE) — COD_PAIS, ANIO, COD_CIERRE, COD_EMPLEADO, COD_VIRTUAL, COD_VERTICAL, COD_ESPECIALIDAD, COD_OFICINA, COD_PURE_PLAYER, MES, ID_CDC_ORIGEN, ID_CDC_DESTINO, COD_TIPO_AGRUPACION_CDC_ORI, COD_TIPO_AGRUPACION_CDC_DEST, COD_DEPARTAMENTO

### AN_DIARIO_CONSOLIDA_INV_GRUPO

Tabla temporal donde se precalculan los datos por virtual, vertical, especialidad y grupo para su posterior tratamiento en los cálculos de los porcentajes

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER |  |  |  |  |
| ANIO | NUMBER |  |  |  |  |
| COD_VERSION | NUMBER |  |  |  |  |
| COD_VIRTUAL | NUMBER |  |  |  |  |
| COD_VERTICAL | NUMBER |  |  |  |  |
| COD_ESPECIALIDAD | NUMBER |  |  |  |  |
| COD_OFICINA | NUMBER |  |  |  |  |
| COD_GRUPO | NUMBER | ✓ |  |  |  |
| MES | NUMBER |  |  |  |  |
| IMPORTE | NUMBER |  |  |  |  |
| IMPORTE_ACUMULADO | NUMBER |  |  |  |  |

**Índices:**

- `IDX_CONSOLIDA_INV_GRUP_VER` — COD_PAIS, ANIO, COD_VERSION
- `IDX_D_V_VERS_VI` — COD_PAIS, ANIO, COD_VERSION, COD_VIRTUAL
- `IDX_D_V_VERS_VI_VE_ES_OF_GR` — COD_PAIS, ANIO, COD_VERSION, COD_VIRTUAL, COD_VERTICAL, COD_ESPECIALIDAD, COD_OFICINA, COD_GRUPO

### AN_DIARIO_EMPLEADO_PORCENTAJES

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS_ANIO | NUMBER |  |  |  | Para la particion puesto que se puede repitir una versión en varios paises |
| COD_PAIS | NUMBER |  |  |  |  |
| ANIO | NUMBER |  |  |  |  |
| COD_VERSION | NUMBER |  |  |  |  |
| COD_EMPLEADO | NUMBER |  |  |  |  |
| COD_VIRTUAL | NUMBER |  |  | → AN_VIRTUAL |  |
| COD_VERTICAL | NUMBER |  |  | → AN_VERTICAL |  |
| COD_ESPECIALIDAD | NUMBER |  |  | → AN_ESPECIALIDAD |  |
| COD_OFICINA | NUMBER |  |  | → AN_OFICINA |  |
| MES | NUMBER |  |  |  |  |
| ID_CDC_ORIGEN | NUMBER |  |  |  |  |
| COD_TIPO_AGRUPACION_CDC_ORI | NUMBER |  |  | → AN_TIPO_AGRUPACION_CDC |  |
| ID_CDC_DESTINO | NUMBER |  |  |  |  |
| COD_TIPO_AGRUPACION_CDC_DEST | NUMBER |  |  | → AN_TIPO_AGRUPACION_CDC |  |
| IMPORTE | NUMBER |  |  |  |  |
| PORCENTAJE | NUMBER |  |  |  |  |
| IMPORTE_ACUMULADO | NUMBER |  |  |  | Importe acumulado hasta el mes |
| PORCENTAJE_ACUMULADO | NUMBER |  |  |  | Porcentaje con respecto al importe acumulado |
| COD_USUARIO_CARGA | NUMBER |  |  |  |  |
| F_CARGA | DATE |  |  |  |  |
| COM_0_MES | NUMBER | ✓ |  |  |  |
| COM_23_MES | NUMBER | ✓ |  |  |  |
| COM_1_MES | NUMBER | ✓ |  |  |  |
| COM_2_MES | NUMBER | ✓ |  |  |  |
| COM_3_MES | NUMBER | ✓ |  |  |  |
| COM_4_MES | NUMBER | ✓ |  |  |  |
| COM_5_MES | NUMBER | ✓ |  |  |  |
| COM_6_MES | NUMBER | ✓ |  |  |  |
| COM_7_MES | NUMBER | ✓ |  |  |  |
| COM_8_MES | NUMBER | ✓ |  |  |  |
| COM_9_MES | NUMBER | ✓ |  |  |  |
| COM_10_MES | NUMBER | ✓ |  |  |  |
| COM_11_MES | NUMBER | ✓ |  |  |  |
| COM_12_MES | NUMBER | ✓ |  |  |  |
| COM_13_MES | NUMBER | ✓ |  |  |  |
| COM_14_MES | NUMBER | ✓ |  |  |  |
| COM_15_MES | NUMBER | ✓ |  |  |  |
| COM_16_MES | NUMBER | ✓ |  |  |  |
| COM_17_MES | NUMBER | ✓ |  |  |  |
| COM_18_MES | NUMBER | ✓ |  |  |  |
| COM_19_MES | NUMBER | ✓ |  |  |  |
| COM_20_MES | NUMBER | ✓ |  |  |  |
| COM_21_MES | NUMBER | ✓ |  |  |  |
| COM_22_MES | NUMBER | ✓ |  |  |  |
| COM_0_ACU | NUMBER | ✓ |  |  |  |
| COM_23_ACU | NUMBER | ✓ |  |  |  |
| COM_1_ACU | NUMBER | ✓ |  |  |  |
| COM_2_ACU | NUMBER | ✓ |  |  |  |
| COM_3_ACU | NUMBER | ✓ |  |  |  |
| COM_4_ACU | NUMBER | ✓ |  |  |  |
| COM_5_ACU | NUMBER | ✓ |  |  |  |
| COM_6_ACU | NUMBER | ✓ |  |  |  |
| COM_7_ACU | NUMBER | ✓ |  |  |  |
| COM_8_ACU | NUMBER | ✓ |  |  |  |
| COM_9_ACU | NUMBER | ✓ |  |  |  |
| COM_10_ACU | NUMBER | ✓ |  |  |  |
| COM_11_ACU | NUMBER | ✓ |  |  |  |
| COM_12_ACU | NUMBER | ✓ |  |  |  |
| COM_13_ACU | NUMBER | ✓ |  |  |  |
| COM_14_ACU | NUMBER | ✓ |  |  |  |
| COM_15_ACU | NUMBER | ✓ |  |  |  |
| COM_16_ACU | NUMBER | ✓ |  |  |  |
| COM_17_ACU | NUMBER | ✓ |  |  |  |
| COM_18_ACU | NUMBER | ✓ |  |  |  |
| COM_19_ACU | NUMBER | ✓ |  |  |  |
| COM_20_ACU | NUMBER | ✓ |  |  |  |
| COM_21_ACU | NUMBER | ✓ |  |  |  |
| COM_22_ACU | NUMBER | ✓ |  |  |  |
| COD_PURE_PLAYER | NUMBER |  |  | → PY_PURE_PLAYER | Realmente es "equipo". Cuando se decidió cambiar el coste de cambiar el nombre de bbdd es alto |
| COD_DEPARTAMENTO | NUMBER | ✓ |  | → PY_DEPARTAMENTO |  |

**Índices:**

- `IDX_AN_DIAR_EMP_PCT_AN_VER_EMP` — COD_PAIS, ANIO, COD_VERSION, COD_EMPLEADO
- `IDX_EMP_POR_ANIO_VER` — ANIO, COD_VERSION
- `IDX_EMP_POR_PAIS_ANIO_VER` — COD_PAIS, ANIO, COD_VERSION
- `UQ_AN_DIARIO_EMPLEADO_IMPUTA` (UNIQUE) — COD_PAIS_ANIO, COD_PAIS, ANIO, COD_VERSION, COD_EMPLEADO, COD_VIRTUAL, COD_VERTICAL, COD_ESPECIALIDAD, COD_OFICINA, COD_PURE_PLAYER, MES, ID_CDC_ORIGEN, ID_CDC_DESTINO, COD_TIPO_AGRUPACION_CDC_ORI, COD_TIPO_AGRUPACION_CDC_DEST, COD_DEPARTAMENTO

### AN_EMPLEADO_ACCESO_AP

Tabla que se carga desde financials con los empleados que pueden acceder a Payables

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| DNI_NIE_EMPLEADO | VARCHAR2(150) |  |  |  |  |
| DES_EMPLEADO | VARCHAR2(240) |  |  |  |  |
| EMPLOYEE_NUMBER | NUMBER |  |  |  |  |
| PERSON_ID | NUMBER |  |  |  |  |
| COD_PAIS | NUMBER |  |  |  |  |
| COD_COMPANIA | NUMBER |  |  |  |  |
| MAIL | VARCHAR2(240) | ✓ |  |  |  |

### AN_EMPLEADO_ACCESO_GL

Tabla que se carga desde financials con los empleados que pueden acceder a General Ledger

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| DNI_NIE_EMPLEADO | VARCHAR2(150) |  |  |  |  |
| DES_EMPLEADO | VARCHAR2(240) |  |  |  |  |
| EMPLOYEE_NUMBER | NUMBER |  |  |  |  |
| PERSON_ID | NUMBER |  |  |  |  |
| COD_PAIS | NUMBER |  |  |  |  |
| COD_COMPANIA | NUMBER |  |  |  |  |
| MAIL | VARCHAR2(240) | ✓ |  |  |  |

### AN_EMPLEADO_ACCESO_PO

Tabla que se carga desde financials con los empleados que pueden acceder a purchasing

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| DNI_NIE_EMPLEADO | VARCHAR2(150) |  |  |  |  |
| DES_EMPLEADO | VARCHAR2(240) |  |  |  |  |
| EMPLOYEE_NUMBER | NUMBER |  |  |  |  |
| TIPO | VARCHAR2(11) |  |  |  |  |
| COD_TIPO | NUMBER |  |  |  |  |
| PERSON_ID | NUMBER(10,0) |  |  |  |  |
| COD_PAIS | NUMBER |  |  |  |  |
| COD_COMPANIA | NUMBER |  |  |  |  |
| MAIL | VARCHAR2(240) | ✓ |  |  |  |

### AN_EMPLEADO_DIST_IMPUTA

Dentro de cada empleado para España puede tener varias imputaciones distintas con porcentaje

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_EMPLEADO_DIST_IMPUTACION | NUMBER |  | PK |  |  |
| COD_EMPLEADO_IMPUTACION | NUMBER |  |  | → AN_EMPLEADO_IMPUTA |  |
| PCT_DISTRIBUCION | NUMBER |  |  |  |  |
| IND_VIRTUAL_TODOS | NUMBER | ✓ |  |  | 1 Tiene todas las virtuals 0 tiene que seleccionar alguna en la tabla |
| IND_VERTICAL_TODOS | NUMBER | ✓ |  |  | 1 Tiene todas las verticales 0 tiene que seleccionar alguna en la tabla |
| IND_ESPECIALIDAD_TODOS | NUMBER | ✓ |  |  | 1 Tiene todas las especialidades 0 tiene que seleccionar alguna en la tabla |
| IND_OFICINA_TODOS | NUMBER | ✓ |  |  | 1 Tiene todas las oficinas 0 tiene que seleccionar alguna en la tabla |
| COD_PURE_PLAYER | NUMBER |  |  | → PY_PURE_PLAYER | Realmente es "equipo". Cuando se decidió cambiar el coste de cambiar el nombre de bbdd es alto |
| COD_DEPARTAMENTO | NUMBER | ✓ |  | → PY_DEPARTAMENTO |  |
| IND_MEDIA_ON | NUMBER(1,0) | ✓ |  |  | Indica si el emplaedo esMedia On (1)  o Media off (0) o ambas (2). Inicialmente cogerá el del departamento |

**Índices:**

- `IDX_AN_EMP_DIST_IMP_COD_EMP_IM` — COD_EMPLEADO_IMPUTACION
- `PK_AN_EMPLEADO_DIST_IMPUTA` (UNIQUE) — COD_EMPLEADO_DIST_IMPUTACION

### AN_EMPLEADO_IMPUTA

Tabla maestra de las imputaciones por empleado

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_EMPLEADO_IMPUTACION | NUMBER |  | PK |  |  |
| COD_PAIS | NUMBER |  |  |  | País que se utilizará para la versión. Se podría sacar de V_COMPANIA_CDC, pero para identificar la versión lo ponemos también aquí |
| ANIO | NUMBER(4,0) |  |  |  | Para la versión |
| COD_VERSION | NUMBER |  |  |  | Código versión pptos. 999 real. |
| COD_EMPLEADO | NUMBER |  |  | → PY_EMPLEADO |  |
| ID_CDC | NUMBER |  |  |  |  |
| COD_UBICACION | NUMBER |  |  | → PY_UBICACION |  |
| IND_PLAZA_GARAJE | NUMBER |  |  |  |  |
| IND_HC | NUMBER |  |  |  | 0: No cuenta como head count 1: Sí cuenta |
| COD_NATURALEZA | NUMBER |  |  |  |  |
| PCT_RECLASIFICACION | NUMBER |  |  |  |  |
| F_DESDE | DATE |  |  |  |  |
| F_HASTA | DATE |  |  |  |  |
| IND_COSTE | NUMBER |  |  |  | 0: Gasto 1: Coste |
| NAME_CC_WD | VARCHAR2(200) | ✓ |  |  |  |
| DIGITAL | VARCHAR2(50) | ✓ |  |  |  |

**Índices:**

- `IDX_AN_EMP_IMP_PAIS_A_V_EMPL` — COD_PAIS, ANIO, COD_VERSION, COD_EMPLEADO
- `IDX_AN_EMP_IMP_PAS_AN_VER` — COD_PAIS, ANIO, COD_VERSION
- `PK_AN_EMPLEADO_IMPUTA` (UNIQUE) — COD_EMPLEADO_IMPUTACION

### AN_EMPLEADO_IMPUTA_ESPE

Imputación por especialidad no entra el nombre completo.

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_EMPLEADO_DIST_IMPUTACION | NUMBER |  | PK | → AN_EMPLEADO_DIST_IMPUTA |  |
| COD_ESPECIALIDAD | NUMBER |  | PK | → AN_ESPECIALIDAD |  |

**Índices:**

- `PK_AN_EMPLEADO_IMPUTA_AN_ESP` (UNIQUE) — COD_EMPLEADO_DIST_IMPUTACION, COD_ESPECIALIDAD

### AN_EMPLEADO_IMPUTA_OFICINA

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_EMPLEADO_DIST_IMPUTACION | NUMBER |  | PK | → AN_EMPLEADO_DIST_IMPUTA |  |
| COD_OFICINA | NUMBER |  | PK | → AN_OFICINA |  |

**Índices:**

- `PK_AN_EMPLEADO_IMPUTA_OFICINA` (UNIQUE) — COD_EMPLEADO_DIST_IMPUTACION, COD_OFICINA

### AN_EMPLEADO_IMPUTA_PORCENTAJE

Imputaciones por centro de coste y porcentaje (Global, services ..)

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_EMPLEADO_IMPUTACION | NUMBER |  | PK | → AN_EMPLEADO_IMPUTA |  |
| ID_CDC | NUMBER |  | PK |  | Codigo centro coste-companía de v_compania_cdc |
| COD_TIPO_AGRUPACION_CDC | NUMBER |  | PK | → AN_TIPO_AGRUPACION_CDC | Indica si es reparto de Global o de service |
| PORCENTAJE | NUMBER |  |  |  |  |

**Índices:**

- `PK_AN_EMPLEADO_IMP_PORCENTAJE` (UNIQUE) — COD_EMPLEADO_IMPUTACION, ID_CDC, COD_TIPO_AGRUPACION_CDC

### AN_EMPLEADO_IMPUTA_VERTICAL

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_EMPLEADO_DIST_IMPUTACION | NUMBER |  | PK | → AN_EMPLEADO_DIST_IMPUTA |  |
| COD_VERTICAL | NUMBER |  | PK | → AN_VERTICAL |  |

**Índices:**

- `IDX_EMP_IMP_VER_DIS_IM` — COD_EMPLEADO_DIST_IMPUTACION
- `PK_AN_EMPLEADO_IMP_AN_VERT` (UNIQUE) — COD_EMPLEADO_DIST_IMPUTACION, COD_VERTICAL

### AN_EMPLEADO_IMPUTA_VIRTUAL

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_EMPLEADO_DIST_IMPUTACION | NUMBER |  | PK | → AN_EMPLEADO_DIST_IMPUTA |  |
| COD_VIRTUAL | NUMBER |  | PK | → AN_VIRTUAL |  |

**Índices:**

- `PK_AN_EMPLEADO_IMPUTACION_VIRT` (UNIQUE) — COD_EMPLEADO_DIST_IMPUTACION, COD_VIRTUAL

### AN_EMPLEADO_IMPUTA_VIRTUAL_PUESTA_PROD

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_EMPLEADO_DIST_IMPUTACION | NUMBER |  |  |  |  |
| COD_VIRTUAL | NUMBER |  |  |  |  |

### AN_EMPLEADO_TIPO_AGRUPA_PCT

Tabla que contiene la distrubución por tipo agrupacion porcentual

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_EMPLEADO_IMPUTACION | NUMBER |  | PK | → AN_EMPLEADO_IMPUTA |  |
| COD_TIPO_AGRUPACION_CDC | NUMBER |  | PK | → AN_TIPO_AGRUPACION_CDC |  |
| PORCENTAJE | NUMBER |  |  |  |  |

**Índices:**

- `PK_AN_EMPLEADO_TIPO_AGRUPA_PCT` (UNIQUE) — COD_EMPLEADO_IMPUTACION, COD_TIPO_AGRUPACION_CDC

### AN_EMPLEADO_TIPO_IMPUTA_EXCEP

Esta tabla contiene excepciones a la asignacion de tipo de imputacion por empleado especificada por RRHH para su uso en TimeSheet.  Son excepciones a la vista V_AN_EMPLEADO_TIPO_IMPUTACION Usada para Controllers.

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_EMPLEADO | NUMBER |  | PK | → PY_EMPLEADO |  |
| ANIO | NUMBER(4,0) |  | PK |  |  |
| COD_VERSION | NUMBER |  | PK |  |  |
| F_DESDE | DATE |  | PK |  |  |
| F_HASTA | DATE |  | PK |  |  |
| TIPO_IMPUTACION | CHAR(1) | ✓ |  |  |  |

**Índices:**

- `PK_AN_EMPLEADO_TIPO_IMPUTA_EXC` (UNIQUE) — COD_EMPLEADO, ANIO, COD_VERSION, F_DESDE, F_HASTA

### AN_ESPECIALIDAD

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_ESPECIALIDAD | NUMBER |  | PK |  |  |
| DES_ESPECIALIDAD | VARCHAR2(50) |  |  |  |  |
| IND_ESTADO_ESPECIALIDAD | NUMBER |  |  |  | 1: Visible app 2: Tratado en "TODO" 4: FRONT 8: Funcionamiento FRONT o Data Insights (No es una especialidad como tal) 16: Mostrar/Tratar si la VERTICAL seleccionada es FRONT 32: GLOBAL -- Es global para el reparto de porcentajes automáticos.  64: SERVICE -- Es services para el reparto de porcentajes automáticos.  128: ESPAÑA -- Es España para el reparto de porcentajes automáticos 256: Se habilita la combo de pure player en Distribución |
| COD_GRUPO_ESPECIALIDAD | NUMBER | ✓ |  |  | Grupo Especialidad de presupuestos. se utiliza inicialmente en Dashboard para distribuir compensación |
| F_BAJA | DATE | ✓ |  |  |  |
| IND_ON | NUMBER |  |  |  | Indica si especialidad la siempre es ON independientemente de la vertical o si se tiene en cuenta la vertical para que sea ON/OFF |

**Índices:**

- `PK_AN_ESPECIALIDAD` (UNIQUE) — COD_ESPECIALIDAD

### AN_MAP_COMPANIA_CDC_REPARTO

Utilizada para la refacturación que dado compañia origen y compañía destino DE ESPAÑA nos da el centro de coste a repartir total compensacion. 
En el caso de los GLOBALES y SERVICES, directamente se pone la destino.
Hay que tener en cuenta que en la definicion de CDCs de OF la compañia del centro de coste es la origen.

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_COMPANIA_ORIGEN | NUMBER |  |  |  |  |
| COD_CDC_ORIGEN | VARCHAR2(50) |  |  |  |  |
| COD_COMPANIA_DESTINO | NUMBER |  |  |  |  |
| COD_ESPECIALIDAD | NUMBER | ✓ |  | → AN_ESPECIALIDAD | Si es nulo son todos menos si hay alguno con la misma combinación no nulo |
| COD_CDC_DESTINO | VARCHAR2(50) |  |  |  |  |
| COD_CDC_DESTINO_REFACTURACION | VARCHAR2(50) | ✓ |  |  | Columna de backup con los destinos cuando haya refacturación (se deshabilita poniendo destino ESP0). Cuando se quiera que se refacture se copia esta columna en COD_CDC_DESTINO |
| COD_CDC_DESTINO_ORI | VARCHAR2(50) | ✓ |  |  | De momento solo para Global y Services, si compañía origen es distinta que compañía destino se busca con este campo |
| COD_VIRTUAL | NUMBER | ✓ |  | → AN_VIRTUAL | Si no tiene valor no se utiliza, si tiene valor sí. Se comprueba después de la especialidad, en caso que tengan los dos prima la especialiad. |

**Índices:**

- `UNQ_IDX_AN_MAP_CDC_REPARTO` — COD_COMPANIA_ORIGEN, COD_CDC_ORIGEN, COD_COMPANIA_DESTINO, COD_ESPECIALIDAD, COD_CDC_DESTINO_ORI, COD_VIRTUAL

### AN_MAP_COMPANIA_CDC_REPARTO_BK_17092025

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_COMPANIA_ORIGEN | NUMBER |  |  |  |  |
| COD_CDC_ORIGEN | VARCHAR2(50) |  |  |  |  |
| COD_COMPANIA_DESTINO | NUMBER |  |  |  |  |
| COD_ESPECIALIDAD | NUMBER | ✓ |  |  |  |
| COD_CDC_DESTINO | VARCHAR2(50) |  |  |  |  |
| COD_CDC_DESTINO_REFACTURACION | VARCHAR2(50) | ✓ |  |  |  |
| COD_CDC_DESTINO_ORI | VARCHAR2(50) | ✓ |  |  |  |
| COD_VIRTUAL | NUMBER | ✓ |  |  |  |

### AN_MAP_COMPANIA_CDC_REPARTO_BK_CIERRE_2025

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_COMPANIA_ORIGEN | NUMBER |  |  |  |  |
| COD_CDC_ORIGEN | VARCHAR2(50) |  |  |  |  |
| COD_COMPANIA_DESTINO | NUMBER |  |  |  |  |
| COD_ESPECIALIDAD | NUMBER | ✓ |  |  |  |
| COD_CDC_DESTINO | VARCHAR2(50) |  |  |  |  |
| COD_CDC_DESTINO_REFACTURACION | VARCHAR2(50) | ✓ |  |  |  |
| COD_CDC_DESTINO_ORI | VARCHAR2(50) | ✓ |  |  |  |
| COD_VIRTUAL | NUMBER | ✓ |  |  |  |

### AN_MAP_COMPANIA_CDC_REPARTO_BK_CIERRE2024

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_COMPANIA_ORIGEN | NUMBER |  |  |  |  |
| COD_CDC_ORIGEN | VARCHAR2(50) |  |  |  |  |
| COD_COMPANIA_DESTINO | NUMBER |  |  |  |  |
| COD_ESPECIALIDAD | NUMBER | ✓ |  |  |  |
| COD_CDC_DESTINO | VARCHAR2(50) |  |  |  |  |
| COD_CDC_DESTINO_REFACTURACION | VARCHAR2(50) | ✓ |  |  |  |
| COD_CDC_DESTINO_ORI | VARCHAR2(50) | ✓ |  |  |  |
| COD_VIRTUAL | NUMBER | ✓ |  |  |  |

### AN_MAP_COMPANIA_CDC_REPARTO_PRUEBA_REFACT_2025

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_COMPANIA_ORIGEN | NUMBER |  |  |  |  |
| COD_CDC_ORIGEN | VARCHAR2(50) |  |  |  |  |
| COD_COMPANIA_DESTINO | NUMBER |  |  |  |  |
| COD_ESPECIALIDAD | NUMBER | ✓ |  |  |  |
| COD_CDC_DESTINO | VARCHAR2(50) |  |  |  |  |
| COD_CDC_DESTINO_REFACTURACION | VARCHAR2(50) | ✓ |  |  |  |
| COD_CDC_DESTINO_ORI | VARCHAR2(50) | ✓ |  |  |  |
| COD_VIRTUAL | NUMBER | ✓ |  |  |  |

### AN_MAP_CUENTA_OF_CONCEPTO

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| CUENTA_OF | VARCHAR2(50) |  | PK |  |  |
| COD_CONCEPTO_COMPENSACION | NUMBER |  | PK | → PY_CONCEPTO_COMPENSACION |  |
| CATEGORY_WD | VARCHAR2(1) | ✓ |  |  |  |
| CODE_WD | VARCHAR2(3) | ✓ |  |  |  |

**Índices:**

- `IDX_MAP_CTA_OF_CONC` — COD_CONCEPTO_COMPENSACION
- `PK_AN_CUENTA_OF_CONCEPTO_REPAR` (UNIQUE) — CUENTA_OF, COD_CONCEPTO_COMPENSACION

### AN_MAP_ESPECIALIDAD

Tabla que mapea las especialidades de RRHH con las especialidades de Presupuestos + ONOFF

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_MAP_ESPECIALIDAD | NUMBER |  | PK |  |  |
| COD_ESPECIALIDAD | NUMBER |  |  | → AN_ESPECIALIDAD | Especialidad de RRHH |
| COD_ESPECIALIDAD_PPTO | NUMBER |  |  |  | Codigo de especialiad de presupuestos |
| IND_ON_OFF | NUMBER |  |  |  | 1: ON 0:OFF |

**Índices:**

- `PK_AN_MAP_ESPECIALIDAD` (UNIQUE) — COD_MAP_ESPECIALIDAD
- `UNQ_AN_MAP_ESPECIALIDAD` (UNIQUE) — COD_ESPECIALIDAD, COD_ESPECIALIDAD_PPTO, IND_ON_OFF

### AN_MAP_OFICINA

Mapeamos las oficinas de analitacas con las oficinas de MMS

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_OFICINA | NUMBER |  | PK | → AN_OFICINA |  |
| COD_COMPANIA | NUMBER |  | PK |  |  |
| COD_DELEGACION | NUMBER |  | PK |  |  |

**Índices:**

- `PK_AN_MAP_OFICINA` (UNIQUE) — COD_OFICINA, COD_COMPANIA, COD_DELEGACION

### AN_MAP_VERTICAL

Tabla de mapeo de una vertical de rrhh con los medios de MMS. un medio solo puede estar en una vertical

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_MAP_VERTICAL | NUMBER |  | PK |  |  |
| COD_VERTICAL | NUMBER |  |  | → AN_VERTICAL |  |
| COD_PAIS | NUMBER |  |  |  |  |
| COD_MEDIO | NUMBER |  |  |  |  |

**Índices:**

- `PK_AN_MAP_VERTICAL` (UNIQUE) — COD_MAP_VERTICAL
- `UNQ_AN_MAP_VERTICAL` (UNIQUE) — COD_VERTICAL, COD_PAIS, COD_MEDIO

### AN_MAP_VIRTUAL

Tabla que relaciona las virtual con los entornos de presupuestos. Un entorno solo puede estar en una virtual.

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_MAP_VIRTUAL | NUMBER |  | PK |  |  |
| COD_VIRTUAL | NUMBER |  |  | → AN_VIRTUAL |  |
| COD_ENTORNO | NUMBER |  |  |  | Entorno de presupuestos |

**Índices:**

- `PK_AN_MAP_VIRTUAL` (UNIQUE) — COD_MAP_VIRTUAL
- `UNQ_AN_MAP_VIRTUAL` (UNIQUE) — COD_VIRTUAL, COD_ENTORNO

### AN_OFICINA

Tabla que contiene las oficinas. De momento es para Proximia y estará relacionadas conlas oficinas de MMS (Inicialmente  Proximia)

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_OFICINA | NUMBER |  | PK |  |  |
| DES_OFICINA | VARCHAR2(50) |  |  |  |  |
| IND_ESTADO_OFICINA | NUMBER |  |  |  | 1: Visible app 2: Tratado en "TODO" |
| F_BAJA | DATE | ✓ |  |  |  |

**Índices:**

- `PK_AN_OFICINA` (UNIQUE) — COD_OFICINA

### AN_STATUS_IMPUTACION_CIERRE

Tabla que se carga en la carga del cierre para tener un foto del estado de las imputaciones.

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| ANIO | NUMBER |  |  |  |  |
| COD_CIERRE | NUMBER |  |  |  |  |
| COD_EMPLEADO_IMPUTACION | NUMBER |  |  |  |  |
| COD_PAIS | NUMBER |  |  |  |  |
| COD_EMPLEADO | NUMBER |  |  |  |  |
| F_DESDE | DATE |  |  |  |  |
| F_HASTA | DATE |  |  |  |  |
| COD_COMPANIA | NUMBER |  |  |  |  |
| CIA_ESTATUTARIA | VARCHAR2(40) |  |  |  |  |
| ID_CDC | NUMBER |  |  |  |  |
| COD_CDC | VARCHAR2(150) |  |  |  |  |
| DES_CDC | VARCHAR2(240) |  |  |  |  |
| TIPO | VARCHAR2(50) |  |  |  |  |
| COD_VIRTUAL | NUMBER |  |  |  |  |
| COD_VERTICAL | NUMBER |  |  |  |  |
| COD_ESPECIALIDAD | NUMBER |  |  |  |  |
| COD_OFICINA | NUMBER |  |  |  |  |
| COD_NATURALEZA | NUMBER |  |  |  |  |
| COD_UBICACION | NUMBER | ✓ |  |  |  |
| IND_PLAZA_GARAJE | NUMBER | ✓ |  |  |  |
| IND_HC | NUMBER | ✓ |  |  |  |
| PCT_RECLASIFICACION | NUMBER |  |  |  |  |
| PCT_DISTRIBUCION | NUMBER |  |  |  |  |
| COD_PURE_PLAYER | NUMBER |  |  |  | Equipo |
| COD_DEPARTAMENTO | NUMBER | ✓ |  |  |  |

**Índices:**

- `IDX_STATUS_IMPUTACION_CIERR_AN` — ANIO, COD_CIERRE
- `IDX_STATUS_IMPUTACION_CIERRE` — COD_CIERRE

### AN_TIPO_AGRUPACION_CDC

Global, Services, etc

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_TIPO_AGRUPACION_CDC | NUMBER |  | PK |  |  |
| DES_TIPO_AGRUPACION_CDC | VARCHAR2(50) |  |  |  |  |

**Índices:**

- `PK_AN_TIPO_AGRUPACION_CDC` (UNIQUE) — COD_TIPO_AGRUPACION_CDC

### AN_TIPO_ORGANIZACION

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_TIPO_ORGANIZACION | NUMBER |  | PK |  |  |
| DES_TIPO_ORGANIZACION | VARCHAR2(50) |  |  |  |  |

**Índices:**

- `PK_AN_TIPO_ORGANIZACION` (UNIQUE) — COD_TIPO_ORGANIZACION

### AN_VERTICAL

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_VERTICAL | NUMBER |  | PK |  |  |
| DES_VERTICAL | VARCHAR2(50) |  |  |  |  |
| IND_ESTADO_VERTICAL | NUMBER |  |  |  | 1: Visible app 2: Tratado en "TODO" 4: FRONT O RESEARCH 8: GLOBAL -- Es global para el reparto de porcentajes automáticos.  16: SERVICE -- Es services para el reparto de porcentajes automáticos.  32: ESPAÑA -- Es España para el reparto de porcentajes automáticos |
| F_BAJA | DATE | ✓ |  |  |  |
| IND_ON | NUMBER |  |  |  | Indica si la vertical es ON o no en el caso de las especialidades que puedan ser ON/OFF |

**Índices:**

- `PK_AN_VERTICAL` (UNIQUE) — COD_VERTICAL

### AN_VIRTUAL

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_VIRTUAL | NUMBER |  | PK |  |  |
| DES_VIRTUAL | VARCHAR2(50) |  |  |  |  |
| DES_VIRTUAL_ABREV | VARCHAR2(50) |  |  |  |  |
| COD_COMPANIA | NUMBER |  |  |  | Compañía estaturaria que se utilizará en el proceso. |
| IND_ESTADO_VIRTUAL | NUMBER |  |  |  | 1: Visible app 2: Tratado en "TODO" 4: FRONT O RESEARCH 8: GLOBAL -- Es global para el reparto de porcentajes automáticos.  16: SERVICE -- Es services para el reparto de porcentajes automáticos.  32: ESPAÑA -- Es España para el reparto de porcentajes automáticos 64: Proximia. Para que se muestren las oficinas 128: Havas Madrid  256: Havas Madrid 0 |
| COD_GRUPO_ENTORNO | NUMBER |  |  |  | Se utiliza inicialmente para el dashboard y poder identificar en que grupo está aunque no tenga mapping |
| F_BAJA | DATE | ✓ |  |  |  |
| COD_ENTORNO_SEGURIDAD | NUMBER | ✓ |  |  | De momento, para la seguridad de los miners de TimeSheet |
| COD_UBICACION | NUMBER |  |  | → PY_UBICACION |  |

**Índices:**

- `IDX_AN_VIRUTAL_COD_GRUPO_ENTRN` — COD_GRUPO_ENTORNO
- `PK_AN_VIRTUAL` (UNIQUE) — COD_VIRTUAL

### AUDIT_TS_GRUPO_NO_DISTR_VIRT

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| SECUENCIA | NUMBER |  | PK |  |  |
| COD_PAIS_OLD | NUMBER | ✓ |  |  |  |
| COD_PAIS_NEW | NUMBER | ✓ |  |  |  |
| COD_GRUPO_OLD | NUMBER | ✓ |  |  |  |
| COD_GRUPO_NEW | NUMBER | ✓ |  |  |  |
| F_MODIFICACION_OLD | DATE | ✓ |  |  |  |
| F_MODIFICACION_NEW | DATE | ✓ |  |  |  |
| COD_USUARIO_MODIFICACION_OLD | NUMBER | ✓ |  |  |  |
| COD_USUARIO_MODIFICACION_NEW | NUMBER | ✓ |  |  |  |

**Índices:**

- `PK_AUDIT_TS_GRUPO_NO_DISTR_VRT` (UNIQUE) — SECUENCIA

### AUDIT_TS_IMPUTA

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| SECUENCIA | NUMBER |  | PK |  |  |
| COD_TS_IMPUTA | NUMBER | ✓ |  |  |  |
| COD_EMPLEADO_OLD | NUMBER | ✓ |  |  |  |
| COD_EMPLEADO_NEW | NUMBER | ✓ |  |  |  |
| COD_PAIS_OLD | NUMBER | ✓ |  |  |  |
| COD_PAIS_NEW | NUMBER | ✓ |  |  |  |
| ANIO_OLD | NUMBER | ✓ |  |  |  |
| ANIO_NEW | NUMBER | ✓ |  |  |  |
| COD_VERSION_OLD | NUMBER | ✓ |  |  |  |
| COD_VERSION_NEW | NUMBER | ✓ |  |  |  |
| IND_TODOS_GRUPOS_OLD | NUMBER | ✓ |  |  |  |
| IND_TODOS_GRUPOS_NEW | NUMBER | ✓ |  |  |  |
| IND_TODOS_ENTORNOS_OLD | VARCHAR2(50) | ✓ |  |  |  |
| IND_TODOS_ENTORNOS_NEW | NUMBER | ✓ |  |  |  |
| IND_JIRA_OLD | NUMBER | ✓ |  |  |  |
| IND_JIRA_NEW | NUMBER | ✓ |  |  |  |
| F_CONFIRMACION_OLD | DATE | ✓ |  |  |  |
| F_CONFIRMACION_NEW | DATE | ✓ |  |  |  |
| PORCENTAJE_GRUPO_OLD | NUMBER(6,3) | ✓ |  |  |  |
| PORCENTAJE_GRUPO_NEW | NUMBER(6,3) | ✓ |  |  |  |
| PORCENTAJE_GRUPO_ENTORNO_OLD | NUMBER(6,3) | ✓ |  |  |  |
| PORCENTAJE_GRUPO_ENTORNO_NEW | NUMBER(6,3) | ✓ |  |  |  |
| F_MODIFICACION_OLD | DATE | ✓ |  |  |  |
| F_MODIFICACION_NEW | DATE | ✓ |  |  |  |
| COD_USUARIO_MODIFICACION_OLD | NUMBER | ✓ |  |  |  |
| COD_USUARIO_MODIFICACION_NEW | NUMBER | ✓ |  |  |  |
| F_LOG | DATE |  |  |  | Fecha en que se genera el log. |

**Índices:**

- `PK_AUDIT_TS_IMPUTA` (UNIQUE) — SECUENCIA

### AUDIT_TS_IMPUTA_GRUPO

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| SECUENCIA | NUMBER |  | PK |  |  |
| COD_TS_IMPUTA | NUMBER | ✓ |  |  |  |
| COD_PAIS | NUMBER | ✓ |  |  |  |
| COD_GRUPO | NUMBER | ✓ |  |  |  |
| MES | NUMBER | ✓ |  |  |  |
| IND_AUTOMATICO_OLD | NUMBER | ✓ |  |  |  |
| IND_AUTOMATICO_NEW | NUMBER | ✓ |  |  |  |
| PORCENTAJE_OLD | NUMBER(6,3) | ✓ |  |  |  |
| PORCENTAJE_NEW | NUMBER(6,3) | ✓ |  |  |  |
| F_MODIFICACION_OLD | DATE | ✓ |  |  |  |
| F_MODIFICACION_NEW | DATE | ✓ |  |  |  |
| COD_USUARIO_MODIFICACION_OLD | NUMBER | ✓ |  |  |  |
| COD_USUARIO_MODIFICACION_NEW | NUMBER | ✓ |  |  |  |
| F_LOG | DATE |  |  |  | Fecha en que se genera el log |

**Índices:**

- `PK_AUDIT_TS_IMPUTA_GRUPO` (UNIQUE) — SECUENCIA

### AUDIT_TS_IMPUTA_GRUPO_ENTORNO

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| SECUENCIA | NUMBER |  | PK |  |  |
| COD_TS_IMPUTA | NUMBER | ✓ |  |  |  |
| COD_GRUPO_ENTORNO | NUMBER | ✓ |  |  |  |
| MES | NUMBER | ✓ |  |  |  |
| IND_AUTOMATICO_OLD | NUMBER | ✓ |  |  |  |
| IND_AUTOMATICO_NEW | NUMBER | ✓ |  |  |  |
| PORCENTAJE_OLD | NUMBER(6,3) | ✓ |  |  |  |
| PORCENTAJE_NEW | NUMBER(6,3) | ✓ |  |  |  |
| F_MODIFICACION_OLD | DATE | ✓ |  |  |  |
| F_MODIFICACION_NEW | DATE | ✓ |  |  |  |
| COD_USUARIO_MODIFICACION_OLD | NUMBER | ✓ |  |  |  |
| COD_USUARIO_MODIFICACION_NEW | VARCHAR2(50) | ✓ |  |  |  |
| F_LOG | DATE |  |  |  | Fecha en que se genera el log |

**Índices:**

- `PK_AUDIT_TS_IMPUTA__GRUPO_ENT` (UNIQUE) — SECUENCIA

### AUDIT_TS_PY_EMPLEADO

Tabla que audita si un empleado cambio tanto de indicador de virtual o no o de tipo network

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| SECUENCIA | NUMBER |  | PK |  |  |
| COD_EMPLEADO | NUMBER |  |  |  |  |
| IND_TS_VIRTUAL_OLD | NUMBER |  |  |  |  |
| IND_TS_VIRTUAL_NEW | NUMBER |  |  |  |  |
| COD_TIPO_NETWORK_OLD | NUMBER |  |  |  |  |
| COD_TIPO_NETWORK_NEW | NUMBER |  |  |  |  |
| F_MODIFICACION | DATE |  |  |  |  |
| COD_USUARIO_MODIFCACION | NUMBER |  |  |  |  |

**Índices:**

- `PK_AUDIT_TS_PY_EMPLEADOLEADO` (UNIQUE) — SECUENCIA

### BACK_AN_DIARIO_EMPLEADO_PORCE

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS_ANIO | NUMBER |  |  |  | Para la particion puesto que se puede repitir una versión en varios paises |
| COD_PAIS | NUMBER |  |  |  |  |
| ANIO | NUMBER |  |  |  |  |
| COD_VERSION | NUMBER |  |  |  |  |
| COD_EMPLEADO | NUMBER |  |  |  |  |
| COD_VIRTUAL | NUMBER |  |  |  |  |
| COD_VERTICAL | NUMBER |  |  |  |  |
| COD_ESPECIALIDAD | NUMBER |  |  |  |  |
| COD_OFICINA | NUMBER |  |  |  |  |
| MES | NUMBER |  |  |  |  |
| ID_CDC_ORIGEN | NUMBER |  |  |  |  |
| COD_TIPO_AGRUPACION_CDC_ORI | NUMBER |  |  |  |  |
| ID_CDC_DESTINO | NUMBER |  |  |  |  |
| COD_TIPO_AGRUPACION_CDC_DEST | NUMBER |  |  |  |  |
| IMPORTE | NUMBER |  |  |  |  |
| PORCENTAJE | NUMBER |  |  |  |  |
| IMPORTE_ACUMULADO | NUMBER |  |  |  | Importe acumulado hasta el mes |
| PORCENTAJE_ACUMULADO | NUMBER |  |  |  | Porcentaje con respecto al importe acumulado |
| COD_USUARIO_CARGA | NUMBER |  |  |  |  |
| F_CARGA | DATE |  |  |  |  |
| COM_0_MES | NUMBER | ✓ |  |  |  |
| COM_23_MES | NUMBER | ✓ |  |  |  |
| COM_1_MES | NUMBER | ✓ |  |  |  |
| COM_2_MES | NUMBER | ✓ |  |  |  |
| COM_3_MES | NUMBER | ✓ |  |  |  |
| COM_4_MES | NUMBER | ✓ |  |  |  |
| COM_5_MES | NUMBER | ✓ |  |  |  |
| COM_6_MES | NUMBER | ✓ |  |  |  |
| COM_7_MES | NUMBER | ✓ |  |  |  |
| COM_8_MES | NUMBER | ✓ |  |  |  |
| COM_9_MES | NUMBER | ✓ |  |  |  |
| COM_10_MES | NUMBER | ✓ |  |  |  |
| COM_11_MES | NUMBER | ✓ |  |  |  |
| COM_12_MES | NUMBER | ✓ |  |  |  |
| COM_13_MES | NUMBER | ✓ |  |  |  |
| COM_14_MES | NUMBER | ✓ |  |  |  |
| COM_15_MES | NUMBER | ✓ |  |  |  |
| COM_16_MES | NUMBER | ✓ |  |  |  |
| COM_17_MES | NUMBER | ✓ |  |  |  |
| COM_18_MES | NUMBER | ✓ |  |  |  |
| COM_19_MES | NUMBER | ✓ |  |  |  |
| COM_20_MES | NUMBER | ✓ |  |  |  |
| COM_21_MES | NUMBER | ✓ |  |  |  |
| COM_22_MES | NUMBER | ✓ |  |  |  |
| COM_0_ACU | NUMBER | ✓ |  |  |  |
| COM_23_ACU | NUMBER | ✓ |  |  |  |
| COM_1_ACU | NUMBER | ✓ |  |  |  |
| COM_2_ACU | NUMBER | ✓ |  |  |  |
| COM_3_ACU | NUMBER | ✓ |  |  |  |
| COM_4_ACU | NUMBER | ✓ |  |  |  |
| COM_5_ACU | NUMBER | ✓ |  |  |  |
| COM_6_ACU | NUMBER | ✓ |  |  |  |
| COM_7_ACU | NUMBER | ✓ |  |  |  |
| COM_8_ACU | NUMBER | ✓ |  |  |  |
| COM_9_ACU | NUMBER | ✓ |  |  |  |
| COM_10_ACU | NUMBER | ✓ |  |  |  |
| COM_11_ACU | NUMBER | ✓ |  |  |  |
| COM_12_ACU | NUMBER | ✓ |  |  |  |
| COM_13_ACU | NUMBER | ✓ |  |  |  |
| COM_14_ACU | NUMBER | ✓ |  |  |  |
| COM_15_ACU | NUMBER | ✓ |  |  |  |
| COM_16_ACU | NUMBER | ✓ |  |  |  |
| COM_17_ACU | NUMBER | ✓ |  |  |  |
| COM_18_ACU | NUMBER | ✓ |  |  |  |
| COM_19_ACU | NUMBER | ✓ |  |  |  |
| COM_20_ACU | NUMBER | ✓ |  |  |  |
| COM_21_ACU | NUMBER | ✓ |  |  |  |
| COM_22_ACU | NUMBER | ✓ |  |  |  |
| COD_PURE_PLAYER | NUMBER |  |  |  | Realmente es "equipo". Cuando se decidió cambiar el coste de cambiar el nombre de bbdd es alto |
| COD_DEPARTAMENTO | NUMBER | ✓ |  |  |  |

### BACK_EMP_PCT_GR_ENT_NO_INV

TABLA CON LOS GRUPOS PRECALCULADOS. GRUPO ENTORNO SIN INVERSION EN ORIGEN

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER |  |  |  |  |
| ANIO | NUMBER |  |  |  |  |
| COD_VERSION | NUMBER |  |  |  |  |
| COD_EMPLEADO | NUMBER |  |  |  |  |
| COD_GRUPO_ENTORNO | NUMBER |  |  |  |  |
| COD_VIRTUAL | NUMBER |  |  |  |  |
| COD_VERTICAL | NUMBER |  |  |  |  |
| COD_ESPECIALIDAD | NUMBER |  |  |  |  |
| COD_PURE_PLAYER | NUMBER |  |  |  |  |
| COD_DEPARTAMENTO | NUMBER | ✓ |  |  |  |
| COD_OFICINA | NUMBER |  |  |  |  |
| ID_CDC_DESTINO | NUMBER |  |  |  |  |
| MES | NUMBER |  |  |  |  |
| IMPORTE | NUMBER |  |  |  |  |
| IMPORTE_ACUMULADO | NUMBER |  |  |  |  |
| PCT | NUMBER |  |  |  |  |
| PCT_ACUM | NUMBER |  |  |  |  |

### BACK_TA_GRUPO_ENTORNO

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_TS_IMPUTA | NUMBER |  |  |  |  |
| COD_GRUPO_ENTORNO | NUMBER |  |  |  |  |
| MES | NUMBER |  |  |  |  |
| IND_AUTOMATICO | NUMBER |  |  |  | 0: Manual 1: Automático (se obtiene de billings) |
| PORCENTAJE | NUMBER(6,3) |  |  |  |  |
| F_MODIFICACION | DATE |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |

### BACK_TS_CALC_EMP_PCT_GRUPO

TABLA CON LOS GRUPOS PRECALCULADOS. TANTO DE ENTORNO CON INVERSION COMO DE GRUPO CON/SIN INVERSION

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER |  |  |  |  |
| ANIO | NUMBER |  |  |  |  |
| COD_VERSION | NUMBER |  |  |  |  |
| COD_EMPLEADO | NUMBER |  |  |  |  |
| COD_GRUPO | NUMBER |  |  |  |  |
| COD_VIRTUAL | NUMBER |  |  |  |  |
| COD_VERTICAL | NUMBER |  |  |  |  |
| COD_ESPECIALIDAD | NUMBER |  |  |  |  |
| COD_PURE_PLAYER | NUMBER |  |  |  |  |
| COD_DEPARTAMENTO | NUMBER | ✓ |  |  |  |
| COD_OFICINA | NUMBER |  |  |  |  |
| ID_CDC_DESTINO | NUMBER |  |  |  |  |
| MES | NUMBER |  |  |  |  |
| IMPORTE | NUMBER |  |  |  |  |
| IMPORTE_ACUMULADO | NUMBER |  |  |  |  |
| PCT | NUMBER |  |  |  |  |
| PCT_ACUM | NUMBER |  |  |  |  |

### BACK_TS_CONSOLIDADO

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER |  |  |  |  |
| ANIO | NUMBER |  |  |  |  |
| COD_VERSION | NUMBER |  |  |  |  |
| MES | NUMBER |  |  |  |  |
| COD_EMPLEADO | NUMBER | ✓ |  |  |  |
| COD_COMPANIA | NUMBER | ✓ |  |  |  |
| COD_GRUPO | NUMBER | ✓ |  |  |  |
| COD_VIRTUAL | NUMBER | ✓ |  |  |  |
| COD_GRUPO_ENTORNO | NUMBER | ✓ |  |  |  |
| COD_VERTICAL | NUMBER | ✓ |  |  |  |
| COD_MEDIO | NUMBER | ✓ |  |  |  |
| COD_ESPECIALIDAD | NUMBER | ✓ |  |  |  |
| COD_ESPECIALIDAD_PPTO | NUMBER | ✓ |  |  |  |
| COD_OFICINA | NUMBER | ✓ |  |  |  |
| IMP_NETO_VENTA | NUMBER | ✓ |  |  |  |
| IMP_MARGEN_BRUTO | NUMBER | ✓ |  |  |  |
| COSTE | NUMBER | ✓ |  |  |  |
| FTE | NUMBER | ✓ |  |  |  |
| FTE_PRORRATEADO | NUMBER | ✓ |  |  |  |
| COD_ORIGEN_CONSOLIDADO | NUMBER |  |  |  |  |
| COD_USUARIO | NUMBER |  |  |  | Usuario que lo ejecuta -1 Sistemas, desconocido |
| F_ACTUALIZACION | DATE | ✓ |  |  |  |
| SBA | NUMBER |  |  |  |  |
| AYUDA_ALIMENTARIA | NUMBER |  |  |  |  |
| INGRESO_CUENTA_ESPECIE | NUMBER |  |  |  |  |
| SEGURO | NUMBER |  |  |  |  |
| OTROS_SEGUROS | NUMBER |  |  |  |  |
| AHORROS_SEGURO_VIDA | NUMBER |  |  |  |  |
| EXTRA_BONUS | NUMBER |  |  |  |  |
| SEGURIDAD_SOCIAL | NUMBER |  |  |  |  |
| OTROS_AHORROS_SS | NUMBER |  |  |  |  |
| LOCOMOCION | NUMBER |  |  |  |  |
| FORMACION | NUMBER |  |  |  |  |
| BONUS | NUMBER |  |  |  |  |
| APPORTACION_PJ | NUMBER |  |  |  |  |
| ESPECIE_CORPORATIVA | NUMBER |  |  |  |  |
| INDEMNIZACION | NUMBER |  |  |  |  |

### BACK_TS_IMPUTA

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_TS_IMPUTA | NUMBER |  |  |  |  |
| COD_EMPLEADO | NUMBER |  |  |  |  |
| COD_PAIS | NUMBER |  |  |  |  |
| ANIO | NUMBER |  |  |  |  |
| COD_VERSION | NUMBER |  |  |  |  |
| IND_TODOS_GRUPOS | NUMBER |  |  |  | 0: No son todos sus grupos. Necesita datos en TS_IMPUTA_GRUPO 1: Tienen todos los grupos |
| IND_TODOS_ENTORNOS | NUMBER |  |  |  | 0: No son todos sus entornos. Necesita datos en TS_IMPUTA_GRUPO 1: Tienen todos los entornos |
| IND_JIRA | NUMBER | ✓ |  |  | 1: Viene de JIRA |
| F_CONFIRMACION | DATE | ✓ |  |  |  |
| PORCENTAJE_GRUPO | NUMBER(6,3) |  |  |  | Porcentaje que se distribuye por grupo. Si es 0 no debería haber ningún regustro de TS_IMPUTA_GRUPO |
| PORCENTAJE_GRUPO_ENTORNO | NUMBER(6,3) |  |  |  | Porcentaje que se distribuye por grupo entorno. Si es 0 no debería haber ningún regustro de TS_IMPUTA_GRUPO_ENTORNO |
| F_MODIFICACION | DATE |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |

### BACK_TS_IMPUTA_GRUPO

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_TS_IMPUTA | NUMBER |  |  |  |  |
| COD_PAIS | NUMBER |  |  |  |  |
| COD_GRUPO | NUMBER |  |  |  | De momento solo se grabarán los grupos principales |
| MES | NUMBER |  |  |  |  |
| IND_AUTOMATICO | NUMBER |  |  |  | 0: Manual 1: Automático (se obtiene de billings) |
| PORCENTAJE | NUMBER(6,3) |  |  |  |  |
| F_MODIFICACION | DATE |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |

### DBTOOLS$MCP_LOG

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| ID | NUMBER |  | PK |  |  |
| MCP_CLIENT | VARCHAR2(200) |  |  |  |  |
| MODEL | VARCHAR2(200) | ✓ |  |  |  |
| END_POINT_TYPE | VARCHAR2(12) | ✓ |  |  |  |
| END_POINT_NAME | VARCHAR2(100) |  |  |  |  |
| LOG_MESSAGE | CLOB | ✓ |  |  |  |
| CREATED_ON | TIMESTAMP(6) |  |  |  |  |
| CREATED_BY | VARCHAR2(100) |  |  |  |  |
| UPDATED_ON | TIMESTAMP(6) | ✓ |  |  |  |
| UPDATED_BY | VARCHAR2(100) | ✓ |  |  |  |

**Índices:**

- `SYS_C001090506` (UNIQUE) — ID

### GRUPO_NAME_CC

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_GRUPO_NAME_CC | NUMBER |  | PK |  |  |
| DES_GRUPO_NAME_CC | VARCHAR2(50) |  |  |  |  |
| F_ALTA | DATE |  |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |
| OBSERVACIONES | CLOB | ✓ |  |  |  |

**Índices:**

- `PK_GRUPO_NAME_CC` (UNIQUE) — COD_GRUPO_NAME_CC

### LOG_IMPUTA_OTHER_EXPENSES_TMP

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| NUM_INFORME | NUMBER | ✓ |  |  |  |

### LOG_OTHER_EXPENSES

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| NUM_INFORME | NUMBER | ✓ |  |  |  |

### LOG_PROCESO

Tabla para guardar el log de los procesos que necesitan log

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_TRAZA | NUMBER |  | PK |  |  |
| PROCESO | CLOB |  |  |  |  |
| SUBPROCESO | CLOB | ✓ |  |  |  |
| F_INICIO | DATE |  |  |  |  |
| F_FIN | DATE | ✓ |  |  |  |
| COD_USUARIO | NUMBER |  |  |  |  |
| RESULTADO | CLOB | ✓ |  |  |  |
| SQL | CLOB | ✓ |  |  | Por si queremos guarda alguna query |

**Índices:**

- `PK_LOG_PROCESO` (UNIQUE) — COD_TRAZA

### MAIL_CONFIG

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| DE | VARCHAR2(500) | ✓ |  |  |  |
| PARA_ERROR | VARCHAR2(1000) | ✓ |  |  |  |
| TRAZA | NUMBER(1,0) | ✓ |  |  |  |
| PARA_TRAZA | VARCHAR2(500) | ✓ |  |  |  |
| APP | NUMBER | ✓ |  |  | 1 - CODIGO APP PRESUPUESTOS
2 - CODIGO APP IMPUTACIONES Y RRHH
3 - CODIGO APP IPUTACIONES Y RRHH SOLO NIFICACION TI |

### MAP_NMD_TO_MEDIO_NETWORK_TIPO

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_OBJETIVO | NUMBER |  | PK |  |  |
| COD_DISCIPLINA | NUMBER |  | PK |  |  |
| COD_ALCANCE | NUMBER |  | PK |  |  |
| COD_TIPO_COMPRA | NUMBER |  | PK |  |  |
| COD_DIVERSIFIED | NUMBER |  | PK |  |  |
| COD_MEDIO | NUMBER |  | PK |  |  |
| COD_NETWORK | NUMBER |  | PK |  |  |
| COD_TIPO_PRESUPUESTO | NUMBER | ✓ |  |  |  |

**Índices:**

- `PK_MAP_NMD_TO_MEDIO_NETWORK_TIPO` (UNIQUE) — COD_OBJETIVO, COD_DISCIPLINA, COD_ALCANCE, COD_TIPO_COMPRA, COD_DIVERSIFIED, COD_MEDIO, COD_NETWORK

### MGN_INFORME

Entidad que guarda los informes que se reportan a Magnitude y los conceptos que se reportan en ese informe

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_INFORME | NUMBER |  | PK |  |  |
| DES_INFORME | VARCHAR2(100) |  |  |  |  |

**Índices:**

- `PK_MGN_INFORME` (UNIQUE) — COD_INFORME

### MGN_INFORME_COLUMNA

Columnas que se reportan en los informes de MAGNITUDE. 

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_INFORME_COLUMNA | NUMBER |  | PK |  |  |
| COD_INFORME | NUMBER |  |  | → MGN_INFORME |  |
| ORDEN_COLUMNA | NUMBER |  |  |  | Orden en el que salen las columnas en el fichero |
| DES_COLUMNA | VARCHAR2(25) |  |  |  | Cabecera de la columna |
| TIPO_COLUMNA | NUMBER |  |  |  | Indica si la columna se calcula a nivel de cabecera (0) --> una vez para todo el informe porque mantiene el mismo valor en todo el informe o  a nivel de línea (1) --> depende de los valores leídos del registro de previsiones |
| IND_NULO | NUMBER(1,0) |  |  |  | 0 - No es nulo, por lo que se calcula llamando al paquete PKG_MAGNITUDE.FUNCION siendo FUNCION = nombre la columna  1 - Nulo |

**Índices:**

- `PK_MGN_INFORME_COLUMNA` (UNIQUE) — COD_INFORME_COLUMNA
- `UQ_MGN_INFORME_COLUMNA` (UNIQUE) — COD_INFORME, DES_COLUMNA
- `UQ_MGN_INFORME_COLUMNA_2` (UNIQUE) — COD_INFORME, ORDEN_COLUMNA

### MGN_INFORME_NETWORK

Indica qué networks salen de cada informe y las columnas que hay que reportar

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_INFORME | NUMBER |  | PK | → MGN_INFORME |  |
| COD_NETWORK | NUMBER |  | PK |  |  |
| BIT_CONCEPTO_PRESUPUESTOS | NUMBER |  |  |  | BITAND para indicar qué conceptos se reportan |

**Índices:**

- `PK_MGN_INFORME_NETWORK` (UNIQUE) — COD_INFORME, COD_NETWORK

### MGN_NETWORK_PPLAYER

Relación entre Network y Pure Player que se reportan en Magnitude

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_NETWORK | NUMBER |  | PK |  |  |
| COD_PURE_PLAYER | NUMBER |  | PK | → PPT_PURE_PLAYER |  |

**Índices:**

- `PK_MGN_NETWORK_PPLAYER` (UNIQUE) — COD_NETWORK, COD_PURE_PLAYER

### MGN_PARAM

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_INFORME_COLUMNA | NUMBER |  | PK | → MGN_INFORME_COLUMNA |  |
| BIT_CONCEPTO_PRESUPUESTOS | NUMBER |  | PK |  |  |
| IND_SIGNO | NUMBER(1,0) |  |  |  |  |
| VALOR | VARCHAR2(50) |  |  |  |  |

**Índices:**

- `PK_MGN_PARAM` (UNIQUE) — COD_INFORME_COLUMNA, BIT_CONCEPTO_PRESUPUESTOS

### MGN_PARAM_NETWORK

Parametrización de todos los conceptos y columnas que dependen de la network

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_INFORME_COLUMNA | NUMBER |  | PK | → MGN_INFORME_COLUMNA |  |
| COD_NETWORK | NUMBER |  | PK |  | Referencia a COD_NETWORK de la tabla NETWORK |
| BIT_CONCEPTO_PRESUPUESTOS | NUMBER |  | PK |  | Máscara de bits para los conceptos de presupuestos que están parametrizados en este registro |
| IND_SIGNO | NUMBER(1,0) |  | PK |  | Indica el valor que se coge cuando el signo del importe a reportar es positivo (1), negativo (-1) o ambos (0) |
| VALOR | VARCHAR2(50) |  |  |  | Valor a devolver. Si la columna que se está parametrizando es la del IMPORTE (P_AMOUNT) es el valor por el que se multiplica el i mporte, porque a veces se devuelve el importe cambiado de signo y en otros se devulve un 0 |

**Índices:**

- `PK_MGN_PARAM_NETWORK` (UNIQUE) — COD_NETWORK, COD_INFORME_COLUMNA, BIT_CONCEPTO_PRESUPUESTOS, IND_SIGNO

### MGN_PARAM_NETWORK_PPLAYER

Parametrización de todos los conceptos y columnas que dependen de la network + Pure Player

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_INFORME_COLUMNA | NUMBER |  | PK | → MGN_INFORME_COLUMNA |  |
| COD_NETWORK | NUMBER |  | PK |  |  |
| COD_PURE_PLAYER | NUMBER |  | PK | → PPT_PURE_PLAYER |  |
| BIT_CONCEPTO_PRESUPUESTOS | NUMBER |  | PK |  |  |
| IND_SIGNO | NUMBER(1,0) |  | PK |  | Indica el valor que se coge cuando el signo del importe a reportar es positivo (1), negativo (-1) o ambos (0) |
| VALOR | VARCHAR2(50) |  |  |  | Valor a devolver. Si la columna que se está parametrizando es la del IMPORTE (P_AMOUNT) es el valor por el que se multiplica el i mporte, porque a veces se devuelve el importe cambiado de signo y en otros se devulve un 0 |

**Índices:**

- `PK_MGN_PARAM_NETWORK_PPLAYER` (UNIQUE) — COD_NETWORK, COD_INFORME_COLUMNA, IND_SIGNO, COD_PURE_PLAYER, BIT_CONCEPTO_PRESUPUESTOS

### MINER_OTHER_EXP_2022_CODDETALLE

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER | ✓ |  |  |  |
| ANIO | NUMBER | ✓ |  |  |  |
| ANIO_CONTABLE | NUMBER | ✓ |  |  |  |
| MES_CONTABLE | NUMBER | ✓ |  |  |  |
| COD_VERSION | NUMBER | ✓ |  |  |  |
| DES_VERSION | VARCHAR2(40) | ✓ |  |  |  |
| MES | NUMBER | ✓ |  |  |  |
| NUM_INFORME | VARCHAR2(50) | ✓ |  |  |  |
| IND_REFACTURABLE | VARCHAR2(1) | ✓ |  |  |  |
| IND_CALCULO_HC | VARCHAR2(1) | ✓ |  |  |  |
| COD_EMPLEADO | NUMBER | ✓ |  |  |  |
| NOMBRE | VARCHAR2(100) | ✓ |  |  |  |
| APELLIDO1 | VARCHAR2(100) | ✓ |  |  |  |
| APELLIDO2 | VARCHAR2(100) | ✓ |  |  |  |
| NOMBRE_COMPLETO | VARCHAR2(152) | ✓ |  |  |  |
| F_ALTA | DATE | ✓ |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |
| COD_VIRTUAL | NUMBER | ✓ |  |  |  |
| DES_VIRTUAL | VARCHAR2(50) | ✓ |  |  |  |
| COD_VERTICAL | NUMBER | ✓ |  |  |  |
| DES_VERTICAL | VARCHAR2(50) | ✓ |  |  |  |
| COD_ESPECIALIDAD | NUMBER | ✓ |  |  |  |
| DES_ESPECIALIDAD | VARCHAR2(50) | ✓ |  |  |  |
| COD_GRUPO_ESPECIALIDAD | VARCHAR2(40) | ✓ |  |  |  |
| DES_GRUPO_ESPECIALIDAD | VARCHAR2(50) | ✓ |  |  |  |
| COD_OFICINA | NUMBER | ✓ |  |  |  |
| DES_OFICINA | VARCHAR2(50) | ✓ |  |  |  |
| COD_CDC_ORIGEN | VARCHAR2(150) | ✓ |  |  |  |
| COD_COMPANIA_ORIGEN | NUMBER | ✓ |  |  |  |
| DES_CDC_ORIGEN | VARCHAR2(240) | ✓ |  |  |  |
| DES_COMPANIA_ORIGEN | VARCHAR2(40) | ✓ |  |  |  |
| COD_CDC_DESTINO | VARCHAR2(4000) | ✓ |  |  |  |
| COD_COMPANIA_DESTINO | NUMBER | ✓ |  |  |  |
| DES_CDC_DESTINO | VARCHAR2(240) | ✓ |  |  |  |
| DES_COMPANIA_DESTINO | VARCHAR2(40) | ✓ |  |  |  |
| COD_TIPO_AGRUPACION_DESTINO | NUMBER | ✓ |  |  |  |
| CUENTA_CONTABLE | VARCHAR2(150) | ✓ |  |  |  |
| COD_CONCEPTO_OE | VARCHAR2(25) | ✓ |  |  |  |
| DES_CUENTA_CONTABLE | VARCHAR2(240) | ✓ |  |  |  |
| COD_ORIGEN_GASTO | NUMBER | ✓ |  |  |  |
| DES_ORIGEN_GASTO | VARCHAR2(50) | ✓ |  |  |  |
| NUMERO_SOLICITUD | VARCHAR2(100) | ✓ |  |  |  |
| NUMERO_PEDIDO | VARCHAR2(100) | ✓ |  |  |  |
| SOLICITANTE | VARCHAR2(240) | ✓ |  |  |  |
| FECHA_DESDE_SOLPED | DATE | ✓ |  |  |  |
| FECHA_HASTA_SOLPED | DATE | ✓ |  |  |  |
| COMPRADOR | VARCHAR2(240) | ✓ |  |  |  |
| DESCRIPCION_PEDIDO | VARCHAR2(240) | ✓ |  |  |  |
| MATRICULA | VARCHAR2(150) | ✓ |  |  |  |
| COD_EMPLEADO_MATRICULA | NUMBER | ✓ |  |  |  |
| EMPLEADO_MATRICULA | VARCHAR2(152) | ✓ |  |  |  |
| UBICACION_ARRENDAMINETO | VARCHAR2(100) | ✓ |  |  |  |
| UBICACION_RECURRENTE | VARCHAR2(100) | ✓ |  |  |  |
| COD_GRUPO_PARKING | NUMBER | ✓ |  |  |  |
| DES_GRUPO_PARKING | VARCHAR2(100) | ✓ |  |  |  |
| NUM_FACTURA | VARCHAR2(240) | ✓ |  |  |  |
| FECHA_FACTURA | DATE | ✓ |  |  |  |
| COD_PROVEEDOR | VARCHAR2(80) | ✓ |  |  |  |
| DES_PROVEEDOR | VARCHAR2(360) | ✓ |  |  |  |
| FECHA_GASTO_IEXPENSES | DATE | ✓ |  |  |  |
| ERROR | VARCHAR2(100) | ✓ |  |  |  |
| DES_ERROR | VARCHAR2(100) | ✓ |  |  |  |
| DES_TIPO_AGRUPACION_DESTINO | VARCHAR2(50) | ✓ |  |  |  |
| DESCRIPCION_LINEA | VARCHAR2(2000) | ✓ |  |  |  |
| DES_ASIENTO | VARCHAR2(2000) | ✓ |  |  |  |
| NOMBRE_LOTE | VARCHAR2(100) | ✓ |  |  |  |
| PORCENTAJE | NUMBER | ✓ |  |  |  |
| IMPORTE | NUMBER | ✓ |  |  |  |
| COD_CONCEPTO | NUMBER | ✓ |  |  |  |

**Índices:**

- `IND_MINER_OTHER_EXP_2022_DETALLE_N1` — ANIO_CONTABLE, MES_CONTABLE, COD_COMPANIA_ORIGEN, COD_ORIGEN_GASTO

### MINER_OTHER_EXP_2022_CODDETALLE_02

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER | ✓ |  |  |  |
| ANIO | NUMBER | ✓ |  |  |  |
| ANIO_CONTABLE | NUMBER | ✓ |  |  |  |
| MES_CONTABLE | NUMBER | ✓ |  |  |  |
| COD_VERSION | NUMBER |  |  |  |  |
| DES_VERSION | VARCHAR2(40) | ✓ |  |  |  |
| MES | NUMBER |  |  |  |  |
| NUM_INFORME | VARCHAR2(50) | ✓ |  |  |  |
| IND_REFACTURABLE | VARCHAR2(1) | ✓ |  |  |  |
| IND_CALCULO_HC | VARCHAR2(1) | ✓ |  |  |  |
| COD_EMPLEADO | NUMBER | ✓ |  |  |  |
| NOMBRE | VARCHAR2(100) | ✓ |  |  |  |
| APELLIDO1 | VARCHAR2(100) | ✓ |  |  |  |
| APELLIDO2 | VARCHAR2(100) | ✓ |  |  |  |
| NOMBRE_COMPLETO | VARCHAR2(100) | ✓ |  |  |  |
| F_ALTA | DATE | ✓ |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |
| COD_VIRTUAL | NUMBER |  |  |  |  |
| DES_VIRTUAL | VARCHAR2(50) |  |  |  |  |
| COD_VERTICAL | NUMBER |  |  |  |  |
| DES_VERTICAL | VARCHAR2(50) |  |  |  |  |
| COD_ESPECIALIDAD | NUMBER | ✓ |  |  |  |
| DES_ESPECIALIDAD | VARCHAR2(50) | ✓ |  |  |  |
| COD_GRUPO_ESPECIALIDAD | NUMBER | ✓ |  |  |  |
| DES_GRUPO_ESPECIALIDAD | VARCHAR2(50) | ✓ |  |  |  |
| COD_OFICINA | NUMBER |  |  |  |  |
| DES_OFICINA | VARCHAR2(50) |  |  |  |  |
| COD_CDC_ORIGEN | VARCHAR2(150) | ✓ |  |  |  |
| COD_COMPANIA_ORIGEN | NUMBER | ✓ |  |  |  |
| DES_CDC_ORIGEN | VARCHAR2(240) | ✓ |  |  |  |
| DES_COMPANIA_ORIGEN | VARCHAR2(40) | ✓ |  |  |  |
| COD_CDC_DESTINO | VARCHAR2(4000) | ✓ |  |  |  |
| COD_COMPANIA_DESTINO | NUMBER | ✓ |  |  |  |
| DES_CDC_DESTINO | VARCHAR2(240) | ✓ |  |  |  |
| DES_COMPANIA_DESTINO | VARCHAR2(40) | ✓ |  |  |  |
| COD_TIPO_AGRUPACION_DESTINO | NUMBER |  |  |  |  |
| CUENTA_CONTABLE | VARCHAR2(25) | ✓ |  |  |  |
| COD_CONCEPTO_OE | VARCHAR2(25) | ✓ |  |  |  |
| DES_CUENTA_CONTABLE | VARCHAR2(240) | ✓ |  |  |  |
| COD_ORIGEN_GASTO | NUMBER |  |  |  |  |
| DES_ORIGEN_GASTO | VARCHAR2(50) |  |  |  |  |
| NUMERO_SOLICITUD | VARCHAR2(100) | ✓ |  |  |  |
| NUMERO_PEDIDO | VARCHAR2(100) | ✓ |  |  |  |
| SOLICITANTE | VARCHAR2(100) | ✓ |  |  |  |
| FECHA_DESDE_SOLPED | DATE | ✓ |  |  |  |
| FECHA_HASTA_SOLPED | DATE | ✓ |  |  |  |
| COMPRADOR | VARCHAR2(100) | ✓ |  |  |  |
| DESCRIPCION_PEDIDO | VARCHAR2(100) | ✓ |  |  |  |
| MATRICULA | VARCHAR2(100) | ✓ |  |  |  |
| COD_EMPLEADO_MATRICULA | NUMBER | ✓ |  |  |  |
| EMPLEADO_MATRICULA | VARCHAR2(100) | ✓ |  |  |  |
| UBICACION_ARRENDAMINETO | VARCHAR2(100) | ✓ |  |  |  |
| UBICACION_RECURRENTE | VARCHAR2(100) | ✓ |  |  |  |
| COD_GRUPO_PARKING | NUMBER | ✓ |  |  |  |
| DES_GRUPO_PARKING | VARCHAR2(100) | ✓ |  |  |  |
| NUM_FACTURA | VARCHAR2(240) | ✓ |  |  |  |
| FECHA_FACTURA | DATE | ✓ |  |  |  |
| COD_PROVEEDOR | VARCHAR2(80) | ✓ |  |  |  |
| DES_PROVEEDOR | VARCHAR2(360) | ✓ |  |  |  |
| FECHA_GASTO_IEXPENSES | DATE | ✓ |  |  |  |
| ERROR | VARCHAR2(100) | ✓ |  |  |  |
| DES_ERROR | VARCHAR2(100) | ✓ |  |  |  |
| DES_TIPO_AGRUPACION_DESTINO | VARCHAR2(50) | ✓ |  |  |  |
| DESCRIPCION_LINEA | VARCHAR2(2000) | ✓ |  |  |  |
| DES_ASIENTO | VARCHAR2(2000) | ✓ |  |  |  |
| NOMBRE_LOTE | VARCHAR2(100) | ✓ |  |  |  |
| PORCENTAJE | NUMBER | ✓ |  |  |  |
| IMPORTE | NUMBER | ✓ |  |  |  |
| COD_CONCEPTO | CHAR(1) | ✓ |  |  |  |

### NAME_CC_GRUPO_NAME_CC

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| NAME_CC_WD | VARCHAR2(240) |  | PK |  |  |
| COD_GRUPO_NAME_CC | NUMBER |  |  | → GRUPO_NAME_CC |  |
| F_BAJA | DATE | ✓ |  |  |  |
| OBSERVACIONES | CLOB | ✓ |  |  |  |

**Índices:**

- `PK_NAME_CC_GRUPO_NAME_CC` (UNIQUE) — NAME_CC_WD

### OE_CIERRE_CIA_REPARTO

Solo hasta el mes de cierre

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER |  |  |  |  |
| ANIO | NUMBER |  |  |  |  |
| COD_CIERRE | NUMBER |  |  |  | Cierre al que estará asociado |
| COD_EMPLEADO | NUMBER | ✓ |  |  | Si es iExpenses no debe ser nulo, PO sí, se harán validaciones |
| DNI | VARCHAR2(10) | ✓ |  |  | Si es iExpenses no debe ser nulo, PO sí, se harán validaciones |
| NUM_INFORME | VARCHAR2(50) | ✓ |  |  | Si es iExpenses no debe ser nulo, PO sí, se harán validaciones |
| COD_TIPO_AGRUPACION_CDC | NUMBER(8,2) |  |  |  |  |
| COD_CDC_ORIGEN | VARCHAR2(50) |  |  |  | -1 Sin centro de coste origen |
| COD_CDC_DESTINO | VARCHAR2(50) |  |  |  | Solo para Global y Services de momento -1 Sin centro de coste origen |
| COD_COMPANIA_ORIGEN | NUMBER |  |  |  |  |
| COD_COMPANIA_DESTINO | NUMBER |  |  |  |  |
| COD_CONCEPTO_OE | VARCHAR2(25) |  |  |  |  |
| CLIENTE | VARCHAR2(25) |  |  |  |  |
| CENTRO_DE_COSTE | VARCHAR2(25) |  |  |  |  |
| IMPUTACION | VARCHAR2(25) |  |  |  |  |
| INTERCIA | VARCHAR2(25) |  |  |  |  |
| LIBRE | VARCHAR2(25) |  |  |  |  |
| F_CIERRE | DATE |  |  |  |  |
| FULL_NAME | VARCHAR2(200) | ✓ |  |  | Si es iExpenses no debe ser nulo, PO sí, se harán validaciones |
| IMPORTE | NUMBER |  |  |  |  |
| COD_ORIGEN_GASTO | NUMBER |  |  |  |  |
| NUM_PEDIDO | VARCHAR2(20) | ✓ |  |  | Para PO |
| DESCRIPCION_LINEA_PEDIDO | VARCHAR2(250) | ✓ |  |  | Para PO |
| FACTURA | VARCHAR2(240) | ✓ |  |  |  |
| COD_PROVEEDOR | VARCHAR2(20) | ✓ |  |  |  |
| ASIENTO_RCL | VARCHAR2(1) | ✓ |  |  |  |
| COD_CONCEPTO_OE_NEW | VARCHAR2(25) | ✓ |  |  |  |

**Índices:**

- `IND_OE_CIERRE_CIA_REPARTO_IN1` — COD_COMPANIA_ORIGEN, COD_PAIS, F_CIERRE
- `UK_OE_CIERRE_CIA_REPARTO` (UNIQUE) — COD_PAIS, ANIO, COD_CIERRE, COD_ORIGEN_GASTO, COD_TIPO_AGRUPACION_CDC, COD_CDC_ORIGEN, COD_CDC_DESTINO, COD_COMPANIA_ORIGEN, COD_EMPLEADO, DNI, NUM_INFORME, COD_COMPANIA_DESTINO, COD_CONCEPTO_OE, CLIENTE, CENTRO_DE_COSTE, IMPUTACION, INTERCIA, LIBRE, FACTURA

### OE_CIERRE_EMPLEADO_PCT_BACKUP

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_TIPO_CIERRE | NUMBER |  |  |  | Si es cierre de nónina, de other expenses, etc ... |
| COD_CIERRE | NUMBER |  |  |  |  |
| ANIO | NUMBER |  |  |  |  |
| COD_PAIS | NUMBER |  |  |  |  |
| COD_ORIGEN_GASTO | NUMBER |  |  |  |  |
| NUM_INFORME | VARCHAR2(50) | ✓ |  |  |  |
| COD_CONCEPTO_OE | NUMBER |  |  |  | Concepto de other expenses = cuenta contable |
| COD_EMPLEADO | NUMBER | ✓ |  |  |  |
| COD_VIRTUAL | NUMBER |  |  |  |  |
| COD_VERTICAL | NUMBER |  |  |  |  |
| COD_ESPECIALIDAD | NUMBER |  |  |  |  |
| COD_OFICINA | NUMBER |  |  |  |  |
| MES | NUMBER |  |  |  |  |
| ANIO_EJECUCION | NUMBER | ✓ |  |  |  |
| MES_EJECUCION | NUMBER | ✓ |  |  |  |
| ID_CDC_ORIGEN | NUMBER |  |  |  |  |
| ID_CDC_DESTINO | NUMBER |  |  |  |  |
| COD_TIPO_AGRUPACION_CDC_DEST | NUMBER |  |  |  |  |
| CLIENTE | VARCHAR2(25) |  |  |  |  |
| CENTRO_DE_COSTE | VARCHAR2(25) |  |  |  |  |
| IMPUTACION | VARCHAR2(25) |  |  |  |  |
| INTERCIA | VARCHAR2(25) | ✓ |  |  |  |
| LIBRE | VARCHAR2(25) | ✓ |  |  |  |
| IMPORTE | NUMBER |  |  |  |  |
| PORCENTAJE | NUMBER |  |  |  |  |
| COD_USUARIO_CARGA | NUMBER |  |  |  |  |
| F_CARGA | DATE |  |  |  |  |
| F_BACKUP | DATE |  |  |  |  |
| ANIO_CONTABLE | NUMBER | ✓ |  |  | De momento solo para PO, Año en que se contabiliza |
| MES_CONTABLE | NUMBER | ✓ |  |  | De momento solo para PO, Mes en que se contabiliza |
| IND_TIPO | NUMBER |  |  |  | Tipo de informe que es: 0:Otros (o que no se utiliza) 1: Solicitud , 2: Pedido |

### OE_CIERRE_EMPLEADO_PORCENTAJES

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_TIPO_CIERRE | NUMBER |  |  | → OE_TIPO_CIERRE | Si es cierre de nónina, de other expenses, etc ... |
| COD_CIERRE | NUMBER |  |  |  |  |
| ANIO | NUMBER |  |  |  |  |
| COD_PAIS | NUMBER |  |  |  |  |
| COD_ORIGEN_GASTO | NUMBER |  |  | → OE_ORIGEN_GASTO |  |
| NUM_INFORME | VARCHAR2(50) | ✓ |  |  |  |
| COD_CONCEPTO_OE | VARCHAR2(25) |  |  |  | Concepto de other expenses = cuenta contable |
| COD_EMPLEADO | NUMBER | ✓ |  | → PY_EMPLEADO |  |
| COD_VIRTUAL | NUMBER |  |  | → AN_VIRTUAL |  |
| COD_VERTICAL | NUMBER |  |  | → AN_VERTICAL |  |
| COD_ESPECIALIDAD | NUMBER |  |  | → AN_ESPECIALIDAD |  |
| COD_OFICINA | NUMBER |  |  | → AN_OFICINA |  |
| MES | NUMBER |  |  |  |  |
| ANIO_EJECUCION | NUMBER | ✓ |  |  |  |
| MES_EJECUCION | NUMBER | ✓ |  |  |  |
| ID_CDC_ORIGEN | NUMBER |  |  |  |  |
| ID_CDC_DESTINO | NUMBER |  |  |  |  |
| COD_TIPO_AGRUPACION_CDC_DEST | NUMBER |  |  |  |  |
| CLIENTE | VARCHAR2(25) |  |  |  | Para OF |
| CENTRO_DE_COSTE | VARCHAR2(25) |  |  |  | Para OF |
| IMPUTACION | VARCHAR2(25) |  |  |  | Para OF |
| INTERCIA | VARCHAR2(25) |  |  |  | Para OF |
| LIBRE | VARCHAR2(25) |  |  |  | Para OF |
| IMPORTE | NUMBER |  |  |  |  |
| PORCENTAJE | NUMBER |  |  |  |  |
| COD_USUARIO_CARGA | NUMBER |  |  |  |  |
| F_CARGA | DATE |  |  |  |  |
| ANIO_CONTABLE | NUMBER | ✓ |  |  | De momento solo para PO, Año en que se contabiliza |
| MES_CONTABLE | NUMBER | ✓ |  |  | De momento solo para PO, Mes en que se contabiliza |
| IND_TIPO | NUMBER |  |  |  | Tipo de informe que es: 0:Otros (o que no se utiliza) 1: Solicitud , 2: Pedido |

**Índices:**

- `UQ_OE_CIERRE_EMPLEADO_PCT` (UNIQUE) — ANIO, COD_CIERRE, COD_TIPO_CIERRE, COD_ORIGEN_GASTO, NUM_INFORME, IND_TIPO, COD_CONCEPTO_OE, COD_EMPLEADO, COD_VIRTUAL, COD_VERTICAL, COD_ESPECIALIDAD, COD_OFICINA, MES, ANIO_EJECUCION, MES_EJECUCION, ANIO_CONTABLE, MES_CONTABLE, ID_CDC_ORIGEN, ID_CDC_DESTINO, COD_TIPO_AGRUPACION_CDC_DEST, CLIENTE, CENTRO_DE_COSTE, IMPUTACION, INTERCIA, LIBRE

### OE_CONF_PROV_GET_EMPLEADO

Tabla que se utiiliza en código para saber donde buscar el código de empleado dado un proveedor 

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PROVEEDOR | NUMBER |  | PK |  |  |
| CAMPO | VARCHAR2(50) |  |  |  | Campo donde vamos a buscar el código del empleado |
| TABLA_EMPLEADO | VARCHAR2(50) | ✓ |  |  | Tabla donde hay que buscar el código de empleado. Nulo significa que está en la misma tabla |

**Índices:**

- `PK_OE_FACT_PROV_GET_EMP` (UNIQUE) — COD_PROVEEDOR

### OE_DIARIO_EMPLEADO_PORCENTAJES

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS_ANIO | NUMBER |  |  |  | Para la particion puesto que se puede repitir una versión en varios paises |
| COD_PAIS | NUMBER |  |  |  |  |
| ANIO | NUMBER |  |  |  |  |
| COD_VERSION | NUMBER |  |  |  |  |
| COD_ORIGEN_GASTO | NUMBER |  |  | → OE_ORIGEN_GASTO |  |
| COD_CONCEPTO_OE | VARCHAR2(25) |  |  |  | Concepto de other expenses = cuenta contable |
| NUM_INFORME | VARCHAR2(50) | ✓ |  |  |  |
| COD_EMPLEADO | NUMBER | ✓ |  | → PY_EMPLEADO |  |
| COD_VIRTUAL | NUMBER |  |  | → AN_VIRTUAL |  |
| COD_VERTICAL | NUMBER |  |  | → AN_VERTICAL |  |
| COD_ESPECIALIDAD | NUMBER |  |  | → AN_ESPECIALIDAD |  |
| COD_OFICINA | NUMBER |  |  | → AN_OFICINA |  |
| MES | NUMBER |  |  |  |  |
| ANIO_EJECUCION | NUMBER | ✓ |  |  |  |
| MES_EJECUCION | NUMBER | ✓ |  |  |  |
| ID_CDC_ORIGEN | NUMBER |  |  |  |  |
| ID_CDC_DESTINO | NUMBER |  |  |  |  |
| COD_TIPO_AGRUPACION_CDC_DEST | NUMBER |  |  | → AN_TIPO_AGRUPACION_CDC |  |
| CLIENTE | VARCHAR2(25) |  |  |  | Para OF |
| CENTRO_DE_COSTE | VARCHAR2(25) |  |  |  | Para OF |
| IMPUTACION | VARCHAR2(25) |  |  |  | Para OF |
| INTERCIA | VARCHAR2(25) |  |  |  | Para OF |
| LIBRE | VARCHAR2(25) |  |  |  | Para OF |
| IMPORTE | NUMBER |  |  |  |  |
| PORCENTAJE | NUMBER |  |  |  |  |
| F_CARGA | DATE |  |  |  |  |
| COD_USUARIO_CARGA | NUMBER |  |  |  |  |
| ANIO_CONTABLE | NUMBER | ✓ |  |  | De momento solo para PO, Año en que se contabiliza |
| MES_CONTABLE | NUMBER | ✓ |  |  | De momento solo para PO, Mes en que se contabiliza |
| IND_TIPO | NUMBER |  |  |  | Tipo de informe que es: 0:Otros (o que no se utiliza) 1: Solicitud , 2: Pedido |

**Índices:**

- `IDX_EMP_PORC_NUM_INF` — COD_ORIGEN_GASTO, NUM_INFORME
- `IDX_EMP_PORC_ORI_VER` — COD_ORIGEN_GASTO, COD_PAIS, ANIO, COD_VERSION
- `IDX_EMP_PORC_ORI_VER_NUM` — COD_ORIGEN_GASTO, COD_PAIS, ANIO, COD_VERSION, NUM_INFORME
- `IDX_OE_DIARIO_EMPLEADO_PORCENTAJES_ID1` — COD_PAIS, ANIO, COD_VERSION, COD_ORIGEN_GASTO
- `IDX_OE_DR_MPL_PCT_VER_CDC_DEST` — COD_PAIS, ANIO, COD_VERSION, ID_CDC_DESTINO
- `IDX_OE_DR_MPL_PCT_VER_CDC_ORI` — COD_PAIS, ANIO, COD_VERSION, ID_CDC_ORIGEN
- `UQ_OE_DIARIO_EMPLEADO_PORC` (UNIQUE) — COD_PAIS_ANIO, COD_PAIS, ANIO, COD_VERSION, COD_ORIGEN_GASTO, NUM_INFORME, IND_TIPO, COD_CONCEPTO_OE, COD_EMPLEADO, COD_VIRTUAL, COD_VERTICAL, COD_ESPECIALIDAD, COD_OFICINA, MES, ANIO_EJECUCION, MES_EJECUCION, ANIO_CONTABLE, MES_CONTABLE, ID_CDC_ORIGEN, ID_CDC_DESTINO, COD_TIPO_AGRUPACION_CDC_DEST, CLIENTE, CENTRO_DE_COSTE, IMPUTACION, INTERCIA, LIBRE

### OE_DIARIO_EMPLEADO_PORCENTAJES_BK

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS_ANIO | NUMBER |  |  |  |  |
| COD_PAIS | NUMBER |  |  |  |  |
| ANIO | NUMBER |  |  |  |  |
| COD_VERSION | NUMBER |  |  |  |  |
| COD_ORIGEN_GASTO | NUMBER |  |  |  |  |
| COD_CONCEPTO_OE | VARCHAR2(25) |  |  |  |  |
| NUM_INFORME | VARCHAR2(50) | ✓ |  |  |  |
| COD_EMPLEADO | NUMBER | ✓ |  |  |  |
| COD_VIRTUAL | NUMBER |  |  |  |  |
| COD_VERTICAL | NUMBER |  |  |  |  |
| COD_ESPECIALIDAD | NUMBER |  |  |  |  |
| COD_OFICINA | NUMBER |  |  |  |  |
| MES | NUMBER |  |  |  |  |
| ANIO_EJECUCION | NUMBER | ✓ |  |  |  |
| MES_EJECUCION | NUMBER | ✓ |  |  |  |
| ID_CDC_ORIGEN | NUMBER |  |  |  |  |
| ID_CDC_DESTINO | NUMBER |  |  |  |  |
| COD_TIPO_AGRUPACION_CDC_DEST | NUMBER |  |  |  |  |
| CLIENTE | VARCHAR2(25) |  |  |  |  |
| CENTRO_DE_COSTE | VARCHAR2(25) |  |  |  |  |
| IMPUTACION | VARCHAR2(25) |  |  |  |  |
| INTERCIA | VARCHAR2(25) |  |  |  |  |
| LIBRE | VARCHAR2(25) |  |  |  |  |
| IMPORTE | NUMBER |  |  |  |  |
| PORCENTAJE | NUMBER |  |  |  |  |
| F_CARGA | DATE |  |  |  |  |
| COD_USUARIO_CARGA | NUMBER |  |  |  |  |
| ANIO_CONTABLE | NUMBER | ✓ |  |  |  |
| MES_CONTABLE | NUMBER | ✓ |  |  |  |
| IND_TIPO | NUMBER |  |  |  |  |

### OE_DIST_IMPUTACION

Dentro de cada empleado para España puede tener varias imputaciones distintas con porcentaje

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_DIST_IMPUTACION | NUMBER |  | PK |  |  |
| COD_IMPUTACION | NUMBER |  |  | → OE_IMPUTA |  |
| PCT_DISTRIBUCION | NUMBER |  |  |  |  |
| IND_VIRTUAL_TODOS | NUMBER | ✓ |  |  | 1 Tiene todas las virtuals 0 tiene que seleccionar alguna en la tabla |
| IND_VERTICAL_TODOS | NUMBER | ✓ |  |  | 1 Tiene todas las verticales 0 tiene que seleccionar alguna en la tabla |
| IND_ESPECIALIDAD_TODOS | NUMBER | ✓ |  |  | 1 Tiene todas las especialidades 0 tiene que seleccionar alguna en la tabla |
| IND_OFICINA_TODOS | NUMBER | ✓ |  |  | 1 Tiene todas las oficinas 0 tiene que seleccionar alguna en la tabla |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  | Como se borra y vuelve a crear, nunca vamos a saber el usuario de creación. La única forma es el usuario de creación de la imputación |
| F_USUARIO_MODIFICACION | DATE | ✓ |  |  | Como se borra y vuelve a crear, nunca vamos a saber la fecha de creación. La única forma es la fecha de creación de la imputación |

**Índices:**

- `IDX_OE_DIST_IMPUTACION_ID1` — COD_IMPUTACION
- `PK_OE_DIST_IMPUTACION` (UNIQUE) — COD_DIST_IMPUTACION

### OE_IMPUTA

Tabla maestra de las imputaciones por empleado (Origen OF)

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_IMPUTACION | NUMBER |  | PK |  |  |
| COD_PAIS | NUMBER |  |  |  | País que se utilizará para la versión. Se podría sacar de V_COMPANIA_CDC, pero para identificar la versión lo ponemos también aquí |
| ANIO | NUMBER(4,0) |  |  |  | Para la versión |
| COD_VERSION | NUMBER |  |  |  | Código versión pptos. 999 real. |
| NUM_INFORME | VARCHAR2(50) |  |  |  | Para IExpense: Numero de informe Para PO: id único de línea de solicitud o pedido |
| COD_ORIGEN_GASTO | NUMBER |  |  | → OE_ORIGEN_GASTO |  |
| ID_CDC | NUMBER |  |  |  |  |
| COD_EMPLEADO | NUMBER | ✓ |  | → PY_EMPLEADO | Puede ser nulo si el origen no es gastos |
| COD_PROVEEDOR | VARCHAR2(20) | ✓ |  |  | Es nulo si el origen no es proveedores |
| PCT_RECLASIFICACION | NUMBER |  |  |  |  |
| OBSERVACIONES | CLOB | ✓ |  |  |  |
| IND_TIPO | NUMBER | ✓ |  |  | Tipo de informe que es: 0:Otros (o que no se utiliza) 1: Solicitud , 2: Pedido |
| NUM_SOLICITUD_PEDIDO | VARCHAR2(20) | ✓ |  |  |  |
| COD_USUARIO_CREACION | NUMBER |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |
| F_CREACION | DATE | ✓ |  |  |  |
| F_MODIFICACION | DATE | ✓ |  |  |  |
| IND_ESTADO_IMPUTACION | NUMBER |  |  |  | Bitand 1: Origen Plantilla (para PO) 2: Refacturable 4: Calculo por HC 8: Modificado por pantalla (de momento solo AP) 16: Refacturable Global y Local Corporate |

**Índices:**

- `IDX_OE_IMPUTA` — ANIO, COD_VERSION, NUM_INFORME
- `PK_OE_IMPUTA` (UNIQUE) — COD_IMPUTACION

### OE_IMPUTA_ESPECIALIDAD

Imputación por especialidad no entra el nombre completo.

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_DIST_IMPUTACION | NUMBER |  | PK | → OE_DIST_IMPUTACION |  |
| COD_ESPECIALIDAD | NUMBER |  | PK | → AN_ESPECIALIDAD |  |

**Índices:**

- `PK_OE_IMPUTA_AN_ESP` (UNIQUE) — COD_DIST_IMPUTACION, COD_ESPECIALIDAD

### OE_IMPUTA_OFICINA

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_DIST_IMPUTACION | NUMBER |  | PK | → OE_DIST_IMPUTACION |  |
| COD_OFICINA | NUMBER |  | PK | → AN_OFICINA |  |

**Índices:**

- `PK_OE_IMPUTA_OFICINA` (UNIQUE) — COD_DIST_IMPUTACION, COD_OFICINA

### OE_IMPUTA_PORCENTAJE

Imputaciones destino por centro de coste y porcentaje (Global, services ..)

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_IMPUTACION | NUMBER |  | PK | → OE_IMPUTA |  |
| ID_CDC | NUMBER |  | PK |  | Codigo centro coste-companía de v_compania_cdc |
| COD_TIPO_AGRUPACION_CDC | NUMBER |  | PK |  | Indica si es reparto de Global o de service |
| PORCENTAJE | NUMBER |  |  |  |  |

**Índices:**

- `PK_OE_IMPUTA_PORCENTAJE` (UNIQUE) — COD_IMPUTACION, ID_CDC, COD_TIPO_AGRUPACION_CDC

### OE_IMPUTA_VERTICAL

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_DIST_IMPUTACION | NUMBER |  | PK | → OE_DIST_IMPUTACION |  |
| COD_VERTICAL | NUMBER |  | PK | → AN_VERTICAL |  |

**Índices:**

- `IDX_OE_IMPUTA_VERTICAL_ID1` — COD_DIST_IMPUTACION
- `PK_OE_IMP_AN_VERT` (UNIQUE) — COD_DIST_IMPUTACION, COD_VERTICAL

### OE_IMPUTA_VIRTUAL

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_DIST_IMPUTACION | NUMBER |  | PK | → OE_DIST_IMPUTACION |  |
| COD_VIRTUAL | NUMBER |  | PK | → AN_VIRTUAL |  |

**Índices:**

- `PK_OE_IMPUTACION_VIRTUAL` (UNIQUE) — COD_DIST_IMPUTACION, COD_VIRTUAL

### OE_ORIGEN_GASTO

Tabla que nos dice que tipo gasto es: Gasto, Proveedores, etc.

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_ORIGEN_GASTO | NUMBER |  | PK |  |  |
| DES_ORIGEN_GASTO | VARCHAR2(50) |  |  |  |  |
| IND_SIN_IMPUTA_MINER | NUMBER |  |  |  | 1: En el miner de other expenses se muestra aunque no tenga imputación 0: No tiene en cuenta los que tienen imputacion |

**Índices:**

- `PK_OE_ORIGEN_GASTO` (UNIQUE) — COD_ORIGEN_GASTO

### OE_OTHER_EXPENSES_HISTORICO

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| ORIGEN_DATO | VARCHAR2(15) | ✓ |  |  |  |
| COD_PAIS | NUMBER | ✓ |  |  |  |
| ANIO | NUMBER | ✓ |  |  |  |
| ANIO_CONTABLE | NUMBER | ✓ |  |  |  |
| MES_CONTABLE | NUMBER | ✓ |  |  |  |
| COD_VERSION | NUMBER | ✓ |  |  |  |
| DES_VERSION | VARCHAR2(40) | ✓ |  |  |  |
| MES | NUMBER | ✓ |  |  |  |
| NUM_INFORME | VARCHAR2(81) | ✓ |  |  |  |
| IND_REFACTURABLE | VARCHAR2(1) | ✓ |  |  |  |
| IND_CALCULO_HC | VARCHAR2(1) | ✓ |  |  |  |
| COD_EMPLEADO | NUMBER | ✓ |  |  |  |
| NOMBRE | VARCHAR2(100) | ✓ |  |  |  |
| APELLIDO1 | VARCHAR2(100) | ✓ |  |  |  |
| APELLIDO2 | VARCHAR2(100) | ✓ |  |  |  |
| NOMBRE_COMPLETO | VARCHAR2(152) | ✓ |  |  |  |
| F_ALTA | DATE | ✓ |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |
| COD_VIRTUAL | NUMBER | ✓ |  |  |  |
| DES_VIRTUAL | VARCHAR2(50) | ✓ |  |  |  |
| COD_VERTICAL | NUMBER | ✓ |  |  |  |
| DES_VERTICAL | VARCHAR2(50) | ✓ |  |  |  |
| COD_ESPECIALIDAD | NUMBER | ✓ |  |  |  |
| DES_ESPECIALIDAD | VARCHAR2(50) | ✓ |  |  |  |
| COD_GRUPO_ESPECIALIDAD | NUMBER | ✓ |  |  |  |
| DES_GRUPO_ESPECIALIDAD | VARCHAR2(50) | ✓ |  |  |  |
| COD_OFICINA | NUMBER | ✓ |  |  |  |
| DES_OFICINA | VARCHAR2(50) | ✓ |  |  |  |
| COD_CDC_ORIGEN | VARCHAR2(150) | ✓ |  |  |  |
| COD_COMPANIA_ORIGEN | NUMBER | ✓ |  |  |  |
| DES_CDC_ORIGEN | VARCHAR2(240) | ✓ |  |  |  |
| DES_COMPANIA_ORIGEN | VARCHAR2(50) | ✓ |  |  |  |
| COD_CDC_DESTINO | VARCHAR2(4000) | ✓ |  |  |  |
| COD_COMPANIA_DESTINO | NUMBER | ✓ |  |  |  |
| DES_CDC_DESTINO | VARCHAR2(240) | ✓ |  |  |  |
| DES_COMPANIA_DESTINO | VARCHAR2(50) | ✓ |  |  |  |
| COD_TIPO_AGRUPACION_DESTINO | NUMBER | ✓ |  |  |  |
| CUENTA_CONTABLE | VARCHAR2(150) | ✓ |  |  |  |
| COD_CONCEPTO_OE | VARCHAR2(25) | ✓ |  |  |  |
| DES_CUENTA_CONTABLE | VARCHAR2(240) | ✓ |  |  |  |
| COD_ORIGEN_GASTO | NUMBER | ✓ |  |  |  |
| DES_ORIGEN_GASTO | VARCHAR2(50) | ✓ |  |  |  |
| NUMERO_SOLICITUD | VARCHAR2(100) | ✓ |  |  |  |
| NUMERO_PEDIDO | VARCHAR2(100) | ✓ |  |  |  |
| SOLICITANTE | VARCHAR2(240) | ✓ |  |  |  |
| FECHA_DESDE_SOLPED | DATE | ✓ |  |  |  |
| FECHA_HASTA_SOLPED | DATE | ✓ |  |  |  |
| COMPRADOR | VARCHAR2(240) | ✓ |  |  |  |
| DESCRIPCION_PEDIDO | VARCHAR2(240) | ✓ |  |  |  |
| MATRICULA | VARCHAR2(150) | ✓ |  |  |  |
| COD_EMPLEADO_MATRICULA | NUMBER | ✓ |  |  |  |
| EMPLEADO_MATRICULA | VARCHAR2(152) | ✓ |  |  |  |
| UBICACION_ARRENDAMINETO | VARCHAR2(100) | ✓ |  |  |  |
| UBICACION_RECURRENTE | VARCHAR2(100) | ✓ |  |  |  |
| COD_GRUPO_PARKING | NUMBER | ✓ |  |  |  |
| DES_GRUPO_PARKING | VARCHAR2(100) | ✓ |  |  |  |
| NUM_FACTURA | VARCHAR2(240) | ✓ |  |  |  |
| FECHA_FACTURA | DATE | ✓ |  |  |  |
| COD_PROVEEDOR | VARCHAR2(80) | ✓ |  |  |  |
| DES_PROVEEDOR | VARCHAR2(360) | ✓ |  |  |  |
| FECHA_GASTO_IEXPENSES | DATE | ✓ |  |  |  |
| ERROR | VARCHAR2(100) | ✓ |  |  |  |
| DES_ERROR | VARCHAR2(100) | ✓ |  |  |  |
| DES_TIPO_AGRUPACION_DESTINO | VARCHAR2(50) | ✓ |  |  |  |
| DESCRIPCION_LINEA | VARCHAR2(2000) | ✓ |  |  |  |
| DES_ASIENTO | VARCHAR2(2000) | ✓ |  |  |  |
| NOMBRE_LOTE | VARCHAR2(100) | ✓ |  |  |  |
| PORCENTAJE | NUMBER | ✓ |  |  |  |
| IMPORTE1 | NUMBER | ✓ |  |  |  |
| IMPORTE | NUMBER | ✓ |  |  |  |

**Índices:**

- `IND_ANIO_CONTABLE` — ANIO_CONTABLE
- `IND_OE_OTHER_EXPENSES_HIST_U1` — COD_COMPANIA_ORIGEN, CUENTA_CONTABLE, COD_CONCEPTO_OE
- `IND_OE_OTHER_EXPENSES_HIST_U2` — ANIO_CONTABLE, COD_COMPANIA_ORIGEN, COD_ORIGEN_GASTO, NUMERO_PEDIDO

### OE_TIPO_AGRUPA_PORCENTAJE

Tabla que contiene la distrubución por tipo agrupacion porcentual 

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_IMPUTACION | NUMBER |  | PK | → OE_IMPUTA |  |
| COD_TIPO_AGRUPACION_CDC | NUMBER |  | PK | → AN_TIPO_AGRUPACION_CDC |  |
| PORCENTAJE | NUMBER |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  | Como se borra y vuelve a crear, nunca vamos a saber el usuario de creación. La única forma es el usuario de creación de la imputación |
| F_MODIFICACION | DATE | ✓ |  |  | Como se borra y vuelve a crear, nunca vamos a saber la fecha de creación. La única forma es la fecha de modificacion de la imputacion |

**Índices:**

- `PK_OE_IMPUTA_TIPO_AGRUPA_PCT` (UNIQUE) — COD_IMPUTACION, COD_TIPO_AGRUPACION_CDC

### OE_TIPO_CIERRE

Tipo de cierre (nómina,other expenses, etc)

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_TIPO_CIERRE | NUMBER |  | PK |  |  |
| DES_TIPO_CIERRE | VARCHAR2(50) |  |  |  |  |

**Índices:**

- `PK_OE_TIPO_CIERRE` (UNIQUE) — COD_TIPO_CIERRE

### OF_ORIGEN_ASIENTO

Tabla para cargar orígenes de asientos

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_COMPANIA_MMS | NUMBER |  |  |  |  |
| PAIS_MMS | NUMBER |  |  |  |  |
| ORIGEN_ASIENTO | VARCHAR2(25) |  |  |  |  |

### OF_PROVEEDOR

Tabla proveedores OF

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_COMPANIA | NUMBER |  |  |  |  |
| VENDOR_ID | NUMBER |  |  |  |  |
| VENDOR_SITES_ID | NUMBER |  |  |  |  |
| COD_PROVEEDOR | VARCHAR2(10) |  |  |  |  |
| NIF | VARCHAR2(30) |  |  |  |  |
| NOM_PROVEEDOR | VARCHAR2(240) |  |  |  |  |

### PLN_DIST_IMPUTACION

Dentro de cada empleado para España puede tener varias imputaciones distintas con porcentaje

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_DIST_IMPUTA_PLANTILLA | NUMBER |  | PK |  |  |
| COD_IMPUTA_PLANTILLA | NUMBER |  |  | → PLN_IMPUTA |  |
| PCT_DISTRIBUCION | NUMBER |  |  |  |  |
| IND_VIRTUAL_TODOS | NUMBER | ✓ |  |  | 1 Tiene todas las virtuals 0 tiene que seleccionar alguna en la tabla |
| IND_VERTICAL_TODOS | NUMBER | ✓ |  |  | 1 Tiene todas las verticales 0 tiene que seleccionar alguna en la tabla |
| IND_ESPECIALIDAD_TODOS | NUMBER | ✓ |  |  | 1 Tiene todas las especialidades 0 tiene que seleccionar alguna en la tabla |
| IND_OFICINA_TODOS | NUMBER | ✓ |  |  | 1 Tiene todas las oficinas 0 tiene que seleccionar alguna en la tabla |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  | Como se borra y vuelve a crear, nunca vamos a saber el usuario de creación. La única forma es el usuario de modificacion de la imputacion |
| F_MODIFICACION | DATE |  |  |  | Como se borra y vuelve a crear, nunca vamos a saber la fecha de creación. La única forma es la fecha de modificacion de la imputacion |

**Índices:**

- `PK_PLN_DIST_IMP_PLANT` (UNIQUE) — COD_DIST_IMPUTA_PLANTILLA

### PLN_IMPUTA

Tabla maestra de la imputación de la plantilla

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_IMPUTA_PLANTILLA | NUMBER |  | PK |  |  |
| DES_PLANTILLA | VARCHAR2(50) |  |  |  |  |
| COD_PAIS | NUMBER |  |  |  | País que se utilizará para la versión. Se podría sacar de V_COMPANIA_CDC, pero para identificar la versión lo ponemos también aquí |
| COD_COMPANIA | NUMBER |  |  |  | Las plantillas en un futuro se pueden utilizar para cualquier tipo de gasto, así que solo se pone la compañía. El ID_CDC se tendrá que calcular dependiendo del origen |
| COD_USUARIO | NUMBER |  |  |  |  |
| IND_PREDETERMINADO | NUMBER(1,0) | ✓ |  |  | Indica si se considera predetermiando. En principio se controlará por código que sea uno compañía  |
| OBSERVACIONES | CLOB | ✓ |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |
| COD_USUARIO_BAJA | NUMBER | ✓ |  |  |  |
| IND_ESTADO_IMPUTACION | NUMBER |  |  |  | Bitand 1: Origen Plantilla (para PO) 2: Refacturable 4: Calculo por HC 8: Modificado por pantalla (de momento solo AP) 16: Refacturable Global y Local Corporate |

**Índices:**

- `PK_PLN_IMPUTA` (UNIQUE) — COD_IMPUTA_PLANTILLA

### PLN_IMPUTA_ESPECIALIDAD

Imputación por especialidad no entra el nombre completo.

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_DIST_IMPUTA_PLANTILLA | NUMBER |  | PK | → PLN_DIST_IMPUTACION |  |
| COD_ESPECIALIDAD | NUMBER |  | PK | → AN_ESPECIALIDAD |  |

**Índices:**

- `PK_PLN_IMPUTA_PLN_AN_ESP` (UNIQUE) — COD_DIST_IMPUTA_PLANTILLA, COD_ESPECIALIDAD

### PLN_IMPUTA_OFICINA

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_DIST_IMPUTA_PLANTILLA | NUMBER |  | PK | → PLN_DIST_IMPUTACION |  |
| COD_OFICINA | NUMBER |  | PK | → AN_OFICINA |  |

**Índices:**

- `PK_PLN_IMPUTA_OFICINA` (UNIQUE) — COD_DIST_IMPUTA_PLANTILLA, COD_OFICINA

### PLN_IMPUTA_PORCENTAJE

Imputaciones destino por centro de coste y porcentaje (Global, services ..)

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_IMPUTA_PLANTILLA | NUMBER |  | PK | → PLN_IMPUTA |  |
| ID_CDC | NUMBER |  | PK |  | Codigo centro coste-companía de v_compania_cdc |
| COD_TIPO_AGRUPACION_CDC | NUMBER |  | PK |  | Indica si es reparto de Global o de service |
| PORCENTAJE | NUMBER |  |  |  |  |

**Índices:**

- `PK_PLN_IMPUTA_PORCENTAJE` (UNIQUE) — COD_IMPUTA_PLANTILLA, ID_CDC, COD_TIPO_AGRUPACION_CDC

### PLN_IMPUTA_VERTICAL

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_DIST_IMPUTA_PLANTILLA | NUMBER |  | PK | → PLN_DIST_IMPUTACION |  |
| COD_VERTICAL | NUMBER |  | PK | → AN_VERTICAL |  |

**Índices:**

- `PK_PLN_IMP_PLN_AN_VERT` (UNIQUE) — COD_DIST_IMPUTA_PLANTILLA, COD_VERTICAL

### PLN_IMPUTA_VIRTUAL

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_DIST_IMPUTA_PLANTILLA | NUMBER |  | PK | → PLN_DIST_IMPUTACION |  |
| COD_VIRTUAL | NUMBER |  | PK | → AN_VIRTUAL |  |

**Índices:**

- `PK_PLN_IMPUTACION_VIRTUAL` (UNIQUE) — COD_DIST_IMPUTA_PLANTILLA, COD_VIRTUAL

### PLN_PLANTILLA_PREDETERMINADA

Tabla que nos indica que plantilla es predeterminada por compania, origen y proveedor,pudiendo ser cualquier combinacion pero sin repetirse, ejemplo por solo compañía, solo por compañia origen, solo origen, etc. Se añade ind_valija a la pk puesto que puede ser una misma conbinación, uno para valija y otra no. No tiene PK y sí UQ porque puede haber nulos dentro del índice único

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_IMPUTA_PLANTILLA | NUMBER |  |  | → PLN_IMPUTA |  |
| COD_COMPANIA | NUMBER | ✓ |  |  |  |
| COD_ORIGEN_GASTO | NUMBER | ✓ |  | → OE_ORIGEN_GASTO |  |
| COD_PROVEEDOR | VARCHAR2(20) | ✓ |  |  | Para cod_origen_gasto = 4 -> COD_PROVEEDOR Para cod_origen_gasto = 5 -> CDC |
| IND_VALIJA | NUMBER |  |  |  | Se utiliza para diferenciar por proveedor lo que es una plantilla de valija y lo que no. |

**Índices:**

- `UQ_PLN_PLANTILLA_PRED` (UNIQUE) — COD_COMPANIA, COD_ORIGEN_GASTO, COD_PROVEEDOR, IND_VALIJA

### PLN_TIPO_AGRUPA_PORCENTAJE

Tabla que contiene la distrubución por tipo agrupacion porcentual 

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_IMPUTA_PLANTILLA | NUMBER |  | PK | → PLN_IMPUTA |  |
| COD_TIPO_AGRUPACION_CDC | NUMBER |  | PK | → AN_TIPO_AGRUPACION_CDC |  |
| PORCENTAJE | NUMBER |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  | Como se borra y vuelve a crear, nunca vamos a saber el usuario de creación. La única forma es el usuario de modificacion de la imputacion |
| F_MODIFICACION | DATE |  |  |  | Como se borra y vuelve a crear, nunca vamos a saber la fecha de creación. La única forma es la fecha de modificacion de la imputacion |

**Índices:**

- `PK_PLN_IMP_PLAN_TIPO_AGRUP_PCT` (UNIQUE) — COD_IMPUTA_PLANTILLA, COD_TIPO_AGRUPACION_CDC

### PPT_ACCION_ETIQUETA

Maestro de etiquetas que pueden venir en los ficheros de parámetros que se van a guardar en los log

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_ACCION_ETIQUETA | NUMBER |  | PK |  | Secuencia SQC_PPT_ACCION_ETIQUETA |
| DES_ACCION_ETIQUETA | VARCHAR2(100) |  |  |  | Etiqueta que hay dentro de los Json de parámetros |
| IND_SEGURIDAD | NUMBER(1,0) |  |  |  | -1  no se trata porque puede ser un valor que puede llevar ese valor (un importe)
1  lleva seguridad, por lo que hay que sustituir el -1 por la lista de códigos a las que tiene acceso. Si tiene acceso a todo se dejará el .1  
0 esta etiqueta no admite llevar -1 porque la seguridad tiene que venir implementada desde la pantalla
 |
| COD_DATO | NUMBER | ✓ |  |  | clave del campo en la tabla TIPO_DATO_SEGURIDAD |
| TABLA | VARCHAR2(50) | ✓ |  |  | Tabla maestra a la que hace referencia la etiqueta |
| COLUMNA | VARCHAR2(50) | ✓ |  |  | Columna de la tabla maestra a la que hace referencia la etiqueta |

**Índices:**

- `PK_PPT_ACCION_ETIQUETA` (UNIQUE) — COD_ACCION_ETIQUETA
- `UQ_PPT_ACCION_ETIQUETA` (UNIQUE) — DES_ACCION_ETIQUETA

### PPT_ACCION_LOG

Entidad que guarda el proceso a auditar, quién lo ha llamado, fechas de inicio y de finalización y el json con los parámetros

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_ACCION_LOG | NUMBER |  | PK |  | Secuencia SQC_ACCION_LOG |
| DES_PROCESO | VARCHAR2(4000) |  |  |  | Stack de la llamada de los procesos |
| COD_USUARIO | NUMBER |  |  |  | Usuario que ha lanzado el proceso |
| FECHA_INICIO | DATE |  |  |  | Fecha y hora en la que se lanza el proceso |
| FECHA_FIN | DATE | ✓ |  |  | Fecha en la que se finaliza el proceso |
| PARAMETROS | CLOB |  |  |  | Fichero con los parámetros con los que se ha lanzado el proceso |
| RESULTADO | NUMBER | ✓ |  |  | Código de error que devuelve el proceso: 0 es correcto, >0 error controlado , <0 error oracle |
| RESULTADO_STR | CLOB | ✓ |  |  | String con el resultado que devuelve el proceso |
| PARAMETROS_INY_SEGURIDAD | CLOB | ✓ |  |  | Fichero con los parámetros tras  INYECTAR SEGURIDAD |

**Índices:**

- `PK_PPT_ACCION_LOG` (UNIQUE) — COD_ACCION_LOG

### PPT_BUSINESS_MAGNITUDE

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_BUSINESS_MAGNITUDE | NUMBER |  | PK |  |  |
| COD_PAIS | NUMBER |  |  |  |  |
| COD_GRUPO | NUMBER |  |  |  |  |
| COD_MAGNITUDE | VARCHAR2(150) |  |  |  |  |
| IND_TIMESHEET | NUMBER(1,0) |  |  |  |  |

**Índices:**

- `PK_PPT_BUSINESS_MAGNITUDE` (UNIQUE) — COD_BUSINESS_MAGNITUDE
- `UQ_PPT_BUSINESS_MAGNITUDE` (UNIQUE) — COD_PAIS, COD_GRUPO

### PPT_CHANNEL

Maestro de channels para los reportes de Itracker

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CHANNEL | NUMBER |  | PK |  |  |
| DES_CHANNEL | VARCHAR2(100) |  |  |  |  |
| IND_MODO_CALCULO | NUMBER(1,0) |  |  |  | Modo de cálculo   1-. Por SUM de los importes      2-. Por diferencias |

**Índices:**

- `PK_PPT_CHANNEL` (UNIQUE) — COD_CHANNEL

### PPT_CHANNEL_DISCIPLINA_IN

Disciplinas que componen ese channel. Si no hay, se cogen todos

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CHANNEL | NUMBER |  | PK | → PPT_CHANNEL |  |
| COD_DISCIPLINA | NUMBER |  | PK |  |  |

**Índices:**

- `PK_PPT_CHANNEL_DISCIPLINA_IN` (UNIQUE) — COD_CHANNEL, COD_DISCIPLINA

### PPT_CHANNEL_DISCIPLINA_OUT

Channels que se excluyen de un publisher. Si no hay registros, no se excluye nada

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CHANNEL | NUMBER |  | PK | → PPT_CHANNEL |  |
| COD_DISCIPLINA | NUMBER |  | PK |  |  |

**Índices:**

- `PK_PPT_CHANNEL_DISCIPLINA_OUT` (UNIQUE) — COD_CHANNEL, COD_DISCIPLINA

### PPT_CHANNEL_MEDIO_IN

Medios que componen ese channel. Si no hay, se cogen todos

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CHANNEL | NUMBER |  | PK | → PPT_CHANNEL |  |
| COD_PAIS | NUMBER |  | PK |  |  |
| COD_MEDIO | NUMBER |  | PK |  |  |

**Índices:**

- `PK_PPT_CHANNEL_MEDIO_IN` (UNIQUE) — COD_CHANNEL, COD_PAIS, COD_MEDIO

### PPT_CHANNEL_MEDIO_OUT

Medios que se excluyen de ese channel. Si no hay, no se excluye nada

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CHANNEL | NUMBER |  | PK | → PPT_CHANNEL |  |
| COD_PAIS | NUMBER |  | PK |  |  |
| COD_MEDIO | NUMBER |  | PK |  |  |

**Índices:**

- `PK_PPT_CHANNEL_MEDIO_OUT` (UNIQUE) — COD_CHANNEL, COD_PAIS, COD_MEDIO

### PPT_CHANNEL_OBJETIVO_IN

Objetivos que componen ese channel. Si no hay, se cogen todos

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CHANNEL | NUMBER |  | PK | → PPT_CHANNEL |  |
| COD_OBJETIVO | NUMBER |  | PK |  |  |

**Índices:**

- `PK_PPT_CHANNEL_OBJETIVO_IN` (UNIQUE) — COD_CHANNEL, COD_OBJETIVO

### PPT_CHANNEL_OBJETIVO_OUT

Objetivos que se excluyen de ese channel. Si no hay, no se excluye nada

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CHANNEL | NUMBER |  | PK | → PPT_CHANNEL |  |
| COD_OBJETIVO | NUMBER |  | PK |  |  |

**Índices:**

- `PK_PPT_CHANNEL_OBJETIVO_OUT` (UNIQUE) — COD_CHANNEL, COD_OBJETIVO

### PPT_CHANNEL_TIPO_COMPRA_IN

Tipos de Compra que componen ese channel. Si no hay, se cogen todos

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CHANNEL | NUMBER |  | PK | → PPT_CHANNEL |  |
| COD_TIPO_COMPRA | NUMBER |  | PK |  |  |

**Índices:**

- `PK_PPT_CHANNEL_TCOMPRA_IN` (UNIQUE) — COD_CHANNEL, COD_TIPO_COMPRA

### PPT_CHANNEL_TIPO_COMPRA_OUT

Tipos de Compra que se excluyen de un channel. Si no hay registros, no se excluye nada

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CHANNEL | NUMBER |  | PK | → PPT_CHANNEL |  |
| COD_TIPO_COMPRA | NUMBER |  | PK |  |  |

**Índices:**

- `PK_PPT_CHANNEL_TCOMPRA_OUT` (UNIQUE) — COD_TIPO_COMPRA, COD_CHANNEL

### PPT_CONCEPTOS_CONDICIONES

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CONCEPTO_CONDICION | NUMBER |  | PK |  |  |
| DES_CONCEPTO_CONDICION | VARCHAR2(50) |  |  |  |  |
| DES_ABRV_CONCEPTO_CONDICION | VARCHAR2(10) |  |  |  |  |
| IND_SIGNO | NUMBER(1,0) |  |  |  | Indica el signo del concepto (1 = Positivo, -1 = Negativo) |
| IND_CALCULO | NUMBER | ✓ |  |  | Indica para el concepto, sobre qué se hace el cálculo. (1=Neto Venta, 2 = Sobreprima, 0 = Elige el usuario). Estos valores se arrastran a PPT_CONDICION_MEDIO y si es 0, el usuario tiene que rellenar si será 1 ó 2 cuando se inserte en esta última tabla |
| F_ALTA | DATE |  |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |
| COD_CONCEPTO_ORDEN_INGES | NUMBER |  |  |  |  |

**Índices:**

- `PK_PPT_CONCEPTOS_CONDICIONES` (UNIQUE) — COD_CONCEPTO_CONDICION

### PPT_CONCEPTOS_CONDICIONES_PAIS

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CONCEPTO_CONDICION | NUMBER |  | PK | → PPT_CONCEPTOS_CONDICIONES |  |
| COD_PAIS | NUMBER |  | PK |  |  |

**Índices:**

- `PK_PPT_CONCEPTOS_CONDICIO_PAIS` (UNIQUE) — COD_CONCEPTO_CONDICION, COD_PAIS

### PPT_CONCEPTOS_PPTO_NETWORK

Entidad para relacionar los conceptos de presupuestos con las Network y asignarles su código de Magnitude

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CONCEPTO_PPTO_NETWORK | NUMBER |  | PK |  |  |
| COD_CONCEPTO_PRESUPUESTO | NUMBER |  |  | → PPT_CONCEPTOS_PRESUPUESTOS | Código de concepto del presupuesto |
| COD_NETWORK | NUMBER |  |  |  | Relacionado con NETWORK sin FK por estar en otro esquema |
| COD_CONCEPTO_MAGNITUDE | NUMBER |  |  |  | Código en Magnitude de la dupla Concepto Presupuesto + Network |
| F_ALTA | DATE |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |

**Índices:**

- `PK_PPT_CONCEPTOS_PPTO_NETWORK` (UNIQUE) — COD_CONCEPTO_PPTO_NETWORK
- `UQ_PPT_CONCEPTOS_PPTO_NETWORK` (UNIQUE) — COD_CONCEPTO_PRESUPUESTO, COD_NETWORK

### PPT_CONCEPTOS_PRESUPUESTOS

Almacena los conceptos de los presupuestos

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CONCEPTO_PRESUPUESTO | NUMBER |  | PK |  | Código concepto presupuesto |
| DES_CONCEPTO_PRESUPUESTO | VARCHAR2(50) |  |  |  | Descripción concepto presupuesto |
| DES_ABRV_CONCEPTO_PRESUPUESTO | VARCHAR2(20) |  |  |  | Abreviatura  concepto presupuesto |
| F_ALTA | DATE |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |
| DES_CONCEPTO_MINER_TRANSFER | VARCHAR2(50) |  |  |  |  |

**Índices:**

- `PK_PPT_CONCEPTOS_PRESUPUESTOS` (UNIQUE) — COD_CONCEPTO_PRESUPUESTO

### PPT_CONCEPTOS_SOBREPRIMA

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CONCEPTO_SOBREPRIMA | NUMBER |  | PK |  |  |
| COD_CONCEPTO_ORDEN_INGES | NUMBER |  |  |  | Relacionado con MMS (tabla CONCEPTO_ORDEN_INGES) |
| DES_CONCEPTO_SOBREPRIMA | VARCHAR2(50) |  |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |
| COD_TIPO_DEVOLUCION | NUMBER |  |  |  | Relacionado con MMS (tabla TIPO_DEVOLUCION) |

**Índices:**

- `PK_PPT_CONCEPTOS_SOBREPRIMA` (UNIQUE) — COD_CONCEPTO_SOBREPRIMA

### PPT_CONCEPTOS_SOBREPRIMAS_PAIS

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CONCEPTO_SOBREPRIMA | NUMBER |  | PK | → PPT_CONCEPTOS_SOBREPRIMA |  |
| COD_PAIS | NUMBER |  | PK |  |  |

**Índices:**

- `PK_PPTO_CONCEPTO_SOBREPRIMA_PA` (UNIQUE) — COD_CONCEPTO_SOBREPRIMA, COD_PAIS

### PPT_COND_EXC_ALCANCE

Guarda el código de alcance asociado al medio dentro de una jerarquía de excepciones. Solo puede haber un registro

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CONDICION_MEDIO | NUMBER |  | PK | → PPT_CONDICION_MEDIO |  |
| COD_ALCANCE | NUMBER |  |  |  |  |
| COD_USUARIO | NUMBER |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |
| F_ALTA | DATE |  |  |  |  |
| F_MODIFICACION | DATE |  |  |  |  |

**Índices:**

- `PK_PPT_COND_EXC_ALCANCE` (UNIQUE) — COD_CONDICION_MEDIO

### PPT_COND_EXC_DISCIPLINA

Guarda el código de disciplina asociado al medio dentro de una jerarquía de excepciones. Solo puede haber un registro

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CONDICION_MEDIO | NUMBER |  | PK | → PPT_CONDICION_MEDIO |  |
| COD_DISCIPLINA | NUMBER |  |  |  |  |
| COD_USUARIO | NUMBER |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |
| F_ALTA | DATE |  |  |  |  |
| F_MODIFICACION | DATE |  |  |  |  |

**Índices:**

- `PK_PPT_COND_EXC_DISCIPLINA` (UNIQUE) — COD_CONDICION_MEDIO

### PPT_COND_EXC_DISCIPLINA_GRUPO

Guarda el código de disciplina Grupo asociado al medio dentro de una jerarquía de excepciones. Solo puede haber un registro

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CONDICION_MEDIO | NUMBER |  | PK | → PPT_CONDICION_MEDIO |  |
| COD_DISCIPLINA_GRUPO | NUMBER |  |  |  |  |
| COD_USUARIO | NUMBER |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |
| F_ALTA | DATE |  |  |  |  |
| F_MODIFICACION | DATE |  |  |  |  |

**Índices:**

- `PK_PPT_COND_EXC_DISCIPLINA_GRP` (UNIQUE) — COD_CONDICION_MEDIO

### PPT_COND_EXC_DIVERSIFIED

Guarda el código de diversified asociado al medio dentro de una jerarquía de excepciones. Solo puede haber un registro

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CONDICION_MEDIO | NUMBER |  | PK | → PPT_CONDICION_MEDIO |  |
| COD_DIVERSIFIED | NUMBER |  |  |  |  |
| COD_USUARIO | NUMBER |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |
| F_ALTA | DATE |  |  |  |  |
| F_MODIFICACION | DATE |  |  |  |  |

**Índices:**

- `PK_PPT_COND_EXC_DIVERSIFIED` (UNIQUE) — COD_CONDICION_MEDIO

### PPT_COND_EXC_OBJETIVO

Guarda el código de objetivo asociado al medio dentro de una jerarquía de excepciones. Solo puede haber un registro

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CONDICION_MEDIO | NUMBER |  | PK | → PPT_CONDICION_MEDIO |  |
| COD_OBJETIVO | NUMBER |  |  |  |  |
| COD_USUARIO | NUMBER |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |
| F_ALTA | DATE |  |  |  |  |
| F_MODIFICACION | DATE |  |  |  |  |

**Índices:**

- `PK_PPT_COND_EXC_OBJETIVO` (UNIQUE) — COD_CONDICION_MEDIO

### PPT_COND_EXC_TIPO_COMPRA

Guarda el código de tipo de compra asociado al medio dentro de una jerarquía de excepciones. Solo puede haber un registro

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CONDICION_MEDIO | NUMBER |  | PK | → PPT_CONDICION_MEDIO |  |
| COD_TIPO_COMPRA | NUMBER |  |  |  |  |
| COD_USUARIO | NUMBER |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |
| F_ALTA | DATE |  |  |  |  |
| F_MODIFICACION | DATE |  |  |  |  |

**Índices:**

- `PK_PPT_COND_EXC_TIPO_COMPRA` (UNIQUE) — COD_CONDICION_MEDIO

### PPT_COND_EXC_TIPO_DISCIPLINA

Guarda el código de Tipo de Disciplina asociado al medio dentro de una jerarquía de excepciones. Solo puede haber un registro

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CONDICION_MEDIO | NUMBER |  | PK | → PPT_CONDICION_MEDIO |  |
| COD_TIPO_DISCIPLINA | NUMBER |  |  |  |  |
| COD_USUARIO | NUMBER |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |
| F_ALTA | DATE |  |  |  |  |
| F_MODIFICACION | DATE |  |  |  |  |

**Índices:**

- `PK_PPT_COND_EXC_TIPO_DISCIPLIN` (UNIQUE) — COD_CONDICION_MEDIO

### PPT_CONDICION_MEDIO

Guarda los porcentajes por medio de la vigencia con la que está relacionada. La del propio medio, sin nada más,  tendrá el número de jerarquía = 0, mientras que la del medio combinada con otros objetos tendrá jerarquía > 0, mantenida por el usuario. Estos objetos tienen sus tablas personalizadas para guardar qué objeto es.

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CONDICION_MEDIO | NUMBER |  | PK |  |  |
| COD_PAIS | NUMBER |  |  |  | Relacionado junto con COD_MEDIO con MEDIO |
| COD_MEDIO | NUMBER |  |  |  | Relacionado junto con COD_PAIS con MEDIO |
| COD_CONDICION_VIGENCIA | NUMBER |  |  | → PPT_CONDICION_VIGENCIA | Relacionado con PPT_CONDICION_VIGENCIA |
| COD_CONCEPTO_CONDICION | NUMBER |  |  | → PPT_CONCEPTOS_CONDICIONES |  |
| IND_CALCULO | NUMBER |  |  |  | Indica para el concepto, sobre qué se hace el cálculo. (1=Neto Venta, 2 = Sobreprima) |
| NUM_JERARQUIA | NUMBER |  |  |  | Jerarquía = 0 es para el registro que guarda los % del Medio, sin tener relación con las tablas de excepciones.  Para jerarquías NO 0, es para el medio más la combinación con las tablas de excepciones |
| PCT_CONDICION_MEDIO | NUMBER(5,2) | ✓ |  |  | Porcentaje de la condición según el concepto de la vigencia para cada medio |
| COD_USUARIO | NUMBER |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |
| F_ALTA | DATE |  |  |  |  |
| F_MODIFICACION | DATE |  |  |  |  |

**Índices:**

- `IND_PPT_CONDICION_MEDIO_CONCP` — COD_CONCEPTO_CONDICION
- `IND_PPT_CONDICION_MEDIO_VIG` — COD_CONDICION_VIGENCIA
- `PK_PPT_CONDICION_MEDIO` (UNIQUE) — COD_CONDICION_MEDIO
- `UQ_PPT_CONDICION_MEDIO` (UNIQUE) — COD_PAIS, COD_MEDIO, COD_CONDICION_VIGENCIA, COD_CONCEPTO_CONDICION, NUM_JERARQUIA

### PPT_CONDICION_VIGENCIA

Vigencias en un año (versión) / concepto / network / pais / grupo / indicador acuerdo. Los meses desde y hasta no se pueden solapar en el mismo año 

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CONDICION_VIGENCIA | NUMBER |  | PK |  | Secuencia SQC_CONDICION_VIGENCIA |
| COD_VERSION | NUMBER |  |  | → PPT_VERSIONES |  |
| COD_NETWORK | NUMBER |  |  |  |  |
| COD_PAIS | NUMBER |  |  |  |  |
| COD_GRUPO | NUMBER |  |  |  |  |
| MES_DESDE | NUMBER(2,0) |  |  |  | Per |
| MES_HASTA | NUMBER(2,0) |  |  |  |  |
| IND_ACUERDO | NUMBER |  |  |  | 1 = Sí     0 = No |
| COD_USUARIO_ALTA | NUMBER |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |
| F_ALTA | DATE |  |  |  |  |
| F_MODIFICACION | DATE |  |  |  |  |

**Índices:**

- `IND_PPT_COND_VIG_COMPUESTO` — COD_VERSION, COD_NETWORK, COD_PAIS, COD_GRUPO, IND_ACUERDO
- `IND_PPT_CONDICION_VIG_VER` — COD_VERSION
- `PK_PPT_CONDICION_VIGENCIA` (UNIQUE) — COD_CONDICION_VIGENCIA

### PPT_CONFIGURACION

Existe un Trgiger que valida que el contenido de VALOR es acorde al tipo de dato de VALOR

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CONFIGURACION | NUMBER |  | PK |  | Código de configuración |
| DES_CONFIGURACION | VARCHAR2(100) |  |  |  | Descripción del parámetro de configuración |
| TIPO_VALOR | NUMBER |  |  |  | 1 - VARCHAR     2 - NUMBER   3 - DATE (DD/MM/YYYY) 4 - TIME (HH24:MI:SS)     5 - DATETIME (DD/MM/YYYY HH24:MI:SS)    6 - JSON 7 - XML |
| VALOR | CLOB |  |  |  | Valor del parámetro |
| COMENTARIO | VARCHAR2(1000) |  |  |  | Comentarios adiccionales para compresión del parámetro |
| F_ALTA | DATE |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |
| F_MODIFICACION | DATE |  |  |  |  |

**Índices:**

- `PK_PPT_CONFIGURACION` (UNIQUE) — COD_CONFIGURACION

### PPT_ESTADOS_VERSIONES

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_ESTADO_VERSION | NUMBER |  | PK |  |  |
| BITAND | NUMBER |  |  |  |  |
| DES_ESTADO_VERSION | VARCHAR2(50) |  |  |  |  |
| IND_MOSTRAR | NUMBER |  |  |  |  |
| IND_VERSION_UNICA | NUMBER |  |  |  | 0: Es un esto, por ejemplo abierta, cerrada, etc  1:Es un tipo de version, que solo puede haber uno por versión (por ejemplo, objetivo, version abierta, FT1, etc... |
| ORDEN | NUMBER |  |  |  | Orden para mostrar en la pantalla |

**Índices:**

- `PK_PPT_ESTADOS_VERSIONES` (UNIQUE) — COD_ESTADO_VERSION

### PPT_ESTADOS_VERSIONES_IDIOMAS

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_ESTADO_VERSION_IDIOMA | NUMBER |  | PK |  |  |
| COD_ESTADO_VERSION | NUMBER |  |  | → PPT_ESTADOS_VERSIONES |  |
| COD_IDIOMA | NUMBER |  |  |  |  |
| DES_ESTADO_VERSION_IDIOMA | VARCHAR2(50) |  |  |  |  |
| DES_ESTADO_VERSION_ABRV_IDIOMA | VARCHAR2(10) |  |  |  |  |
| LEYENDA | VARCHAR2(100) |  |  |  |  |

**Índices:**

- `PK_PPT_ESTADOS_VERSIONES_IDIOM` (UNIQUE) — COD_ESTADO_VERSION_IDIOMA
- `UQ_ESTADO_VERSION_IDIOMA` (UNIQUE) — COD_ESTADO_VERSION, COD_IDIOMA

### PPT_PREVISION_CONDICIONES_ABS

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PREVISION_CONDICION_ABS | NUMBER |  | PK |  |  |
| COD_VERSION | NUMBER |  |  | → PPT_VERSIONES |  |
| COD_NETWORK | NUMBER |  |  |  |  |
| MES | NUMBER(2,0) |  |  |  |  |
| COD_PAIS | NUMBER |  |  |  |  |
| COD_GRUPO | NUMBER |  |  |  |  |
| COD_MEDIO | NUMBER |  |  |  |  |
| COD_EDITORIAL_COMERCIAL | NUMBER |  |  |  |  |
| COD_ALCANCE | NUMBER |  |  |  |  |
| COD_OBJETIVO | NUMBER |  |  |  |  |
| COD_DISCIPLINA | NUMBER |  |  |  |  |
| COD_DIVERSIFIED | NUMBER |  |  |  |  |
| COD_TIPO_COMPRA | NUMBER |  |  |  |  |
| IND_ACUERDO | NUMBER(1,0) |  |  |  | 1: Es un acuerdo 0: No es un acuerdo |
| IND_INTERCO | NUMBER(1,0) |  |  |  |  |
| IND_MEDIO_0 | NUMBER(1,0) |  |  |  |  |
| ANIO | NUMBER(4,0) |  |  | → PPT_VERSIONES | Para la particion |
| COD_TIPO_VERSION | NUMBER |  |  | → PPT_VERSIONES | Para particionar, porque está en versiones |
| IMP_SAG_ABS | NUMBER |  |  |  |  |
| IMP_DEV_ABS | NUMBER |  |  |  |  |
| IMP_MANPOWER_ABS | NUMBER |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |
| F_MODIFICACION | DATE |  |  |  |  |

**Índices:**

- `PK_PPT_PREVISION_CONDICNS_ABS` (UNIQUE) — COD_PREVISION_CONDICION_ABS
- `UQ_PPT_PREVISIONES_COND_ABS` (UNIQUE) — COD_VERSION, COD_NETWORK, MES, COD_PAIS, COD_GRUPO, COD_MEDIO, COD_EDITORIAL_COMERCIAL, COD_ALCANCE, COD_OBJETIVO, COD_DISCIPLINA, COD_DIVERSIFIED, COD_TIPO_COMPRA, IND_ACUERDO, IND_INTERCO, IND_MEDIO_0

### PPT_PREVISION_CONDICIONES_FIJO

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PREVISION_CONDICION_FIJO | NUMBER |  | PK |  |  |
| COD_VERSION | NUMBER |  |  | → PPT_VERSIONES |  |
| COD_NETWORK | NUMBER |  |  |  |  |
| MES | NUMBER(2,0) |  |  |  |  |
| COD_PAIS | NUMBER |  |  |  |  |
| COD_GRUPO | NUMBER |  |  |  |  |
| COD_MEDIO | NUMBER |  |  |  |  |
| COD_EDITORIAL_COMERCIAL | NUMBER |  |  |  |  |
| COD_ALCANCE | NUMBER |  |  |  |  |
| COD_OBJETIVO | NUMBER |  |  |  |  |
| COD_DISCIPLINA | NUMBER |  |  |  |  |
| COD_DIVERSIFIED | NUMBER |  |  |  |  |
| COD_TIPO_COMPRA | NUMBER |  |  |  |  |
| IND_ACUERDO | NUMBER(1,0) |  |  |  | 1: Es un acuerdo 0: No es un acuerdo |
| IND_INTERCO | NUMBER(1,0) |  |  |  |  |
| IND_MEDIO_0 | NUMBER(1,0) |  |  |  |  |
| ANIO | NUMBER(4,0) |  |  | → PPT_VERSIONES | Para la particion |
| COD_TIPO_VERSION | NUMBER |  |  | → PPT_VERSIONES | Para particionar, porque está en versiones |
| IMP_SAG_FIJO | NUMBER |  |  |  |  |
| IMP_DEV_FIJO | NUMBER |  |  |  |  |
| IMP_MANPOWER_FIJO | NUMBER |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |
| F_MODIFICACION | DATE |  |  |  |  |

**Índices:**

- `PK_PPT_PREVISION_CONDICNS_FIJO` (UNIQUE) — COD_PREVISION_CONDICION_FIJO
- `UQ_PPT_PREVISIONES_COND_FIJO` (UNIQUE) — COD_VERSION, COD_NETWORK, MES, COD_PAIS, COD_GRUPO, COD_MEDIO, COD_EDITORIAL_COMERCIAL, COD_ALCANCE, COD_OBJETIVO, COD_DISCIPLINA, COD_DIVERSIFIED, COD_TIPO_COMPRA, IND_ACUERDO, IND_INTERCO, IND_MEDIO_0

### PPT_PREVISION_CONDICIONES_REAL_OLD

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PREVISION_CONDICION_REAL | NUMBER |  | PK |  |  |
| COD_VERSION | NUMBER |  |  | → PPT_VERSIONES |  |
| COD_NETWORK | NUMBER |  |  |  |  |
| MES | NUMBER(2,0) |  |  |  |  |
| COD_PAIS | NUMBER |  |  |  |  |
| COD_GRUPO | NUMBER |  |  |  |  |
| COD_MEDIO | NUMBER |  |  |  |  |
| COD_EDITORIAL_COMERCIAL | NUMBER |  |  |  |  |
| COD_ALCANCE | NUMBER |  |  |  |  |
| COD_OBJETIVO | NUMBER |  |  |  |  |
| COD_DISCIPLINA | NUMBER |  |  |  |  |
| COD_DIVERSIFIED | NUMBER |  |  |  |  |
| COD_TIPO_COMPRA | NUMBER |  |  |  |  |
| IND_ACUERDO | NUMBER(1,0) |  |  |  | 1: Es un acuerdo 0: No es un acuerdo |
| IND_INTERCO | NUMBER(1,0) |  |  |  |  |
| IND_MEDIO_0 | NUMBER(1,0) |  |  |  |  |
| ANIO | NUMBER(4,0) |  |  |  | Para la particion |
| COD_TIPO_VERSION | NUMBER |  |  | → PPT_TIPOS_VERSIONES | Para particionar, porque está en versiones |
| IMP_SAG | NUMBER |  |  |  |  |
| IMP_SAG_FIJO | NUMBER |  |  |  |  |
| IMP_DEV | NUMBER |  |  |  |  |
| IMP_DEV_FIJO | NUMBER |  |  |  |  |
| IMP_MANPOWER | NUMBER |  |  |  |  |
| IMP_MANPOWER_FIJO | NUMBER |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |
| F_MODIFICACION | DATE |  |  |  |  |

**Índices:**

- `IND_PPT_PREV_COND_REAL_TIP_VER` — COD_TIPO_VERSION
- `PK_PPT_PREVISION_CONDICNS_REAL` (UNIQUE) — COD_PREVISION_CONDICION_REAL
- `UQ_PPT_PREVISIONES_COND_REAL` (UNIQUE) — COD_VERSION, COD_NETWORK, MES, COD_PAIS, COD_GRUPO, COD_MEDIO, COD_EDITORIAL_COMERCIAL, COD_ALCANCE, COD_OBJETIVO, COD_DISCIPLINA, COD_DIVERSIFIED, COD_TIPO_COMPRA, IND_ACUERDO, IND_INTERCO, IND_MEDIO_0

### PPT_PREVISION_SOBREPRIMAS_ABS

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PREVISION_SOBREPRIMA_ABS | NUMBER |  | PK |  |  |
| COD_VERSION | NUMBER |  |  | → PPT_VERSIONES |  |
| COD_NETWORK | NUMBER |  |  |  |  |
| MES | NUMBER(2,0) |  |  |  |  |
| COD_PAIS | NUMBER |  |  |  |  |
| COD_GRUPO | NUMBER |  |  |  |  |
| COD_MEDIO | NUMBER |  |  |  |  |
| COD_EDITORIAL_COMERCIAL | NUMBER |  |  |  |  |
| COD_ALCANCE | NUMBER |  |  |  |  |
| COD_OBJETIVO | NUMBER |  |  |  |  |
| COD_DISCIPLINA | NUMBER |  |  |  |  |
| COD_DIVERSIFIED | NUMBER |  |  |  |  |
| COD_TIPO_COMPRA | NUMBER |  |  |  |  |
| IND_ACUERDO | NUMBER(1,0) |  |  |  |  |
| IND_INTERCO | NUMBER(1,0) |  |  |  |  |
| IND_MEDIO_0 | NUMBER(1,0) |  |  |  |  |
| ANIO | NUMBER(4,0) |  |  | → PPT_VERSIONES |  |
| COD_TIPO_VERSION | NUMBER |  |  | → PPT_VERSIONES |  |
| IMP_SOBREPRIMA | NUMBER |  |  |  |  |
| IMP_SLA | NUMBER |  |  |  |  |
| IMP_HVP | NUMBER | ✓ |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |
| F_MODIFICACION | DATE |  |  |  |  |

**Índices:**

- `IDX_PPT_PRV_SB_ABS_V_GR` — COD_VERSION, COD_GRUPO
- `IDX_PPT_PRV_SB_ABS_VER` — COD_VERSION
- `IXFK_PPT_PREVISIO_PPT_TIP01` — COD_TIPO_VERSION
- `PK_PPT_PREVISION_SOB_ABS` (UNIQUE) — COD_PREVISION_SOBREPRIMA_ABS
- `UQ_PPT_PREVISIONES_SOB_ABS` — COD_VERSION, COD_NETWORK, MES, COD_PAIS, COD_GRUPO, COD_MEDIO, COD_EDITORIAL_COMERCIAL, COD_ALCANCE, COD_OBJETIVO, COD_DISCIPLINA, COD_DIVERSIFIED, COD_TIPO_COMPRA

### PPT_PREVISIONES

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PREVISION | NUMBER |  | PK |  |  |
| COD_VERSION | NUMBER |  |  | → PPT_VERSIONES |  |
| COD_NETWORK | NUMBER |  |  |  |  |
| MES | NUMBER(2,0) |  |  |  |  |
| COD_PAIS | NUMBER |  |  |  |  |
| COD_GRUPO | NUMBER |  |  |  |  |
| COD_MEDIO | NUMBER |  |  |  |  |
| COD_EDITORIAL_COMERCIAL | NUMBER |  |  |  |  |
| COD_ALCANCE | NUMBER |  |  |  |  |
| COD_OBJETIVO | NUMBER |  |  |  |  |
| COD_DISCIPLINA | NUMBER |  |  |  |  |
| COD_DIVERSIFIED | NUMBER |  |  |  |  |
| COD_TIPO_COMPRA | NUMBER |  |  |  |  |
| IND_ACUERDO | NUMBER(1,0) |  |  |  | 1: Es un acuerdo 0: No es un acuerdo |
| ANIO | NUMBER(4,0) |  |  | → PPT_VERSIONES | Para la particion |
| COD_TIPO_VERSION | NUMBER |  |  | → PPT_VERSIONES | Para particionar, porque está en versiones |
| IMP_NETO_VENTA | NUMBER |  |  |  |  |
| IMP_NETO_COMPRA | NUMBER |  |  |  |  |
| IMP_RAPPEL_NO_CMP | NUMBER |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |
| F_MODIFICACION | DATE |  |  |  |  |
| IND_INTERCO | NUMBER(1,0) |  |  |  | Indicador de Intercompany (1 = Intercompany, 0 = No Intercompany) |
| IND_MEDIO_0 | NUMBER(1,0) |  |  |  | Indicador de Medio 0 (1 = Es Medio 0, 0 = NO es Medio 0) |

**Índices:**

- `IND_PPT_PREV_PAIS_INT_VER` — COD_PAIS, IND_INTERCO, COD_VERSION
- `IND_PPT_PREV_PAIS_INT_VER_EDIT` — COD_PAIS, IND_INTERCO, COD_VERSION, COD_EDITORIAL_COMERCIAL
- `IND_PPT_PREV_PAIS_INT_VER_GRU` — COD_PAIS, IND_INTERCO, COD_VERSION, COD_GRUPO
- `IND_PPT_PREV_VER_MES_NET_MED` — COD_VERSION, MES, COD_NETWORK, COD_MEDIO
- `IND_PPT_PREVISIONES_TIP_VER` — COD_TIPO_VERSION
- `IND_PPT_PREVISIONES_VER_GRP` — COD_VERSION, COD_GRUPO
- `IND_PPT_PREVISIONES_VER_MES` — COD_VERSION, MES
- `IND_PPT_PREVISIONES_VER_MES_GRP` — COD_VERSION, MES, COD_GRUPO
- `IND_PPT_PREVISIONES_VER_NET_MED` — COD_VERSION, COD_NETWORK, COD_MEDIO
- `PK_PPT_PREVISIONES` (UNIQUE) — COD_PREVISION
- `UQ_PPT_PREVISIONES` (UNIQUE) — COD_VERSION, COD_NETWORK, MES, COD_PAIS, COD_GRUPO, COD_MEDIO, COD_EDITORIAL_COMERCIAL, COD_ALCANCE, COD_OBJETIVO, COD_DISCIPLINA, COD_DIVERSIFIED, COD_TIPO_COMPRA, IND_ACUERDO, IND_INTERCO, IND_MEDIO_0

### PPT_PREVISIONES_AJUSTES

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PREVISION_AJUSTE | NUMBER |  | PK |  |  |
| COD_VERSION | NUMBER |  |  | → PPT_VERSIONES |  |
| COD_NETWORK | NUMBER |  |  |  |  |
| MES | NUMBER(2,0) |  |  |  |  |
| COD_PAIS | NUMBER |  |  |  |  |
| COD_GRUPO | NUMBER |  |  |  |  |
| COD_MEDIO | NUMBER |  |  |  |  |
| COD_EDITORIAL_COMERCIAL | NUMBER |  |  |  |  |
| COD_ALCANCE | NUMBER |  |  |  |  |
| COD_OBJETIVO | NUMBER |  |  |  |  |
| COD_DISCIPLINA | NUMBER |  |  |  |  |
| COD_DIVERSIFIED | NUMBER |  |  |  |  |
| COD_TIPO_COMPRA | NUMBER |  |  |  |  |
| ANIO | NUMBER(4,0) |  |  | → PPT_VERSIONES | Para la particion si se pone de momento no parece necesaria |
| COD_TIPO_VERSION | NUMBER |  |  | → PPT_VERSIONES | Para particionar, porque está en versiones |
| IMP_NETO_VENTA | NUMBER |  |  |  |  |
| IMP_NETO_COMPRA | NUMBER |  |  |  |  |
| IMP_SAG | NUMBER |  |  |  |  |
| IMP_DEVOLUCION | NUMBER |  |  |  |  |
| IMP_SOBREPRIMA | NUMBER |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |
| F_MODIFICACION | DATE |  |  |  |  |

**Índices:**

- `IXFK_PPT_PREVISIO_PPT_VER01` — COD_VERSION
- `PK_PPT_PREVISIONES_AJUSTES` (UNIQUE) — COD_PREVISION_AJUSTE
- `UQ_PPT_PREVISIONES_AJUSTES` (UNIQUE) — COD_VERSION, COD_NETWORK, MES, COD_PAIS, COD_GRUPO, COD_MEDIO, COD_EDITORIAL_COMERCIAL, COD_ALCANCE, COD_OBJETIVO, COD_DISCIPLINA, COD_DIVERSIFIED, COD_TIPO_COMPRA

### PPT_PREVISIONES_AJUSTES_AUD

Tabla de auditoría para PPT_PREVISIONES_AJUSTES

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PPT_PREVISIONES_AJUSTES_AUD | NUMBER |  | PK |  |  |
| COD_PREVISION_AJUSTE | NUMBER |  |  |  |  |
| COD_VERSION_OLD | NUMBER | ✓ |  |  |  |
| COD_VERSION_NEW | NUMBER | ✓ |  |  |  |
| COD_NETWORK_OLD | NUMBER | ✓ |  |  |  |
| COD_NETWORK_NEW | NUMBER | ✓ |  |  |  |
| MES_OLD | NUMBER | ✓ |  |  |  |
| MES_NEW | NUMBER | ✓ |  |  |  |
| COD_PAIS_OLD | NUMBER | ✓ |  |  |  |
| COD_PAIS_NEW | NUMBER | ✓ |  |  |  |
| COD_GRUPO_OLD | NUMBER | ✓ |  |  |  |
| COD_GRUPO_NEW | NUMBER | ✓ |  |  |  |
| COD_MEDIO_OLD | NUMBER | ✓ |  |  |  |
| COD_MEDIO_NEW | NUMBER | ✓ |  |  |  |
| COD_EDITORIAL_COMERCIAL_OLD | NUMBER | ✓ |  |  |  |
| COD_EDITORIAL_COMERCIAL_NEW | NUMBER | ✓ |  |  |  |
| COD_ALCANCE_OLD | NUMBER | ✓ |  |  |  |
| COD_ALCANCE_NEW | NUMBER | ✓ |  |  |  |
| COD_OBJETIVO_OLD | NUMBER | ✓ |  |  |  |
| COD_OBJETIVO_NEW | NUMBER | ✓ |  |  |  |
| COD_DISCIPLINA_OLD | NUMBER | ✓ |  |  |  |
| COD_DISCIPLINA_NEW | NUMBER | ✓ |  |  |  |
| COD_DIVERSIFIED_OLD | NUMBER | ✓ |  |  |  |
| COD_DIVERSIFIED_NEW | NUMBER | ✓ |  |  |  |
| COD_TIPO_COMPRA_OLD | NUMBER | ✓ |  |  |  |
| COD_TIPO_COMPRA_NEW | NUMBER | ✓ |  |  |  |
| ANIO_OLD | NUMBER(4,0) | ✓ |  |  |  |
| ANIO_NEW | NUMBER(4,0) | ✓ |  |  |  |
| COD_TIPO_VERSION_OLD | NUMBER | ✓ |  |  |  |
| COD_TIPO_VERSION_NEW | NUMBER | ✓ |  |  |  |
| IMP_NETO_VENTA_OLD | NUMBER | ✓ |  |  |  |
| IMP_NETO_VENTA_NEW | NUMBER | ✓ |  |  |  |
| IMP_NETO_COMPRA_OLD | NUMBER | ✓ |  |  |  |
| IMP_NETO_COMPRA_NEW | NUMBER | ✓ |  |  |  |
| IMP_SAG_OLD | NUMBER | ✓ |  |  |  |
| IMP_SAG_NEW | NUMBER | ✓ |  |  |  |
| IMP_DEVOLUCION_OLD | NUMBER | ✓ |  |  |  |
| IMP_DEVOLUCION_NEW | NUMBER | ✓ |  |  |  |
| IMP_SOBREPRIMA_OLD | NUMBER | ✓ |  |  |  |
| IMP_SOBREPRIMA_NEW | NUMBER | ✓ |  |  |  |
| COD_USUARIO_MODIFICACION_OLD | NUMBER | ✓ |  |  |  |
| COD_USUARIO_MODIFICACION_NEW | NUMBER | ✓ |  |  |  |
| F_MODIFICACION_OLD | DATE | ✓ |  |  |  |
| F_MODIFICACION_NEW | DATE | ✓ |  |  |  |
| TIPO_EVENTO | VARCHAR2(10) |  |  |  |  |
| F_EVENTO | DATE |  |  |  | Fecha y hora en que se produjo el evento de auditoría |
| BD_USUARIO_EVENTO | VARCHAR2(30) |  |  |  | Usuario de base de datos que realizó la operación |
| OS_USUARIO_EVENTO | VARCHAR2(30) |  |  |  | Usuario del sistema operativo que ejecutó la sesión |
| IP_EVENTO | VARCHAR2(50) |  |  |  | Dirección IP del cliente que realizó la operación |
| CLIENT_IDENTIFIER_EVENTO | VARCHAR2(64) | ✓ |  |  | Identificador de usuario de aplicación (valor de CLIENT_IDENTIFIER, asignado mediante DBMS_SESSION.SET_IDENTIFIER) |

**Índices:**

- `PK_PPT_PREVISIONES_AJUSTES_AUD` (UNIQUE) — COD_PPT_PREVISIONES_AJUSTES_AUD

### PPT_PREVISIONES_AUD

Tabla de auditoría para PPT_PREVISIONES

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PPT_PREVISIONES_AUD | NUMBER |  | PK |  |  |
| COD_PREVISION | NUMBER |  |  |  |  |
| COD_VERSION_OLD | NUMBER | ✓ |  |  |  |
| COD_VERSION_NEW | NUMBER | ✓ |  |  |  |
| COD_NETWORK_OLD | NUMBER | ✓ |  |  |  |
| COD_NETWORK_NEW | NUMBER | ✓ |  |  |  |
| MES_OLD | NUMBER(2,0) | ✓ |  |  |  |
| MES_NEW | NUMBER(2,0) | ✓ |  |  |  |
| COD_PAIS_OLD | NUMBER | ✓ |  |  |  |
| COD_PAIS_NEW | NUMBER | ✓ |  |  |  |
| COD_GRUPO_OLD | NUMBER | ✓ |  |  |  |
| COD_GRUPO_NEW | NUMBER | ✓ |  |  |  |
| COD_MEDIO_OLD | NUMBER | ✓ |  |  |  |
| COD_MEDIO_NEW | NUMBER | ✓ |  |  |  |
| COD_EDITORIAL_COMERCIAL_OLD | NUMBER | ✓ |  |  |  |
| COD_EDITORIAL_COMERCIAL_NEW | NUMBER | ✓ |  |  |  |
| COD_ALCANCE_OLD | NUMBER | ✓ |  |  |  |
| COD_ALCANCE_NEW | NUMBER | ✓ |  |  |  |
| COD_OBJETIVO_OLD | NUMBER | ✓ |  |  |  |
| COD_OBJETIVO_NEW | NUMBER | ✓ |  |  |  |
| COD_DISCIPLINA_OLD | NUMBER | ✓ |  |  |  |
| COD_DISCIPLINA_NEW | NUMBER | ✓ |  |  |  |
| COD_DIVERSIFIED_OLD | NUMBER | ✓ |  |  |  |
| COD_DIVERSIFIED_NEW | NUMBER | ✓ |  |  |  |
| COD_TIPO_COMPRA_OLD | NUMBER | ✓ |  |  |  |
| COD_TIPO_COMPRA_NEW | NUMBER | ✓ |  |  |  |
| IND_ACUERDO_OLD | NUMBER(1,0) | ✓ |  |  |  |
| IND_ACUERDO_NEW | NUMBER(1,0) | ✓ |  |  |  |
| ANIO_OLD | NUMBER(4,0) | ✓ |  |  |  |
| ANIO_NEW | NUMBER(4,0) | ✓ |  |  |  |
| COD_TIPO_VERSION_OLD | NUMBER | ✓ |  |  |  |
| COD_TIPO_VERSION_NEW | NUMBER | ✓ |  |  |  |
| IMP_NETO_VENTA_OLD | NUMBER | ✓ |  |  |  |
| IMP_NETO_VENTA_NEW | NUMBER | ✓ |  |  |  |
| IMP_NETO_COMPRA_OLD | NUMBER | ✓ |  |  |  |
| IMP_NETO_COMPRA_NEW | NUMBER | ✓ |  |  |  |
| IMP_RAPPEL_NO_CMP_OLD | NUMBER | ✓ |  |  |  |
| IMP_RAPPEL_NO_CMP_NEW | NUMBER | ✓ |  |  |  |
| COD_USUARIO_MODIFICACION_OLD | NUMBER | ✓ |  |  |  |
| COD_USUARIO_MODIFICACION_NEW | NUMBER | ✓ |  |  |  |
| F_MODIFICACION_OLD | DATE | ✓ |  |  |  |
| F_MODIFICACION_NEW | DATE | ✓ |  |  |  |
| IND_INTERCO_OLD | NUMBER(1,0) | ✓ |  |  |  |
| IND_INTERCO_NEW | NUMBER(1,0) | ✓ |  |  |  |
| IND_MEDIO_0_OLD | NUMBER(1,0) | ✓ |  |  |  |
| IND_MEDIO_0_NEW | NUMBER(1,0) | ✓ |  |  |  |
| TIPO_EVENTO | VARCHAR2(10) |  |  |  |  |
| F_EVENTO | DATE |  |  |  | Fecha y hora en que se produjo el evento de auditoría |
| BD_USUARIO_EVENTO | VARCHAR2(30) |  |  |  | Usuario de base de datos que realizó la operación |
| OS_USUARIO_EVENTO | VARCHAR2(30) |  |  |  | Usuario del sistema operativo que ejecutó la sesión |
| IP_EVENTO | VARCHAR2(50) |  |  |  | Dirección IP del cliente que realizó la operación |
| CLIENT_IDENTIFIER_EVENTO | VARCHAR2(64) | ✓ |  |  | Identificador de usuario de aplicación (valor de CLIENT_IDENTIFIER, asignado mediante DBMS_SESSION.SET_IDENTIFIER) |

**Índices:**

- `PK_PPT_PREVISIONES_AUD` (UNIQUE) — COD_PPT_PREVISIONES_AUD

### PPT_PUBLISHER

Maestro de publishers para los reportes de Itracker

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PUBLISHER | NUMBER |  | PK |  |  |
| DES_PUBLISHER | VARCHAR2(100) |  |  |  |  |
| IND_MODO_CALCULO | NUMBER(1,0) |  |  |  | 1-. Neto Compra    2-. Inversión con Sobreprima |
| IND_ORIGEN | NUMBER(1,0) |  |  |  | 1-. Previsiones     2- MMS |

**Índices:**

- `PK_PPT_PUBLISHER` (UNIQUE) — COD_PUBLISHER

### PPT_PUBLISHER_AGRP_COMER_IN

Agrupaciones Comerciales que componen ese publisher. Si no hay, se cogen todos.  Las agrupaciones comerciales están en un CATEGORIZER y corresponden a CATEGORIA_TIPO.COD_CATEGORIA = 19   AND CATEGORIA_TIPO.COD_CATEGORIA_TIPO = 82

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PUBLISHER | NUMBER |  | PK | → PPT_PUBLISHER |  |
| COD_AGRUPACION_COMERCIAL | NUMBER |  | PK |  |  |

**Índices:**

- `PK_PPT_PUBLISHER_AGRP_COMER_IN` (UNIQUE) — COD_PUBLISHER, COD_AGRUPACION_COMERCIAL

### PPT_PUBLISHER_AGRP_COMER_OUT

Agrupaciones Comerciales que se excluyen de este publisher. Si no hay, no se excluye nada.  Las agrupaciones comerciales están en un CATEGORIZER y corresponden a CATEGORIA_TIPO.COD_CATEGORIA = 19   AND CATEGORIA_TIPO.COD_CATEGORIA_TIPO = 82

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PUBLISHER | NUMBER |  | PK | → PPT_PUBLISHER |  |
| COD_AGRUPACION_COMERCIAL | NUMBER |  | PK |  |  |

**Índices:**

- `PK_PPT_PUBLISHER_AGR_COMER_OUT` (UNIQUE) — COD_PUBLISHER, COD_AGRUPACION_COMERCIAL

### PPT_PUBLISHER_DISCIP_GRUPO_IN

Disciplinas Grupo que componen ese publisher. Si no hay, se cogen todos

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PUBLISHER | NUMBER |  | PK | → PPT_PUBLISHER |  |
| COD_DISCIPLINA_GRUPO | NUMBER |  | PK |  |  |

**Índices:**

- `PK_PPT_PUBLISHER_DISCIP_GRP_IN` (UNIQUE) — COD_PUBLISHER, COD_DISCIPLINA_GRUPO

### PPT_PUBLISHER_DISCIP_GRUPO_OUT

Disciplinas Grupo que se excluyen de un publisher. Si no hay registros, no se excluye nada

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PUBLISHER | NUMBER |  | PK | → PPT_PUBLISHER |  |
| COD_DISCIPLINA_GRUPO | NUMBER |  | PK |  |  |

**Índices:**

- `PK_PPT_PUBLISHER_DISCP_GRP_OUT` (UNIQUE) — COD_PUBLISHER, COD_DISCIPLINA_GRUPO

### PPT_PUBLISHER_DISCIPLINA_IN

Disciplinas que componen ese publisher. Si no hay, se cogen todos

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PUBLISHER | NUMBER |  | PK | → PPT_PUBLISHER |  |
| COD_DISCIPLINA | NUMBER |  | PK |  |  |

**Índices:**

- `PK_PPT_PUBLISHER_DISCIPLINA_IN` (UNIQUE) — COD_PUBLISHER, COD_DISCIPLINA

### PPT_PUBLISHER_DISCIPLINA_OUT

Disciplinas que se excluyen de un publisher. Si no hay registros, no se excluye nada

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PUBLISHER | NUMBER |  | PK | → PPT_PUBLISHER |  |
| COD_DISCIPLINA | NUMBER |  | PK |  |  |

**Índices:**

- `PK_PPT_PUBLISHER_DISCPLINA_OUT` (UNIQUE) — COD_PUBLISHER, COD_DISCIPLINA

### PPT_PUBLISHER_EDIT_COMER_IN

Editoriales comerciales que componen ese publisher. Si no hay, se cogen todos

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PUBLISHER | NUMBER |  | PK | → PPT_PUBLISHER |  |
| COD_EDITORIAL_COMERCIAL | NUMBER |  | PK |  |  |

**Índices:**

- `PK_PPT_PUBLISHER_EDIT_COMER_IN` (UNIQUE) — COD_PUBLISHER, COD_EDITORIAL_COMERCIAL

### PPT_PUBLISHER_EDIT_COMER_OUT

Editoriales Comerciales que se excluyen de un publisher. Si no hay registros, no se excluye nada

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PUBLISHER | NUMBER |  | PK | → PPT_PUBLISHER |  |
| COD_EDITORIAL_COMERCIAL | NUMBER |  | PK |  |  |

**Índices:**

- `PK_PPT_PUBLISHER_EDIT_COMER_OUT` (UNIQUE) — COD_PUBLISHER, COD_EDITORIAL_COMERCIAL

### PPT_PUBLISHER_MEDIO_IN

Medios que componen ese publisher. Si no hay, se cogen todos

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PUBLISHER | NUMBER |  | PK | → PPT_PUBLISHER |  |
| COD_PAIS | NUMBER |  | PK |  |  |
| COD_MEDIO | NUMBER |  | PK |  |  |

**Índices:**

- `PK_PPT_PUBLISHER_MEDIO_IN` (UNIQUE) — COD_PUBLISHER, COD_PAIS, COD_MEDIO

### PPT_PUBLISHER_MEDIO_OUT

Medios que se excluyen de un publisher. Si no hay registros, no se excluye nada

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PUBLISHER | NUMBER |  | PK | → PPT_PUBLISHER |  |
| COD_PAIS | NUMBER |  | PK |  |  |
| COD_MEDIO | NUMBER |  | PK |  |  |

**Índices:**

- `PK_PPT_PUBLISHER_MEDIO_OUT` (UNIQUE) — COD_PUBLISHER, COD_PAIS, COD_MEDIO

### PPT_PUBLISHER_PLATAFORMA_IN

Plataformas (DSP) que componen ese publisher. Si no hay, se cogen todos

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PUBLISHER | NUMBER |  | PK | → PPT_PUBLISHER |  |
| COD_PLATAFORMA | NUMBER |  | PK |  |  |

**Índices:**

- `PK_PPT_PUBLISHER_PLATAFORMA_IN` (UNIQUE) — COD_PUBLISHER, COD_PLATAFORMA

### PPT_PUBLISHER_PLATAFORMA_OUT

Plataformas (DSP) que se excluyen de un publisher. Si no hay registros, no se excluye nada

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PUBLISHER | NUMBER |  | PK | → PPT_PUBLISHER |  |
| COD_PLATAFORMA | NUMBER |  | PK |  |  |

**Índices:**

- `PK_PPT_PUBLISHER_PLATFORMA_OUT` (UNIQUE) — COD_PUBLISHER, COD_PLATAFORMA

### PPT_PURE_PLAYER

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PURE_PLAYER | NUMBER |  | PK |  |  |
| DES_PURE_PLAYER | VARCHAR2(100) |  |  |  |  |
| PURE | VARCHAR2(1) |  |  |  |  |
| TRANSFER | VARCHAR2(1) |  |  |  |  |
| F_ALTA | DATE |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |

**Índices:**

- `PK_PPT_PURE_PLAYER` (UNIQUE) — COD_PURE_PLAYER
- `UQ_PPT_PURE_PLAYER` (UNIQUE) — PURE

### PPT_PURE_PLAYER_DISCIPLINA

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PURE_PLAYER_DISCIPLINA | NUMBER |  | PK |  |  |
| COD_PURE_PLAYER | NUMBER |  |  | → PPT_PURE_PLAYER |  |
| COD_DISCIPLINA | NUMBER |  |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |

**Índices:**

- `PK_PPT_PURE_PLAYER_DISCIPLINA` (UNIQUE) — COD_PURE_PLAYER_DISCIPLINA
- `UQ_PPT_PURE_PLAYER_DISCIPLINA` (UNIQUE) — COD_PURE_PLAYER, COD_DISCIPLINA

### PPT_PURE_PLAYER_DIVERSIFIED

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PURE_PLAYER_DIVERSIFIED | NUMBER |  | PK |  |  |
| COD_PURE_PLAYER | NUMBER |  |  | → PPT_PURE_PLAYER |  |
| COD_DIVERSIFIED | NUMBER |  |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |

**Índices:**

- `PK_PPT_PURE_PLAYER_DIVERSIFIED` (UNIQUE) — COD_PURE_PLAYER_DIVERSIFIED
- `UQ_PPT_PURE_PLAYER_DIVERSIFIED` (UNIQUE) — COD_PURE_PLAYER, COD_DIVERSIFIED

### PPT_PURE_PLAYER_OBJETIVO

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PURE_PLAYER_OBJETIVO | NUMBER |  | PK |  |  |
| COD_PURE_PLAYER | NUMBER |  |  | → PPT_PURE_PLAYER |  |
| COD_OBJETIVO | NUMBER |  |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |

**Índices:**

- `PK_PPT_PURE_PLAYER_OBJETIVO` (UNIQUE) — COD_PURE_PLAYER_OBJETIVO
- `UQ_PPT_PURE_PLAYER_OBJETIVO` (UNIQUE) — COD_PURE_PLAYER, COD_OBJETIVO

### PPT_PURE_PLAYER_TIPO_COMPRA

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PURE_PLAYER_TIPO_COMPRA | NUMBER |  | PK |  |  |
| COD_PURE_PLAYER | NUMBER |  |  | → PPT_PURE_PLAYER |  |
| COD_TIPO_COMPRA | NUMBER |  |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |

**Índices:**

- `PK_PPT_PURE_PLAYER_TIPO_COMPRA` (UNIQUE) — COD_PURE_PLAYER_TIPO_COMPRA
- `UQ_PPT_PURE_PLAYER_TIPO_COMPRA` (UNIQUE) — COD_PURE_PLAYER, COD_TIPO_COMPRA

### PPT_SOBREPRIMAS_MEDIO

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_SOBREPRIMA_MEDIO | NUMBER |  | PK |  |  |
| COD_VERSION | NUMBER |  |  | → PPT_VERSIONES |  |
| COD_CONCEPTO_SOBREPRIMA | NUMBER |  |  | → PPT_CONCEPTOS_SOBREPRIMA |  |
| COD_NETWORK | NUMBER |  |  |  |  |
| COD_PAIS | NUMBER |  |  |  |  |
| COD_MEDIO | NUMBER |  |  |  |  |
| COD_EDITORIAL_COMERCIAL | NUMBER |  |  |  |  |
| PCT_SOBREPRIMA | NUMBER(5,2) |  |  |  | Acutualmente solo si ind_real = 0 |
| F_ALTA | DATE |  |  |  |  |
| F_MODIFICACION | DATE |  |  |  |  |
| COD_USUARIO_ALTA | NUMBER |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |

**Índices:**

- `IND_PPT_SOBREPRIMAS_CONCP` — COD_CONCEPTO_SOBREPRIMA
- `PK_PPT_SOBREPRIMAS` (UNIQUE) — COD_SOBREPRIMA_MEDIO
- `UQ_PPT_SOBREPRIMA` (UNIQUE) — COD_VERSION, COD_CONCEPTO_SOBREPRIMA, COD_NETWORK, COD_PAIS, COD_MEDIO, COD_EDITORIAL_COMERCIAL

### PPT_TIPOS_VERSIONES

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_TIPO_VERSION | NUMBER |  | PK |  |  |
| DES_TIPO_VERSION | VARCHAR2(50) |  |  |  |  |

**Índices:**

- `PK_PPT_TIPO_VERSIONES_PRESUPUE` (UNIQUE) — COD_TIPO_VERSION

### PPT_VERSIONES

Tiene asociado un Trigger para forzar los bits de estado cuando es REAL. 

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_VERSION | NUMBER |  | PK |  |  |
| COD_PAIS | NUMBER |  |  |  |  |
| ANIO | NUMBER |  |  |  |  |
| DES_VERSION | VARCHAR2(50) |  |  |  |  |
| MES_VERSION_RRHH | NUMBER |  |  |  | Mes que se utilizará en el calculo de los FTEs  al mes-año indicado |
| IND_ESTADO_VERSION | NUMBER |  |  |  | VERSIONES ACTUALES: 
 El 1er bit indica si esta cerrada (1)  o abierta (0). Tened en cuenta que aunque este cerrada, los controllers pueden modificarla. El 2º indica si es Version consejo ademas se distingue por la fecha de consejo (cuando se cierra). El 3º indica si es version para Reporting Magnitude. (4) El 4º indica que la version se esta GENERANDO (8). El 5º es version Objetivo es el (16) El 6º es version RRHH es el (32) (Mar-19) El 7º es version Abierta es el (64) El 8º es version Pruebas (128) El 9º es version Landing 256 - pkg_rbx_generico El 10º es version PILOT 512 El 11º es version publicada para el miner Contribution Margin 1024 El 12º es version dashboard 2048 El 13º es version TimeSheet (2021) 4096 |
| ORDEN | NUMBER |  |  |  |  |
| COD_TIPO_VERSION | NUMBER |  |  | → PPT_TIPOS_VERSIONES | Tipo versiones, ejemplo: 1: Versión de Usuario 2: Versión Mensual 3: Versión de Backup |
| MES_BLOQUEO | NUMBER(2,0) |  |  |  | Mes hasta el que se considera cerrado el mes, bloqueando el los meses anteriores y teniendo en cuanta hasta ese mes también solo los valores absolutos, no los variables |
| ANIO_VERSION_RRHH | NUMBER(4,0) |  |  |  | Año que se utilizará en el calculo de los FTEs  al año indicado, independientemente del año de la versión |

**Índices:**

- `IDX_PPT_VERSIONES_BITAND` — SYS_NC00009$, COD_VERSION
- `IND_PPT_VERSIONES_TIP_VER` — COD_TIPO_VERSION
- `PK_PPT_VERSIONES` (UNIQUE) — COD_VERSION
- `UQ_PPT_VER_ANIO_COD_TIPO` (UNIQUE) — ANIO, COD_VERSION, COD_TIPO_VERSION
- `UQ_PPT_VER_COD_TIPO` (UNIQUE) — COD_VERSION, COD_TIPO_VERSION
- `UQ_PPT_VERSIONES` (UNIQUE) — COD_PAIS, ANIO, DES_VERSION
- `UQ_PPT_VERSIONES_ORDEN` (UNIQUE) — COD_PAIS, ANIO, ORDEN

### PY_CATEGORIA

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CATEGORIA | NUMBER |  | PK |  |  |
| DES_CATEGORIA | VARCHAR2(50) |  |  |  |  |

**Índices:**

- `PK_PY_CATEGORIA` (UNIQUE) — COD_CATEGORIA

### PY_CENTRO_TRABAJO

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CENTRO_TRABAJO | NUMBER |  | PK |  |  |
| DES_CENTRO_TRABAJO | VARCHAR2(50) |  |  |  |  |

**Índices:**

- `PK_PY_CENTRO_TRABAJO` (UNIQUE) — COD_CENTRO_TRABAJO

### PY_CONCEPTO_BAJA

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CONCEPTO_BAJA | NUMBER |  | PK |  |  |
| DES_CONCEPTO_BAJA | VARCHAR2(50) |  |  |  |  |

**Índices:**

- `PK_PY_CONCEPTO_BAJA` (UNIQUE) — COD_CONCEPTO_BAJA

### PY_CONCEPTO_COMPENSACION

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CONCEPTO_COMPENSACION | NUMBER |  | PK |  |  |
| DES_CONCEPTO_COMPENSACION | VARCHAR2(50) |  |  |  |  |
| IND_ESTADO_CONCEPTO | NUMBER |  |  |  | Indicadores de Estado (bitand):
1: Seguridad Social
2: No presupuestos |

**Índices:**

- `PK_PY_CONCEPTO_COMPENSACION` (UNIQUE) — COD_CONCEPTO_COMPENSACION

### PY_CONCEPTO_NATURALEZA

Tabla que contienen todos los posibles motivos que tiene una naturaleza

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CONCEPTO_COMPENSACION | NUMBER |  | PK | → PY_CONCEPTO_COMPENSACION |  |
| COD_NATURALEZA | NUMBER |  | PK | → PY_NATURALEZA |  |

**Índices:**

- `PK_PY_CONCEPTO_NATURALEZA` (UNIQUE) — COD_CONCEPTO_COMPENSACION, COD_NATURALEZA

### PY_CONTROL_CIERRE

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| ANIO | NUMBER(4,0) |  | PK |  |  |
| COD_PERIODO | NUMBER(2,0) |  | PK |  |  |

**Índices:**

- `PK_PY_CONTROL_CIERRE` (UNIQUE) — ANIO, COD_PERIODO

### PY_DEPARTAMENTO

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_DEPARTAMENTO | NUMBER |  | PK |  |  |
| DES_DEPARTAMENTO | VARCHAR2(100) |  |  |  |  |
| F_ALTA | DATE |  |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |
| COD_USUARIO_ALTA | NUMBER |  |  |  |  |
| COD_USUARIO_BAJA | NUMBER | ✓ |  |  |  |
| IND_MEDIA_ON | NUMBER(1,0) |  |  |  | Indica si el departamento es Media On (1) o Media off (0) o Ambos (2). Es el que se pondrá por defecto cuando se seleccione en imputaciones. |
| TIPO_IMPUTACION | VARCHAR2(1) |  |  |  | Nos indica si es directo (D) o trasversal (indirecto) (I) |
| COD_TIPO_NETWORK | NUMBER | ✓ |  |  | Al tipo de network al que pertenece. NULL o no se le ha puesto o pertenece a todas. (A TABLA ESTÁ EN PRESUPUESTOS_NEW |
| NAME_CC_WD | VARCHAR2(200) | ✓ |  |  | Centro Coste de WORKDAY.  |
| COD_GRUPO_DEPARTAMENTO | NUMBER | ✓ |  | → PY_GRUPO_DEPARTAMENTO | De momento solo se utiliza para el dashboard (sep 2022) |

**Índices:**

- `PK_PY_DEPARTAMENTO` (UNIQUE) — COD_DEPARTAMENTO

### PY_DEPARTAMENTO_ESPECIALIDAD

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_DEPARTAMENTO | NUMBER |  | PK | → PY_DEPARTAMENTO |  |
| COD_ESPECIALIDAD | NUMBER |  | PK | → AN_ESPECIALIDAD |  |

**Índices:**

- `PK_PY_DEPARTAMENTO_ESPECIALIDA` (UNIQUE) — COD_DEPARTAMENTO, COD_ESPECIALIDAD

### PY_DEPARTAMENTO_VERTICAL

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_DEPARTAMENTO | NUMBER |  | PK | → PY_DEPARTAMENTO |  |
| COD_VERTICAL | NUMBER |  | PK | → AN_VERTICAL |  |

**Índices:**

- `PK_PY_DEPARTAMENTO_VERTICAL` (UNIQUE) — COD_DEPARTAMENTO, COD_VERTICAL

### PY_EMPLEADO

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_EMPLEADO | NUMBER |  | PK |  |  |
| NOMBRE | VARCHAR2(50) |  |  |  |  |
| APELLIDO1 | VARCHAR2(50) |  |  |  |  |
| APELLIDO2 | VARCHAR2(50) | ✓ |  |  |  |
| ID_CDC | NUMBER |  |  |  | ID_CDC actual  |
| COD_NATURALEZA | NUMBER |  |  | → PY_NATURALEZA | Código de naturaleza actual |
| COD_CENTRO_TRABAJO | NUMBER |  |  | → PY_CENTRO_TRABAJO |  |
| COD_CATEGORIA | NUMBER |  |  | → PY_CATEGORIA |  |
| COD_TIPO_CONTRATO | VARCHAR2(7) |  |  | → PY_TIPO_CONTRATO |  |
| COD_MOTIVO | NUMBER |  |  | → PY_MOTIVO_CONCEPTO_BAJA |  |
| COD_CONCEPTO_BAJA | NUMBER |  |  | → PY_MOTIVO_CONCEPTO_BAJA |  |
| F_CAMBIO_SALARIAL | DATE |  |  |  |  |
| F_CAMBIO_REDUCCION_JORNADA | DATE | ✓ |  |  |  |
| F_VALOR_ESTRUCTURA_COSTES | DATE | ✓ |  |  |  |
| F_ALTA_CATEGORIA | DATE |  |  |  |  |
| F_ALTA_CONTRATO | DATE |  |  |  |  |
| F_ALTA | DATE |  |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |
| F_ANTIGUEDAD | DATE |  |  |  |  |
| PORCENTAJE_REDUCCION_JORNADA | VARCHAR2(64) |  |  |  | Encriptado |
| F_REGISTRO | DATE |  |  |  | Fecha en la que se inserta el registro/cambio |
| DNI | VARCHAR2(10) | ✓ |  |  |  |
| SEXO | VARCHAR2(1) | ✓ |  |  | H:Hombre;M:Mujer |
| F_NACIMIENTO | DATE | ✓ |  |  |  |
| COD_RESPONSABLE | NUMBER | ✓ |  | → PY_EMPLEADO | Empleado responsable, es un empelado que tiene que estár en esta tabla |
| IND_TS_VIRTUAL | NUMBER |  |  |  | Indica si en timeSheet puede elegir vitual para repartir (0 no, 1 Sí) |
| COD_TIPO_NETWORK | NUMBER | ✓ |  |  | Al tipo de network al que pertenece. NULL o no se le ha puesto o pertenece a todas. Inicialmente se utiliza si tiene distribución por grupo virtual a que tipo se distribuirá |
| NAME_CC_WD | VARCHAR2(200) | ✓ |  |  | Centro Coste de WORKDAY. Viene por enterface. |
| IND_TIMESHEET_EXTERNO | NUMBER |  |  |  | 1: Si es un empleado externo tendrá un dato en las cargas de ficheros con datos Timesheet de externos. 0: No  |
| DIGITAL | VARCHAR2(50) | ✓ |  |  |  |
| F_FINCONTRATO | DATE | ✓ |  |  |  |
| MANAGER_PAYROLL_ID | NUMBER | ✓ |  |  |  |
| EMAIL_GLOBALSERVS | VARCHAR2(300) | ✓ |  |  |  |

**Índices:**

- `IDX_PY_EMPLEADO_NIF` — DNI
- `IDX_PY_EMPLEADO_NIF_FALTA` — DNI, F_ALTA
- `IDX_PY_EMPLEADO_RESPONSABLE` — COD_RESPONSABLE
- `PK_PY_EMPLEADO` (UNIQUE) — COD_EMPLEADO

### PY_EMPLEADO_AUSENCIA

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_EMPLEADO | NUMBER |  |  |  |  |
| ANIO | NUMBER |  |  |  |  |
| MES | NUMBER |  |  |  |  |
| DIAS_BAJA | NUMBER |  |  |  |  |
| RATIO_BAJA_MES | NUMBER | ✓ |  |  |  |
| PROCESADO | NUMBER | ✓ |  |  |  |

### PY_EMPLEADO_CIERRE

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER |  | PK |  |  |
| ANIO | NUMBER |  | PK |  |  |
| COD_EMPLEADO | NUMBER |  | PK | → PY_EMPLEADO |  |
| COD_CONCEPTO_COMPENSACION | NUMBER |  | PK | → PY_CONCEPTO_COMPENSACION |  |
| MES | NUMBER |  | PK |  |  |
| VALOR | VARCHAR2(64) |  |  |  | Valor encriptado |
| F_REGISTRO | DATE |  |  |  | Fecha en la que se inserta/modifica registro |

**Índices:**

- `PK_PY_EMPLEADO_CIERRE` (UNIQUE) — COD_PAIS, ANIO, COD_EMPLEADO, COD_CONCEPTO_COMPENSACION, MES

### PY_EMPLEADO_DETALLE

Tabla que indica datos de un empleado con fecha alta y baja. Por ejemplo la compañía, preveedor si lo tiene, etc)

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_EMPLEADO_DETALLE | NUMBER |  | PK |  |  |
| COD_EMPLEADO | NUMBER |  |  | → PY_EMPLEADO |  |
| F_INICIO | DATE |  |  |  |  |
| F_FIN | DATE | ✓ |  |  |  |
| ID_CDC | NUMBER |  |  |  |  |
| COD_NATURALEZA | NUMBER |  |  | → PY_NATURALEZA | Por si un empleado cambia de naturaleza |
| COD_PROVEEDOR | NUMBER | ✓ |  |  |  |
| COSTE | NUMBER |  |  |  |  |

**Índices:**

- `PK_PY_EMPLEADO_DETALLE` (UNIQUE) — COD_EMPLEADO_DETALLE
- `UQ_PK_PY_EMPLEADO_DET` (UNIQUE) — COD_EMPLEADO, F_INICIO, F_FIN

### PY_EMPLEADO_HISTORICO

Tabla con los distintos valores que ha tenido el usuaro anteriormente a la situación actual

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_EMPLEADO | NUMBER |  |  |  |  |
| NOMBRE | VARCHAR2(50) |  |  |  |  |
| APELLIDO1 | VARCHAR2(50) |  |  |  |  |
| APELLIDO2 | VARCHAR2(50) | ✓ |  |  |  |
| ID_CDC | NUMBER |  |  |  | ID_CDC actual  |
| COD_NATURALEZA | NUMBER |  |  |  | Código de naturaleza actual |
| COD_CENTRO_TRABAJO | NUMBER |  |  |  |  |
| COD_CATEGORIA | NUMBER |  |  |  |  |
| COD_TIPO_CONTRATO | NUMBER |  |  |  |  |
| COD_MOTIVO | NUMBER |  |  |  |  |
| COD_CONCEPTO_BAJA | NUMBER |  |  |  |  |
| F_REGISTRO | DATE |  |  |  |  |
| F_CAMBIO_SALARIAL | DATE |  |  |  |  |
| F_CAMBIO_REDUCCION_JORNADA | DATE | ✓ |  |  |  |
| F_VALOR_ESTRUCTURA_COSTES | DATE | ✓ |  |  |  |
| F_ALTA_CATEGORIA | DATE |  |  |  |  |
| F_ALTA_CONTRATO | DATE |  |  |  |  |
| F_ALTA | DATE |  |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |
| F_ANTIGUEDAD | DATE |  |  |  |  |
| PORCENTAJE_REDUCCION_JORNADA | VARCHAR2(64) |  |  |  | Encriptado |
| F_HISTORICO | DATE |  |  |  | Fecha en que se creó el historico |
| NAME_CC_WD | VARCHAR2(200) | ✓ |  |  | Centro Coste de WORKDAY. Viene por enterface. |
| COD_TIPO_NETWORK | NUMBER | ✓ |  |  |  |
| DNI | VARCHAR2(10) | ✓ |  |  |  |
| SEXO | VARCHAR2(1) | ✓ |  |  |  |
| F_NACIMIENTO | DATE | ✓ |  |  |  |
| COD_RESPONSABLE | NUMBER | ✓ |  |  |  |
| IND_TS_VIRTUAL | VARCHAR2(50) | ✓ |  |  |  |
| DIGITAL | VARCHAR2(50) | ✓ |  |  |  |
| F_FINCONTRATO | DATE | ✓ |  |  |  |
| MANAGER_PAYROLL_ID | NUMBER | ✓ |  |  |  |
| EMAIL_GLOBALSERVS | VARCHAR2(300) | ✓ |  |  |  |

**Índices:**

- `IDX_EMP_HIST_COD_EMP` — COD_EMPLEADO
- `IDX_EMP_HIST_COD_EMP_FHIS` — COD_EMPLEADO, F_HISTORICO

### PY_EMPLEADO_PREVISION

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER |  | PK |  |  |
| ANIO | NUMBER |  | PK |  |  |
| COD_VERSION | NUMBER |  | PK |  |  |
| COD_EMPLEADO | NUMBER |  | PK | → PY_EMPLEADO |  |
| COD_CONCEPTO_COMPENSACION | NUMBER |  | PK | → PY_CONCEPTO_COMPENSACION |  |
| MES | NUMBER |  | PK |  |  |
| VALOR | VARCHAR2(64) |  |  |  | Valor encriptado |
| F_REGISTRO | DATE |  |  |  | Fecha en la que se inserta/modifica registro |

**Índices:**

- `IDX_EMP_PREV_ANIO_VER` — COD_PAIS, ANIO, COD_VERSION
- `PK_PY_EMPLEADO_PREV` (UNIQUE) — COD_PAIS, ANIO, COD_VERSION, COD_EMPLEADO, COD_CONCEPTO_COMPENSACION, MES

### PY_EMPLEADO_PREVISION_03052022

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER | ✓ |  |  |  |
| ANIO | NUMBER | ✓ |  |  |  |
| COD_VERSION | NUMBER | ✓ |  |  |  |
| COD_EMPLEADO | NUMBER | ✓ |  |  |  |
| COD_CONCEPTO_COMPENSACION | NUMBER | ✓ |  |  |  |
| MES | NUMBER | ✓ |  |  |  |
| VALOR | VARCHAR2(64) | ✓ |  |  | Valor encriptado |
| F_REGISTRO | DATE | ✓ |  |  | Fecha en la que se inserta/modifica registro |

### PY_EMPLEADO_TEORICO

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_EMPLEADO | NUMBER |  | PK | → PY_EMPLEADO |  |
| COD_CONCEPTO_COMPENSACION | NUMBER |  | PK | → PY_CONCEPTO_COMPENSACION |  |
| VALOR | VARCHAR2(64) |  |  |  | Valor encriptado |
| F_REGISTRO | DATE |  |  |  | Fecha en la que se inserta/modifica registro |

**Índices:**

- `PK_PY_EMPLEADO_TEORICO` (UNIQUE) — COD_EMPLEADO, COD_CONCEPTO_COMPENSACION

### PY_EMPLEADO_TEORICO_HISTORICO

Contiene los distintos valores que ha tenido en empleado antes de la situación actual

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_EMPLEADO | NUMBER |  |  |  |  |
| COD_CONCEPTO_COMPENSACION | NUMBER |  |  |  |  |
| VALOR | VARCHAR2(64) |  |  |  | Valor encriptado |
| F_REGISTRO | DATE |  |  |  |  |
| F_HISTORICO | DATE | ✓ |  |  |  |

### PY_FLAG_CIERRE

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| ACTIVA_CIERRE | NUMBER | ✓ |  |  |  |
| TIPO_CIERRE | NUMBER | ✓ |  |  |  |

### PY_GRUPO_DEPARTAMENTO

De momento solo se utiliza para el dashboard (sep 2022)

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_GRUPO_DEPARTAMENTO | NUMBER |  | PK |  |  |
| DES_GRUPO_DEPARTAMENTO | VARCHAR2(50) |  |  |  |  |

**Índices:**

- `PK_PY_GRUPO_DEPARTAMENTO` (UNIQUE) — COD_GRUPO_DEPARTAMENTO

### PY_GRUPO_NATURALEZA

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_GRUPO_NATURALEZA | NUMBER |  | PK |  |  |
| DES_GRUPO_NATURALEZA | VARCHAR2(50) |  |  |  |  |

**Índices:**

- `PK_PY_GRUPO_NATURALEZA` (UNIQUE) — COD_GRUPO_NATURALEZA

### PY_GRUPO_PARKING

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_GRUPO_PARKING | NUMBER |  | PK |  |  |
| DES_GRUPO_PARKING | VARCHAR2(50) |  |  |  |  |
| F_ALTA | DATE |  |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |
| COD_USUARIO_CREACION | NUMBER |  |  |  | usuario que lo ha creado |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |
| F_CREACION | DATE |  |  |  |  |
| F_MODIFICACION | DATE |  |  |  |  |
| IND_ESTADO_GRUPO_PARKING | NUMBER |  |  |  | Bitand: 1: Visita 2: Difusión 4: Incluida Alquiler Edificio |

**Índices:**

- `PK_PY_GRUPO_PARKING` (UNIQUE) — COD_GRUPO_PARKING

### PY_LOG_CONFIRMACION

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_USUARIO | NUMBER |  | PK |  |  |
| COD_EMPLEADO | NUMBER |  | PK | → PY_EMPLEADO |  |
| F_CONFIRMACION | DATE |  |  |  |  |
| AUX | NUMBER | ✓ |  |  | codigo de empleado en la asociacion de vacantes |

**Índices:**

- `PK_PY_BAJA_CONFIRMACION` (UNIQUE) — COD_USUARIO, COD_EMPLEADO

### PY_MATRICULA

Matrículas de coches 

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| MATRICULA | VARCHAR2(10) |  | PK |  |  |
| MARCA | VARCHAR2(50) |  |  |  |  |
| MODELO | VARCHAR2(50) |  |  |  |  |
| F_ALTA | DATE |  |  |  |  |
| F_BAJA | DATE |  |  |  |  |

**Índices:**

- `PK_PY_MATRICULA` (UNIQUE) — MATRICULA

### PY_MATRICULA_DETALLE

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| MATRICULA | VARCHAR2(10) |  | PK | → PY_MATRICULA |  |
| COD_EMPLEADO | NUMBER |  | PK | → PY_EMPLEADO | Empleado que tiene el coche entre la fecha inicio y fecha fin |
| F_INICIO | DATE |  | PK |  |  |
| F_FIN | DATE |  | PK |  |  |
| OBSERVACIONES | VARCHAR2(100) | ✓ |  |  |  |

**Índices:**

- `PK_PY_MATRICULA_DETALLE` (UNIQUE) — MATRICULA, COD_EMPLEADO, F_INICIO, F_FIN

### PY_MOTIVO_CONCEPTO_BAJA

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_MOTIVO | NUMBER |  | PK |  |  |
| COD_CONCEPTO_BAJA | NUMBER |  | PK | → PY_CONCEPTO_BAJA |  |
| DES_MOTIVO_CONCEPTO_BAJA | VARCHAR2(50) |  |  |  |  |
| IND_HC | VARCHAR2(1) | ✓ |  |  |  |

**Índices:**

- `PK_PY_MOTIVO_CONCEPTO_BAJA` (UNIQUE) — COD_MOTIVO, COD_CONCEPTO_BAJA

### PY_NATURALEZA

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_NATURALEZA | NUMBER |  | PK |  |  |
| DES_NATURALEZA | VARCHAR2(50) |  |  |  |  |
| IND_ESTADO_NATURALEZA | NUMBER |  |  |  | 1: Se utiliza en la pantalla de Mantenimiento de Ajustes, Vacantes, ... 2: Se muestra check Coste en la pantalla compensación.  4: Se marca Coste por defecto en la pantalla compensación (en caso visible).  8: Se marca Gasto por defecto en la pantalla compensación (en caso visible).   |
| COD_GRUPO_NATURALEZA | NUMBER |  |  |  |  |

**Índices:**

- `PK_PY_NATURALEZA` (UNIQUE) — COD_NATURALEZA

### PY_PARKING

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PARKING | NUMBER |  | PK |  |  |
| COD_GRUPO_PARKING | NUMBER |  |  | → PY_GRUPO_PARKING |  |
| COD_UBICACION | NUMBER |  |  | → PY_UBICACION |  |
| PLAZA | VARCHAR2(10) |  |  |  |  |
| F_ALTA | DATE |  |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |
| OBSERVACIONES | CLOB | ✓ |  |  |  |
| COD_USUARIO_CREACION | NUMBER |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |
| F_MODIFICACION | DATE |  |  |  |  |
| F_CREACION | DATE |  |  |  |  |
| INTERCIA | VARCHAR2(3) | ✓ |  |  | Compañía INTERCIA a la que se asigna. Se alimentará cuando no tiene empleados asignados por ejemplo empresas que no tienen empleados en la aplicación |

**Índices:**

- `PK_PY_PARKING` (UNIQUE) — COD_PARKING
- `UQ_PARKING` (UNIQUE) — COD_UBICACION, PLAZA, F_BAJA

### PY_PARKING_EMPLEADO

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PARKING | NUMBER |  | PK | → PY_PARKING |  |
| COD_EMPLEADO | NUMBER |  | PK | → PY_EMPLEADO |  |
| OBSERVACIONES | CLOB | ✓ |  |  |  |
| F_ALTA | DATE |  |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |

**Índices:**

- `PK_PY_PARKING_EMPLEADO` (UNIQUE) — COD_PARKING, COD_EMPLEADO

### PY_PURE_PLAYER

Realmente es "equipo". Cuando se decidió cambiar el coste de cambiar el nombre de bbdd es alto. Tabla originalmente creada para los pure players. Posiblemente cambie el significado en la aplicación al añadirse más valores que no son pure players

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PURE_PLAYER | NUMBER |  | PK |  |  |
| DES_PURE_PLAYER | VARCHAR2(50) |  |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |

**Índices:**

- `PK_PY_PURE_PLAYER` (UNIQUE) — COD_PURE_PLAYER

### PY_TIPO_CONTRATO

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_TIPO_CONTRATO | VARCHAR2(20) |  | PK |  |  |
| DES_TIPO_CONTRATO | VARCHAR2(50) |  |  |  |  |

**Índices:**

- `PK_PY_TIPO_CONTRATO` (UNIQUE) — COD_TIPO_CONTRATO

### PY_TMP_BAJA_EMPLEADO

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_EMPLEADO | NUMBER | ✓ |  |  |  |
| DNI | VARCHAR2(15) | ✓ |  |  |  |
| COD_EMPRESA | VARCHAR2(2) | ✓ |  |  |  |
| NAME_ORG | VARCHAR2(100) | ✓ |  |  |  |
| F_INICIO | DATE | ✓ |  |  |  |
| F_FIN | DATE | ✓ |  |  |  |
| COD_MOTIVO_FIN | VARCHAR2(4) | ✓ |  |  |  |
| NOMBRE | VARCHAR2(50) | ✓ |  |  |  |
| APELLIDO1 | VARCHAR2(50) | ✓ |  |  |  |
| APELLIDO2 | VARCHAR2(50) | ✓ |  |  |  |
| DES_MOTIVO_FIN | VARCHAR2(100) | ✓ |  |  |  |

### PY_UBICACION

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_UBICACION | NUMBER |  | PK |  |  |
| DES_UBICACION | VARCHAR2(50) |  |  |  |  |
| DIRECCION | VARCHAR2(350) | ✓ |  |  |  |
| IND_ESTADO_UBICACION | NUMBER |  |  |  | Bitand:  1: Oficina 2: Parking 4: Inactivo	 |

**Índices:**

- `PK_PY_UBICACION` (UNIQUE) — COD_UBICACION

### QUEST_SL_TEMP_EXPLAIN1

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| STATEMENT_ID | VARCHAR2(30) | ✓ |  |  |  |
| PLAN_ID | NUMBER | ✓ |  |  |  |
| TIMESTAMP | DATE | ✓ |  |  |  |
| REMARKS | VARCHAR2(4000) | ✓ |  |  |  |
| OPERATION | VARCHAR2(30) | ✓ |  |  |  |
| OPTIONS | VARCHAR2(255) | ✓ |  |  |  |
| OBJECT_NODE | VARCHAR2(128) | ✓ |  |  |  |
| OBJECT_OWNER | VARCHAR2(30) | ✓ |  |  |  |
| OBJECT_NAME | VARCHAR2(30) | ✓ |  |  |  |
| OBJECT_ALIAS | VARCHAR2(65) | ✓ |  |  |  |
| OBJECT_INSTANCE | NUMBER | ✓ |  |  |  |
| OBJECT_TYPE | VARCHAR2(30) | ✓ |  |  |  |
| OPTIMIZER | VARCHAR2(255) | ✓ |  |  |  |
| SEARCH_COLUMNS | NUMBER | ✓ |  |  |  |
| ID | NUMBER | ✓ |  |  |  |
| PARENT_ID | NUMBER | ✓ |  |  |  |
| DEPTH | NUMBER | ✓ |  |  |  |
| POSITION | NUMBER | ✓ |  |  |  |
| COST | NUMBER | ✓ |  |  |  |
| CARDINALITY | NUMBER | ✓ |  |  |  |
| BYTES | NUMBER | ✓ |  |  |  |
| OTHER_TAG | VARCHAR2(255) | ✓ |  |  |  |
| PARTITION_START | VARCHAR2(255) | ✓ |  |  |  |
| PARTITION_STOP | VARCHAR2(255) | ✓ |  |  |  |
| PARTITION_ID | NUMBER | ✓ |  |  |  |
| OTHER | LONG | ✓ |  |  |  |
| OTHER_XML | CLOB | ✓ |  |  |  |
| DISTRIBUTION | VARCHAR2(30) | ✓ |  |  |  |
| CPU_COST | NUMBER | ✓ |  |  |  |
| IO_COST | NUMBER | ✓ |  |  |  |
| TEMP_SPACE | NUMBER | ✓ |  |  |  |
| ACCESS_PREDICATES | VARCHAR2(4000) | ✓ |  |  |  |
| FILTER_PREDICATES | VARCHAR2(4000) | ✓ |  |  |  |
| PROJECTION | VARCHAR2(4000) | ✓ |  |  |  |
| TIME | NUMBER(20,2) | ✓ |  |  |  |
| QBLOCK_NAME | VARCHAR2(30) | ✓ |  |  |  |

### TEMP_SEGURIDAD_CODIGOS

Tabla temporal que guarda los códigos, por ejemplo de GRUPO, cuando en un JSON viene un -1 en el valor de la etiqueta de un código. Cuando viene -1 hay que cambiar ese -1 por todos los códigos a los que el usuario tiene acceso. Para ello, al aplicar seguridad en PKG_VALIDACION_JSON, cambia el -1 por un -2 y guarda todos los códigos en la tabla temporal. Luego los programas, al hacer las select contra el JSON, si el valor es -2 mete un IN  (select el código de la tabla temporal)

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| TABLA | VARCHAR2(50) | ✓ |  |  |  |
| COLUMNA | VARCHAR2(50) | ✓ |  |  |  |
| CODIGO | NUMBER | ✓ |  |  |  |

### TLF_OPERADOR

Operadores de telefonía

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_OPERADOR | NUMBER |  | PK |  |  |
| DES_OPERADOR | VARCHAR2(100) |  |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |

**Índices:**

- `PK_TLF_OPERADOR_TELEFONIA` (UNIQUE) — COD_OPERADOR

### TLF_TELEFONO

Tabla con los teléfonos que luego se asignarán a los usuarios

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_TELEFONO | NUMBER |  | PK |  |  |
| COD_OPERADOR | NUMBER |  |  | → TLF_OPERADOR |  |
| NUM_TELEFONO | VARCHAR2(50) |  |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |
| OBSERVACIONES | VARCHAR2(150) | ✓ |  |  |  |

**Índices:**

- `PK_TLF_TELEFONO` (UNIQUE) — COD_TELEFONO
- `UQ_TLF_TELEFONO_NUM_TELE_OPER` (UNIQUE) — COD_OPERADOR, NUM_TELEFONO

### TLF_TELEFONO_EMPLEADO

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_TELEFONO | NUMBER |  |  | → TLF_TELEFONO |  |
| COD_EMPLEADO | NUMBER |  |  | → PY_EMPLEADO |  |
| F_ALTA | DATE |  |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |

**Índices:**

- `UQ_TLF_TELEFONO__COD_TELEFONO` (UNIQUE) — COD_TELEFONO, F_ALTA, F_BAJA

### TMP_CIERRE

Tabla temporal para cargar los ficheros de nómina por empleado para el cuadre

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| STATUS | VARCHAR2(3) | ✓ |  |  |  |
| LEDGER_ID | VARCHAR2(64) | ✓ |  |  |  |
| USER_JE_SOURCE_NAME | VARCHAR2(64) | ✓ |  |  |  |
| USER_JE_CATEGORY_NAME | VARCHAR2(64) | ✓ |  |  |  |
| ACCOUNTING_DATE | VARCHAR2(8) | ✓ |  |  |  |
| CURRENCY_CODE | VARCHAR2(3) | ✓ |  |  |  |
| CREATED_BY | VARCHAR2(3) | ✓ |  |  |  |
| ACTUAL_FLAG | VARCHAR2(3) | ✓ |  |  |  |
| PERIOD_NAME | VARCHAR2(6) | ✓ |  |  |  |
| DEBE | NUMBER | ✓ |  |  | dividido por 100 |
| HABER | NUMBER | ✓ |  |  | dividido por 100 |
| CIA_OF | NUMBER | ✓ |  |  |  |
| CUENTA | NUMBER | ✓ |  |  |  |
| CDC | VARCHAR2(4) | ✓ |  |  |  |
| CTA | VARCHAR2(7) | ✓ |  |  |  |
| IMPUTACION | VARCHAR2(1) | ✓ |  |  |  |
| SEGMENT6 | VARCHAR2(3) | ✓ |  |  |  |
| SEGMENT7 | VARCHAR2(2) | ✓ |  |  |  |
| COD_EMPLEADO | NUMBER | ✓ |  |  |  |

### TMP_CIERRE_AGREGADO

Tabla temporal para cargar los ficheros de nómina agregados para el cuadre (los usamos para cuadrar retros de 2018 en Septiembre)

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| STATUS | VARCHAR2(3) | ✓ |  |  |  |
| LEDGER_ID | VARCHAR2(64) | ✓ |  |  |  |
| USER_JE_SOURCE_NAME | VARCHAR2(64) | ✓ |  |  |  |
| USER_JE_CATEGORY_NAME | VARCHAR2(64) | ✓ |  |  |  |
| ACCOUNTING_DATE | VARCHAR2(8) | ✓ |  |  |  |
| CURRENCY_CODE | VARCHAR2(3) | ✓ |  |  |  |
| CREATED_BY | VARCHAR2(3) | ✓ |  |  |  |
| ACTUAL_FLAG | VARCHAR2(3) | ✓ |  |  |  |
| PERIOD_NAME | VARCHAR2(6) | ✓ |  |  |  |
| DEBE | NUMBER | ✓ |  |  |  |
| HABER | NUMBER | ✓ |  |  |  |
| CIA_OF | NUMBER | ✓ |  |  |  |
| CUENTA | NUMBER | ✓ |  |  |  |
| CDC | VARCHAR2(4) | ✓ |  |  |  |
| CTA | VARCHAR2(7) | ✓ |  |  |  |
| IMPUTACION | VARCHAR2(1) | ✓ |  |  |  |
| SEGMENT6 | VARCHAR2(3) | ✓ |  |  |  |
| SEGMENT7 | VARCHAR2(2) | ✓ |  |  |  |

### TMP_DIST_IMPUTACION

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_EMPLEADO | NUMBER | ✓ |  |  |  |
| PCT_DISTRIBUCION | NUMBER | ✓ |  |  |  |
| COD_EMPLEADO_DIST_IMPUTACION | NUMBER | ✓ |  |  |  |
| COD_COMPANIA | NUMBER | ✓ |  |  |  |
| IND_ESTADO_VIRTUAL | NUMBER | ✓ |  |  |  |
| IND_ESTADO_VIRTUAL_REAL | NUMBER | ✓ |  |  |  |
| COD_VIRTUAL | NUMBER | ✓ |  |  |  |
| COD_VIRTUAL_REAL | NUMBER | ✓ |  |  |  |
| IND_VIRTUAL_TODOS | NUMBER | ✓ |  |  |  |
| COD_VERTICAL | NUMBER | ✓ |  |  |  |
| COD_VERTICAL_REAL | NUMBER | ✓ |  |  |  |
| IND_ESTADO_VERTICAL | NUMBER | ✓ |  |  |  |
| IND_ESTADO_VERTICAL_REAL | NUMBER | ✓ |  |  |  |
| IND_VERTICAL_TODOS | NUMBER | ✓ |  |  |  |
| COD_DEPARTAMENTO | NUMBER | ✓ |  |  |  |
| COD_ESPECIALIDAD | NUMBER | ✓ |  |  |  |
| COD_ESPECIALIDAD_REAL | NUMBER | ✓ |  |  |  |
| COD_PURE_PLAYER | NUMBER | ✓ |  |  |  |
| IND_ESTADO_ESPECIALIDAD | NUMBER | ✓ |  |  |  |
| IND_ESTADO_ESPECIALIDAD_REAL | NUMBER | ✓ |  |  |  |
| IND_ESPECIALIDAD_TODOS | NUMBER | ✓ |  |  |  |
| COD_OFICINA | NUMBER | ✓ |  |  |  |
| COD_OFICINA_REAL | NUMBER | ✓ |  |  |  |
| IND_ESTADO_OFICINA | NUMBER | ✓ |  |  |  |
| IND_ESTADO_OFICINA_REAL | NUMBER | ✓ |  |  |  |
| IND_OFICINA_TODOS | NUMBER | ✓ |  |  |  |

**Índices:**

- `IDX_TMP_DIST_IMPUTACI_COD_DIST` — COD_EMPLEADO_DIST_IMPUTACION
- `IDX_TMP_DIST_IMPUTACION` — COD_VERTICAL, COD_ESPECIALIDAD, COD_VIRTUAL

### TMP_EMPLEADO_AJUSTE

Tabla para parametrizar ajustes por empleado, mes y concepto

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_EMPLEADO | NUMBER | ✓ |  |  |  |
| VALOR | NUMBER | ✓ |  |  |  |
| MES | NUMBER | ✓ |  |  |  |
| PROCESAR | NUMBER | ✓ |  |  |  |
| FECHA_MODIFICACION | DATE | ✓ |  |  |  |
| COD_CONCEPTO | NUMBER | ✓ |  |  |  |
| ANIO | NUMBER | ✓ |  |  |  |
| OBSERVACIONES | VARCHAR2(200) | ✓ |  |  |  |

### TMP_EMPLEADO_AUSENCIA

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| EMP | NUMBER | ✓ |  |  |  |
| COD_CENTRO | NUMBER | ✓ |  |  |  |
| CENTRO | VARCHAR2(80) | ✓ |  |  |  |
| COD_EMPLEADO | NUMBER |  |  |  |  |
| NOMBRE | VARCHAR2(50) | ✓ |  |  |  |
| APELLIDOS | VARCHAR2(50) | ✓ |  |  |  |
| COD_MOTIVO | NUMBER |  |  |  |  |
| AUSENCIA | VARCHAR2(50) | ✓ |  |  |  |
| FECHA_ACCIDENTE | VARCHAR2(50) | ✓ |  |  |  |
| F_INICIO | DATE |  |  |  |  |
| F_FIN | DATE | ✓ |  |  |  |
| DIAS | NUMBER | ✓ |  |  |  |
| ABIERTA | VARCHAR2(2) | ✓ |  |  |  |
| ACTIVO | VARCHAR2(2) | ✓ |  |  |  |
| F_ANTIGUEDAD | DATE | ✓ |  |  |  |
| COD_TIPO_CONTRATO | VARCHAR2(20) | ✓ |  |  |  |
| TIPO_CONTRARO | VARCHAR2(100) | ✓ |  |  |  |
| PCT | NUMBER | ✓ |  |  |  |
| F_NAC | DATE | ✓ |  |  |  |
| COD_DEL | NUMBER | ✓ |  |  |  |
| DELEGACION | VARCHAR2(100) | ✓ |  |  |  |
| COD_PUESTO | VARCHAR2(50) | ✓ |  |  |  |
| PUESTO | VARCHAR2(50) | ✓ |  |  |  |

### TMP_PPT_ITRACKER

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_VERSION | NUMBER | ✓ |  |  |  |
| COD_NETWORK | NUMBER | ✓ |  |  |  |
| ANIO | NUMBER | ✓ |  |  |  |
| COD_PAIS | NUMBER | ✓ |  |  |  |
| COD_MEDIO | NUMBER | ✓ |  |  |  |
| COD_DISCIPLINA | NUMBER | ✓ |  |  |  |
| COD_OBJETIVO | NUMBER | ✓ |  |  |  |
| COD_TIPO_COMPRA | NUMBER | ✓ |  |  |  |
| COUNTRY | VARCHAR2(50) | ✓ |  |  |  |
| YEAR | NUMBER(4,0) | ✓ |  |  |  |
| MONTH | NUMBER(2,0) | ✓ |  |  |  |
| ENTITY_CODE | VARCHAR2(100) | ✓ |  |  |  |
| CAMPAIGN_TYPE | VARCHAR2(100) | ✓ |  |  |  |
| CURRENCY | VARCHAR2(10) | ✓ |  |  |  |
| AMOUNT_MONTH | NUMBER | ✓ |  |  |  |
| AMOUNT_YEAR | NUMBER | ✓ |  |  |  |
| COMMENTS | VARCHAR2(500) | ✓ |  |  |  |

### TMP_PPT_MAGNITUDE

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| D_AC | VARCHAR2(100) | ✓ |  |  |  |
| D_ACTY | VARCHAR2(100) | ✓ |  |  |  |
| D_AREA | VARCHAR2(100) | ✓ |  |  |  |
| D_AU | VARCHAR2(100) | ✓ |  |  |  |
| D_CA | VARCHAR2(100) | ✓ |  |  |  |
| D_CLIENT | VARCHAR2(100) | ✓ |  |  |  |
| D_CU | VARCHAR2(100) | ✓ |  |  |  |
| D_DEST | VARCHAR2(100) | ✓ |  |  |  |
| D_DEVTR | VARCHAR2(100) | ✓ |  |  |  |
| D_DIM2 | VARCHAR2(100) | ✓ |  |  |  |
| D_DIM3 | VARCHAR2(100) | ✓ |  |  |  |
| D_DIM4 | VARCHAR2(100) | ✓ |  |  |  |
| D_DP | VARCHAR2(100) | ✓ |  |  |  |
| D_FL | VARCHAR2(100) | ✓ |  |  |  |
| D_GO | VARCHAR2(100) | ✓ |  |  |  |
| D_LE | VARCHAR2(100) | ✓ |  |  |  |
| D_NU | VARCHAR2(100) | ✓ |  |  |  |
| D_ORU | VARCHAR2(100) | ✓ |  |  |  |
| D_PE | VARCHAR2(100) | ✓ |  |  |  |
| D_RU | VARCHAR2(100) | ✓ |  |  |  |
| D_SAJOB | VARCHAR2(100) | ✓ |  |  |  |
| D_T1 | VARCHAR2(100) | ✓ |  |  |  |
| D_T2 | VARCHAR2(100) | ✓ |  |  |  |
| D_TO | VARCHAR2(100) | ✓ |  |  |  |
| P_AMOUNT | NUMBER | ✓ |  |  |  |
| P_COMMENT | VARCHAR2(100) | ✓ |  |  |  |
| ANIO | NUMBER | ✓ |  |  |  |
| MES | NUMBER | ✓ |  |  |  |
| COD_CONCEPTO | NUMBER | ✓ |  |  |  |
| COD_PAIS | NUMBER | ✓ |  |  |  |
| COD_GRUPO | NUMBER | ✓ |  |  |  |
| COD_NETWORK | NUMBER | ✓ |  |  |  |
| COD_PURE_PLAYER | NUMBER | ✓ |  |  |  |

### TMP_TS_BILLINGS

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER |  |  |  |  |
| ANIO | NUMBER(4,0) |  |  |  |  |
| COD_VERSION | NUMBER |  |  |  |  |
| MES | NUMBER(2,0) |  |  |  |  |
| COD_COMPANIA | NUMBER |  |  |  |  |
| COD_GRUPO | NUMBER |  |  |  |  |
| COD_GRUPO_PRINCIPAL | NUMBER |  |  |  |  |
| COD_MEDIO | NUMBER |  |  |  |  |
| COD_GRUPO_ENTORNO | NUMBER |  |  |  |  |
| COD_VIRTUAL | NUMBER |  |  |  |  |
| COD_ESPECIALIDAD_PPTO | NUMBER |  |  |  |  |
| IMP_NETO_VENTA | NUMBER |  |  |  |  |
| MARGEN_BRUTO | NUMBER |  |  |  |  |
| PORCENTAJE_VTA | NUMBER |  |  |  |  |
| PORCENTAJE_MARGEN | NUMBER |  |  |  |  |

### TMP_TS_COMP

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER |  |  |  |  |
| ANIO | NUMBER |  |  |  |  |
| COD_VERSION | NUMBER |  |  |  |  |
| COD_EMPLEADO | NUMBER |  |  |  |  |
| COD_NATURALEZA | NUMBER | ✓ |  |  |  |
| MES | NUMBER |  |  |  |  |
| COD_VIRTUAL | NUMBER |  |  |  |  |
| COD_VERTICAL | NUMBER |  |  |  |  |
| COD_ESPECIALIDAD | NUMBER |  |  |  |  |
| COD_TIPO_AGRUPACION_DESTINO | NUMBER |  |  |  |  |
| SBA | NUMBER |  |  |  |  |
| AYUDA_ALIMENTARIA | NUMBER |  |  |  |  |
| INGRESO_CUENTA_ESPECIE | NUMBER |  |  |  |  |
| SEGURO | NUMBER |  |  |  |  |
| OTROS_SEGUROS | NUMBER |  |  |  |  |
| AHORROS_SEGURO_VIDA | NUMBER |  |  |  |  |
| EXTRA_BONUS | NUMBER |  |  |  |  |
| SEGURIDAD_SOCIAL | NUMBER |  |  |  |  |
| OTROS_AHORROS_SS | NUMBER |  |  |  |  |
| LOCOMOCION | NUMBER |  |  |  |  |
| FORMACION | NUMBER |  |  |  |  |
| BONUS | NUMBER |  |  |  |  |
| APORTACION_PJ | NUMBER |  |  |  |  |
| ESPECIE_CORPORATIVA | NUMBER |  |  |  |  |
| FTE | NUMBER |  |  |  |  |
| FTE_PRORRATEADO | NUMBER |  |  |  |  |
| INDEMNIZACION | NUMBER |  |  |  |  |
| SBA_BECARIO | NUMBER |  |  |  |  |
| FTE_BECARIO | NUMBER |  |  |  |  |
| PORCENTAJE | NUMBER |  |  |  |  |

**Índices:**

- `IDX_TMP_TS_COMP_MES_MD_SP_GRU` — COD_PAIS, ANIO, COD_VERSION, MES

### TMP_TS_CONSOLIDADO

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER |  |  |  |  |
| ANIO | NUMBER |  |  |  |  |
| COD_VERSION | NUMBER |  |  |  |  |
| MES | NUMBER |  |  |  |  |
| COD_EMPLEADO | NUMBER | ✓ |  |  |  |
| COD_COMPANIA | NUMBER | ✓ |  |  |  |
| COD_GRUPO | NUMBER | ✓ |  |  |  |
| COD_VIRTUAL | NUMBER | ✓ |  |  |  |
| COD_GRUPO_ENTORNO | NUMBER | ✓ |  |  |  |
| COD_VERTICAL | NUMBER | ✓ |  |  |  |
| COD_MEDIO | NUMBER | ✓ |  |  |  |
| COD_ESPECIALIDAD | NUMBER | ✓ |  |  |  |
| COD_ESPECIALIDAD_PPTO | NUMBER | ✓ |  |  |  |
| COD_OFICINA | NUMBER | ✓ |  |  |  |
| IMP_NETO_VENTA | NUMBER | ✓ |  |  |  |
| IMP_MARGEN_BRUTO | NUMBER | ✓ |  |  |  |
| COSTE | NUMBER | ✓ |  |  |  |
| FTE | NUMBER | ✓ |  |  |  |
| FTE_PRORRATEADO | NUMBER | ✓ |  |  |  |
| COD_ORIGEN_CONSOLIDADO | NUMBER |  |  |  |  |
| COD_USUARIO | NUMBER |  |  |  |  |
| F_ACTUALIZACION | DATE | ✓ |  |  |  |
| SBA | NUMBER |  |  |  |  |
| AYUDA_ALIMENTARIA | NUMBER |  |  |  |  |
| INGRESO_CUENTA_ESPECIE | NUMBER |  |  |  |  |
| SEGURO | NUMBER |  |  |  |  |
| OTROS_SEGUROS | NUMBER |  |  |  |  |
| AHORROS_SEGURO_VIDA | VARCHAR2(50) |  |  |  |  |
| EXTRA_BONUS | NUMBER |  |  |  |  |
| SEGURIDAD_SOCIAL | NUMBER |  |  |  |  |
| OTROS_AHORROS_SS | NUMBER |  |  |  |  |
| LOCOMOCION | NUMBER |  |  |  |  |
| FORMACION | NUMBER |  |  |  |  |
| BONUS | NUMBER |  |  |  |  |
| APPORTACION_PJ | NUMBER |  |  |  |  |
| ESPECIE_CORPORATIVA | NUMBER |  |  |  |  |
| INDEMNIZACION | NUMBER |  |  |  |  |
| SBA_BECARIO | NUMBER |  |  |  |  |
| PORCENTAJE | NUMBER |  |  |  |  |
| COD_NATURALEZA | NUMBER | ✓ |  |  |  |

### TS_ACCION

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_ACCION | NUMBER |  | PK |  |  |
| DES_ACCION | VARCHAR2(50) |  |  |  |  |

**Índices:**

- `PK_TS_ACCION` (UNIQUE) — COD_ACCION

### TS_CALC_EMP_PCT_GR_ENT_NO_INV

TABLA CON LOS GRUPOS PRECALCULADOS. GRUPO ENTORNO SIN INVERSION EN ORIGEN

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER |  |  |  |  |
| ANIO | NUMBER |  |  |  |  |
| COD_VERSION | NUMBER |  |  |  |  |
| COD_EMPLEADO | NUMBER |  |  |  |  |
| COD_GRUPO_ENTORNO | NUMBER |  |  |  |  |
| COD_VIRTUAL | NUMBER |  |  |  |  |
| COD_VERTICAL | NUMBER |  |  |  |  |
| COD_ESPECIALIDAD | NUMBER |  |  |  |  |
| COD_PURE_PLAYER | NUMBER |  |  |  |  |
| COD_DEPARTAMENTO | NUMBER | ✓ |  |  |  |
| COD_OFICINA | NUMBER |  |  |  |  |
| ID_CDC_DESTINO | NUMBER |  |  |  |  |
| MES | NUMBER |  |  |  |  |
| IMPORTE | NUMBER |  |  |  |  |
| IMPORTE_ACUMULADO | NUMBER |  |  |  |  |
| PCT | NUMBER |  |  |  |  |
| PCT_ACUM | NUMBER |  |  |  |  |

**Índices:**

- `IDX_TS_CALC_PT_GRE_P_AN_VR_EMP` — COD_PAIS, ANIO, COD_VERSION, COD_EMPLEADO
- `IDX_TS_CL_PGR_P_N_VR_MP_MS_GRE` — COD_PAIS, ANIO, COD_VERSION, COD_EMPLEADO, MES, COD_GRUPO_ENTORNO
- `IDX_TS_CLC_PT_GRE_P_N_VR_MP_MS` — COD_PAIS, ANIO, COD_VERSION, COD_EMPLEADO, MES

### TS_CALC_EMP_PCT_GRUPO

TABLA CON LOS GRUPOS PRECALCULADOS. TANTO DE ENTORNO CON INVERSION COMO DE GRUPO CON/SIN INVERSION

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER |  |  |  |  |
| ANIO | NUMBER |  |  |  |  |
| COD_VERSION | NUMBER |  |  |  |  |
| COD_EMPLEADO | NUMBER |  |  |  |  |
| COD_GRUPO | NUMBER |  |  |  |  |
| COD_VIRTUAL | NUMBER |  |  |  |  |
| COD_VERTICAL | NUMBER |  |  |  |  |
| COD_ESPECIALIDAD | NUMBER |  |  |  |  |
| COD_PURE_PLAYER | NUMBER |  |  |  |  |
| COD_DEPARTAMENTO | NUMBER | ✓ |  |  |  |
| COD_OFICINA | NUMBER |  |  |  |  |
| ID_CDC_DESTINO | NUMBER |  |  |  |  |
| MES | NUMBER |  |  |  |  |
| IMPORTE | NUMBER |  |  |  |  |
| IMPORTE_ACUMULADO | NUMBER |  |  |  |  |
| PCT | NUMBER |  |  |  |  |
| PCT_ACUM | NUMBER |  |  |  |  |

**Índices:**

- `IDX_TS_CALC_PCT_GR_P_AN_VR_EMP` — COD_PAIS, ANIO, COD_VERSION, COD_EMPLEADO
- `IDX_TS_CL_P_GR_P_N_VR_MP_MS_GR` — COD_PAIS, ANIO, COD_VERSION, COD_EMPLEADO, MES, COD_GRUPO
- `IDX_TS_CLC_PCT_GR_P_N_VR_MP_MS` — COD_PAIS, ANIO, COD_VERSION, COD_EMPLEADO, MES

### TS_CONFIG

Tabla de configuración para TimeSheet. por ejemplo, literales de la web, etc. 

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CONFIG | NUMBER |  | PK |  |  |
| DES_CONFIG | VARCHAR2(50) |  |  |  |  |
| VALOR | CLOB |  |  |  | Se pone un CLOB para que se pueda utilizar cualquier cosa, incluso un HTML. |
| OBSERVACIONES | VARCHAR2(200) | ✓ |  |  |  |

**Índices:**

- `PK_TS_CONFIG` (UNIQUE) — COD_CONFIG

### TS_CONFIG_ESTADO_CELDA

Tabla que se utiliza para saber que color poner a una celda, si está bloqueada o no etc dependiendo del estado. También sirve para las leyendas que se mostrarán, por lo que tendrán un orden

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_CONFIG_ESTADO_CELDA | NUMBER |  | PK |  |  |
| IND_ESTADO | NUMBER |  |  |  | Bit del estado |
| PRIORIDAD | NUMBER |  |  |  | Para saber en el caso de tener dos estados cual es más prioritario |
| COLOR | VARCHAR2(50) |  |  |  | Color con el que se pintará la celda |
| IND_BLOQUEADO | NUMBER(1,0) |  |  |  | 1: La celda se bloqueará 0: no se bloqueará por esta configuración |
| ORDEN | NUMBER |  |  |  | Orden en el que se mostrarán len las leyendas |
| DESCRIPCION_LEYENDA | VARCHAR2(100) |  |  |  | Descripción que se mostrará enla leyenda |
| OBSERVACIONES | VARCHAR2(200) | ✓ |  |  |  |

**Índices:**

- `PK_TS_CONFIG_ESTADO_CELDA` (UNIQUE) — COD_CONFIG_ESTADO_CELDA

### TS_CONSOLIDADO

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER |  |  |  |  |
| ANIO | NUMBER |  |  |  |  |
| COD_VERSION | NUMBER |  |  |  |  |
| MES | NUMBER |  |  |  |  |
| COD_EMPLEADO | NUMBER | ✓ |  |  |  |
| COD_COMPANIA | NUMBER | ✓ |  |  |  |
| COD_GRUPO | NUMBER | ✓ |  |  |  |
| COD_VIRTUAL | NUMBER | ✓ |  |  |  |
| COD_GRUPO_ENTORNO | NUMBER | ✓ |  |  |  |
| COD_VERTICAL | NUMBER | ✓ |  |  |  |
| COD_MEDIO | NUMBER | ✓ |  |  |  |
| COD_ESPECIALIDAD | NUMBER | ✓ |  |  |  |
| COD_ESPECIALIDAD_PPTO | NUMBER | ✓ |  |  |  |
| COD_OFICINA | NUMBER | ✓ |  |  |  |
| IMP_NETO_VENTA | NUMBER | ✓ |  |  |  |
| IMP_MARGEN_BRUTO | NUMBER | ✓ |  |  |  |
| COSTE | NUMBER | ✓ |  |  |  |
| FTE | NUMBER | ✓ |  |  |  |
| FTE_PRORRATEADO | NUMBER | ✓ |  |  |  |
| COD_ORIGEN_CONSOLIDADO | NUMBER |  |  | → TS_ORIGEN_CONSOLIDADO |  |
| COD_USUARIO | NUMBER |  |  |  | Usuario que lo ejecuta -1 Sistemas, desconocido |
| F_ACTUALIZACION | DATE | ✓ |  |  |  |
| SBA | NUMBER |  |  |  |  |
| AYUDA_ALIMENTARIA | NUMBER |  |  |  |  |
| INGRESO_CUENTA_ESPECIE | NUMBER |  |  |  |  |
| SEGURO | NUMBER |  |  |  |  |
| OTROS_SEGUROS | NUMBER |  |  |  |  |
| AHORROS_SEGURO_VIDA | NUMBER |  |  |  |  |
| EXTRA_BONUS | NUMBER |  |  |  |  |
| SEGURIDAD_SOCIAL | NUMBER |  |  |  |  |
| OTROS_AHORROS_SS | NUMBER |  |  |  |  |
| LOCOMOCION | NUMBER |  |  |  |  |
| FORMACION | NUMBER |  |  |  |  |
| BONUS | NUMBER |  |  |  |  |
| APPORTACION_PJ | NUMBER |  |  |  |  |
| ESPECIE_CORPORATIVA | NUMBER |  |  |  |  |
| INDEMNIZACION | NUMBER |  |  |  |  |
| SBA_BECARIO | NUMBER | ✓ |  |  |  |
| PORCENTAJE | NUMBER | ✓ |  |  |  |
| COD_NATURALEZA | NUMBER | ✓ |  | → PY_NATURALEZA |  |

**Índices:**

- `IDX_TS_CONSO_ANIOVERSION_GRUEN` — SYS_NC00041$, COD_GRUPO_ENTORNO
- `IDX_TS_CONSOLIDADO_ANIOVERSION` — SYS_NC00042$
- `IDX_TS_CONSOLIDADO_ANIOVESION` — SYS_NC00041$
- `IDX_TS_CONSOLIDADO_VERSION` — COD_PAIS, ANIO, COD_VERSION
- `IND_TS_CONSOLIDADO_VER_EMP` — COD_PAIS, ANIO, COD_VERSION, COD_EMPLEADO

### TS_CONSOLIDADO_BAKCUP

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER |  |  |  |  |
| ANIO | NUMBER |  |  |  |  |
| COD_VERSION | NUMBER |  |  |  |  |
| MES | NUMBER |  |  |  |  |
| COD_EMPLEADO | NUMBER | ✓ |  |  |  |
| COD_COMPANIA | NUMBER | ✓ |  |  |  |
| COD_GRUPO | NUMBER | ✓ |  |  |  |
| COD_VIRTUAL | NUMBER | ✓ |  |  |  |
| COD_GRUPO_ENTORNO | NUMBER | ✓ |  |  |  |
| COD_VERTICAL | NUMBER | ✓ |  |  |  |
| COD_MEDIO | NUMBER | ✓ |  |  |  |
| COD_ESPECIALIDAD | NUMBER | ✓ |  |  |  |
| COD_ESPECIALIDAD_PPTO | NUMBER | ✓ |  |  |  |
| COD_OFICINA | NUMBER | ✓ |  |  |  |
| IMP_NETO_VENTA | NUMBER | ✓ |  |  |  |
| IMP_MARGEN_BRUTO | NUMBER | ✓ |  |  |  |
| COSTE | NUMBER | ✓ |  |  |  |
| FTE | NUMBER | ✓ |  |  |  |
| FTE_PRORRATEADO | NUMBER | ✓ |  |  |  |
| COD_ORIGEN_CONSOLIDADO | NUMBER |  |  |  |  |
| COD_USUARIO | NUMBER |  |  |  |  |
| F_ACTUALIZACION | DATE | ✓ |  |  |  |
| SBA | NUMBER |  |  |  |  |
| AYUDA_ALIMENTARIA | NUMBER |  |  |  |  |
| INGRESO_CUENTA_ESPECIE | NUMBER |  |  |  |  |
| SEGURO | NUMBER |  |  |  |  |
| OTROS_SEGUROS | NUMBER |  |  |  |  |
| AHORROS_SEGURO_VIDA | NUMBER |  |  |  |  |
| EXTRA_BONUS | NUMBER |  |  |  |  |
| SEGURIDAD_SOCIAL | NUMBER |  |  |  |  |
| OTROS_AHORROS_SS | NUMBER |  |  |  |  |
| LOCOMOCION | NUMBER |  |  |  |  |
| FORMACION | NUMBER |  |  |  |  |
| BONUS | NUMBER |  |  |  |  |
| APPORTACION_PJ | NUMBER |  |  |  |  |
| ESPECIE_CORPORATIVA | NUMBER |  |  |  |  |
| INDEMNIZACION | NUMBER |  |  |  |  |
| F_BACKUP | DATE |  |  |  |  |
| COD_USUARIO_BACKUP | NUMBER |  |  |  |  |

### TS_CONSOLIDADO_INVERSION

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER |  |  |  |  |
| ANIO | NUMBER |  |  |  |  |
| COD_VERSION | NUMBER |  |  |  |  |
| MES | NUMBER |  |  |  |  |
| COD_COMPANIA | NUMBER | ✓ |  |  |  |
| COD_GRUPO | NUMBER | ✓ |  |  |  |
| COD_VIRTUAL | NUMBER | ✓ |  |  |  |
| COD_GRUPO_ENTORNO | NUMBER | ✓ |  |  |  |
| COD_VERTICAL | NUMBER | ✓ |  |  |  |
| COD_MEDIO | NUMBER | ✓ |  |  |  |
| COD_ESPECIALIDAD | NUMBER | ✓ |  |  |  |
| COD_ESPECIALIDAD_PPTO | NUMBER | ✓ |  |  |  |
| IMP_NETO_VENTA | NUMBER | ✓ |  |  |  |
| IMP_MARGEN_BRUTO | NUMBER | ✓ |  |  |  |
| COD_USUARIO | NUMBER |  |  |  | Usuario que lo ejecuta -1 Sistemas, desconocido |
| F_ACTUALIZACION | DATE | ✓ |  |  |  |
| IND_AJUSTE | NUMBER | ✓ |  |  |  |

**Índices:**

- `IDX_TS_CONS_INV_ANIOVERSION` — SYS_NC00019$
- `IDX_TS_CONSO_INV_ANVER_GRUEN` — SYS_NC00018$, COD_GRUPO_ENTORNO
- `IDX_TS_CONSOLIADO_VER` — COD_PAIS, ANIO, COD_VERSION

### TS_GRUPO_NO_DISTR_VIRTUAL

Grupos que no se utilizan en la distribución de billings cuando se hace por virtual. En principio solo habrá clientes principales.

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER |  | PK |  |  |
| COD_GRUPO | NUMBER |  | PK |  |  |
| F_MODIFICACION | DATE | ✓ |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER | ✓ |  |  |  |

**Índices:**

- `PK_TS_GRUPO_NO_DIST_VIR` (UNIQUE) — COD_PAIS, COD_GRUPO

### TS_IMPUTA

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_TS_IMPUTA | NUMBER |  | PK |  |  |
| COD_EMPLEADO | NUMBER |  |  | → PY_EMPLEADO |  |
| COD_PAIS | NUMBER |  |  |  |  |
| ANIO | NUMBER |  |  |  |  |
| COD_VERSION | NUMBER |  |  |  |  |
| IND_TODOS_GRUPOS | NUMBER |  |  |  | 0: No son todos sus grupos. Necesita datos en TS_IMPUTA_GRUPO 1: Tienen todos los grupos |
| IND_TODOS_ENTORNOS | NUMBER |  |  |  | 0: No son todos sus entornos. Necesita datos en TS_IMPUTA_GRUPO 1: Tienen todos los entornos |
| IND_JIRA | NUMBER | ✓ |  |  | 1: Viene de JIRA |
| F_CONFIRMACION | DATE | ✓ |  |  |  |
| PORCENTAJE_GRUPO | NUMBER(6,3) |  |  |  | Porcentaje que se distribuye por grupo. Si es 0 no debería haber ningún regustro de TS_IMPUTA_GRUPO |
| PORCENTAJE_GRUPO_ENTORNO | NUMBER(6,3) |  |  |  | Porcentaje que se distribuye por grupo entorno. Si es 0 no debería haber ningún regustro de TS_IMPUTA_GRUPO_ENTORNO |
| F_MODIFICACION | DATE |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |

**Índices:**

- `IDX_IMPUTA_TS_GR_NO_GRU` — COD_PAIS, ANIO, COD_VERSION, PORCENTAJE_GRUPO
- `IDX_IMPUTA_TS_GR_NO_GRUE` — COD_PAIS, ANIO, COD_VERSION, PORCENTAJE_GRUPO_ENTORNO
- `PK_TS_IMPUTA` (UNIQUE) — COD_TS_IMPUTA
- `UQ_TS_IMPUTA` (UNIQUE) — COD_PAIS, ANIO, COD_VERSION, COD_EMPLEADO

### TS_IMPUTA_GRUPO

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_TS_IMPUTA | NUMBER |  | PK | → TS_IMPUTA |  |
| COD_PAIS | NUMBER |  | PK |  |  |
| COD_GRUPO | NUMBER |  | PK |  | De momento solo se grabarán los grupos principales |
| MES | NUMBER |  | PK |  |  |
| IND_AUTOMATICO | NUMBER |  |  |  | 0: Manual 1: Automático (se obtiene de billings) |
| PORCENTAJE | NUMBER(6,3) |  |  |  |  |
| F_MODIFICACION | DATE |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |

**Índices:**

- `PK_TS_IMPUTA_GRUPO` (UNIQUE) — COD_TS_IMPUTA, COD_GRUPO, COD_PAIS, MES

### TS_IMPUTA_GRUPO_ENTORNO

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_TS_IMPUTA | NUMBER |  | PK | → TS_IMPUTA |  |
| COD_GRUPO_ENTORNO | NUMBER |  | PK |  |  |
| MES | NUMBER |  | PK |  |  |
| IND_AUTOMATICO | NUMBER |  |  |  | 0: Manual 1: Automático (se obtiene de billings) |
| PORCENTAJE | NUMBER(6,3) |  |  |  |  |
| F_MODIFICACION | DATE |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |

**Índices:**

- `PK_TS_IMPUTA_GRUPO_ENTORNO` (UNIQUE) — COD_TS_IMPUTA, COD_GRUPO_ENTORNO, MES

### TS_IMPUTA_GRUPO_ENTORNO_MIG

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| ANIO | NUMBER |  |  |  |  |
| COD_VERSION | NUMBER |  |  |  |  |
| COD_EMPLEADO | NUMBER |  |  |  |  |
| FULL_NAME | VARCHAR2(200) |  |  |  |  |
| COD_GRUPO_ENTORNO | NUMBER |  |  |  |  |
| DES_GRUPO_ENTORNO | VARCHAR2(200) |  |  |  |  |
| MES | NUMBER |  |  |  |  |
| IND_AUTOMATICO | NUMBER(1,0) |  |  |  |  |
| PORCENTAJE | NUMBER(6,3) |  |  |  |  |

**Índices:**

- `TS_IMPUTA_GR_ENT_MIG_EMPLEADO` — COD_EMPLEADO

### TS_IMPUTA_GRUPO_MIG

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| ANIO | NUMBER |  |  |  |  |
| COD_VERSION | NUMBER |  |  |  |  |
| COD_EMPLEADO | NUMBER |  |  |  |  |
| FULL_NAME | VARCHAR2(200) |  |  |  |  |
| COD_GRUPO | NUMBER |  |  |  |  |
| DES_GRUPO | VARCHAR2(200) |  |  |  |  |
| MES | NUMBER |  |  |  |  |
| IND_AUTOMATICO | NUMBER(1,0) |  |  |  |  |
| PORCENTAJE | NUMBER(6,3) |  |  |  |  |

### TS_IMPUTA_MIG

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| ANIO | NUMBER |  |  |  |  |
| COD_VERSION | NUMBER |  |  |  |  |
| COD_EMPLEADO | NUMBER |  |  |  |  |
| FULL_NAME | VARCHAR2(200) |  |  |  |  |
| PORCENTAJE_GRUPO | NUMBER(6,3) | ✓ |  |  |  |
| IND_TODOS_GRUPOS | NUMBER | ✓ |  |  |  |
| PORCENTAJE_GRUPO_ENTORNO | NUMBER(6,3) | ✓ |  |  |  |
| IND_TODOS_ENTORNOS | NUMBER | ✓ |  |  |  |

### TS_LOG

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_ACCION | NUMBER |  |  | → TS_ACCION |  |
| COD_USUARIO | NUMBER |  |  |  |  |
| COD_EMPLEADO_ACCION | NUMBER | ✓ |  |  |  |
| COD_PAIS | NUMBER | ✓ |  |  |  |
| ANIO | NUMBER | ✓ |  |  |  |
| COD_VERSION | NUMBER | ✓ |  |  |  |
| COD_EMPLEADO | NUMBER | ✓ |  |  |  |
| F_LOG | DATE |  |  |  |  |

### TS_MV_AUX_COSTE_COMP

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| ANIO | NUMBER | ✓ |  |  |  |
| COD_VERSION | NUMBER | ✓ |  |  |  |
| COD_EMPLEADO | NUMBER | ✓ |  |  |  |
| DNI | VARCHAR2(10) | ✓ |  |  |  |
| COD_NATURALEZA | NUMBER | ✓ |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |
| SBA | NUMBER | ✓ |  |  |  |
| BONUS | NUMBER | ✓ |  |  |  |
| SEGUROS | NUMBER | ✓ |  |  |  |
| SEGURIDAD_SOCIAL | NUMBER | ✓ |  |  |  |
| OTROS_GASTOS | NUMBER | ✓ |  |  |  |
| TOT | NUMBER | ✓ |  |  |  |

**Índices:**

- `IDX_TMACC_ZUX` — ANIO, COD_VERSION, COD_EMPLEADO

### TS_MV_AUX_COSTE_COMP_CONCEPTO_MES

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| ANIO | NUMBER | ✓ |  |  |  |
| COD_PERIODO | NUMBER | ✓ |  |  |  |
| COD_VERSION | NUMBER | ✓ |  |  |  |
| COD_NATURALEZA | NUMBER | ✓ |  |  |  |
| COD_EMPLEADO | NUMBER | ✓ |  |  |  |
| DNI | VARCHAR2(10) | ✓ |  |  |  |
| COD_VIRTUAL | NUMBER | ✓ |  |  |  |
| DES_VIRTUAL | VARCHAR2(50) | ✓ |  |  |  |
| COD_VERTICAL | NUMBER | ✓ |  |  |  |
| DES_VERTICAL | VARCHAR2(50) | ✓ |  |  |  |
| COD_ESPECIALIDAD | NUMBER | ✓ |  |  |  |
| DES_ESPECIALIDAD | VARCHAR2(50) | ✓ |  |  |  |
| COD_OFICINA | NUMBER | ✓ |  |  |  |
| DES_OFICINA | VARCHAR2(50) | ✓ |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |
| SBA | NUMBER | ✓ |  |  |  |
| BONUS | NUMBER | ✓ |  |  |  |
| SEGUROS | NUMBER | ✓ |  |  |  |
| EXTRABONUS | NUMBER | ✓ |  |  |  |
| SEGURIDAD_SOCIAL | NUMBER | ✓ |  |  |  |
| OTROS_GASTOS | NUMBER | ✓ |  |  |  |
| TOT | NUMBER | ✓ |  |  |  |

**Índices:**

- `TS_MACCCM_IDX` — ANIO, COD_PERIODO, COD_VERSION, COD_EMPLEADO

### TS_MV_AUX_COSTE_COMP_MES

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| ANIO | NUMBER | ✓ |  |  |  |
| COD_PERIODO | NUMBER | ✓ |  |  |  |
| COD_VERSION | NUMBER | ✓ |  |  |  |
| COD_NATURALEZA | NUMBER | ✓ |  |  |  |
| COD_EMPLEADO | NUMBER | ✓ |  |  |  |
| DNI | VARCHAR2(10) | ✓ |  |  |  |
| COD_VIRTUAL | NUMBER | ✓ |  |  |  |
| DES_VIRTUAL | VARCHAR2(50) | ✓ |  |  |  |
| COD_VERTICAL | NUMBER | ✓ |  |  |  |
| DES_VERTICAL | VARCHAR2(50) | ✓ |  |  |  |
| COD_ESPECIALIDAD | NUMBER | ✓ |  |  |  |
| DES_ESPECIALIDAD | VARCHAR2(50) | ✓ |  |  |  |
| COD_OFICINA | NUMBER | ✓ |  |  |  |
| DES_OFICINA | VARCHAR2(50) | ✓ |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |
| TOT | NUMBER | ✓ |  |  |  |

**Índices:**

- `IDX_TMACC_ZUX_MES` — ANIO, COD_VERSION, COD_PERIODO, COD_EMPLEADO

### TS_ORIGEN_CONSOLIDADO

Esta tabla indica de donde o de que forma se saca el consolidado que nos indicarán por ejemplo si es cuando no tiene inversión, o cuando no hay empleado, etc.

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_ORIGEN_CONSOLIDADO | NUMBER |  | PK |  |  |
| DES_ORIGEN_CONSOLIDADO | VARCHAR2(150) |  |  |  |  |

**Índices:**

- `PK_ORIGEN_CONSOLIDADO` (UNIQUE) — COD_ORIGEN_CONSOLIDADO

### TS_VERSION_VERSION_BILLING

Tabla que relaciona las version de timesheet con la de billings. Sin no tiene nada en esta tabla será la misma

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER |  | PK |  |  |
| ANIO | NUMBER |  | PK |  |  |
| COD_VERSION_TS | NUMBER |  | PK |  |  |
| COD_VERSION_BILLING | NUMBER |  | PK |  |  |

**Índices:**

- `PK_TS_VERSION_VERSION_BILLING` (UNIQUE) — COD_PAIS, ANIO, COD_VERSION_TS, COD_VERSION_BILLING

### TS_VERSION_VERSION_COMPENSACION

Tabla que relaciona las version de timesheet con la de compensación. Sin no tiene nada en esta tabla será la misma

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER |  | PK |  |  |
| ANIO | NUMBER |  | PK |  |  |
| COD_VERSION_TS | NUMBER |  | PK |  |  |
| COD_VERSION_COMP | NUMBER |  | PK |  |  |

**Índices:**

- `PK_TS_VERSION_VERSIION_COMP` (UNIQUE) — COD_PAIS, ANIO, COD_VERSION_TS, COD_VERSION_COMP

### TSGL_HORAS_MES_UBICACION

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER |  | PK |  |  |
| ANIO | NUMBER |  | PK |  |  |
| MES | NUMBER |  | PK |  |  |
| HORAS | NUMBER |  |  |  |  |
| COD_UBICACION | NUMBER |  | PK |  | De momento solo se grabarán ubicaciones 1 Madrid EG y 8 Barcelona, el por defecto será 1 |

**Índices:**

- `PK_TSGL_HORAS_MES_UBICACION` (UNIQUE) — COD_PAIS, ANIO, MES, COD_UBICACION

### V_AN_EMPLEADO_TIPO_IMPUTACION

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_EMPLEADO | NUMBER | ✓ |  |  |  |
| ANIO | NUMBER(4,0) | ✓ |  |  |  |
| COD_VERSION | NUMBER | ✓ |  |  |  |
| F_DESDE | DATE | ✓ |  |  |  |
| F_HASTA | DATE | ✓ |  |  |  |
| TIPO_IMPUTACION | CHAR(1) | ✓ |  |  |  |
| TIPO_IMPUTACION_ESP | CHAR(1) | ✓ |  |  |  |

**Índices:**

- `IDX_AN_EMP_TIP_IMP_EMP_AN_VER` — COD_EMPLEADO, ANIO, COD_VERSION
- `IDX_AN_EMP_TIP_IMP_EMP_AN_VER_F` — COD_EMPLEADO, ANIO, COD_VERSION, F_DESDE, F_HASTA

### V_COMPANIA_CDC

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER | ✓ |  |  |  |
| COD_COMPANIA | NUMBER | ✓ |  |  |  |
| DES_COMPANIA | VARCHAR2(40) | ✓ |  |  |  |
| ID_CDC | NUMBER | ✓ |  |  |  |
| COD_CDC | VARCHAR2(150) | ✓ |  |  |  |
| DES_CDC | VARCHAR2(240) | ✓ |  |  |  |
| COD_CLIENTE_GENERICO | VARCHAR2(240) | ✓ |  |  |  |
| COD_CUENTA_STAT | VARCHAR2(240) | ✓ |  |  |  |
| COD_TIPO_AGRUPACION_CDC | NUMBER | ✓ |  |  |  |

### VM_MINER_OTHER_EXP_DIARIO_FACT

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PAIS | NUMBER | ✓ |  |  |  |
| ANIO | NUMBER | ✓ |  |  |  |
| ANIO_CONTABLE | NUMBER | ✓ |  |  |  |
| MES_CONTABLE | NUMBER | ✓ |  |  |  |
| COD_VERSION | NUMBER | ✓ |  |  |  |
| DES_VERSION | VARCHAR2(40) | ✓ |  |  |  |
| MES | NUMBER | ✓ |  |  |  |
| NUM_INFORME | VARCHAR2(50) | ✓ |  |  |  |
| IND_REFACTURABLE | VARCHAR2(1) | ✓ |  |  |  |
| IND_CALCULO_HC | VARCHAR2(1) | ✓ |  |  |  |
| COD_EMPLEADO | NUMBER | ✓ |  |  |  |
| NOMBRE | VARCHAR2(100) | ✓ |  |  |  |
| APELLIDO1 | VARCHAR2(100) | ✓ |  |  |  |
| APELLIDO2 | VARCHAR2(100) | ✓ |  |  |  |
| NOMBRE_COMPLETO | VARCHAR2(152) | ✓ |  |  |  |
| F_ALTA | DATE | ✓ |  |  |  |
| F_BAJA | DATE | ✓ |  |  |  |
| COD_VIRTUAL | NUMBER | ✓ |  |  |  |
| DES_VIRTUAL | VARCHAR2(50) | ✓ |  |  |  |
| COD_VERTICAL | NUMBER | ✓ |  |  |  |
| DES_VERTICAL | VARCHAR2(50) | ✓ |  |  |  |
| COD_ESPECIALIDAD | NUMBER | ✓ |  |  |  |
| DES_ESPECIALIDAD | VARCHAR2(50) | ✓ |  |  |  |
| COD_GRUPO_ESPECIALIDAD | NUMBER | ✓ |  |  |  |
| DES_GRUPO_ESPECIALIDAD | VARCHAR2(50) | ✓ |  |  |  |
| COD_OFICINA | NUMBER | ✓ |  |  |  |
| DES_OFICINA | VARCHAR2(50) | ✓ |  |  |  |
| COD_CDC_ORIGEN | VARCHAR2(150) | ✓ |  |  |  |
| COD_COMPANIA_ORIGEN | NUMBER | ✓ |  |  |  |
| DES_CDC_ORIGEN | VARCHAR2(240) | ✓ |  |  |  |
| DES_COMPANIA_ORIGEN | VARCHAR2(40) | ✓ |  |  |  |
| COD_CDC_DESTINO | VARCHAR2(4000) | ✓ |  |  |  |
| COD_COMPANIA_DESTINO | NUMBER | ✓ |  |  |  |
| DES_CDC_DESTINO | VARCHAR2(240) | ✓ |  |  |  |
| DES_COMPANIA_DESTINO | VARCHAR2(40) | ✓ |  |  |  |
| COD_TIPO_AGRUPACION_DESTINO | NUMBER | ✓ |  |  |  |
| CUENTA_CONTABLE | VARCHAR2(150) | ✓ |  |  |  |
| COD_CONCEPTO_OE | VARCHAR2(25) | ✓ |  |  |  |
| DES_CUENTA_CONTABLE | VARCHAR2(240) | ✓ |  |  |  |
| COD_ORIGEN_GASTO | NUMBER | ✓ |  |  |  |
| DES_ORIGEN_GASTO | VARCHAR2(50) | ✓ |  |  |  |
| NUMERO_SOLICITUD | VARCHAR2(100) | ✓ |  |  |  |
| NUMERO_PEDIDO | VARCHAR2(100) | ✓ |  |  |  |
| SOLICITANTE | VARCHAR2(240) | ✓ |  |  |  |
| FECHA_DESDE_SOLPED | DATE | ✓ |  |  |  |
| FECHA_HASTA_SOLPED | DATE | ✓ |  |  |  |
| COMPRADOR | VARCHAR2(240) | ✓ |  |  |  |
| DESCRIPCION_PEDIDO | VARCHAR2(240) | ✓ |  |  |  |
| MATRICULA | VARCHAR2(150) | ✓ |  |  |  |
| COD_EMPLEADO_MATRICULA | NUMBER | ✓ |  |  |  |
| EMPLEADO_MATRICULA | VARCHAR2(152) | ✓ |  |  |  |
| UBICACION_ARRENDAMINETO | VARCHAR2(100) | ✓ |  |  |  |
| UBICACION_RECURRENTE | VARCHAR2(100) | ✓ |  |  |  |
| COD_GRUPO_PARKING | NUMBER | ✓ |  |  |  |
| DES_GRUPO_PARKING | VARCHAR2(100) | ✓ |  |  |  |
| NUM_FACTURA | VARCHAR2(240) | ✓ |  |  |  |
| FECHA_FACTURA | DATE | ✓ |  |  |  |
| COD_PROVEEDOR | VARCHAR2(80) | ✓ |  |  |  |
| DES_PROVEEDOR | VARCHAR2(360) | ✓ |  |  |  |
| FECHA_GASTO_IEXPENSES | DATE | ✓ |  |  |  |
| ERROR | VARCHAR2(100) | ✓ |  |  |  |
| DES_ERROR | VARCHAR2(100) | ✓ |  |  |  |
| DES_TIPO_AGRUPACION_DESTINO | VARCHAR2(50) | ✓ |  |  |  |
| DESCRIPCION_LINEA | VARCHAR2(2000) | ✓ |  |  |  |
| DES_ASIENTO | VARCHAR2(2000) | ✓ |  |  |  |
| NOMBRE_LOTE | VARCHAR2(100) | ✓ |  |  |  |
| PORCENTAJE | NUMBER | ✓ |  |  |  |
| IMPORTE | NUMBER | ✓ |  |  |  |

### VM_XXHM_ES_OTHER_EXP_AN_CIERRE

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| NUM_PEDIDO | VARCHAR2(20) | ✓ |  |  |  |
| CUENTA | VARCHAR2(25) | ✓ |  |  |  |
| FACTURA | VARCHAR2(240) | ✓ |  |  |  |
| DISTRIBUTION_ID_PEDIDO | VARCHAR2(40) | ✓ |  |  |  |
| TOTAL | NUMBER | ✓ |  |  |  |

**Índices:**

- `IDX_OF_TOTHER_EX_AN_LAST_CIER_DIST_ID_PED` — DISTRIBUTION_ID_PEDIDO

### VM_XXHM_ES_OTHER_EXPENSES_AN

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_COMPANIA_MMS | VARCHAR2(10) | ✓ |  |  |  |
| COD_PAIS_MMS | VARCHAR2(10) | ✓ |  |  |  |
| NAME_ORG | VARCHAR2(240) | ✓ |  |  |  |
| ORG_ID | NUMBER(15,0) | ✓ |  |  |  |
| NUM_TERCERO | VARCHAR2(80) | ✓ |  |  |  |
| NUM_PEDIDO | VARCHAR2(20) | ✓ |  |  |  |
| COD_GASTO_PEDIDO | VARCHAR2(100) | ✓ |  |  |  |
| PLANTILLA_PEDIDO | VARCHAR2(150) | ✓ |  |  |  |
| COD_ORIGEN_GASTO | NUMBER | ✓ |  |  |  |
| SOURCE | VARCHAR2(10) | ✓ |  |  |  |
| COMPANIA | VARCHAR2(25) | ✓ |  |  |  |
| CUENTA | VARCHAR2(25) | ✓ |  |  |  |
| CENTRO_DE_COSTE | VARCHAR2(25) | ✓ |  |  |  |
| ID_CDC | NUMBER(15,0) | ✓ |  |  |  |
| ID_CDC_ORI | NUMBER(15,0) | ✓ |  |  |  |
| CDC_ORIGEN | VARCHAR2(150) | ✓ |  |  |  |
| CLIENTE | VARCHAR2(25) | ✓ |  |  |  |
| IMPUTACION | VARCHAR2(25) | ✓ |  |  |  |
| INTERCIA | VARCHAR2(25) | ✓ |  |  |  |
| LIBRE | VARCHAR2(25) | ✓ |  |  |  |
| FECHA_CIERRE | DATE | ✓ |  |  |  |
| FECHA_CONTABLE | DATE | ✓ |  |  |  |
| TIPO_GASTO | VARCHAR2(240) | ✓ |  |  |  |
| DIVISA | CHAR(3) | ✓ |  |  |  |
| CONCEPTO_PEDIDO | VARCHAR2(240) | ✓ |  |  |  |
| DISTRIBUTION_ID_PEDIDO | NUMBER | ✓ |  |  |  |
| DEBE | NUMBER | ✓ |  |  |  |
| HABER | NUMBER | ✓ |  |  |  |
| TOTAL | NUMBER | ✓ |  |  |  |
| MAIL | VARCHAR2(240) | ✓ |  |  |  |

**Índices:**

- `IDX_OF_OTHER_EX_AN_LAST_DIST_ID_PED` — DISTRIBUTION_ID_PEDIDO
- `IDX_OF_OTHER_EX_AN_LAST_F_CIERRE` — SYS_NC00031$

### VM_XXHM_OE_ASIENTOS_AN

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_COMPANIA_MMS | VARCHAR2(10) | ✓ |  |  |  |
| COD_PAIS_MMS | VARCHAR2(10) | ✓ |  |  |  |
| NAME_ORG | VARCHAR2(240) | ✓ |  |  |  |
| ORG_ID | NUMBER(15,0) | ✓ |  |  |  |
| SOURCE_OF | CHAR(17) | ✓ |  |  |  |
| COD_ORIGEN_GASTO | NUMBER | ✓ |  |  |  |
| SOURCE | VARCHAR2(25) | ✓ |  |  |  |
| CONTABILIZADO | VARCHAR2(1) | ✓ |  |  |  |
| SECUENCIA | NUMBER | ✓ |  |  |  |
| NUM_LINEA | VARCHAR2(81) | ✓ |  |  |  |
| CODE_COMBINATION_ID | NUMBER(15,0) | ✓ |  |  |  |
| COMPANIA | VARCHAR2(25) | ✓ |  |  |  |
| CUENTA | VARCHAR2(25) | ✓ |  |  |  |
| CDC | VARCHAR2(25) | ✓ |  |  |  |
| ID_CDC | NUMBER(15,0) | ✓ |  |  |  |
| ID_CDC_ORI | NUMBER(15,0) | ✓ |  |  |  |
| CDC_ORIGEN | VARCHAR2(150) | ✓ |  |  |  |
| CLIENTE | VARCHAR2(25) | ✓ |  |  |  |
| IMPUTACION | VARCHAR2(25) | ✓ |  |  |  |
| INTERCIA | VARCHAR2(25) | ✓ |  |  |  |
| LIBRE | VARCHAR2(25) | ✓ |  |  |  |
| FECHA_CREACION | DATE | ✓ |  |  |  |
| FECHA_CONTABLE | DATE | ✓ |  |  |  |
| PERIOD_NAME | VARCHAR2(15) | ✓ |  |  |  |
| FECHA_CIERRE | DATE | ✓ |  |  |  |
| TIPO_GASTO | VARCHAR2(240) | ✓ |  |  |  |
| DIVISA | CHAR(3) | ✓ |  |  |  |
| DESCRIPCION_LINEA | VARCHAR2(240) | ✓ |  |  |  |
| DEBE | NUMBER | ✓ |  |  |  |
| HABER | NUMBER | ✓ |  |  |  |
| TOTAL | NUMBER | ✓ |  |  |  |
| NOMBRE_LOTE | VARCHAR2(100) | ✓ |  |  |  |
| DES_ASIENTO | VARCHAR2(240) | ✓ |  |  |  |
| NUMERO_LINEA | NUMBER(15,0) | ✓ |  |  |  |
| ASIENTO_ID | NUMBER(15,0) | ✓ |  |  |  |
| PLANTILLA | VARCHAR2(150) | ✓ |  |  |  |
| USER_ID | NUMBER(15,0) | ✓ |  |  |  |
| TIPO_CDC | VARCHAR2(240) | ✓ |  |  |  |

**Índices:**

- `IDX_OF_XXHM_OE_ASIENTOS_AN_IN1` — FECHA_CONTABLE, NUM_LINEA

### VM_XXHM_OE_FACTURAS_AN

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_COMPANIA_MMS | VARCHAR2(10) | ✓ |  |  |  |
| PAIS_MMS | VARCHAR2(10) | ✓ |  |  |  |
| NAME_ORG | VARCHAR2(240) | ✓ |  |  |  |
| ORG_ID | NUMBER(15,0) | ✓ |  |  |  |
| NUM_TERCERO | VARCHAR2(80) | ✓ |  |  |  |
| NOM_TERCERO | VARCHAR2(360) | ✓ |  |  |  |
| CATEGORIA | VARCHAR2(30) | ✓ |  |  |  |
| NUM_FACTURA | VARCHAR2(240) | ✓ |  |  |  |
| IMP_TOTAL_FACTURA | NUMBER | ✓ |  |  |  |
| LINEA | VARCHAR2(81) | ✓ |  |  |  |
| INVOICE_ID | NUMBER | ✓ |  |  |  |
| LINE_NUM | NUMBER | ✓ |  |  |  |
| FECHA_FACTURA | DATE | ✓ |  |  |  |
| CONTABILIZADO | VARCHAR2(1) | ✓ |  |  |  |
| CREATION_DATE | DATE | ✓ |  |  |  |
| FECHA_CONTABLE | DATE | ✓ |  |  |  |
| FECHA_CONTABLE_REAL | DATE | ✓ |  |  |  |
| PERIOD_NAME | VARCHAR2(15) | ✓ |  |  |  |
| FECHA_CIERRE | DATE | ✓ |  |  |  |
| TIPO_GASTO | VARCHAR2(240) | ✓ |  |  |  |
| DIVISA | CHAR(3) | ✓ |  |  |  |
| DESCRIPCION_LINEA | VARCHAR2(240) | ✓ |  |  |  |
| DNI | VARCHAR2(150) | ✓ |  |  |  |
| ATTRIBUTE2 | VARCHAR2(160) | ✓ |  |  |  |
| ATTRIBUTE3 | VARCHAR2(150) | ✓ |  |  |  |
| DISTRIBUTION_LINE_NUMBER | NUMBER | ✓ |  |  |  |
| AMOUNT | NUMBER | ✓ |  |  |  |
| COD_CDC | VARCHAR2(25) | ✓ |  |  |  |
| ID_CDC | NUMBER(15,0) | ✓ |  |  |  |
| CUENTA | VARCHAR2(25) | ✓ |  |  |  |
| CDC | VARCHAR2(25) | ✓ |  |  |  |
| CLIENTE | VARCHAR2(25) | ✓ |  |  |  |
| IMPUTACION | VARCHAR2(25) | ✓ |  |  |  |
| INTERCIA | VARCHAR2(25) | ✓ |  |  |  |
| LIBRE | VARCHAR2(25) | ✓ |  |  |  |
| COD_TIPO_GASTO | CHAR(1) | ✓ |  |  |  |
| COD_NATURALEZA | NUMBER | ✓ |  |  |  |
| INPLANT | VARCHAR2(1) | ✓ |  |  |  |
| VALIJA | VARCHAR2(150) | ✓ |  |  |  |
| ANIO | VARCHAR2(4) | ✓ |  |  |  |

### XXHM_ES_MINER_OTHER_EXPENSES_AN_BK

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_COMPANIA_MMS | VARCHAR2(10) | ✓ |  |  |  |
| COD_PAIS_MMS | VARCHAR2(10) | ✓ |  |  |  |
| NAME_ORG | VARCHAR2(240) | ✓ |  |  |  |
| ORG_ID | NUMBER | ✓ |  |  |  |
| SOURCE_OF | VARCHAR2(25) | ✓ |  |  |  |
| NUM_TERCERO | VARCHAR2(80) | ✓ |  |  |  |
| NOM_TERCERO | VARCHAR2(360) | ✓ |  |  |  |
| NUM_PEDIDO | VARCHAR2(50) | ✓ |  |  |  |
| LINEA_ID_PEDIDO | VARCHAR2(50) | ✓ |  |  |  |
| TIPO_GASTO_PEDIDO | VARCHAR2(50) | ✓ |  |  |  |
| COD_GASTO_PEDIDO | VARCHAR2(50) | ✓ |  |  |  |
| PLANTILLA_PEDIDO | VARCHAR2(50) | ✓ |  |  |  |
| NUMERO_LINEA_PEDIDO | VARCHAR2(50) | ✓ |  |  |  |
| FECHA_FACTURA | DATE | ✓ |  |  |  |
| COD_ORIGEN_GASTO | NUMBER | ✓ |  |  |  |
| SOURCE | VARCHAR2(25) | ✓ |  |  |  |
| CONTABILIZADO | VARCHAR2(1) | ✓ |  |  |  |
| CODE_COMBINATION_ID | NUMBER | ✓ |  |  |  |
| COMPANIA | VARCHAR2(25) | ✓ |  |  |  |
| CUENTA | VARCHAR2(25) | ✓ |  |  |  |
| CDC | VARCHAR2(25) | ✓ |  |  |  |
| ID_CDC | NUMBER | ✓ |  |  |  |
| TIPO_CDC | VARCHAR2(240) | ✓ |  |  |  |
| ID_CDC_ORI | NUMBER | ✓ |  |  |  |
| CDC_ORIGEN | VARCHAR2(150) | ✓ |  |  |  |
| DES_CDC_ORICEN | VARCHAR2(240) | ✓ |  |  |  |
| CLIENTE | VARCHAR2(25) | ✓ |  |  |  |
| IMPUTACION | VARCHAR2(25) | ✓ |  |  |  |
| INTERCIA | VARCHAR2(25) | ✓ |  |  |  |
| LIBRE | VARCHAR2(25) | ✓ |  |  |  |
| FECHA_CREACION | DATE | ✓ |  |  |  |
| FECHA_PAGO | VARCHAR2(50) | ✓ |  |  |  |
| FECHA_CONTABLE | DATE | ✓ |  |  |  |
| ANIO | VARCHAR2(4) | ✓ |  |  |  |
| MES | VARCHAR2(2) | ✓ |  |  |  |
| PERIOD_NAME | VARCHAR2(15) | ✓ |  |  |  |
| FECHA_CIERRE | DATE | ✓ |  |  |  |
| TIPO_GASTO | VARCHAR2(240) | ✓ |  |  |  |
| DIVISA | VARCHAR2(15) | ✓ |  |  |  |
| AUTORIZADOR | VARCHAR2(50) | ✓ |  |  |  |
| FECHA_EJECUCION | VARCHAR2(50) | ✓ |  |  |  |
| FECHA_AUDITORIA | VARCHAR2(50) | ✓ |  |  |  |
| CONCEPTO_PEDIDO | VARCHAR2(50) | ✓ |  |  |  |
| DESCRIPCION_LINEA | VARCHAR2(2000) | ✓ |  |  |  |
| DISTRIBUTION_ID_PEDIDO | NUMBER | ✓ |  |  |  |
| DEBE | NUMBER | ✓ |  |  |  |
| HABER | NUMBER | ✓ |  |  |  |
| TOTAL | NUMBER | ✓ |  |  |  |
| SECUENCIA | NUMBER | ✓ |  |  |  |
| NOMBRE_LOTE | VARCHAR2(100) | ✓ |  |  |  |
| FACTURA | VARCHAR2(240) | ✓ |  |  |  |
| INVOICE_ID | NUMBER | ✓ |  |  |  |
| LINEA | VARCHAR2(81) | ✓ |  |  |  |
| LINE_NUM | NUMBER | ✓ |  |  |  |
| DES_ASIENTO | VARCHAR2(2000) | ✓ |  |  |  |
| EMAIL_ADDRESS | VARCHAR2(240) | ✓ |  |  |  |
| NUM_LINEA | VARCHAR2(81) | ✓ |  |  |  |

**Índices:**

- `IN_XXHM_MINER_OTHER_EXPENSES_U1` — FECHA_CONTABLE, COD_COMPANIA_MMS
- `IN_XXHM_MINER_OTHER_EXPENSES_U2` — NUM_LINEA

### ZZ_TMP_PPT_MAGNITUDE

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_ACCION_LOG | NUMBER |  |  |  |  |
| D_AC | VARCHAR2(100) | ✓ |  |  |  |
| D_ACTY | VARCHAR2(100) | ✓ |  |  |  |
| D_AREA | VARCHAR2(100) | ✓ |  |  |  |
| D_AU | VARCHAR2(100) | ✓ |  |  |  |
| D_CA | VARCHAR2(100) | ✓ |  |  |  |
| D_CLIENT | VARCHAR2(100) | ✓ |  |  |  |
| D_CU | VARCHAR2(100) | ✓ |  |  |  |
| D_DEST | VARCHAR2(100) | ✓ |  |  |  |
| D_DEVTR | VARCHAR2(100) | ✓ |  |  |  |
| D_DIM2 | VARCHAR2(100) | ✓ |  |  |  |
| D_DIM3 | VARCHAR2(100) | ✓ |  |  |  |
| D_DIM4 | VARCHAR2(100) | ✓ |  |  |  |
| D_DP | VARCHAR2(100) | ✓ |  |  |  |
| D_FL | VARCHAR2(100) | ✓ |  |  |  |
| D_GO | VARCHAR2(100) | ✓ |  |  |  |
| D_LE | VARCHAR2(100) | ✓ |  |  |  |
| D_NU | VARCHAR2(100) | ✓ |  |  |  |
| D_ORU | VARCHAR2(100) | ✓ |  |  |  |
| D_PE | VARCHAR2(100) | ✓ |  |  |  |
| D_RU | VARCHAR2(100) | ✓ |  |  |  |
| D_SAJOB | VARCHAR2(100) | ✓ |  |  |  |
| D_T1 | VARCHAR2(100) | ✓ |  |  |  |
| D_T2 | VARCHAR2(100) | ✓ |  |  |  |
| D_TO | VARCHAR2(100) | ✓ |  |  |  |
| P_AMOUNT | NUMBER | ✓ |  |  |  |
| P_COMMENT | VARCHAR2(100) | ✓ |  |  |  |
| ANIO | NUMBER | ✓ |  |  |  |
| MES | NUMBER | ✓ |  |  |  |
| COD_CONCEPTO | NUMBER | ✓ |  |  |  |
| COD_PAIS | NUMBER | ✓ |  |  |  |
| COD_GRUPO | NUMBER | ✓ |  |  |  |
| COD_NETWORK | NUMBER | ✓ |  |  |  |
| COD_PURE_PLAYER | NUMBER | ✓ |  |  |  |

### ZZZ_DASHBOARD_PPTO_COMP

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| ANIO | NUMBER | ✓ |  |  |  |
| MES | DATE | ✓ |  |  |  |
| COD_VERSION | NUMBER | ✓ |  |  |  |
| DES_VERSION | VARCHAR2(40) | ✓ |  |  |  |
| SBA | NUMBER | ✓ |  |  |  |
| BONUS | NUMBER | ✓ |  |  |  |
| FTE | NUMBER | ✓ |  |  |  |

### ZZZ_MAPEO_MEDIO_PRESUPUESTO

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| MEDIO | VARCHAR2(50) |  | PK |  |  |
| TIPO_PRESUPUESTO | VARCHAR2(50) |  | PK |  |  |
| ALCANCE | VARCHAR2(50) | ✓ |  |  |  |
| OBJETIVO | VARCHAR2(50) | ✓ |  |  |  |
| DISCIPLINA | VARCHAR2(100) | ✓ |  |  |  |
| TIPO_COMPRA | VARCHAR2(50) | ✓ |  |  |  |
| DIVERSIFIED | VARCHAR2(50) | ✓ |  |  |  |

**Índices:**

- `PK_ZZZ_MAPEO_MEDIO_PRESUPUESTO` (UNIQUE) — MEDIO, TIPO_PRESUPUESTO

### ZZZ_PPT_PREVISION_CONDICIONES_REAL_23012026_V1

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PREVISION_CONDICION_REAL | NUMBER |  |  |  |  |
| COD_VERSION | NUMBER |  |  |  |  |
| COD_NETWORK | NUMBER |  |  |  |  |
| MES | NUMBER(2,0) |  |  |  |  |
| COD_PAIS | NUMBER |  |  |  |  |
| COD_GRUPO | NUMBER |  |  |  |  |
| COD_MEDIO | NUMBER |  |  |  |  |
| COD_EDITORIAL_COMERCIAL | NUMBER |  |  |  |  |
| COD_ALCANCE | NUMBER |  |  |  |  |
| COD_OBJETIVO | NUMBER |  |  |  |  |
| COD_DISCIPLINA | NUMBER |  |  |  |  |
| COD_DIVERSIFIED | NUMBER |  |  |  |  |
| COD_TIPO_COMPRA | NUMBER |  |  |  |  |
| IND_ACUERDO | NUMBER(1,0) |  |  |  |  |
| IND_INTERCO | NUMBER(1,0) |  |  |  |  |
| IND_MEDIO_0 | NUMBER(1,0) |  |  |  |  |
| ANIO | NUMBER(4,0) |  |  |  |  |
| COD_TIPO_VERSION | NUMBER |  |  |  |  |
| IMP_SAG | NUMBER |  |  |  |  |
| IMP_SAG_FIJO | NUMBER |  |  |  |  |
| IMP_DEV | NUMBER |  |  |  |  |
| IMP_DEV_FIJO | NUMBER |  |  |  |  |
| IMP_MANPOWER | NUMBER |  |  |  |  |
| IMP_MANPOWER_FIJO | NUMBER |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |
| F_MODIFICACION | DATE |  |  |  |  |

### ZZZ_PPT_PREVISIONES_23012025_V1

| Columna | Tipo | Nulable | PK | FK | Descripción |
|---|---|:---:|:---:|:---:|---|
| COD_PREVISION | NUMBER |  |  |  |  |
| COD_VERSION | NUMBER |  |  |  |  |
| COD_NETWORK | NUMBER |  |  |  |  |
| MES | NUMBER(2,0) |  |  |  |  |
| COD_PAIS | NUMBER |  |  |  |  |
| COD_GRUPO | NUMBER |  |  |  |  |
| COD_MEDIO | NUMBER |  |  |  |  |
| COD_EDITORIAL_COMERCIAL | NUMBER |  |  |  |  |
| COD_ALCANCE | NUMBER |  |  |  |  |
| COD_OBJETIVO | NUMBER |  |  |  |  |
| COD_DISCIPLINA | NUMBER |  |  |  |  |
| COD_DIVERSIFIED | NUMBER |  |  |  |  |
| COD_TIPO_COMPRA | NUMBER |  |  |  |  |
| IND_ACUERDO | NUMBER(1,0) |  |  |  |  |
| ANIO | NUMBER(4,0) |  |  |  |  |
| COD_TIPO_VERSION | NUMBER |  |  |  |  |
| IMP_NETO_VENTA | NUMBER |  |  |  |  |
| IMP_NETO_COMPRA | NUMBER |  |  |  |  |
| IMP_RAPPEL_NO_CMP | NUMBER |  |  |  |  |
| COD_USUARIO_MODIFICACION | NUMBER |  |  |  |  |
| F_MODIFICACION | DATE |  |  |  |  |
| IND_INTERCO | NUMBER(1,0) |  |  |  |  |
| IND_MEDIO_0 | NUMBER(1,0) |  |  |  |  |

## Vistas

- `AN_DIARIO_CONSOLIDA_INVERSION`
- `HMES_TIPOS_DE_GASTOS`
- `MINER_PPTOS_GL_EMPLEADO`
- `OE_CIERRE_CIA_REPARTO_BK`
- `TMP_TRASPASO_CIERRE`
- `TS_JERARQUIA_HR`
- `TS_USUARIO_JERARQUIA`
- `TS_USUARIO_JERARQUIA_OLD`
- `V_AN_ALL_EMPLEADO_VIRTUAL`
- `V_AN_CDC_STAT_OF`
- `V_AN_CIERRE_CIA_PCT_REPARTO`
- `V_AN_CIERRE_COMPANIA_IMP_PCT`
- `V_AN_DIARIO_EMP_PORCENTAJES`
- `V_AN_EMPLEADO`
- `V_AN_EMPLEADO_DATA`
- `V_AN_EMPLEADO_ESPECIALIDAD`
- `V_AN_EMPLEADO_IMPUTA_PCT`
- `V_AN_EMPLEADO_OFICINA`
- `V_AN_EMPLEADO_REPARTO`
- `V_AN_EMPLEADO_TIPO_IMPUTA_TS`
- `V_AN_EMPLEADO_VERTICAL`
- `V_AN_EMPLEADO_VIRTUAL`
- `V_AN_ESPECIALIDAD_MAP`
- `V_AN_VERTICAL_MAP`
- `V_AN_VIRTUAL_MAP`
- `V_COMPANIA_CDC_MAPPING`
- `V_COMPANIA_TIPO_NETWORK`
- `V_CONS_CIERRE`
- `V_CONS_CIERRE_INTERNET_DISTR`
- `V_CONS_REAL`
- `V_CONS_REAL_INTERNET_DISTR`
- `V_CONS_VERSION`
- `V_CONS_VERSION_HVM0_DISTR`
- `V_CONS_VERSION_INTERNET_DISTR`
- `V_CONS_VERSION_OFI_TOTAL_MES`
- `V_EMPLEADO_USUARIO_MMS`
- `V_EMPLEADOS_COPIA`
- `V_GRUPO_COMPANIA_POR_CLIENTE`
- `V_HJOB_VERSION`
- `V_MAESTRO_DESCRIPCION`
- `V_MEDIO_PPTO`
- `V_MINER_COMPENSA_CIERRE`
- `V_MINER_COMPENSA_CIERRE_ACUM`
- `V_MINER_COMPENSA_DIARIO_ACUM`
- `V_MINER_COMPENSACION_DIARIO`
- `V_MINER_DIF_CIERRE_GL`
- `V_MINER_EMPLEADOS`
- `V_MINER_IMPUTACIONES_CIERRE`
- `V_MINER_IMPUTACIONES_CIERRE_AC`
- `V_MINER_IMPUTACIONES_DIARIO`
- `V_MINER_IMPUTACIONES_DIARIO_NEW1`
- `V_MINER_MANAGEMENT`
- `V_MINER_OTHER_EXP_DETALLE`
- `V_MINER_OTHER_EXP_DIARIO_FACT`
- `V_MINER_OTHER_EXPENSES_CIERRE`
- `V_MINER_PARKING_EMPLEADOS`
- `V_MINER_PLANTILLAS`
- `V_MINER_RENTING_VEHICULOS`
- `V_MINER_STATUS_IMPUTACION`
- `V_MINER_TELEFONOS`
- `V_MINER_TS_CONTRIB`
- `V_MINER_TS_FTE`
- `V_MINER_TS_FTE_MES`
- `V_MINER_TS_FTE_REDUCIDO`
- `V_MINER_TS_GENERAL`
- `V_MINER_TS_IMPUTACIONES`
- `V_OE_ESPECIALIDAD`
- `V_OE_IMPUTA_PCT`
- `V_OE_OFICINA`
- `V_OE_VERTICAL`
- `V_OE_VIRTUAL`
- `V_OTHER_EXPENSES_DIARIO`
- `V_PLN_ESPECIALIDAD`
- `V_PLN_OFICINA`
- `V_PLN_VERTICAL`
- `V_PLN_VIRTUAL`
- `V_PPT_CONDICION_MEDIO`
- `V_PPT_CONDICION_MEDIO_EXCEP`
- `V_PPT_MAP_CLIENTES_INTCOM_MAGN`
- `V_PPT_MAP_CLIENTES_MAGNITUDE`
- `V_PPT_MINER_TRANSFER`
- `V_PPT_PREV_MAGNITUDE`
- `V_PPT_PREV_MAGNITUDE_PPLAYER`
- `V_PPT_PREVISIONES`
- `V_PURCHASING_DIF_OF_OE`
- `V_TS_CALC_EMP_PCT_GRUPO_AGRUP`
- `V_TS_GRUPO_GRUPO_VIRTUAL`
- `V_TS_GRUPOS_ENTORNO`
- `V_TS_GRUPOS_ENTORNO_EMPLEADO`
- `V_TS_GRUPOS_VERSION_EMPLEADO`
- `V_TS_IMPUTACION_ALL`
- `VERSIONES_PRESUPUESTO`
- `VXXHM_ES_EMPLEADOS_ACCESO_AP`
- `VXXHM_ES_EMPLEADOS_ACCESO_PO`
- `ZZZ_PRUEBA_CAMBIADA_1`
- `ZZZ_V_AN_EMPLEADO_TIPO_IMPUTACION_OLD`
- `ZZZ_V_PPT_MINER_TRANSFER_TUNNING`


