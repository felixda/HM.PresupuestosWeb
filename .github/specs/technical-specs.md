# Especificaciones Técnicas — HM.Presupuestos

> Documento de referencia para la IA y el equipo.  
> Describe **cómo está construido** el sistema, no qué hace.

---

## 1. Stack Tecnológico

| Capa | Tecnología | Versión |
|---|---|---|
| Runtime | .NET | 10 |
| Frontend | Blazor Server (Interactive Server Components) | .NET 10 |
| UI Components | DevExpress Blazor | DxGrid, DxPopup, DxFormLayout, DxTreeView, DxDrawer... |
| Autenticación | Azure AD SSO | Microsoft.Identity.Web + OpenIdConnect |
| Tests E2E | Playwright + NUnit | — |
| Logging | NLog + API propia (HM.CORE) | — |
| JS interop | IJSRuntime | Blazor estándar |

---

## 2. Arquitectura — Estructura de Capas

```
HM.Presupuestos.Domain          → Entidades, enums, excepciones de dominio
HM.Presupuestos.Application     → Implementaciones de casos de uso (servicios)
HM.Presupuestos.Infrastructure  → Implementaciones, repositorios, acceso a datos
HM.Presupuestos.Web             → Blazor Server: páginas, componentes, layout
HM.Presupuestos.E2ETest         → Tests end-to-end con Playwright + NUnit
```

### Responsabilidades clave

- **Domain**: entidades puras (`Indicador`, `IdiomaIndicador`, `VersionResumen`...), enums (`CodigosMenu`, `EstadoEntidad`), excepciones (`ValidacionException`, `ExcepcionBaseDatos`)
- **Application**: interfaces e implementaciones de los casos de uso (`IIndicadoresService` / `IndicadoresService`, `IVersionesService` / `VersionesService`...). Nunca depende de Infrastructure.
  > **Nota de diseño:** En arquitectura hexagonal estricta, los puertos primarios (`IXxxService`) podrían vivir en Domain. 
  En este proyecto se ha optado por colocarlos junto a su implementación en `Application/CasosDeUso/`, siguiendo el patrón de Clean Architecture (Uncle Bob).
  Esto es un compromiso pragmático válido: `Web` sigue sin referenciar `Infrastructure`, y `Domain` permanece sin dependencias externas. 
  Los puertos secundarios (`IXxxRepository`) sí viven en `Domain/Puertos/Repositorios/` porque definen contratos que el dominio necesita conocer.
- **Infrastructure**: implementa las interfaces de Application. Llama a la API HM.CORE o base de datos.
- **Web**: consume Application vía DI. Nunca referencia Infrastructure directamente.

### Diagrama de arquitectura hexagonal

```
┌─────────────────────────────────────────────────────────┐
│  ADAPTADORES PRIMARIOS (Driving)                        │
│  HM.Presupuestos.Web  (Blazor Server)                   │
│  Pages/*.razor.cs → inyecta IService (Application)      │
└──────────────────────┬──────────────────────────────────┘
                       │ depende de ▼
┌──────────────────────▼──────────────────────────────────┐
│  APLICACIÓN                                             │
│  HM.Presupuestos.Application                            │
│  CasosDeUso/*.cs → implementan IService                 │
│  Solo depende de Domain (Puertos + Entidades)           │
└──────────────────────┬──────────────────────────────────┘
                       │ depende de ▼
┌──────────────────────▼──────────────────────────────────┐
│  DOMINIO  (núcleo)                                      │
│  HM.Presupuestos.Domain  ← SIN dependencias externas    │
│  Entidades/  ← modelos de negocio                       │
│  Puertos/    ← interfaces (IXxxRepository, IXxxService) │
└──────────────────────▲──────────────────────────────────┘
                       │ implementa ▲
┌──────────────────────┴──────────────────────────────────┐
│  ADAPTADORES SECUNDARIOS (Driven)                       │
│  HM.Presupuestos.Infrastructure                         │
│  Repositorios/*.cs → implementan IXxxRepository         │
│  Solo depende de Domain                                 │
└─────────────────────────────────────────────────────────┘
```

