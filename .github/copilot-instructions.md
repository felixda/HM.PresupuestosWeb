# Copilot Instructions — HM.Presupuestos

## Tecnologías
- .NET 10, Blazor Server (Interactive Server Components)
- DevExpress Blazor UI (DxGrid, DxPopup, DxFormLayout, DxTreeView, DxDrawer...)
- Autenticación SSO con Azure AD (Microsoft.Identity.Web + OpenIdConnect)
- Arquitectura en capas: Domain, Application, Infrastructure, Web

## Convenciones de código
- Los componentes Blazor heredan de `Context` o `ContextProtegido` (nunca de `ComponentBase` directamente)
- El usuario está disponible en `OnUsuarioDisponibleAsync()`, nunca en `OnInitializedAsync()`
- Las páginas con permisos implementan `InicializarPaginaAsync()` y `OnPermisoDenegadoAsync()`
- Las operaciones async se envuelven en `EjecutarAsync(async () => { ... })` para overlay + manejo de errores
- Los textos y traducciones se obtienen con `ObtenerTexto(AppResources.Seccion.Clave)`
- Cuando una tarea requiera añadir etiquetas, mensajes o cualquier texto en la UI, seguir el proceso de `.github/prompts/anadir-traduccion.prompt.md`
- Los menús y permisos vienen del objeto `UsuarioEntidad` obtenido de la API HM.CORE

## Tests E2E
- Framework: Playwright + NUnit en el proyecto `HM.Presupuestos.E2ETest`
- La sesión SSO se guarda en `sesion_auth.json` (excluido del repositorio, generado con `GuardarSesion.ps1`)
- Los tests heredan de `E2ETestBase` y usan `IrAUrl("ruta-relativa")` para navegar
- La URL base se configura en `appsettings.json` sección `E2ETest:BaseUrl` (default: `https://localhost:7001`)

## Especificaciones técnicas
Ver `.github/specs/technical-specs.md` para el stack completo, arquitectura, ciclo de vida de componentes, patrones de código, modelo de datos y estructura de tests.
