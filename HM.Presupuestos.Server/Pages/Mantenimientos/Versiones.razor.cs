using DevExpress.Blazor.Internal;
using HM.Core.Comun.v6.Seguridad.Interfaces;
using System.Text.Json;
using Version = HM.Presupuestos.Domain.Entidades.Version;

namespace HM.Presupuestos.Server.Pages.Mantenimientos
{
    public partial class Versiones
    {
        #region Inyecci?n de Dependencias

        [Inject] protected IJwt Jwt { get; set; } = default!;
        [Inject] protected IIndicadoresService IndicadoresService { get; set; } = default!;
        [Inject] protected TraduccionesHelper TraduccionesHelper { get; set; } = default!;
        [Inject] protected ILogAccionesService LogAccionesService { get; set; } = default!;
        [Inject] protected NavigationManager Navigation { get; set; } = default!;

        #endregion

        #region Private Properties

        #region Page

        private string _pageTitle { get; set; } = string.Empty;
        private string _textoToolTipAyuda { get; set; } = string.Empty;

        private bool _hayCambios => JsonSerializer.Serialize(_listVersion) != JsonSerializer.Serialize(_listOriginVersion);

        #endregion

        #region Filter

        private CodigoDescripcion? _itemYearSelected { get; set; }

        #endregion

        #region Grid Versiones

        private IGrid? _GridVersiones { get; set; }
        private List<CodigoDescripcion> _listTipoVersion = [];
        private List<CodigoDescripcion> _listMonth = [];
        private List<CodigoDescripcion> _listYear = [];
        private List<Version> _listOriginVersion = [];
        private List<Version> _listVersion = [];
        private List<Version> _listVersionCache = [];
        private List<Indicador> _listMasterIndicador = [];

        #endregion


        #region Grid Leyendas

        private IGrid? _GridLeyendas { get; set; }

		#endregion

		#endregion
		protected override CodigosMenu CodigoMenuPermiso => CodigosMenu.Versiones;

		#region Page

		protected override Task OnPermisoDenegadoAsync()
        {
            Console.WriteLine("? Permiso denegado");
            return Task.CompletedTask;
        }

		/// <summary>
		/// ? Solo la l?gica espec?fica de inicializaci?n
		/// </summary>
		protected override async Task InicializarPaginaAsync()
		{
			_listYear = await VersionesService.ObtenerAniosConVersiones(true);

			if (_listYear.Any())
			{
				_itemYearSelected = _listYear[1];
			}
			string currentLanguageCode = IdiomaService.Idioma;
			var obLanguageList = ResourceService.ObtenerIdiomas();
			_listTipoVersion = ObtenerTiposVersion();
			_listMonth = await TraduccionesHelper.ObtenerMeses();
			_listMasterIndicador = await IndicadoresService.ObtenerIndicadoresConIdiomas();
			Idioma language = obLanguageList.First(c => c.Iso == currentLanguageCode);
			_listMasterIndicador.ForEach(c => c.CodigoIdioma = language.CodigoIdioma);

		}

		//protected override async Task OnPermisoValidadoAsync()
  //      {
  //          try
  //          {
  //              Console.WriteLine("[Versiones] ?? OnPermisoValidadoAsync iniciando...");

  //              _pageTitle = T($"Menu:Menu_{(int)CodigosMenu.Versiones}:label");
  //              _textoToolTipAyuda = T(AppResources.Pages.GestionVersiones.ToolTip);
  //              Console.WriteLine($"[Versiones] T?tulo de p?gina: {_pageTitle}");

  //              LayerOverlayService.Start($"{T(AppResources.Common.Loading)} {_pageTitle}");

  //              await PageInitialize();

  //              Console.WriteLine("[Versiones] ? OnPermisoValidadoAsync completado");

