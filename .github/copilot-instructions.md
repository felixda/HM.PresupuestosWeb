# Copilot Instructions — HM.Presupuestos

## Tecnologías
- .NET 8, Blazor Server (Interactive Server Components)
- DevExpress Blazor UI (DxTreeView, DxPopup, DxDrawer, DxGridLayout...)
- Autenticación SSO con Azure AD (Microsoft.Identity.Web + OpenIdConnect)
- Arquitectura en capas: Domain, Application, Infrastructure, Server

## Convenciones de código
- Los componentes Blazor heredan de `Context` (no de `ComponentBase` directamente)
- La lógica de usuario se obtiene a través de `IUsuarioServicio` y el evento `OnUsuarioCargado`
- El usuario está disponible en `OnUsuarioDisponibleAsync()`, nunca en `OnInitializedAsync()`
- Los textos y traducciones se obtienen con el helper `T("clave:subclave")`
- Los menús y permisos vienen del objeto `UsuarioEntidad` obtenido de la API HM.CORE

## Tests E2E
	- Framework: Playwright + NUnit en el proyecto `HM.Presupuestos.E2ETest`
- La sesión SSO se guarda en `sesion_auth.json` (excluido del repositorio)
- Los tests heredan de `E2ETestBase`
- La app corre en `http://localhost:8080` durante los tests

## Especificaciones técnicas
Ver `.github/specs/technical-specs.md` para el stack, arquitectura, convenciones y patrones completos.
