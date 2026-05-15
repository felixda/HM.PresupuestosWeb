using HM.Presupuestos.Domain.Helper;
using HM.Presupuestos.Server.Modelos;
using HM.Presupuestos.Server.Servicios;
using System.ComponentModel.Design;
using System.Text.Json;
using ILocalizadorRecursos = HM.Presupuestos.Server.Services.ILocalizadorRecursos;


namespace HM.Presupuestos.Server.Helper
{

    public abstract class Context : ComponentBase, IDisposable
    {
        [Inject] protected ControlCambiosService ControlCambios { get; set; } = default!;
        [Inject] protected ISesionUsuario UsuarioService { get; set; } = default!;
        [Inject] protected ILocalizadorRecursos TraductorRecursos { get; set; } = default!;
        [Inject] protected IMapaMenu MapaMenu { get; set; } = default!;
        [Inject] protected IGestorIdioma GestorIdioma { get; set; } = default!;

        [Inject] protected IRegistroAplicacion LogService { get; set; } = default!;
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

        protected async Task MarcarCambios(bool conCambios = true)
        {
            await ControlCambios.MarcarCambios(conCambios);
        }

        protected void LimpiarCambios()
        {
            ControlCambios.LimpiarCambios();
        }

        public bool UsuarioEsAdmin => Usuario?.Reglas?.Any(o => o.Id == Constantes.User.RULE_ADMIN) ?? false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                GestorIdioma.IdiomaCambiado += ActualizarIdioma;


                UsuarioService.UsuarioCargado += async () =>
                {
                    UsuarioApp = UsuarioService.UsuarioApp!;

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
                UsuarioApp = UsuarioService.UsuarioApp!;

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
        protected string ObtenerTexto(string claveRecurso) => TraductorRecursos.ObtenerTexto(claveRecurso);

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
            //var codigoMenu = MapaMenu.ObtenerCodigoMenuPorUrl(urlNormalizada);
            //if (codigoMenu < 0) return string.Empty;
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


        public async Task TratarExcepcionGeneradaEnBD(ExcepcionBaseDatos ex, string titulo)
        {
            bool esErrorControlado = Math.Abs(ex.Codigo) >= 20001 && Math.Abs(ex.Codigo) <= 20999;
            bool enviarErrorLogWatcher = Math.Abs(ex.Codigo) >= 20001 && Math.Abs(ex.Codigo) <= 20499;

            string? mensaje = null;

            if (esErrorControlado)
            {
                string clave = $"MensajeErrorBD:{ex.Codigo}:label";
                mensaje = ExisteRecurso(clave)
                    ? ObtenerTexto(clave)
                    : null;
            }

            if (!esErrorControlado || enviarErrorLogWatcher)
            {
                var excepcion = new Exception(ex.Message);
                await LogService.RegistrarExcepcion(excepcion);
            }

            await MensajesHelper.MostrarMensajeError(titulo, mensaje);
        }


        public bool ExisteRecurso(string claveRecurso)
        {
            return TraductorRecursos.ExisteRecurso(claveRecurso);
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
                await LogService.RegistrarExcepcion(ex);
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
                await LogService.RegistrarExcepcion(ex);

                await LogService.RegistrarExcepcion(ex);
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


    }
}