**Reglas de dependencia — nunca romper:**
- ❌ `Web` no referencia `Infrastructure`
- ❌ `Application` no referencia `Infrastructure` ni `Web`
- ❌ `Domain` no referencia ningún otro proyecto de la solución
- ✅ La DI (registro de servicios) vive en `Web/Program.cs` y es el único sitio que conoce todas las capas

### Regla de acceso a repositorios

**Un repositorio solo puede ser accedido por su servicio correspondiente.** Ningún servicio puede inyectar ni llamar directamente al repositorio de otro dominio. Si un servicio necesita datos de otro dominio, debe hacerlo a través del servicio de ese dominio, obtenido por inyección de dependencias en el constructor.

```csharp
// ✅ CORRECTO — CondicionesService accede solo a ICondicionesRepository (su repositorio)
// Para datos de presupuestos usa IPresupuestosService (no IPresupuestosRepository)
public class CondicionesService(
    ILogger<CondicionesService> logger,
    ICondicionesRepository condicionesRepository,
    IPresupuestosService presupuestosService,
    ILogAccionesService logAccionesService) : ICondicionesService
{
    private readonly ILogger<CondicionesService> _logger = logger;
    private readonly ICondicionesRepository _condicionesRepository = condicionesRepository;
    private readonly IPresupuestosService _presupuestosService = presupuestosService;
    private readonly ILogAccionesService _logAccionesService = logAccionesService;
}

// ❌ MAL — CondicionesService inyecta un repositorio ajeno
public class CondicionesService(
    ICondicionesRepository condicionesRepository,
    IPresupuestosRepository presupuestosRepository) // ❌ repositorio de otro dominio
```

