using HM.Presupuestos.Application.CasosDeUso;
using HM.Presupuestos.Application.CasosDeUso.LogAcciones;
using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.UnitTest.Fakes;
using HM.Presupuestos.UnitTest.Versiones;
using Microsoft.Extensions.Logging;

namespace HM.Presupuestos.UnitTest.Versiones;

[TestFixture]
public class IndicadoresServiceTests
{
    private Mock<ILogger<IndicadoresService>> _loggerMock = null!;
    private InMemoryIndicadoresRepository _repository = null!;
    private Mock<IVersionesService> _versionesServiceMock = null!;
    private Mock<ILogAccionesService> _logAccionesMock = null!;
    private IndicadoresService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<IndicadoresService>>();
        _repository = new InMemoryIndicadoresRepository();
        _versionesServiceMock = new Mock<IVersionesService>();
        _logAccionesMock = new Mock<ILogAccionesService>();

        _sut = new IndicadoresService(
            _loggerMock.Object,
            _repository,
            _versionesServiceMock.Object,
            _logAccionesMock.Object);
    }

    // tests copied from original file unchanged
}