  //              await InvokeAsync(StateHasChanged);
  //          }
  //          catch (Exception ex)
  //          {
  //              Console.WriteLine($"[Versiones] ? Error en OnPermisoValidadoAsync: {ex.Message}");
  //              Console.WriteLine($"[Versiones] StackTrace: {ex.StackTrace}");
  //              await LogService.InsertException(ex);

  //              await ErrorService.MostrarErrorInicializandoPagina(_pageTitle, ex);
  //          }
  //          finally
  //          {
  //              LayerOverlayService.Stop();
  //          }
  //      }

        protected override async Task OnUsuarioLoginDesconectado()
        {
            await base.OnUsuarioLoginDesconectado();

            await InvokeAsync(StateHasChanged);
        }


        
        //private async Task PageInitialize()
        //{
        //    _listYear = await VersionesService.ObtenerAniosConVersiones(true);

        //    if (_listYear.Any())
        //    {
        //        _itemYearSelected = _listYear[1];
        //    }
        //    string currentLanguageCode = IdiomaService.Idioma;
        //    var obLanguageList = ResourceService.ObtenerIdiomas();
        //    _listTipoVersion = ObtenerTiposVersion();
        //    _listMonth = await TraduccionesHelper.ObtenerMeses();
        //    _listMasterIndicador = await IndicadoresService.ObtenerIndicadoresConIdiomas();
        //    Idioma language = obLanguageList.First(c => c.Iso == currentLanguageCode);
        //    _listMasterIndicador.ForEach(c => c.CodigoIdioma = language.CodigoIdioma);


        //}

        public List<CodigoDescripcion> ObtenerTiposVersion()
        {
            List<CodigoDescripcion> listTiposVersion = [];

            listTiposVersion.Add(new CodigoDescripcion
            {
                Codigo = Convert.ToInt32(T(AppResources.Pages.Versiones.TipoVersion.User_Code)),
                Descripcion = T(AppResources.Pages.Versiones.TipoVersion.User)
            });

            listTiposVersion.Add(new CodigoDescripcion
            {
                Codigo = Convert.ToInt32(T(AppResources.Pages.Versiones.TipoVersion.Monthly_Code)),
                Descripcion = T(AppResources.Pages.Versiones.TipoVersion.Monthly)
            });

            listTiposVersion.Add(new CodigoDescripcion
            {
                Codigo = Convert.ToInt32(T(AppResources.Pages.Versiones.TipoVersion.Backup_Code)),
                Descripcion = T(AppResources.Pages.Versiones.TipoVersion.Backup)
            });

            return listTiposVersion;
        }

        #endregion



        #region Filter


        ///<summary>
        /// Event param Source Year changed
        ///</summary>
        private async Task Param_Year_SelectedDataItemChanged(SelectedDataItemChangedEventArgs<CodigoDescripcion> e)
        {
            if (e.DataItem != null)
            {
                try
                {
                    LayerOverlayService.Start();
                    int anioSeleccionado = e.DataItem.Codigo;
                    _listVersion = await VersionesService.ObtenerVersiones(anioSeleccionado);

                    _listOriginVersion = new List<Version>();
                    if (_listVersion != null)
                    {
                        _listOriginVersion = DatosHelper.ClonarObjeto(_listVersion);
                    }
                }
                catch (Exception ex)
                {
                    await LogService.InsertException(ex);
                    await MensajesHelper.MostrarMensajeError(_pageTitle);
                }
                finally
                {
                    LayerOverlayService.Stop();
                }
            }
            else
            {
                _listVersion = [];
            }
        }

        #endregion

        #region Grid Versiones

