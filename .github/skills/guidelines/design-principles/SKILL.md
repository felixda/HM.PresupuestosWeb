---
name: design-principles
description: Este skill debe usarse al escribir o revisar código C#, al refactorizar, o cuando el usuario pregunta sobre "nomenclatura", "funciones", "clases", "comentarios", "manejo de errores" o "diseño de código".
---

# Principios de Diseño

Estándares de diseño de código que cubren nomenclatura, funciones, clases/módulos, comentarios y manejo de errores. Aplican a los proyectos `HM.Presupuestos.Domain`, `HM.Presupuestos.Application`, `HM.Presupuestos.Infrastructure` y `HM.Presupuestos.Web`.

## Nomenclatura

- Nombres descriptivos en español del dominio; abreviaciones solo en lambdas de ámbito reducido (`x =>`, `r =>`)
- Prefijo `I` obligatorio en interfaces (`IVersionesService`, `ICondicionesRepository`)
- Prefijo `_` obligatorio en campos privados (`_listVersion`, `_versionesRepository`)
- Nombres de clase: sustantivos PascalCase (`IndicadoresService`, `CondicionFiltro`)
- Nombres de método: verbos PascalCase (`ObtenerVersiones`, `EliminarVigencia`)
- Sufijos permitidos: `Service`, `Repository`, `Dto`, `ViewModel`, `Filtro`, `Param`, `Tests`
- Constantes de dominio: ALL_CAPS (`NUMERO_ESTADOS`, `RULE_ADMIN`)
- Valores de enum: PascalCase (`SinCambios`, `Modificado`, `Nuevo`)
- Métodos async: sufijo `Async` siempre (excepción: event handlers de DevExpress que devuelven `void`)
- Sin información de tipo en el nombre; el IDE ya lo muestra
- Un concepto, un nombre; sin sinónimos ni alias
- Preferir enums sobre literales para conjuntos fijos de valores

Para reglas completas con ejemplos, ver `references/naming.md`.

## Funciones / Métodos

1. **Responsabilidad única**: cada método hace exactamente lo que indica su nombre
2. **Nomenclatura impecable**: verbos en español que describen con precisión la acción; sufijo `Async` en todos los métodos `Task`/`Task<T>`
3. **Aridad mínima**: 0-3 parámetros; si se supera, agrupar en clase de filtro (`CondicionFiltro`, `SobreprimaFiltro`)
4. **Sin flags booleanos**: preferir parámetros `int?` o `string?` para variantes; o métodos específicos cuando el comportamiento diverge completamente
5. **Guard clauses**: salir temprano en casos límite; reducir indentación y complejidad ciclomática
6. **Separar flujo de control y lógica de negocio**: extraer iteraciones y ramas a métodos privados (`TratarCondicion`, `TratarExcepcion`)
7. **Estilo declarativo**: usar LINQ (`.Where()`, `.Select()`, `.Any()`, `.FirstOrDefault()`) cuando mejora la legibilidad; preferir `[.. lista.Where(...)]` para materializaciones
8. **CQS**: comandos mutan estado y retornan `void`/`Task`; consultas retornan valor y no mutan estado
9. **Sin comentarios en métodos** (salvo `/// <summary>` en métodos públicos de interfaz, o cuando la complejidad lo requiera)
10. **Constantes cerca del uso**: `const` local o `private const` de clase; excepción: constantes de dominio compartidas entre capas van en `Constantes.cs` en Domain
11. **Sin null en retornos de listas**: devolver siempre `List<T>` vacía; usar `T?` para valores escalares opcionales

Para reglas completas con ejemplos de código, ver `references/functions.md`.

## Clases y Módulos

1. **Constructor primario con asignación explícita**: `public class XxxService(ILogger logger, IRepo repo)` con campos `private readonly IXxx _xxx = xxx;`
2. **Organización con `#region`**: agrupar miembros por responsabilidad (Propiedades, Inicialización, Grid-Eventos, Grid-CRUD, etc.)
3. **Encapsulación**: campos y métodos auxiliares `private`; solo exponer lo necesario en la interfaz
4. **Ley de Demeter / Tell, Don't Ask**: evitar cadenas largas de acceso; delegar en el objeto que tiene el dato
5. **Modelos ricos**: las entidades de dominio encapsulan comportamiento (setters con redondeo, propiedades computadas, `IconoCssClass`)
6. **Objetos completos en construcción**: sin inicialización parcial
7. **Tipos específicos de dominio**: usar `ValidacionException(CampoErrorValidacion, valor)` en lugar de excepciones genéricas; usar enums en lugar de enteros mágicos
8. **Composición sobre herencia**: las páginas heredan de `Context` o `ContextProtegido`; no usar herencia para reutilizar lógica de negocio

Para reglas completas con ejemplos de código, ver `references/classes-modules.md`.

## Comentarios y Formato

- El código debe ser autoexplicativo; los comentarios indican código poco claro
- Solo comentar el POR QUÉ, nunca el QUÉ
- `/// <summary>` solo en métodos públicos de interfaz o lógica especialmente compleja
- No dejar código comentado; para eso existe el control de versiones
- `#region` para agrupar bloques lógicos en páginas y servicios grandes
- Sin líneas en blanco dentro de métodos; líneas en blanco solo entre métodos

## Manejo de Errores

| Tipo | Clase | Cuándo |
|---|---|---|
| Validación de negocio | `ValidacionException(CampoErrorValidacion, valor)` | Valor duplicado (Descripción, Orden, BitAnd) |
| Estado inválido | `InvalidOperationException` | Acceso sin usuario autenticado; estado de objeto inválido |
| Error técnico | `Exception` | Fallo de BD, red, timeout — dejar burbujear |

- Lanzar `ValidacionException` para violaciones de unicidad; capturar por separado en la página para dar mensaje específico al usuario
- En transacciones: `catch { RollbackAsync(); throw; }` — nunca silenciar errores de transacción
- Errores de auditoría (post-commit): capturar, loguear con `_logger.LogError`, **no relanzar**
- `LogAccionesService` absorbe sus propios errores internamente; nunca llega al llamador
- Envolver operaciones async en páginas con `EjecutarAsync(...)` (overlay automático + manejo centralizado)
- Devolver listas vacías para "sin resultados"; lanzar para estados inválidos

Para el patrón completo con ejemplos de código, ver `references/error-handling.md`.

## Reglas No Negociables

- Nunca nombres de variable genéricos (`data`, `info`, `obj`, `temp`)
- Nunca flags booleanos que cambien el comportamiento de un método
- Nunca `null` como retorno de una lista
- Nunca silenciar errores de transacción (siempre `throw` tras `RollbackAsync`)
- Nunca acceder a `OnInitializedAsync` para lógica que depende del usuario; usar `OnUsuarioDisponibleAsync()`
- Siempre sufijo `Async` en métodos `Task`/`Task<T>`
- Siempre prefijo `_` en campos privados
- Siempre prefijo `I` en interfaces
- Siempre CQS: comandos mutan (`void`/`Task`), consultas retornan (sin mutación)
- Siempre composición sobre herencia para lógica de negocio
