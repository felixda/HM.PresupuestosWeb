# Dominio de Negocio — PRESUPUESTOS

> Generado automáticamente por Sync-OracleSchema.ps1  
> Última actualización: 2026-06-04 20:30:32

## Agrupación por dominio funcional

Los dominios se infieren del prefijo de nombre de tabla (prefijo antes del primer _).

### Dominio: AN (37 tablas)

| Tabla | Descripción |
|---|---|
| AN_CI_CIA_PCT_REPARTO_22062018 | BACKUP cierre marzo antes nueva carga. Eliminar cuando sea posible |
| AN_CIERRE_CIA_PCT_REPARTO | Solo hasta el mes de cierre |
| AN_CIERRE_CONSOLIDA_INVERSION | Tabla temporal donde se precalculan los datos por virtual, vertical y especialidad para su posterior tratamiento en los cálculos de los porcentajes |
| AN_CIERRE_EMPLEADO_PCT_BACKUP | Tabla que guardará un backup cuando se vuelve a recargar un cierre. |
| AN_CIERRE_EMPLEADO_PORCENTAJES |  |
| AN_DIARIO_CONSOLIDA_INV_GRUPO | Tabla temporal donde se precalculan los datos por virtual, vertical, especialidad y grupo para su posterior tratamiento en los cálculos de los porcentajes |
| AN_DIARIO_EMPLEADO_PORCENTAJES |  |
| AN_EMPLEADO_ACCESO_AP | Tabla que se carga desde financials con los empleados que pueden acceder a Payables |
| AN_EMPLEADO_ACCESO_GL | Tabla que se carga desde financials con los empleados que pueden acceder a General Ledger |
| AN_EMPLEADO_ACCESO_PO | Tabla que se carga desde financials con los empleados que pueden acceder a purchasing |
| AN_EMPLEADO_DIST_IMPUTA | Dentro de cada empleado para España puede tener varias imputaciones distintas con porcentaje |
| AN_EMPLEADO_IMPUTA | Tabla maestra de las imputaciones por empleado |
| AN_EMPLEADO_IMPUTA_ESPE | Imputación por especialidad no entra el nombre completo. |
| AN_EMPLEADO_IMPUTA_OFICINA |  |
| AN_EMPLEADO_IMPUTA_PORCENTAJE | Imputaciones por centro de coste y porcentaje (Global, services ..) |
| AN_EMPLEADO_IMPUTA_VERTICAL |  |
| AN_EMPLEADO_IMPUTA_VIRTUAL |  |
| AN_EMPLEADO_IMPUTA_VIRTUAL_PUESTA_PROD |  |
| AN_EMPLEADO_TIPO_AGRUPA_PCT | Tabla que contiene la distrubución por tipo agrupacion porcentual |
| AN_EMPLEADO_TIPO_IMPUTA_EXCEP | Esta tabla contiene excepciones a la asignacion de tipo de imputacion por empleado especificada por RRHH para su uso en TimeSheet.  Son excepciones a la vista V_AN_EMPLEADO_TIPO_IMPUTACION Usada para Controllers. |
| AN_ESPECIALIDAD |  |
| AN_MAP_COMPANIA_CDC_REPARTO | Utilizada para la refacturación que dado compañia origen y compañía destino DE ESPAÑA nos da el centro de coste a repartir total compensacion. 
En el caso de los GLOBALES y SERVICES, directamente se pone la destino.
Hay que tener en cuenta que en la definicion de CDCs de OF la compañia del centro de coste es la origen. |
| AN_MAP_COMPANIA_CDC_REPARTO_BK_17092025 |  |
| AN_MAP_COMPANIA_CDC_REPARTO_BK_CIERRE_2025 |  |
| AN_MAP_COMPANIA_CDC_REPARTO_BK_CIERRE2024 |  |
| AN_MAP_COMPANIA_CDC_REPARTO_PRUEBA_REFACT_2025 |  |
| AN_MAP_CUENTA_OF_CONCEPTO |  |
| AN_MAP_ESPECIALIDAD | Tabla que mapea las especialidades de RRHH con las especialidades de Presupuestos + ONOFF |
| AN_MAP_OFICINA | Mapeamos las oficinas de analitacas con las oficinas de MMS |
| AN_MAP_VERTICAL | Tabla de mapeo de una vertical de rrhh con los medios de MMS. un medio solo puede estar en una vertical |
| AN_MAP_VIRTUAL | Tabla que relaciona las virtual con los entornos de presupuestos. Un entorno solo puede estar en una virtual. |
| AN_OFICINA | Tabla que contiene las oficinas. De momento es para Proximia y estará relacionadas conlas oficinas de MMS (Inicialmente  Proximia) |
| AN_STATUS_IMPUTACION_CIERRE | Tabla que se carga en la carga del cierre para tener un foto del estado de las imputaciones. |
| AN_TIPO_AGRUPACION_CDC | Global, Services, etc |
| AN_TIPO_ORGANIZACION |  |
| AN_VERTICAL |  |
| AN_VIRTUAL |  |

