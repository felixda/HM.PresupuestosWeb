# Guía de Configuración del Entorno — HM.Presupuestos

Esta guía permite a un nuevo miembro del equipo tener el entorno completamente operativo.

---

## 1. Requisitos previos

| Herramienta | Versión mínima | Enlace |
|---|---|---|
| .NET SDK | 10.0 | https://dotnet.microsoft.com/download/dotnet/10.0 |
| Visual Studio | 2022 v17.10+ | https://visualstudio.microsoft.com/ |
| VS Code (alternativa) | cualquiera | + extensión **C# Dev Kit** |
| Git | cualquiera | https://git-scm.com/ |
| GitHub Copilot | — | Plan Individual o Business con acceso al repo |

---

## 2. Clonar y restaurar

```powershell
git clone <url-repositorio>
cd ESFNN-PresupuestosWeb
dotnet restore
```

> El fichero `NuGet.Config` incluye el feed privado de Havas (`https://nuget.havasmedia.com/nuget`).
> Si obtienes error 401 al restaurar, contacta con el equipo para obtener credenciales de acceso al feed.

---

## 3. Configurar los secretos del entorno

La aplicación usa `dotnet user-secrets` para los valores sensibles (Azure AD, Oracle, JWT…).
El repositorio incluye un script `Switch-Env.ps1` que automatiza el proceso.

### 3.1 Crear tu fichero de secretos local

Copia la plantilla y rellena los valores reales (pídelos al equipo):

```powershell
cp secrets.local.template.json secrets.local.json
# Editar secrets.local.json con los valores reales de cada entorno
```

> `secrets.local.json` está en `.gitignore` — nunca se subirá al repositorio.

### 3.2 Cambiar al entorno deseado

```powershell
.\Switch-Env.ps1 -Env DEV    # o PRU, PRE, PRO
```

El script limpia los secretos actuales y carga los del entorno indicado, usando `dotnet user-secrets`.
Al terminar muestra un listado de los secretos activos para verificar.

### 3.3 Desde GitHub Copilot (alternativa)

También puedes cambiar de entorno usando lenguaje natural:

> *«Cambia al entorno PRU»*

El agente `env-switcher` detecta el entorno, ejecuta el script y confirma el cambio.

### 3.4 Claves que se configuran por entorno

| Clave | Descripción |
|---|---|
| `AppSettings:AppEnvironment` | Identificador del entorno (DEV / PRU / PRE / PRO) |
| `AppSettings:AzureAd:TenantId` | Tenant de Azure AD |
| `AppSettings:AzureAd:ClientId` | Client ID de la aplicación registrada |
| `AppSettings:AzureAd:ClientSecret` | Secreto de la aplicación Azure AD |
| `AppSettings:Session:TokenInternalAuthentication` | Token para llamadas internas al Core |
| `Jwt:Clave` | Clave de firma JWT |
| `ServicioCore:Core` | URL base del servicio HM.CORE |
| `ConnectionStrings:CadenasConexion:0:CadenaConexion` | Cadena de conexión Oracle (havas_seguridad) |
| `ConnectionStrings:CadenasConexion:1:CadenaConexion` | Cadena de conexión Oracle (Presupuestos) |

---

## 4. Ejecutar la aplicación

```powershell
dotnet run --project HM.Presupuestos.Web
```

La aplicación arranca en `https://localhost:8080`. La primera petición redirige al login de Azure AD.

---

## 5. Ejecutar los tests

### Tests unitarios

```powershell
dotnet test HM.Presupuestos.UnitTest
dotnet test HM.Presupuestos.Web.UnitTest
```

O con el script incluido:

```powershell
.\RunUnitTests.ps1
```

### Tests E2E (Playwright)

Los tests E2E requieren que la aplicación esté corriendo. Configurar `HM.Presupuestos.E2ETest/appsettings.json` con las credenciales de test:

