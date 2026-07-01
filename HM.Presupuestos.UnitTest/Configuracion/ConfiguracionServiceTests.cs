using HM.Presupuestos.Application.CasosDeUso;
using HM.Presupuestos.UnitTest.Fakes;
using HM.Presupuestos.Domain.Entidades;

namespace HM.Presupuestos.UnitTest.Configuracion;

[TestFixture]
public class ConfiguracionServiceTests
{
    private InMemoryConfiguracionRepository _repo = null!;
    private ConfiguracionService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new InMemoryConfiguracionRepository();
        _sut = new ConfiguracionService(Mock.Of<HM.Core.Comun.v6.Loggers.Interfaces.ILogger>(), _repo);
    }

    [Test]
    public async Task ObtenerAnioDiario_RetornaValor()
    {
        var result = await _sut.ObtenerAnioDiario();
        Assert.That(result.Codigo, Is.EqualTo(2026));
    }

    [Test]
    public async Task ActualizarAnioDiario_CambiaValor()
    {
        await _sut.ActualizarAnioDiario(2030);
        var result = await _repo.ObtenerAnioDiario();
        Assert.That(result.Codigo, Is.EqualTo(2030));
    }
}
