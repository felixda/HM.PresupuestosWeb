# Manejo de Errores

## Tipos de Error en el Proyecto

| Tipo | Clase | Cuándo |
|---|---|---|
| Error de validación de negocio | `ValidacionException` | Valor duplicado en BD (Descripción, Orden, BitAnd) |
| Error técnico | `Exception` | Fallo de BD, red, timeout |
| Error de estado inválido | `InvalidOperationException` | Acceso sin usuario autenticado |

---

## `ValidacionException` — Error de Dominio Tipado

En lugar de `Exception` genéricas, el dominio tiene su propia excepción para violaciones de unicidad. Vive en `HM.Presupuestos.Domain/Compartido/ValidacionException.cs`:

```csharp
public class ValidacionException : Exception
{
    public CampoErrorValidacion CampoValidado { get; }
    public string Valor { get; }

    public ValidacionException(CampoErrorValidacion campoValidado, string valor)
        : base("Error de validación")
    {
        CampoValidado = campoValidado;
        Valor = valor;
    }
}

public enum CampoErrorValidacion { Descripcion, Orden, BitAnd }
```

### Lanzar en el Servicio (Application)

```csharp
// IndicadoresService.cs — validación antes de persistir
public async Task Grabar(Indicador indicador, ...)
{
    if (indicador.Estado != EstadoEntidad.SinCambios)
    {
        if (await _indicadoresRepository.ExisteIndicador(indicador))
            throw new ValidacionException(CampoErrorValidacion.Descripcion, indicador.Descripcion);

        if (await _indicadoresRepository.ExisteOrden(indicador))
            throw new ValidacionException(CampoErrorValidacion.Orden, indicador.Orden.ToString());

        if (await _indicadoresRepository.ExisteBitAnd(indicador))
            throw new ValidacionException(CampoErrorValidacion.BitAnd, indicador.BitAnd.ToString());
    }
    // ... continuar con el guardado
}
```

### Capturar en la Página (Web)

La página captura `ValidacionException` **separado** de `Exception` para dar un mensaje específico al usuario:

```csharp
// Indicadores.razor.cs
try
{
    LayerOverlayService.Start();
    await GuardarDatosAsync();   // llama al servicio que puede lanzar ValidacionException
}
catch (ValidacionException exv)
{
    // Mensaje específico con el campo y valor duplicado
    await MensajesHelper.MostrarMensajeAviso(TituloPagina,
        ObtenerMensajeValidacion(exv.CampoValidado, exv.Valor));
}
catch (Exception ex)
{
    // Cualquier otro error técnico
    await ManejarExcepcion(ex);
}
finally
{
    LayerOverlayService.Stop();
}

private string ObtenerMensajeValidacion(CampoErrorValidacion campo, string valor)
{
    string nombreCampo = ObtenerTexto($"Common:{campo}:label");
    return string.Format(ObtenerTexto(AppResources.Mensajes.ValorCampoRepetido), nombreCampo, valor);
}
```

---

## `EjecutarAsync` — Wrapper de Error en Páginas

La clase base `Context` expone `EjecutarAsync(...)` para envolver operaciones async con **overlay automático + manejo de errores centralizado**. Es el patrón estándar en todas las páginas:

```csharp
// Sobrecarga más común — Task sin retorno
protected async Task EjecutarAsync(
    Func<Task> action,
    string? mensajePersonalizado = null,
    bool showOverlay = true)
{
    try
    {
        if (showOverlay) LayerOverlayService.Start();
        await action();
    }
    catch (Exception ex)
    {
        await ManejarExcepcion(ex, mensajePersonalizado);
    }
    finally
    {
        if (showOverlay) LayerOverlayService.Stop();
    }
}

// Sobrecarga con retorno — devuelve defaultValue si falla
protected async Task<TResult?> EjecutarAsync<TResult>(
    Func<Task<TResult>> func,
    TResult? defaultValue = default,
    string? customErrorMessage = null,
    bool showOverlay = true)
```

### Uso en Páginas

```csharp
// ✅ CORRECTO — operaciones de carga y guardado envueltas en EjecutarAsync
protected override async Task InicializarPaginaAsync()
{
    await EjecutarAsync(async () =>
    {
        DatosIndicadores = await IndicadoresService.ObtenerIndicadoresConIdiomas(null);
    });
}

private async Task ActualizarGridAsync()
{
    await EjecutarAsync(async () =>
    {
        DatosIndicadores = await IndicadoresService.ObtenerIndicadoresConIdiomas(null);
        GridIndicadores.Reload();
    });
}

// Sin overlay (operaciones rápidas de UI)
await EjecutarAsync(() =>
{
    FiltroActivo = true;
}, showOverlay: false);

// Con retorno
var items = await EjecutarAsync(
    async () => await VersionesService.ObtenerVersiones(anio),
    defaultValue: []
) ?? [];
```