### Dominio: AUDIT (5 tablas)

| Tabla | Descripción |
|---|---|
| AUDIT_TS_GRUPO_NO_DISTR_VIRT |  |
| AUDIT_TS_IMPUTA |  |
| AUDIT_TS_IMPUTA_GRUPO |  |
| AUDIT_TS_IMPUTA_GRUPO_ENTORNO |  |
| AUDIT_TS_PY_EMPLEADO | Tabla que audita si un empleado cambio tanto de indicador de virtual o no o de tipo network |

### Dominio: BACK (7 tablas)

| Tabla | Descripción |
|---|---|
| BACK_AN_DIARIO_EMPLEADO_PORCE |  |
| BACK_EMP_PCT_GR_ENT_NO_INV | TABLA CON LOS GRUPOS PRECALCULADOS. GRUPO ENTORNO SIN INVERSION EN ORIGEN |
| BACK_TA_GRUPO_ENTORNO |  |
| BACK_TS_CALC_EMP_PCT_GRUPO | TABLA CON LOS GRUPOS PRECALCULADOS. TANTO DE ENTORNO CON INVERSION COMO DE GRUPO CON/SIN INVERSION |
| BACK_TS_CONSOLIDADO |  |
| BACK_TS_IMPUTA |  |
| BACK_TS_IMPUTA_GRUPO |  |

### Dominio: GENERAL (1 tablas)

| Tabla | Descripción |
|---|---|
| DBTOOLS$MCP_LOG |  |

### Dominio: GRUPO (1 tablas)

| Tabla | Descripción |
|---|---|
| GRUPO_NAME_CC |  |

### Dominio: LOG (3 tablas)

| Tabla | Descripción |
|---|---|
| LOG_IMPUTA_OTHER_EXPENSES_TMP |  |
| LOG_OTHER_EXPENSES |  |
| LOG_PROCESO | Tabla para guardar el log de los procesos que necesitan log |

### Dominio: MAIL (1 tablas)

| Tabla | Descripción |
|---|---|
| MAIL_CONFIG |  |

### Dominio: MAP (1 tablas)

| Tabla | Descripción |
|---|---|
| MAP_NMD_TO_MEDIO_NETWORK_TIPO |  |

### Dominio: MGN (7 tablas)

| Tabla | Descripción |
|---|---|
| MGN_INFORME | Entidad que guarda los informes que se reportan a Magnitude y los conceptos que se reportan en ese informe |
| MGN_INFORME_COLUMNA | Columnas que se reportan en los informes de MAGNITUDE.  |
| MGN_INFORME_NETWORK | Indica qué networks salen de cada informe y las columnas que hay que reportar |
| MGN_NETWORK_PPLAYER | Relación entre Network y Pure Player que se reportan en Magnitude |
| MGN_PARAM |  |
| MGN_PARAM_NETWORK | Parametrización de todos los conceptos y columnas que dependen de la network |
| MGN_PARAM_NETWORK_PPLAYER | Parametrización de todos los conceptos y columnas que dependen de la network + Pure Player |

### Dominio: MINER (2 tablas)

| Tabla | Descripción |
|---|---|
| MINER_OTHER_EXP_2022_CODDETALLE |  |
| MINER_OTHER_EXP_2022_CODDETALLE_02 |  |

### Dominio: NAME (1 tablas)

| Tabla | Descripción |
|---|---|
| NAME_CC_GRUPO_NAME_CC |  |

