
namespace HM.Presupuestos.Server.Pages.Mantenimientos
{
    /// <summary>
    /// Página de mantenimiento de Indicadores
    /// Gestiona el CRUD de indicadores con sus traducciones multiidioma
    /// </summary>
    public partial class Indicadores : ContextProtegido
    {
        #region Inyección de Dependencias

        [Inject] protected IIndicadoresService IndicadoresService { get; set; } = default!;
        [Inject] protected ErrorDialogService ErrorService { get; set; } = default!;

        #endregion

        #region Propiedades para Data Binding 

        private string TituloPagina { get; set; } = string.Empty;

        private DxGrid GridIndicadores { get; set; } = new DxGrid();

        /// <summary>
        /// Indicadores cargados desde base de datos
        /// </summary>
        private List<Indicador> DatosIndicadores { get; set; } = [];

        /// <summary>
        /// Indicador actualmente en edición o creación
        /// </summary>
        private Indicador IndicadorEnEdicion { get; set; } = new();


        private bool EsPopupEdicionVisible { get; set; }

        /// <summary>
        /// Referencia al grid de idiomas dentro del popup
        /// </summary>
        private DxGrid GridIdiomasIndicador { get; set; } = new();

        /// <summary>
        /// Idiomas disponibles para selección
        /// </summary>
        private IEnumerable<Idioma> Idiomas { get; set; } = [];

        #endregion


        #region Ciclo de Vida del Componente

        /// <summary>
        /// Se ejecuta cuando el usuario no tiene permisos para acceder a la página
        /// </summary>
        protected override Task OnPermisoDenegadoAsync()
        {
            Console.WriteLine("[Indicadores] ❌ Permiso denegado");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Se ejecuta cuando el usuario tiene permisos válidos para acceder
        /// Inicializa la página y carga los datos necesarios
        /// </summary>
        protected override async Task OnPermisoValidadoAsync()
        {
            try
            {
                Console.WriteLine("[Indicadores] 🔄 OnPermisoValidadoAsync iniciando...");

                TituloPagina = T(AppResources.Menu.ObtenerEtiqueta((int)CodigosMenu.Indicadores));

                Console.WriteLine($"[Indicadores] Título de página: {TituloPagina}");

                LayerOverlayService.Start($"{T(AppResources.Common.Loading)} {TituloPagina}");

                await InicializarPaginaAsync();

                Console.WriteLine("[Indicadores] ✅ OnPermisoValidadoAsync completado");

                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Indicadores] ❌ Error en OnPermisoValidadoAsync: {ex.Message}");
                Console.WriteLine($"[Indicadores] StackTrace: {ex.StackTrace}");
                await LogService.InsertException(nameof(Indicadores), ex);

                await ErrorService.MostrarErrorInicializandoPagina(TituloPagina, ex);
            }
            finally
            {
                LayerOverlayService.Stop();
            }
        }

