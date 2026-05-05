using HM.Core.Comun.v6.Entidades.Seguridad;
using HM.Core.Comun.v6.Seguridad.Interfaces;
using HM.Core.Modelo.v6.Login;
using HM.Presupuestos.Application.Servicios;
using HM.Presupuestos.Infrastructure;
using HM.Presupuestos.Domain.Comun;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Helper;
using HM.Presupuestos.Server.Helper;
using Microsoft.AspNetCore.Components.Authorization;
using System.Text.Json;

namespace HM.Presupuestos.Server.Services
{
    public interface IUserService
    {
        UsuarioEntidad? Usuario { get; set; }
        Task<UsuarioEntidad> ObtenerUsuario();
        Task EstablecerUsuario(UsuarioEntidad usuario);
       // Task LimpiarUsuario();
       
    }

    public class UserService : IUserService
    {
        private UsuarioEntidad? _usuario;
        private readonly ISessionService _sessionService;
        private readonly IControlador _controlador;
        private readonly IJwt _jwt;
        private readonly IConfiguration _configuration;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILogService _logService;
        private readonly ILogAccionesService _logAccionesService;
        private readonly ICookieService _cookieService;
        private readonly ILogger<UserService> _logger;

        private bool _isInitializing = false;

        public UserService(
            ISessionService sessionService,
            IControlador controlador,
            IJwt jwt,
            IConfiguration configuration,
            AuthenticationStateProvider authStateProvider,
            ILogService logService,
            ILogAccionesService logAccionesService,
            ICookieService cookieService,
            ILogger<UserService> logger)
        {
            _sessionService = sessionService;
            _controlador = controlador;
            _jwt = jwt;
            _configuration = configuration;
            _authStateProvider = authStateProvider;
            _logService = logService;
            _logAccionesService = logAccionesService;
            _cookieService = cookieService;
            _logger = logger;
        }

        public UsuarioEntidad? Usuario 
        { 
            get => _usuario;
            set => _usuario = value;
        }

        public async Task<UsuarioEntidad> ObtenerUsuario()
        {
            if (_usuario != null && !string.IsNullOrEmpty(_usuario.Login))
            {
                _logger.LogDebug("Usuario obtenido desde memoria: {Login}", _usuario.Login);
                return _usuario;
            }

            // Evitar múltiples inicializaciones concurrentes
            if (_isInitializing)
            {
                Console.WriteLine("[UserService] ? Inicialización en progreso, esperando...");
                // Esperar a que termine la inicialización en curso
                var maxWait = 50; // 5 segundos máximo
                for (int i = 0; i < maxWait && _isInitializing; i++)
                {
                    await Task.Delay(100);
                }

                // Si ya terminó, devolver usuario
                if (_usuario != null && !string.IsNullOrEmpty(_usuario.Login))
                {
                    return _usuario;
                }
            }

       
            _isInitializing = true;
            try
            {
                // Obtener desde servicio externo
                _logger.LogInformation("Obteniendo desde servicio externo...");
               // _usuario = await CargarUsuarioDesdeServicioExterno(true);

                if (_usuario != null && !string.IsNullOrEmpty(_usuario.Login))
                {
                    _logger.LogInformation("? Usuario cargado desde servicio externo: {Login}", _usuario.Login);
                    return _usuario;
                }

                _logger.LogWarning("?? No se pudo cargar el usuario");
                return new UsuarioEntidad();
            }
            finally
            {
                _isInitializing = false;
            }

        }

        public async Task EstablecerUsuario(UsuarioEntidad usuario)
        {
            _usuario = usuario;

        }


        
    }
}