using HM.Core.Comun.v6.Loggers.Interfaces;
using HM.Presupuestos.Application.CasosDeUso;
using HM.Presupuestos.Domain.Puertos;
using HM.Presupuestos.UnitTest.Fakes;

namespace HM.Presupuestos.UnitTest.Admin;

[TestFixture]
public class AdminServiceTests
{
    private Mock<ILogger> _loggerMock = null!;
    private InMemoryAdminRepository _repo = null!;
    private AdminService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger>();
        _repo = new InMemoryAdminRepository();
        _sut = new AdminService(_loggerMock.Object, _repo);
    }

    [Test]
    public async Task ObtenerMesesBloqueados_RetornaListado()
    {
        // Arrange
        await _repo.InsertarMesBloqueado(2026, 1);
        await _repo.InsertarMesBloqueado(2026, 2);

        // Act
        var result = await _sut.ObtenerMesesBloqueados(2026);

        // Assert
        Assert.That(result, Is.EquivalentTo(new[] { 1, 2 }));
    }

    [Test]
    public async Task InsertarMesesBloqueado_ReemplazaYHaceCommit()
    {
        // Arrange
        await _repo.InsertarMesBloqueado(2026, 99); // inicial
        var meses = new List<int> { 3, 4 };

        // Act
        await _sut.InsertarMesesBloqueado(2026, meses);

        // Assert
        Assert.That(_repo.All(), Is.EquivalentTo(meses));
        Assert.That(_repo.Transaccion.CommitInvocado, Is.True);
    }
}
