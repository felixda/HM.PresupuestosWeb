# API REST de Maestros - Implementacion y Funcionamiento

## Objetivo

Este documento describe, de forma separada, como se implemento el API REST de maestros en HM.Presupuestos y como funciona de extremo a extremo.

## Alcance funcional

El API REST expone catalogos maestros para la pantalla de administracion:
- Tipologias
- Networks
- Medios
- Grupos de clientes

La UI consume estos datos y los muestra en una grilla, permitiendo cambiar de maestro desde un combo.

## Componentes implicados

### API
- Bootstrap y autenticacion: `HM.Presupuestos.Api/Program.cs`
- Endpoints REST: `HM.Presupuestos.Api/Controllers/V1/MaestrosController.cs`
- Contrato de salida: `HM.Presupuestos.Api/Contracts/V1/Maestros/CodigoDescripcionResponse.cs`

### Web
- Servicio consumidor REST: `HM.Presupuestos.Web/Adaptadores/Api/MaestrosApiService.cs`
- Pagina Blazor: `HM.Presupuestos.Web/Pages/Admin/MaestrosApi.razor`
- Logica code-behind: `HM.Presupuestos.Web/Pages/Admin/MaestrosApi.razor.cs`

### Capa de negocio e infraestructura
- Caso de uso: `IMaestrosService` / `MaestrosService`
- Acceso datos: `IPresupuestosRepository` / `PresupuestosRepository`

## Endpoints publicados

Prefijo base: `api/v1/maestros`

Endpoints GET:
- `api/v1/maestros/tipologias`
- `api/v1/maestros/networks`
- `api/v1/maestros/medios`
- `api/v1/maestros/grupos-clientes`

Respuesta estandar:
- HTTP 200 con lista de `codigo` y `descripcion`.
- HTTP 401 cuando no se puede inicializar usuario desde bearer token.

## Flujo tecnico completo

1. El usuario abre la pagina `/administracion/maestros-api`.
2. `InicializarPaginaAsync()` carga opciones del combo y selecciona por defecto tipologias.
3. La pagina invoca `MaestrosApiService` para llamar al endpoint REST correspondiente.
4. El servicio Web crea `HttpClient` con `ApiRest:BaseUrl` y adjunta `Authorization: Bearer <token>` si existe JWT en contexto.
5. La API recibe la llamada y en `MaestrosController` intenta reconstruir el contexto de usuario desde el token.
6. Si el token es correcto, delega en `IMaestrosService` para obtener datos.
7. Se mapean a `CodigoDescripcionResponse` y se devuelve `200 OK`.
8. La pagina renderiza el resultado en `DxGrid`.

## Autenticacion y seguridad

Estado de configuracion actual en API:
- `ValidateLifetime = true`
- `ValidateIssuerSigningKey = true`
- `ValidateIssuer = false` (temporal)
- `ValidateAudience = false` (temporal)

Razon de la configuracion temporal:
- Se priorizo restaurar compatibilidad con el token emitido en los entornos actuales durante la estabilizacion.

Objetivo final recomendado:
- Cerrar issuer y audience de forma estricta por entorno (DEV/PRU/PRE/PRO).

## Problemas encontrados durante la implantacion

### 1) Tipologias sin datos
- Se observo inconsistencia: algunos maestros devolvian datos y tipologias no.
- Se ajusto el flujo de inicializacion del usuario JWT en API.

### 2) Error 401 Unauthorized
- Aparecio al endurecer autenticacion sin alinear emision/validacion del token.
- Se aplico configuracion temporal compatible para recuperar operativa.

### 3) Doble llamada inicial de tipologias
- La UI hacia dos llamadas en la carga inicial.
- Se corrigio en `OnComboMaestroSelectedDataItemChangedAsync` ignorando cambios redundantes.

## Validacion realizada

### Unit tests
- `HM.Presupuestos.UnitTest`: OK
- `HM.Presupuestos.Web.UnitTest`: OK

### E2E
- Script automatizado: `RunE2ETests.ps1`
- Flujo: arranca Web, espera readiness, ejecuta E2E y detiene Web.
- Estado: ejecucion automatizada operativa.

## Como funciona en operacion diaria

- La pagina se comporta como un cliente REST interno.
- El backend API encapsula seguridad y acceso a datos.
- Application mantiene la logica de negocio.
- Infrastructure mantiene acceso a Oracle/repositorios.

El resultado es una integracion limpia y extensible: agregar nuevos maestros implica sumar endpoint + metodo de servicio + opcion de UI, manteniendo el mismo patron.

## Proximos pasos recomendados

1. Cerrar validacion JWT estricta (`issuer` y `audience`).
2. Parametrizar valores por entorno y documentarlos.
3. Añadir tests de integracion de seguridad (token valido/invalido/expirado).
4. Mantener trazas de diagnostico de autenticacion durante estabilizacion de despliegue.
