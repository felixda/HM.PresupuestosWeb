
namespace HM.Presupuestos.Web.Pages.Mantenimientos
{
    /// <summary>
    /// Página de mantenimiento de Indicadores
    /// Gestiona el CRUD de indicadores con sus traducciones multiidioma
    /// </summary>
    public partial class Indicadores : ContextProtegido
    {
        #region Inyección de Dependencias

        [Inject] protected IIndicadoresService IndicadoresService { get; set; } = default!;

        #endregion

        #region Propiedades para Data Binding 

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
            Console.WriteLine("[Indicadores] ? Permiso denegado");
            return Task.CompletedTask;
        }

        protected override async Task InicializarPaginaAsync()
        {
            Idiomas = LocalizadorRecursos.ObtenerIdiomas();
            DatosIndicadores = await IndicadoresService.ObtenerIndicadoresConIdiomas(null);

            if (DatosIndicadores.Count == 0)
            {
                await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(AppResources.Mensajes.RegistrosNoEncontrados));
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
            await EjecutarAsync(async () =>{
                IndicadorEnEdicion = new();

                int ultimoBitand = await IndicadoresService.ObtenerUltimoBitAnd();
                IndicadorEnEdicion.BitAnd = ultimoBitand * 2;

                int ultimoOrden = await IndicadoresService.ObtenerUltimoOrden();
                IndicadorEnEdicion.Orden = ultimoOrden + 10;

                IndicadorEnEdicion.Estado = EstadoEntidad.Nuevo;
                EsPopupEdicionVisible = true;
            });

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
                await ManejarExcepcion(ex);
            }
        }

        /// <summary>
        /// Elimina un indicador previa confirmación del usuario
        /// </summary>
        /// <param name="indicador">Indicador a eliminar</param>
        private async Task EliminarIndicadorAsync(Indicador indicador)
        {
            await EjecutarAsync(async () =>
            {
                bool confirmado = await MensajesHelper.MostrarMensajeParaConfirmacion(
                    TituloPagina,
                    ObtenerTexto(AppResources.Mensajes.ConfirmacionEliminar));

                if (!confirmado) return;

                await IndicadoresService.Eliminar(indicador);

                DatosIndicadores.Remove(indicador);
                GridIndicadores.Reload();

                await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(AppResources.Mensajes.RegistroEliminado));

            }, ObtenerTexto(AppResources.Mensajes.ErrorDelete));
        
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
                        ObtenerTexto(AppResources.Common.Idioma),
                        () => (originIdioma.CodigoIdioma, itemIdiomaEdit.CodigoIdioma)
                    },
                    {
                        ObtenerTexto(AppResources.Common.Descripcion),
                        () => (originIdioma.Descripcion, itemIdiomaEdit.Descripcion)
                    },
                    {
                        ObtenerTexto(AppResources.Common.DescripcionAbreviada),
                        () => (originIdioma.DescripcionAbreviada, itemIdiomaEdit.DescripcionAbreviada)
                    },
                    {
                        ObtenerTexto(AppResources.Common.Leyenda),
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
                await ManejarExcepcion(ex);
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
        private async Task OnGuardarIndicadorAsync()
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
                            ObtenerTexto(AppResources.Mensajes.LongitudCaracteres) + " " + descripcionCampoInvalido);
                        return;
                    }

                    if (!ValidarCamposRequeridos())
                    {
                        await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(AppResources.Mensajes.CamposObligatorios));
                        return;
                    }

                    if (IndicadorEnEdicion.Orden <= 0)
                    {
                        await MensajesHelper.MostrarMensajeInfo(TituloPagina,
                            string.Format(ObtenerTexto(AppResources.Mensajes.CampoMayorQueCero), ObtenerTexto(AppResources.Common.Orden)));
                        return;
                    }

                    if (IndicadorEnEdicion.BitAnd <= 0)
                    {
                        await MensajesHelper.MostrarMensajeInfo(TituloPagina,
                            string.Format(ObtenerTexto(AppResources.Mensajes.CampoMayorQueCero), ObtenerTexto(AppResources.Common.BitAnd)));
                        return;
                    }

                    if (!EsBitAndValido())
                    {
                        await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(AppResources.Mensajes.ValidarBitAnd));
                        return;
                    }

                    if (TieneIdiomasDuplicados())
                    {
                        await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(AppResources.Mensajes.IdiomasDuplicados));
                        return;
                    }

                    if (TieneDatosIdiomaIncompletos())
                    {
                        await MensajesHelper.MostrarMensajeAviso(TituloPagina, ObtenerTexto(AppResources.Pages.Indicadores.LanguageDataIncompleted));
                        return;
                    }

                    await GuardarDatosAsync();
                }
                else
                {
                    await MensajesHelper.MostrarMensajeAviso(TituloPagina, ObtenerTexto(AppResources.Mensajes.SinModificaciones));
                }
            }
            catch (ValidacionException exv)
            {
                await MensajesHelper.MostrarMensajeAviso(TituloPagina, ObtenerMensajeValidacion(exv.CampoValidado, exv.Valor));
            }
            catch (Exception ex)
            {
                await ManejarExcepcion(ex);
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
            string nombreCampo = ObtenerTexto($"Common:{campo}:label");
            return string.Format(ObtenerTexto(AppResources.Mensajes.ValorCampoRepetido), nombreCampo, valor);
        }

        /// <summary>
        /// Cancela la edición del indicador
        /// Solicita confirmación si hay cambios sin guardar
        /// ? Método con sufijo Async
        /// </summary>
        private async Task CancelarEdicionIndicadorAsync()
        {
            await EjecutarAsync(async () =>
            {
                if (TieneCambios())
                {
                    bool confirmado = await MensajesHelper.MostrarMensajeParaConfirmacion(
                        TituloPagina,
                        ObtenerTexto(AppResources.Mensajes.AvisoAntesCancelar));

                    if (confirmado)
                    {
                        CerrarPopupEdicion();
                    }
                }
                else
                {
                    CerrarPopupEdicion();
                }

            });
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
                return ObtenerTexto(AppResources.Common.Descripcion);
            }

            if (IndicadorEnEdicion.Idiomas.Any(o => o.Descripcion.Length > 50))
            {
                return ObtenerTexto(AppResources.Common.Descripcion);
            }

            if (IndicadorEnEdicion.Idiomas.Any(o => o.DescripcionAbreviada.Length > 10))
            {
                return ObtenerTexto(AppResources.Common.DescripcionAbreviada);
            }

            if (IndicadorEnEdicion.Idiomas.Any(o => o.Leyenda.Length > 100))
            {
                return ObtenerTexto(AppResources.Common.Leyenda);
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
        /// Persiste los cambios del indicador en base de datos.
        /// Delega en el método correspondiente según el estado del indicador.
        /// </summary>
        private async Task GuardarDatosAsync()
        {
            if (IndicadorEnEdicion.Estado == EstadoEntidad.Nuevo)
            {
                await GuardarIndicadorNuevoAsync();
            }
            else if (IndicadorEnEdicion.Estado == EstadoEntidad.Modificado)
            {
                await GuardarIndicadorModificadoAsync();
            }

            GridIndicadores.Reload();
            await MensajesHelper.MostrarMensajeExito(TituloPagina, ObtenerTexto(AppResources.Mensajes.RegistroGrabado));
        }

        /// <summary>
        /// Persiste un indicador nuevo junto con todos sus idiomas.
        /// Tras guardar, recarga la lista completa y posiciona el foco en el nuevo registro.
        /// </summary>
        private async Task GuardarIndicadorNuevoAsync()
        {
            var nuevosIdiomas = IndicadorEnEdicion.Idiomas;

            await IndicadoresService.Grabar(
                IndicadorEnEdicion,
                nuevosIdiomas,
                idiomasActualizar: [],
                idiomasEliminar: []);

            DatosIndicadores = await IndicadoresService.ObtenerIndicadoresConIdiomas(null);
            GridIndicadores.SetFocusedRowIndex(IndicadorEnEdicion.Indice);

            CerrarPopupEdicion();
        }

        /// <summary>
        /// Persiste los cambios de un indicador existente.
        /// Detecta qué idiomas son nuevos, han sido modificados o deben eliminarse
        /// comparando con el estado actual en base de datos.
        /// </summary>
        private async Task GuardarIndicadorModificadoAsync()
        {
            var indicadores = await IndicadoresService.ObtenerIndicadoresConIdiomas(null);
            var indicadorOriginal = indicadores.FirstOrDefault(x => x.Codigo == IndicadorEnEdicion.Codigo);

            var (nuevosIdiomas, idiomasActualizar, idiomasEliminar) = ObtenerCambiosIdiomas(indicadorOriginal);

            await IndicadoresService.Grabar(
                IndicadorEnEdicion,
                nuevosIdiomas,
                idiomasActualizar,
                idiomasEliminar);

            ActualizarIndicadorEnListaLocal();

            CerrarPopupEdicion();
        }

        /// <summary>
        /// Calcula las listas de idiomas nuevos, modificados y a eliminar
        /// comparando el indicador en edición con su versión original en base de datos.
        /// </summary>
        /// <param name="indicadorOriginal">Versión original del indicador obtenida de base de datos, o null si no existe.</param>
        /// <returns>Tupla con las tres listas de cambios de idiomas.</returns>
        private (List<IdiomaIndicador> nuevos, 
            List<IdiomaIndicador> actualizados, 
            List<IdiomaIndicador> aEliminar)  ObtenerCambiosIdiomas(Indicador? indicadorOriginal)
        {
            if (indicadorOriginal == null)
                return ([], [], []);

            var nuevos = IndicadorEnEdicion.Idiomas
                .Where(i => i.Codigo == null)
                .ToList();

            var aEliminar = indicadorOriginal.Idiomas
                .Where(orig => !IndicadorEnEdicion.Idiomas.Any(i => i.Codigo == orig.Codigo))
                .ToList();

            var actualizados = IndicadorEnEdicion.Idiomas
                .Where(i => indicadorOriginal.Idiomas.Any(orig =>
                    orig.Codigo == i.Codigo &&
                    (orig.CodigoIdioma != i.CodigoIdioma ||
                     orig.Descripcion != i.Descripcion ||
                     orig.DescripcionAbreviada != i.DescripcionAbreviada ||
                     orig.Leyenda != i.Leyenda)))
                .ToList();

            return (nuevos, actualizados, aEliminar);
        }

        /// <summary>
        /// Reemplaza el indicador modificado en la lista local para reflejar los cambios
        /// sin necesidad de recargar todos los datos desde base de datos.
        /// </summary>
        private void ActualizarIndicadorEnListaLocal()
        {
            int indice = DatosIndicadores.FindIndex(c => c.Codigo == IndicadorEnEdicion.Codigo);
            if (indice != -1)
            {
                DatosIndicadores[indice] = IndicadorEnEdicion;
            }
        }

        #endregion
    }
}