```json
{
  "E2ETest": {
    "BaseUrl": "http://localhost:8080",
    "Usuario": "<usuario de test>",
    "Password": "<contraseña de test>",
    "Headless": true
  }
}
```

Para guardar la sesión autenticada (evitar login en cada ejecución):

```powershell
cd HM.Presupuestos.E2ETest
.\GuardarSesion.ps1
```

---

## 6. Configurar GitHub Copilot (OpenSpec)

El proyecto usa un workflow de IA asistida llamado **OpenSpec** que automatiza la escritura de propuestas, diseños y tareas de implementación.

### 6.1 Requisitos

- Extensión **GitHub Copilot** instalada en VS Code o Visual Studio
- Acceso a modelos con herramientas (Claude Sonnet o superior)

### 6.2 Agentes disponibles

Los agentes están definidos en `.github/agents/`. Se invocan desde el chat de Copilot con `@nombre-agente`:

| Agente | Propósito |
|---|---|
| `project-validator` | Compila y ejecuta todos los tests |
| `code-reviewer` | Revisa calidad del código C#/Blazor |
| `architecture-reviewer` | Verifica cumplimiento de arquitectura hexagonal |
| `tests-reviewer` | Revisa calidad y cobertura de tests |
| `frontend-reviewer` | Revisa componentes Blazor y patrones UI |
| `repositories-reviewer` | Revisa queries SQL en repositorios Oracle |
| `qa-tester` | Prueba flujos funcionales con Playwright |
| `ux-reviewer` | Evaluación visual de UX en el navegador |

### 6.3 Comandos de OpenSpec

Desde el chat de Copilot (modo agente):

| Comando | Descripción |
|---|---|
| `/openspec-propose` | Proponer un nuevo cambio (genera proposal + design + tasks) |
| `/openspec-apply-change` | Implementar tareas de un cambio activo |
| `/openspec-explore` | Modo exploración: pensar un problema antes de proponer |
| `/openspec-archive-change` | Archivar un cambio completado |
| `/task-validate` | Compilar y ejecutar todos los tests |
| `/task-code-review` | Revisión de calidad del código |
| `/task-architecture-review` | Verificar arquitectura hexagonal |
| `/task-testing-review` | Revisión de cobertura de tests |
| `/task-frontend-review` | Revisión de UI Blazor |
| `/action-tdd` | Aplicar ciclo TDD (red → green → refactor) |
| `/action-refactor` | Refactorizar código siguiendo principios del proyecto |

### 6.4 Estructura de OpenSpec

```
openspec/
├── config.yaml          ← contexto del proyecto y reglas para el agente
├── changes/             ← cambios en curso (uno activo a la vez)
│   └── <nombre-cambio>/
│       ├── proposal.md  ← qué se va a hacer y por qué
│       ├── design.md    ← diseño técnico por capa
│       └── tasks.md     ← tareas TDD ordenadas de dentro hacia fuera
└── specs/               ← especificaciones funcionales reutilizables
```

---

## 7. Documentación del esquema Oracle

Los ficheros del esquema están en `.github/skills/guidelines/database/`:

| Fichero | Contenido |
|---|---|
| `database-schema.md` | Todas las tablas con columnas, tipos y PKs/FKs |
| `business-domain.md` | Tablas agrupadas por dominio funcional |
| `database-erd-ppt.md` | ERD visual (tablas PPT) — visualizable con `Ctrl+Shift+V` en VS Code |
| `schema-changelog.md` | Historial de cambios del esquema |

Para regenerar la documentación del esquema Oracle:

```powershell
$env:ORACLE_CONNECTION_STRING = "user id=...;password=...;data source=IBEDESA19c;Pooling=True;"
.\Sync-OracleSchema.ps1
```

> Ver `.github/skills/guidelines/database/README.md` para instrucciones completas.

---

## 8. Convenciones y guías del proyecto

Toda la documentación técnica está en `.github/skills/guidelines/`. Ver el índice completo en:

👉 [`.github/skills/guidelines/README.md`](.github/skills/guidelines/README.md)