### Dominio: OE (17 tablas)

| Tabla | Descripción |
|---|---|
| OE_CIERRE_CIA_REPARTO | Solo hasta el mes de cierre |
| OE_CIERRE_EMPLEADO_PCT_BACKUP |  |
| OE_CIERRE_EMPLEADO_PORCENTAJES |  |
| OE_CONF_PROV_GET_EMPLEADO | Tabla que se utiiliza en código para saber donde buscar el código de empleado dado un proveedor  |
| OE_DIARIO_EMPLEADO_PORCENTAJES |  |
| OE_DIARIO_EMPLEADO_PORCENTAJES_BK |  |
| OE_DIST_IMPUTACION | Dentro de cada empleado para España puede tener varias imputaciones distintas con porcentaje |
| OE_IMPUTA | Tabla maestra de las imputaciones por empleado (Origen OF) |
| OE_IMPUTA_ESPECIALIDAD | Imputación por especialidad no entra el nombre completo. |
| OE_IMPUTA_OFICINA |  |
| OE_IMPUTA_PORCENTAJE | Imputaciones destino por centro de coste y porcentaje (Global, services ..) |
| OE_IMPUTA_VERTICAL |  |
| OE_IMPUTA_VIRTUAL |  |
| OE_ORIGEN_GASTO | Tabla que nos dice que tipo gasto es: Gasto, Proveedores, etc. |
| OE_OTHER_EXPENSES_HISTORICO |  |
| OE_TIPO_AGRUPA_PORCENTAJE | Tabla que contiene la distrubución por tipo agrupacion porcentual  |
| OE_TIPO_CIERRE | Tipo de cierre (nómina,other expenses, etc) |

### Dominio: OF (2 tablas)

| Tabla | Descripción |
|---|---|
| OF_ORIGEN_ASIENTO | Tabla para cargar orígenes de asientos |
| OF_PROVEEDOR | Tabla proveedores OF |

### Dominio: PLN (9 tablas)

| Tabla | Descripción |
|---|---|
| PLN_DIST_IMPUTACION | Dentro de cada empleado para España puede tener varias imputaciones distintas con porcentaje |
| PLN_IMPUTA | Tabla maestra de la imputación de la plantilla |
| PLN_IMPUTA_ESPECIALIDAD | Imputación por especialidad no entra el nombre completo. |
| PLN_IMPUTA_OFICINA |  |
| PLN_IMPUTA_PORCENTAJE | Imputaciones destino por centro de coste y porcentaje (Global, services ..) |
| PLN_IMPUTA_VERTICAL |  |
| PLN_IMPUTA_VIRTUAL |  |
| PLN_PLANTILLA_PREDETERMINADA | Tabla que nos indica que plantilla es predeterminada por compania, origen y proveedor,pudiendo ser cualquier combinacion pero sin repetirse, ejemplo por solo compañía, solo por compañia origen, solo origen, etc. Se añade ind_valija a la pk puesto que puede ser una misma conbinación, uno para valija y otra no. No tiene PK y sí UQ porque puede haber nulos dentro del índice único |
| PLN_TIPO_AGRUPA_PORCENTAJE | Tabla que contiene la distrubución por tipo agrupacion porcentual  |

### Dominio: PPT (59 tablas)

