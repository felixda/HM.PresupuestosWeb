# Analisis de implementacion API REST en HM.Presupuestos

## 1. Objetivo funcional

Se implemento un flujo REST para consultar maestros (tipologias, networks, medios y grupos de clientes) desde la UI de Blazor, reutilizando la capa de Application/Domain existente y manteniendo la integracion con el contexto de usuario JWT del ecosistema HM.Core.

Objetivo de negocio alcanzado:
- Exponer endpoints REST versionados para maestros.
- Consumir esos endpoints desde una pagina de administracion en Web.
- Gestionar autenticacion por bearer token sin romper el flujo actual.
- Evitar doble llamada inicial en la carga de tipologias.

## 2. Arquitectura aplicada

Se mantiene arquitectura por capas (Web -> API -> Application -> Domain -> Infrastructure):

- Web (Blazor Server)
  - Pantalla: `HM.Presupuestos.Web/Pages/Admin/MaestrosApi.razor`
  - Code-behind: `HM.Presupuestos.Web/Pages/Admin/MaestrosApi.razor.cs`
  - Cliente API: `HM.Presupuestos.Web/Adaptadores/Api/MaestrosApiService.cs`
- API (ASP.NET Core)
  - Bootstrap y auth: `HM.Presupuestos.Api/Program.cs`
  - Endpoints: `HM.Presupuestos.Api/Controllers/V1/MaestrosController.cs`
  - Contrato REST: `HM.Presupuestos.Api/Contracts/V1/Maestros/CodigoDescripcionResponse.cs`
- Application
  - Caso de uso: `IMaestrosService` / `MaestrosService` (ya existente)
- Infrastructure
  - Repositorio Oracle: `IPresupuestosRepository` / `PresupuestosRepository` (ya existente)

La API actua como adaptador de entrada y no contiene logica de negocio compleja: delega en `IMaestrosService` y transforma DTOs hacia contrato REST.

## 3. Endpoints implementados y contrato

Controlador versionado:
- Prefijo: `api/v1/maestros`
- Endpoints GET:
  - `api/v1/maestros/tipologias`
  - `api/v1/maestros/networks`
  - `api/v1/maestros/medios`
  - `api/v1/maestros/grupos-clientes`

Respuesta:
- 200 OK con lista de objetos `{ codigo, descripcion }`.
- 401 Unauthorized cuando no se puede inicializar usuario desde bearer token.

Modelo de salida:
- `CodigoDescripcionResponse` (contrato simple y estable para front).

## 4. Flujo tecnico de autenticacion/autorizacion

### 4.1 Configuracion en API (`Program.cs`)

La API configura JWT Bearer con clave de firma y validacion de vida del token:
- `ValidateLifetime = true`
- `ValidateIssuerSigningKey = true`
- `IssuerSigningKey = SymmetricSecurityKey(...)`

Estado actual (temporalmente permisivo):
- `ValidateIssuer = false`
- `ValidateAudience = false`
- `MapInboundClaims = false`

Motivo:
- Resolver incompatibilidades de claims/issuer/audience entre emision y validacion durante estabilizacion inicial.

### 4.2 Inicializacion de usuario en el controlador

En `MaestrosController`, antes de atender cada endpoint:
- Se intenta leer `Authorization: Bearer ...`.
- Se parsea el token con `IJwt.ObtenerDatosUsuarioJwt(...)`.
- Se asigna `IJwt.Usuario` para que Application/Infrastructure dispongan del contexto de usuario esperado.

Si falla cualquiera de esos pasos, se devuelve 401.

### 4.3 Envio de token desde Web

`MaestrosApiService`:
- Crea `HttpClient` con `ApiRest:BaseUrl`.
- Adjunta cabecera `Authorization` si `_jwt.Usuario?.Jwt` existe.
- Llama endpoint y valida `IsSuccessStatusCode`.
- Loguea detalle de errores HTTP y lanza excepcion enriquecida.

## 5. Funcionamiento end-to-end

Flujo de ejecucion en tiempo real:
1. El usuario entra en `/administracion/maestros-api`.
2. `InicializarPaginaAsync` en `MaestrosApi.razor.cs`:
   - configura opciones de maestro,
   - selecciona por defecto tipologias,
   - ejecuta `CargarMaestroAsync()`.
3. `CargarMaestroAsync` llama a `MaestrosApiService` segun el maestro seleccionado.
4. El servicio Web invoca API REST con bearer token.
5. `MaestrosController` valida contexto JWT y delega en `IMaestrosService`.
6. Application obtiene datos desde repositorio y retorna lista codigo/descripcion.
7. La pagina pinta datos en `DxGrid`.

## 6. Incidencias encontradas y resolucion

