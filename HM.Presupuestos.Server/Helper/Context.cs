using HM.Presupuestos.Domain.Helper;
using HM.Presupuestos.Server.Modelos;
using System.Runtime.CompilerServices;
using System.Text.Json;
using ILocalizadorRecursos = HM.Presupuestos.Server.Services.ILocalizadorRecursos;


namespace HM.Presupuestos.Server.Helper
{

    public abstract class Context : ComponentBase, IDisposable
    {
        [Inject] protected IControlCambiosNavegacion ControlCambios { get; set; } = default!;
        [Inject] protected ISesionUsuario SesionUsuario { get; set; } = default!;
        [Inject] protected ILocalizadorRecursos LocalizadorRecursos { get; set; } = default!;
        [Inject] protected IMapaMenu MapaMenu { get; set; } = default!;
        [Inject] protected IGestorIdioma GestorIdioma { get; set; } = default!;
        [Inject] protected IRegistroAplicacion RegistroAplicacion { get; set; } = default!;
        [Inject] protected IVersionesService VersionesService { get; set; } = default!;
        [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] protected MensajesHelper MensajesHelper { get; set; } = default!;
        [Inject] protected ILayerOverlayService LayerOverlayService { get; set; } = default!;
        [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
        [Inject] protected IRutasNavegacion RutasNavegacion { get; set; } = default!;


        protected UsuarioApp? UsuarioApp { get; set; }

        /// <summary>
        /// Usuario actual(impersonado si hubiera y si no el SSO) con sus reglas, menús y permisos.
        /// </summary>
        protected UsuarioEntidad Usuario => UsuarioApp!.Usuario;

        protected UsuarioEntidad? UsuarioImpersonado => UsuarioApp!.ObtenerUsuarioImpersonado();
        protected UsuarioEntidad UsuarioSSO => UsuarioApp!.ObtenerUsuarioSSO();

        protected bool UsuarioCargado { get; private set; } = false;


        private async Task ActualizarIdioma()
        {
            await InvokeAsync(StateHasChanged);
        }

        protected async Task ActualizarEstadoCambios(bool conCambios = true)
        {
            await ControlCambios.ActualizarEstadoCambios(conCambios);
        }

        protected void LimpiarCambiosPendientes()
        {
            ControlCambios.LimpiarCambiosPendientes();
        }

        public bool UsuarioEsAdmin => Usuario?.Reglas?.Any(o => o.Id == Constantes.User.RULE_ADMIN) ?? false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                GestorIdioma.IdiomaCambiado += ActualizarIdioma;


                SesionUsuario.UsuarioCargado += async () =>
                {
                    UsuarioApp = SesionUsuario.UsuarioApp!;

                    if (UsuarioApp != null)
                    {
                        UsuarioCargado = true;

                        // Verificar si se desconectó el usuario login
                        if (UsuarioApp.ObtenerUsuarioImpersonado() == null)
                        {
                            await OnUsuarioLoginDesconectado();
                        }

                        // ? Aquí se llama cuando el usuario está disponible
                        await OnUsuarioDisponibleAsync();
                        await InvokeAsync(StateHasChanged);
                    }
                };


                // Llamamos por si ya estaba cargado
                UsuarioApp = SesionUsuario.UsuarioApp!;

                if (UsuarioApp != null)
                {
                    UsuarioCargado = true;
                    await OnUsuarioDisponibleAsync();
                    await InvokeAsync(StateHasChanged);
                }
            }
        }


        protected virtual async Task OnUsuarioDisponibleAsync()
        {
            TituloPagina = ObtenerTituloPagina();
            await Task.CompletedTask;
        }


        protected virtual async Task OnUsuarioLoginDesconectado()
        {
            UsuarioCargado = true;
            await InvokeAsync(StateHasChanged);
        }


        public virtual void Dispose()
        {
            GestorIdioma.IdiomaCambiado -= ActualizarIdioma;
        }



    
        /// <summary>
        /// Obtiene el valor traducido de una clave de recurso
        /// </summary>
        /// <param name="claveRecurso">Clave del recurso (ej: "Common:Aceptar:label")</param>
        /// <returns>Texto traducido según el idioma actual</returns>
        protected string ObtenerTexto(string claveRecurso) => LocalizadorRecursos.ObtenerTexto(claveRecurso);

        /// <summary>
        /// Título de la página obtenido automáticamente desde recursos.
        /// Disponible en todas las páginas sin necesidad de calcularlo manualmente.
        /// </summary>
        protected string TituloPagina { get; private set; } = string.Empty;