| Tabla | Descripción |
|---|---|
| PPT_ACCION_ETIQUETA | Maestro de etiquetas que pueden venir en los ficheros de parámetros que se van a guardar en los log |
| PPT_ACCION_LOG | Entidad que guarda el proceso a auditar, quién lo ha llamado, fechas de inicio y de finalización y el json con los parámetros |
| PPT_BUSINESS_MAGNITUDE |  |
| PPT_CHANNEL | Maestro de channels para los reportes de Itracker |
| PPT_CHANNEL_DISCIPLINA_IN | Disciplinas que componen ese channel. Si no hay, se cogen todos |
| PPT_CHANNEL_DISCIPLINA_OUT | Channels que se excluyen de un publisher. Si no hay registros, no se excluye nada |
| PPT_CHANNEL_MEDIO_IN | Medios que componen ese channel. Si no hay, se cogen todos |
| PPT_CHANNEL_MEDIO_OUT | Medios que se excluyen de ese channel. Si no hay, no se excluye nada |
| PPT_CHANNEL_OBJETIVO_IN | Objetivos que componen ese channel. Si no hay, se cogen todos |
| PPT_CHANNEL_OBJETIVO_OUT | Objetivos que se excluyen de ese channel. Si no hay, no se excluye nada |
| PPT_CHANNEL_TIPO_COMPRA_IN | Tipos de Compra que componen ese channel. Si no hay, se cogen todos |
| PPT_CHANNEL_TIPO_COMPRA_OUT | Tipos de Compra que se excluyen de un channel. Si no hay registros, no se excluye nada |
| PPT_CONCEPTOS_CONDICIONES |  |
| PPT_CONCEPTOS_CONDICIONES_PAIS |  |
| PPT_CONCEPTOS_PPTO_NETWORK | Entidad para relacionar los conceptos de presupuestos con las Network y asignarles su código de Magnitude |
| PPT_CONCEPTOS_PRESUPUESTOS | Almacena los conceptos de los presupuestos |
| PPT_CONCEPTOS_SOBREPRIMA |  |
| PPT_CONCEPTOS_SOBREPRIMAS_PAIS |  |
| PPT_COND_EXC_ALCANCE | Guarda el código de alcance asociado al medio dentro de una jerarquía de excepciones. Solo puede haber un registro |
| PPT_COND_EXC_DISCIPLINA | Guarda el código de disciplina asociado al medio dentro de una jerarquía de excepciones. Solo puede haber un registro |
| PPT_COND_EXC_DISCIPLINA_GRUPO | Guarda el código de disciplina Grupo asociado al medio dentro de una jerarquía de excepciones. Solo puede haber un registro |
| PPT_COND_EXC_DIVERSIFIED | Guarda el código de diversified asociado al medio dentro de una jerarquía de excepciones. Solo puede haber un registro |
| PPT_COND_EXC_OBJETIVO | Guarda el código de objetivo asociado al medio dentro de una jerarquía de excepciones. Solo puede haber un registro |
| PPT_COND_EXC_TIPO_COMPRA | Guarda el código de tipo de compra asociado al medio dentro de una jerarquía de excepciones. Solo puede haber un registro |
| PPT_COND_EXC_TIPO_DISCIPLINA | Guarda el código de Tipo de Disciplina asociado al medio dentro de una jerarquía de excepciones. Solo puede haber un registro |
| PPT_CONDICION_MEDIO | Guarda los porcentajes por medio de la vigencia con la que está relacionada. La del propio medio, sin nada más,  tendrá el número de jerarquía = 0, mientras que la del medio combinada con otros objetos tendrá jerarquía > 0, mantenida por el usuario. Estos objetos tienen sus tablas personalizadas para guardar qué objeto es. |
| PPT_CONDICION_VIGENCIA | Vigencias en un año (versión) / concepto / network / pais / grupo / indicador acuerdo. Los meses desde y hasta no se pueden solapar en el mismo año  |
| PPT_CONFIGURACION | Existe un Trgiger que valida que el contenido de VALOR es acorde al tipo de dato de VALOR |
| PPT_ESTADOS_VERSIONES |  |
| PPT_ESTADOS_VERSIONES_IDIOMAS |  |
| PPT_PREVISION_CONDICIONES_ABS |  |
| PPT_PREVISION_CONDICIONES_FIJO |  |
| PPT_PREVISION_CONDICIONES_REAL_OLD |  |
| PPT_PREVISION_SOBREPRIMAS_ABS |  |
| PPT_PREVISIONES |  |
| PPT_PREVISIONES_AJUSTES |  |
| PPT_PREVISIONES_AJUSTES_AUD | Tabla de auditoría para PPT_PREVISIONES_AJUSTES |
| PPT_PREVISIONES_AUD | Tabla de auditoría para PPT_PREVISIONES |
| PPT_PUBLISHER | Maestro de publishers para los reportes de Itracker |
| PPT_PUBLISHER_AGRP_COMER_IN | Agrupaciones Comerciales que componen ese publisher. Si no hay, se cogen todos.  Las agrupaciones comerciales están en un CATEGORIZER y corresponden a CATEGORIA_TIPO.COD_CATEGORIA = 19   AND CATEGORIA_TIPO.COD_CATEGORIA_TIPO = 82 |
| PPT_PUBLISHER_AGRP_COMER_OUT | Agrupaciones Comerciales que se excluyen de este publisher. Si no hay, no se excluye nada.  Las agrupaciones comerciales están en un CATEGORIZER y corresponden a CATEGORIA_TIPO.COD_CATEGORIA = 19   AND CATEGORIA_TIPO.COD_CATEGORIA_TIPO = 82 |
| PPT_PUBLISHER_DISCIP_GRUPO_IN | Disciplinas Grupo que componen ese publisher. Si no hay, se cogen todos |
| PPT_PUBLISHER_DISCIP_GRUPO_OUT | Disciplinas Grupo que se excluyen de un publisher. Si no hay registros, no se excluye nada |
| PPT_PUBLISHER_DISCIPLINA_IN | Disciplinas que componen ese publisher. Si no hay, se cogen todos |
| PPT_PUBLISHER_DISCIPLINA_OUT | Disciplinas que se excluyen de un publisher. Si no hay registros, no se excluye nada |
| PPT_PUBLISHER_EDIT_COMER_IN | Editoriales comerciales que componen ese publisher. Si no hay, se cogen todos |
| PPT_PUBLISHER_EDIT_COMER_OUT | Editoriales Comerciales que se excluyen de un publisher. Si no hay registros, no se excluye nada |
| PPT_PUBLISHER_MEDIO_IN | Medios que componen ese publisher. Si no hay, se cogen todos |
| PPT_PUBLISHER_MEDIO_OUT | Medios que se excluyen de un publisher. Si no hay registros, no se excluye nada |
| PPT_PUBLISHER_PLATAFORMA_IN | Plataformas (DSP) que componen ese publisher. Si no hay, se cogen todos |
| PPT_PUBLISHER_PLATAFORMA_OUT | Plataformas (DSP) que se excluyen de un publisher. Si no hay registros, no se excluye nada |
| PPT_PURE_PLAYER |  |
| PPT_PURE_PLAYER_DISCIPLINA |  |
| PPT_PURE_PLAYER_DIVERSIFIED |  |
| PPT_PURE_PLAYER_OBJETIVO |  |
| PPT_PURE_PLAYER_TIPO_COMPRA |  |
| PPT_SOBREPRIMAS_MEDIO |  |
| PPT_TIPOS_VERSIONES |  |
| PPT_VERSIONES | Tiene asociado un Trigger para forzar los bits de estado cuando es REAL.  |