**Patrón de inyección en servicios:** se usa **primary constructor** (C# 12) asignando directamente a campos `_camelCase`.

### Regla de auditoría en operaciones de negocio

**Las acciones de auditoría se registran siempre desde el servicio, nunca desde la página web.** La página llama al servicio y no sabe nada del log de auditoría.

**Cuándo registrar:**
- **Después** de la operación si es una modificación o eliminación (garantiza que solo se registra si tuvo éxito)
- **Antes** de la operación si es un proceso largo o asíncrono (ej: importaciones, copias de procesos batch)

**Cómo registrar:**
```csharp
// ✅ CORRECTO — auditoría en el servicio, después de la operación exitosa
public async Task EliminarVigencia(Vigencia vigencia)
{
    await _condicionesRepository.EliminarVigencia(vigencia.Codigo);

    // Auditoría DESPUÉS de eliminación exitosa
    try
    {
        await _logAccionesService.Insertar(AccionesLog.EliminarVigencia, vigencia);
    }
    catch (Exception logEx)
    {
        _logger.LogError(logEx, "Error registrando auditoría (eliminación exitosa)");
        // No propagar — la operación de negocio fue exitosa
    }
}

// ❌ MAL — auditoría registrada desde la página web
await CondicionesService.EliminarVigencia(vigencia);
await _logAccionesService.Insertar(AccionesLog.EliminarVigencia, vigencia); // ❌ no aquí
```

**Cómo crear una nueva acción de auditoría:**

Si la acción no existe, añadirla al enum `AccionesLog` en `HM.Presupuestos.Domain/Compartido/Enumerados.cs` con un `[Description]` en castellano que explique la acción:

```csharp
// En HM.Presupuestos.Domain/Compartido/Enumerados.cs
public enum AccionesLog
{
    // ...existentes...
    [Description("Eliminar Vigencia")]
    EliminarVigencia,
    [Description("Grabar Indicador")]
    GrabarIndicador,
    // ✅ Nueva acción — descripción en castellano, clara y concisa
    [Description("Eliminar Indicador de versión")]
    EliminarIndicadorVersion,
}
```

---

## 3. Convenciones de Componentes Blazor

### Jerarquía de herencia

```
ComponentBase
  └── Context                  (base de todos los componentes)
        └── ContextProtegido   (páginas con validación de permisos)
              └── MiPagina.razor
```

### Ciclo de vida obligatorio

```csharp
// ❌ NUNCA sobreescribir OnInitializedAsync en páginas
// ✅ SIEMPRE usar estos hooks:

// Hook para inicialización de la página (solo si tiene permiso)
protected override async Task InicializarPaginaAsync() { }

// Hook cuando el usuario no tiene permiso
protected override async Task OnPermisoDenegadoAsync() { }

// El usuario está disponible AQUÍ (nunca antes)
protected override async Task OnUsuarioDisponibleAsync() { }
```

### Servicios disponibles en Context (sin [Inject] adicional)

| Propiedad | Tipo | Uso |
|---|---|---|
| `Usuario` | `UsuarioEntidad` | Usuario activo (impersonado o SSO) |
| `UsuarioCargado` | `bool` | Guard para renderizado condicional |
| `TituloPagina` | `string` | Título calculado automáticamente desde recursos |
| `LayerOverlayService` | `ILayerOverlayService` | Overlay de carga |
| `MensajesHelper` | `MensajesHelper` | Diálogos de confirmación/error/info |
| `RegistroAplicacion` | `IRegistroAplicacion` | Logging centralizado |
| `NavigationManager` | — | Navegación Blazor estándar |

### Patrón de ejecución segura

```csharp
// Para operaciones async con overlay + manejo de errores automático:
await EjecutarAsync(async () =>
{
    var datos = await MiServicio.ObtenerDatos();
    MiLista = datos;
});

// Sin overlay:
await EjecutarAsync(() => { ... }, showOverlay: false);

// Con retorno de valor:
var items = await EjecutarAsync(
    async () => await MiServicio.ObtenerItems(),
    defaultValue: new List<Item>()
) ?? [];
```

---

## 4. Convenciones de Nomenclatura y Código

### Naming por contexto

| Contexto | Tipo | Convención | Ejemplo |
|---|---|---|---|
| Flag interno / estado privado | Campo | `_camelCase` | `private bool _componentInitialized = false;` |
| Binding en Razor (`@bind`) | Propiedad | `PascalCase` | `private string Mensaje { get; set; }` |
| Valor calculado (`=>`) | Propiedad | `PascalCase` | `private string CssClase => $"btn-{Tipo}";` |
| Inyección de dependencia (`[Inject]`) | Propiedad | `PascalCase` | `[Inject] protected IIndicadoresService IndicadoresService` |
| Parámetro de componente | `[Parameter]` | `PascalCase` | `[Parameter] public string Titulo { get; set; }` |
| Clases y selectores CSS | — | `kebab-case` | `.boton-principal`, `.contenedor-centrado` |
| URLs y rutas | — | `kebab-case` | `/administracion/meses-bloqueados` |

### ✅ Campo privado — `_camelCase`

```csharp
private bool _componentInitialized = false; // ✅
private DxGrid _gridRef = default!;         // ✅
```

Úsalo cuando:
- Solo se usa internamente en el code-behind
- **No** se referencia en markup Razor (`@bind`, `@NombreVariable`)
- Es un flag, estado interno o referencia de componente
- No necesita acceso público ni protegido

### ✅ Propiedad privada — `PascalCase`

```csharp
private string Mensaje { get; set; } = string.Empty;   // ✅ para binding
private List<Indicador> DatosIndicadores { get; set; } = []; // ✅ para data binding
```

Úsala cuando:
- Se referencia en markup Razor (`@Mensaje`, `@bind-Value="Mensaje"`)
- Puede necesitar lógica en `get`/`set` en el futuro
- Se expone como dato para la vista

### ✅ Propiedad calculada — `PascalCase` con `=>`

```csharp
private string ClaseTextArea => $"form-control textarea-{ClaseAviso}"; // ✅
private bool TieneErrores => Errores.Count > 0; // ✅
```

Úsala cuando:
- El valor se calcula en tiempo de lectura
- Depende de otras propiedades
- Solo necesitas `get` (sin `set`)

### ✅ Inyección de dependencias — siempre en code-behind

Las dependencias deben declararse en el archivo `.razor.cs`, **nunca con `@inject` en el `.razor`**:

```csharp
// ✅ CORRECTO — en MiPagina.razor.cs
[Inject] protected IIndicadoresService IndicadoresService { get; set; } = default!;
[Inject] protected IVersionesService VersionesService { get; set; } = default!;
```

```razor
@* ❌ EVITAR — @inject en el archivo .razor *@
@inject IIndicadoresService IndicadoresService
```

Motivos: `@inject` genera propiedades públicas; el code-behind ofrece Intellisense completo, refactoring, debugging con breakpoints y mejor separación de concerns.

### ✅ CSS — `kebab-case`

```css
/* ✅ CORRECTO */
.boton-principal { background-color: #007bff; }
.contenedor-centrado { display: flex; justify-content: center; }

/* ❌ EVITAR */
.botonPrincipal { }
.ContenedorCentrado { }
```

### Resumen de decisión campo vs propiedad

```
¿Se referencia en markup Razor (@bind, @Variable)?
  ├── SÍ  → Propiedad PascalCase  (private string Nombre { get; set; })
  └── NO  → Campo _camelCase      (private bool _flag = false)

¿El valor se calcula a partir de otras propiedades?
  └── SÍ  → Propiedad calculada PascalCase con =>
```

---

## 5. Traducciones y Recursos

```csharp
// Obtener texto traducido
ObtenerTexto(TextosApp.Common.Descripcion)
ObtenerTexto(TextosApp.Pages.Indicadores.Mostrar)
ObtenerTexto(TextosApp.Mensajes.ConfirmacionEliminar)

// Verificar si existe una clave
ExisteRecurso("MensajeErrorBD:-20001:label")
```

- Los textos son multiidioma. **Nunca usar strings literales en la UI.**
- Los idiomas disponibles se obtienen con `LocalizadorRecursos.ObtenerIdiomas()`.

### ⚠️ Cómo añadir nuevos textos traducibles

Cuando una tarea requiera añadir **etiquetas, mensajes, tooltips o cualquier texto visible en la UI**, se debe seguir el proceso definido en:

> **`.github/prompts/anadir-traduccion.prompt.md`**

Este prompt cubre el flujo completo para registrar nuevas claves en el sistema de recursos multiidioma. **Debe usarse siempre** antes de referenciar una clave nueva con `ObtenerTexto(...)`.

---

## 6. Seguridad y Permisos

### Autenticación
- SSO con Azure AD vía OpenIdConnect + `Microsoft.Identity.Web`
- La sesión se propaga a través de `ISesionUsuario` → `ContextoUsuario`

### Autorización en páginas
```razor
@attribute [Authorize]
@inherits ContextProtegido
```
`ContextProtegido` valida automáticamente si el usuario tiene permiso para la URL actual consultando `IControlAccesoNavegacion.PuedeAccederA(url)`.

### Permisos y menús
- Vienen del objeto `UsuarioEntidad` obtenido de la API HM.CORE
- Los menús se identifican por `CodigosMenu` (enum en Domain)
- El admin se identifica por regla: `Constantes.User.RULE_ADMIN`

### Gestión de sesión y persistencia ante F5

La sesión del usuario se almacena en `ProtectedSessionStorage` del navegador (cifrado automático por ASP.NET Core). Esto garantiza que **la sesión sobrevive a una recarga de página (F5)**.

Hay dos slots de sesión independientes (claves en `Constantes.Session`):

| Clave | Contenido | Servicio |
|---|---|---|
| `USER_SSO` | Usuario autenticado vía Azure AD | `GuardarUsuarioSSO` / `ObtenerUsuarioSSO` |
| `USER_LOGIN` | Usuario impersonado vía login manual | `GuardarUsuarioImpersonado` / `ObtenerUsuarioImpersonado` |

**Flujo de inicialización** (`SesionUsuario.InicializarUsuarioAsync`):
1. Lee `USER_SSO` del storage → `ContextoUsuario.UsuarioAutenticado`
2. Lee `USER_LOGIN` del storage → `ContextoUsuario.UsuarioImpersonado` (si existe)
3. Dispara el evento `UsuarioCargado` → todos los componentes suscritos actualizan su estado
4. Si ya hay `UsuarioApp` cargado en memoria, no vuelve a inicializar (idempotente)

**Flujo ante F5:**
- Blazor Server pierde el estado en memoria, pero el `ProtectedSessionStorage` persiste en el navegador
- Al reconectar el circuito, `InicializarUsuarioAsync` rehidrata `UsuarioApp` desde el storage
- Los componentes reciben el evento `UsuarioCargado` y renderizan con el usuario correcto

### Impersonación

La impersonación permite que un usuario SSO opere con la identidad y permisos de otro usuario. Se gestiona desde la página `/administracion/impersonacion` (requiere permiso `CodigosMenu.Impersonacion`).

**Flujo de impersonación:**
1. El usuario SSO (admin) introduce login + password del usuario a impersonar
2. Se llama a `ISesionUsuario.AutenticarUsuarioPorLoginAsync(login, password)`
3. La API HM.CORE valida las credenciales y devuelve el `UsuarioEntidad` del usuario impersonado
4. Se guarda en `USER_LOGIN` del `ProtectedSessionStorage`
5. `ContextoUsuario.UsuarioImpersonado` se actualiza → `Usuario` (activo) pasa a ser el impersonado
6. Se dispara `UsuarioCargado` → toda la UI se refresca con los permisos del usuario impersonado

**Flujo de cierre de impersonación:**
1. Se llama a `ISesionUsuario.CerrarSesionLoginAsync()`
2. Se elimina `USER_LOGIN` del storage (`IAlmacenSesionUsuario.EliminarUsuarioImpersonado`)
3. `ContextoUsuario.UsuarioImpersonado` queda a `null` → `Usuario` vuelve a ser el SSO
4. Se dispara `OnUsuarioImpersonadoDesconectado()` en los componentes → hook para limpiar estado

**Propiedades relevantes en `Context`:**

| Propiedad | Descripción |
|---|---|
| `Usuario` | Usuario **activo** — impersonado si lo hay, SSO si no |
| `UsuarioAutenticado` | Siempre el usuario SSO real, independientemente de impersonación |
| `UsuarioImpersonado` | El usuario impersonado, o `null` si no hay impersonación activa |
| `UsuarioEsAdmin` | `true` si el usuario activo tiene la regla `RULE_ADMIN` |

---

## 7. Error Handling y Logging

### Jerarquía de logging (3 niveles de fallback)
1. **API HM.CORE** → registro en base de datos
2. **Archivo NLog** → si la API falla
3. **Console** → último recurso

### Errores de base de datos controlados
- Códigos `-20001` a `-20999`: errores de negocio controlados
- Códigos `-20001` a `-20499`: además se envían al log
- Tienen mensaje traducible: `MensajeErrorBD:{codigo}:label`

```csharp
// En páginas con ExcepcionBaseDatos:
await TratarExcepcionGeneradaEnBD(ex, TituloPagina);   // error
await TratarWarningGeneradoEnBD(ex, TituloPagina);     // advertencia
```

### ❌ Anti-patrón: Catch & Rethrow sin valor añadido

**Nunca** escribir un catch que solo relanza envuelto en una nueva excepción:

```csharp
// ❌ MAL — no añade nada, empeora el diagnóstico
catch (Exception ex)
{
    throw new Exception("IndicadoresRepository.EliminarIdiomaIndicador", ex);
}
```

Problemas de este patrón:
- No añade funcionalidad: solo captura para relanzar
- Pierde el stack trace original al envolver en una nueva excepción
- No hace logging: el error pasa sin registrarse
- El string se interpreta como **mensaje** de la excepción, no como identificador del método

**Alternativas correctas:**

```csharp
// ✅ OPCIÓN A: No capturar — dejar que la excepción suba con su stack trace intacto
// (la capa superior, Context.EjecutarAsync, la capturará y registrará)
public async Task EliminarIdiomaIndicador(int codigo)
{
    await _repositorio.EliminarAsync(codigo);
}

// ✅ OPCIÓN B: Capturar solo si se añade contexto real (logging + rethrow)
catch (Exception ex)
{
    _logger.LogError(ex, "Error al eliminar idioma indicador {Codigo}", codigo);
    throw; // rethrow sin envolver, preserva el stack trace
}

// ✅ OPCIÓN C: En capas superiores (páginas), usar EjecutarAsync
await EjecutarAsync(async () =>
{
    await IndicadoresService.EliminarIdiomaAsync(codigo);
});
```

---

## 8. Modelo de Datos — Entidades principales

### Indicador
```
Codigo          int       // PK, calculado
Descripcion     string    // Única
Orden           int       // Único, múltiplos de 10
BitAnd          int       // Potencia de 2, única
IndMostrar      bool
IndVersionUnica bool
Idiomas         List<IdiomaIndicador>
Estado          EstadoEntidad   // Nuevo | Modificado | SinCambios
```

### IdiomaIndicador
```
Codigo                int
CodigoIdioma          string
Descripcion           string
DescripcionAbreviada  string
Leyenda               string
```

---

## 9. Estructura de Tests E2E

### Framework
- **Playwright + NUnit** en proyecto `HM.Presupuestos.E2ETest`
- Todos los tests heredan de `E2ETestBase`

### Configuración
```json
// appsettings.json → sección "E2ETest"
{
  "E2ETest": {
    "BaseUrl": "https://localhost:7001",
    "Usuario": "",
    "Password": "",
    "Headless": true,
    "SlowMo": 0
  }
}
```

### Sesión SSO
- Se guarda en `sesion_auth.json` (excluido del repositorio)
- Generado con script `GuardarSesion.ps1`
- `E2ETestBase` lo inyecta automáticamente en `ContextOptions()`

### Patrón de test
```csharp
[TestFixture]
public class MiPaginaTests : E2ETestBase
{
    [Test]
    public async Task MiTest()
    {
        await IrAUrl("mi-ruta");
        // Page es la instancia Playwright (heredada de PageTest)
        await Expect(Page.Locator("...")).ToBeVisibleAsync();
    }
}
```

### Estructura de carpetas
```
HM.Presupuestos.E2ETest/
├── Base/
│   └── E2ETestBase.cs
├── Configuracion/
│   └── E2ETestSettings.cs
├── {NombrePagina}/
│   └── {NombrePagina}Tests.cs
└── appsettings.json
```

---

## 10. Setup del Entorno Local

### Configuración
- `appsettings.json` en `HM.Presupuestos.Web` → URLs de API, Azure AD
- `appsettings.json` en `HM.Presupuestos.E2ETest` → `E2ETest:BaseUrl`, credenciales

### Sesión para tests
```powershell
# Generar sesion_auth.json (ejecutar una vez o cuando expire la sesión SSO)
.\GuardarSesion.ps1
```

### Ejecutar la app para tests E2E
```powershell
dotnet run --project HM.Presupuestos.Web
```

---

## 11. Git Workflow

| Campo | Valor |
|---|---|
| Rama principal | `main` |
| Rama activa | `upgrade-to-NET10` |
| Remote | `origin → https://github.com/felixda/HM.PresupuestosWeb` |

---

## 12. Ficheros de contexto para IA

| Fichero | Contenido |
|---|---|
| `.github/copilot-instructions.md` | Reglas generales del proyecto para Copilot |
| `.github/specs/technical-specs.md` | **Este fichero** — especificaciones técnicas completas |
