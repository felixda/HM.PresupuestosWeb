using HM.Presupuestos.Application.CasosDeUso.Compartido;
using HM.Presupuestos.UnitTest.Fakes;
using HM.Presupuestos.Domain.Entidades;

namespace HM.Presupuestos.UnitTest.Compartido;

[TestFixture]
public class MaestrosServiceTests
{
    private InMemoryPresupuestosRepository _repo = null!;
    private MaestrosService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new InMemoryPresupuestosRepository();
        _sut = new MaestrosService(Mock.Of<HM.Core.Comun.v6.Loggers.Interfaces.ILogger>(), _repo);
    }

    [Test]
    public async Task ObtenerTipologias_RetornaDatosSembrados()
    {
        // Arrange
        _repo.SeedTipologia(new Builders.CodigoDescripcionBuilder().WithCodigo(10).WithDescripcion("T1").Build());

        // Act
        var result = await _sut.ObtenerTipologias();

        // Assert
        Assert.That(result.Select(r => r.Codigo), Is.EquivalentTo(new[] { 10 }));
    }
}