### Dominio: PY (33 tablas)

| Tabla | Descripción |
|---|---|
| PY_CATEGORIA |  |
| PY_CENTRO_TRABAJO |  |
| PY_CONCEPTO_BAJA |  |
| PY_CONCEPTO_COMPENSACION |  |
| PY_CONCEPTO_NATURALEZA | Tabla que contienen todos los posibles motivos que tiene una naturaleza |
| PY_CONTROL_CIERRE |  |
| PY_DEPARTAMENTO |  |
| PY_DEPARTAMENTO_ESPECIALIDAD |  |
| PY_DEPARTAMENTO_VERTICAL |  |
| PY_EMPLEADO |  |
| PY_EMPLEADO_AUSENCIA |  |
| PY_EMPLEADO_CIERRE |  |
| PY_EMPLEADO_DETALLE | Tabla que indica datos de un empleado con fecha alta y baja. Por ejemplo la compañía, preveedor si lo tiene, etc) |
| PY_EMPLEADO_HISTORICO | Tabla con los distintos valores que ha tenido el usuaro anteriormente a la situación actual |
| PY_EMPLEADO_PREVISION |  |
| PY_EMPLEADO_PREVISION_03052022 |  |
| PY_EMPLEADO_TEORICO |  |
| PY_EMPLEADO_TEORICO_HISTORICO | Contiene los distintos valores que ha tenido en empleado antes de la situación actual |
| PY_FLAG_CIERRE |  |
| PY_GRUPO_DEPARTAMENTO | De momento solo se utiliza para el dashboard (sep 2022) |
| PY_GRUPO_NATURALEZA |  |
| PY_GRUPO_PARKING |  |
| PY_LOG_CONFIRMACION |  |
| PY_MATRICULA | Matrículas de coches  |
| PY_MATRICULA_DETALLE |  |
| PY_MOTIVO_CONCEPTO_BAJA |  |
| PY_NATURALEZA |  |
| PY_PARKING |  |
| PY_PARKING_EMPLEADO |  |
| PY_PURE_PLAYER | Realmente es "equipo". Cuando se decidió cambiar el coste de cambiar el nombre de bbdd es alto. Tabla originalmente creada para los pure players. Posiblemente cambie el significado en la aplicación al añadirse más valores que no son pure players |
| PY_TIPO_CONTRATO |  |
| PY_TMP_BAJA_EMPLEADO |  |
| PY_UBICACION |  |

