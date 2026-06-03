# Guideline: ClienteApiCore — Cliente HTTP para la API de HM.CORE

> Documento SDD de referencia para el diseño, uso y extensión de `ClienteApiCore`.  
> Aplica a cualquier desarrollo que necesite comunicarse con la API externa HM.CORE.

---

## 1. Propósito

`ClienteApiCore` es el **único punto de acceso** a la API REST de HM.CORE desde la capa `Infrastructure`.  
Centraliza la autenticación JWT, la serialización JSON, el manejo de errores HTTP y el trazado de llamadas.

---

## 2. Ubicación en la arquitectura

```
HM.Presupuestos.Infrastructure
  └── Servicios
        ├── IClienteApiCore.cs   ← Adaptador secundario (no puerto de dominio)
        └── ClienteApiCore.cs    ← Implementación
```

### 2.1 ¿Por qué `IClienteApiCore` está en `Infrastructure` y no en `Domain/Puertos/`?

En arquitectura hexagonal estricta los puertos secundarios deberían vivir en `Domain`. Sin embargo,
`IClienteApiCore` **no puede moverse a `Domain`** porque sus métodos usan tipos de `HM.Core.Comun.v6`
(paquete externo que `Domain` no referencia ni debe referenciar):

```csharp
Task GuardarCodigosDeMenusFavoritos(ElementoConfiguracion configuracionFavoritos); // ← HM.Core.Comun.v6
Task RegistrarLog(string jwtUsuario, DatosPeticionLogData datosLog);               // ← HM.Core.Comun.v6
```

`IClienteApiCore` es un **adaptador técnico de infraestructura**, no un puerto de negocio.

### 2.2 El puerto de dominio real: `IRegistroErroresCore`

El verdadero puerto de dominio secundario es `IRegistroErroresCore`, en `Domain/Puertos/Servicios/`,
porque solo maneja `DetalleError` (entidad propia del dominio) y actúa como intermediario:

```
Domain                          Infrastructure
───────────────────────         ──────────────────────────────────────────────
IRegistroErroresCore    →       RegistroErroresCore  →  IClienteApiCore
(DetalleError: tipo propio)      (traduce a              (DatosPeticionLogData:
                                  DatosPeticionLogData)   tipo HM.Core.Comun.v6)
```

Esta separación garantiza que `Domain` permanece sin dependencias externas y que el
aislamiento de `HM.Core.Comun.v6` queda confinado en `Infrastructure`.

---

## 3. Diseño técnico

### 3.1 Registro en DI

```csharp
// Program.cs
builder.Services.AddHttpClient<IClienteApiCore, ClienteApiCore>();
```

**Typed HttpClient**: el `HttpClient` lo gestiona `IHttpClientFactory` → sin *socket exhaustion*.  
Ciclo de vida: **Transient**.

### 3.2 Constructor (Primary Constructor — C# 12)

```csharp
public sealed class ClienteApiCore(
    HttpClient httpClient,
    ILogger<ClienteApiCore> logger,
    IConfiguration configuration,
    IJwt jwt) : IClienteApiCore
```

| Dependencia | Origen | Propósito |
|---|---|---|
| `HttpClient` | Inyectado por `IHttpClientFactory` | Transporte HTTP reutilizable |
| `ILogger<ClienteApiCore>` | DI estándar | Trazas y errores |
| `IConfiguration` | DI estándar | URL base (`ServicioCore:Core`) |
| `IJwt` | Scoped (por circuito Blazor) | Token JWT del usuario autenticado |

### 3.3 Obtención del token JWT — dos estrategias

#### Estrategia A — Token desde `IJwt` (automático)

Para métodos invocados **solo con usuario ya autenticado** (ej: `PageHeader`):

```csharp
private string JwtUsuarioActual =>
    _jwt.Usuario?.Jwt
    ?? throw new InvalidOperationException("No hay usuario autenticado con token JWT válido.");
```

Se aplica en: `ObtenerCodigosDeMenusFavoritos()`, `GuardarCodigosDeMenusFavoritos(...)`.

#### Estrategia B — Token explícito del caller