        ///<summary>
        /// Checkbox checked changed event: Update Estado attribute from Indicador
        ///</summary>
        /// <param name="newValue">new bool value</param>
        /// <param name="versionCodigo">Codigo Version</param>
        /// <param name="indicadorCodigo">Codigo Indicador</param> 
        private async void CheckboxIndicadorCheckedChanged(bool? newValue, int? versionCodigo, int? indicadorCodigo)
        {
            if (newValue is null || indicadorCodigo is null) return;

            var itemVersion = _listVersion.Find(x => x.Codigo == versionCodigo);
            if (itemVersion == null) return;

            var itemIndicador = itemVersion.IndicadorList.Find(x => x.Codigo == indicadorCodigo);
            if (itemIndicador == null) return;

            try
            {
                //Activamos el indicador chequeado
                itemIndicador.Estado = (bool)newValue;

                //Si hemos chequeado el indicador de REAL el indicador de CERRADA se chequea automaticmaente tambien
                //y todos los demas se desactivan
                if ((itemIndicador.Codigo == Constantes.CodigosIndicadores.REAL) && (bool)newValue)
                {
                    foreach (var item in itemVersion.IndicadorList)
                    {
                        if (item.Codigo != Constantes.CodigosIndicadores.REAL && item.Codigo != Constantes.CodigosIndicadores.CERRADA)
                        {
                            item.Estado = false;
                        }
                        else if (item.Codigo == Constantes.CodigosIndicadores.CERRADA)
                        {
                            item.Estado = true;
                        }
                    }
                }
                //SI el que se ha chequeado es el indicador de CERRADA, se comprueba si el indicador de REAL esta chequeado y de ser asi,
                //no deja cambiarlo y muestra aviso
                else if ((itemIndicador.Codigo == Constantes.CodigosIndicadores.CERRADA) && !(bool)newValue)
                {
                    var itemIndicadorReal = itemVersion.IndicadorList.Find(x => x.Codigo == Constantes.CodigosIndicadores.REAL);
                    if (itemIndicadorReal != null)
                    {
                        if (itemIndicadorReal.Estado)
                        {
                            itemIndicador.Estado = true;
                            //await MensajesHelper.MostrarMensajeInfo(_pageTitle, "Si la versi?n tiene el indicador de Real el indicador de Cerrada tiene que estar acivo");

                            string descripcionIndicadorReal = "Real";
                            var indicadorReal = _listMasterIndicador.Find(c => c.Codigo == Constantes.CodigosIndicadores.REAL);
                            if (indicadorReal != null)
                            {
                                descripcionIndicadorReal = indicadorReal.DescripcionTraducida;
                            }

                            string descripcionIndicadorCerrada = "Cerrada";
                            var indicadorCerrada = _listMasterIndicador.Find(c => c.Codigo == Constantes.CodigosIndicadores.CERRADA);
                            if (indicadorCerrada != null)
                            {
                                descripcionIndicadorCerrada = indicadorCerrada.DescripcionTraducida;
                            }

                            await MensajesHelper.MostrarMensajeGeneral(_pageTitle, string.Format(T(AppResources.Mensajes.IndicadorCerradaConIndicadorReal), descripcionIndicadorReal, descripcionIndicadorCerrada), MessageBoxRenderStyle.Warning);
                        }
                    }
                }
                //Para los demas estados, comprobamos si el indicadior REAL Esta activo, y si es asi no hace nada
                else
                {
                    var itemIndicadorReal = itemVersion.IndicadorList.Find(x => x.Codigo == Constantes.CodigosIndicadores.REAL);
                    if (itemIndicadorReal != null)
                    {
                        if (itemIndicadorReal.Estado)
                        {
                            itemIndicador.Estado = false;
                        }
                    }
                }

                await MarcarCambios(_hayCambios);
            }
            catch (Exception ex)
            {
                await LogService.InsertException(ex);
                await MensajesHelper.MostrarMensajeError(_pageTitle);
            }
        }