### Dominio: QUEST (1 tablas)

| Tabla | Descripción |
|---|---|
| QUEST_SL_TEMP_EXPLAIN1 |  |

### Dominio: TEMP (1 tablas)

| Tabla | Descripción |
|---|---|
| TEMP_SEGURIDAD_CODIGOS | Tabla temporal que guarda los códigos, por ejemplo de GRUPO, cuando en un JSON viene un -1 en el valor de la etiqueta de un código. Cuando viene -1 hay que cambiar ese -1 por todos los códigos a los que el usuario tiene acceso. Para ello, al aplicar seguridad en PKG_VALIDACION_JSON, cambia el -1 por un -2 y guarda todos los códigos en la tabla temporal. Luego los programas, al hacer las select contra el JSON, si el valor es -2 mete un IN  (select el código de la tabla temporal) |

### Dominio: TLF (3 tablas)

| Tabla | Descripción |
|---|---|
| TLF_OPERADOR | Operadores de telefonía |
| TLF_TELEFONO | Tabla con los teléfonos que luego se asignarán a los usuarios |
| TLF_TELEFONO_EMPLEADO |  |

### Dominio: TMP (10 tablas)

| Tabla | Descripción |
|---|---|
| TMP_CIERRE | Tabla temporal para cargar los ficheros de nómina por empleado para el cuadre |
| TMP_CIERRE_AGREGADO | Tabla temporal para cargar los ficheros de nómina agregados para el cuadre (los usamos para cuadrar retros de 2018 en Septiembre) |
| TMP_DIST_IMPUTACION |  |
| TMP_EMPLEADO_AJUSTE | Tabla para parametrizar ajustes por empleado, mes y concepto |
| TMP_EMPLEADO_AUSENCIA |  |
| TMP_PPT_ITRACKER |  |
| TMP_PPT_MAGNITUDE |  |
| TMP_TS_BILLINGS |  |
| TMP_TS_COMP |  |
| TMP_TS_CONSOLIDADO |  |

### Dominio: TS (22 tablas)

| Tabla | Descripción |
|---|---|
| TS_ACCION |  |
| TS_CALC_EMP_PCT_GR_ENT_NO_INV | TABLA CON LOS GRUPOS PRECALCULADOS. GRUPO ENTORNO SIN INVERSION EN ORIGEN |
| TS_CALC_EMP_PCT_GRUPO | TABLA CON LOS GRUPOS PRECALCULADOS. TANTO DE ENTORNO CON INVERSION COMO DE GRUPO CON/SIN INVERSION |
| TS_CONFIG | Tabla de configuración para TimeSheet. por ejemplo, literales de la web, etc.  |
| TS_CONFIG_ESTADO_CELDA | Tabla que se utiliza para saber que color poner a una celda, si está bloqueada o no etc dependiendo del estado. También sirve para las leyendas que se mostrarán, por lo que tendrán un orden |
| TS_CONSOLIDADO |  |
| TS_CONSOLIDADO_BAKCUP |  |
| TS_CONSOLIDADO_INVERSION |  |
| TS_GRUPO_NO_DISTR_VIRTUAL | Grupos que no se utilizan en la distribución de billings cuando se hace por virtual. En principio solo habrá clientes principales. |
| TS_IMPUTA |  |
| TS_IMPUTA_GRUPO |  |
| TS_IMPUTA_GRUPO_ENTORNO |  |
| TS_IMPUTA_GRUPO_ENTORNO_MIG |  |
| TS_IMPUTA_GRUPO_MIG |  |
| TS_IMPUTA_MIG |  |
| TS_LOG |  |
| TS_MV_AUX_COSTE_COMP |  |
| TS_MV_AUX_COSTE_COMP_CONCEPTO_MES |  |
| TS_MV_AUX_COSTE_COMP_MES |  |
| TS_ORIGEN_CONSOLIDADO | Esta tabla indica de donde o de que forma se saca el consolidado que nos indicarán por ejemplo si es cuando no tiene inversión, o cuando no hay empleado, etc. |
| TS_VERSION_VERSION_BILLING | Tabla que relaciona las version de timesheet con la de billings. Sin no tiene nada en esta tabla será la misma |
| TS_VERSION_VERSION_COMPENSACION | Tabla que relaciona las version de timesheet con la de compensación. Sin no tiene nada en esta tabla será la misma |

