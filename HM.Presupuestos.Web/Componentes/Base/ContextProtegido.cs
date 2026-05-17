using DocumentFormat.OpenXml.Spreadsheet;

public abstract class ContextProtegido : Context
{
    #region Servicios de Seguridad

    [Inject] protected IControlAccesoNavegacion PermisosService { get; set; } = default!;
    [Inject] protected DialogoErrores ErrorService { get; set; } = default!;
    [Inject] protected IRutasNavegacion NavigationService { get; set; } = default!;



    #endregion

    #region Propiedades para UI

    /// <summary>
    /// Indica si se está validando los permisos del usuario
    /// Útil para mostrar spinner/mensaje de carga en UI
    /// </summary>
    protected bool ValidandoPermisos { get; private set; } = true;

    /// <summary>
    /// Indica si el usuario tiene permiso para acceder a la página
    /// Se actualiza después de validar permisos
    /// </summary>
    protected bool? TienePermiso { get; private set; } = null;

    #endregion

    #region Propiedades Abstractas

    /// <summary>
    /// Código del menú asociado a la página (para validación de permisos)
    /// </summary>
   // protected abstract CodigosMenu CodigoMenuPermiso { get; }

    #endregion

    #region Ciclo de Vida con Validación de Permisos

    //protected override async Task OnAfterRenderAsync(bool firstRender)
    //{
    //    if (firstRender)
    //    {
    //        await base.OnAfterRenderAsync(firstRender);

    //        // Validar permisos cuando el usuario esté disponible
    //        //if (UsuarioCargado)
    //        //{
    //        //    await ValidarPermisosAsync();
    //        //}
    //    }
    //}

    protected override async Task OnUsuarioDisponibleAsync()
    {
        await base.OnUsuarioDisponibleAsync();

        Console.WriteLine($"[ContextProtegido] ?? OnUsuarioDisponibleAsync - Usuario: {Usuario?.Login ?? "NULL"}, UsuarioCargado: {UsuarioCargado}");

        // ? Verificación estricta: Usuario DEBE existir
        if (UsuarioApp == null || Usuario == null || !UsuarioCargado)
        {
            Console.WriteLine($"[ContextProtegido] ?? Usuario no disponible, esperando...");

            // Intentar esperar un poco y reintentar (solo una vez)
            await Task.Delay(100);

            if (UsuarioApp == null || Usuario == null)
            {
                Console.WriteLine($"[ContextProtegido] ? Usuario sigue NULL después de esperar");
                return;
            }
        }

        await ValidarPermisosAsync();
    }

    /// <summary>
    /// Valida si el usuario tiene permisos para acceder a la página
    /// </summary>
    private async Task ValidarPermisosAsync()
    {
        // ? Verificación adicional de seguridad
        if (UsuarioApp == null || Usuario == null)
        {
            Console.WriteLine($"[ContextProtegido] ?? Abortando ValidarPermisosAsync - Usuario NULL");
            TienePermiso = false;
            ValidandoPermisos = false;
            await InvokeAsync(StateHasChanged);
            return;
        }

        ValidandoPermisos = true;
        await InvokeAsync(StateHasChanged);

        try
        {
            string url = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            var urlNormalizada = NavigationService.NormalizarRuta(url);

            TienePermiso = PermisosService.PuedeAccederA(urlNormalizada);

            if ((bool)TienePermiso)
            {
                await OnPermisoValidadoAsync();
            }
            else
            {
                await RegistroAplicacion.RegistrarIntentoAccesoNoAutorizado(urlNormalizada);
                await OnPermisoDenegadoAsync();
            }
        }
        catch (Exception ex)
        {
            await RegistroAplicacion.RegistrarExcepcion(ex);
            TienePermiso = false;
        }
        finally
        {
            ValidandoPermisos = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    #endregion

    #region Template Methods - Ciclo de Vida con Permisos

    /// <summary>
    /// ? Método plantilla que encapsula el patrón completo de inicialización
    /// Maneja automáticamente: título, overlay, logging, errores
    /// ? NO sobrescribir en páginas hijas, usar InicializarPaginaAsync() en su lugar
    /// </summary>
    protected virtual async Task OnPermisoValidadoAsync()
    {
        try
        {
            // 1. Mostrar overlay con mensaje de carga
            MostrarOverlayCarga(TituloPagina);

            // 2. Ejecutar inicialización específica de la página (hook point)
            await InicializarPaginaAsync();

            // 3. Notificar cambio de estado
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await RegistroAplicacion.RegistrarExcepcion(ex);
            await ErrorService.MostrarErrorInicializandoPagina(TituloPagina, ex);
        }
        finally
        {
            LayerOverlayService.Stop();
        }
    }

    /// <summary>
    /// Obtiene el título de la página desde el menú del usuario usando CodigoMenuPermiso.
    /// </summary>
    //protected override string ObtenerTituloPagina()
    //{
    //    return ObtenerTexto(AppResources.Menu.ObtenerEtiqueta((int)CodigoMenuPermiso));
    //}

    /// <summary>
    /// Muestra el overlay con mensaje de carga personalizado
    /// </summary>
    /// <param name="tituloPagina">Título de la página</param>
    protected virtual void MostrarOverlayCarga(string tituloPagina)
    {
        var mensajeCarga = $"{ObtenerTexto(AppResources.Common.Loading)} {tituloPagina}";
        LayerOverlayService.Start(mensajeCarga);
    }

    /// <summary>
    /// ? HOOK METHOD: Implementar en cada página para su inicialización específica
    /// Se ejecuta automáticamente con gestión de errores y overlay
    /// </summary>
    /// <returns>Task de inicialización</returns>
    protected abstract Task InicializarPaginaAsync();

    /// <summary>
    /// Se ejecuta cuando el usuario no tiene permisos (hook opcional)
    /// </summary>
    protected abstract Task OnPermisoDenegadoAsync();

    #endregion
}