Para `RegistrarLog`, que puede invocarse **durante el flujo de autenticación inicial** cuando
`IJwt.Usuario` todavía es `null`.

> ⚠️ **Regla crítica:** Si `IJwt.Usuario` es `null` y se usa `JwtUsuarioActual`, se lanza
> `InvalidOperationException` que el middleware interpreta como acceso no autorizado y redirige a
> `/Unauthorized`. El caller (`RegistroAplicacion`, `RegistroErroresCore`) ya tiene el usuario
> cargado y pasa su `usuario.Jwt` directamente.

```csharp
// ✅ CORRECTO — caller pasa el token explícitamente
await _clienteApiCore.RegistrarLog(usuario.Jwt, datosLog);

// ❌ MAL — no existe esta sobrecarga precisamente por esta razón
await _clienteApiCore.RegistrarLog(datosLog);
```

### 3.4 Cabecera de autorización por petición

**Nunca** modificar `DefaultRequestHeaders` (estado compartido → condiciones de carrera).  
Siempre usar `HttpRequestMessage` individual:

```csharp
// ✅ CORRECTO
var request = new HttpRequestMessage(HttpMethod.Post, urlEndpoint);
request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
await _httpClient.SendAsync(request);

// ❌ MAL
_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
```

---

## 4. Métodos públicos

| Método | Token JWT | Cuándo usar |
|---|---|---|
| `ObtenerCodigosDeMenusFavoritos()` | Automático (`IJwt`) | Solo con usuario autenticado (ej: `PageHeader`) |
| `GuardarCodigosDeMenusFavoritos(config)` | Automático (`IJwt`) | Solo con usuario autenticado |
| `RegistrarLog(jwtUsuario, datosLog)` | Explícito del caller | Siempre; válido también durante autenticación |

---

## 5. Cómo añadir un nuevo método

1. Declarar firma en `IClienteApiCore.cs` con XML doc.
2. Si solo se usa con usuario autenticado → `JwtUsuarioActual` (Estrategia A).
3. Si puede llamarse durante autenticación → `string jwtUsuario` como parámetro (Estrategia B).
4. GET: `HttpRequestMessage(HttpMethod.Get)` + `SendAsync`.
5. POST/PUT: `EnviarPostJsonAsync<T>(urlEndpoint, cuerpo, jwtOpcional)`.
6. **Nunca** `new HttpClient()`. **Nunca** `DefaultRequestHeaders`.

### Ejemplo — nuevo método GET con token explícito

```csharp
// IClienteApiCore.cs
Task<MiEntidad> ObtenerDato(string jwtUsuario, int id);

// ClienteApiCore.cs
public async Task<MiEntidad> ObtenerDato(string jwtUsuario, int id)
{
    string urlEndpoint = $"{_urlBaseApi}/api/v6/core/MiRecurso/{id}";
    try
    {
        var request = new HttpRequestMessage(HttpMethod.Get, urlEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtUsuario);
        HttpResponseMessage response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
            return JsonSerializer.Deserialize<MiEntidad>(await response.Content.ReadAsStringAsync())!;

        _logger.LogError("Error: {StatusCode}", response.StatusCode);
        throw new Exception($"Error llamada API -> {response.StatusCode}");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Excepcion en ObtenerDato");
        throw;
    }
}
```

---

## 6. Configuración

```json
{
  "ServicioCore": {
    "Core": "https://servidor-hmcore/api"
  }
}
```

---

## 7. Errores frecuentes y soluciones

| Error | Causa | Solución |
|---|---|---|
| Redirige a `/Unauthorized` al iniciar | `IJwt.Usuario` null al usar `JwtUsuarioActual` | Pasar token explícito (Estrategia B) |
| *Socket exhaustion* en producción | `new HttpClient()` dentro de los métodos | Usar `_httpClient` inyectado |
| Condición de carrera en headers | `DefaultRequestHeaders` modificado | `HttpRequestMessage` por petición |
| `InvalidOperationException: No hay usuario` | `JwtUsuarioActual` sin usuario en sesión | Verificar que el caller tiene usuario cargado |