### Dominio: TSGL (1 tablas)

| Tabla | Descripción |
|---|---|
| TSGL_HORAS_MES_UBICACION |  |

### Dominio: V (2 tablas)

| Tabla | Descripción |
|---|---|
| V_AN_EMPLEADO_TIPO_IMPUTACION |  |
| V_COMPANIA_CDC |  |

### Dominio: VM (5 tablas)

| Tabla | Descripción |
|---|---|
| VM_MINER_OTHER_EXP_DIARIO_FACT |  |
| VM_XXHM_ES_OTHER_EXP_AN_CIERRE |  |
| VM_XXHM_ES_OTHER_EXPENSES_AN |  |
| VM_XXHM_OE_ASIENTOS_AN |  |
| VM_XXHM_OE_FACTURAS_AN |  |

### Dominio: XXHM (1 tablas)

| Tabla | Descripción |
|---|---|
| XXHM_ES_MINER_OTHER_EXPENSES_AN_BK |  |

### Dominio: ZZ (1 tablas)

| Tabla | Descripción |
|---|---|
| ZZ_TMP_PPT_MAGNITUDE |  |

### Dominio: ZZZ (4 tablas)

| Tabla | Descripción |
|---|---|
| ZZZ_DASHBOARD_PPTO_COMP |  |
| ZZZ_MAPEO_MEDIO_PRESUPUESTO |  |
| ZZZ_PPT_PREVISION_CONDICIONES_REAL_23012026_V1 |  |
| ZZZ_PPT_PREVISIONES_23012025_V1 |  |

## Relaciones entre dominios