### 6.1 Tipologias vacias / comportamiento inconsistente

Sintoma:
- Tipologias no devolvia datos mientras otros maestros si.

Acciones:
- Revision del pipeline de auth y contexto usuario en API.
- Asegurar inicializacion de `IJwt.Usuario` desde bearer en controlador.

Resultado:
- Endpoints volvieron a responder datos esperados.

### 6.2 401 Unauthorized al endurecer configuracion

Sintoma:
- Al pasar a modo "definitivo" aparecio 401 en llamadas de maestros.

Causa principal:
- Desalineacion entre token real emitido y reglas de validacion en API (issuer/audience/claims).

Mitigacion aplicada:
- Configuracion de bearer temporalmente compatible (`ValidateIssuer/Audience=false`) para estabilizar operativa.

### 6.3 Doble llamada a `ObtenerTipologias`

Sintoma:
- La UI ejecutaba doble carga inicial al entrar en pagina.

Fix en `MaestrosApi.razor.cs`:
- En el handler `OnComboMaestroSelectedDataItemChangedAsync`, se ignora evento si el valor no cambia:
  - `if (MaestroSeleccionado == e.DataItem.Codigo) return;`

Resultado:
- Se elimina llamada redundante.

## 7. Pruebas y validacion ejecutadas

### 7.1 Unit tests

Validado en verde:
- `HM.Presupuestos.UnitTest`: 32/32 OK.
- `HM.Presupuestos.Web.UnitTest`: 6/6 OK.

### 7.2 E2E

Automatizacion creada/ajustada:
- Script: `RunE2ETests.ps1`.
- Arranca Web con `dotnet run --no-build`.
- Espera readiness HTTP.
- Ejecuta E2E con `--no-build`.
- Cierra proceso Web en `finally`.

Resultado reciente:
- Suite E2E ejecuta correctamente en infraestructura.
- Tests de menu estabilizados para no fallar por falso negativo cuando falta sesion SSO.

## 8. Decisiones tecnicas relevantes

1. Mantener versionado REST en ruta (`/api/v1/...`) para evolucion segura.
2. Separar contrato API (`Contracts`) de modelos internos de Application.
3. Preservar integracion HM.Core (`IJwt`, `IControlador`, `IUserProvider`) en lugar de bypass ad-hoc.
4. Priorizar estabilidad funcional antes de cerrar issuer/audience de forma estricta.

## 9. Estado actual

Implementacion API REST funcional:
- Endpoints de maestros operativos.
- Consumo desde Web en produccion funcional.
- Doble llamada inicial corregida.
- Pipeline de pruebas automatizado y reproducible.

Riesgo residual conocido:
- Validacion JWT en API no esta aun cerrada estrictamente en issuer/audience.

## 10. Recomendaciones para cierre definitivo (hardening)

Para cumplir seguridad estricta sin regresiones:

1. Cerrar issuer y audience en `Program.cs`:
   - `ValidateIssuer = true`
   - `ValidateAudience = true`
   - `ValidIssuer/ValidIssuers` alineado con emisor real.
   - `ValidAudience/ValidAudiences` alineado con API destino.
2. Parametrizar por entorno (DEV/PRU/PRE/PRO) en configuracion, evitando hardcode.
3. Añadir tests de integracion de auth:
   - token valido -> 200,
   - issuer invalido -> 401,
   - audience invalida -> 401,
   - token expirado -> 401.
4. Activar trazas de diagnostico de auth durante despliegue inicial para facilitar soporte.

## 11. Mapa rapido de archivos clave

- API bootstrap: `HM.Presupuestos.Api/Program.cs`
- Endpoints maestros: `HM.Presupuestos.Api/Controllers/V1/MaestrosController.cs`
- Contrato de respuesta: `HM.Presupuestos.Api/Contracts/V1/Maestros/CodigoDescripcionResponse.cs`
- Cliente REST Web: `HM.Presupuestos.Web/Adaptadores/Api/MaestrosApiService.cs`
- Pantalla Blazor: `HM.Presupuestos.Web/Pages/Admin/MaestrosApi.razor`
- Logica de pantalla: `HM.Presupuestos.Web/Pages/Admin/MaestrosApi.razor.cs`
- Ejecucion E2E: `RunE2ETests.ps1`

## 12. Resumen ejecutivo

La implementacion del API REST de maestros esta completada y operativa en flujo real Web->API->Application->Infrastructure. Se resolvieron problemas de 401 y de doble llamada inicial, y se dejo automatizada la validacion E2E. El siguiente hito recomendado es cerrar la validacion de issuer/audience de JWT con configuracion por entorno y tests de integracion de seguridad para consolidar el modo definitivo.