        /// <summary>
        /// Calcula el título de la página desde los recursos buscando por la URL actual.
        /// Sobrescribir para personalizar el título en páginas concretas.
        /// </summary>
        protected virtual string ObtenerTituloPagina()
        {
            var url = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            var urlNormalizada = RutasNavegacion.NormalizarRuta(url);
            return MapaMenu.ObtenerEtiquetaMenuPorUrl(urlNormalizada);
        }


        public async Task EstablecerMenuActivo(int code)
        {
            await JSRuntime.InvokeVoidAsync("Menu.SetMenuActive", code);
        }


        public LogAccion CrearLogAccion(int codigoUsuario, string nombreMetodoLlamador, AccionesLog accion, object? objetoConParametros = null)
        {
            string parametrosJson = objetoConParametros != null
                   ? JsonSerializer.Serialize(objetoConParametros, new JsonSerializerOptions { WriteIndented = true })
                   : string.Empty;

            LogAccion logAccion = new()
            {
                CodigoUsuario = codigoUsuario,
                Accion = $"({nombreMetodoLlamador}) -> {accion.ObtenerDescripcion()} ",
                Parametros = parametrosJson
            };

            return logAccion;
        }



        public string ObtenerValoresSeleccionados<T, TValue>(IEnumerable<T>? listaObjetos,
           Func<T, TValue> selector, string separador = ",")
        {
            if (listaObjetos == null)
                return string.Empty;

            ArgumentNullException.ThrowIfNull(selector);

            return string.Join(separador, listaObjetos
                .Where(x => x is not null)
                .Select(x => (selector(x) ?? default!)?.ToString() ?? string.Empty));
        }

        public string ObtenerValoresSeleccionados<T, TValue>(object? listaObjetos,
            Func<T, TValue> selector, string separador = ",")
        {
            if (listaObjetos is not IEnumerable<T> lista)
                return string.Empty;

            ArgumentNullException.ThrowIfNull(selector);

            return string.Join(separador, lista
                .Where(x => x is not null)
                .Select(x => (selector(x) ?? default!)?.ToString() ?? string.Empty));
        }

        public List<int> ObtenerListaValoresSeleccionados<T, TValue>(object? listaObjetos,
            Func<T, TValue> selector, string separador = ",")
        {
            string cadena = ObtenerValoresSeleccionados(listaObjetos, selector, separador);

            if (string.IsNullOrWhiteSpace(cadena))
                return [];

            return [.. cadena
                .Split(separador, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => int.TryParse(x, out _))
                .Select(int.Parse)];
        }

        public string GetDropDownBoxTextoSeleccionados<T, TValue>(DropDownBoxQueryDisplayTextContext arg,
            Func<T, TValue> selector, string textoTodos = "", string separador = ",")
        {
            if (arg.Value is not IEnumerable<T> listaObjetos || !listaObjetos.Any())
                return textoTodos;

            var seleccionados = listaObjetos
                .Select(selector)
                .Where(v => v != null && !string.IsNullOrWhiteSpace(v.ToString()))
                .Select(v => v!.ToString()!)
                .ToList();

            if (seleccionados.Count == 0)
                return textoTodos;

            return string.Join(separador, seleccionados);
        }

        public string GetDropDownBoxTextoSeleccionados<T, TValue>(IEnumerable<T>? listaObjetos,
            Func<T, TValue> selector, string todos, string separador = ",")
        {
            if (listaObjetos == null)
                return todos;

            ArgumentNullException.ThrowIfNull(selector);

            return string.Join(separador, listaObjetos
                .Where(x => x is not null)
                .Select(x => (selector(x) ?? default!)?.ToString() ?? string.Empty));
        }

        public async Task<List<VersionResumen>> ObtenerVersionesPorPermisos(int anio)
        {
            if (UsuarioEsAdmin)
            {
                return await VersionesService.ObtenerVersionesResumen(anio, null, Constantes.BitAndVersion.REAL);
            }
            else
            {
                return await VersionesService.ObtenerVersionesResumen(anio, Constantes.BitAndVersion.PUBLICADA,
                    Constantes.BitAndVersion.REAL);
            }
        }

        /// <summary>
        /// Maneja una excepción generada por la base de datos mostrando error al usuario
        /// </summary>
        public async Task TratarExcepcionGeneradaEnBD(ExcepcionBaseDatos ex, string titulo)
        {
            await TratarExcepcionBaseDatos(ex, titulo, esWarning: false);
        }

        /// <summary>
        /// Maneja una excepción generada por la base de datos mostrando advertencia al usuario
        /// </summary>
        public async Task TratarWarningGeneradoEnBD(ExcepcionBaseDatos ex, string titulo)
        {
            await TratarExcepcionBaseDatos(ex, titulo, esWarning: true);
        }

        