        ///<summary>
        /// New Version item
        ///</summary>
        private async Task NewVersion()
        {
            if (!await UsuarioTienePermisos()) return;

            if (_itemYearSelected != null && _listVersion.Find(x => x.Codigo == 0) == null)
            {
                try
                {
                    var nuevaVersion = new Version();
                    nuevaVersion.CodigoTipo = 1;  //Por defecto el tipo Usuario
                    nuevaVersion.Mes = 12;
                    nuevaVersion.Anio = _itemYearSelected.Codigo;
                    nuevaVersion.IndicadorList = _listMasterIndicador.Select(m => new Version.VersionIndicador
                    {
                        Codigo = m.Codigo!.Value,
                        Estado = false
                    }).ToList();


                    int orden = 1;
                    if (_listVersion.Count > 0)
                    {
                        orden = _listVersion.Max(c => c.Orden) + 1;
                    }
                    nuevaVersion.Orden = orden;

                    _listVersion.Insert(0, nuevaVersion);
                    _GridVersiones!.Reload(); //Porque si no desplaza la ultima fila y no se ve
                    await MarcarCambios(true);

                }
                catch (Exception ex)
                {
                    await LogService.InsertException(ex);
                    await MensajesHelper.MostrarMensajeError(_pageTitle);
                }
            }
        }



        ///<summary>
        /// Cancel new Version item
        ///</summary>
        private void CancelVersion()
        {
            _listVersion = DatosHelper.ClonarObjeto(_listOriginVersion);
            LimpiarCambios();
        }


        ///<summary>
        /// Grid customize row event
        ///</summary>
        private void GridVersiones_CustomizeDataRowEditor(GridCustomizeDataRowEditorEventArgs e)
        {
            if (e.EditSettings is ITextEditSettings settings)
            {
                settings.ShowValidationIcon = true;
            }
        }


