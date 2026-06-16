using DevExpress.Blazor.Internal;
using HM.Core.Comun.v6.Seguridad.Interfaces;
using System.Text.Json;
using Version = HM.Presupuestos.Domain.Entidades.Version;

namespace HM.Presupuestos.Web.Pages.Mantenimientos
{
    public partial class Versiones
    {
        #region InyecciÃ³n de Dependencias

        [Inject] protected IIndicadoresService IndicadoresService { get; set; } = default!;

        #endregion

        #region Private Properties

        #region Page


        private bool HayCambios => JsonSerializer.Serialize(ListVersion) != JsonSerializer.Serialize(ListOriginVersion);

        #endregion

        #region Filter

        private CodigoDescripcion? ItemYearSelected { get; set; }

        #endregion

        #region Grid Versiones

        private IGrid? GridVersiones { get; set; }
        private List<CodigoDescripcion> ListTipoVersion { get; set; } = [];
        private List<CodigoDescripcion> ListMonth { get; set; } = [];
        private List<CodigoDescripcion> ListYear { get; set; } = [];
        private List<Version> ListOriginVersion { get; set; } = [];
        private List<Version> ListVersion { get; set; } = [];
        private List<Version> ListVersionCache { get; set; } = [];
        private List<Indicador> ListMasterIndicador { get; set; } = [];

        #endregion


        #region Grid Leyendas

        private IGrid? GridLeyendas { get; set; }

		#endregion

		#endregion

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
			ListYear = await VersionesService.ObtenerAniosConVersiones(true);

			if (ListYear.Any())
			{
				ItemYearSelected = ListYear[1];
			}
			string currentLanguageCode = GestorIdioma.IdiomaActual;
			var obLanguageList = LocalizadorRecursos.ObtenerIdiomas();
			ListTipoVersion = ObtenerTiposVersion();
			ListMonth = GestorIdioma.ObtenerMeses();
			ListMasterIndicador = await IndicadoresService.ObtenerIndicadoresConIdiomas();
			Idioma language = obLanguageList.First(c => c.Iso == currentLanguageCode);
			ListMasterIndicador.ForEach(c => c.CodigoIdioma = language.CodigoIdioma);

		}

        protected override async Task OnUsuarioImpersonadoDesconectado()
        {
            await base.OnUsuarioImpersonadoDesconectado();

            await InvokeAsync(StateHasChanged);
        }


        public List<CodigoDescripcion> ObtenerTiposVersion()
        {
            List<CodigoDescripcion> listTiposVersion = [];

            listTiposVersion.Add(new CodigoDescripcion
            {
                Codigo = Convert.ToInt32(ObtenerTexto(TextosApp.Pages.Versiones.TipoVersion.User_Code)),
                Descripcion = ObtenerTexto(TextosApp.Pages.Versiones.TipoVersion.User)
            });

            listTiposVersion.Add(new CodigoDescripcion
            {
                Codigo = Convert.ToInt32(ObtenerTexto(TextosApp.Pages.Versiones.TipoVersion.Monthly_Code)),
                Descripcion = ObtenerTexto(TextosApp.Pages.Versiones.TipoVersion.Monthly)
            });

            listTiposVersion.Add(new CodigoDescripcion
            {
                Codigo = Convert.ToInt32(ObtenerTexto(TextosApp.Pages.Versiones.TipoVersion.Backup_Code)),
                Descripcion = ObtenerTexto(TextosApp.Pages.Versiones.TipoVersion.Backup)
            });

            return listTiposVersion;
        }

        #endregion



        #region Filter