        private async Task TratarExcepcionBaseDatos(ExcepcionBaseDatos ex, string titulo, bool esWarning)
        {
            bool esErrorControlado = Math.Abs(ex.Codigo) is >= 20001 and <= 20999;
            bool enviarErrorLogWatcher = Math.Abs(ex.Codigo) is >= 20001 and <= 20499;

            string? mensaje = null;

            // Intentar obtener mensaje traducido para errores controlados
            if (esErrorControlado)
            {
                string clave = $"MensajeErrorBD:{ex.Codigo}:label";
                mensaje = ExisteRecurso(clave)
                    ? ObtenerTexto(clave)
                    : (esWarning ? AppResources.Mensajes.DatabaseWarning : null);
            }

            // Registrar en log si es necesario y capturar si falló
            bool loggingFallo = false;
            if (!esErrorControlado || enviarErrorLogWatcher)
            {
                loggingFallo = await RegistrarExcepcionSeguro(ex);
            }

            // Agregar advertencia de logging fallido al mensaje si corresponde
            if (loggingFallo && mensaje != null)
            {
                mensaje += "\n\n" + AppResources.Mensajes.LoggingError;
            }

            // Mostrar mensaje según tipo
            if (esWarning)
            {
                await MensajesHelper.MostrarMensajeAviso(titulo, 
                    mensaje ?? AppResources.Mensajes.DatabaseWarning);
            }
            else
            {
                await MensajesHelper.MostrarMensajeError(titulo, mensaje);
            }
        }


        public bool ExisteRecurso(string claveRecurso)
        {
            return LocalizadorRecursos.ExisteRecurso(claveRecurso);
        }


        #region Ejecución (disponible para TODOS los componentes)

