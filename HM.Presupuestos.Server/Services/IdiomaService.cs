
using HM.Presupuestos.Contratos.Entidades;
using HM.Presupuestos.Server.Modelos;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace HM.Presupuestos.Server.Services
{
    public interface IIdiomaService
    {
        event Func<Task>? OnIdiomaCambiado;

        Task CambiarIdioma(string nuevoIdioma);

        string Idioma { get; }
    }


    public class IdiomaService : IIdiomaService
    {
        #region Propiedades privadas

        private const string IDIOMA_COOKIE_KEY = "app_idioma";
        private const int IDIOMA_COOKIE_EXPIRE_DAYS = 365;
        private const string IDIOMA_POR_DEFECTO = "es";

        private readonly IConfiguration _config;
        private readonly ICookieService _cookieService;

        #endregion

        public event Func<Task>? OnIdiomaCambiado;

        public string Idioma { get; private set; }

        #region Constructor
        /// <summary>
        /// Constructor que inicializa el idioma desde la cookie
        /// </summary>
        public IdiomaService(IConfiguration config, ICookieService cookieservice)
        {
            _config = config;
            _cookieService = cookieservice;

            // ✅ Inicializar idioma desde cookie al crear el servicio
            var idiomaEncookie = _cookieService.GetCookie(IDIOMA_COOKIE_KEY);

            if (idiomaEncookie == null)
            {
                Idioma = _config.GetValue<string>("AppSettings:DefaultLanguage") ?? IDIOMA_POR_DEFECTO;
                _cookieService.SetCookie(IDIOMA_COOKIE_KEY, Idioma, IDIOMA_COOKIE_EXPIRE_DAYS);
            }
            else
            {
                Idioma = idiomaEncookie;
            }

            Console.WriteLine($"[IdiomaService] ✅ Servicio creado con idioma: {Idioma}");
        }
        #endregion


        #region Métodos

        /// <summary>
        /// Cambia el idioma actual y notifica a los suscriptores
        /// </summary>
        /// <param name="nuevoIdioma">Código ISO del nuevo idioma (es, en, pt)</param>
        public async Task CambiarIdioma(string nuevoIdioma)
        {
            if (Idioma != nuevoIdioma)
            {
                await _cookieService.SetCookieAsync(IDIOMA_COOKIE_KEY, nuevoIdioma, IDIOMA_COOKIE_EXPIRE_DAYS);
                Idioma = nuevoIdioma;
                Console.WriteLine($"[IdiomaService] 🔄 Idioma cambiado a: {nuevoIdioma}");

                if (OnIdiomaCambiado != null)
                {
                    await OnIdiomaCambiado.Invoke();
                }
            }
        }

        #endregion
    }
}