        ///<summary>
        /// Grid customize element event
        ///</summary>
        private async void GridVersiones_CustomizeElement(GridCustomizeElementEventArgs ea)
        {
            if (ea.ElementType == GridElementType.DataCell)
            {
                try
                {
                    var column = (IGridDataColumn)ea.Column;
                    var version = (Version)_GridVersiones!.GetDataItem(ea.VisibleIndex);
                    if (version != null)
                    {
                        var itemVersionOrigin = _listOriginVersion.Find(x => x.Codigo == version.Codigo);


                        if (itemVersionOrigin == null)
                        {
                            ea.CssClass = "grid-modified-cell";
                            return;
                        }


                        var caption = column.Caption;

                        if (caption.StartsWith("Indicador_"))
                        {
                            var codigoIndicador = caption.Substring("Indicador_".Length);
                            Console.WriteLine($"Columna para indicador: {codigoIndicador}");

                            var item1 = version.IndicadorList.First(x => x.Codigo == Int16.Parse(codigoIndicador));

                            var item2 = itemVersionOrigin.IndicadorList.First(x => x.Codigo == Int16.Parse(codigoIndicador));

                            if (item1.Estado != item2.Estado)
                            {
                                ea.CssClass = "grid-modified-cell";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    await LogService.InsertException(ex);
                    await MensajesHelper.MostrarMensajeError(_pageTitle);
                }
            }
        }



        ///<summary>
        /// Grid customize edit model saving event
        ///</summary>
        private async void GridVersiones_EditModelSaving(GridEditModelSavingEventArgs e)
        {
            var editVersion = (Version)e.EditModel;
            var indexVersion = _listVersion.FindIndex(x => x.Codigo == editVersion.Codigo);
            if (indexVersion != -1)
            {
                _listVersion[indexVersion] = editVersion;
                await MarcarCambios(true);
            }
        }


        ///<summary>
        /// Delete Version from DB
        ///</summary>
        /// <param name="dataItem">object from grid</param>
        private async Task DeleteVersion(object dataItem)
        {
            if (!await UsuarioTienePermisos()) return;
            var version = (Version)dataItem;

            if (version.Codigo > 0)
            {
                try
                {
                    if (!await MensajesHelper.MostrarMensajeParaConfirmacion(_pageTitle, T(AppResources.Mensajes.ConfirmacionEliminar)))
                    {
                        return;
                    }
                    LayerOverlayService.Start();
                    var result = await VersionesService.EliminarVersion(version);
                    if (result)
                    {
                        _listVersion = await VersionesService.ObtenerVersiones(_itemYearSelected!.Codigo);
                        _listOriginVersion = new List<Version>();
                        if (_listVersion != null && _listVersion.Count > 0)
                        {
                            _listOriginVersion = DatosHelper.ClonarObjeto(_listVersion);
                        }
                        await MensajesHelper.MostrarMensajeExito(_pageTitle, T(AppResources.Mensajes.RegistroEliminado));
                    }
                    else
                    {
                        await MensajesHelper.MostrarMensajeError(_pageTitle, T(AppResources.Mensajes.ErrorDelete));
                    }
                }
                catch (Exception ex)
                {
                    await LogService.InsertException(ex);
                    await MensajesHelper.MostrarMensajeError(_pageTitle, T(AppResources.Mensajes.ErrorDelete));
                }
                finally
                {
                    LayerOverlayService.Stop();
                }
            }
            else
            {
                _listVersion.Remove(version);
                _GridVersiones!.Reload();
            }
        }


        ///<summary>
        /// Validate version data list from grid
        ///</summary>
        private async Task<bool> ValidateData()
        {
            //Cuando es nueva la vesion, la descripcion no la valida automaticamente
            if (_listVersion.Any(o => o.Descripcion.Trim().Length == 0))
            {
                await MensajesHelper.MostrarMensajeInfo(_pageTitle, T(AppResources.Mensajes.CampoVersionObligatorio));
                return false;
            }

            //Validate text size
            if (_listVersion.Any(o => o.Descripcion.Length > 50))
            {
                await MensajesHelper.MostrarMensajeInfo(_pageTitle, T(AppResources.Mensajes.LongitudCaracteres) + "`" + T(AppResources.Common.Descripcion) + "`");
                return false;
            }

            //Validate unique indicator
            var listIndicadorUnique = _listMasterIndicador.Where(c => c.IndVersionUnica).ToList();
            Indicador? itemIndicadorUniqueDuplicated = null;
            foreach (var itemIndicadorUnique in listIndicadorUnique)
            {
                if (itemIndicadorUniqueDuplicated != null)
                    break;
                var itemIndicadorUniqueCounter = 0;
                foreach (var itemVersion in _listVersion)
                {
                    itemIndicadorUniqueCounter += itemVersion.IndicadorList.Where(x => x.Codigo == itemIndicadorUnique.Codigo && x.Estado == true).ToList().Count;
                    if (itemIndicadorUniqueCounter > 1)
                    {
                        itemIndicadorUniqueDuplicated = itemIndicadorUnique;
                        break;
                    }
                }
            }
            if (itemIndicadorUniqueDuplicated != null)
            {
                await MensajesHelper.MostrarMensajeGeneral(_pageTitle, string.Format(T(AppResources.Mensajes.IndicadorUnico), itemIndicadorUniqueDuplicated.DescripcionTraducida), MessageBoxRenderStyle.Warning);
                return false;
            }

            //Validate description duplicated
            Version? itemVersionDescriptionDuplicated = null;
            foreach (var itemVersion in _listVersion)
            {
                if (_listVersion.Find(x => x.Descripcion == itemVersion.Descripcion && x.Codigo != itemVersion.Codigo) != null)
                {
                    itemVersionDescriptionDuplicated = itemVersion;
                    break;
                }
            }
            if (itemVersionDescriptionDuplicated != null)
            {
                await MensajesHelper.MostrarMensajeGeneral(_pageTitle, string.Format(T(AppResources.Pages.Versiones.Message_VersionDescriptionError), itemVersionDescriptionDuplicated.Descripcion), MessageBoxRenderStyle.Warning);
                return false;
            }



            return true;
        }


        ///<summary>
        /// Calculate summary of Indicador list in Version
        ///</summary>
        /// <param name="itemVersion">Version object</param>
        /// <returns>Summary value</returns>
        public int CalculateSummaryIndicadorVersion(Version itemVersion)
        {
            int summaryIndicador = 0;

            foreach (var versionIndicador in itemVersion.IndicadorList)
            {
                if (versionIndicador.Estado)
                {
                    var itemMasterIndicador = _listMasterIndicador.Find(x => x.Codigo == versionIndicador.Codigo);
                    if (itemMasterIndicador != null)
                    {
                        summaryIndicador += itemMasterIndicador.BitAnd;
                    }
                }
            }

            return summaryIndicador;
        }


        ///<summary>
        /// Validate and save version data list from grid
        ///</summary>
        private async Task SaveVersionData()
        {
            try
            {
                if (!await UsuarioTienePermisos()) return;

                LayerOverlayService.Start();
                if (await ValidateData())
                {

                    var newVersionList = new List<Version>();
                    var updateVersionList = new List<Version>();
                    foreach (var itemVersion in _listVersion)
                    {
                        if (itemVersion.Codigo == 0)
                        {
                            itemVersion.IndEstado = CalculateSummaryIndicadorVersion(itemVersion);
                            newVersionList.Add(itemVersion);
                        }
                        else
                        {
                            var itemOriginVersion = _listOriginVersion.FirstOrDefault(x => x.Codigo == itemVersion.Codigo);
                            if (itemOriginVersion != null)
                            {
                                if (JsonSerializer.Serialize(itemVersion) != JsonSerializer.Serialize(itemOriginVersion))
                                {
                                    itemVersion.IndEstado = CalculateSummaryIndicadorVersion(itemVersion);
                                    updateVersionList.Add(itemVersion);
                                }

                            }
                        }
                    }

                    if (newVersionList.Count > 0 || updateVersionList.Count > 0)
                    {
                        var result = await VersionesService.GrabarVersiones(newVersionList, updateVersionList, UsuarioService.UsuarioApp!.Usuario!.CodigoPais);

                        if (result)
                        {
                            _listOriginVersion = new List<Version>();
                            if (_listVersion != null && _listVersion.Count > 0)
                                _listOriginVersion = DatosHelper.ClonarObjeto(_listVersion);

                            await MensajesHelper.MostrarMensajeInfo(_pageTitle, T(AppResources.Common.DatosGrabados));
                            await MarcarCambios(false);
                            LimpiarCambios();
                        }
                        else
                        {
                            await MensajesHelper.MostrarMensajeError(_pageTitle, T(AppResources.Mensajes.ErrorAlGrabar));
                        }
                    }
                    else
                    {
                        await MensajesHelper.MostrarMensajeInfo(_pageTitle, T(AppResources.Mensajes.SinModificaciones));
                    }
                }
            }
            catch (Exception ex)
            {
                await LogService.InsertException(ex);
                await MensajesHelper.MostrarMensajeError(_pageTitle, T(AppResources.Mensajes.ErrorAlGrabar));
            }
            finally
            {
                LayerOverlayService.Stop();
            }
        }

        #endregion


        #region Grid Leyendas

        ///<summary>
        /// Grid customize customize element event
        ///</summary>
        private void GridLeyendas_CustomizeElement(GridCustomizeElementEventArgs e)
        {
            if (e.ElementType == GridElementType.DataRow && e.VisibleIndex % 2 == 1)
            {
                e.CssClass = "alt-item";
            }
            if (e.ElementType == GridElementType.HeaderCell)
            {
                e.Style = "background-color: var(--DS-color-surface-neutral-default-selected)";
                e.CssClass = "header-bold";
            }
        }

        private async Task<bool> UsuarioTienePermisos()
        {
            if (!UsuarioEsAdmin)
            {
                await MensajesHelper.MostrarMensajeError(_pageTitle, T(AppResources.Mensajes.PermisosInsuficientes));
                return false;
            }
            return true;
        }

        #endregion

    }
}
