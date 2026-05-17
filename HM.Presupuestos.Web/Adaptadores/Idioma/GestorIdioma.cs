
using HM.Presupuestos.Domain.Entidades;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace HM.Presupuestos.Web.Adaptadores.Idioma
{
    public interface IGestorIdioma
    {
        event Func<Task>? IdiomaCambiado;

        Task CambiarIdioma(string nuevoIdioma);

        string IdiomaActual { get; }
    }


    public class GestorIdioma : IGestorIdioma
    {
        #region Propiedades privadas

        private const string IDIOMA_COOKIE_KEY = "app_idioma";
        private const int IDIOMA_COOKIE_EXPIRE_DAYS = 365;
        private const string IDIOMA_POR_DEFECTO = "es";

        private readonly IConfiguration _configuracion;
        private readonly IGestorCookies _gestorCookies;

        #endregion

        public event Func<Task>? IdiomaCambiado;

        public string IdiomaActual { get; private set; }

        #region Constructor
        /// <summary>
        /// Constructor que inicializa el idioma desde la cookie
        /// </summary>
        public GestorIdioma(IConfiguration configuracion, IGestorCookies gestorCookies)
        {
            _configuracion = configuracion;
            _gestorCookies = gestorCookies;

            // ? Inicializar idioma desde cookie al crear el servicio
            var idiomaEnCookie = _gestorCookies.Obtener(IDIOMA_COOKIE_KEY);

            if (idiomaEnCookie  == null)
            {
                IdiomaActual = _configuracion.GetValue<string>("AppSettings:DefaultLanguage") ?? IDIOMA_POR_DEFECTO;
                _gestorCookies.Grabar(IDIOMA_COOKIE_KEY, IdiomaActual, IDIOMA_COOKIE_EXPIRE_DAYS);
            }
            else
            {
                IdiomaActual = idiomaEnCookie;
            }

            Console.WriteLine($"[IdiomaService] ? Servicio creado con idioma: {IdiomaActual}");
        }
        #endregion


        #region Métodos

        /// <summary>
        /// Cambia el idioma actual y notifica a los suscriptores
        /// </summary>
        /// <param name="nuevoIdioma">Código ISO del nuevo idioma (es, en, pt)</param>
        public async Task CambiarIdioma(string nuevoIdioma)
        {
            if (IdiomaActual != nuevoIdioma)
            {
                await _gestorCookies.GrabarAsync(IDIOMA_COOKIE_KEY, nuevoIdioma, IDIOMA_COOKIE_EXPIRE_DAYS);
                IdiomaActual = nuevoIdioma;
                Console.WriteLine($"[IdiomaService] ?? Idioma cambiado a: {nuevoIdioma}");

                if (IdiomaCambiado != null)
                {
                    await IdiomaCambiado.Invoke();
                }
            }
        }

        #endregion
    }
}



