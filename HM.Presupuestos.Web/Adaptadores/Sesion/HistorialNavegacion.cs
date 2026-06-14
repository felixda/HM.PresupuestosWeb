using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;

namespace HM.Presupuestos.Web.Adaptadores.Sesion
{
    public record EntradaHistorial(string Pagina, DateTime Hora);

    public interface IHistorialNavegacion
    {
        void RegistrarVisita(string login, string pagina);
        IReadOnlyList<EntradaHistorial> ObtenerHistorial(string login);
    }

    public class HistorialNavegacion : IHistorialNavegacion
    {
        private readonly ConcurrentDictionary<string, Queue<EntradaHistorial>> _historial = new();
        private readonly int _maxEntradas;

        public HistorialNavegacion(IConfiguration configuration)
        {
            _maxEntradas = int.TryParse(configuration["HistorialNavegacion:MaxEntradas"], out var valor) && valor > 0
                ? valor
                : 50;
        }

        public void RegistrarVisita(string login, string pagina)
        {
            var cola = _historial.GetOrAdd(login, _ => new Queue<EntradaHistorial>());

            lock (cola)
            {
                if (cola.Count >= _maxEntradas)
                    cola.Dequeue();

                cola.Enqueue(new EntradaHistorial(pagina, DateTime.UtcNow));
            }
        }

        public IReadOnlyList<EntradaHistorial> ObtenerHistorial(string login)
        {
            if (!_historial.TryGetValue(login, out var cola))
                return [];

            lock (cola)
                return cola.ToList();
        }
    }
}