- AN_CIERRE_EMPLEADO_PORCENTAJES.COD_DEPARTAMENTO → PY_DEPARTAMENTO. [AN → PY]
- AN_CIERRE_EMPLEADO_PORCENTAJES.COD_PURE_PLAYER → PY_PURE_PLAYER. [AN → PY]
- AN_DIARIO_EMPLEADO_PORCENTAJES.COD_DEPARTAMENTO → PY_DEPARTAMENTO. [AN → PY]
- AN_DIARIO_EMPLEADO_PORCENTAJES.COD_PURE_PLAYER → PY_PURE_PLAYER. [AN → PY]
- AN_EMPLEADO_DIST_IMPUTA.COD_DEPARTAMENTO → PY_DEPARTAMENTO. [AN → PY]
- AN_EMPLEADO_DIST_IMPUTA.COD_PURE_PLAYER → PY_PURE_PLAYER. [AN → PY]
- AN_EMPLEADO_IMPUTA.COD_EMPLEADO → PY_EMPLEADO. [AN → PY]
- AN_EMPLEADO_IMPUTA.COD_UBICACION → PY_UBICACION. [AN → PY]
- AN_EMPLEADO_TIPO_IMPUTA_EXCEP.COD_EMPLEADO → PY_EMPLEADO. [AN → PY]
- AN_MAP_CUENTA_OF_CONCEPTO.COD_CONCEPTO_COMPENSACION → PY_CONCEPTO_COMPENSACION. [AN → PY]
- AN_VIRTUAL.COD_UBICACION → PY_UBICACION. [AN → PY]
- MGN_NETWORK_PPLAYER.COD_PURE_PLAYER → PPT_PURE_PLAYER. [MGN → PPT]
- MGN_PARAM_NETWORK_PPLAYER.COD_PURE_PLAYER → PPT_PURE_PLAYER. [MGN → PPT]
- NAME_CC_GRUPO_NAME_CC.COD_GRUPO_NAME_CC → GRUPO_NAME_CC. [NAME → GRUPO]
- OE_CIERRE_EMPLEADO_PORCENTAJES.COD_EMPLEADO → PY_EMPLEADO. [OE → PY]
- OE_CIERRE_EMPLEADO_PORCENTAJES.COD_ESPECIALIDAD → AN_ESPECIALIDAD. [OE → AN]
- OE_CIERRE_EMPLEADO_PORCENTAJES.COD_OFICINA → AN_OFICINA. [OE → AN]
- OE_CIERRE_EMPLEADO_PORCENTAJES.COD_VERTICAL → AN_VERTICAL. [OE → AN]
- OE_CIERRE_EMPLEADO_PORCENTAJES.COD_VIRTUAL → AN_VIRTUAL. [OE → AN]
- OE_DIARIO_EMPLEADO_PORCENTAJES.COD_EMPLEADO → PY_EMPLEADO. [OE → PY]
- OE_DIARIO_EMPLEADO_PORCENTAJES.COD_ESPECIALIDAD → AN_ESPECIALIDAD. [OE → AN]
- OE_DIARIO_EMPLEADO_PORCENTAJES.COD_OFICINA → AN_OFICINA. [OE → AN]
- OE_DIARIO_EMPLEADO_PORCENTAJES.COD_TIPO_AGRUPACION_CDC_DEST → AN_TIPO_AGRUPACION_CDC. [OE → AN]
- OE_DIARIO_EMPLEADO_PORCENTAJES.COD_VERTICAL → AN_VERTICAL. [OE → AN]
- OE_DIARIO_EMPLEADO_PORCENTAJES.COD_VIRTUAL → AN_VIRTUAL. [OE → AN]
- OE_IMPUTA_ESPECIALIDAD.COD_ESPECIALIDAD → AN_ESPECIALIDAD. [OE → AN]
- OE_IMPUTA_OFICINA.COD_OFICINA → AN_OFICINA. [OE → AN]
- OE_IMPUTA_VERTICAL.COD_VERTICAL → AN_VERTICAL. [OE → AN]
- OE_IMPUTA_VIRTUAL.COD_VIRTUAL → AN_VIRTUAL. [OE → AN]
- OE_IMPUTA.COD_EMPLEADO → PY_EMPLEADO. [OE → PY]
- OE_TIPO_AGRUPA_PORCENTAJE.COD_TIPO_AGRUPACION_CDC → AN_TIPO_AGRUPACION_CDC. [OE → AN]
- PLN_IMPUTA_ESPECIALIDAD.COD_ESPECIALIDAD → AN_ESPECIALIDAD. [PLN → AN]
- PLN_IMPUTA_OFICINA.COD_OFICINA → AN_OFICINA. [PLN → AN]
- PLN_IMPUTA_VERTICAL.COD_VERTICAL → AN_VERTICAL. [PLN → AN]
- PLN_IMPUTA_VIRTUAL.COD_VIRTUAL → AN_VIRTUAL. [PLN → AN]
- PLN_PLANTILLA_PREDETERMINADA.COD_ORIGEN_GASTO → OE_ORIGEN_GASTO. [PLN → OE]
- PLN_TIPO_AGRUPA_PORCENTAJE.COD_TIPO_AGRUPACION_CDC → AN_TIPO_AGRUPACION_CDC. [PLN → AN]
- PY_DEPARTAMENTO_ESPECIALIDAD.COD_ESPECIALIDAD → AN_ESPECIALIDAD. [PY → AN]
- PY_DEPARTAMENTO_VERTICAL.COD_VERTICAL → AN_VERTICAL. [PY → AN]
- TLF_TELEFONO_EMPLEADO.COD_EMPLEADO → PY_EMPLEADO. [TLF → PY]
- TS_CONSOLIDADO.COD_NATURALEZA → PY_NATURALEZA. [TS → PY]
- TS_IMPUTA.COD_EMPLEADO → PY_EMPLEADO. [TS → PY]


