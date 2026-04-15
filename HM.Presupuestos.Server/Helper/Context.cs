using HM.Presupuestos.Contratos.Helper;
using HM.Presupuestos.Server.Modelos;
using HM.Presupuestos.Server.Servicios;
using System.Text.Json;


namespace HM.Presupuestos.Server.Helper
{

    public class Context : ComponentBase, IDisposable
    {
        [Inject] protected ControlCambiosService ControlCambios { get; set; } = default!;
        [Inject] protected IUsuarioServicio UsuarioService { get; set; } = default!;
        [Inject] protected IResourceService ResourceService { get; set; } = default!;
        [Inject] protected IIdiomaService IdiomaService { get; set; } = default!;

        [Inject] protected ILogService LogService { get; set; } = default!;
        [Inject] protected IVersionesService VersionesService { get; set; } = default!;

        [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;

        [Inject] protected MensajesHelper MensajesHelper { get; set; } = default!;
        [Inject] protected ILayerOverlayService LayerOverlayService { get; set; } = default!;


        protected UsuarioApp? UsuarioApp { get; set; }

        protected UsuarioEntidad Usuario => UsuarioApp!.Usuario;

        protected UsuarioEntidad? UsuarioLogin => UsuarioApp!.ObtenerUsuarioLogin();
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
                IdiomaService.OnIdiomaCambiado += ActualizarIdioma;


                UsuarioService.OnUsuarioCargado += async () =>
                {
                    UsuarioApp = UsuarioService.UsuarioApp!;

                    if (UsuarioApp != null)
                    {
                        UsuarioCargado = true;

                        // Verificar si se desconectó el usuario login
                        if (UsuarioApp.ObtenerUsuarioLogin() == null)
                        {
                            await OnUsuarioLoginDesconectado();
                        }

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
            await Task.CompletedTask;
        }


        protected virtual async Task OnUsuarioLoginDesconectado()
        {
            UsuarioCargado = true;
            await InvokeAsync(StateHasChanged);
        }


        public virtual void Dispose()
        {
            IdiomaService.OnIdiomaCambiado -= ActualizarIdioma;
        }



        /// <summary>
        /// Get resource value by expression
        /// </summary>
        /// <param name="elementExpression">Element expression</param>
        /// <returns>Language code</returns>
        public string T(string elementExpression)
        {
            return ResourceService.T(elementExpression);
        }


        public async Task SetMenuActive(int code)
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



        public string GetValoresSeleccionados<T, TValue>(IEnumerable<T>? listaObjetos,
           Func<T, TValue> selector, string separador = ",")
        {
            if (listaObjetos == null)
                return string.Empty;

            ArgumentNullException.ThrowIfNull(selector);

            return string.Join(separador, listaObjetos
                .Where(x => x is not null)
                .Select(x => (selector(x) ?? default!)?.ToString() ?? string.Empty));
        }

        public string GetValoresSeleccionados<T, TValue>(object? listaObjetos,
            Func<T, TValue> selector, string separador = ",")
        {
            if (listaObjetos is not IEnumerable<T> lista)
                return string.Empty;

            ArgumentNullException.ThrowIfNull(selector);

            return string.Join(separador, lista
                .Where(x => x is not null)
                .Select(x => (selector(x) ?? default!)?.ToString() ?? string.Empty));
        }

        public List<int> GetListaValoresSeleccionados<T, TValue>(object? listaObjetos,
            Func<T, TValue> selector, string separador = ",")
        {
            string cadena = GetValoresSeleccionados(listaObjetos, selector, separador);

            if (string.IsNullOrWhiteSpace(cadena))
                return new List<int>();

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
                mensaje = ExisteResourceValue(clave)
                    ? T(clave)
                    : null;
            }

            if (!esErrorControlado || enviarErrorLogWatcher)
            {
                var excepcion = new Exception(ex.Message);
                await LogService.InsertException(GetType().Name, excepcion);
            }

            await MensajesHelper.MostrarMensajeError(titulo, mensaje);
        }


        public bool ExisteResourceValue(string elementExpression)
        {
            return ResourceService.ExisteValue(elementExpression);
        }
    }
}