        /// <summary>
        /// Se ejecuta cuando el usuario cierra sesión o se desconecta
        /// </summary>
        protected override async Task OnUsuarioLoginDesconectado()
        {
            await base.OnUsuarioLoginDesconectado();
            await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// Inicializa la página cargando los datos maestros necesarios
        /// </summary>
        private async Task InicializarPaginaAsync()
        {
            try
            {
                Idiomas = ResourceService.ObtenerIdiomas();

                if (Idiomas == null || !Idiomas.Any())
                {
                    Console.WriteLine("[Indicadores] ⚠️ No se cargaron idiomas, usando lista vacía");
                    Idiomas = [];
                }

                DatosIndicadores = await IndicadoresService.ObtenerIndicadoresConIdiomas(null);
                
                if (DatosIndicadores.Count == 0)
                {
                    await MensajesHelper.MostrarMensajeInfo(TituloPagina, T(AppResources.Mensajes.RegistrosNoEncontrados));
                }

                Console.WriteLine($"[Indicadores] ✅ Página inicializada correctamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Indicadores] ❌ Error en InicializarPaginaAsync: {ex.Message}");
                await LogService.InsertException(nameof(Indicadores), ex);
                throw;
            }
        }

        #endregion



        #region Grid Indicadores - Eventos

        /// <summary>
        /// Maneja el evento de doble clic en una fila del grid
        /// Abre el popup de edición para el indicador seleccionado
        /// </summary>
        /// <param name="e">Argumentos del evento de clic</param>
        private async Task GridIndicadores_DoubleClick(GridRowClickEventArgs e)
        {
            var indicador = (Indicador?)GridIndicadores.GetDataItem(e.VisibleIndex);

            if (indicador == null) return;
            await EditarIndicadorAsync(indicador);
        }

        #endregion

        #region Grid Indicadores - CRUD

        /// <summary>
        /// Crea un nuevo indicador con valores predeterminados
        /// Calcula automáticamente BitAnd (siguiente potencia de 2) y Orden (último + 10)
        /// </summary>
        private async Task NuevoIndicadorAsync()
        {
            try
            {
                LayerOverlayService.Start();
                IndicadorEnEdicion = new();

                int ultimoBitand = await IndicadoresService.ObtenerUltimoBitAnd();
                IndicadorEnEdicion.BitAnd = ultimoBitand * 2;

                int ultimoOrden = await IndicadoresService.ObtenerUltimoOrden();
                IndicadorEnEdicion.Orden = ultimoOrden + 10;

                IndicadorEnEdicion.Estado = EstadoEntidad.Nuevo;
                EsPopupEdicionVisible = true;
            }
            catch (Exception ex)
            {
                await LogService.InsertException(this.GetType().Name, ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina);
            }
            finally
            {
                LayerOverlayService.Stop();
            }
        }

        /// <summary>
        /// Abre el popup para editar un indicador existente
        /// Clona el indicador para no modificar el original hasta guardar
        /// </summary>
        /// <param name="item">Indicador a editar</param>
        private async Task EditarIndicadorAsync(Indicador item)
        {
            try
            {
                IndicadorEnEdicion = DatosHelper.ClonarObjeto(item);
                IndicadorEnEdicion.Estado = EstadoEntidad.Modificado;
                EsPopupEdicionVisible = true;
            }
            catch (Exception ex)
            {
                await LogService.InsertException(this.GetType().Name, ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina);
            }
        }

        /// <summary>
        /// Elimina un indicador previa confirmación del usuario
        /// </summary>
        /// <param name="indicador">Indicador a eliminar</param>
        private async Task EliminarIndicadorAsync(Indicador indicador)
        {
            try
            {
                bool confirmado = await MensajesHelper.MostrarMensajeParaConfirmacion(
                    TituloPagina,
                    T(AppResources.Mensajes.ConfirmacionEliminar));

                if (!confirmado) return;

                LayerOverlayService.Start();

                await IndicadoresService.Eliminar(indicador);

                DatosIndicadores.Remove(indicador);
                GridIndicadores.Reload();

                await MensajesHelper.MostrarMensajeInfo(TituloPagina, T(AppResources.Mensajes.RegistroEliminado));
            }
            catch (Exception ex)
            {
                await LogService.InsertException(this.GetType().Name, ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina, T(AppResources.Mensajes.ErrorDelete));
            }
            finally
            {
                LayerOverlayService.Stop();
            }
        }

        #endregion

        #region Grid Idiomas - Eventos

       
        /// <summary>
        /// Guarda los cambios del idioma editado en la lista del indicador
        /// Distingue entre nuevo y modificación
        /// </summary>
        /// <param name="e">Argumentos del evento</param>
        private void GridIdiomasIndicador_EditModelSaving(GridEditModelSavingEventArgs e)
        {
            var itemIdiomaEdit = (IdiomaIndicador)e.EditModel;

            if (e.IsNew)
            {
                IndicadorEnEdicion.Idiomas.Add(itemIdiomaEdit);
            }
            else
            {
                var indexIdioma = IndicadorEnEdicion.Idiomas.FindIndex(x => x.Codigo == itemIdiomaEdit.Codigo);
                if (indexIdioma != -1)
                {
                    IndicadorEnEdicion.Idiomas[indexIdioma] = itemIdiomaEdit;
                }
            }
        }

        /// <summary>
        /// Resalta visualmente las celdas que han sido modificadas
        /// Compara valores actuales con valores originales campo por campo
        /// </summary>
        /// <param name="ea">Argumentos del evento de personalización</param>
        private async void GridIdiomasIndicador_CustomizeElement(GridCustomizeElementEventArgs ea)
        {
            if (ea.ElementType != GridElementType.DataCell) return;

            try
            {
                var column = (IGridDataColumn)ea.Column;

                var itemIdiomaEdit = GridIdiomasIndicador.GetDataItem(ea.VisibleIndex) as IdiomaIndicador;
                if (itemIdiomaEdit == null) return;

                var itemIndicador = DatosIndicadores.FirstOrDefault(x => x.Codigo == IndicadorEnEdicion.Codigo);
                if (itemIndicador == null) return;

                var originIdioma = itemIndicador.Idiomas.FirstOrDefault(x => x.Codigo == itemIdiomaEdit.Codigo);
                if (originIdioma == null) return;

                var comparadores = new Dictionary<string, Func<(object? original, object? actual)>>()
                {
                    {
                        T(AppResources.Common.Idioma),
                        () => (originIdioma.CodigoIdioma, itemIdiomaEdit.CodigoIdioma)
                    },
                    {
                        T(AppResources.Common.Descripcion),
                        () => (originIdioma.Descripcion, itemIdiomaEdit.Descripcion)
                    },
                    {
                        T(AppResources.Common.DescripcionAbreviada),
                        () => (originIdioma.DescripcionAbreviada, itemIdiomaEdit.DescripcionAbreviada)
                    },
                    {
                        T(AppResources.Common.Leyenda),
                        () => (originIdioma.Leyenda, itemIdiomaEdit.Leyenda)
                    }
                };

                if (comparadores.TryGetValue(column.Caption, out var selector))
                {
                    var (original, actual) = selector();
                    if (!Equals(original, actual))
                    {
                        ea.CssClass = "grid-modified-cell";
                    }
                }
            }
            catch (Exception ex)
            {
                await LogService.InsertException(this.GetType().Name, ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina);
            }
        }

        /// <summary>
        /// Personaliza el editor de celdas del grid de idiomas
        /// Habilita el ícono de validación
        /// </summary>
        /// <param name="e">Argumentos del evento</param>
        private void GridIdiomasIndicador_CustomizeDataRowEditor(GridCustomizeDataRowEditorEventArgs e)
        {
            if (e.EditSettings is not ITextEditSettings settings) return;
            settings.ShowValidationIcon = true;
        }

        /// <summary>
        /// Inicia el proceso de agregar un nuevo idioma al indicador
        /// </summary>
        private async Task AgregarIdiomaAsync()
        {
            await GridIdiomasIndicador.StartEditNewRowAsync();
        }

        /// <summary>
        /// Elimina un idioma de la lista del indicador en edición
        /// No persiste hasta que se guarde el indicador
        /// </summary>
        /// <param name="dataItem">Item de idioma a eliminar</param>
        private void EliminarIdioma(object dataItem)
        {
            var idioma = (IdiomaIndicador)dataItem;
            if (idioma != null)
            {
                IndicadorEnEdicion.Idiomas.Remove(idioma);
            }
            GridIdiomasIndicador.CancelEditAsync();
        }

        #endregion


        #region Popup Edit - Validación y Guardado

        /// <summary>
        /// Valida y guarda el indicador editado
        /// Realiza validaciones completas antes de persistir
        /// </summary>
        private async Task GuardarEdicionIndicadorAsync()
        {
            try
            {
                if (TieneCambios())
                {
                    LayerOverlayService.Start();

                    var descripcionCampoInvalido = ValidarLongitudDatos();
                    if (!string.IsNullOrEmpty(descripcionCampoInvalido))
                    {
                        await MensajesHelper.MostrarMensajeInfo(TituloPagina,
                            T(AppResources.Mensajes.LongitudCaracteres) + " " + descripcionCampoInvalido);
                        return;
                    }

                    if (!ValidarCamposRequeridos())
                    {
                        await MensajesHelper.MostrarMensajeInfo(TituloPagina, T(AppResources.Mensajes.CamposObligatorios));
                        return;
                    }

                    if (IndicadorEnEdicion.Orden <= 0)
                    {
                        await MensajesHelper.MostrarMensajeInfo(TituloPagina,
                            string.Format(T(AppResources.Mensajes.CampoMayorQueCero), T(AppResources.Common.Orden)));
                        return;
                    }

                    if (IndicadorEnEdicion.BitAnd <= 0)
                    {
                        await MensajesHelper.MostrarMensajeInfo(TituloPagina,
                            string.Format(T(AppResources.Mensajes.CampoMayorQueCero), T(AppResources.Common.BitAnd)));
                        return;
                    }

                    if (!EsBitAndValido())
                    {
                        await MensajesHelper.MostrarMensajeInfo(TituloPagina, T(AppResources.Mensajes.ValidarBitAnd));
                        return;
                    }

                    if (TieneIdiomasDuplicados())
                    {
                        await MensajesHelper.MostrarMensajeInfo(TituloPagina, T(AppResources.Mensajes.IdiomasDuplicados));
                        return;
                    }

                    if (TieneDatosIdiomaIncompletos())
                    {
                        await MensajesHelper.MostrarMensajeAviso(TituloPagina, T(AppResources.Pages.Indicadores.LanguageDataIncompleted));
                        return;
                    }

                    await GuardarDatosAsync();
                }
                else
                {
                    await MensajesHelper.MostrarMensajeAviso(TituloPagina, T(AppResources.Mensajes.SinModificaciones));
                }
            }
            catch (ValidacionException exv)
            {
                await MensajesHelper.MostrarMensajeAviso(TituloPagina, ObtenerMensajeValidacion(exv.CampoValidado, exv.Valor));
            }
            catch (Exception ex)
            {
                await LogService.InsertException(this.GetType().Name, ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina);
            }
            finally
            {
                LayerOverlayService.Stop();
            }
        }

        /// <summary>
        /// Genera mensaje de validación para campos con valores duplicados
        /// </summary>
        /// <param name="campo">Campo que tiene el error de validación</param>
        /// <param name="valor">Valor duplicado encontrado</param>
        /// <returns>Mensaje formateado con el nombre del campo y valor</returns>
        private string ObtenerMensajeValidacion(CampoErrorValidacion campo, string valor)
        {
            string nombreCampo = T($"Common:{campo}:label");
            return string.Format(T(AppResources.Mensajes.ValorCampoRepetido), nombreCampo, valor);
        }

        /// <summary>
        /// Cancela la edición del indicador
        /// Solicita confirmación si hay cambios sin guardar
        /// ✅ Método con sufijo Async
        /// </summary>
        private async Task CancelarEdicionIndicadorAsync()
        {
            try
            {
                if (TieneCambios())
                {
                    bool confirmado = await MensajesHelper.MostrarMensajeParaConfirmacion(
                        TituloPagina,
                        T(AppResources.Mensajes.AvisoAntesCancelar));

                    if (confirmado)
                    {
                        CerrarPopupEdicion();
                    }
                }
                else
                {
                    CerrarPopupEdicion();
                }
            }
            catch (Exception ex)
            {
                await LogService.InsertException(this.GetType().Name, ex);
                await MensajesHelper.MostrarMensajeError(TituloPagina);
            }
        }

        /// <summary>
        /// Cierra el popup de edición y reinicia el estado
        /// </summary>
        private void CerrarPopupEdicion()
        {
            EsPopupEdicionVisible = false;
            IndicadorEnEdicion = new();
        }

        #endregion


        #region Métodos de Validación

        /// <summary>
        /// Verifica si hay cambios en el indicador actual comparando con el original
        /// </summary>
        /// <returns>True si hay cambios pendientes de guardar</returns>
        private bool TieneCambios()
        {
            if (IndicadorEnEdicion.Estado == EstadoEntidad.Nuevo)
            {
                return true;
            }

            var itemIndicador = DatosIndicadores.FirstOrDefault(x => x.Codigo == IndicadorEnEdicion.Codigo);
            if (itemIndicador != null)
            {
                var editItem = DatosHelper.ClonarObjeto(IndicadorEnEdicion);
                var originItem = DatosHelper.ClonarObjeto(itemIndicador);
                editItem.Estado = EstadoEntidad.SinCambios;
                originItem.Estado = EstadoEntidad.SinCambios;

                if (!editItem.Equals(originItem))
                {
                    IndicadorEnEdicion.Estado = EstadoEntidad.Modificado;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Valida que los campos no excedan la longitud máxima permitida
        /// Límites: Descripción (50), DescripcionAbreviada (10), Leyenda (100)
        /// </summary>
        /// <returns>Nombre del campo que excede la longitud, o string vacío si todo OK</returns>
        private string ValidarLongitudDatos()
        {
            if (IndicadorEnEdicion.Descripcion.Length > 50)
            {
                return T(AppResources.Common.Descripcion);
            }

            if (IndicadorEnEdicion.Idiomas.Any(o => o.Descripcion.Length > 50))
            {
                return T(AppResources.Common.Descripcion);
            }

            if (IndicadorEnEdicion.Idiomas.Any(o => o.DescripcionAbreviada.Length > 10))
            {
                return T(AppResources.Common.DescripcionAbreviada);
            }

            if (IndicadorEnEdicion.Idiomas.Any(o => o.Leyenda.Length > 100))
            {
                return T(AppResources.Common.Leyenda);
            }

            return string.Empty;
        }

        /// <summary>
        /// Valida que los campos obligatorios estén completos
        /// Campos requeridos: Descripción, BitAnd, Orden
        /// </summary>
        /// <returns>True si todos los campos obligatorios están completos</returns>
        private bool ValidarCamposRequeridos()
        {
            return !(string.IsNullOrEmpty(IndicadorEnEdicion.Descripcion) ||
                     string.IsNullOrEmpty(IndicadorEnEdicion.BitAnd.ToString()) ||
                     string.IsNullOrEmpty(IndicadorEnEdicion.Orden.ToString()));
        }

        /// <summary>
        /// Valida que el BitAnd sea una potencia de 2 (solo un bit a 1 en binario)
        /// Algoritmo: n & (n-1) == 0 solo si n es potencia de 2
        /// </summary>
        /// <returns>True si el BitAnd es válido</returns>
        private bool EsBitAndValido()
        {
            return IndicadorEnEdicion.BitAnd <= 0 ||
                   (IndicadorEnEdicion.BitAnd > 0 && (IndicadorEnEdicion.BitAnd & (IndicadorEnEdicion.BitAnd - 1)) == 0);
        }

        /// <summary>
        /// Verifica si hay idiomas duplicados en la lista del indicador
        /// </summary>
        /// <returns>True si existen idiomas duplicados</returns>
        private bool TieneIdiomasDuplicados()
        {
            return IndicadorEnEdicion.Idiomas.GroupBy(x => x.CodigoIdioma).Any(g => g.Count() > 1);
        }

        /// <summary>
        /// Valida que al menos un idioma tenga todos los campos completos
        /// </summary>
        /// <returns>True si ningún idioma tiene datos completos</returns>
        private bool TieneDatosIdiomaIncompletos()
        {
            return IndicadorEnEdicion.Idiomas.FirstOrDefault(x =>
                !string.IsNullOrEmpty(x.Descripcion.Trim()) &&
                !string.IsNullOrEmpty(x.DescripcionAbreviada.Trim()) &&
                !string.IsNullOrEmpty(x.Leyenda.Trim())) == null;
        }

        #endregion


        #region Persistencia de Datos

        /// <summary>
        /// Persiste los cambios del indicador en base de datos
        /// Detecta automáticamente qué idiomas son nuevos, modificados o eliminados
        /// </summary>
        private async Task GuardarDatosAsync()
        {
            var nuevosIdiomas = new List<IdiomaIndicador>();
            var idiomasAEliminar = new List<IdiomaIndicador>();
            var idiomasAActualizar = new List<IdiomaIndicador>();

            var indicadores = await IndicadoresService.ObtenerIndicadoresConIdiomas(null);

            if (IndicadorEnEdicion.Estado == EstadoEntidad.Nuevo)
            {
                nuevosIdiomas = IndicadorEnEdicion.Idiomas;
            }
            else if (IndicadorEnEdicion.Estado == EstadoEntidad.Modificado)
            {
                var indicadorOriginal = indicadores.FirstOrDefault(x => x.Codigo == IndicadorEnEdicion.Codigo);
                if (indicadorOriginal != null)
                {
                    nuevosIdiomas = [.. IndicadorEnEdicion.Idiomas.Where(newItem => newItem.Codigo == null)];

                    idiomasAEliminar = [.. indicadorOriginal.Idiomas.Where(removeItem => !IndicadorEnEdicion.Idiomas.Any(orig => orig.Codigo == removeItem.Codigo))];

                    idiomasAActualizar = [.. IndicadorEnEdicion.Idiomas
                        .Where(updateItem => indicadorOriginal.Idiomas.Any(orig =>
                            orig.Codigo == updateItem.Codigo &&
                            (orig.CodigoIdioma != updateItem.CodigoIdioma ||
                             orig.Descripcion != updateItem.Descripcion ||
                             orig.DescripcionAbreviada != updateItem.DescripcionAbreviada ||
                             orig.Leyenda != updateItem.Leyenda)))];
                }
            }

            await IndicadoresService.Grabar(
                IndicadorEnEdicion, 
                nuevosIdiomas, 
                idiomasAActualizar, 
                idiomasAEliminar);

            CerrarPopupEdicion();

            if (IndicadorEnEdicion.Estado == EstadoEntidad.Nuevo)
            {
                DatosIndicadores = await IndicadoresService.ObtenerIndicadoresConIdiomas(null);
                GridIndicadores.SetFocusedRowIndex(IndicadorEnEdicion.Indice);
            }
            else
            {
                int indice = DatosIndicadores.FindIndex(c => c.Codigo == IndicadorEnEdicion.Codigo);
                if (indice != -1)
                {
                    DatosIndicadores[indice] = IndicadorEnEdicion;
                }
            }

            GridIndicadores.Reload();
            await MensajesHelper.MostrarMensajeExito(TituloPagina, T(AppResources.Mensajes.RegistroGrabado));
        }

        #endregion
    }
}

