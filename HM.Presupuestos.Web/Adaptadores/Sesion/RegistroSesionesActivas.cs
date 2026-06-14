using System.Collections.Concurrent;

namespace HM.Presupuestos.Web.Adaptadores.Sesion
{
    public record SesionActivaInfo(string Login, string PaginaActual, DateTime Inicio);

    public interface IRegistroSesionesActivas
    {
        void Registrar(string login, DateTime inicio);
        void ActualizarPagina(string login, string pagina);
        void Eliminar(string login);
        IReadOnlyList<SesionActivaInfo> ObtenerTodas();
    }

    public class RegistroSesionesActivas : IRegistroSesionesActivas
    {
        private readonly ConcurrentDictionary<string, SesionActivaInfo> _sesiones = new();

        public void Registrar(string login, DateTime inicio)
        {
            _sesiones[login] = new SesionActivaInfo(login, string.Empty, inicio);
        }

        public void ActualizarPagina(string login, string pagina)
        {
            _sesiones.AddOrUpdate(
                login,
                _ => new SesionActivaInfo(login, pagina, DateTime.UtcNow),
                (_, anterior) => anterior with { PaginaActual = pagina });
        }

        public void Eliminar(string login)
        {
            _sesiones.TryRemove(login, out _);
        }

        public IReadOnlyList<SesionActivaInfo> ObtenerTodas()
        {
            return _sesiones.Values.OrderBy(s => s.Inicio).ToList();
        }
    }
}