        ///<summary>
        /// Event param Source Year changed
        ///</summary>
        private async Task OnAnioSelectedDataItemChangedAsync(SelectedDataItemChangedEventArgs<CodigoDescripcion> e)
        {
            if (e.DataItem != null)
            {
                await EjecutarAsync(async () =>
                {
                    int anioSeleccionado = e.DataItem.Codigo;
                    ListVersion = await VersionesService.ObtenerVersiones(anioSeleccionado);

                    ListOriginVersion = new List<Version>();
                    if (ListVersion != null)
                    {
                        ListOriginVersion = DatosHelper.ClonarObjeto(ListVersion);
                    }
                });
            }
            else
            {
                ListVersion = [];
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
        private async void OnIndicadorCheckedChanged(bool? newValue, int? versionCodigo, int? indicadorCodigo)
        {
            if (newValue is null || indicadorCodigo is null) return;

            var itemVersion = ListVersion.Find(x => x.Codigo == versionCodigo);
            if (itemVersion == null) return;

            var itemIndicador = itemVersion.IndicadorList.Find(x => x.Codigo == indicadorCodigo);
            if (itemIndicador == null) return;

            await EjecutarAsync(async () =>
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

                            string descripcionIndicadorReal = "Real";
                            var indicadorReal = ListMasterIndicador.Find(c => c.Codigo == Constantes.CodigosIndicadores.REAL);
                            if (indicadorReal != null)
                            {
                                descripcionIndicadorReal = indicadorReal.DescripcionTraducida;
                            }

                            string descripcionIndicadorCerrada = "Cerrada";
                            var indicadorCerrada = ListMasterIndicador.Find(c => c.Codigo == Constantes.CodigosIndicadores.CERRADA);
                            if (indicadorCerrada != null)
                            {
                                descripcionIndicadorCerrada = indicadorCerrada.DescripcionTraducida;
                            }

                            await MensajesHelper.MostrarMensajeGeneral(TituloPagina, string.Format(ObtenerTexto(TextosApp.Mensajes.IndicadorCerradaConIndicadorReal), descripcionIndicadorReal, descripcionIndicadorCerrada), MessageBoxRenderStyle.Warning);
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

                await ActualizarEstadoCambios(HayCambios);
            }, showOverlay: false);
        }


        ///<summary>
        /// New Version item
        ///</summary>
        private async Task OnNuevaVersion()
        {
            if (!await UsuarioTienePermisos()) return;

            if (ItemYearSelected != null && ListVersion.Find(x => x.Codigo == 0) == null)
            {
                await EjecutarAsync(async () =>
                {
                    var nuevaVersion = new Version();
                    nuevaVersion.CodigoTipo = 1;  //Por defecto el tipo Usuario
                    nuevaVersion.Mes = 12;
                    nuevaVersion.Anio = ItemYearSelected.Codigo;
                    nuevaVersion.IndicadorList = [.. ListMasterIndicador.Select(m => new Version.VersionIndicador
                    {
                        Codigo = m.Codigo!.Value,
                        Estado = false
                    })];


                    int orden = 1;
                    if (ListVersion.Count > 0)
                    {
                        orden = ListVersion.Max(c => c.Orden) + 1;
                    }
                    nuevaVersion.Orden = orden;

                    ListVersion.Insert(0, nuevaVersion);
                    GridVersiones!.Reload(); //Porque si no desplaza la ultima fila y no se ve
                    await ActualizarEstadoCambios(true);
                });
            }
        }

        private void OnCancelarVersion()
        {
            ListVersion = DatosHelper.ClonarObjeto(ListOriginVersion);
            LimpiarCambiosPendientes();
        }

        ///<summary>
        /// Grid customize row event
        ///</summary>
        private void OnGridVersionesDataRowEditorCustomized(GridCustomizeDataRowEditorEventArgs e)
        {
            if (e.EditSettings is ITextEditSettings settings)
            {
                settings.ShowValidationIcon = true;
            }
        }


        ///<summary>
        /// Grid customize element event
        ///</summary>
        private async void OnGridVersionesElementCustomized(GridCustomizeElementEventArgs ea)
        {
            if (ea.ElementType == GridElementType.DataCell)
            {
                await EjecutarAsync(async () =>
                {
                    var column = (IGridDataColumn)ea.Column;
                    var version = (Version)GridVersiones!.GetDataItem(ea.VisibleIndex);
                    if (version != null)
                    {
                        var itemVersionOrigin = ListOriginVersion.Find(x => x.Codigo == version.Codigo);


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
                }, showOverlay: false);
            }
        }



        ///<summary>
        /// Grid customize edit model saving event
        ///</summary>
        private async void OnGridVersionesEditModelSaving(GridEditModelSavingEventArgs e)
        {
            var editVersion = (Version)e.EditModel;
            var indexVersion = ListVersion.FindIndex(x => x.Codigo == editVersion.Codigo);
            if (indexVersion != -1)
            {
                ListVersion[indexVersion] = editVersion;
                await ActualizarEstadoCambios(true);
            }
        }


        ///<summary>
        /// Delete Version from DB
        ///</summary>
        /// <param name="dataItem">object from grid</param>
        private async Task OnEliminarVersion(object dataItem)
        {
            if (!await UsuarioTienePermisos()) return;
            var version = (Version)dataItem;

            if (version.Codigo > 0)
            {
                if (!await MensajesHelper.MostrarMensajeParaConfirmacion(TituloPagina, ObtenerTexto(TextosApp.Mensajes.ConfirmacionEliminar)))
                {
                    return;
                }
                await EjecutarAsync(async () =>
                {
                    var result = await VersionesService.EliminarVersion(version);
                    if (result)
                    {
                        ListVersion = await VersionesService.ObtenerVersiones(ItemYearSelected!.Codigo);
                        ListOriginVersion = [];
                        if (ListVersion != null && ListVersion.Count > 0)
                        {
                            ListOriginVersion = DatosHelper.ClonarObjeto(ListVersion);
                        }
                        await MensajesHelper.MostrarMensajeExito(TituloPagina, ObtenerTexto(TextosApp.Mensajes.RegistroEliminado));
                    }
                    else
                    {
                        await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Mensajes.ErrorDelete));
                    }
                });
            }
            else
            {
                ListVersion.Remove(version);
                GridVersiones!.Reload();
            }
        }