        /// <summary>
        /// Ejecuta una acción con gestión automática de overlay, logging y manejo de errores
        /// ? Disponible en TODOS los componentes (protegidos o no)
        /// </summary>
        /// <param name="action">Acción asíncrona a ejecutar</param>
        /// <param name="showOverlay">Si debe mostrar el overlay de carga (default: true)</param>
        /// <param name="customErrorMessage">Mensaje de error personalizado (opcional)</param>
        /// <example>
        /// <code>
        /// await EjecutarAsync(async () =>
        /// {
        ///     var data = await Service.LoadData();
        ///     Items = data;
        /// });
        /// </code>
        /// </example>
        protected async Task EjecutarAsync(
            Func<Task> action,
            string? customErrorMessage = null,
            bool showOverlay = true
            )
        {
            try
            {
                if (showOverlay)
                {
                    LayerOverlayService.Start();
                }

                await action();
            }
            catch (Exception ex)
            {
                // Log de la excepción
                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina, customErrorMessage);
            }
            finally
            {
                if (showOverlay)
                {
                    LayerOverlayService.Stop();
                }
            }
        }

        /// <summary>
        /// Ejecuta una función con gestión automática y retorna el resultado
        /// ? Retorna el valor por defecto si ocurre un error
        /// </summary>
        /// <typeparam name="TResult">Tipo de resultado</typeparam>
        /// <param name="func">Función asíncrona a ejecutar</param>
        /// <param name="defaultValue">Valor por defecto en caso de error</param>
        /// <param name="customErrorMessage"></param>
        /// <param name="showOverlay">Si debe mostrar el overlay de carga (default: true)</param>
        /// <returns>Resultado de la función o valor por defecto</returns>
        /// <example>
        /// <code>
        /// var items = await EjecutarAsync(
        ///     async () => await Service.GetItems(),
        ///     defaultValue: new List&lt;Item&gt;(),
        /// ) ?? [];
        /// </code>
        /// </example>
        protected async Task<TResult?> EjecutarAsync<TResult>(
            Func<Task<TResult>> func,
            TResult? defaultValue = default,
            string? customErrorMessage = null,
            bool showOverlay = true)
        {
            try
            {
                if (showOverlay)
                {
                    LayerOverlayService.Start();
                }

                return await func();
            }
            catch (Exception ex)
            {
                await RegistroAplicacion.RegistrarExcepcion(ex);

                await RegistroAplicacion.RegistrarExcepcion(ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina, customErrorMessage);

                return defaultValue;
            }
            finally
            {
                if (showOverlay)
                {
                    LayerOverlayService.Stop();
                }
            }
        }

        /// <summary>
        /// Ejecuta una acción síncrona con gestión automática
        /// </summary>
        /// <param name="action">Acción síncrona a ejecutar</param>
        /// <param name="customErrorMessage">Mensaje de error personalizado (opcional)</param>
        /// <param name="showOverlay">Si debe mostrar el overlay de carga (default: true)</param>
        /// <example>
        /// <code>
        /// await EjecutarAsync(() =>
        /// {
        ///     Items = Service.GetItemsSync();
        /// });
        ///
        /// // Sin overlay:
        /// await EjecutarAsync(() =>
        /// {
        ///     FiltroActivo = true;
        /// }, showOverlay: false);
        /// </code>
        /// </example>
        protected async Task EjecutarAsync(
            Action action,
            string? customErrorMessage = null,
            bool showOverlay = true)
        {
            await EjecutarAsync(
                () =>
                {
                    action();
                    return Task.CompletedTask;
                },
                customErrorMessage,
                showOverlay);
        }

        #endregion


        /// <summary>
        /// Maneja excepciones de forma centralizada: registra el error y muestra mensaje al usuario
        /// </summary>
        /// <param name="ex">Excepción capturada</param>
        /// <param name="pageTitle">Título de la página</param>
        /// <param name="mensajePersonalizado">Mensaje personalizado opcional</param>
        /// <param name="nombreMetodoLlamador">Nombre del método que llamó a esta función</param>
        /// <param name="callerFilePath">Ruta del archivo que llama (se captura automáticamente)</param>
        protected async Task ManejarExcepcion(Exception ex,
            string pageTitle,
            string? mensajePersonalizado = null,
            [CallerMemberName] string nombreMetodoLlamador = "",
            [CallerFilePath] string callerFilePath = "")
        {
            // ✅ Construir categoría automáticamente: "NombreClase.NombreMetodo"
            var className = ExtractClassNameFromFilePath(callerFilePath);
            var category = $"{className}.{nombreMetodoLlamador}";

            // ✅ Registrar con fallback seguro y capturar si falló
            bool loggingFallo = await RegistrarExcepcionSeguro(ex, category);

            // Construir mensaje para el usuario
            var mensaje = mensajePersonalizado ?? AppResources.Mensajes.ErrorGeneral;
                //GetResourceValue("mensajes:ErrorGenerico:label");

            // Agregar advertencia de logging fallido si corresponde
            if (loggingFallo)
            {
                mensaje += "\n\n" + AppResources.Mensajes.ErrorGeneral; 
                //Añadir entrada error login
            }

            await MensajesHelper.MostrarMensajeError(pageTitle, mensaje);
        }

        /// <summary>
        /// Extrae el nombre de la clase desde la ruta completa del archivo
        /// </summary>
        /// <param name="filePath">Ruta completa del archivo (ej: C:\Proyectos\...\Pages\Versiones.razor.cs)</param>
        /// <returns>Nombre de la clase (ej: Versiones)</returns>
        private static string ExtractClassNameFromFilePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return "Unknown";

            // ✅ Obtener nombre del archivo sin extensión
            var fileName = Path.GetFileNameWithoutExtension(filePath);

            // ✅ Remover extensiones múltiples (.razor.cs → Versiones)
            if (fileName.EndsWith(".razor"))
                fileName = fileName[..^6]; // Remover ".razor"

            return fileName;
        }

        /// <summary>
        /// Registra una excepción con múltiples niveles de fallback para garantizar que siempre se registre
        /// </summary>
        /// <param name="ex">Excepción a registrar</param>
        /// <param name="category">Categoría de la excepción</param>
        /// <returns>True si hubo un fallo total al registrar la excepción, False en caso contrario</returns>
        private async Task<bool> RegistrarExcepcionSeguro(Exception ex, string? category = null)
        {
            try
            {
                // Nivel 1: Comportamiento normal (API + Archivo)
                await RegistroAplicacion.RegistrarExcepcion(category, ex);
                return false; // ✅ Éxito
            }
            catch (Exception exLogging)
            {
                try
                {
                    // Nivel 2: Fallback a solo archivo NLog
                    await RegistroAplicacion.RegistrarExcepcion(
                        ex,
                        comments: "[FALLBACK] Error al registrar en API",
                        insertDBLog: false,  // ❌ Skip API
                        insertFileLog: true  // ✅ Solo NLog
                    );
                    return false; // ✅ Éxito en fallback
                }
                catch (Exception exFallback)
                {
                    // Nivel 3: Console como último recurso
                    Console.WriteLine($"[CRITICAL] Error registrando excepción:");
                    Console.WriteLine($"  - Excepción original: {ex.Message}");
                    Console.WriteLine($"  - Error logging API: {exLogging.Message}");
                    return true; // ❌ Fallo total (notifica al usuario)
                }
            }
        }


    }
}