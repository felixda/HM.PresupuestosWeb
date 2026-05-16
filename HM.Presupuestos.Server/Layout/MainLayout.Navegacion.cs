namespace HM.Presupuestos.Server.Layout
{
    public partial class MainLayout
    {
        #region Navigation

        private async void OnLocationChangedAsync(object? sender, LocationChangedEventArgs e)
        {
            Console.WriteLine($"[MainLayout] 🔄 OnLocationChangedAsync: {e.Location}");

            try
            {
                var urlNormalizada = RutasNavegacion.NormalizarRuta(e.Location);
                if (urlNormalizada == _ultimaRutaVisitada)
                {
                    Console.WriteLine($"[MainLayout] ⏭️ URL duplicada en navegación, saltando procesamiento: {urlNormalizada}");
                    return;
                }

                _ultimaRutaVisitada = urlNormalizada;

                if (!EsPaginaIndex(urlNormalizada))
                {
                    await RegistrarAccesoPagina(urlNormalizada);
                }
                else
                {
                    Console.WriteLine($"[MainLayout] ⏭️ Página Index/Home detectada, no se registra log de acceso");
                }

                await ActualizarSubscripcionesInactividad();
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MainLayout] ❌ Error en OnLocationChangedAsync: {ex.Message}");
                await RegistroAplicacion.RegistrarExcepcion(nameof(MainLayout), ex);
            }
        }

        /// <summary>
        /// Verifica si la URL corresponde a la principal (Index/Home)
        /// para evitar registrar logs de acceso a la página de inicio
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
        /// Controla la navegación interna dentro de la aplicación,
        /// mostrando un modal de confirmación si hay cambios pendientes
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

        private async Task<bool> MostrarConfirmacionCambiosPendientes(string destino)
        {
            if (ModalConfirmacionCambios is not null)
            {
                string mensaje = AppResources.Mensajes.AvisoCambiosPendientes;
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
                    Console.WriteLine($"[MainLayout] ⏭️ URL duplicada, no se registra: {urlNormalizada}");
                    return;
                }

                string tituloParaLog = MapaMenu.ObtenerEtiquetaMenuPorUrl(urlNormalizada);

                await RegistroAplicacion.RegistrarAccesoAPagina(tituloParaLog);

                _ultimaUrlRegistrada = urlNormalizada;

                Console.WriteLine($"[MainLayout] ✅ Log registrado: {tituloParaLog} ({urlNormalizada})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MainLayout] ⚠️ Error al registrar log: {ex.Message}");
                await RegistroAplicacion.RegistrarExcepcion(nameof(MainLayout), ex);
            }
        }

        #endregion
    }
}