        ///<summary>
        /// Validate version data list from grid
        ///</summary>
        private async Task<bool> ValidarDatos()
        {
            //Cuando es nueva la vesion, la descripcion no la valida automaticamente
            if (ListVersion.Any(o => o.Descripcion.Trim().Length == 0))
            {
                await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(TextosApp.Mensajes.CampoVersionObligatorio));
                return false;
            }

            //Validate text size
            if (ListVersion.Any(o => o.Descripcion.Length > 50))
            {
                await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(TextosApp.Mensajes.LongitudCaracteres) + "`" + ObtenerTexto(TextosApp.Common.Descripcion) + "`");
                return false;
            }

            //Validate unique indicator
            var listIndicadorUnique = ListMasterIndicador.Where(c => c.IndVersionUnica).ToList();
            Indicador? itemIndicadorUniqueDuplicated = null;
            foreach (var itemIndicadorUnique in listIndicadorUnique)
            {
                if (itemIndicadorUniqueDuplicated != null)
                    break;
                var itemIndicadorUniqueCounter = 0;
                foreach (var itemVersion in ListVersion)
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
                await MensajesHelper.MostrarMensajeGeneral(TituloPagina, string.Format(ObtenerTexto(TextosApp.Mensajes.IndicadorUnico), itemIndicadorUniqueDuplicated.DescripcionTraducida), MessageBoxRenderStyle.Warning);
                return false;
            }

            //Validate description duplicated
            Version? itemVersionDescriptionDuplicated = null;
            foreach (var itemVersion in ListVersion)
            {
                if (ListVersion.Find(x => x.Descripcion == itemVersion.Descripcion && x.Codigo != itemVersion.Codigo) != null)
                {
                    itemVersionDescriptionDuplicated = itemVersion;
                    break;
                }
            }
            if (itemVersionDescriptionDuplicated != null)
            {
                await MensajesHelper.MostrarMensajeGeneral(TituloPagina, string.Format(ObtenerTexto(TextosApp.Pages.Versiones.Message_VersionDescriptionError), itemVersionDescriptionDuplicated.Descripcion), MessageBoxRenderStyle.Warning);
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
                    var itemMasterIndicador = ListMasterIndicador.Find(x => x.Codigo == versionIndicador.Codigo);
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
        private async Task GuardarDatosVersion()
        {
            if (!await UsuarioTienePermisos()) return;

            await EjecutarAsync(async () =>
            {
                if (await ValidarDatos())
                {

                    var newVersionList = new List<Version>();
                    var updateVersionList = new List<Version>();
                    foreach (var itemVersion in ListVersion)
                    {
                        if (itemVersion.Codigo == 0)
                        {
                            itemVersion.IndEstado = CalculateSummaryIndicadorVersion(itemVersion);
                            newVersionList.Add(itemVersion);
                        }
                        else
                        {
                            var itemOriginVersion = ListOriginVersion.FirstOrDefault(x => x.Codigo == itemVersion.Codigo);
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
                        var result = await VersionesService.GrabarVersiones(newVersionList, updateVersionList, SesionUsuario.UsuarioApp!.UsuarioActivo!.CodigoPais);

                        if (result)
                        {
                            ListOriginVersion = new List<Version>();
                            if (ListVersion != null && ListVersion.Count > 0)
                                ListOriginVersion = DatosHelper.ClonarObjeto(ListVersion);

                            await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(TextosApp.Common.DatosGrabados));
                            await ActualizarEstadoCambios(false);
                            LimpiarCambiosPendientes();
                        }
                        else
                        {
                            await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Mensajes.ErrorAlGrabar));
                        }
                    }
                    else
                    {
                        await MensajesHelper.MostrarMensajeInfo(TituloPagina, ObtenerTexto(TextosApp.Mensajes.SinModificaciones));
                    }
                }
            });
        }

        #endregion


        #region Grid Leyendas

        ///<summary>
        /// Grid customize customize element event
        ///</summary>
        private void OnGridLeyendasElementCustomized(GridCustomizeElementEventArgs e)
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
                await MensajesHelper.MostrarMensajeError(TituloPagina, ObtenerTexto(TextosApp.Mensajes.PermisosInsuficientes));
                return false;
            }
            return true;
        }

        #endregion

    }
}

