using HM.Presupuestos.Application.CasosDeUso;
using HM.Presupuestos.UnitTest.Fakes;

namespace HM.Presupuestos.UnitTest.Favoritos;

[TestFixture]
public class MenuFavoritosServiceTests
{
    private InMemoryFavoritosService _store = null!;

    [SetUp]
    public void SetUp()
    {
        _store = new InMemoryFavoritosService();
    }

    [Test]
    public async Task GuardarYObtenerFavoritos_Persiste()
    {
        var favoritos = new HashSet<string> { "A", "B" };
        await _store.GuardarFavoritos(favoritos);
        var res = await _store.ObtenerFavoritos();
        Assert.That(res, Is.EquivalentTo(favoritos));
    }
}
