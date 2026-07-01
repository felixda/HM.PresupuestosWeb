using HM.Presupuestos.Application.CasosDeUso;

namespace HM.Presupuestos.UnitTest.Fakes;

internal sealed class InMemoryFavoritosService : IMenuFavoritosService
{
    private HashSet<string> _favoritos = new();

    public Task<HashSet<string>> ObtenerFavoritos()
    {
        return Task.FromResult(new HashSet<string>(_favoritos));
    }

    public Task GuardarFavoritos(HashSet<string> favoritos)
    {
        _favoritos = new HashSet<string>(favoritos);
        return Task.CompletedTask;
    }
}
