using HM.Presupuestos.Server.Extensiones;
using HM.Presupuestos.Server.Modelos;
using HM.Presupuestos.Server.Servicios;

namespace HM.Presupuestos.Server.Services
{

    public interface IPermisosService
    {
        void EstablecerMenus(UsuarioApp usuarioApp);
        bool TienePermiso(string url);

    }


    public class PermisosService : IPermisosService
    {
        private HashSet<string> _urlsPermitidas = [];
        private bool _inicializado = false;

        private readonly SemaphoreSlim _initSemaphore = new(initialCount: 1, maxCount: 1);

        private readonly IResourceService _ResourceService;
        private readonly IUsuarioServicio _UsuarioService;

        public PermisosService(IResourceService resourceService, IUsuarioServicio usuarioService)
        {
            _ResourceService = resourceService;
            _UsuarioService = usuarioService;

            // Suscribirse al evento de usuario cargado para inicializar permisos
            _UsuarioService.OnUsuarioCargado += InicializarAsync;
        }


        /// <summary>
        /// Inicializa los permisos si aún no se han cargado
        /// </summary>
        public async Task InicializarAsync()
        {
            // 🔒 Intenta "entrar" (espera si ya hay alguien)
            await _initSemaphore.WaitAsync();

            try
            {
                // ✅ Solo UNA tarea puede ejecutar esto a la vez
                if (_inicializado)
                {
                    return;
                }

                if (_UsuarioService.UsuarioApp == null)
                {
                    return;
                }

                EstablecerMenus(_UsuarioService.UsuarioApp);
                _inicializado = true;
            }
            finally
            {
                _initSemaphore.Release();
            }
        }


        public void EstablecerMenus(UsuarioApp usuarioApp)
        {
            if (_urlsPermitidas.Count == 0)
            {
                // ✅ Filtrar solo menús con IdPadre != null (menús hijos que tienen URL)
                var urls = usuarioApp.Usuario.Menus
                    .Where(m => m.IdPadre != null)
                    .Select(m => m.Url(_ResourceService))
                    .Where(url => !string.IsNullOrEmpty(url))
                    .ToList();

                _urlsPermitidas = new HashSet<string>(
                   urls.Select(u => u!.ToLower()),
                   StringComparer.OrdinalIgnoreCase
               );
            }
        }

        public bool TienePermiso(string url)
        {
            EstablecerMenus(_UsuarioService.UsuarioApp!);

            return _urlsPermitidas.Contains(url.ToLower());
        }
    }
}


//┌─────────────────────────────────────────────────┐
//│              SemaphoreSlim(1, 1)                │
//│                                                 │
//│  Tarea A llama InicializarAsync()               │
//│  ├─ WaitAsync() → ✅ ENTRA (semáforo: 1 → 0)    │
//│  ├─ Ejecuta inicialización...                   │
//│  │                                              │
//│  │  Tarea B llama InicializarAsync()            │
//│  │  ├─ WaitAsync() → ⏳ ESPERA (semáforo: 0)   │
//│  │  │                                          │
//│  │  │  Tarea C llama InicializarAsync()        │
//│  │  │  ├─ WaitAsync() → ⏳ ESPERA (semáforo: 0)│
//│  │  │  │                                        │
//│  ├─ Release() → 🔓 SALE(semáforo: 0 → 1)       │
//│  │                                             │
//│  │  ├─ Tarea B → ✅ ENTRA (semáforo: 1 → 0)    │
//│  │  ├─ Ve _inicializado = true                 │
//│  │  ├─ Release() → 🔓 SALE (semáforo: 0 → 1)   │
//│  │                                              │
//│  │  │  ├─ Tarea C → ✅ ENTRA (semáforo: 1 → 0) │
//│  │  │  ├─ Ve _inicializado = true              │
//│  │  │  ├─ Release() → 🔓 SALE                  │
//└─────────────────────────────────────────────────┘