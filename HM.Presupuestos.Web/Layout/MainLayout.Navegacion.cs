namespace HM.Presupuestos.Web.Layout
{
    public partial class MainLayout
    {
        #region Navigation

        private async void OnLocationChangedAsync(object? sender, LocationChangedEventArgs e)
        {
            Console.WriteLine($"[MainLayout] ðŸ”„ OnLocationChangedAsync: {e.Location}");

            try
            {
                var urlNormalizada = RutasNavegacion.NormalizarRuta(e.Location);
                if (urlNormalizada == _ultimaRutaVisitada)
                {
                    Console.WriteLine($"[MainLayout] URL duplicada en navegaciónn, saltando procesamiento: {urlNormalizada}");
                    return;
                }

                _ultimaRutaVisitada = urlNormalizada;

                if (!EsPaginaIndex(urlNormalizada))
                {
                    await RegistrarAccesoPagina(urlNormalizada);
                }
                else
                {
                    Console.WriteLine($"[MainLayout] â­ï¸ PÃ¡gina Index/Home detectada, no se registra log de acceso");
                }

                await ActualizarSubscripcionesInactividad();

                var loginSSO = SesionUsuario.UsuarioApp?.UsuarioAutenticado?.Login;
                if (!string.IsNullOrEmpty(loginSSO))
                {
                    RegistroSesionesActivas.ActualizarPagina(loginSSO, urlNormalizada);
                }

                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MainLayout] Error en OnLocationChangedAsync: {ex.Message}");
                await RegistroAplicacion.RegistrarExcepcion(nameof(MainLayout), ex);
            }
        }

        /// <summary>
        /// Verifica si la URL corresponde a la principal (Index/Home)
        /// para evitar registrar logs de acceso a la pÃ¡gina de inicio
        /// </summary>
        private static bool EsPaginaIndex(string urlNormalizada)
        {
            if (string.IsNullOrEmpty(urlNormalizada))
                return true;

            var urlLower = urlNormalizada.ToLower().Trim('/');

            return urlLower == string.Empty ||
                   urlLower == "home" ||
                   urlLower == "index";
        }

        /// <summary>
        /// Controla la navegaciÃ³n interna dentro de la aplicaciÃ³n,
        /// mostrando un modal de confirmaciÃ³n si hay cambios pendientes
        /// </summary>
        private async Task OnBeforeInternalNavigation(LocationChangingContext context)
        {
            bool permitir = await ControlCambiosNavegacion.PuedeAbandonarPagina(context.TargetLocation);

            if (!permitir)
            {
                context.PreventNavigation();
            }
            else
            {
                ControlCambiosNavegacion.LimpiarCambiosPendientes();
            }
        }

        private async Task<bool> MostrarConfirmacionCambiosPendientes(string mensaje)
        {
            if (ModalConfirmacionCambios is not null)
            {
                return await ModalConfirmacionCambios.Show(mensaje);
            }

            return true;
        }

        private async Task RegistrarAccesoPagina(string urlNormalizada)
        {
            try
            {
                if (urlNormalizada == _ultimaUrlRegistrada)
                {
                    Console.WriteLine($"[MainLayout] â­ï¸ URL duplicada, no se registra: {urlNormalizada}");
                    return;
                }

                string tituloParaLog = RecursosApp.ObtenerEtiquetaMenuPorUrl(urlNormalizada);

                await RegistroAplicacion.RegistrarAccesoAPagina(tituloParaLog);

                _ultimaUrlRegistrada = urlNormalizada;

                Console.WriteLine($"[MainLayout] âœ… Log registrado: {tituloParaLog} ({urlNormalizada})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MainLayout] âš ï¸ Error al registrar log: {ex.Message}");
                await RegistroAplicacion.RegistrarExcepcion(nameof(MainLayout), ex);
            }
        }

        #endregion
    }
}