```csharp
// ❌ INCORRECTO — try/catch manual en páginas para errores técnicos
private async Task CargarDatos()
{
    try
    {
        DatosIndicadores = await IndicadoresService.ObtenerIndicadoresConIdiomas(null);
    }
    catch (Exception ex)
    {
        // manejo ad-hoc en lugar de EjecutarAsync
        Console.WriteLine(ex.Message);
    }
}
```

---

## Errores de Auditoría — Nunca Propagan

Los errores al registrar en `ILogAccionesService` **nunca deben hacer fallar la operación principal**. Se capturan, se loguean y se continúa:

```csharp
// ✅ CORRECTO — auditoría post-commit, error silenciado con log
await transaction.CommitAsync();
try
{
    await _logAccionesService.Insertar(AccionesLog.GrabarIndicador, indicador);
}
catch (Exception logEx)
{
    _logger.LogError(logEx, "Error registrando auditoría (grabado exitoso)");
    // No propagar — la operación principal fue exitosa
}
```

El propio `LogAccionesService` también absorbe sus propios errores internamente para que ningún fallo de log llegue al llamador:

```csharp
// LogAccionesService.cs — errores de inserción de log no se propagan
public async Task Insertar(string accion, object? parametros = null, [CallerMemberName] string nombreMetodoLlamador = "")
{
    try
    {
        await _logAccionesRepository.Insertar(logAccion);
    }
    catch (Exception ex)
    {
        await InsertErrorLog(this.GetType().Name, ex, CodigoUsuario);
        // No propagar
    }
}
```

---

## Transacciones — Rollback en `catch`, Rethrow

Dentro de una transacción, cualquier error hace rollback y se relanza para que el llamador (la página) lo gestione:

```csharp
// ✅ CORRECTO — catch con rollback + rethrow
using var transaction = _indicadoresRepository.ObtenerTransaccion();
try
{
    await _indicadoresRepository.InsertarIndicador(indicador);
    // ... más operaciones
    await transaction.CommitAsync();
}
catch
{
    await transaction.RollbackAsync();
    throw;   // relanzar — la página decidirá cómo mostrar el error
}
```

```csharp
// ❌ INCORRECTO — capturar sin relanzar dentro de transacción
catch (Exception ex)
{
    await transaction.RollbackAsync();
    _logger.LogError(ex, "Error");
    // sin throw → la página no sabe que falló, puede mostrar éxito falso
}
```

---

## `InvalidOperationException` en Infraestructura

Las propiedades de repositorio que dependen del usuario autenticado lanzan `InvalidOperationException` si no hay sesión activa. Es un error de programación (acceso antes de autenticación), no de negocio:

```csharp
// BasePresupuestosRepository.cs
protected int CodigoUsuario =>
    Jwt.Usuario?.CodigoUsuario
    ?? throw new InvalidOperationException(
        "No se puede obtener el código de usuario: no hay un usuario autenticado válido.");
```

---

## Cuándo Lanzar vs Devolver Vacío

| Situación | Patrón |
|---|---|
| El valor no existe y es **obligatorio** para continuar | `throw new ValidacionException(...)` |
| La consulta no devuelve resultados (es válido) | Devolver `List<T>` vacía, nunca `null` |
| Acceso a recurso antes de estar disponible | `throw new InvalidOperationException(...)` |
| Error técnico en una transacción | `catch { RollbackAsync(); throw; }` |
| Error en auditoría / log | Capturar, loguear, **no relanzar** |

```csharp
// Devolver vacío — sin resultados es un estado válido
public async Task<List<Vigencia>> ObtenerVigencias(CondicionFiltro filtro)
{
    var resultado = new List<Vigencia>();
    // ... consulta a BD
    return resultado;  // puede ser vacío, nunca null
}

// Lanzar — condición de negocio violada
if (await _indicadoresRepository.ExisteIndicador(indicador))
    throw new ValidacionException(CampoErrorValidacion.Descripcion, indicador.Descripcion);
```

---

## Mensajes de Error al Usuario

Los mensajes se muestran siempre mediante `MensajesHelper`, nunca con alertas nativas de JavaScript ni mensajes inline en el markup:

```csharp
// Error técnico genérico
await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(AppResources.Mensajes.ErrorGeneral));

// Aviso de validación (dato duplicado)
await MensajesHelper.MostrarMensajeAviso(TituloPagina, mensajeValidacion);

// Información (sin resultados)
await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(AppResources.Mensajes.RegistrosNoEncontrados));

// Confirmación antes de acción destructiva
bool confirmado = await MensajesHelper.MostrarMensajeParaConfirmacion(
    TituloPagina,
    ObtenerTexto(AppResources.Mensajes.AvisoAntesCancelar));
if (!confirmado) return;
```

**Nunca incluir en los mensajes**: stack traces, nombres de tabla, rutas de fichero, mensajes internos de excepción.
