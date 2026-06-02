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
		├── IClienteApiCore.cs   ← Contrato (puerto secundario)
		└── ClienteApiCore.cs    ← Implementación
```

Sigue el patrón **Puerto/Adaptador** de la arquitectura hexagonal:  
- La interfaz `IClienteApiCore` define el contrato que consume la capa `Application`/`Web`.  
- La implementación `ClienteApiCore` es el adaptador que conoce los detalles HTTP de HM.CORE.

---

## 3. Diseño técnico

### 3.1 Registro en DI

```csharp
// Program.cs
builder.Services.AddHttpClient<IClienteApiCore, ClienteApiCore>();
```

Se usa **Typed HttpClient** (`AddHttpClient<TInterface, TImpl>`):
- El `HttpClient` es gestionado por el pool de `IHttpClientFactory` → sin *socket exhaustion*.
- El ciclo de vida del servicio es **Transient** (uno por resolución).

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

Este es el punto de diseño más importante del componente.

#### Estrategia A — Token desde `IJwt` (automático)

Usada en métodos que solo se invocan cuando el usuario **ya está autenticado**:

```csharp
private string JwtUsuarioActual =>
	_jwt.Usuario?.Jwt
	?? throw new InvalidOperationException("No hay un usuario autenticado con token JWT válido.");
```

Se aplica en: `ObtenerCodigosDeMenusFavoritos()`, `GuardarCodigosDeMenusFavoritos(...)`.

#### Estrategia B — Token explícito del caller

Usada en `RegistrarLog(string jwtUsuario, ...)` porque **puede invocarse durante el flujo de autenticación inicial**, momento en que `IJwt.Usuario` todavía es `null`.

> ⚠️ **Regla crítica:** Si `IJwt.Usuario` es `null` en ese momento y se intenta obtener el token automáticamente, se lanza `InvalidOperationException` que el middleware interpreta como acceso no autorizado y redirige a `/Unauthorized`.
>
> **Solución:** El caller (`RegistroAplicacion`, `RegistroErroresCore`) ya tiene el usuario cargado en memoria y pasa su `usuario.Jwt` directamente.

```csharp
// ✅ CORRECTO — caller pasa el token explícitamente
await _clienteApiCore.RegistrarLog(usuario.Jwt, datosLog);

// ❌ MAL — no usar en contextos de autenticación inicial
await _clienteApiCore.RegistrarLog(datosLog); // no existe esta sobrecarga por esta razón
```

### 3.4 Cabecera de autorización por petición

Para evitar condiciones de carrera en el `HttpClient` compartido, **nunca** se modifica `DefaultRequestHeaders`. El token se aplica en cada `HttpRequestMessage` individual:

```csharp
// ✅ CORRECTO — header en la petición individual
var request = new HttpRequestMessage(HttpMethod.Post, urlEndpoint);
request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
await _httpClient.SendAsync(request);

// ❌ MAL — modifica estado compartido del HttpClient
_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
```

---

## 4. Métodos públicos

| Método | Token JWT | Cuándo usar |
|---|---|---|
| `ObtenerCodigosDeMenusFavoritos()` | Automático (`IJwt`) | Solo desde componentes con usuario autenticado (ej: `PageHeader`) |
| `GuardarCodigosDeMenusFavoritos(config)` | Automático (`IJwt`) | Solo desde componentes con usuario autenticado |
| `RegistrarLog(jwtUsuario, datosLog)` | Explícito del caller | Siempre que se registre un log; también válido durante autenticación |

---

## 5. Cómo añadir un nuevo método

1. **Declarar** la firma en `IClienteApiCore.cs` con su XML doc.
2. **Implementar** en `ClienteApiCore.cs`:
   - Si el método solo se usa con usuario autenticado → usar `JwtUsuarioActual` (Estrategia A).
   - Si el método puede llamarse durante la autenticación → recibir `string jwtUsuario` como parámetro (Estrategia B).
3. Para **GET**: construir `HttpRequestMessage(HttpMethod.Get, url)` y llamar `_httpClient.SendAsync(request)`.
4. Para **POST/PUT**: usar `EnviarPostJsonAsync<T>(urlEndpoint, cuerpo, jwtOpcional)`.
5. **No** crear instancias de `HttpClient` con `new HttpClient()`.
6. **No** modificar `_httpClient.DefaultRequestHeaders`.

### Ejemplo de nuevo método GET con token explícito

```csharp
// En IClienteApiCore.cs
Task<MiEntidad> ObtenerDato(string jwtUsuario, int id);

// En ClienteApiCore.cs
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

La URL base se lee de `appsettings.json`:

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
| Redirige a `/Unauthorized` al iniciar | `IJwt.Usuario` es `null` al llamar a un método que usa `JwtUsuarioActual` | Pasar el token explícitamente (Estrategia B) |
| *Socket exhaustion* en producción | Uso de `new HttpClient()` dentro de los métodos | Usar siempre el `_httpClient` inyectado |
| Condición de carrera en `DefaultRequestHeaders` | Modificar headers globales del `HttpClient` compartido | Usar `HttpRequestMessage` con headers por petición |
| `InvalidOperationException: No hay usuario autenticado` | Llamar a `JwtUsuarioActual` sin usuario en sesión | Verificar que el caller tiene usuario cargado o usar Estrategia B |
